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
    /// The class that describes MediaHighway1 summary data.
    /// </summary>
    public class MediaHighway1SummaryData
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
        /// Get the event identification.
        /// </summary>
        public int ReplayCount 
        { 
            get 
            {
                if (replays == null)
                    return (0);
                else
                    return (replays.Count); 
            } 
        }

        /// <summary>
        /// Get the collection of replay definitions.
        /// </summary>
        public Collection<MediaHighway1Replay> Replays { get { return (replays); } }

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
                    throw (new InvalidOperationException("MediaHighway1SummaryData: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int eventID;
        private string shortDescription;
        private byte[] unknown;
        private int replayCount;
        
        private Collection<MediaHighway1Replay> replays;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway1SummaryData class.
        /// </summary>
        public MediaHighway1SummaryData() { }

        /// <summary>
        /// Parse the summary data.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the summary data.</param>
        /// <param name="index">Index of the first byte of the summary data in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                eventID = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                unknown = Utils.GetBytes(byteData, lastIndex, 3);
                lastIndex += 3;

                replayCount = (int)byteData[lastIndex];
                lastIndex++;

                if (replayCount != 0)
                {
                    replays = new Collection<MediaHighway1Replay>();

                    int repeatLoop = 0;

                    while (repeatLoop < replayCount)
                    {
                        MediaHighway1Replay replay = new MediaHighway1Replay();
                        replay.Process(byteData, lastIndex);
                        replays.Add(replay);

                        lastIndex = replay.Index;
                        repeatLoop++;
                    }
                }
                
                shortDescription = Utils.GetString(byteData, lastIndex, byteData.Length - lastIndex, true);
                lastIndex += (byteData.Length - lastIndex);                

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Summary Data message is short"));
            }
            catch (OverflowException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Summary Data message cause an overflow exception"));
            }
            catch (ArithmeticException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Summary Data message cause an arithmetic exception"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW1 SUMMARY DATA: Event ID: " + eventID +
                " Unknown: " + Utils.ConvertToHex(unknown) +
                " Replays: " + replayCount +
                " Short desc: " + shortDescription);

            if (replays != null)
            {
                foreach (MediaHighway1Replay replay in replays)
                {
                    Logger.IncrementProtocolIndent();
                    replay.LogMessage();
                    Logger.DecrementProtocolIndent();
                }
            }
        }
    }
}
