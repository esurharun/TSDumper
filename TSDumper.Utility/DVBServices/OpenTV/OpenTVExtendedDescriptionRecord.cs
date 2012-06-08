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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an OpenTV extended description record.
    /// </summary>
    internal class OpenTVExtendedDescriptionRecord : OpenTVRecordBase
    {
        /// <summary>
        /// Get the tag value for this record.
        /// </summary>
        public const int TagValue = 0xbb;

        /// <summary>
        /// Get the extended description.
        /// </summary>
        public string Description { get { return (SingleTreeDictionaryEntry.DecodeData(description)); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the record.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The record has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVExtendedDescriptionRecord: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] description;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVExtendedDescriptionRecord class.
        /// </summary>
        internal OpenTVExtendedDescriptionRecord() { }

        /// <summary>
        /// Parse the record.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the record.</param>
        /// <param name="index">Index of the first byte of the record data in the MPEG2 section.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                description = Utils.GetBytes(byteData, lastIndex, Length);
                lastIndex += Length;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
        }

        /// <summary>
        /// Validate the record data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A record data field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the record data fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "");
            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV EXTENDED DESCRIPTION RECORD: Description: " + Utils.ConvertToHex(description));
        }
    }
}
