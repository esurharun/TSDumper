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
    /// The class that describes Open TV title data.
    /// </summary>
    public class OpenTVTitleData
    {
        /// <summary>
        /// Get the event identification.
        /// </summary>
        public int EventID { get { return (eventID); } }

        /// <summary>
        /// Get the start time of the event.
        /// </summary>
        public DateTime StartTime 
        { 
            get 
            {
                OpenTVTitleDataRecord record = (OpenTVTitleDataRecord)getRecord(OpenTVTitleDataRecord.TagValue);
                return (baseDate + record.StartTimeOffset);
            } 
        }

        /// <summary>
        /// Get the duration of the event.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                OpenTVTitleDataRecord record = (OpenTVTitleDataRecord)getRecord(OpenTVTitleDataRecord.TagValue);
                return (record.Duration);
            } 
        }

        /// <summary>
        /// Get the theme identification of the event.
        /// </summary>
        public int CategoryID
        {
            get
            {
                OpenTVTitleDataRecord record = (OpenTVTitleDataRecord)getRecord(OpenTVTitleDataRecord.TagValue);
                return (record.CategoryID);
            } 
        }

        /// <summary>
        /// Get the name of the event.
        /// </summary>
        public string EventName
        {
            get
            {
                OpenTVTitleDataRecord record = (OpenTVTitleDataRecord)getRecord(OpenTVTitleDataRecord.TagValue);
                return (record.DecodedEventName);
            } 
        }

        /// <summary>
        /// Get the raw bytes of the event name.
        /// </summary>
        public byte[] EventNameBytes
        {
            get
            {
                OpenTVTitleDataRecord record = (OpenTVTitleDataRecord)getRecord(OpenTVTitleDataRecord.TagValue);
                return (record.EventName);
            }
        }

        /// <summary>
        /// Get the flags field of the event.
        /// </summary>
        public byte[] Flags
        {
            get
            {
                OpenTVTitleDataRecord record = (OpenTVTitleDataRecord)getRecord(OpenTVTitleDataRecord.TagValue);
                return (record.Flags);
            } 
        }

        /// <summary>
        /// Get the collection of records for this title section.
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
        /// Get the collection of undefined records for this title section.
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
        /// Return true if the entry is empty; false otherwise.
        /// </summary>
        public bool IsEmpty { get { return (records == null || records.Count == 0); } }

        /// <summary>
        /// Get the PID of the section containing the data.
        /// </summary>
        public int PID { get { return (pid); } }
        /// <summary>
        /// Get the table ID of the section containing the data.
        /// </summary>
        public int Table { get { return (table); } }
        /// <summary>
        /// Get the timestamp when the data arrived.
        /// </summary>
        public DateTime TimeStamp { get { return (timeStamp); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the title data.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The title data has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVTitleData: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int eventID;
        private int length;

        private DateTime baseDate;
        
        private int pid;
        private int table;
        private DateTime timeStamp;

        private Collection<OpenTVRecordBase> records;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVTitleData class.
        /// </summary>
        public OpenTVTitleData() { }

        /// <summary>
        /// Parse the title data.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the title data.</param>
        /// <param name="index">Index of the first byte of the title data in the MPEG2 section.</param>
        /// <param name="baseDate">The base date for the programs.</param>
        /// <param name="channel">The channel for the data.</param>
        /// <param name="pid">The PID of the section.</param>
        /// <param name="table">The table ID of the section.</param> 
        internal void Process(byte[] byteData, int index, DateTime baseDate, int channel, int pid, int table)
        {
            lastIndex = index;

            this.pid = pid;
            this.table = table;
            this.baseDate = baseDate;

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

                timeStamp = DateTime.Now;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
        }

        /// <summary>
        /// Validate the title data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A title data field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the title data fields.
        /// </summary>
        public void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV TITLE DATA: Event ID: " + eventID +
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
                return(null);

            foreach (OpenTVRecordBase record in records)
            {
                if (record.Tag == tag)
                    return(record);
            }

            return(null);
        }
    }
}
