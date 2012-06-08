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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a MediaHighway1 channel section.
    /// </summary>
    public class MediaHighway1ChannelSection
    {
        /// <summary>
        /// Get the collection of channels.
        /// </summary>
        public Collection<MediaHighwayChannelInfoEntry> Channels
        {
            get
            {
                if (channels == null)
                    channels = new Collection<MediaHighwayChannelInfoEntry>();
                return (channels);
            }
        }

        private byte[] unknown;

        private Collection<MediaHighwayChannelInfoEntry> channels;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway1ChannelSection class.
        /// </summary>
        internal MediaHighway1ChannelSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="index">The index of the first byte of the data portion.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                unknown = Utils.GetBytes(byteData, lastIndex, 1);
                lastIndex++;

                while (lastIndex < byteData.Length)
                {
                    MediaHighwayChannelInfoEntry channelEntry = new MediaHighwayChannelInfoEntry();
                    channelEntry.Process(byteData, lastIndex);
                    Channels.Add(channelEntry);

                    lastIndex = channelEntry.Index;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Channel Section message is short"));
            }

            Validate();
        }

        /// <summary>
        /// Validate the section fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A section field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW1 CHANNEL SECTION");

            if (channels != null)
            {
                foreach (MediaHighwayChannelInfoEntry channelInfoEntry in channels)
                {
                    Logger.IncrementProtocolIndent();
                    channelInfoEntry.LogMessage();
                    Logger.DecrementProtocolIndent();
                }
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the Open TV Title table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        public static MediaHighway1ChannelSection ProcessMediaHighwayChannelTable(byte[] byteData)
        {
            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);

                MediaHighway1ChannelSection channelSection = new MediaHighway1ChannelSection();
                channelSection.Process(byteData, mpeg2Header.Index);
                channelSection.LogMessage();
                return (channelSection);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Channel section parsing failed: " + e.Message);
                return (null);
            }
        }
    }
}
