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
    /// The class that describes a satellite dish.
    /// </summary>
    public class SatelliteDish
    {
        /// <summary>
        /// Get the first set of default LNB settings.
        /// </summary>
        public static SatelliteDish FirstDefault
        {
            get
            {
                currentDefault = -1;
                return (Default);
            }
        }

        /// <summary>
        /// Get the next set of default LNB settings.
        /// </summary>
        public static SatelliteDish Default
        {
            get
            {
                if (defaultList == null)
                {
                    defaultList = new Collection<SatelliteDish>();

                    SatelliteDish default1 = new SatelliteDish();
                    default1.LNBLowBandFrequency = 9750000;
                    default1.LNBHighBandFrequency = 10600000;
                    default1.LNBSwitchFrequency = 11700000;
                    defaultList.Add(default1);

                    SatelliteDish default2 = new SatelliteDish();
                    default2.LNBLowBandFrequency = 10750000;
                    default2.LNBHighBandFrequency = 10750000;
                    default2.LNBSwitchFrequency = 0;
                    defaultList.Add(default2);

                    SatelliteDish default3 = new SatelliteDish();
                    default3.LNBLowBandFrequency = 11300000;
                    default3.LNBHighBandFrequency = 11300000;
                    default3.LNBSwitchFrequency = 0;
                    defaultList.Add(default3);                    

                    SatelliteDish default4 = new SatelliteDish();
                    default4.LNBLowBandFrequency = 10700000;
                    default4.LNBHighBandFrequency = 10700000;
                    default4.LNBSwitchFrequency = 0;
                    defaultList.Add(default4);

                    SatelliteDish default5 = new SatelliteDish();
                    default5.LNBLowBandFrequency = 10600000;
                    default5.LNBHighBandFrequency = 10600000;
                    default5.LNBSwitchFrequency = 0;
                    defaultList.Add(default5);

                    currentDefault = -1;
                }

                if (currentDefault < defaultList.Count - 1)
                    currentDefault++;
                else
                    currentDefault = 0;

                return (defaultList[currentDefault]);
            }
        }

        /// <summary>
        /// Get or set the low band frequency.
        /// </summary>
        public int LNBLowBandFrequency
        {
            get { return (lnbLowBandFrequency); }
            set { lnbLowBandFrequency = value; }
        }

        /// <summary>
        /// Get or set the high band frequency.
        /// </summary>
        public int LNBHighBandFrequency
        {
            get { return (lnbHighBandFrequency); }
            set { lnbHighBandFrequency = value; }
        }

        /// <summary>
        /// Get or set the switch frequency.
        /// </summary>
        public int LNBSwitchFrequency
        {
            get { return (lnbSwitchFrequency); }
            set { lnbSwitchFrequency = value; }
        }

        /// <summary>
        /// The string that controls disqec switching.
        /// </summary>
        public string DiseqcSwitch
        {
            get { return (diseqcSwitch); }
            set { diseqcSwitch = value; }
        }

        /// <summary>
        /// Get the default low band frequency.
        /// </summary>
        public const int DefaultLNBLowBandFrequency = 9750000;
        /// <summary>
        /// Get the default high band frequency.
        /// </summary>
        public const int DefaultLNBHighBandFrequency = 10750000;
        /// <summary>
        /// Get the default switch frequency.
        /// </summary>
        public const int DefaultLNBSwitchFrequency = 11700000;

        private int lnbLowBandFrequency = DefaultLNBLowBandFrequency;
        private int lnbHighBandFrequency = DefaultLNBHighBandFrequency;
        private int lnbSwitchFrequency = DefaultLNBSwitchFrequency;

        private string diseqcSwitch;
        
        private static Collection<SatelliteDish> defaultList;
        private static int currentDefault;

        /// <summary>
        /// Initialize a new instance of the SatelliteDish class.
        /// </summary>
        public SatelliteDish() { }

        /// <summary>
        /// Create a copy of this instance.
        /// </summary>
        /// <returns>A new instance with the same properties.</returns>
        public SatelliteDish Clone()
        {
            SatelliteDish dish = new SatelliteDish();

            dish.LNBLowBandFrequency = lnbLowBandFrequency;
            dish.LNBHighBandFrequency = lnbHighBandFrequency;
            dish.LNBSwitchFrequency = lnbSwitchFrequency;
            dish.DiseqcSwitch = diseqcSwitch;

            return (dish);
        }

        /// <summary>
        /// Check if this instance is equal to another.
        /// </summary>
        /// <param name="dish">The other instance.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public bool EqualTo(SatelliteDish dish)
        {
            if (lnbLowBandFrequency != dish.LNBLowBandFrequency)
                return (false);

            if (lnbHighBandFrequency != dish.LNBHighBandFrequency)
                return (false);

            if (lnbSwitchFrequency != dish.LNBSwitchFrequency)
                return (false);

            if (diseqcSwitch != dish.DiseqcSwitch)
                return (false);

            return (true);
        }

        /// <summary>
        /// Get a description of this dish.
        /// </summary>
        /// <returns>A string describing the dish.</returns>
        public override string ToString()
        {
            return (LNBLowBandFrequency + ":" + LNBHighBandFrequency + ":" + LNBSwitchFrequency);
        }
    }
}
