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
using System.Runtime.InteropServices;

namespace DirectShowAPI
{
    #region Declarations

    /// <summary>
    /// From ScanModulationTypes
    /// </summary>
    [Flags]
    public enum ScanModulationTypes
    {
      ScanMod16QAM = 0x00000001,
      ScanMod32QAM = 0x00000002,
      ScanMod64QAM = 0x00000004,
      ScanMod80QAM = 0x00000008,
      ScanMod96QAM = 0x00000010,
      ScanMod112QAM = 0x00000020,
      ScanMod128QAM = 0x00000040,
      ScanMod160QAM = 0x00000080,
      ScanMod192QAM = 0x00000100,
      ScanMod224QAM = 0x00000200,
      ScanMod256QAM = 0x00000400,
      ScanMod320QAM = 0x00000800,
      ScanMod384QAM = 0x00001000,
      ScanMod448QAM = 0x00002000,
      ScanMod512QAM = 0x00004000,
      ScanMod640QAM = 0x00008000,
      ScanMod768QAM = 0x00010000,
      ScanMod896QAM = 0x00020000,
      ScanMod1024QAM = 0x00040000,
      ScanModQPSK = 0x00080000,
      ScanModBPSK = 0x00100000,
      ScanModOQPSK = 0x00200000,
      ScanMod8VSB = 0x00400000,
      ScanMod16VSB = 0x00800000,
      ScanModAM_RADIO = 0x01000000,
      ScanModFM_RADIO = 0x02000000,
      ScanMod8PSK = 0x04000000,
      ScanModRF = 0x08000000,
      MCEDigitalCable = ModulationType.Mod640Qam | ModulationType.Mod256Qam,
      MCETerrestrialATSC = ModulationType.Mod8Vsb,
      MCEAnalogTv = ModulationType.ModRF,
      MCEAll_TV = unchecked((int)0xffffffff),
    }

    /// <summary>
    /// From RollOff
    /// </summary>
    public enum RollOff
    {
      NotSet = -1,
      NotDefined = 0,
      Twenty = 1,
      TwentyFive,
      ThirtyFive,
      Max
    }

    /// <summary>
    /// From Pilot
    /// </summary>
    public enum Pilot
    {
      NotSet = -1,
      NotDefined = 0,
      Off = 1,
      On,
      Max
    }

    /// <summary>
    /// From FECMethod
    /// </summary>
    public enum FECMethod
    {
        MethodNotSet = -1,
        MethodNotDefined = 0,
        Viterbi = 1, // FEC is a Viterbi Binary Convolution.
        RS204_188, // The FEC is Reed-Solomon 204/188 (outer FEC)
        Ldpc,
        Bch,
        RS147_130,
        Max,
    }

    /// <summary>
    /// From BinaryConvolutionCodeRate
    /// </summary>
    public enum BinaryConvolutionCodeRate
    {
        RateNotSet = -1,
        RateNotDefined = 0,
        Rate1_2 = 1, // 1/2
        Rate2_3, // 2/3
        Rate3_4, // 3/4
        Rate3_5,
        Rate4_5,
        Rate5_6, // 5/6
        Rate5_11,
        Rate7_8, // 7/8
        Rate1_4,
        Rate1_3,
        Rate2_5,
        Rate6_7,
        Rate8_9,
        Rate9_10,
        RateMax
    }

    /// <summary>
    /// From Polarisation
    /// </summary>
    public enum Polarisation
    {
        NotSet = -1,
        NotDefined = 0,
        LinearH = 1, // Linear horizontal polarisation
        LinearV, // Linear vertical polarisation
        CircularL, // Circular left polarisation
        CircularR, // Circular right polarisation
        Max,
    }

    /// <summary>
    /// From SpectralInversion
    /// </summary>
    public enum SpectralInversion
    {
        NotSet = -1,
        NotDefined = 0,
        Automatic = 1,
        Normal,
        Inverted,
        Max
    }

    /// <summary>
    /// From ModulationType
    /// </summary>
    public enum ModulationType
    {
        ModNotSet = -1,
        ModNotDefined = 0,
        Mod16Qam = 1,
        Mod32Qam,
        Mod64Qam,
        Mod80Qam,
        Mod96Qam,
        Mod112Qam,
        Mod128Qam,
        Mod160Qam,
        Mod192Qam,
        Mod224Qam,
        Mod256Qam,
        Mod320Qam,
        Mod384Qam,
        Mod448Qam,
        Mod512Qam,
        Mod640Qam,
        Mod768Qam,
        Mod896Qam,
        Mod1024Qam,
        ModQpsk,
        ModBpsk,
        ModOqpsk,
        Mod8Vsb,
        Mod16Vsb,
        ModAnalogAmplitude, // std am
        ModAnalogFrequency, // std fm
        Mod8Psk,
        ModRF,
        Mod16Apsk,
        Mod32Apsk,
        ModNbcQpsk,
        ModNbc8Psk,
        ModDirectTv,
        ModMax
    }

    /// <summary>
    /// From DVBSystemType
    /// </summary>
    public enum DVBSystemType
    {
        Cable,
        Terrestrial,
        Satellite,
        ISDBT,
        ISDBS
    }

    /// <summary>
    /// From HierarchyAlpha
    /// </summary>
    public enum HierarchyAlpha
    {
        HAlphaNotSet = -1,
        HAlphaNotDefined = 0,
        HAlpha1 = 1, // Hierarchy alpha is 1.
        HAlpha2, // Hierarchy alpha is 2.
        HAlpha4, // Hierarchy alpha is 4.
        HAlphaMax,
    }

    /// <summary>
    /// From GuardInterval
    /// </summary>
    public enum GuardInterval
    {
        GuardNotSet = -1,
        GuardNotDefined = 0,
        Guard1_32 = 1, // Guard interval is 1/32
        Guard1_16, // Guard interval is 1/16
        Guard1_8, // Guard interval is 1/8
        Guard1_4, // Guard interval is 1/4
        GuardMax,
    }

    /// <summary>
    /// From TransmissionMode
    /// </summary>
    public enum TransmissionMode
    {
        ModeNotSet = -1,
        ModeNotDefined = 0,
        Mode2K = 1, // Transmission uses 1705 carriers (use a 2K FFT)
        Mode8K, // Transmission uses 6817 carriers (use an 8K FFT)
        Mode4K,
        Mode2KInterleaved,
        Mode4KInterleaved,
        ModeMax,
    }

    /// <summary>
    /// From ComponentStatus
    /// </summary>
    public enum ComponentStatus
    {
        Active,
        Inactive,
        Unavailable
    }

    /// <summary>
    /// From ComponentCategory
    /// </summary>
    public enum ComponentCategory
    {
        NotSet = -1,
        Other = 0,
        Video,
        Audio,
        Text,
        Data
    }

    /// <summary>
    /// From BDA_TEMPLATE_CONNECTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BDATemplateConnection
    {
        public int FromNodeType;
        public int FromNodePinType;
        public int ToNodeType;
        public int ToNodePinType;
    }

    #endregion
}
