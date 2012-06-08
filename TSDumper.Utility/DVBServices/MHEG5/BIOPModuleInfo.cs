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
    /// The class that describes the BIOP module information.
    /// </summary>
    public class BIOPModuleInfo
    {
        /// <summary>
        /// Get the module timeout.
        /// </summary>
        public int ModuleTimeout { get { return (moduleTimeout); } }
        /// <summary>
        /// Get the block timeout.
        /// </summary>
        public int BlockTimeout { get { return (blockTimeout); } }
        /// <summary>
        /// Get the minimum block time.
        /// </summary>
        public int MinimumBlockTime { get { return (minimumBlockTime); } }
        
        /// <summary>
        /// Get the collection of Taps for the module.
        /// </summary>
        public Collection<BIOPTap> Taps { get { return (taps); } }
        /// <summary>
        /// Get the collection of descriptors for the module.
        /// </summary>
        public Collection<BIOPDescriptor> Descriptors { get { return (descriptors); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the module information.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The module information has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPModuleInfo: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int moduleTimeout;
        private int blockTimeout;
        private int minimumBlockTime;
        private int tapsCount;
        private int userInfoLength;    

        private Collection<BIOPTap> taps;
        private Collection<BIOPDescriptor> descriptors;

        private int lastIndex = -1;

        private const int DVBDescriptorTagCompressedModule = 0x09;
        private const int MHPDescriptorTagLabel = 0x09;
        private const int MHPDescriptorTagLCachingPriority = 0x09;
        private const int MHPDescriptorTagContentType = 0x09;

        /// <summary>
        /// Initialize a new instance of the BIOPModuleInfo class.
        /// </summary>
        public BIOPModuleInfo() { }

        /// <summary>
        /// Parse the module information.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the module information.</param>
        /// <param name="index">Index of the first byte of the module information in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                moduleTimeout = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                blockTimeout = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                minimumBlockTime = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                tapsCount = (int)byteData[lastIndex];
                lastIndex++;

                if (tapsCount != 0)
                {
                    taps = new Collection<BIOPTap>();

                    while (taps.Count < tapsCount)
                    {
                        BIOPTap tap = new BIOPTap();
                        tap.Process(byteData, lastIndex);
                        taps.Add(tap);

                        lastIndex = tap.Index;
                    }
                }

                userInfoLength = (int)byteData[lastIndex];
                lastIndex++;

                if (userInfoLength != 0)
                {
                    descriptors = new Collection<BIOPDescriptor>();

                    while (userInfoLength > 0)
                    {
                        BIOPDescriptor descriptor = BIOPDescriptor.Create(byteData, lastIndex);
                        descriptors.Add(descriptor);

                        userInfoLength -= (descriptor.Index - lastIndex);
                        lastIndex = descriptor.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Module Info message is short"));
            }
        }

        /// <summary>
        /// Validate the module information.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The module information is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the module information fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP MODULE INFO: Mod t/0: " + moduleTimeout +
                " Block timeout: " + blockTimeout +
                " Min block time: " + minimumBlockTime +
                " Taps count: " + tapsCount + 
                " User info lth: " + userInfoLength);

            if (taps != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPTap tap in taps)
                    tap.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            if (descriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPDescriptor descriptor in descriptors)
                    descriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
