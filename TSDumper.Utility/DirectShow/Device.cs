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

namespace DirectShow
{
    /// <summary>
    /// The class that describes a tuner device.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Get the name of the device.
        /// </summary>
        public string Name { get { return(name); } }
        /// <summary>
        /// Get the description of the device.
        /// </summary>
        public string Description { get { return (description); } }
        /// <summary>
        /// Get the path to the device.
        /// </summary>
        public string Path { get { return (path); } }

        private string name;
        private string description;
        private string path;

        private Device() { }

        /// <summary>
        /// Initialize a new instance of the Device clas.
        /// </summary>
        /// <param name="name">The name of the device.</param>
        /// <param name="description">The description of the device.</param>
        /// <param name="path">The path to the device.</param>
        public Device(string name, string description, string path)
        {
            this.name = name;
            this.description = description;
            this.path = path;
        }

        /// <summary>
        /// Get a description of the device.
        /// </summary>
        /// <returns>The description of the device.</returns>
        public override string ToString()
        {
            if (description != null)
                return (description);
            else
                return (name);
        }
    }
}
