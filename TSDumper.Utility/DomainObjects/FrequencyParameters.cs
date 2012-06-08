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
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Security.Principal;

namespace DomainObjects
{
    /// <summary>
    /// The class that holds the run parameters for a frequency.
    /// </summary>
    public class FrequencyParameters
    {
        /// <summary>
        /// Get the selected tuners.
        /// </summary>
        public Collection<int> SelectedTuners
        {
            get
            {
                if (selectedTuners == null)
                    selectedTuners = new Collection<int>();
                return (selectedTuners);
            }
        }

        /// <summary>
        /// Get or set the timeout for acquiring data for a frequency.
        /// </summary>
        /// <remarks>
        /// The default of 300 seconds will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public TimeSpan FrequencyTimeout
        {
            get { return (frequencyTimeout); }
            set { frequencyTimeout = value; }
        }

        /// <summary>
        /// Get or set the timeout for acquiring a signal lock and receiving station information.
        /// </summary>
        /// <remarks>
        /// The default of 10 seconds will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public TimeSpan LockTimeout
        {
            get { return (lockTimeout); }
            set { lockTimeout = value; }
        }

        /// <summary>
        /// Get or set the number of repeats for collections that cannot determine data complete.
        /// </summary>
        /// <remarks>
        /// The default of 5 will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public int Repeats
        {
            get { return (repeats); }
            set { repeats = value; }
        }

        /// <summary>
        /// Get or set the country code.
        /// </summary>
        public string CountryCode
        {
            get { return (countryCode); }
            set { countryCode = value; }
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
        /// Get or set the language code.
        /// </summary>
        public string LanguageCode
        {
            get { return (languageCode); }
            set { languageCode = value; }
        }

        /// <summary>
        /// Get the collection of options.
        /// </summary>
        public Collection<string> Options { get { return (options); } }

        /// <summary>
        /// Get or set the channel bouquet.
        /// </summary>
        public int ChannelBouquet
        {
            get { return (channelBouquet); }
            set { channelBouquet = value; }
        }

        /// <summary>
        /// Get or set the channel region.
        /// </summary>
        public int ChannelRegion
        {
            get { return (channelRegion); }
            set { channelRegion = value; }
        }

        /// <summary>
        /// Get or set the character set.
        /// </summary>
        public string CharacterSet
        {
            get { return (characterSet); }
            set { characterSet = value; }
        }

        /// <summary>
        /// Get or set the EIT PID number.
        /// </summary>
        public int EITPid
        {
            get { return (eitPid); }
            set { eitPid = value; }
        }

        /// <summary>
        /// Get or set the MHW1 PID numbers.
        /// </summary>
        public int[] MHW1Pids
        {
            get { return (mhw1Pids); }
            set { mhw1Pids = value; }
        }

        /// <summary>
        /// Get or set the MHW2 PID numbers.
        /// </summary>
        public int[] MHW2Pids
        {
            get { return (mhw2Pids); }
            set { mhw2Pids = value; }
        }

        private Collection<int> selectedTuners;
        
        private TimeSpan frequencyTimeout = new TimeSpan(0, 5, 0);
        private TimeSpan lockTimeout = new TimeSpan(0, 0, 10);
        private int repeats = 5;

        private string countryCode;
        private int region;
        private string languageCode;
        
        private int channelBouquet = -1;
        private int channelRegion = -1;

        private string characterSet;

        private int eitPid = -1;
        private int[] mhw1Pids;
        private int[] mhw2Pids;

        private Collection<string> options = new Collection<string>();
        
        /// <summary>
        /// Initialise a new instance of the FrequencyParameters class.
        /// </summary>
        public FrequencyParameters() { }
    }
}
