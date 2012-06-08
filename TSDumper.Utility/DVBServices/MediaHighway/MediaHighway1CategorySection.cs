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
    /// The class that describes a MediaHighway1 category section.
    /// </summary>
    public class MediaHighway1CategorySection
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

        private Collection<MediaHighwayCategoryEntry> categories;

        /// <summary>
        /// Initialize a new instance of the MediaHighway1CategorySection class.
        /// </summary>
        internal MediaHighway1CategorySection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="index">The index of the first byte of the data portion.</param>
        internal void Process(byte[] byteData, int index)
        {
            int descriptionIndex = index + 16;
            int categoryNumber = 0;

            int categoryIndex = 0;
            int categoryID = 0;

            try
            {
                while (descriptionIndex < byteData.Length)
                {
                    MediaHighwayCategoryEntry categoryEntry = new MediaHighwayCategoryEntry();

                    if (byteData[categoryID + 3] == categoryIndex)
                    {
                        categoryNumber = categoryID * 16;
                        categoryID++;
                    }

                    categoryEntry.Number = categoryNumber;                    
                    categoryEntry.Description = Utils.GetString(byteData, descriptionIndex, 15, true).Trim();
                    Categories.Add(categoryEntry);

                    descriptionIndex += 15;
                    categoryNumber++;
                    categoryIndex++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MediaHighway1 Category Section message is short"));
            }

            Validate();
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW1 CATEGORY SECTION");

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
        public static MediaHighway1CategorySection ProcessMediaHighwayCategoryTable(byte[] byteData)
        {
            if (RunParameters.Instance.DebugIDs.Contains("MHW1CATEGORYSECTIONS"))
                Logger.Instance.Dump("MHW1 Category Section", byteData, byteData.Length);

            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);

                MediaHighway1CategorySection categorySection = new MediaHighway1CategorySection();
                categorySection.Process(byteData, mpeg2Header.Index);
                categorySection.LogMessage();
                return (categorySection);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Category section parsing failed: " + e.Message);
                return (null);
            }
        }
    }
}
