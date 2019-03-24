using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Krillin
{
    public partial class SendHtml : Form, ISendHtml
    {
        public SendHtml()
        {
            InitializeComponent();
        }
        private string BrowseFile(string fileFilter)
        {
            string fileName = string.Empty;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = fileFilter;
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
            }
            return fileName;
        }
        private void btnAttch_Click(object sender, EventArgs e)
        {
            lblAttch.Text = BrowseFile("Doc files (*.docx)|*.docx|Pdf files (*.pdf)|*.pdf");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to send broadcast email", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            presenter.Send();
        }

        public string Cc
        {
            get { return txtCc.Text; }
        }

        public string Bcc
        {
            get { return txtBcc.Text; }
        }

        public string Subject
        {
            get { return txtSubject.Text; }
        }

        public string Embd
        {
            get
            {
                string v_Content = string.Empty;
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open File";
                    dlg.Filter = "All files (*.html)|*.html";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        v_Content = File.ReadAllText(dlg.FileName);                                                
                    }

                }
                return v_Content;
            }
        }

        public string Attch
        {
            get { return lblAttch.Text; }
        }

        public string ErrMsg
        {
            set { MessageBox.Show(value); }
        }
        SendHtmlPresenter presenter;
        private void SendEmail_Load(object sender, EventArgs e)
        {
            presenter = new SendHtmlPresenter(this);

        }

        public string To
        {
            get { return txtTo.Text; }
        }

        private void btnTmpl_Click(object sender, EventArgs e)
        {

        }


        public string ExType
        {
            get { return txtExType.Text; }
        }
    }
}
