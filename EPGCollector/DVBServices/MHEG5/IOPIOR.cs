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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an IOP:IOR
    /// </summary>
    public class IOPIOR
    {
        /// <summary>
        /// Get the collection of BIOP profiles.
        /// </summary>
        public Collection<BIOPProfileBase> TaggedProfiles { get { return(taggedProfiles); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the IOP:IOR.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The IOP:IOR has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("IOPIOR: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int typeIDLength;
        private string typeID = "?";
        private int taggedProfilesCount;
        private Collection<BIOPProfileBase> taggedProfiles;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the IOPIOR class.
        /// </summary>
        public IOPIOR() { }

        /// <summary>
        /// Parse the IOP:IOR.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the IOP:IOR.</param>
        /// <param name="index">Index of the first byte of the IOP:IOR in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                typeIDLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                typeID = Utils.GetString(byteData, lastIndex, typeIDLength);
                lastIndex += typeIDLength;

                if ((typeIDLength % 4) != 0)
                {
                    for (int filler = 0; filler < (4 - (typeIDLength % 4)); filler++)
                        lastIndex++;
                }

                taggedProfilesCount = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (taggedProfilesCount != 0)
                {
                    taggedProfiles = new Collection<BIOPProfileBase>();

                    while (taggedProfiles.Count != taggedProfilesCount)
                    {
                        BIOPProfileBase taggedProfile = new BIOPProfileBase();
                        taggedProfile.Process(byteData, lastIndex);
                        taggedProfiles.Add(taggedProfile);

                        lastIndex = taggedProfile.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The IOP:IOR message is short"));
            }
        }

        /// <summary>
        /// Validate the IOP:IOR fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An IOP:IOR field is not valid.
        /// </exception>
        public void Validate()
        {
            if (taggedProfiles.Count == 0)
                throw (new ArgumentOutOfRangeException("There are no tagged profiles defined in the IOP:IOR"));
        }

        /// <summary>
        /// Log the IOP:IOR fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "IOPIOR: Type ID lth: " + typeIDLength +
                " Type ID: " + typeID +
                " Profiles count: " + taggedProfilesCount);

            if (taggedProfiles != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPProfileBase profileBase in taggedProfiles)
                    profileBase.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
