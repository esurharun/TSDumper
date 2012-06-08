////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2010 nzsjb/ukkiwi                                    //
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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a bouquet.
    /// </summary>
    public class Bouquet
    {
        /// <summary>
        /// Get the collection of bouquets.
        /// </summary>
        public static Collection<Bouquet> Bouquets
        {
            get
            {
                if (bouquets == null)
                    bouquets = new Collection<Bouquet>();
                return (bouquets);
            }
        }

        /// <summary>
        /// Get or set the bouquet ID.
        /// </summary>
        public int BouquetID
        {
            get { return (bouquetID); }
            set { bouquetID = value; }
        }

        /// <summary>
        /// Get or set the bouquet name.
        /// </summary>
        public string Name
        {
            get { return (name); }
            set { name = value; }
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
        /// Get the collection of channels for the bouquet/region.
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

        private int bouquetID;
        private string name;
        private int region;

        private static Collection<Bouquet> bouquets;
        private Collection<Channel> channels;

        private Bouquet() { }

        /// <summary>
        /// Initialize a new instance of the Bouquet class.
        /// </summary>
        /// <param name="bouquetID">The bouquet ID.</param>
        /// <param name="name">The name of the bouquet.</param>
        /// <param name="region">The region within the bouquet.</param>
        public Bouquet(int bouquetID, string name, int region)
        {
            this.bouquetID = bouquetID;
            this.name = name;
            this.region = region;
        }

        /// <summary>
        /// Add a bouquet to the collection.
        /// </summary>
        /// <param name="newBouquet">The bouquet to be added.</param>
        public static void AddBouquet(Bouquet newBouquet)
        {
            foreach (Bouquet oldBouquet in Bouquets)
            {
                if (oldBouquet.BouquetID == newBouquet.BouquetID && oldBouquet.Region == newBouquet.Region)
                    return;
            }

            bouquets.Add(newBouquet);            
        }

        /// <summary>
        /// Find a bouquet.
        /// </summary>
        /// <param name="bouquetID">The bouquet ID.</param>
        /// <param name="region">The region within the bouquet.</param>
        /// <returns>A bouqet instance or null if the bouquet ID/region does not exist.</returns>
        public static Bouquet FindBouquet(int bouquetID, int region)
        {
            foreach (Bouquet bouquet in Bouquets)
            {
                if (bouquet.BouquetID == bouquetID && bouquet.Region == region)
                    return (bouquet);
            }

            return (null);
        }

        /// <summary>
        /// Get the bouquets in name sequence.
        /// </summary>
        /// <returns>The bouquets sorted in name order.</returns>
        public static Collection<Bouquet> GetBouquetsInNameOrder()
        {
            Collection<Bouquet> sortedBouquets = new Collection<Bouquet>();

            foreach (Bouquet bouquet in Bouquets)
                addBouquetInNameOrder(sortedBouquets, bouquet);

            return (sortedBouquets);
        }

        private static void addBouquetInNameOrder(Collection<Bouquet> sortedBouquets, Bouquet newBouquet)
        {
            foreach (Bouquet oldBouquet in sortedBouquets)
            {
                if (oldBouquet.Name.CompareTo(newBouquet.Name) > 0)
                {
                    sortedBouquets.Insert(sortedBouquets.IndexOf(oldBouquet), newBouquet);
                    return;
                }
            }

            sortedBouquets.Add(newBouquet); 
        }

        /// <summary>
        /// Add a channel to the bouquet/region.
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
