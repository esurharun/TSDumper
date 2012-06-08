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
    /// The class that describes an exclusion entry for repeat program checking.
    /// </summary>
    public class RepeatExclusion
    {
        /// <summary>
        /// Get or set the collection of exclusions.
        /// </summary>
        public static Collection<RepeatExclusion> Exclusions
        {
            get { return (exclusions); }
            set { exclusions = value; }
        }

        /// <summary>
        /// Get or set the phrases to ignore in program titles and descriptions.
        /// </summary>
        public static Collection<string> PhrasesToIgnore
        {
            get { return(phrasesToIgnore); }
            set { phrasesToIgnore = value; }
        }

        /// <summary>
        /// Get the title of the program.
        /// </summary>
        public string Title { get { return (title); } }
        /// <summary>
        /// Get the description of the program.
        /// </summary>
        public string Description { get { return (description); } }
        
        private string title;
        private string description;

        private static Collection<RepeatExclusion> exclusions;
        private static Collection<string> phrasesToIgnore;

        private RepeatExclusion() { }

        /// <summary>
        /// Initialize a new instance of the RepeatExclusion class.
        /// </summary>
        /// <param name="title">The title of the program.</param>
        /// <param name="description">The description of the program.</param>
        public RepeatExclusion(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }
}
