using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RWGame.Helpers
{
    public enum Emojis
    {   
        [Description("")]
        none = 0,
        [Description("\U0001F947")]
        medalFirstPlace = 1,
        [Description("\U0001F948")]
        medalSecondPlace = 2,
        [Description("\U0001F949")]
        medalThirdPlace = 3
    }
    public static class Emoji
    {
        public static string ToDescriptionString(this Emojis val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
