using System;
using System.Collections.Generic;
using System.Text;

namespace AjaxLibrary
{
    [Flags]
    public enum JsonOptions
    {
        None = 0x0000,
        QuoteNames = 0x0001,
        EnclosingParens = 0x0002,
        IncludeNulls = 0x0004,
        Formatted = 0x0008
    }
}
