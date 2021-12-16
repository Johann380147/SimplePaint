using System;

namespace ExternalForms
{
    public static class StringExtensions
    {
        public static float ToSingle(this string text)
        {
            float number = 0;
            bool isSingle = Single.TryParse(text, out number);
            if(isSingle)
            {
                return number;   
            }
            else
            {
                return -999;
            }
        }
    }
}
