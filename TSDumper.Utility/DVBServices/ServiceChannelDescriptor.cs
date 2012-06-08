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
    /// The class that describes a Channel descriptor that occurs in the SDT.
    /// </summary>
    internal class ServiceChannelDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the  channel number.
        /// </summary>
        public int ChannelNumber { get { return (channelNumber); } }        

        /// <summary>
        /// Get the index of the next byte in the section following this entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The entry has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ServiceChannelDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int channelNumber = -1;
        private byte[] undefinedData;

        private int lastIndex = -1;        

        /// <summary>
        /// Initialize a new instance of the ServiceChannelDescriptor class.
        /// </summary>
        internal ServiceChannelDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the entry.</param>
        /// <param name="index">Index of the first byte in the MPEG2 section of the entry.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                channelNumber = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                int undefinedDataLength = Length - (lastIndex - index);
                if (undefinedDataLength != 0)
                    undefinedData = Utils.GetBytes(byteData, lastIndex, undefinedDataLength);

                lastIndex = index + Length;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Service Channel Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            string undefinedDataString;
            if (undefinedData != null)
                undefinedDataString = Utils.ConvertToHex(undefinedData);
            else
                undefinedDataString = "Not present";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "SERVICE CHANNEL DESCRIPTOR: Channel no: " + channelNumber +
                " Undefined: " + undefinedDataString);
        }
    }
}
