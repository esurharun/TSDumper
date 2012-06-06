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
    /// The class that describes an OpenTV series link record.
    /// </summary>
    internal class OpenTVSeriesLinkRecord : OpenTVRecordBase
    {
        /// <summary>
        /// Get the tag value for this record.
        /// </summary>
        public const int TagValue = 0xc1;

        /// <summary>
        /// Get the series link.
        /// </summary>
        public int SeriesLink { get { return (seriesLink); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the record.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The record has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVSeriesLinkRecord: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int seriesLink;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVSeriesLinkRecord class.
        /// </summary>
        internal OpenTVSeriesLinkRecord() { }

        /// <summary>
        /// Parse the record.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the record.</param>
        /// <param name="index">Index of the first byte of the record data in the MPEG2 section.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                seriesLink = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
        }

        /// <summary>
        /// Validate the record data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A record data field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the record data fields.
        /// </summary>
        internal override void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "");
            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV SERIES LINK RECORD: Series link: " + seriesLink);
        }
    }
}
