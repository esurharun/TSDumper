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
    /// The class that describes a DSMCC download info indication message.
    /// </summary>
    public class DSMCCDownloadInfoIndication : DSMCCMessage
    {
        /// <summary>
        /// The message ID of a download info indication message.
        /// </summary>
        public const int MessageIDDownloadInfoIndication = 0x1002;

        /// <summary>
        /// Get the download ID.
        /// </summary>
        public int DownloadID { get { return (downloadID); } }
        /// <summary>
        /// Get the block size.
        /// </summary>
        public int BlockSize { get { return (blockSize); } }
        /// <summary>
        /// Get the window size.
        /// </summary>
        public int WindowSize { get { return (windowSize); } }
        /// <summary>
        /// Get gthe ACK period.
        /// </summary>
        public int AckPeriod { get { return (ackPeriod); } }
        /// <summary>
        /// Get the download window.
        /// </summary>
        public int TCDownloadWindow { get { return (tcDownloadWindow); } }
        /// <summary>
        /// Get the download scenario.
        /// </summary>
        public int TCDownloadScenario { get { return (tcDownloadScenario); } }
        /// <summary>
        /// Get the compatibility descriptor.
        /// </summary>
        public DSMCCCompatibilityDescriptor CompatibilityDescriptor { get { return (compatibilityDescriptor); } }
        /// <summary>
        /// Get the collection of modules.
        /// </summary>
        public Collection<DSMCCDownloadInfoIndicationModule> ModuleList { get { return (moduleList); } }
        /// <summary>
        /// Get the length of the private data.
        /// </summary>
        public int PrivateDataLength { get { return (privateDataLength); } }
        /// <summary>
        /// Get the private data.
        /// </summary>
        public byte[] PrivateData { get { return (privateData); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the message.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The message has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCDownloadInfoIndication: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int downloadID;
        private int blockSize;
        private int windowSize;
        private int ackPeriod;
        private int tcDownloadWindow;
        private int tcDownloadScenario;
        private int compatibilityDescriptorLength;
        private DSMCCCompatibilityDescriptor compatibilityDescriptor;
        private int numberOfModules;
        private Collection<DSMCCDownloadInfoIndicationModule> moduleList;
        private int privateDataLength;
        private byte[] privateData;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCDownloadInfoIndication class.
        /// </summary>
        /// <param name="dsmccHeader">The DSMCC header that preceedes the message.</param>
        public DSMCCDownloadInfoIndication(DSMCCHeader dsmccHeader) : base(dsmccHeader) { }

        /// <summary>
        /// Parse the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">Index of the first byte of the message following the header in the MPEG2 section.</param>
        public override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                downloadID = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                blockSize = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                windowSize = (int)byteData[lastIndex];
                lastIndex++;

                ackPeriod = (int)byteData[lastIndex];
                lastIndex++;

                tcDownloadWindow = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                tcDownloadScenario = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                compatibilityDescriptorLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (compatibilityDescriptorLength != 0)
                {
                    compatibilityDescriptor = new DSMCCCompatibilityDescriptor();
                    compatibilityDescriptor.Process(byteData, lastIndex);
                    lastIndex = compatibilityDescriptor.Index;
                }

                numberOfModules = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (numberOfModules != 0)
                {
                    moduleList = new Collection<DSMCCDownloadInfoIndicationModule>();

                    while (moduleList.Count < numberOfModules)
                    {
                        DSMCCDownloadInfoIndicationModule module = new DSMCCDownloadInfoIndicationModule();
                        module.Process(byteData, lastIndex);
                        moduleList.Add(module);

                        lastIndex = module.Index;
                    }
                }

                privateDataLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (privateDataLength != 0)
                {
                    privateData = Utils.GetBytes(byteData, lastIndex, privateDataLength);
                    lastIndex += privateDataLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC DownloadInfo Indication message is short"));
            }
        }

        /// <summary>
        /// Validate the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The message information is not valid.
        /// </exception>
        public override void Validate()
        {
            if (windowSize != 0)
                throw (new ArgumentOutOfRangeException("The window size is not zero"));

            if (ackPeriod != 0)
                throw (new ArgumentOutOfRangeException("The ack period is not zero"));

            if (tcDownloadWindow != 0)
                throw (new ArgumentOutOfRangeException("The tc download window is not zero"));

            if (tcDownloadScenario != 0)
                throw (new ArgumentOutOfRangeException("The tc download scenario is not zero"));

            if (compatibilityDescriptor != null)
                throw (new ArgumentOutOfRangeException("The compatibility descriptor length is not zero"));
        }

        /// <summary>
        /// Log the message fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            base.LogMessage();

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DOWNLOAD INFO INDICATION: Download ID: " + Utils.ConvertToHex(downloadID) +
                " Blk size: " + blockSize +
                " Win size: " + windowSize +
                " Ack per: " + ackPeriod +
                " TC dwn win: " + tcDownloadWindow +
                " TC dwn scn: " + tcDownloadScenario +
                " Compat lth: " + compatibilityDescriptorLength +
                " Priv data lth: " + privateDataLength +
                " No. mods: " + numberOfModules);

            if (compatibilityDescriptor != null)
            {
                Logger.IncrementProtocolIndent();
                compatibilityDescriptor.LogMessage();
                Logger.DecrementProtocolIndent();
            }

            if (moduleList != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DSMCCDownloadInfoIndicationModule module in moduleList)
                    module.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
