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
    /// The class that describes a MediaHighway2 channel section.
    /// </summary>
    public class MediaHighway2ChannelSection
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

        private byte[] fillBytes;
        private int channelCount;

        private Collection<MediaHighwayChannelInfoEntry> channels;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway2ChannelSection class.
        /// </summary>
        internal MediaHighway2ChannelSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="index">The index of the first byte of the data portion.</param>
        /// <returns>True if the section is an MHW2 channel section; false otherwise.</returns>
        internal bool Process(byte[] byteData, int index)
        {
            lastIndex = index;

            if (byteData[lastIndex] != 0x00)
                return (false);

            try
            {
                fillBytes = Utils.GetBytes(byteData, lastIndex, 117);
                lastIndex += fillBytes.Length;

                channelCount = (int)byteData[lastIndex];
                lastIndex++;

                int nameIndex = lastIndex + (8 * channelCount);

                while (Channels.Count < channelCount)
                {
                    MediaHighwayChannelInfoEntry channelEntry = new MediaHighwayChannelInfoEntry();
                    channelEntry.Process(byteData, lastIndex, nameIndex);
                    Channels.Add(channelEntry);

                    lastIndex = channelEntry.Index;
                    nameIndex = channelEntry.NameIndex;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway2 Channel Section message is short"));
            }

            Validate();

            return (true);
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW2 CHANNEL SECTION");

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
        public static MediaHighway2ChannelSection ProcessMediaHighwayChannelTable(byte[] byteData)
        {
            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);                

                MediaHighway2ChannelSection channelSection = new MediaHighway2ChannelSection();
                bool process = channelSection.Process(byteData, mpeg2Header.Index);
                if (process)
                {
                    channelSection.LogMessage();
                    return (channelSection);
                }
                else
                    return (null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Channel section parsing failed: " + e.Message);
                return (null);
            }
        }
    }
}
