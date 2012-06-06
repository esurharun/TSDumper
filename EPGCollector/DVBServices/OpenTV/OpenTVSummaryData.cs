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
    /// The class that describes Open TV summary data.
    /// </summary>
    public class OpenTVSummaryData
    {
        /// <summary>
        /// Get the event identification.
        /// </summary>
        public int EventID { get { return (eventID); } }

        /// <summary>
        /// Get the short description of the event.
        /// </summary>
        public string ShortDescription
        {
            get
            {
                OpenTVShortDescriptionRecord record = (OpenTVShortDescriptionRecord)getRecord(OpenTVShortDescriptionRecord.TagValue);
                if (record != null)
                    return (record.Description);
                else
                    return ("No Synopsis Available");
            } 
        }

        /// <summary>
        /// Get the raw bytes of short description of the event.
        /// </summary>
        public byte[] ShortDescriptionBytes
        {
            get
            {
                OpenTVShortDescriptionRecord record = (OpenTVShortDescriptionRecord)getRecord(OpenTVShortDescriptionRecord.TagValue);
                if (record != null)
                    return (record.DescriptionBytes);
                else
                    return (new byte[] { 0x00 });
            }
        }

        /// <summary>
        /// Get the extended description of the event.
        /// </summary>
        public string ExtendedDescription
        {
            get
            {
                OpenTVExtendedDescriptionRecord record = (OpenTVExtendedDescriptionRecord)getRecord(OpenTVExtendedDescriptionRecord.TagValue);
                if (record != null)
                    return (record.Description);
                else
                    return (null);
            }
        }

        /// <summary>
        /// Get the series link of the event.
        /// </summary>
        public int SeriesLink
        {
            get
            {
                OpenTVSeriesLinkRecord record = (OpenTVSeriesLinkRecord)getRecord(OpenTVSeriesLinkRecord.TagValue);
                if (record != null)
                    return (record.SeriesLink);
                else
                    return (-1);
            }
        }

        /// <summary>
        /// Get the collection of records for this summary section.
        /// </summary>
        internal Collection<OpenTVRecordBase> Records
        {
            get
            {
                if (records == null)
                    records = new Collection<OpenTVRecordBase>();
                return (records);
            }
        }

        /// <summary>
        /// Get the collection of undefined records for this summary section.
        /// </summary>
        internal Collection<OpenTVRecordBase> UndefinedRecords
        {
            get
            {
                if (records == null)
                    return (null);

                Collection<OpenTVRecordBase> undefinedRecords = new Collection<OpenTVRecordBase>();

                foreach (OpenTVRecordBase record in records)
                {
                    if (record.IsUndefined)
                        undefinedRecords.Add(record);
                }

                return (undefinedRecords);
            }
        }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the summary data.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The summary data has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVSummaryData: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int eventID;
        private int length;

        private Collection<OpenTVRecordBase> records;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVSummaryData class.
        /// </summary>
        public OpenTVSummaryData() { }

        /// <summary>
        /// Parse the summary data.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the summary data.</param>
        /// <param name="index">Index of the first byte of the summary data in the MPEG2 section.</param>
        /// <param name="baseDate">The base date for the program events.</param>
        internal void Process(byte[] byteData, int index, DateTime baseDate)
        {
            lastIndex = index;

            try
            {
                eventID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                length = ((byteData[lastIndex] & 0x0f) * 256) + byteData[lastIndex + 1];
                lastIndex += 2;

                int recordLength = length;

                while (recordLength != 0)
                {
                    OpenTVRecordBase record = OpenTVRecordBase.Instance(byteData, lastIndex);
                    Records.Add(record);

                    lastIndex += record.TotalLength;
                    recordLength -= record.TotalLength;
                }  

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Open TV Summary Data message is short"));
            }
        }

        /// <summary>
        /// Validate the summary data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A summary data field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the summary data fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV SUMMARY DATA: Event ID: " + eventID +
                " Length: " + length);

            if (records != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (OpenTVRecordBase record in records)
                    record.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }

        private OpenTVRecordBase getRecord(int tag)
        {
            if (records == null)
                return (null);

            foreach (OpenTVRecordBase record in records)
            {
                if (record.Tag == tag)
                    return (record);
            }

            return (null);
        }
    }
}
