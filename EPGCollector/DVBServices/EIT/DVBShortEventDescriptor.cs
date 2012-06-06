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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// DVB Short Event descriptor class.
    /// </summary>
    internal class DVBShortEventDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the event name.
        /// </summary>
        public virtual string EventName { get { return (eventName); } }

        /// <summary>
        /// Get the short description.
        /// </summary>
        public virtual string ShortDescription { get { return (shortDescription); } }

        /// <summary>
        /// Get the language code.
        /// </summary>
        public string LanguageCode { get { return (languageCode); } }

        /// <summary>
        /// Get the index of the next byte in the EIT section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception>        
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ShortEventDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private string languageCode;
        private string eventName;
        private string shortDescription;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBShortEventDescriptor class.
        /// </summary>
        internal DVBShortEventDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        { 	 
            lastIndex = index;

            try
            {
                languageCode = Utils.GetString(byteData, lastIndex, 3);
                lastIndex += languageCode.Length;

                int eventNameLength = (int)byteData[lastIndex];
                lastIndex++;

                if (eventNameLength != 0)
                {
                    int nameIndex = lastIndex;

                    eventName = Utils.GetString(byteData, lastIndex, eventNameLength);
                    lastIndex += eventNameLength;
                }

                int textLength = (int)byteData[lastIndex];
                lastIndex++;

                if (textLength != 0)
                {
                    shortDescription = Utils.GetString(byteData, lastIndex, textLength);
                    lastIndex += textLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Instance.Dump("DVB Short Event Descriptor message is short", byteData, byteData.Length);
                throw (new ArgumentOutOfRangeException("The DVB Short Event Descriptor message is short"));
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

            string eventName;
            if (EventName != null)
                eventName = EventName;
            else
                eventName = "?";

            string shortDescription;
            if (ShortDescription != null)
                shortDescription = ShortDescription;
            else
                shortDescription = "?";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB SHORT EVENT DESCRIPTOR: Lang code: " + languageCode + 
                 " Event name: " + eventName +
                 " Short desc: " + shortDescription);
        }
    }
}
