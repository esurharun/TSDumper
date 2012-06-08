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
    /// The class that describes signal polarization.
    /// </summary>
    public class SignalPolarization
    {
        /// <summary>
        /// Get a collection of all the possible polarizations.
        /// </summary>
        public static Collection<SignalPolarization> Polarizations
        {
            get
            {
                Collection<SignalPolarization> polarizations = new Collection<SignalPolarization>();

                polarizations.Add(new SignalPolarization(LinearHorizontal));
                polarizations.Add(new SignalPolarization(LinearVertical));
                polarizations.Add(new SignalPolarization(CircularLeft));
                polarizations.Add(new SignalPolarization(CircularRight));                

                return (polarizations);
            }
        }

        /// <summary>
        /// Get or set the polarization.
        /// </summary>
        public string Polarization
        {
            get { return (polarization); }
            set
            {
                switch (value)
                {
                    case LinearHorizontal:
                    case LinearVertical:
                    case CircularLeft:
                    case CircularRight:
                        polarization = value;
                        break;
                    default:
                        throw (new ArgumentException("SignalPolarization given unknown value of " + value));
                }
            }
        }

        /// <summary>
        /// Get the single letter abbreviation for this instance.
        /// </summary>
        public string PolarizationAbbreviation
        {
            get
            {
                switch (polarization)
                {
                    case LinearHorizontal:
                        return ("H");
                    case LinearVertical:
                        return ("V");
                    case CircularLeft:
                        return ("L");
                    case CircularRight:
                        return ("R");
                    default:
                        throw (new ArgumentException("SignalPolarization given unknown value of " + polarization));
                }
            }
        }

        /// <summary>
        /// The value for a signal polarization of linear horizontal.
        /// </summary>
        public const string LinearHorizontal = "Linear Horizontal";
        /// <summary>
        /// The value for a signal polarization of linear vertical.
        /// </summary>
        public const string LinearVertical = "Linear Vertical";
        /// <summary>
        /// The value for a signal polarization of circular left.
        /// </summary>
        public const string CircularLeft = "Circular Left";
        /// <summary>
        /// The value for a signal polarization of circular right.
        /// </summary>
        public const string CircularRight = "Circular Right";

        private string polarization = LinearHorizontal;

        /// <summary>
        /// Initialize a new instance of the SignalPolarization class.
        /// </summary>
        public SignalPolarization() { }

        /// <summary>
        /// Initialize a new instance of the SignalPolarization class.
        /// </summary>
        /// <param name="polarization">The signal polarization.</param>
        public SignalPolarization(string polarization)
        {
            Polarization = polarization; 
        }

        /// <summary>
        /// Initialize a new instance of the SignalPolarization class.
        /// </summary>
        /// <param name="polarization">The signal polarization.</param>
        public SignalPolarization(char polarization)
        {
            switch (polarization)
            {
                case 'H':
                    Polarization = LinearHorizontal;
                    break;
                case 'V':
                    Polarization = LinearVertical;
                    break;
                case 'L':
                    Polarization = CircularLeft;
                    break;
                case 'R':
                    Polarization = CircularRight;
                    break;
                default:
                    Polarization = LinearHorizontal;
                    break;
            }
        }

        /// <summary>
        /// Check if a polarization is valid.
        /// </summary>
        /// <param name="polarization">The polarization to be checked.</param>
        /// <returns>True if the polarization is valid; false otherwise.</returns>
        public static bool IsValid(string polarization)
        {
            if (polarization.Trim().Length != 1)
                return (false);

            string checkValue = polarization.Trim().ToUpper();

            return (checkValue == "H" || checkValue == "V" || checkValue == "L" || checkValue == "R");
        }

        /// <summary>
        /// Get a description of this instance.
        /// </summary>
        /// <returns>A tring desribing this instance.</returns>
        public override string ToString()
        {
            return (polarization);
        }
    }
}
