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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a MediaHighway category entry.
    /// </summary>
    public class MediaHighwayCategoryEntry
    {
        /// <summary>
        /// Get or set the category number.
        /// </summary>
        public int Number
        {
            get { return (number); }
            set { number = value; }
        }

        /// <summary>
        /// Get or set the category description.
        /// </summary>
        public string Description
        {
            get { return (description); }
            set { description = value; }
        }

        private int number;
        private string description;

        /// <summary>
        /// Initialize a new instance of the MediaHighwayCategoryEntry class.
        /// </summary>
        public MediaHighwayCategoryEntry() { }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW CATEGORY ENTRY: Number: " + number +
                " Description: " + description);
        }
    }
}
