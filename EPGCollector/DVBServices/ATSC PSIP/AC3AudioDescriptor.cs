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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// ATSC PSIP AC3 Audio descriptor class.
    /// </summary>
    internal class AC3AudioDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the sample rate code.
        /// </summary>
        public int SampleRateCode { get { return (sampleRateCode); } }
        /// <summary>
        /// Get the BSID.
        /// </summary>
        public int Bsid { get { return (bsid); } }
        /// <summary>
        /// Get the bit rate code.
        /// </summary>
        public int BitRateCode { get { return (bitRateCode); } }
        /// <summary>
        /// Get the surround mode.
        /// </summary>
        public int SurroundMode { get { return (surroundMode); } }
        /// <summary>
        /// Get the BSMOD.
        /// </summary>
        public int Bsmod { get { return (bsmod); } }
        /// <summary>
        /// Get the number of channels.
        /// </summary>
        public int NumberOfChannels { get { return (numberOfChannels); } }
        /// <summary>
        /// Get the full service flag.
        /// </summary>
        public bool FullServiceFlag { get { return (fullServiceFlag); } }
        /// <summary>
        /// Get the language code.
        /// </summary>
        public int LanguageCode { get { return (languageCode); } }
        /// <summary>
        /// Get the languageCode 2.
        /// </summary>
        public int LanguageCode2 { get { return (languageCode2); } }
        /// <summary>
        /// Get the main ID.
        /// </summary>
        public int MainID { get { return (mainID); } }
        /// <summary>
        /// Get the prioroty.
        /// </summary>
        public int Priority { get { return (priority); } }
        /// <summary>
        /// Get the ASVC flags.
        /// </summary>
        public byte ASVCFlags { get { return (asvcFlags); } }
        /// <summary>
        /// Get the description.
        /// </summary>
        public string Description { get { return (description); } }
        /// <summary>
        /// Get the language.
        /// </summary>
        public string Language { get { return (language); } }
        /// <summary>
        /// Get the language 2.
        /// </summary>
        public string Language2 { get { return (language2); } }
        /// <summary>
        /// Get the additional info.
        /// </summary>
        public byte[] AdditionalInfo { get { return (additionalInfo); } }

        /// <summary>
        /// Get the index of the next byte in the section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("PSIPAC3AudioDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int sampleRateCode;
        private int bsid;
        private int bitRateCode;
        private int surroundMode;
        private int bsmod;
        private int numberOfChannels;
        private bool fullServiceFlag;
        private int languageCode;
        private int languageCode2;
        private int mainID;
        private int priority;
        private byte asvcFlags;
        private string description;
        private string language;
        private string language2;
        private byte[] additionalInfo;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the AC3AudioDescriptor class.
        /// </summary>
        internal AC3AudioDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                sampleRateCode = byteData[lastIndex] >> 5;
                bsid = byteData[lastIndex] & 0x1f;
                lastIndex++;

                bitRateCode = byteData[lastIndex] >> 2;
                surroundMode = byteData[lastIndex] & 0x03;
                lastIndex++;

                bsmod = byteData[lastIndex] >> 5;
                numberOfChannels = (byteData[lastIndex] >> 1) & 0x0f;
                fullServiceFlag = ((byteData[lastIndex] & 0x01) != 0);
                lastIndex++;

                if (lastIndex - index == Length)
                {
                    Validate();
                    return;
                }

                languageCode = (int)byteData[lastIndex];
                lastIndex++;

                if (lastIndex - index == Length)
                {
                    Validate();
                    return;
                }

                if (numberOfChannels == 0)
                {
                    languageCode2 = (int)byteData[lastIndex];
                    lastIndex++;
                }

                if (lastIndex - index == Length)
                {
                    Validate();
                    return;
                }

                if (bsmod < 2)
                {
                    mainID = byteData[lastIndex] >> 5;
                    priority = (byteData[lastIndex] >> 3) & 0x03;
                }
                else
                    asvcFlags = byteData[lastIndex];
                lastIndex++;

                if (lastIndex - index == Length)
                {
                    Validate();
                    return;
                }

                int descriptionLength = byteData[lastIndex] >> 1;
                bool descriptionCode = ((byteData[lastIndex] & 0x01) != 0);
                lastIndex++;

                if (descriptionLength != 0)
                {
                    if (descriptionCode)
                        description = Utils.GetString(byteData, lastIndex, descriptionLength);
                    else
                        description = Utils.GetUnicodeString(byteData, lastIndex, descriptionLength);
                    lastIndex += descriptionLength;
                }

                if (lastIndex - index == Length)
                {
                    Validate();
                    return;
                }

                bool languageFlag = ((byteData[lastIndex] >> 7) != 0);
                bool languageFlag2 = (((byteData[lastIndex] >> 6) & 0x01) != 0);
                lastIndex++;

                if (languageFlag)
                {
                    language = Utils.GetString(byteData, lastIndex, 3);
                    lastIndex += 3;
                }

                if (languageFlag2)
                {
                    language2 = Utils.GetString(byteData, lastIndex, 3);
                    lastIndex += 3;
                }

                additionalInfo = null;

                lastIndex = index + Length;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP AC3 Audio Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            string descriptionString;
            if (description != null)
                descriptionString = description;
            else
                descriptionString = "n/a";

            string languageString;
            if (language != null)
                languageString = language;
            else
                languageString = "n/a";

            string languageString2;
            if (language2 != null)
                languageString2 = language2;
            else
                languageString2 = "n/a";

            string additionalInfoString;
            if (additionalInfo != null)
                additionalInfoString = Utils.ConvertToHex(additionalInfo);
            else
                additionalInfoString = "n/a";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP AC3 AUDIO DESCRIPTOR: Sample rate: " + sampleRateCode +
                " BSID: " + bsid +
                " Bit rate: " + bitRateCode +
                " Surround mode: " + surroundMode +
                " BSMOD: " + bsmod +
                " No. channels: " + numberOfChannels +
                " Full svc: " + fullServiceFlag +
                " Lang code: " + languageCode +
                " Lang code2: " + languageCode2 +
                " Main ID: " + mainID +
                " Priority: " + priority +
                " ASVC flags: " + asvcFlags +
                " Desc: " + descriptionString +
                " Language: " + languageString +
                " Language2: " + languageString2 +
                " Additional info: " + additionalInfoString);

            
        }
    }
}
