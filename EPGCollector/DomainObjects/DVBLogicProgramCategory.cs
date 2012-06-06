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
using System.Linq;
using System.Text;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a DVBLogic program category.
    /// </summary>
    public class DVBLogicProgramCategory
    {
        /// <summary>
        /// Check if a description contains valid DVBLogic program categories.
        /// </summary>
        /// <param name="description">The description to check.</param>
        /// <returns>True if the description is valid; false otherwise.</returns>
        public static bool CheckDescription(string description)
        {
            if (description == null || description.Length == 0)
                return (true);

            string[] descriptionParts = description.ToLowerInvariant().Trim().Split(new char[] { ',' });

            foreach (string descriptionPart in descriptionParts)
            {
                if (descriptionPart != "action" &&
                    descriptionPart != "comedy" &&
                    descriptionPart != "documentary" &&
                    descriptionPart != "drama" &&
                    descriptionPart != "educational" &&
                    descriptionPart != "horror" &&
                    descriptionPart != "kids" &&
                    descriptionPart != "movie" &&
                    descriptionPart != "music" &&
                    descriptionPart != "news" &&
                    descriptionPart != "reality" &&
                    descriptionPart != "romance" &&
                    descriptionPart != "scifi" &&
                    descriptionPart != "serial" &&
                    descriptionPart != "soap" &&
                    descriptionPart != "special" &&
                    descriptionPart != "sports" &&
                    descriptionPart != "thriller" &&
                    descriptionPart != "adult")
                    return (false);
            }

            return (true);
        }
    }
}
