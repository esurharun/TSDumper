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

namespace DomainObjects
{
    /// <summary>
    /// The class that describes Windows Media Center ATSC tuning information. 
    /// </summary>
    public class WMCATSCTuningInfo : WMCTuningInfo
    {
        /// <summary>
        /// Get the major channel number.
        /// </summary>
        public int MajorChannel { get { return (majorChannel); } }
        /// <summary>
        /// Get the minor channel number.
        /// </summary>
        public int MinorChannel { get { return (minorChannel); } }
        
        private int minorChannel;
        private int majorChannel;
        
        private WMCATSCTuningInfo() { }

        /// <summary>
        /// Initialize a new instance of the WMCATSCTuningInfo class.
        /// </summary>
        /// <param name="frequency">The transponder frequency.</param>
        /// <param name="majorChannel">The major channel number.</param>
        /// <param name="minorChannel">The minor channle number.</param>
        public WMCATSCTuningInfo(int frequency, int majorChannel, int minorChannel) : base(frequency)
        {
            this.majorChannel = majorChannel;
            this.minorChannel = minorChannel;
        }
    }
}
