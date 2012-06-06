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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a channel title.
    /// </summary>
    public class MediaHighwayChannelTitle
    {
        /// <summary>
        /// Get the channel.
        /// </summary>
        public MediaHighwayChannel Channel { get { return (channel); } }
        /// <summary>
        /// Get the title data.
        /// </summary>
        public MediaHighwayTitle Title { get { return (title); } }

        private MediaHighwayChannel channel;
        private MediaHighwayTitle title;

        private MediaHighwayChannelTitle() { }

        /// <summary>
        /// Initialize a new instance of the MediaHighwayChannleTitle class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="title">The title.</param>
        public MediaHighwayChannelTitle(MediaHighwayChannel channel, MediaHighwayTitle title)
        {
            this.channel = channel;
            this.title = title;
        }
    }
}
