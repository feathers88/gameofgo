using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoG.WinRT.ViewModels.IgsProtocol
{
    public struct Command
    {
        internal Command(Codes code, SubCodes? subcode, string text)
        {
            Code = code;
            SubCode = subcode;
            Text = text;
        }

        public Codes Code;
        public SubCodes? SubCode;
        public string Text;
    }
}
