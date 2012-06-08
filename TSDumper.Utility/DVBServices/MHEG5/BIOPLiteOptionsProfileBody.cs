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
    /// The class that describes a BIOP Lite Options Profile body.
    /// </summary>
    public class BIOPLiteOptionsProfileBody
    {
        /// <summary>
        /// Get the DSM service location.
        /// </summary>
        public DSMServiceLocation DSMServiceLocation { get { return (dsmServiceLocation); } }
        /// <summary>
        /// Get the collection of BIOP Lite components.
        /// </summary>
        public Collection<BIOPLiteComponent> LiteComponents { get { return (liteComponents); } }

        /// <summary>
        /// Get the profile tag.
        /// </summary>
        public static byte[] Tag = new byte[4] { 0x49, 0x53, 0x4f, 0x05 };

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the profile body.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The profile body has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPLiteOptionsProfileBody: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int componentsCount;
        private DSMServiceLocation dsmServiceLocation;
        private Collection<BIOPLiteComponent> liteComponents;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPLiteOptionsProfileBody class.
        /// </summary>
        public BIOPLiteOptionsProfileBody() { }

        /// <summary>
        /// Parse the profile body.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the profile body.</param>
        /// <param name="index">Index of the first byte of the profile body in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                componentsCount = (int)byteData[lastIndex]; ;
                lastIndex++;

                dsmServiceLocation = new DSMServiceLocation();
                dsmServiceLocation.Process(byteData, lastIndex);
                lastIndex += dsmServiceLocation.Index;

                if (componentsCount > 1)
                {
                    liteComponents = new Collection<BIOPLiteComponent>();

                    while (liteComponents.Count != componentsCount - 1)
                    {
                        BIOPLiteComponent liteComponent = new BIOPLiteComponent();
                        liteComponent.Process(byteData, lastIndex);
                        liteComponents.Add(liteComponent);

                        lastIndex = liteComponent.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Lite Options Profile Body message is short"));
            }
        }

        /// <summary>
        /// Validate the profile body fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A profile body field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the profile body fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP LITE OPTIONS PROFILE BODY: Component ct: " + componentsCount);

            if (dsmServiceLocation != null)
                dsmServiceLocation.LogMessage();

            if (liteComponents != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPLiteComponent liteComponent in liteComponents)
                    liteComponent.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
