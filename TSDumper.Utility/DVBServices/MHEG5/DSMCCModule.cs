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
using System.IO;
using System.Collections.ObjectModel;

using DomainObjects;

using ComponentAce.Compression.Libs.zlib;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a DSMCC module.
    /// </summary>
    public class DSMCCModule
    {
        /// <summary>
        /// Get the module ID.
        /// </summary>
        public int ModuleID { get { return (moduleID); } }
        /// <summary>
        /// Get the version number of the module.
        /// </summary>
        public int Version { get { return (version); } }
        /// <summary>
        /// Get the collection of data blocks that make up the module.
        /// </summary>
        public Collection<DSMCCDownloadDataBlock> Blocks { get { return (blocks); } }
        /// <summary>
        /// Get the module data.
        /// </summary>
        public byte[] Data { get { return (data); } }
        /// <summary>
        /// Returns true if the module has been completely loaded; false otherwise.
        /// </summary>
        public bool Complete { get { return (complete); } }
        /// <summary>
        /// Returns true if the module is compressed; false otherwise.
        /// </summary>
        public bool Compressed { get { return (size != originalSize); } }
        /// <summary>
        /// Get the collection of BIOP messages for the module.
        /// </summary>
        public Collection<BIOPMessage> Objects { get { return (biopMessages); } }

        private int moduleID;
        private int version;
        private Collection<DSMCCDownloadDataBlock> blocks;

        private int size;
        private int originalSize;

        private bool complete;
        private Collection<BIOPMessage> biopMessages;
        private byte[] data = new byte[1] { 0x00 };

        /// <summary>
        /// Initialize a new instance of the DSMCCModule class.
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="version">The version number of the module.</param>
        /// <param name="size">The size of the module.</param>
        /// <param name="originalSize">The original size of the module.</param>
        public DSMCCModule(int moduleID, int version, int size, int originalSize)
        {
            this.moduleID = moduleID;
            this.version = version;
            this.size = size;
            this.originalSize = originalSize;

            blocks = new Collection<DSMCCDownloadDataBlock>();            
        }

        /// <summary>
        /// Add a download data block to the module.
        /// </summary>
        /// <param name="newBlock">The block to be added.</param>
        /// <returns>True if the block has been added; false otherwise.</returns>
        public bool AddBlock(DSMCCDownloadDataBlock newBlock)
        {
            foreach (DSMCCDownloadDataBlock oldBlock in blocks)
            {
                if (oldBlock.BlockNumber == newBlock.BlockNumber)
                    return (false);

                if (oldBlock.BlockNumber > newBlock.BlockNumber)
                {
                    blocks.Insert(blocks.IndexOf(oldBlock), newBlock);
                    try
                    {
                        complete = checkComplete();
                        return (true);
                    }
                    catch (InvalidOperationException)
                    {
                        blocks.Clear();
                        return (false);
                    }
                }
            }

            blocks.Add(newBlock);

            try
            {
                complete = checkComplete();
                return (true);
            }
            catch (InvalidOperationException)
            {
                blocks.Clear();
                return (false);
            }
        }

        private bool checkComplete()
        {
            int totalSize = 0;

            foreach (DSMCCDownloadDataBlock block in blocks)
                totalSize += block.DataSize;

            if (totalSize != size)
                return (false);

            if (!Compressed)
                createData();
            else
            {
                string reply = deflate();
                if (reply != null)
                {
                    if (Logger.ProtocolLogger != null)
                        Logger.ProtocolLogger.Write("Zlib deflate failed: " + reply);
                    throw (new InvalidOperationException("Zlib deflate failed: " + reply));
                }
            }

            if (Utils.CompareBytes(data, BIOPMessage.BiopMagic, BIOPMessage.BiopMagic.Length))
            {
                biopMessages = new Collection<BIOPMessage>();

                int processedSize = 0;

                while (processedSize != originalSize)
                {
                    BIOPMessage biopMessage = new BIOPMessage();
                    biopMessage.Process(data, processedSize);
                    biopMessages.Add(biopMessage);

                    processedSize = biopMessage.Index;
                }
            }

            return (true);
        }

        private void createData()
        {
            int totalSize = 0;

            foreach (DSMCCDownloadDataBlock block in blocks)
                totalSize += block.DataSize;

            data = new byte[totalSize];
            int dataIndex = 0;

            foreach (DSMCCDownloadDataBlock block in blocks)
            {
                for (int inputIndex = 0; inputIndex < block.DataSize; inputIndex++)
                {
                    data[dataIndex] = block.Data[inputIndex];
                    dataIndex++;
                }
            }
        }

        private string deflate()
        {
            int totalSize = 0;

            foreach (DSMCCDownloadDataBlock block in blocks)
                totalSize += block.DataSize;

            byte[] moduleData = new byte[totalSize];
            int dataIndex = 0;

            foreach (DSMCCDownloadDataBlock block in blocks)
            {
                for (int inputIndex = 0; inputIndex < block.DataSize; inputIndex++)
                {
                    moduleData[dataIndex] = block.Data[inputIndex];
                    dataIndex++;
                }
            }

            try
            {
                MemoryStream output = new MemoryStream();
                ZOutputStream zstream = new ZOutputStream(output);

                data = new byte[originalSize];
                zstream.Write(moduleData, 0, moduleData.Length);
                zstream.Flush();
                output.Seek(0, SeekOrigin.Begin);
                int readCount = output.Read(data, 0, data.Length);

                return (null);
            }
            catch (Exception e)
            {
                return (e.Message);
            }
        }

        /// <summary>
        /// Log the fields in the module.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSMCC MODULE: Module ID: " + Utils.ConvertToHex(moduleID) +
                " Vers no: " + Utils.ConvertToHex(version) +
                " Size: " + size +
                " Orig size: " + originalSize +
                " Complete: " + Complete);

            if (biopMessages != null)
            {
                foreach (BIOPMessage biopMessage in biopMessages)
                {
                    Logger.IncrementProtocolIndent();
                    biopMessage.LogMessage();
                    Logger.DecrementProtocolIndent();
                }
            }
        }        
    }
}
