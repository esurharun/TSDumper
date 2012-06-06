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
    /// The class that describes any frequency that can be referenced by a channel number.
    /// </summary>
    public abstract class ChannelTuningFrequency : TuningFrequency
    {
        /// <summary>
        /// Get or set the channel number.
        /// </summary>
        public int ChannelNumber
        {
            get { return (channelNumber); }
            set { channelNumber = value; }
        }

        private int channelNumber;

        /// <summary>
        /// Create a new instance of the ChannelTuningFrequency class.
        /// </summary>
        public ChannelTuningFrequency() { }

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A new instance with the same properties as the old instance.</returns>
        public void Clone(ChannelTuningFrequency newFrequency)
        {
            base.Clone(newFrequency);

            newFrequency.ChannelNumber = channelNumber;
        }
    }
}
