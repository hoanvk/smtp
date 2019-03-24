using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using log4net;
using System.Net.Mime;
using Oracle.DataAccess.Client;
namespace Krillin
{
    public class EmailUtil
    {
        private static readonly ILog log = LogManager.GetLogger("Krillin");
        
        IList<ClntEmailModel> GetListModel(string to, string extype)
        {
            IList<ClntEmailModel> model = new List<ClntEmailModel>();
            if (to == string.Empty)
            {
                OracleConnection m_OraConn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["APPABS"].ConnectionString);
                try
                {
                    m_OraConn.Open();
                    OracleCommand v_OraCmd = new OracleCommand();
                    v_OraCmd.Connection = m_OraConn;

                    OracleDataAdapter v_OraDa = new OracleDataAdapter();
                    v_OraDa.SelectCommand = v_OraCmd;
                    DataSet entities = new DataSet();

                    v_OraCmd.CommandType = CommandType.StoredProcedure;
                    v_OraCmd.CommandText = "sp_exeml_search";
                    v_OraCmd.Parameters.Add("p_extype", OracleDbType.Varchar2).Value = extype;
                    v_OraCmd.Parameters.Add("p_rows", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    v_OraDa.Fill(entities);
                    foreach (DataRow v_Row in entities.Tables[0].Rows)
                    {
                        ClntEmailModel item = new ClntEmailModel();
                        item.ClntCode = v_Row.Field<string>("custodycd");
                        item.ClntName = v_Row.Field<string>("fullname");
                        item.ClntEmail = v_Row.Field<string>("email");                        
                        model.Add(item);
                        
                    }

                }
                catch (Exception ex)
                { log.Error(ex.Message); }
                finally
                {
                    if (m_OraConn.State != ConnectionState.Closed)
                        m_OraConn.Close();
                }
            }
            else
            {
                ClntEmailModel item = new ClntEmailModel();
                item.ClntCode = "018CXXXXXX";
                item.ClntName = "Cong ty CP CK An Binh";
                item.ClntEmail = to;
                model.Add(item);
            }
            return model;
        }
        public string EmailHtml(string to, string cc, string bcc, string subject, string body, string attch, string extype)
        {
            int success = 0;

            IList<ClntEmailModel> model = GetListModel(to, extype);                
                RegexUtilities util = new RegexUtilities();
                foreach (ClntEmailModel item in model)
                {
                    string recepient = item.ClntEmail;
                    if (!util.IsValidEmail(recepient))
                    {
                        continue;
                    }
                    try
                    {

                        MailMessage mMailMessage = new MailMessage();
                        string from = ConfigurationManager.AppSettings["from"];
                        mMailMessage.From = new MailAddress(from);
                        // Set the sender address of the mail message
                        //mMailMessage.From = new MailAddress(from);                
                        // Set the recepient address of the mail message
                        mMailMessage.To.Add(new MailAddress(recepient.ToString()));
                        // Check if the bcc value is null or an empty string
                        if ((bcc != null) && (bcc != string.Empty))
                        {
                            // Set the Bcc address of the mail message
                            mMailMessage.Bcc.Add(new MailAddress(bcc));
                        }

                        // Check if the cc value is null or an empty value
                        if ((cc != null) && (cc != string.Empty))
                        {
                            // Set the CC address of the mail message
                            mMailMessage.CC.Add(new MailAddress(cc));
                        }       // Set the subject of the mail message
                        mMailMessage.Subject = subject;                        

                        mMailMessage.Body = body;
                        mMailMessage.IsBodyHtml = true;
                        // Set the priority of the mail message to normal
                        mMailMessage.Priority = MailPriority.Normal;
                        //Attachment            
                        if (attch != string.Empty)
                        {
                            if (File.Exists(attch))
                            {
                                System.IO.Stream stream = File.OpenRead(attch);
                                Attachment attach = new Attachment(stream, string.Empty);
                                attach.Name = attch;
                                mMailMessage.Attachments.Add(attach);
                            }
                        }
                        //NetworkCredential
                        string user_name = ConfigurationManager.AppSettings["username"];
                        string password = ConfigurationManager.AppSettings["password"];
                        NetworkCredential loginInfo = new NetworkCredential(user_name, password);
                        // Instantiate a new instance of SmtpClient
                        string host = ConfigurationManager.AppSettings["host"];
                        int port = int.Parse(ConfigurationManager.AppSettings["port"]);
                        SmtpClient mSmtpClient = new SmtpClient(host, port);
                        mSmtpClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["ssl"]);
                        mSmtpClient.UseDefaultCredentials = false;
                        mSmtpClient.Credentials = loginInfo;
                        // Send the mail message              
                        mSmtpClient.Send(mMailMessage);
                        log.Info(string.Format("ClntCode: {0} ClntName {1} ClntEmail {2}", item.ClntCode, item.ClntName, item.ClntEmail));
                        success++;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }                       
            return string.Format("Sent {0} successful!", success);
        }
        
        public string ImageBase64(string embd)
        {
            string imgBase64 = string.Empty;
            if (embd != string.Empty)
            {
                if (File.Exists(embd))
                {
                    Stream myStream = File.OpenRead(embd);
                    using (myStream)
                    {
                        byte[] byteArray = new byte[myStream.Length];
                        myStream.Read(byteArray, 0, byteArray.Length);
                        // Insert code to read the stream here.
                        imgBase64 = Convert.ToBase64String(byteArray);
                    }
                }
            }
            return imgBase64;
        }
    }
}
