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
    /// The class that describes MediaHighway2 summary data.
    /// </summary>
    public class MediaHighway2SummaryData
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
                if (shortDescription != null)
                    return (shortDescription);
                else
                    return ("No Synopsis Available");
            }
        }

        /// <summary>
        /// Get the unidentified data.
        /// </summary>
        public byte[] Unknown { get { return (unknown); } }

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
                    throw (new InvalidOperationException("MediaHighway2SummaryData: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int eventID;
        private string shortDescription;
        private byte[] unknown;        
        
        private int lastIndex = -1;

        int summaryLength;
        int lineCount;
        int lineLength;

        /// <summary>
        /// Initialize a new instance of the MediaHighway2SummaryData class.
        /// </summary>
        public MediaHighway2SummaryData() { }

        /// <summary>
        /// Parse the summary data.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the summary data.</param>
        /// <param name="index">Index of the first byte of the summary data in the MPEG2 section.</param>
        /// <returns>True if the block contains data; false otherwise.</returns>
        internal bool Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                eventID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                unknown = Utils.GetBytes(byteData, lastIndex, 9);
                lastIndex += unknown.Length;

                if (unknown[1] != 0x00)
                {
                    if (RunParameters.Instance.DebugIDs.Contains("MHW2UNKNOWN"))
                    {
                        Logger.Instance.Write("Index: " + index);
                        Logger.Instance.Dump("Unknown Data Block", byteData, byteData.Length);
                    }
                    return (false);
                }

                if (unknown[2] != 0x00 && unknown[2] != 0x01)
                {
                    if (RunParameters.Instance.DebugIDs.Contains("MHW2UNKNOWN"))
                    {
                        Logger.Instance.Write("Index: " + index);
                        Logger.Instance.Dump("Unknown Data Block", byteData, byteData.Length);
                    }
                    return (false);
                }

                summaryLength = (int)byteData[lastIndex];
                lastIndex++;

                if (summaryLength == 0)
                {
                    if (RunParameters.Instance.DebugIDs.Contains("MHW2UNKNOWN"))
                    {
                        Logger.Instance.Write("Index: " + index);
                        Logger.Instance.Dump("Summary Length Zero", byteData, byteData.Length);
                    }
                    return (false);
                }

                shortDescription = Utils.GetString(byteData, lastIndex, summaryLength, true);
                lastIndex += summaryLength;

                lineCount = byteData[lastIndex] & 0x0f;
                lastIndex++;

                while (lineCount > 0)
                {
                    lineLength = (int)byteData[lastIndex];
                    lastIndex++;

                    if (lineLength > 0)
                    {
                        shortDescription += " " + Utils.GetString(byteData, lastIndex, lineLength, true);
                        lastIndex += lineLength;
                    }

                    lineCount--;
                }

                Validate();

                return (true);
            }
            catch (IndexOutOfRangeException)
            {
                byte[] data = (Utils.GetBytes(byteData, 0, byteData.Length));
                Logger.Instance.Dump("Exception Data", data, data.Length);
                throw (new ArgumentOutOfRangeException("The MediaHighway2 Summary Data message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW2 SUMMARY DATA: Event ID: " + eventID +
                " Unknown: " + Utils.ConvertToHex(unknown) +
                " Short desc: " + shortDescription);
        }
    }
}
