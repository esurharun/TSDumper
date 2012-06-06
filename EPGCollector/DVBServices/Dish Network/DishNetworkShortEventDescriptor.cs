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
    /// Dish Network Short Event descriptor class.
    /// </summary>
    internal class DishNetworkShortEventDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the event name.
        /// </summary>
        public string EventName { get { return (eventName); } }

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
                    throw (new InvalidOperationException("DishNetworkShortEventDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] startBytes;
        private byte[] eventNameBytes;
        private string eventName;

        private int huffmanTable;
        private int compressedLength;
        private int decompressedLength;
        private int loggedStartIndex;
        
        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DishNetworkShortEventDescriptor class.
        /// </summary>
        internal DishNetworkShortEventDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            startBytes = Utils.GetBytes(byteData, lastIndex, 2);

            int startIndex;

            if ((byteData[lastIndex + 1] & 0xf8) == 0x80)
            {
                compressedLength = Length;

                if ((byteData[lastIndex] & 0x40) != 0)
                    decompressedLength = (byteData[lastIndex] & 0x3f) | ((byteData[lastIndex + 1] << 6) & 0xff);
                else
                    decompressedLength = byteData[lastIndex] & 0x3f;

                startIndex = lastIndex + 2;
                loggedStartIndex = 2;
            }
            else
            {
                compressedLength = Length - 1;
                decompressedLength = byteData[lastIndex] & 0x7f;
                startIndex = lastIndex + 1;
                loggedStartIndex = 1;
            }

            if (compressedLength <= 0)
            {
                lastIndex = index + Length;
                return;
            }

            eventNameBytes = Utils.GetBytes(byteData, startIndex, compressedLength);

            if (Table <= 0x80)
                huffmanTable = 1;
            else
                huffmanTable = 2;

            eventName = SingleTreeDictionaryEntry.DecodeData(huffmanTable, eventNameBytes);

            lastIndex = index + Length;

            Validate();            
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DISH SHORT EVENT DESCRIPTOR: Huffman table: " + huffmanTable +
                " Compressed lth: " + compressedLength +
                " Decompressed lth: " + decompressedLength +
                " Start bytes: " + Utils.ConvertToHex(startBytes) +
                " Start index: " + loggedStartIndex +
                " Name: " + eventName);
        }
    }
}
