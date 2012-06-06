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

namespace DirectShow
{
    /// <summary>
    /// The class that describes a stream information packet.
    /// </summary>
    public class SIPacket
    {
        /// <summary>
        /// Get the transport information packet.
        /// </summary>
        public TransportPacket TransportPacket { get { return (transportPacket); } }
        /// <summary>
        /// Get the data bytes.
        /// </summary>
        public byte[] ByteData { get { return (byteData); } }
        /// <summary>
        /// Get the data pointer.
        /// </summary>
        public int Pointer { get { return (pointer); } }
        /// <summary>
        /// Get the data index.
        /// </summary>
        public int DataIndex { get { return (lastIndex + pointer); } }
        
        /// <summary>
        /// Get the current index position.
        /// </summary>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("SIPacket: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private TransportPacket transportPacket;
        private int pointer;

        private int lastIndex = -1;
        private byte[] byteData;

        /// <summary>
        /// Initialize a new instance of the SIPacket class.
        /// </summary>
        public SIPacket() { }

        /// <summary>
        /// Process the packet.
        /// </summary>
        /// <param name="byteData">The first byte of the packet.</param>
        /// <param name="transportPacket">The tranport packet containing this packet.</param>
        public void Process(byte[] byteData, TransportPacket transportPacket)
        {
            this.transportPacket = transportPacket;
            this.byteData = byteData;
            lastIndex = transportPacket.Index;

            try
            {
                if (transportPacket.StartIndicator)
                {
                    pointer = (int)byteData[lastIndex];
                    lastIndex++;
                }
                else
                    pointer = 0;
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Transport Packet is short"));
            }

            Validate();
            
        }

        /// <summary>
        /// Validate the packet fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A packet field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the packet fields.
        /// </summary>
        public void LogMessage() { }
    }
}
