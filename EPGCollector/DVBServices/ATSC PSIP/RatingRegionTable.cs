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
    /// The class that describes a Rating Region Table section.
    /// </summary>
    public class RatingRegionTable
    {
        /// <summary>
        /// Get the collection of Rating Regions.
        /// </summary>
        public static Collection<RatingRegion> Regions
        {
            get
            {
                if (regions == null)
                    regions = new Collection<RatingRegion>();
                return (regions);
            }
        }

        /// <summary>
        /// Get the protocol version.
        /// </summary>
        public int ProtocolVersion { get { return (protocolVersion); } }
        /// <summary>
        /// Get the region.
        /// </summary>
        public RatingRegion Region { get { return (region); } }

        private static Collection<RatingRegion> regions;

        private int protocolVersion;
        private RatingRegion region;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the RatingRegionTable class.
        /// </summary>
        public RatingRegionTable()
        {
            Logger.ProtocolIndent = "";
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;

            protocolVersion = (int)byteData[lastIndex];
            lastIndex++;

            region = new RatingRegion();
            region.Process(byteData, lastIndex, mpeg2Header.TableIDExtension & 0xff);                        

            Validate();
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "RATING REGION TABLE: Version: " + protocolVersion);

            Logger.IncrementProtocolIndent();
            region.LogMessage();
            Logger.DecrementProtocolIndent();            
        }

        /// <summary>
        /// Add a new region to the collection.
        /// </summary>
        /// <param name="newRegion">The region to be added.</param>
        public static void AddRegion(RatingRegion newRegion)
        {
            foreach (RatingRegion oldRegion in Regions)
            {
                if (oldRegion.Region == newRegion.Region)
                    return;

                if (oldRegion.Region > newRegion.Region)
                {
                    Regions.Insert(Regions.IndexOf(oldRegion), newRegion);
                    return;
                }
            }

            Regions.Add(newRegion);
        }

        internal static bool CheckComplete(int[] regions)
        {
            if (Regions.Count != regions.Length)
                return (false);

            for (int index = 0; index < regions.Length; index++)
            {
                if (Regions[index].Region != regions[index])
                    return (false);
            }

            return (true);
        }

        internal static void Clear()
        {
            regions = null;
        }
    }
}
