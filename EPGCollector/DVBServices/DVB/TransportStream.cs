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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a transport stream.
    /// </summary>
    public class TransportStream
    {
        /// <summary>
        /// Get the transport strema identification (TSID).
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }
        /// <summary>
        /// Get the original network identification (ONID).
        /// </summary>
        public int OriginalNetworkID { get { return (originalNetworkID); } }
        /// <summary>
        /// Get the collection of descriptors describing this transport stream.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }
        /// <summary>
        /// Get the total length of the transport stream data.
        /// </summary>
        public int TotalLength { get { return (totalLength); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the transport stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The transport stream has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("TransportStream: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int transportStreamID;
        private int originalNetworkID;
        private Collection<DescriptorBase> descriptors;
        private int totalLength;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the TransportStream class.
        /// </summary>
        public TransportStream() { }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the transport stream.</param>
        /// <param name="index">Index of the first byte of the transport stream in the MPEG2 section.</param>
        /// <param name="scope">The scope of the processing..</param>
        internal void Process(byte[] byteData, int index, Scope scope)
        {
            lastIndex = index;

            try
            {
                transportStreamID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                originalNetworkID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                int descriptorLoopLength = ((byteData[lastIndex] & 0x0f) * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;

                totalLength = descriptorLoopLength + 6;

                if (descriptorLoopLength != 0)
                {
                    descriptors = new Collection<DescriptorBase>();

                    while (descriptorLoopLength != 0)
                    {
                        DescriptorBase descriptor = DescriptorBase.Instance(byteData, lastIndex, scope);

                        if (!descriptor.IsEmpty)
                        {
                            descriptors.Add(descriptor);

                            lastIndex = descriptor.Index;
                            descriptorLoopLength -= descriptor.TotalLength;
                        }
                        else
                        {
                            lastIndex += DescriptorBase.MinimumDescriptorLength;
                            descriptorLoopLength -= DescriptorBase.MinimumDescriptorLength;
                        }
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Transport Stream message is short"));
            }
        }

        /// <summary>
        /// Validate the transport stream fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A transport stream field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the transport stream fields.
        /// </summary>
        public void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "TRANSPORT STREAM: TSID: " + transportStreamID +
                " ONID: " + originalNetworkID);

            if (descriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DescriptorBase descriptor in descriptors)
                    descriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
