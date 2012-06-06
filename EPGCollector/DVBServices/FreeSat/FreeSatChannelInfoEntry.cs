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
    /// The class that describes a FreeSat channel info entry.
    /// </summary>
    public class FreeSatChannelInfoEntry
    {
        /// <summary>
        /// Get the service ID.
        /// </summary>
        public int ServiceID { get { return (serviceID); } }
        /// <summary>
        /// Get the length of the details.
        /// </summary>
        public int DetailLength { get { return (detailLength); } }
        /// <summary>
        /// Get the user channel ID.
        /// </summary>
        public int UserNumber { get { return (userNumber); } }
        /// <summary>
        /// Get the unknown bytes(1).
        /// </summary>
        public byte[] Unknown1 { get { return (unknown1); } }
        /// <summary>
        /// Get the unknown bytes(2).
        /// </summary>
        public byte[] Unknown2 { get { return (unknown2); } }

        /// <summary>
        /// Get the index of the next byte in the section following this entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The entry has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("FreeSatChannelInfoEntry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        /// <summary>
        /// Get the length of the entry.
        /// </summary>
        public int Length { get { return (length); } }

        private int serviceID;
        private int detailLength;
        private int userNumber = -1;
        private byte[] unknown1;
        private byte[] unknown2;

        private int lastIndex = -1;
        private int length;

        /// <summary>
        /// Initialize a new instance of the FreeSatChannelInfoEntry class.
        /// </summary>
        public FreeSatChannelInfoEntry() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the entry.</param>
        /// <param name="index">Index of the first byte in the MPEG2 section of the entry.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                serviceID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                unknown1 = Utils.GetBytes(byteData, lastIndex, 2);
                lastIndex += 2;

                detailLength = (int)byteData[lastIndex];
                lastIndex++;

                if (detailLength != 0)
                {
                    unknown2 = Utils.GetBytes(byteData, lastIndex, detailLength);
                    lastIndex += detailLength;

                    if (detailLength == 4 || detailLength == 8)
                        userNumber = ((unknown2[0] & 0x0f) * 256) + unknown2[1];
                }

                length = lastIndex - index;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The FreeSat Channel Info Entry message is short"));
            }
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        internal void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            string unknown1String;
            if (unknown1 != null)
                unknown1String = Utils.ConvertToHex(unknown1);
            else
                unknown1String = "not present";

            string unknown2String;
            if (unknown2 != null)
                unknown2String = Utils.ConvertToHex(unknown2);
            else
                unknown2String = "not present";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "FREESAT CHANNEL INFO ENTRY: Service ID: " + serviceID +
                " Det lth: " + detailLength +
                " User No: " + userNumber +
                " Unknown1: " + unknown1String +
                " Unknown2: " + unknown2String);
        }
    }
}
