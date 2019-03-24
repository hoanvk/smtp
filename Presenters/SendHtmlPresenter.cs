using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krillin
{
    public class SendHtmlPresenter
    {
        ISendHtml view;
        public SendHtmlPresenter(ISendHtml _view)
        { view = _view; }
        public void Send()
        {
            EmailUtil util = new EmailUtil();
            view.ErrMsg = util.EmailHtml(view.To, view.Cc, view.Bcc, view.Subject, view.Embd, view.Attch, view.ExType);
        }
    }
}
