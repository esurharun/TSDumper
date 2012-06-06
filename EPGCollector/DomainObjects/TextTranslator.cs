////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2011 nzsjb                                           //
//                                                                              //
//  This Program is free software; you can redistribute it and/or modify        //
//  it under the terms of the GNU General Public License as published by        //
//  the Free Software Foundation; either version 2, or (at your option)         //
//  any later version.                                                          //
//                                                                              //
//  This Program is distributed in the hope that it will be useful,             //
//  but WITHOUT ANY WARRANTY; without even the implied warranty of              //
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                //
//  GNU General Public License for more details.                                //
//                                                                              //
//  You should have received a copy of the GNU General Public License           //
//  along with GNU Make; see the file COPYING.  If not, write to                //
//  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.       //
//  http://www.gnu.org/copyleft/gpl.html                                        //
//                                                                              //  
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace DomainObjects
{
    /// <summary>
    /// The class that handles foreign language translations.
    /// </summary>
    public class TextTranslator
    {
        /// <summary>
        /// Translate a text string.
        /// </summary>
        /// <param name="inputLanguage">The language code of the string.</param>
        /// <param name="outputLanguage">The language code of the translated string.</param>
        /// <param name="text">The text to be translated.</param>
        /// <returns>The translated text.</returns>
        public static string GetTranslatedText(string inputLanguage, string outputLanguage, string text)
        {
            string actualInputLanguage = string.Empty;

            if (inputLanguage != null)
                actualInputLanguage = LanguageCode.FindLanguageCode(inputLanguage).TranslationCode;
            else
                actualInputLanguage = "en";

            if (actualInputLanguage == null || actualInputLanguage == string.Empty)
                return (text);

            string actualOutputLanguage = string.Empty;

            if (outputLanguage != null)
                actualOutputLanguage = LanguageCode.FindLanguageCode(outputLanguage).TranslationCode;
            else
                actualOutputLanguage = "en";

            if (actualOutputLanguage == null || actualOutputLanguage == string.Empty)
                return (text);

            if (outputLanguage == null || inputLanguage == outputLanguage)
                return (text);

            /*string appId = "7212DAA1C62A41C5DC706D0B2F6EC913B85E51BE";
            string buffer = sendWebRequest("http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" +
                appId + "&text=" + text + "&from=" + actualInputLanguage + "&to=" + actualOutputLanguage);*/

            string userString = string.Empty;
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            if (addresses.Length != 0)
                userString = "&userip=" + addresses[0].ToString();

            string appId = "AIzaSyCvxyXBtSIzUYtdm1RCziI5-PzESh7yVpk";
            string buffer = sendWebRequest("https://ajax.googleapis.com/ajax/services/language/translate?v=1.0" +
                "&q=" + text.Replace(" ", "%20") +
                "&langpair=" + actualInputLanguage + 
                "%7C" + actualOutputLanguage + 
                "&key=" + appId + 
                userString);

            if (buffer == null)
                return (text);            

            int index1 = buffer.IndexOf(@"translatedText");
            if (index1 == -1)
                return (buffer);

            index1 += 17;

            int index2 = buffer.IndexOf('"', index1);
            if (index2 == -1)
                return (buffer);

            return (buffer.Substring(index1, index2 - index1));
        }

        private static string sendWebRequest(string httpRequest)
        {
            WebRequest webRequest = WebRequest.Create(httpRequest);
            webRequest.ContentType = "text/html";
            webRequest.Timeout = 20000;
            ((HttpWebRequest)webRequest).UserAgent = "EPGCOLLECTOR";

            HttpWebResponse webResponse = null;

            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                string message = e.Message;

                if (webResponse != null)
                    webResponse.Close();
                return (null);
            }

            Stream receiveStream = webResponse.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");

            StreamReader readStream = new StreamReader(receiveStream, encode);
            string buffer = readStream.ReadToEnd();

            readStream.Close();
            webResponse.Close();

            return (buffer);
        }
    }
}
