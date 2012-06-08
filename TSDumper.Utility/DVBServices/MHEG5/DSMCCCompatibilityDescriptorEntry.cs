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
    /// The class that describes a DSMCC compatability descriptor entry.
    /// </summary>
    public class DSMCCCompatibilityDescriptorEntry
    {
        /// <summary>
        /// Get the descriptor type.
        /// </summary>
        public int DescriptorType { get { return (descriptorType); } }
        /// <summary>
        /// Get the specifier type.
        /// </summary>
        public int SpecifierType { get { return (specifierType); } }
        /// <summary>
        /// Get the specifier data.
        /// </summary>
        public byte[] SpecifierData { get { return (specifierData); } }
        /// <summary>
        /// Get the model number.
        /// </summary>
        public int Model { get { return (model); } }
        /// <summary>
        /// Get the version number.
        /// </summary>
        public int Version { get { return (version); } }
        /// <summary>
        /// Get the collection of sub-descriptors.
        /// </summary>
        public Collection<DSMCCCompatibilityDescriptorSubDescriptor> SubDescriptors { get { return(subDescriptors); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The entry has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCCompatabilityDescriptorEntry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int descriptorType;
        private int descriptorLength;
        private int specifierType;
        private byte[] specifierData;
        private int model;
        private int version;
        private int subDescriptorCount;
        private Collection<DSMCCCompatibilityDescriptorSubDescriptor> subDescriptors;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCCompatibilityDescriptorEntry class.
        /// </summary>
        public DSMCCCompatibilityDescriptorEntry() { }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the entry.</param>
        /// <param name="index">Index of the first byte of the entry in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                descriptorType = (int)byteData[lastIndex];
                lastIndex++;

                descriptorLength = (int)byteData[lastIndex];
                lastIndex++;

                specifierType = (int)byteData[lastIndex];
                lastIndex++;

                specifierData = Utils.GetBytes(byteData, lastIndex, 3);
                lastIndex += 3;

                model = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                version = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                subDescriptorCount = (int)byteData[lastIndex];
                lastIndex++;

                if (subDescriptorCount != 0)
                {
                    subDescriptors = new Collection<DSMCCCompatibilityDescriptorSubDescriptor>();

                    while (subDescriptors.Count != subDescriptorCount)
                    {
                        DSMCCCompatibilityDescriptorSubDescriptor subDescriptor = new DSMCCCompatibilityDescriptorSubDescriptor();
                        subDescriptor.Process(byteData, lastIndex);
                        subDescriptors.Add(subDescriptor);

                        lastIndex = subDescriptor.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Compatability Descriptor Entry message is short"));
            }
        }

        /// <summary>
        /// Validate the entry.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The entry information is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSMCC COMPATIBILITY DESCRIPTOR ENTRY: Descr TYPE: " + Utils.ConvertToHex(descriptorType) +
                " Descr lth: " + descriptorLength +
                " Spec type: " + Utils.ConvertToHex(specifierType) +
                " Spec data: " + Utils.ConvertToHex(specifierData) +
                " Model: " + Utils.ConvertToHex(model) +
                " Version: " + Utils.ConvertToHex(version) +
                " Sub desc ct: " + subDescriptorCount);

            if (subDescriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DSMCCCompatibilityDescriptorSubDescriptor subDescriptor in subDescriptors)
                    subDescriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
