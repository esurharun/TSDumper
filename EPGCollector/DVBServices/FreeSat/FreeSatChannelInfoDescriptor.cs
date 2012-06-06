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
    /// FreeSat Channel Info descriptor class.
    /// </summary>
    internal class FreeSatChannelInfoDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the collection of channel entries.
        /// </summary>
        public Collection<FreeSatChannelInfoEntry> ChannelInfoEntries { get { return (channelInfoEntries); } }

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
                    throw (new InvalidOperationException("FreeSatChannelInfoDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private Collection<FreeSatChannelInfoEntry> channelInfoEntries;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the FreeSatChannelInfoDescriptor class.
        /// </summary>
        internal FreeSatChannelInfoDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                if (Length != 2)
                {
                    channelInfoEntries = new Collection<FreeSatChannelInfoEntry>();

                    int length = Length - 2;

                    while (length > 0)
                    {
                        FreeSatChannelInfoEntry channelInfoEntry = new FreeSatChannelInfoEntry();
                        channelInfoEntry.Process(byteData, lastIndex);
                        if (channelInfoEntry.UserNumber != -1)
                            channelInfoEntries.Add(channelInfoEntry);

                        lastIndex += channelInfoEntry.Length;
                        length -= channelInfoEntry.Length;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The FreeSat Channel Info Descriptor message is short"));
            }
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "FREESAT CHANNEL INFO DESCRIPTOR");

            if (channelInfoEntries != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (FreeSatChannelInfoEntry channelInfoEntry in channelInfoEntries)
                    channelInfoEntry.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
