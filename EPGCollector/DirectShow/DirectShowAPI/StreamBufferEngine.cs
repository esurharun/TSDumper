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
using System.Runtime.InteropServices;
using System.Text;

namespace DirectShowAPI
{
    /// <summary>
    /// From STREAMBUFFER_ATTR_DATATYPE
    /// </summary>
    public enum StreamBufferAttrDataType
    {
        DWord = 0,
        String = 1,
        Binary = 2,
        Bool = 3,
        QWord = 4,
        Word = 5,
        Guid = 6
    }

    /// <summary>
    /// </summary>
    public sealed class StreamBufferEngine
    {
        private StreamBufferEngine() { }

        ////////////////////////////////////////////////////////////////
        //
        // List of pre-defined attributes
        //
        public readonly string Duration = "Duration";
        public readonly string Bitrate = "Bitrate";
        public readonly string Seekable = "Seekable";
        public readonly string Stridable = "Stridable";
        public readonly string Broadcast = "Broadcast";
        public readonly string Protected = "Is_Protected";
        public readonly string Trusted = "Is_Trusted";
        public readonly string Signature_Name = "Signature_Name";
        public readonly string HasAudio = "HasAudio";
        public readonly string HasImage = "HasImage";
        public readonly string HasScript = "HasScript";
        public readonly string HasVideo = "HasVideo";
        public readonly string CurrentBitrate = "CurrentBitrate";
        public readonly string OptimalBitrate = "OptimalBitrate";
        public readonly string HasAttachedImages = "HasAttachedImages";
        public readonly string SkipBackward = "Can_Skip_Backward";
        public readonly string SkipForward = "Can_Skip_Forward";
        public readonly string NumberOfFrames = "NumberOfFrames";
        public readonly string FileSize = "FileSize";
        public readonly string HasArbitraryDataStream = "HasArbitraryDataStream";
        public readonly string HasFileTransferStream = "HasFileTransferStream";

        ////////////////////////////////////////////////////////////////
        //
        // The content description object supports 5 basic attributes.
        //
        public readonly string Title = "Title";
        public readonly string Author = "Author";
        public readonly string Description = "Description";
        public readonly string Rating = "Rating";
        public readonly string Copyright = "Copyright";

        ////////////////////////////////////////////////////////////////
        //
        // These are the additional attributes defined in the WM attribute
        // namespace that give information about the content.
        //
        public readonly string AlbumTitle = "WM/AlbumTitle";
        public readonly string Track = "WM/Track";
        public readonly string PromotionURL = "WM/PromotionURL";
        public readonly string AlbumCoverURL = "WM/AlbumCoverURL";
        public readonly string Genre = "WM/Genre";
        public readonly string Year = "WM/Year";
        public readonly string GenreID = "WM/GenreID";
        public readonly string MCDI = "WM/MCDI";
        public readonly string Composer = "WM/Composer";
        public readonly string Lyrics = "WM/Lyrics";
        public readonly string TrackNumber = "WM/TrackNumber";
        public readonly string ToolName = "WM/ToolName";
        public readonly string ToolVersion = "WM/ToolVersion";
        public readonly string IsVBR = "IsVBR";
        public readonly string AlbumArtist = "WM/AlbumArtist";

        ////////////////////////////////////////////////////////////////
        //
        // These optional attributes may be used to give information
        // about the branding of the content.
        //
        public readonly string BannerImageType = "BannerImageType";
        public readonly string BannerImageData = "BannerImageData";
        public readonly string BannerImageURL = "BannerImageURL";
        public readonly string CopyrightURL = "CopyrightURL";

        ////////////////////////////////////////////////////////////////
        //
        // Optional attributes, used to give information
        // about video stream properties.
        //
        public readonly string AspectRatioX = "AspectRatioX";
        public readonly string AspectRatioY = "AspectRatioY";

        ////////////////////////////////////////////////////////////////
        //
        // The NSC file supports the following attributes.
        //
        public readonly string NSCName = "NSC_Name";
        public readonly string NSCAddress = "NSC_Address";
        public readonly string NSCPhone = "NSC_Phone";
        public readonly string NSCEmail = "NSC_Email";
        public readonly string NSCDescription = "NSC_Description";
    }

    /// <summary>
    /// From STREAMBUFFER_ATTRIBUTE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StreamBufferAttribute
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string pszName;
        public StreamBufferAttrDataType StreamBufferAttributeType;
        public IntPtr pbAttribute; // BYTE *
        public short cbLength;
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("16CA4E03-FE69-4705-BD41-5B7DFC0C95F3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStreamBufferRecordingAttribute
    {
        [PreserveSig]
        int SetAttribute(
            [In] int ulReserved,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszAttributeName,
            [In] StreamBufferAttrDataType StreamBufferAttributeType,
            [In] IntPtr pbAttribute, // BYTE *
            [In] short cbAttributeLength
            );

        [PreserveSig]
        int GetAttributeCount(
            [In] int ulReserved,
            [Out] out short pcAttributes
            );

        [PreserveSig]
        int GetAttributeByName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszAttributeName,
            [In] int pulReserved,
            [Out] out StreamBufferAttrDataType pStreamBufferAttributeType,
            [In, Out] IntPtr pbAttribute, // BYTE *
            [In, Out] ref short pcbLength
            );

        [PreserveSig]
        int GetAttributeByIndex(
            [In] short wIndex,
            [In, Out] ref int pulReserved,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszAttributeName,
            [In, Out] ref short pcchNameLength,
            [Out] out StreamBufferAttrDataType pStreamBufferAttributeType,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbAttribute,
            [In, Out] ref short pcbLength
            );

        int EnumAttributes([Out] out IEnumStreamBufferRecordingAttrib ppIEnumStreamBufferAttrib);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C18A9162-1E82-4142-8C73-5690FA62FE33"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumStreamBufferRecordingAttrib
    {
        [PreserveSig]
        int Next(
            [In] int cRequest,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] StreamBufferAttribute[] pStreamBufferAttribute,
            [In] IntPtr pcReceived
            );

        [PreserveSig]
        int Skip([In] int cRecords);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumStreamBufferRecordingAttrib ppIEnumStreamBufferAttrib);
    }
}
