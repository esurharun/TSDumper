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

using System.Collections.ObjectModel;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a provider.
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Get the name of the provider.
        /// </summary>
        public string Name { get { return(name); } }

        /// <summary>
        /// Get the collection of frequencies from the provider.
        /// </summary>
        public Collection<TuningFrequency> Frequencies
        {
            get
            {
                if (frequencies == null)
                    frequencies = new Collection<TuningFrequency>();
                return (frequencies);
            }
        }

        private string name;
        private Collection<TuningFrequency> frequencies;

        /// <summary>
        /// Initialize a new instance of the Provider class.
        /// </summary>
        public Provider() { }

        /// <summary>
        /// Initialize a new instance of the Provider class. 
        /// </summary>
        /// <param name="name">The name of the provider.</param>
        public Provider(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Add a new frequency.
        /// </summary>
        /// <param name="newFrequency">The frequency to be added.</param>
        public void AddFrequency(TuningFrequency newFrequency)
        {
            foreach (TuningFrequency oldFrequency in Frequencies)
            {
                if (oldFrequency.Frequency == newFrequency.Frequency)
                    return;

                if (oldFrequency.Frequency > newFrequency.Frequency)
                {
                    Frequencies.Insert(Frequencies.IndexOf(oldFrequency), newFrequency);
                    return;
                }
            }

            Frequencies.Add(newFrequency);
        }

        /// <summary>
        /// Find a tuning frequency.
        /// </summary>
        /// <param name="frequency">The frequency to be searched for.</param>
        /// <returns>The tuning frequency or null if it cannot be located.</returns>
        public TuningFrequency FindFrequency(int frequency)
        {
            foreach (TuningFrequency tuningFrequency in Frequencies)
            {
                if (tuningFrequency.Frequency == frequency)
                    return (tuningFrequency);
            }

            return (null);
        }

        /// <summary>
        /// Get a string representing this instance.
        /// </summary>
        /// <returns>The description of this instance.</returns>
        public override string ToString()
        {
            return (name);
        }
    }
}
