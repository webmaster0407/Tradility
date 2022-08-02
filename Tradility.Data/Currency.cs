using System;

namespace Tradility.Data
{
    [Flags]
    public enum Currency
    {
        //Main = 1,
        USD = 1,
        EUR = 2,
        GBP = 4,
        CHF = 8
    }
}
