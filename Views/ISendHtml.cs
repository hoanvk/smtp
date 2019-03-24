using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krillin
{
    public interface ISendHtml
    {
        string To { get; }
        string Cc { get; } 
        string Bcc { get;}
        string Subject { get; }        
        string Embd { get; }
        string Attch { get;}
        string ErrMsg { set; }
        string ExType {get;}
    }
}
