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
using System.Collections.ObjectModel;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes the forward error correction.
    /// </summary>
    public class FECRate
    {
        /// <summary>
        /// The FEC of 1/2.
        /// </summary>
        public const string FECRate12 = "1/2";
        /// <summary>
        /// The FEC of 1/3.
        /// </summary>
        public const string FECRate13 = "1/3";
        /// <summary>
        /// The FEC of 1/4.
        /// </summary>
        public const string FECRate14 = "1/4";
        /// <summary>
        /// The FEC of 2/3.
        /// </summary>
        public const string FECRate23 = "2/3";
        /// <summary>
        /// The FEC of 2/5.
        /// </summary>
        public const string FECRate25 = "2/5";
        /// <summary>
        /// The FEC of 3/4.
        /// </summary>
        public const string FECRate34 = "3/4";
        /// <summary>
        /// The FEC of 3/5.
        /// </summary>
        public const string FECRate35 = "3/5";
        /// <summary>
        /// The FEC of 4/5.
        /// </summary>
        public const string FECRate45 = "4/5";
        /// <summary>
        /// The FEC of 5/11.
        /// </summary>
        public const string FECRate511 = "5/11";
        /// <summary>
        /// The FEC of 5/6.
        /// </summary>
        public const string FECRate56 = "5/6";
        /// <summary>
        /// The FEC of 6/7.
        /// </summary>
        public const string FECRate67 = "6/7";
        /// <summary>
        /// The FEC of 7/8.
        /// </summary>
        public const string FECRate78 = "7/8";
        /// <summary>
        /// The FEC of 8/9.
        /// </summary>
        public const string FECRate89 = "8/9";
        /// <summary>
        /// The FEC of 9/10.
        /// </summary>
        public const string FECRate910 = "9/10";
        /// <summary>
        /// The maximum FEC.
        /// </summary>
        public const string FECRateMax = "Max";

        /// <summary>
        /// Get all the FEC rates.
        /// </summary>
        public static Collection<FECRate> FECRates
        {
            get
            {
                Collection<FECRate> fecRates = new Collection<FECRate>();

                fecRates.Add(new FECRate(FECRate12));
                fecRates.Add(new FECRate(FECRate13));
                fecRates.Add(new FECRate(FECRate14));
                fecRates.Add(new FECRate(FECRate23));
                fecRates.Add(new FECRate(FECRate25));
                fecRates.Add(new FECRate(FECRate34));
                fecRates.Add(new FECRate(FECRate35));
                fecRates.Add(new FECRate(FECRate45));
                fecRates.Add(new FECRate(FECRate511));
                fecRates.Add(new FECRate(FECRate56));
                fecRates.Add(new FECRate(FECRate67));
                fecRates.Add(new FECRate(FECRate78));
                fecRates.Add(new FECRate(FECRate89));
                fecRates.Add(new FECRate(FECRate910));
                fecRates.Add(new FECRate(FECRateMax));


                return (fecRates);
            }
        }

        /// <summary>
        /// Get or set the FEC rate.
        /// </summary>
        public string Rate
        {
            get { return (fecRate); }
            set
            {
                switch (value)
                {
                    case FECRate12:
                    case FECRate13:
                    case FECRate14:
                    case FECRate23:
                    case FECRate25:
                    case FECRate34:
                    case FECRate35:
                    case FECRate45:
                    case FECRate511:
                    case FECRate56:
                    case FECRate67:
                    case FECRate78:
                    case FECRate89:
                    case FECRate910:
                    case FECRateMax:
                        fecRate = value;
                        break;
                    default:
                        throw (new ArgumentException("FECRate given unknown value of " + value));
                }
            }
        }

        private string fecRate = FECRate34;

        /// <summary>
        /// Initialize a new instance of the FECRate class.
        /// </summary>
        public FECRate() { }

        /// <summary>
        /// Initialize a new instance of the FECRate class.
        /// </summary>
        /// <param name="fecRate">The FEC rate to be set.</param>
        public FECRate(string fecRate)
        {
            Rate = fecRate; 
        }

        /// <summary>
        /// Return a string representation of this instance.
        /// </summary>
        /// <returns>A string describing this FEC rate.</returns>
        public override string ToString()
        {
            return (fecRate);
        }
    }
}
