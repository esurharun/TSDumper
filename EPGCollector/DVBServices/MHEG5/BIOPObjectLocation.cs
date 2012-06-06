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

using DomainObjects;

// 2.0.16

namespace DVBServices
{
    /// <summary>
    /// The class that describes a BIOP object location.
    /// </summary>
    public class BIOPObjectLocation
    {
        /// <summary>
        /// Get the carousel ID.
        /// </summary>
        public int CarouselID { get { return(carouselID); } }
        /// <summary>
        /// Get the module ID.
        /// </summary>
        public int ModuleID { get { return(moduleID); } }
        /// <summary>
        /// Get the object key length.
        /// </summary>
        public int ObjectKeyLength { get { return(objectKeyLength); } }
        /// <summary>
        /// Get the object key data.
        /// </summary>
        public byte[] ObjectKeyData { get { return(objectKeyData); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the object location.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The object location has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPObjectLocation: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] componentIDTag = new byte[1] { 0x3f };
        private int dataLength;
        private int carouselID;
        private int moduleID;
        private int majorVersion;
        private int minorVersion;
        private int objectKeyLength;
        private byte[] objectKeyData;

        private int lastIndex = -1;

        private static byte[] tag = new byte[4] { 0x49, 0x53, 0x4f, 0x50 };

        /// <summary>
        /// Initialize a new instance of the BIOPObjectLocation class.
        /// </summary>
        public BIOPObjectLocation() { }

        /// <summary>
        /// Parse the object location.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the object location.</param>
        /// <param name="index">Index of the first byte of the object location in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                componentIDTag = Utils.GetBytes(byteData, lastIndex, 4);
                lastIndex += 4;

                dataLength = (int)byteData[lastIndex];
                lastIndex++;

                carouselID = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                moduleID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                majorVersion = (int)byteData[lastIndex];
                lastIndex++;

                minorVersion = (int)byteData[lastIndex];
                lastIndex++;

                objectKeyLength = (int)byteData[lastIndex];
                lastIndex++;

                if (objectKeyLength != 0)
                {
                    objectKeyData = Utils.GetBytes(byteData, lastIndex, objectKeyLength);
                    lastIndex += objectKeyLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Object Location message is short"));
            }
        }

        /// <summary>
        /// Validate the object location fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An object location field is not valid.
        /// </exception>
        public void Validate()
        {
            if (!Utils.CompareBytes(componentIDTag, BIOPObjectLocation.tag))
                throw (new ArgumentOutOfRangeException("The tag is wrong"));

            if (majorVersion != 1)
                throw (new ArgumentOutOfRangeException("The major version is not 1"));

            if (minorVersion != 0)
                throw (new ArgumentOutOfRangeException("The minor version is not zero"));
        }

        /// <summary>
        /// Log the object location fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP OBJECTLOCATION: Component ID tag: " + Utils.ConvertToHex(componentIDTag) +
                " Data lth: " + dataLength +
                " Carousel ID: " + Utils.ConvertToHex(carouselID) +
                " Module ID: " + Utils.ConvertToHex(moduleID) +
                " Major ver: " + Utils.ConvertToHex(majorVersion) +
                " Minor ver: " + Utils.ConvertToHex(minorVersion) +
                " Obj key lth: " + objectKeyLength +
                " Obj key data: " + Utils.ConvertToHex(objectKeyData));
        }
    }
}
