////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2011                                                 //
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
using System.Runtime.InteropServices;

namespace DirectShow
{
    /// <summary>
    /// CLSID_PSIMemorySink
    /// </summary>
    [ComImport, Guid("90F65EC9-E1B0-4602-86A5-CA69CC4702AB")]
    public class PSIMemorySinkFilter
    {
    }

    /// <summary>
    /// The memory sink interface definition. 
    /// </summary>
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("34D4180C-44EF-4907-8372-82FAB7A36767"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemSinkSettings
    {
        /// <summary>
        /// Return true if data is flowing; false otherwise.
        /// </summary>
        /// <param name="flowing">An indication of data flowing.</param>
        [PreserveSig]
        void get_IsDataFlowing([Out] out bool flowing);
        
        /// <summary>
        /// Clear the filter buffers.
        /// </summary>
        [PreserveSig]
        void clear();
        
        /// <summary>
        /// Get the number of bytes of the buffer used.
        /// </summary>
        /// <param name="bufferUsed">The amount of buffer space used.</param>
        [PreserveSig]
        void get_BufferUsed([Out] out int bufferUsed);
        
        /// <summary>
        /// Get the address of the memory buffer.
        /// </summary>
        /// <param name="bufferAddress">The address of the filters buffer.</param>
        [PreserveSig]
        void get_BufferAddress([Out] out int bufferAddress);

        /// <summary>
        /// Clear the mapped pid's.
        /// </summary>
        [PreserveSig]
        void clearPIDs();

        /// <summary>
        /// Map a pid.
        /// </summary>
        [PreserveSig]
        void mapPID(int pid);

        /// <summary>
        /// Get the number of sync byte searches.
        /// </summary>
        /// <param name="syncByteSearches">The no of sync byte searches.</param>
        [PreserveSig]
        void get_SyncByteSearchCount([Out] out int syncByteSearchCount);

        /// <summary>
        /// Get the number of samples dropped because no sync byte.
        /// </summary>
        /// <param name="samplesDroppeds">The no of samples dropped.</param>
        [PreserveSig]
        void get_SamplesDropped([Out] out int samplesDropped);

        /// <summary>
        /// Get the size of the dump file.
        /// </summary>
        /// <param name="dumpFileSize">The size of the dump file.</param>
        [PreserveSig]
        void get_DumpFileSize([Out] out int dumpFileSize);

        /// <summary>
        /// Get the maximum sample size.
        /// </summary>
        /// <param name="maximumSampleSize">The maximum size of the samples.</param>
        [PreserveSig]
        void get_MaximumSampleSize([Out] out int maximumSampleSize);
    }
}
