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
    /// The class that describes a MediaHighway channel  info entry.
    /// </summary>
    public class MediaHighwayChannelInfoEntry
    {
        /// <summary>
        /// Get the original network ID for the channel.
        /// </summary>
        public int OriginalNetworkID { get { return (originalNetworkID); } }
        /// <summary>
        /// Get the transport stream ID for the channel.
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }
        /// <summary>
        /// Get the service ID for the channel.
        /// </summary>
        public int ServiceID { get { return (serviceID); } }
        /// <summary>
        /// Get the name of the channel.
        /// </summary>
        public string Name { get { return (name); } }
        /// <summary>
        /// Get the unknown data.
        /// </summary>
        public byte[] Unknown { get { return (unknown); } } 

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
                    throw (new InvalidOperationException("MediaHighwayChannelEntry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        /// <summary>
        /// Get the index of the next name byte in the section following this entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The entry has not been processed.
        /// </exception> 
        public int NameIndex
        {
            get
            {
                if (nameIndex == -1)
                    throw (new InvalidOperationException("MediaHighwayChannelEntry:  Name index requested before block processed"));
                return (nameIndex);
            }
        }

        /// <summary>
        /// Get the length of the entry.
        /// </summary>
        public int Length { get { return (length); } }

        private int originalNetworkID;
        private int transportStreamID; 
        private int serviceID;
        private string name;
        private byte[] unknown;
        
        private int lastIndex = -1;
        private int nameIndex;
        private int length;

        /// <summary>
        /// Initialize a new instance of the MediaHighwayChannelInfoEntry.
        /// </summary>
        public MediaHighwayChannelInfoEntry() { }

        /// <summary>
        /// Parse the MHW1 descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the entry.</param>
        /// <param name="index">Index of the first byte in the MPEG2 section of the entry.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                originalNetworkID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                transportStreamID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                serviceID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                name = Utils.GetString(byteData, lastIndex, 16, true).Trim();
                lastIndex += 16;

                length = lastIndex - index;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway Channel Info Entry message is short"));
            }
        }

        /// <summary>
        /// Parse the MHW2 descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the entry.</param>
        /// <param name="index">Index of the first byte in the MPEG2 section of the entry.</param>
        /// <param name="nameLengthIndex">Index of the first byte channel name in the MPEG2 section of the entry.</param>
        internal void Process(byte[] byteData, int index, int nameLengthIndex)
        {
            lastIndex = index;
            nameIndex = nameLengthIndex;

            try
            {
                originalNetworkID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                transportStreamID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                serviceID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                unknown = Utils.GetBytes(byteData, lastIndex, 2);
                lastIndex += unknown.Length;

                int nameLength = (int)byteData[nameIndex] & 0x3f;
                nameIndex++;

                name = Utils.GetString(byteData, nameIndex, nameLength, true).Trim();
                nameIndex += nameLength;

                length = lastIndex - index;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway Channel Info Entry message is short"));
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

            string unknownString;
            if (unknown == null)
                unknownString = "n/a";
            else
                unknownString = Utils.ConvertToHex(unknown);

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW CHANNEL INFO ENTRY: ONID: " + originalNetworkID +
                " TSID: " + transportStreamID +
                " SID: " + serviceID +
                " Name: " + name +
                " Unknown: " + unknownString);
        }
    }
}
