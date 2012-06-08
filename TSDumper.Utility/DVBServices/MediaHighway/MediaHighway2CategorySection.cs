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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a MediaHighway2 category section.
    /// </summary>
    public class MediaHighway2CategorySection
    {
        /// <summary>
        /// Get the collection of categories.
        /// </summary>
        public Collection<MediaHighwayCategoryEntry> Categories
        {
            get
            {
                if (categories == null)
                    categories = new Collection<MediaHighwayCategoryEntry>();
                return (categories);
            }
        }

        private int categoryCount;

        private Collection<MediaHighwayCategoryEntry> categories;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway2CategorySection class.
        /// </summary>
        internal MediaHighway2CategorySection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="index">The index of the first byte of the data portion.</param>
        /// <returns>True if the section is an MHW2 category section; false otherwise.</returns>
        internal bool Process(byte[] byteData, int index)
        {
            lastIndex = index;

            if (byteData[lastIndex] != 0x01)
                return (false);

            lastIndex++;

            categoryCount = (int)byteData[lastIndex];
            lastIndex++;

            try
            {
                int p1 = 0;
                int p2 = 0;

                string themeName = null;

                for (int themeIndex = 0; themeIndex < categoryCount; themeIndex++)
                {
                    p1 = ((byteData[lastIndex + (themeIndex * 2)] << 8) | byteData[lastIndex + 1 + (themeIndex * 2)]) + 3;

                    for (int descriptionIndex = 0; descriptionIndex <= (byteData[p1] & 0x3f); descriptionIndex++)
                    {
                        p2 = ((byteData[p1 + 1 + (descriptionIndex * 2)] << 8) | byteData[p1 + 2 + (descriptionIndex * 2)] + 3);

                        MediaHighwayCategoryEntry categoryEntry = new MediaHighwayCategoryEntry();
                        
                        categoryEntry.Number = ((themeIndex & 0x3f) << 6) | (descriptionIndex & 0x3f);
                        if (descriptionIndex == 0)
                        {
                            themeName = Utils.GetString(byteData, p2 + 1, byteData[p2] & 0x1f, true).Trim();
                            categoryEntry.Description = themeName;
                        }
                        else
                            categoryEntry.Description = themeName + " " + Utils.GetString(byteData, p2 + 1, byteData[p2] & 0x1f, true).Trim();

                        Categories.Add(categoryEntry);
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway2 Category Section message is short"));
            }            

            Validate();

            return (true);
        }



        /// <summary>
        /// Validate the section fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A section field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW2 CATEGORY SECTION");

            if (categories != null)
            {
                foreach (MediaHighwayCategoryEntry categoryEntry in categories)
                {
                    Logger.IncrementProtocolIndent();
                    categoryEntry.LogMessage();
                    Logger.DecrementProtocolIndent();
                }
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the Open TV Title table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        public static MediaHighway2CategorySection ProcessMediaHighwayCategoryTable(byte[] byteData)
        {
            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);

                MediaHighway2CategorySection categorySection = new MediaHighway2CategorySection();
                bool process = categorySection.Process(byteData, mpeg2Header.Index);
                if (process)
                {
                    categorySection.LogMessage();
                    return (categorySection);
                }
                else
                    return (null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Category section parsing failed: " + e.Message);
                return (null);
            }
        }
    }
}
