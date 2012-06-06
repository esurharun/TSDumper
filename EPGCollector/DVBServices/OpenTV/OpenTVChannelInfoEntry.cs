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
    /// The class that describes an OpenTV channel info entry.
    /// </summary>
    public class OpenTVChannelInfoEntry
    {
        /// <summary>
        /// Get the service ID for the channel.
        /// </summary>
        public int ServiceID { get { return (serviceID); } }
        /// <summary>
        /// Get the channel ID of the channel.
        /// </summary>
        public int ChannelID { get { return (channelID); } }
        /// <summary>
        /// Get the user channel number for the channel.
        /// </summary>
        public int UserNumber { get { return (userNumber); } }
        /// <summary>
        /// Get the type of channel.
        /// </summary>
        public int Type { get { return (type); } }
        /// <summary>
        /// Get the channel flags.
        /// </summary>
        public byte[] Flags { get { return (flags); } }

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
                    throw (new InvalidOperationException("OpenTVChannelInfoEntry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        /// <summary>
        /// Get the length of the entry.
        /// </summary>
        public int Length { get { return(length); } }

        private int serviceID;
        private int channelID;
        private int userNumber;
        private int type;
        private byte[] flags;

        private int lastIndex = -1;
        private int length;

        /// <summary>
        /// Initialize a new instance of the OpenTVChannelInfoEntry.
        /// </summary>
        public OpenTVChannelInfoEntry() { }

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

                type = (int)byteData[lastIndex];
                lastIndex++;

                channelID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                userNumber = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                flags = Utils.GetBytes(byteData, lastIndex, 2);
                lastIndex += 2;

                length = lastIndex - index;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Open TV Channel Info Entry message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV CHANNEL INFO ENTRY: Service ID: " + serviceID +
                " Type: " + type +
                " Channel ID: " + channelID +
                " User ID: " + userNumber +
                " Flags: " + Utils.ConvertToHex(flags));
        }
    }
}
