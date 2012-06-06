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
    /// The class that describes a MediaHighway2 Summary section.
    /// </summary>
    public class MediaHighway2SummarySection
    {
        /// <summary>
        /// Get the summary data for this section.
        /// </summary>
        public MediaHighway2SummaryData SummaryData { get { return (summaryData); } }

        private MediaHighway2SummaryData summaryData;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway2SummarySection class.
        /// </summary>
        internal MediaHighway2SummarySection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the summary data.</param>
        /// <param name="index">Index of the first byte of the summary data in the MPEG2 section.</param>
        /// <returns>True if the block contains data; false otherwise.</returns>
        internal bool Process(byte[] byteData, int index)
        {
            lastIndex = index;

            summaryData = new MediaHighway2SummaryData();
            return(summaryData.Process(byteData, lastIndex));
        }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            summaryData.LogMessage();
        }

        /// <summary>
        /// Process an MPEG2 section from the MediaHighway1 summary table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A MediaHighway2SummarySection instance.</returns>
        public static MediaHighway2SummarySection ProcessMediaHighwaySummaryTable(byte[] byteData)
        {
            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);

                MediaHighway2SummarySection summarySection = new MediaHighway2SummarySection();
                bool process = summarySection.Process(byteData, mpeg2Header.Index);
                if (process)
                {
                    summarySection.LogMessage();
                    return (summarySection);
                }
                else
                    return (null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Summary section parsing failed: " + e.Message);
                return (null);
            }
        }
    }
}
