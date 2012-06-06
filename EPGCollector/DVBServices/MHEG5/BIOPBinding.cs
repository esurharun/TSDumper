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
    /// The class that describes a BIOP binding.
    /// </summary>
    public class BIOPBinding
    {
        /// <summary>
        /// The binding type for an object binding.
        /// </summary>
        public const int BindingTypeObject = 1;
        /// <summary>
        /// The binding type for a context binding.
        /// </summary>
        public const int BindingTypeContext = 2;

        /// <summary>
        /// Get the collection of names for the binding.
        /// </summary>
        public Collection<BIOPName> Names { get { return (names); } }
        /// <summary>
        /// Get the binding type of the binding.
        /// </summary>
        public int BindingType { get { return (bindingType); } }
        /// <summary>
        /// Get the IOP:IOR for the binding.
        /// </summary>
        public IOPIOR IOPIOR { get { return (iopIOR); } }
        /// <summary>
        /// Get the length of the object information for the binding.
        /// </summary>
        public int ObjectInfoLength { get { return (objectInfoLength); } }
        /// <summary>
        /// Get the object information for the binding.
        /// </summary>
        public byte[] ObjectInfo { get { return (objectInfo); } }

        /// <summary>
        /// Get the size in bytes of the file contents.
        /// </summary>
        public long FileContentSize
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPBinding: FileContentSize requested before block processed"));
                if (names.Count == 0 || names[0].Kind != "fil")
                    throw (new InvalidOperationException("The file content size is not defined for this kind of binding"));
                return (Utils.Swap8BytesToLong(objectInfo, 0));
            }
        }

        /// <summary>
        /// Get the file descriptors for the binding.
        /// </summary>
        public byte[] FileDescriptors
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPBinding: FileDescriptors requested before block processed"));
                if (objectInfoLength == 0)
                    throw (new InvalidOperationException("BIOPBinding: No file descriptors present"));
                if (names.Count == 0 || names[0].Kind != "fil")
                    return(Utils.GetBytes(objectInfo, 0, objectInfoLength));
                else
                {
                    if (objectInfoLength < 9)
                        throw (new InvalidOperationException("BIOPBinding: No file descriptors present"));
                    else
                        return(Utils.GetBytes(objectInfo, 8, objectInfoLength - 8));
                }
            }
        }

        /// <summary>
        /// Gets the index of the next byte in the MPEG2 section following the binding.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The binding has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPBinding: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int nameCount;
        private Collection<BIOPName> names;
        private int bindingType;
        private IOPIOR iopIOR;
        private int objectInfoLength;
        private byte[] objectInfo = new byte[1] { 0x00 };

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPBinding class.
        /// </summary>
        public BIOPBinding() { }

        /// <summary>
        /// Parse the binding.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the first byte of the binding in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                nameCount = (int)byteData[lastIndex];
                lastIndex++;

                if (nameCount != 0)
                {
                    names = new Collection<BIOPName>();

                    while (names.Count != nameCount)
                    {
                        BIOPName name = new BIOPName();
                        name.Process(byteData, lastIndex);
                        names.Add(name);

                        lastIndex = name.Index;
                    }
                }

                bindingType = (int)byteData[lastIndex];
                lastIndex++;

                iopIOR = new IOPIOR();
                iopIOR.Process(byteData, lastIndex);
                lastIndex = iopIOR.Index;

                objectInfoLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (objectInfoLength != 0)
                {
                    objectInfo = Utils.GetBytes(byteData, lastIndex, objectInfoLength);
                    lastIndex += objectInfoLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Binding message is short"));
            }
        }

        /// <summary>
        /// Validate the bindingr fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the binding fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP BINDING: Name count: " + nameCount +
                " Binding type: " + bindingType +
                " Obj info lth: " + objectInfoLength + 
                " Object info: : " + Utils.ConvertToHex(objectInfo));

            if (names != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPName name in names)
                    name.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            Logger.IncrementProtocolIndent();
            iopIOR.LogMessage();
            Logger.DecrementProtocolIndent();
        }
    }
}
