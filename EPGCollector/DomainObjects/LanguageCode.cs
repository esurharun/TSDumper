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

using System.Collections.ObjectModel;
using System.Xml;
using System.IO;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a language code.
    /// </summary>
    public class LanguageCode
    {
        /// <summary>
        /// The usage of the code.
        /// </summary>
        public enum Usage
        {
            /// <summary>
            /// The language code is used for input.
            /// </summary>
            Input,
            /// <summary>
            /// The language code is used for output.
            /// </summary>
            Output
        }

        /// <summary>
        /// Get the collection of language codes.
        /// </summary>
        public static Collection<LanguageCode> LanguageCodes 
        { 
            get 
            {
                if (languageCodes == null)
                    Load();
                return (languageCodes); 
            } 
        }

        /// <summary>
        /// Get a copy of the collection of language codes.
        /// </summary>
        public static Collection<LanguageCode> LanguageCodeList
        {
            get
            {
                Collection<LanguageCode> languageCodes = new Collection<LanguageCode>();

                foreach (LanguageCode languageCode in LanguageCodes)
                    languageCodes.Add(languageCode);
                
                return (languageCodes);
            }
        }

        /// <summary>
        /// Get the collection of undefined language codes.
        /// </summary>
        public static Collection<string> UndefinedCodes
        {
            get
            {
                if (undefinedCodes == null)
                    undefinedCodes = new Collection<string>();
                return (undefinedCodes);
            }
        }


        /// <summary>
        /// Get or set the language code.
        /// </summary>
        public string Code
        {
            get { return (code); }
            set { code = value; }
        }

        /// <summary>
        /// Get or set the description.
        /// </summary>
        public string Description
        {
            get { return (description); }
            set { description = value; }
        }

        /// <summary>
        /// Get or set the translation language code.
        /// </summary>
        public string TranslationCode
        {
            get { return (translationCode); }
            set { translationCode = value; }
        }

        /// <summary>
        /// Get or set the used flag.
        /// </summary>
        public bool Used
        {
            get { return (used); }
            set { used = value; }
        }

        private static Collection<LanguageCode> languageCodes;
        private static Collection<string> undefinedCodes;

        private string code;
        private string description;
        private string translationCode;
        private bool used;

        private static string fileName = "Language Codes.cfg";

        private LanguageCode() { }

        /// <summary>
        /// Initialize a new instance of the LanguageCode class.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        public LanguageCode(string code, string description)
        {
            this.code = code;
            this.description = description;
        }

        /// <summary>
        /// Return a description of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (description);
        }

        /// <summary>
        /// Load the language code collection from the configuration file.
        /// </summary>
        public static void Load()
        {
            languageCodes = new Collection<LanguageCode>();
            languageCodes.Add(new LanguageCode(string.Empty, " -- Undefined --"));

            LanguageCode languageCode = null;
            XmlReader reader = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            string actualName = Path.Combine(RunParameters.ConfigDirectory, fileName);

            try
            {
                reader = XmlReader.Create(actualName, settings);
            }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + actualName);
                return;
            }

            try
            {
                while (!reader.EOF)
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "LanguageCode":
                                if (languageCode != null)
                                    addLanguageCode(languageCode);

                                languageCode = new LanguageCode();
                                languageCode.Code = reader.GetAttribute("code").Trim().ToLowerInvariant();
                                languageCode.Description = reader.GetAttribute("description").Trim();

                                string translationCode = reader.GetAttribute("translationcode");
                                if (translationCode != null)
                                    languageCode.TranslationCode = translationCode.Trim().ToLowerInvariant();

                                break;
                            default:
                                break;
                        }
                    }
                }

                if (languageCode != null)
                    addLanguageCode(languageCode);
            }
            catch (XmlException e)
            {
                Logger.Instance.Write("Failed to load file " + actualName);
                Logger.Instance.Write("Data exception: " + e.Message);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Failed to load file " + actualName);
                Logger.Instance.Write("I/O exception: " + e.Message);
            }

            if (reader != null)
                reader.Close();
        }

        /// <summary>
        /// Add a language code to the collection.
        /// </summary>
        /// <param name="newLanguageCode">The language code to be added.</param>
        public static void addLanguageCode(LanguageCode newLanguageCode)
        {
            foreach (LanguageCode oldLanguageCode in languageCodes)
            {
                if (oldLanguageCode.Code == newLanguageCode.Code)
                    return;

                if (oldLanguageCode.Code.CompareTo(newLanguageCode.Code) > 0)
                {
                    languageCodes.Insert(languageCodes.IndexOf(oldLanguageCode), newLanguageCode);
                    return;
                }
            }

            languageCodes.Add(newLanguageCode);
        }

        /// <summary>
        /// Find a language code the code.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <returns>The language code or null if it cannot be located.</returns>
        public static LanguageCode FindLanguageCode(string code)
        {
            foreach (LanguageCode languageCode in LanguageCodes)
            {
                if (languageCode.Code == code)
                    return (languageCode);
            }

            return (null);
        }

        /// <summary>
        /// Flag a language code as used.
        /// </summary>
        /// <param name="code">The code that has been used</param>
        public static void RegisterUsage(string code)
        {
            LanguageCode languageCode = FindLanguageCode(code);
            if (languageCode != null)
                languageCode.Used = true;
            else
                addUndefinedCode(code);                        
        }

        private static void addUndefinedCode(string newCode)
        {
            foreach (string oldCode in UndefinedCodes)
            {
                if (oldCode == newCode)
                    return;

                if (oldCode.CompareTo(newCode) > 0)
                {
                    undefinedCodes.Insert(undefinedCodes.IndexOf(oldCode), newCode);
                    return;
                }
            }

            undefinedCodes.Add(newCode);
        }

        /// <summary>
        /// Log the usage.
        /// </summary>
        public static void LogUsage()
        {
            if (languageCodes != null)
            {
                Logger.Instance.WriteSeparator("Language Codes Used");

                foreach (LanguageCode languageCode in languageCodes)
                {
                    if (languageCode.Used)
                        Logger.Instance.Write("Language code: " + languageCode.Code + "," + languageCode.Description);
                    languageCode.Used = false;
                }
            }

            if (undefinedCodes != null)
            {
                Logger.Instance.WriteSeparator("Undefined Language Codes Used");

                foreach (string undefinedCode in undefinedCodes)
                    Logger.Instance.Write("Language code: " + undefinedCode);

                undefinedCodes = null;
            }

            Logger.Instance.WriteSeparator("End Of Language Codes");
        }

        /// <summary>
        /// Validate a language code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="usage">The usage ie input or output.</param>
        /// <returns></returns>
        public static bool Validate(string code, Usage usage)
        {
            if (languageCodes == null)
                Load();

            foreach (LanguageCode languageCode in languageCodes)
            {
                if (languageCode.Code == code)
                    return (true);
            }

            return (false);
        }
    }
}
