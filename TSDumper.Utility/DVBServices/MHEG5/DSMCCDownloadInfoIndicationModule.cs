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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a DSMCC donload info indication module.
    /// </summary>
    public class DSMCCDownloadInfoIndicationModule
    {
        /// <summary>
        /// Get the module ID.
        /// </summary>
        public int ModuleID { get { return (moduleID); } }
        /// <summary>
        /// Get the module size.
        /// </summary>
        public int ModuleSize { get { return (moduleSize); } }
        /// <summary>
        /// Get the module version.
        /// </summary>
        public int ModuleVersion { get { return (moduleVersion); } }
        /// <summary>
        /// Get the length of the module information.
        /// </summary>
        public int ModuleInfoLength { get { return (moduleInfoLength); } }
        /// <summary>
        /// Get the module information.
        /// </summary>
        public BIOPModuleInfo BIOPModuleInformation { get { return (biopModuleInformation); } }

        /// <summary>
        /// Get the original size of the module before compression.
        /// </summary>
        public int OriginalSize
        {
            get
            {
                if (biopModuleInformation == null)
                    return (moduleSize);
                else
                {
                    if (biopModuleInformation.Descriptors == null)
                        return (moduleSize);
                    else
                    {
                        foreach (BIOPDescriptor descriptor in biopModuleInformation.Descriptors)
                        {
                            DVBCompressedModuleDescriptor compressedDescriptor = descriptor as DVBCompressedModuleDescriptor;
                            if (compressedDescriptor != null)
                                return (compressedDescriptor.OriginalSize);
                        }
                    }
                }

                return (moduleSize);
            }
        }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the module.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The module has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCDownloadInfoIndicationModule: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int moduleID;
        private int moduleSize;
        private int moduleVersion;
        private int moduleInfoLength;
        private BIOPModuleInfo biopModuleInformation;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCDownloadInfoIndicationModule class.
        /// </summary>
        public DSMCCDownloadInfoIndicationModule() { }

        /// <summary>
        /// Parse the module.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the module.</param>
        /// <param name="index">Index of the first byte of the module in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                moduleID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                moduleSize = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                moduleVersion = (int)byteData[lastIndex];
                lastIndex++;

                moduleInfoLength = (int)byteData[lastIndex];
                lastIndex++;

                if (moduleInfoLength != 0)
                {
                    biopModuleInformation = new BIOPModuleInfo();
                    biopModuleInformation.Process(byteData, lastIndex);
                    lastIndex = biopModuleInformation.Index;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Download Info Indication Module message is short"));
            }
        }

        /// <summary>
        /// Validate the module.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The module information is not valid.
        /// </exception>
        public void Validate()
        {
            if (moduleSize < 1)
                throw (new ArgumentOutOfRangeException("Module size is incorrect"));

            if (moduleInfoLength < 1)
                throw (new ArgumentOutOfRangeException("Module info length is incorrect"));
        }

        /// <summary>
        /// Log the module fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSMCC DOWNLOAD INFO INDICATION MODULE: Module ID: " + moduleID +
                " Module size: " + moduleSize +
                " Module ver: " + moduleVersion +
                " Info lth: " + moduleInfoLength);

            if (biopModuleInformation != null)
            {
                Logger.IncrementProtocolIndent();
                biopModuleInformation.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }
    }
}
