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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The interface for obtaining samples from the input stream.
    /// </summary>
    public interface ISampleDataProvider
    {
        /// <summary>
        /// Get the amount of buffer space used in bytes
        /// </summary>
        int BufferSpaceUsed { get; }
        /// <summary>
        /// Get the address of the buffer.
        /// </summary>
        IntPtr BufferAddress { get; }
        /// <summary>
        /// Get the current frequency.
        /// </summary>
        TuningFrequency Frequency { get; }

        /// <summary>
        /// Change the PID mapping.
        /// </summary>
        /// <param name="pid">The PID to be mapped.</param>
        void ChangePidMapping(int pid);
        /// <summary>
        /// Change the PID mapping.
        /// </summary>
        /// <param name="pids">A list of PID's to be mapped.</param>
        void ChangePidMapping(int[] pids);

        /// <summary>
        /// Get the number of sync byte searches
        /// </summary>
        int SyncByteSearches { get; }
        /// <summary>
        /// Get the number of samples dropped
        /// </summary>
        int SamplesDropped { get; }
        /// <summary>
        /// Get the maximum sample size
        /// </summary>
        int MaximumSampleSize { get; }
    }

    /// <summary>
    /// The KSProperty structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct KSProperty
    {
        /// <summary>
        /// The GUID that identifies the property set.
        /// </summary>
        public Guid Set;
        /// <summary>
        /// The member of the property set.
        /// </summary>
        public ulong Id;
        /// <summary>
        /// The request type.
        /// </summary>
        public ulong Flags;        
    }

    /// <summary>
    /// The KSPNode structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct KSPNode
    {
        /// <summary>
        /// The property.
        /// </summary>
        public IntPtr Property;
        /// <summary>
        /// The node.
        /// </summary>
        public ulong Node;
        /// <summary>
        /// Not used
        /// </summary>
        public ulong Reserved;        
    }

    /// <summary>
    /// The Diseqc Send structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct BDADiseqcSend
    {
        /// <summary>
        /// The command ID.
        /// </summary>
        public ulong commandID;
        /// <summary>
        /// The command length.
        /// </summary>
        public ulong commandLength;
        /// <summary>
        /// The command.
        /// </summary>
        public byte[] command;
    }


    /// <summary>
    /// The IKsControl interface definition
    /// </summary>
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("28f54685-06fd-11d2-b27a-00a0c9223196"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsControl
    {
        /// <summary>
        /// The KsProperty method of the interface.
        /// </summary>
        /// <param name="node">The property identifier.</param>
        /// <param name="nodeLength">The length of the property identifier.</param>
        /// <param name="propertyData">The property data.</param>
        /// <param name="dataLength">The length of the property data.</param>
        /// <param name="bytesReturned">The number of bytes returned in the property data.</param>
        /// <returns></returns>
        [PreserveSig]
        int KsProperty([In] ref KSPNode node, [In] ulong nodeLength, [In] ref ulong propertyData, [In] ulong dataLength, [In, Out] ref ulong bytesReturned );        
    }

    /// <summary>
    /// The KSPropertyType that species the request type.
    /// </summary>
    public enum KSPropertyType
    {
        /// <summary>
        /// Retrieves the value of the specified property item.
        /// </summary>
        Get = 0x01,
        /// <summary>
        /// Sets the value of the specified property item.
        /// </summary>
        Set = 0x02,
        /// <summary>
        /// Queries if the driver supports this property set.
        /// </summary>
        SetSupport = 0x100
    }

    /// <summary>
    /// Diseqc command values
    /// </summary>
    public enum DiseqCommand
    {
        /// <summary>
        /// Enable driver commands
        /// </summary>
        Enable = 0,
        /// <summary>
        /// Set the signal source
        /// </summary>
        LnbSource,
        /// <summary>
        /// Use tone burst
        /// </summary>
        UseToneBurst,
        /// <summary>
        /// Set the number of repeats
        /// </summary>
        Repeats,
        /// <summary>
        /// Send a diseqc command
        /// </summary>
        Send,
        /// <summary>
        /// Get a command response
        /// </summary>
        Response
    }

    /// <summary>
    /// Static class that defines PropertySet GUID's.
    /// </summary>
    public class PropertySetIds
    {
        /// <summary>
        /// Get the BDA_Diseq_Command property set GUID.
        /// </summary>
        public static Guid BDADiseqCommand { get { return (new Guid("f84e2ab0-3c6b-45e3-a0fc-8669d4b81f11")); } }
    }
}
