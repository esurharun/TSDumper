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
    /// The class that describes a BIOP Lite component.
    /// </summary>
    public class BIOPLiteComponent    
    {
        /// <summary>
        /// Get the data length.
        /// </summary>
        public int DataLength { get { return (dataLength); } }
        /// <summary>
        /// Get the data.
        /// </summary>
        public byte[] Data { get { return (data); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the component.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The component has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPLiteComponent: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int componentIDTag;
        private int dataLength;
        private byte[] data = new byte[1] { 0x00 };

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPLiteComponent class.
        /// </summary>
        public BIOPLiteComponent() { }

        /// <summary>
        /// Parse the component.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the component.</param>
        /// <param name="index">Index of the first byte of the component in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                componentIDTag = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                dataLength = (int)byteData[lastIndex];
                lastIndex++;

                if (dataLength != 0)
                {
                    data = Utils.GetBytes(byteData, lastIndex, dataLength);
                    lastIndex += dataLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Lite Component message is short"));
            }
        }

        /// <summary>
        /// Validate the component fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A component field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the component fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP LITE COMPONENT: Component ID tag: " + Utils.ConvertToHex(componentIDTag) +
                " Data lth: " + dataLength +
                " Data: " + Utils.ConvertToHex(data));
        }
    }
}
