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
    /// The class that describes a DVBViewer program category.
    /// </summary>
    public class DVBViewerProgramCategory
    {
        /// <summary>
        /// Check if a description contains a valid DVBViewer program category.
        /// </summary>
        /// <param name="description">The description to check.</param>
        /// <returns>True if the description is valid; false otherwise.</returns>
        public static bool CheckDescription(string description)
        {
            if (description == null || description.Length == 0)
                return (true);

            string[] descriptionParts = description.Trim().Split(new char[] { ',' });
            if (descriptionParts.Length != 2)
                return (false);

            try
            {
                int contentCode = Int32.Parse(descriptionParts[0].Trim());
                int subContentCode = Int32.Parse(descriptionParts[1].Trim());

                switch (contentCode)
                {
                    case 0:
                        return (subContentCode == 0);
                    case 1:
                        return (subContentCode >= 0 && subContentCode <= 8);
                    case 2:
                        return (subContentCode >= 0 && subContentCode <= 4);
                    case 3:
                        return (subContentCode >= 0 && subContentCode <= 3);
                    case 4:
                        return (subContentCode >= 0 && subContentCode <= 11);
                    case 5:
                        return (subContentCode >= 0 && subContentCode <= 5);
                    case 6:
                        return (subContentCode >= 0 && subContentCode <= 6);
                    case 7:
                        return (subContentCode >= 0 && subContentCode <= 11);
                    case 8:
                        return (subContentCode >= 0 && subContentCode <= 3);
                    case 9:
                        return (subContentCode >= 0 && subContentCode <= 7);
                    case 10:
                        return (subContentCode >= 0 && subContentCode <= 7);
                    case 11:
                        return (subContentCode >= 0 && subContentCode <= 3);
                    default:
                        return (false);
                }
            }
            catch (FormatException)
            {
                return (false);
            }
            catch (ArithmeticException)
            {
                return (false);
            }
        }
    }
}
