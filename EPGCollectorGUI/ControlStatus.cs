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

namespace EPGCentre
{
    /// <summary>
    /// The class that describes a controls status.
    /// </summary>
    public class ControlStatus
    {
        /// <summary>
        /// Get or set the current heading.
        /// </summary>
        public string Heading
        {
            get { return (heading); }
            set { heading = value; }
        }

        /// <summary>
        /// Get or set whether the data needs saving.
        /// </summary>
        public bool Dirty
        {
            get { return (dirty); }
            set { dirty = value; }
        }

        private string heading;
        private bool dirty;

        /// <summary>
        /// Initialize a new instance of the ControlStatus class.
        /// </summary>
        public ControlStatus() { }

        /// <summary>
        /// Initialize a new instance of the ControlStatus class.
        /// </summary>
        /// <param name="heading">The control heading.</param>
        public ControlStatus(string heading)
        {
            this.heading = heading;
        }
    }
}
