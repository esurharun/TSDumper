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
    /// The class that describes a tuning frequency.
    /// </summary>
    public abstract class TuningFrequency
    {
        /// <summary>
        /// Get or set the collection of frequencies defined for this run.
        /// </summary>
       
        public static Collection<TuningFrequency> FrequencyCollection
        {
            get
            {
                if (frequencyCollection == null)
                    frequencyCollection = new Collection<TuningFrequency>();
                return (frequencyCollection);
            }
            set { frequencyCollection = value; }
        }

        /// <summary>
        /// Get or set the frequency.
        /// </summary>
        public int Frequency
        {
            get { return (frequency); }
            set { frequency = value; }
        }

        /// <summary>
        /// Get or set the type of data collection requested.
        /// </summary>
        public CollectionType CollectionType
        {
            get { return (collectionType); }
            set { collectionType = value; }
        }

        /// <summary>
        /// Get or set the DSMCC PID for this frequency.
        /// </summary>
        public int DSMCCPid
        {
            get { return (dsmccPid); }
            set { dsmccPid = value; }
        }

        /// <summary>
        /// Get or set the number of entries for this frequency.
        /// </summary>
        public int UsageCount
        {
            get { return (usageCount); }
            set { usageCount = value; }
        }

        /// <summary>
        /// Get or set the provider for this frequency.
        /// </summary>
        public Provider Provider
        {
            get { return (provider); }
            set { provider = value; }
        }

        /// <summary>
        /// Get or set the OpenTV Huffman code for this frequency.
        /// </summary>
        public string OpenTVCode
        {
            get { return (openTVCode); }
            set { openTVCode = value; }
        }

        /// <summary>
        /// Return true if at least one frequency requires EIT collection; false otherwise.
        /// </summary>
        public static bool HasEITFrequency { get { return (hasCollectionType(CollectionType.EIT)); } }        
        /// <summary>
        /// Return true if at least one frequency requires OpenTV collection; false otherwise.
        /// </summary>
        public static bool HasOpenTVFrequency { get { return (hasCollectionType(CollectionType.OpenTV)); } }

        /// <summary>
        /// Return true if at least one frequency requires MHEG5 collection; false otherwise.
        /// </summary>
        public static bool HasMHEG5Frequency { get { return (hasCollectionType(CollectionType.MHEG5)); } }
        /// <summary>
        /// Return true if at least one frequency requires MHEG5 collection and has a DSMCC PID defined; false otherwise.
        /// </summary>
        public static bool HasUsedMHEG5Frequency { get { return (hasUsedMHEG5Frequency()); } }

        /// <summary>
        /// Return true if there is at least one DVB satellite frequency; false otherwise.
        /// </summary>
        public static bool HasDVBSatelliteFrequency { get { return (hasDVBSatelliteFrequency()); } }
        /// <summary>
        /// Return true if there is at least one DVB terrestrial frequency; false otherwise.
        /// </summary>
        public static bool HasDVBTerrestrialFrequency { get { return (hasDVBTerrestrialFrequency()); } }
        /// <summary>
        /// Return true if there is at least one DVB cable frequency; false otherwise.
        /// </summary>
        public static bool HasDVBCableFrequency { get { return (hasDVBCableFrequency()); } }
        /// <summary>
        /// Return true if there is at least one ATSC frequency; false otherwise.
        /// </summary>
        public static bool HasAtscFrequency { get { return (hasAtscFrequency()); } }
        /// <summary>
        /// Return true if there is at least one Clear QAM frequency; false otherwise.
        /// </summary>
        public static bool HasClearQamFrequency { get { return (hasClearQamFrequency()); } }

        /// <summary>
        /// Return the tuner type required by this frequency. This is overridden by derived classes to return a specific type.
        /// </summary>
        public virtual TunerType TunerType { get { return(TunerType.Other); } }

        private int frequency;
        private int dsmccPid;
        private CollectionType collectionType;
        private int usageCount;
        private Provider provider;
        private string openTVCode;

        private static Collection<TuningFrequency> frequencyCollection;

        /// <summary>
        ///  Initialize a new instance of the TuningFrequency class.
        /// </summary>
        public TuningFrequency() { }
        
        /// <summary>
        /// Initialize a new instance of the TuningFrequency class for a specified frequency and collection type.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <param name="collectionType">The collection type.</param>
        public TuningFrequency(int frequency, CollectionType collectionType)
        {
            this.frequency = frequency;
            this.collectionType = collectionType;
        }

        /// <summary>
        /// Compare another tuning frequency with this one.
        /// </summary>
        /// <param name="compareFrequency">The tuning frequency to be compared to.</param>
        /// <returns>0 if the frequencies are equal, -1 if this instance is less, +1 otherwise.</returns>
        public virtual int CompareTo(object compareFrequency)
        {
            TuningFrequency tuningFrequency = compareFrequency as TuningFrequency;
            if (tuningFrequency != null)
                return(frequency.CompareTo(tuningFrequency.frequency));
            else
                throw (new ArgumentException("Object is not a TuningFrequency"));
        }

        /// <summary>
        /// Get a string representing this instance.
        /// </summary>
        /// <returns>The description of this instance.</returns>
        public override string ToString()
        {
            return (frequency.ToString());
        }

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public abstract TuningFrequency Clone();

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A new instance with the same properties as the old instance.</returns>
        protected void Clone(TuningFrequency newFrequency)
        {
            newFrequency.Frequency = frequency;
            newFrequency.Provider = provider;
            newFrequency.CollectionType = collectionType;
            newFrequency.OpenTVCode = openTVCode;
        }

        /// <summary>
        /// Check this frequency for equality with another.
        /// </summary>
        /// <param name="tuningFrequency">The other frequency.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public virtual bool EqualTo(TuningFrequency tuningFrequency)
        {
            if (frequency != tuningFrequency.Frequency)
                return (false);

            if (provider != tuningFrequency.Provider)
                return (false);

            if (collectionType != tuningFrequency.CollectionType)
                return (false);

            return (true);
        }

        /// <summary>
        /// Compare another object with this one.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the objects are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            TuningFrequency otherFrequency = obj as TuningFrequency;
            if (otherFrequency == null)
                return (false);

            return (EqualTo(otherFrequency));
        }

        /// <summary>
        /// Get a hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return(ToString().GetHashCode());
        }

        /// <summary>
        /// Find a frequency.
        /// </summary>
        /// <param name="frequency">The frequency to be located.</param>
        /// <returns>The frequency instance or null if it cannot be located.</returns>
        public static TuningFrequency FindFrequency(int frequency)
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.Frequency == frequency)
                    return (tuningFrequency);
            }

            return (null);
        }

        private static bool hasCollectionType(CollectionType collectionType)
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.CollectionType == collectionType)
                    return (true);
            }

            return (false);
        }

        private static bool hasUsedMHEG5Frequency()
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.CollectionType == CollectionType.MHEG5 && tuningFrequency.DSMCCPid != 0)
                    return (true);
            }

            return (false);
        }

        private static bool hasDVBSatelliteFrequency()
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.Satellite)
                    return (true);
            }

            return (false);
        }

        private static bool hasDVBTerrestrialFrequency()
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.Terrestrial)
                    return (true);
            }

            return (false);
        }

        private static bool hasDVBCableFrequency()
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.Cable)
                    return (true);
            }

            return (false);
        }

        private static bool hasAtscFrequency()
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.ATSC)
                    return (true);
            }

            return (false);
        }

        private static bool hasClearQamFrequency()
        {
            foreach (TuningFrequency tuningFrequency in FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.ClearQAM)
                    return (true);
            }

            return (false);
        }
    }
}
