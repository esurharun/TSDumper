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

namespace DirectShow
{
    /// <summary>
    /// The class that describes a transport packet.
    /// </summary>
    public class TransportPacket
    {
        /// <summary>
        /// Get the sync byte.
        /// </summary>
        public byte SyncByte { get { return (syncByte); } }
        /// <summary>
        /// Get the error indicator.
        /// </summary>
        public bool ErrorIndicator { get { return (errorIndicator); } }
        /// <summary>
        /// Get the start indicator.
        /// </summary>
        public bool StartIndicator { get { return (startIndicator); } }
        /// <summary>
        /// Get the priority indicator.
        /// </summary>
        public bool PriorityIndicator { get { return (priorityIndicator); } }
        /// <summary>
        /// Gets the PID.
        /// </summary>
        public int PID { get { return (pid); } }
        /// <summary>
        /// Get the scambling control.
        /// </summary>
        public int ScramblingControl { get { return (scramblingControl); } }
        /// <summary>
        /// Get the adaption control.
        /// </summary>
        public int AdaptionControl { get { return (adaptionControl); } }
        /// <summary>
        /// Get the continuity count.
        /// </summary>
        public int ContinuityCount { get { return (continuityCount); } }
        
        /// <summary>
        /// Returns true if it is a null packet; false otherwise.
        /// </summary>
        public bool IsNullPacket { get { return (pid == 0x1fff); } }
        /// <summary>
        /// Returns true if the packet has a payload; false otherwise.
        /// </summary>
        public bool HasPayload { get { return (adaptionControl == 1 || adaptionControl == 3); } }
        /// <summary>
        /// Returns true if the packet is scrmabled; false otherwise.
        /// </summary>
        public bool IsScrambled { get { return (scramblingControl != 0); } }

        /// <summary>
        /// Get the index of the next byte in the section following this packet.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The packet has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("TransportPacket: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte syncByte;
        private bool errorIndicator;
        private bool startIndicator;
        private bool priorityIndicator;
        private int pid;
        private int scramblingControl;
        private int adaptionControl;
        private int continuityCount;
        private int adaptionLength;
        
        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the TransportPacket class.
        /// </summary>
        public TransportPacket() { }

        /// <summary>
        /// Parse the packet.
        /// </summary>
        /// <param name="byteData">The buffer section containing the packet.</param>        
        public void Process(byte[] byteData)
        {
            lastIndex = 0;

            try
            {
                syncByte = byteData[lastIndex];
                lastIndex++;

                errorIndicator = (byteData[lastIndex] & 0x80) != 0;
                startIndicator = (byteData[lastIndex] & 0x40) != 0;
                priorityIndicator = (byteData[lastIndex] & 0x20) != 0;
                pid = ((byteData[lastIndex] & 0x1f) * 256) + byteData[lastIndex + 1];
                lastIndex += 2;

                scramblingControl = byteData[lastIndex] >> 6;
                adaptionControl = (byteData[lastIndex] & 0x30) >> 4;
                continuityCount = byteData[lastIndex] & 0x0f;
                lastIndex++;

                switch (adaptionControl)
                {
                    case 1:
                        adaptionLength = 0;
                        break;
                    case 2:
                    case 3:
                        adaptionLength = (int)byteData[lastIndex];
                        lastIndex += adaptionLength;
                        break;
                    default:
                        break;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {                
                Logger.Instance.Dump("Failing Transport Packet", byteData, byteData.Length);
                throw (new ArgumentOutOfRangeException("Block length: " + byteData.Length + " last index: " + lastIndex));
            }
        }

        /// <summary>
        /// Validate the packet fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A packet field is not valid.
        /// </exception>
        public void Validate() 
        {
            if (syncByte != 0x47)
                throw (new ArgumentOutOfRangeException("Sync byte"));
        }

        /// <summary>
        /// Log the packet fields.
        /// </summary>
        public void LogMessage() { }
    }
}
