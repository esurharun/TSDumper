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
using System.Linq;
using System.Text;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a time offset channel.
    /// </summary>
    public class TimeOffsetChannel
    {
        /// <summary>
        /// Get the collection of time offset channels.
        /// </summary>
        public static Collection<TimeOffsetChannel> Channels 
        { 
            get 
            {
                if (channels == null)
                    channels = new Collection<TimeOffsetChannel>();
                return (channels); 
            } 
        }

        /// <summary>
        /// Get the source channel.
        /// </summary>
        public TVStation SourceChannel { get { return(sourceChannel); } }
        /// <summary>
        /// Get the destination channel.
        /// </summary>
        public TVStation DestinationChannel { get { return (destinationChannel); } }
        /// <summary>
        /// Get the offset in hours.
        /// </summary>
        public int Offset { get { return (offset); } }

        private TVStation sourceChannel;
        private TVStation destinationChannel;
        private int offset;

        private static Collection<TimeOffsetChannel> channels;

        private TimeOffsetChannel() { }

        /// <summary>
        /// Initialize a new instance of the TimeOffsetChannel.
        /// </summary>
        /// <param name="sourceChannel">The source channel.</param>
        /// <param name="destinationChannel">The destination channel.</param>
        /// <param name="offset">The time offset in hours.</param>
        public TimeOffsetChannel(TVStation sourceChannel, TVStation destinationChannel, int offset)
        {
            this.sourceChannel = sourceChannel;
            this.destinationChannel = destinationChannel;
            this.offset = offset;
        }
    }
}
