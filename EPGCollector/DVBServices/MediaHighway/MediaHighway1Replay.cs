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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a MediaHighway1 replay.
    /// </summary>
    public class MediaHighway1Replay
    {
        /// <summary>
        /// Get the replay channel.
        /// </summary>
        public int Channel { get { return (channel); } }
        /// <summary>
        /// Get the replay time.
        /// </summary>
        public DateTime ReplayTime { get { return (replayTime); } }
        /// <summary>
        /// Get the replay subtitles.
        /// </summary>
        public bool Subtitled { get { return (subtitled); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the replay data.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The summary data has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("MediaHighway1Replay: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int channel;
        private DateTime replayTime;
        private bool subtitled;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway1Replay class.
        /// </summary>
        public MediaHighway1Replay() { }

        /// <summary>
        /// Parse the replay data.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the summary data.</param>
        /// <param name="index">Index of the first byte of the summary data in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                channel = (int)byteData[lastIndex];
                lastIndex++;

                replayTime = getRepeatTime(byteData, lastIndex);
                lastIndex += 5;

                subtitled = (byteData[lastIndex] & 0x01) != 0;
                lastIndex++;
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Replay data is short"));
            }
            catch (OverflowException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Replay data cause an overflow exception"));
            }
            catch (ArithmeticException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Replay data cause an arithmetic exception"));
            }
        }

        private DateTime getRepeatTime(byte[] byteData, int index)
        {
            int startDate = Utils.Convert2BytesToInt(byteData, index);

            int year = (int)((startDate - 15078.2) / 365.25);
            int month = (int)(((startDate - 14956.1) - (int)(year * 365.25)) / 30.6001);
            int day = (startDate - 14956) - (int)(year * 365.25) - (int)(month * 30.6001);

            int adjust;

            if (month == 14 || month == 15)
                adjust = 1;
            else
                adjust = 0;

            year = year + 1900 + adjust;
            month = month - 1 - (adjust * 12);

            int hour1 = (int)byteData[index + 2] >> 4;
            int hour2 = (int)byteData[index + 2] & 0x0f;
            int hour = (hour1 * 10) + hour2;

            int minute1 = (int)byteData[index + 3] >> 4;
            int minute2 = (int)byteData[index + 3] & 0x0f;
            int minute = (minute1 * 10) + minute2;

            int second1 = (int)byteData[index + 4] >> 4;
            int second2 = (int)byteData[index + 4] & 0x0f;
            int second = (second1 * 10) + second2;

            DateTime startTime;

            try
            {
                startTime = new DateTime(year, month, day, hour, minute, second);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 replay start time element(s) are out of range"));
            }
            catch (ArgumentException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 replay start time element(s) result in a start time that is out of range"));
            }

            return (startTime);
        }

        /// <summary>
        /// Validate the replay data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A summary data field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the replay data fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW1 REPLAY DATA: Channel: " + channel +
                " Replay time: " + replayTime +
                " Subtitled: " + subtitled);
        }
    }
}
