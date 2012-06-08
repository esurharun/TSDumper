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
    /// The class that describes the program map section.
    /// </summary>
    public class ProgramMapSection
    {
        /// <summary>
        /// Get the service identification (SID).
        /// </summary>
        public int ServiceID { get { return(serviceID); } }
        /// <summary>
        /// Get the collection of descriptor objects for the service ID.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }
        /// <summary>
        /// Get the collection of stream information objects for the service ID.
        /// </summary>
        public Collection<StreamInfo> StreamInfos { get { return (streamInfos); } }

        private int serviceID = -1;
        private int pcrPID = -1;        

        private Collection<StreamInfo> streamInfos;
        private Collection<DescriptorBase> descriptors;

        private int lastIndex = -1;

        private ProgramMapSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            serviceID = mpeg2Header.TableIDExtension;
            
            pcrPID = ((byteData[lastIndex] & 0x1f) * 256) + byteData[lastIndex + 1];
            lastIndex += 2;

            int programInfoLength = ((byteData[lastIndex] & 0x0f) * 256) + byteData[lastIndex + 1];
            lastIndex += 2;

            if (programInfoLength != 0)
            {
                while (programInfoLength > 0)
                {
                    descriptors = new Collection<DescriptorBase>();

                    while (programInfoLength > 0)
                    {
                        DescriptorBase descriptor = DescriptorBase.Instance(byteData, lastIndex);

                        if (!descriptor.IsEmpty)
                        {
                            descriptors.Add(descriptor);

                            lastIndex += descriptor.TotalLength;
                            programInfoLength -= descriptor.TotalLength;
                        }
                        else
                        {
                            lastIndex += DescriptorBase.MinimumDescriptorLength;
                            programInfoLength -= DescriptorBase.MinimumDescriptorLength;
                        }
                    }
                }
            }

            streamInfos = new Collection<StreamInfo>();

            while (lastIndex < byteData.Length - 4)
            {
                StreamInfo streamInfo = new StreamInfo();
                streamInfo.Process(byteData, lastIndex);
                
                streamInfos.Add(streamInfo);

                lastIndex = streamInfo.Index;
            }

            Validate();
        }

        /// <summary>
        /// Validate the section fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage() { }

        /// <summary>
        /// Process an MPEG2 section from the program map table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A ProgramMapSection instance.</returns>
        public static ProgramMapSection ProcessProgramMapTable(byte[] byteData)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    ProgramMapSection programMapSection = new ProgramMapSection();
                    programMapSection.Process(byteData, mpeg2Header);
                    return (programMapSection);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Program Map Section message: " + e.Message);
            }

            return (null);
        }
    }
}
