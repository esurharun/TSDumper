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
using System.Xml;
using System.Globalization;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a Windows Media Center recording.
    /// </summary>
    public class WMCRecording
    {
        /// <summary>
        /// Get the collection of recordings.
        /// </summary>
        public static Collection<WMCRecording> Recordings { get { return (recordings); } }

        /// <summary>
        /// Get the title.
        /// </summary>
        public string Title { get { return (title); } }
        /// <summary>
        /// Get the description.
        /// </summary>
        public string Description { get { return (description); } }
        /// <summary>
        /// Get the start time.
        /// </summary>
        public DateTime StartTime { get { return (startTime); } }

        private string title;
        private string description;
        private DateTime startTime;

        private static Collection<WMCRecording> recordings;

        /// <summary>
        /// Initialize a new instance of the WMCRecording class.
        /// </summary>
        public WMCRecording() { }

        /// <summary>
        /// Load the xml data.
        /// </summary>
        /// <param name="reader">The xml reader for the file.</param>
        public void Load(XmlReader reader)
        {
            title = reader.GetAttribute("title");
            startTime = DateTime.Parse(reader.GetAttribute("startTime"), CultureInfo.InvariantCulture);
            description = reader.GetAttribute("description");
            
            if (recordings == null)
                recordings = new Collection<WMCRecording>();

            recordings.Add(this);  
        }
    }
}
