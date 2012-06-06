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
    /// The class that describes Windows Media Center DVB tuning information. 
    /// </summary>
    public class WMCDVBTuningInfo : WMCTuningInfo
    {
        /// <summary>
        /// Get the ONID.
        /// </summary>
        public int ONID { get { return (onid); } }
        /// <summary>
        /// Get the TSID.
        /// </summary>
        public int TSID { get { return (tsid); } }
        /// <summary>
        /// Get the SID.
        /// </summary>
        public int SID { get { return (sid); } }

        private int onid;
        private int tsid;
        private int sid;

        private WMCDVBTuningInfo() { }

        /// <summary>
        /// Initialize a new instance of the WMCDVBTuningInfo class.
        /// </summary>
        /// <param name="frequency">The transponder frequency.</param>
        /// <param name="onid">The ONID.</param>
        /// <param name="tsid">The TSID.</param>
        /// <param name="sid">The SID</param>
        public WMCDVBTuningInfo(int frequency, int onid, int tsid, int sid) : base(frequency)
        {
            this.onid = onid;
            this.tsid = tsid;
            this.sid = sid;
        }
    }
}
