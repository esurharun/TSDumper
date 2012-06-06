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
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a Bell TV series descriptor.
    /// </summary>
    internal class BellTVSeriesDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the series.
        /// </summary>
        public int Series { get { return (series); } }
        /// <summary>
        /// Get the Episode.
        /// </summary>
        public int Episode { get { return (episode); } }
        /// <summary>
        /// Get the original air date.
        /// </summary>
        public DateTime OriginalAirDate { get { return (originalAirDate); } }

        /// <summary>
        /// Get the index of the next byte in the EIT section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BellTVSeriesDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int entryType;
        private int series = -1;
        private int episode = -1;
        private DateTime originalAirDate;

        private int startDate;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BellTVSeriesDescriptor class.
        /// </summary>
        internal BellTVSeriesDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            entryType = (int)byteData[lastIndex];
            lastIndex++;

            series = byteData[lastIndex] << 12 | byteData[lastIndex + 1] << 0x0a | byteData[lastIndex + 2] << 0x02 | (byteData[lastIndex + 3] & 0xc0) >> 0x06;
            lastIndex += 3;

            episode = (byteData[lastIndex] & 0x3f << 08) | byteData[lastIndex + 1];
            lastIndex += 2;

            originalAirDate = getOriginalAirDate(byteData, lastIndex);

            lastIndex = index + Length;
        }

        private DateTime getOriginalAirDate(byte[] byteData, int index)
        {
            startDate = Utils.Convert2BytesToInt(byteData, index);
            if (startDate == 0)
                return (DateTime.MinValue);

            int seconds = (startDate - 40587) * 86400;
            
            try
            {
                DateTime utcStartTime = new DateTime(1970, 1, 1).AddSeconds(((double)seconds));
                return (utcStartTime.ToLocalTime());
            }
            catch (ArgumentOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The start time element(s) are out of range"));
            }
            catch (ArgumentException)
            {
                throw (new ArgumentOutOfRangeException("The start time element(s) result in a start time that is out of range"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BELL TV SERIES DESCRIPTOR: Entry type: " + entryType + 
                " Series: " + series +
                " Episode: " + episode +
                " Original air date: " + startDate + " " + originalAirDate);
        }
    }
}
