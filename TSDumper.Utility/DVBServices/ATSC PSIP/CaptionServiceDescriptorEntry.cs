////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2012 nzsjb, Harun Esur                                           //
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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// ATSC PSIP Caption Service descriptor entry class.
    /// </summary>
    internal class CaptionServiceDescriptorEntry
    {
        /// <summary>
        /// Get the language.
        /// </summary>
        public string LanguageCode { get { return (languageCode); } }
        /// <summary>
        /// Get the digital cc flag.
        /// </summary>
        public bool DigitalCC { get { return (digitalCC); } }
        /// <summary>
        /// Get the line21 field flag.
        /// </summary>
        public bool Line21Field { get { return (line21Field); } }
        /// <summary>
        /// Get the caption service number.
        /// </summary>
        public int CaptionServiceNumber { get { return (captionServiceNumber); } }
        /// <summary>
        /// Get the easy reader flag.
        /// </summary>
        public bool EasyReader { get { return (easyReader); } }
        /// <summary>
        /// Get the wide aspect ratio flag.
        /// </summary>
        public bool WideAspectRatio { get { return (wideAspectRatio); } }

        /// <summary>
        /// Get the index of the next byte in the section following this entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("CaptionServiceDescriptorEntry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private string languageCode;
        private bool digitalCC;
        private bool line21Field;
        private int captionServiceNumber;
        private bool easyReader;
        private bool wideAspectRatio;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the CaptionServiceDescriptorEntry class.
        /// </summary>
        internal CaptionServiceDescriptorEntry() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                languageCode = Utils.GetString(byteData, lastIndex, 3);
                lastIndex += 3;

                digitalCC = ((byteData[lastIndex] & 0x80) != 0);

                if (digitalCC)
                    line21Field = ((byteData[lastIndex] & 0x01) != 0);
                else
                    captionServiceNumber = byteData[lastIndex] & 0x3f;
                lastIndex++;

                easyReader = ((byteData[lastIndex] & 0x80) != 0);
                wideAspectRatio = ((byteData[lastIndex] & 0x40) != 0);
                lastIndex += 2;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Caption Service Descriptor Entry message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP CAPTION SERVICE DESCRIPTOR ENTRY: Language: " + languageCode +
                " DigitalCC: " + digitalCC +
                " Line21 field: " + line21Field +
                " Caption svc no.: " + captionServiceNumber +
                " Easy rdr: " + easyReader +
                " Wide asp ratio: " + wideAspectRatio);
        }
    }
}
