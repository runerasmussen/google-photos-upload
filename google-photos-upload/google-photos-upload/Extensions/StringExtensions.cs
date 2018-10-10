using System;
using System.Collections.Generic;
using System.Text;

namespace google_photos_upload.Extensions
{
    /// <summary>
    /// Extension methods for System.String
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Change Encoding from Unicode to ASCII
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UnicodeToASCII(this string input)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            return ChangeEncoding(input, unicode, ascii);
        }


        /// <summary>
        /// Changes Encoding on a string
        /// </summary>
        /// <param name="input">string to change Encoding on</param>
        /// <param name="fromEncoding">Encoding of input string</param>
        /// <param name="toEncoding">Encoding to convert to</param>
        /// <returns>string in new Encoding</returns>
        public static string ChangeEncoding(this string input, Encoding fromEncoding, Encoding toEncoding)
        {
            // Convert the string into a byte[].
            byte[] strBytes = fromEncoding.GetBytes(input);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(fromEncoding, toEncoding, strBytes);

            // Convert the new byte[] into a char[] and then into a string.
            // This is a slightly different approach to converting to illustrate
            // the use of GetCharCount/GetChars.
            char[] asciiChars = new char[toEncoding.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            toEncoding.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            return asciiString;
        }


        /// <summary>
        /// UrlEncode of string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlEncode(this string input)
        {
            return System.Net.WebUtility.UrlEncode(input);
        }


        /// <summary>
        /// Convert Default Encoding to UTF8 Encoded string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUTF8(this string input)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(input);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
