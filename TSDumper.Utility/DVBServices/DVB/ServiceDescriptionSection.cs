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
    /// The class that describes a Service Description section.
    /// </summary>
    public class ServiceDescriptionSection
    {
        /// <summary>
        /// Get the original network identification (ONID).
        /// </summary>
        public int OriginalNetworkID { get { return (originalNetworkID); } }
        /// <summary>
        /// Get the tansport stream identification (TSID).
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }        
        /// <summary>
        /// Get the collection of service descriptions in the section.
        /// </summary>
        public Collection<ServiceDescription> ServiceDescriptions { get { return (serviceDescriptions); } }

        /// <summary>
        /// Get the section number.
        /// </summary>
        public int SectionNumber { get { return (sectionNumber); } }

        private int transportStreamID = -1;
        private int originalNetworkID = -1;
        private int reserved1;

        private int sectionNumber;

        private Collection<ServiceDescription> serviceDescriptions;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ServiceDescriptionSection class.
        /// </summary>
        internal ServiceDescriptionSection() 
        {
            serviceDescriptions = new Collection<ServiceDescription>(); 
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            transportStreamID = mpeg2Header.TableIDExtension;
            sectionNumber = mpeg2Header.SectionNumber;

            originalNetworkID = Utils.Convert2BytesToInt(byteData, lastIndex);
            lastIndex += 2;

            reserved1 = (int)byteData[lastIndex];
            lastIndex++;

            while (lastIndex < byteData.Length - 4)
            {
                ServiceDescription serviceDescription = new ServiceDescription();
                serviceDescription.Process(byteData, lastIndex);
                serviceDescriptions.Add(serviceDescription);

                lastIndex = serviceDescription.Index;
            }
        }

        /// <summary>
        /// Validate the section fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "SERVICE DESCRIPTION SECTION: TSID: " + transportStreamID +
                " ONID: " + originalNetworkID);
            
            if (serviceDescriptions != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (ServiceDescription serviceDescription in serviceDescriptions)
                    serviceDescription.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the service description table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A ServiceDescriptionSection instance.</returns>
        public static ServiceDescriptionSection ProcessServiceDescriptionTable(byte[] byteData)
        {
            if (RunParameters.Instance.DebugIDs.Contains("DUMPSDTBLOCK"))
                Logger.Instance.Dump("Service Description Block", byteData, byteData.Length);

            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    try
                    {
                        ServiceDescriptionSection serviceDescriptionSection = new ServiceDescriptionSection();
                        serviceDescriptionSection.Process(byteData, mpeg2Header);
                        serviceDescriptionSection.LogMessage();
                        return (serviceDescriptionSection);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Instance.Write(e.Message);
                        return (null);
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Service Description Section message: " + e.Message);
            }

            return (null);
        }
    }
}
