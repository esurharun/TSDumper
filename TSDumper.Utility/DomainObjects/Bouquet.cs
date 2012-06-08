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

namespace DomainObjects
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
        /// Get the collection of regions for the bouquet.
        /// </summary>
        public Collection<Region> Regions
        {
            get
            {
                if (regions == null)
                    regions = new Collection<Region>();
                return (regions);
            }
        }

        private int bouquetID;
        private string name;
        private Collection<Region> regions;

        private static Collection<Bouquet> bouquets;        

        private Bouquet() { }

        /// <summary>
        /// Initialize a new instance of the Bouquet class.
        /// </summary>
        /// <param name="bouquetID">The bouquet ID.</param>
        /// <param name="name">The name of the bouquet.</param>
        public Bouquet(int bouquetID, string name)
        {
            this.bouquetID = bouquetID;
            this.name = name;            
        }

        /// <summary>
        /// Add a bouquet to the collection.
        /// </summary>
        /// <param name="newBouquet">The bouquet to be added.</param>
        public static void AddBouquet(Bouquet newBouquet)
        {
            if (newBouquet == null)
                throw (new ArgumentException("The bouquet cannot be null", "newBouquet"));

            foreach (Bouquet oldBouquet in Bouquets)
            {
                if (oldBouquet.BouquetID == newBouquet.BouquetID)
                    return;

                if (oldBouquet.BouquetID > newBouquet.BouquetID)
                {
                    bouquets.Insert(bouquets.IndexOf(oldBouquet), newBouquet);
                    return;
                }
            }

            bouquets.Add(newBouquet);            
        }

        /// <summary>
        /// Find a bouquet.
        /// </summary>
        /// <param name="bouquetId">The bouquet ID.</param>
        /// <returns>A bouqet instance or null if the bouquet ID/region does not exist.</returns>
        public static Bouquet FindBouquet(int bouquetId)
        {
            foreach (Bouquet bouquet in Bouquets)
            {
                if (bouquet.BouquetID == bouquetId)
                    return (bouquet);
            }

            return (null);
        }

        /// <summary>
        /// Add a region to the collection.
        /// </summary>
        /// <param name="newRegion">The region to be added.</param>
        public void AddRegion(Region newRegion)
        {
            foreach (Region oldRegion in Regions)
            {
                if (oldRegion.Code == newRegion.Code)
                    return;

                if (oldRegion.Code > newRegion.Code)
                {
                    regions.Insert(regions.IndexOf(oldRegion), newRegion);
                    return;
                }
            }

            regions.Add(newRegion);
        }

        /// <summary>
        /// Find a region.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <returns>A region instance or null if the region does not exist.</returns>
        public Region FindRegion(int regionCode)
        {
            foreach (Region region in Regions)
            {
                if (region.Code == regionCode)
                    return (region);
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
    }
}
