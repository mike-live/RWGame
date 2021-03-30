using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RWGame.Helpers
{
    public enum InfoStringsEnum
    {
        [Description("")]
        NONE = 0,
        [Description("Make first turn!")]
        FIRST_TURN = 1,
        [Description("Wait...")]
        WAIT = 2,
        [Description("Make turn!")]
        TURN = 3,
        [Description("Moves History")]
        END = 4
    }
    public static class InfoString
    {
        public static string ToDescriptionString(this InfoStringsEnum val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
