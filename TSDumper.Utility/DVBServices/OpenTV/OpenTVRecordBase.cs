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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The base OpenTV record class.
    /// </summary>
    internal class OpenTVRecordBase
    {
        /// <summary>
        /// Get the tag of the record.
        /// </summary>
        internal int Tag { get { return (tag); } }
        /// <summary>
        /// Get the length of the record data.
        /// </summary>
        internal int Length { get { return (length); } }
        /// <summary>
        /// Get the record data.
        /// </summary>
        internal byte[] Data { get { return (data); } }
        
        /// <summary>
        /// Get the total length of the record.
        /// </summary>
        internal int TotalLength { get { return (Length + 2); } }
        /// <summary>
        /// Return true if the record is undefined; false otherwise.
        /// </summary>
        internal bool IsUndefined { get { return (isUndefined); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following this record.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public virtual int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVRecordBase: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int lastIndex = -1;

        private int tag;
        private int length;
        private byte[] data;

        private bool isUndefined;

        /// <summary>
        /// Create an instance of the record class.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the record.</param>
        /// <param name="index">The index of the tag byte of the record.</param>
        /// <returns>A descriptor instance.</returns>
        internal static OpenTVRecordBase Instance(byte[] byteData, int index)
        {
            OpenTVRecordBase record;

            switch ((int)byteData[index])
            {
                case OpenTVTitleDataRecord.TagValue:
                    record = new OpenTVTitleDataRecord();
                    break;
                case OpenTVShortDescriptionRecord.TagValue:
                    record = new OpenTVShortDescriptionRecord();
                    break;
                case OpenTVExtendedDescriptionRecord.TagValue:
                    record = new OpenTVExtendedDescriptionRecord();
                    break;
                case OpenTVSeriesLinkRecord.TagValue:
                    record = new OpenTVSeriesLinkRecord();                    
                    break;
                default:
                    record = new OpenTVRecordBase();
                    break;
            }

            record.tag = (int)byteData[index];
            index++;

            record.length = (int)byteData[index];
            index++;

            record.Process(byteData, index);

            return (record);
        }

        /// <summary>
        /// Initialize a new instance of the OpenTVRecordBase class.
        /// </summary>
        internal OpenTVRecordBase() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the record.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal virtual void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            if (Length != 0)
            {
                data = Utils.GetBytes(byteData, 0, Length);
                lastIndex += Length;
            }

            isUndefined = true;
        }

        /// <summary>
        /// Validate the record fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A record field is not valid.
        /// </exception>
        internal virtual void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal virtual void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            if (!RunParameters.Instance.TraceIDs.Contains("GENERICOPENTVRECORD"))
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV GENERIC RECORD: Tag: " + Utils.ConvertToHex(tag) +
                " Length: " + length);

            if (length != 0)
                Logger.ProtocolLogger.Dump("OpenTV Generic Record Data", data, data.Length);
        }
    }
}
