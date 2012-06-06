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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a single time offset entry.
    /// </summary>
    public class DVBLocalTimeOffsetEntry
    {
        /// <summary>
        /// Get or set the country code.
        /// </summary>
        public string CountryCode 
        { 
            get { return (countryCode); }
            set { countryCode = value; }
        }

        /// <summary>
        /// Get or set the region.
        /// </summary>
        public int Region 
        { 
            get { return (region); }
            set { region = value; }
        }

        /// <summary>
        /// Get or set the offset polarity.
        /// </summary>
        public bool OffsetPositive 
        { 
            get { return (offsetPositive); }
            set  { offsetPositive = value; }
        }

        /// <summary>
        /// Get or set the time offset.
        /// </summary>
        public TimeSpan TimeOffset 
        { 
            get { return (timeOffset); }
            set { timeOffset = value; }
        }

        /// <summary>
        /// Get or set the change time.
        /// </summary>
        public DateTime ChangeTime 
        { 
            get { return (changeTime); }
            set { changeTime = value; }
        }

        /// <summary>
        /// Get or set the next time offset.
        /// </summary>
        public TimeSpan NextTimeOffset 
        { 
            get { return (nextTimeOffset); }
            set { nextTimeOffset = value; }
        }

        private string countryCode;
        private int region;
        private bool offsetPositive;
        private TimeSpan timeOffset;
        private DateTime changeTime;
        private TimeSpan nextTimeOffset;

        internal DVBLocalTimeOffsetEntry() { }
    }
}
