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

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a region.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Get the name of the region.
        /// </summary>
        public string Name { get { return (name); } }

        /// <summary>
        /// Get the code of the region.
        /// </summary>
        public int Code { get { return (code); } }

        /// <summary>
        /// Get the collection of channels for the region.
        /// </summary>
        public Collection<Channel> Channels
        {
            get
            {
                if (channels == null)
                    channels = new Collection<Channel>();
                return (channels);
            }
        }

        private string name;
        private int code;
        private Collection<Channel> channels;
        
        private Region() { }

        /// <summary>
        /// Initialize a new instance of the Region class.
        /// </summary>
        /// <param name="name">The name of the region.</param>
        /// <param name="code">The code of the region.</param> 
        public Region(string name, int code)
        {
            this.name = name;
            this.code = code;
        }

        /// <summary>
        /// Return a description of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (name);
        }

        /// <summary>
        /// Add a channel to the region.
        /// </summary>
        /// <param name="newChannel">The channel to be added.</param>
        public void AddChannel(Channel newChannel)
        {
            foreach (Channel oldChannel in Channels)
            {
                if (oldChannel.OriginalNetworkID == newChannel.OriginalNetworkID &&
                    oldChannel.TransportStreamID == newChannel.TransportStreamID &&
                    oldChannel.ServiceID == newChannel.ServiceID &&
                    oldChannel.ChannelID == newChannel.ChannelID)
                    return;

                if (oldChannel.OriginalNetworkID == newChannel.OriginalNetworkID)
                {
                    if (oldChannel.TransportStreamID == newChannel.TransportStreamID)
                    {
                        if (oldChannel.ServiceID == newChannel.ServiceID)
                        {
                            if (oldChannel.ChannelID > newChannel.ChannelID)
                            {
                                Channels.Insert(Channels.IndexOf(oldChannel), newChannel);
                                return;
                            }
                        }
                        else
                        {
                            if (oldChannel.ServiceID > newChannel.ServiceID)
                            {
                                Channels.Insert(Channels.IndexOf(oldChannel), newChannel);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (oldChannel.TransportStreamID > newChannel.TransportStreamID)
                        {
                            Channels.Insert(Channels.IndexOf(oldChannel), newChannel);
                            return;
                        }
                    }
                }
                else
                {
                    if (oldChannel.OriginalNetworkID > newChannel.OriginalNetworkID)
                    {
                        Channels.Insert(Channels.IndexOf(oldChannel), newChannel);
                        return;
                    }
                }
            }

            Channels.Add(newChannel);
        }

        /// <summary>
        /// Get the channels in name order.
        /// </summary>
        /// <returns>The channels sorted in name order.</returns>
        public Collection<Channel> GetChannelsInNameOrder()
        {
            Collection<Channel> sortedChannels = new Collection<Channel>();

            foreach (Channel channel in Channels)
                addChannelInNameOrder(sortedChannels, channel);

            return (sortedChannels);
        }

        private void addChannelInNameOrder(Collection<Channel> sortedChannels, Channel newChannel)
        {
            TVStation newStation = TVStation.FindStation(newChannel.OriginalNetworkID, newChannel.TransportStreamID, newChannel.ServiceID);
            if (newStation == null)
                newStation = new TVStation("No Name");

            foreach (Channel oldChannel in sortedChannels)
            {
                TVStation oldStation = TVStation.FindStation(oldChannel.OriginalNetworkID, oldChannel.TransportStreamID, oldChannel.ServiceID);
                if (oldStation == null)
                    oldStation = new TVStation("No Name");

                if (oldStation.Name.CompareTo(newStation.Name) > 0)
                {
                    sortedChannels.Insert(sortedChannels.IndexOf(oldChannel), newChannel);
                    return;
                }
            }

            sortedChannels.Add(newChannel);
        }

        /// <summary>
        /// Get the channels in user channel number order.
        /// </summary>
        /// <returns>The channels sorted in channel number order.</returns>
        public Collection<Channel> GetChannelsInChannelNumberOrder()
        {
            Collection<Channel> sortedChannels = new Collection<Channel>();

            foreach (Channel channel in Channels)
                addChannelInChannelNumberOrder(sortedChannels, channel);

            return (sortedChannels);
        }

        private void addChannelInChannelNumberOrder(Collection<Channel> sortedChannels, Channel newChannel)
        {
            foreach (Channel oldChannel in sortedChannels)
            {
                if (oldChannel.UserChannel > newChannel.UserChannel)
                {
                    sortedChannels.Insert(sortedChannels.IndexOf(oldChannel), newChannel);
                    return;
                }
            }

            sortedChannels.Add(newChannel);
        }

        /// <summary>
        /// Find a channel.
        /// </summary>
        /// <param name="originalNetworkID">The ONID of the channel.</param>
        /// <param name="transportStreamID">The TSID of the channel.</param>
        /// <param name="serviceID">The SID of the channel.</param>
        /// <returns>A Channel instance if the channel can be located; null otherwise.</returns>
        public Channel FindChannel(int originalNetworkID, int transportStreamID, int serviceID)
        {
            foreach (Channel channel in Channels)
            {
                if (channel.OriginalNetworkID == originalNetworkID &&
                    channel.TransportStreamID == transportStreamID &&
                    channel.ServiceID == serviceID)
                    return (channel);
            }

            return (null);
        }
    }
}
