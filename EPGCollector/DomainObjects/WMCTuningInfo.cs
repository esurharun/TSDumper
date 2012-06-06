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
    /// The base class for Windows Media Center tuning information.
    /// </summary>
    public abstract class WMCTuningInfo
    {
        /// <summary>
        /// Get the frequency.
        /// </summary>
        public int Frequency { get { return (frequency); } }

        private int frequency;

        /// <summary>
        /// Initialize a new instance of the WMCTuningInfo class.
        /// </summary>
        protected WMCTuningInfo() { }

        /// <summary>
        /// Initialize a new instance of the WMCTuningInfo class.
        /// </summary>
        /// <param name="frequency">The transponder frequency.</param>
        protected WMCTuningInfo(int frequency)
        {
            this.frequency = frequency;
        }
    }
}
