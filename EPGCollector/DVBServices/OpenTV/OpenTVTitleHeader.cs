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
    /// The class that describes an Open TV title header.
    /// </summary>
    public class OpenTVTitleHeader
    {
        /// <summary>
        /// Get the channel identification.
        /// </summary>
        public int ChannelID { get { return (channelID); } }
        /// <summary>
        /// Get the title date base.
        /// </summary>
        public DateTime BaseDate { get { return (baseDate); } }
        /// <summary>
        /// Get the data collection related to this title.
        /// </summary>
        public Collection<OpenTVTitleData> TitleData { get { return (titleData); } }
        
        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the title header.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The title header has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVTitleHeader: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int channelID;
        private DateTime baseDate;
        private Collection<OpenTVTitleData> titleData;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVTitleHeader class.
        /// </summary>
        public OpenTVTitleHeader() { }

        /// <summary>
        /// Parse the title header.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the title header.</param>
        /// <param name="index">Index of the first byte of the title header in the MPEG2 section.</param>
        /// <param name="mpeg2Header">The MPEG2 header of the section.</param>
        /// <param name="pid">The PID of the section.</param>
        /// <param name="tid">The table ID of the section.</param>
        internal void Process(byte[] byteData, int index, Mpeg2ExtendedHeader mpeg2Header, int pid, int tid)
        {
            lastIndex = index;

            channelID = mpeg2Header.TableIDExtension;

            try
            {
                baseDate = getDate(Utils.Convert2BytesToInt(byteData, lastIndex));
                lastIndex += 2;

                while (lastIndex < byteData.Length - 4)
                {
                    OpenTVTitleData data = new OpenTVTitleData();
                    data.Process(byteData, lastIndex, baseDate, channelID, pid, tid);

                    if (!data.IsEmpty)
                    {
                        if (titleData == null)
                            titleData = new Collection<OpenTVTitleData>();
                        titleData.Add(data);
                    }

                    lastIndex = data.Index;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
        }

        private DateTime getDate(int mjd)
        {
            int j = mjd + 2400001 + 68569;
            int c = 4 * j / 146097;
            j = j - (146097 * c + 3) / 4;

            int y = 4000 * (j + 1) / 1461001;
            j = j - 1461 * y / 4 + 31;            
            int m = 80 * j / 2447;

            int day = j - 2447 * m / 80;
            j = m / 11;
            int month = m + 2 - (12 * j);
            int year = 100 * (c - 49) + y + j;

            return (new DateTime(year, month, day));
        }

        /// <summary>
        /// Validate the title header fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A title header field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the title header fields.
        /// </summary>
        public void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV TITLE HEADER: Channel ID: " + channelID +
                " Base date : " + baseDate);

            if (titleData != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (OpenTVTitleData data  in titleData)
                    data.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
