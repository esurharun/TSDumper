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

using System.Collections.ObjectModel;

namespace DVBServices
{
    /// <summary>
    /// The class that describes MediaHighway summary.
    /// </summary>
    public class MediaHighwaySummary
    {
        /// <summary>
        /// Get the collection of MediaHighway summary entries.
        /// </summary>
        public static Collection<MediaHighwaySummary> Summaries
        {
            get
            {
                if (summaries == null)
                    summaries = new Collection<MediaHighwaySummary>();
                return (summaries);
            }
        }

        /// <summary>
        /// Get the event identification.
        /// </summary>
        public int EventID 
        { 
            get { return (eventID); }
            set { eventID = value; }
        }

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
            set { shortDescription = value; }
        }

        /// <summary>
        /// Get the number of repeats.
        /// </summary>
        public int ReplayCount
        {
            get { return (replayCount); }
            set { replayCount = value; }
        }

        /// <summary>
        /// Get or set the collection of replay definitions.
        /// </summary>
        public Collection<MediaHighway1Replay> Replays 
        { 
            get { return (replays); }
            set { replays = value; }
        }

        /// <summary>
        /// Get or set the unidentified data.
        /// </summary>
        public byte[] Unknown 
        { 
            get { return (unknown); }
            set { unknown = value; }
        }

        private static Collection<MediaHighwaySummary> summaries;

        private int eventID;
        private string shortDescription;
        private int replayCount;
        private Collection<MediaHighway1Replay> replays;
        private byte[] unknown;
        
        /// <summary>
        /// Initialize a new instance of the MediaHighwaySummaryData class.
        /// </summary>
        public MediaHighwaySummary() { }

        /// <summary>
        /// Add a summary to the collection.
        /// </summary>
        /// <param name="newSummary">The summary to be added.</param>
        public static void AddSummary(MediaHighwaySummary newSummary)
        {
            foreach (MediaHighwaySummary oldSummary in Summaries)
            {
                if (oldSummary.EventID == newSummary.EventID && oldSummary.ShortDescription == newSummary.ShortDescription)
                    return;

                if (oldSummary.EventID > newSummary.EventID)
                {
                    Summaries.Insert(Summaries.IndexOf(oldSummary), newSummary);
                    return;
                }
            }

            Summaries.Add(newSummary);
        }
    }
}
