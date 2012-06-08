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
using System.Runtime.InteropServices.ComTypes;

namespace DirectShowAPI
{
    #region Declarations

    [ComImport, Guid("A2E30750-6C3D-11d3-B653-00C04F79498E")]
    internal class ATSCTuningSpace { }

    [ComImport, Guid("C6B14B32-76AA-4a86-A7AC-5C79AAF58DA7")]
    internal class DVBTuningSpace { }

    [ComImport, Guid("B64016F3-C9A2-4066-96F0-BD9563314726")]
    internal class DVBSTuningSpace { }

    [ComImport, Guid("D9BB4CEE-B87A-47F1-AC92-B08D9C7813FC")]
    internal class DigitalCableTuningSpace { }

    [ComImport, Guid("8872FF1B-98FA-4d7a-8D93-C9F1055F85BB")]
    internal class ATSCLocator { }

    [ComImport, Guid("C531D9FD-9685-4028-8B68-6E1232079F1E")]
    internal class DVBCLocator { }    

    [ComImport, Guid("9CD64701-BDF3-4d14-8E03-F12983D86664")]
    internal class DVBTLocator { }

    [ComImport, Guid("1DF7D126-4050-47f0-A7CF-4C4CA9241333")]
    internal class DVBSLocator { }    

    [ComImport, Guid("03C06416-D127-407A-AB4C-FDD279ABBE5D")]
    internal class DigitalCableLocator { }

    [ComImport, Guid("6504AFED-A629-455c-A7F1-04964DEA5CC4")]
    internal class ISDBSLocator { }

    [ComImport, Guid("9CD64701-BDF3-4D14-8E03-F12983D86664")]
    internal class ISDBTLocator { }    
    
    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("BAD7753B-6B37-4810-AE57-3CE0C4A9E6CB"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalCableTuneRequest : IATSCChannelTuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        #region IChannelTuneRequest Methods

        [PreserveSig]
        new int get_Channel([Out] out int Channel);

        [PreserveSig]
        new int put_Channel([In] int Channel);

        #endregion

        #region IATSCChannelTuneRequest Methods

        [PreserveSig]
        new int get_MinorChannel([Out] out int MinorChannel);

        [PreserveSig]
        new int put_MinorChannel([In] int MinorChannel);

        #endregion

        [PreserveSig]
        int get_MajorChannel([Out] out int pMajorChannel);

        [PreserveSig]
        int put_MajorChannel([In] int MajorChannel);

        [PreserveSig]
        int get_SourceID([Out] out int pSourceID);

        [PreserveSig]
        int put_SourceID([In] int SourceID);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C9897087-E29C-473F-9E4B-7072123DEA14"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IISDBSLocator : IDVBSLocator
    {
        [DispId(1)]
        int CarrierFrequency { 
            [PreserveSig, DispId(1)] get; 
            [param: In] [PreserveSig, DispId(1)] set; 
        }

        [DispId(2)]
        FECMethod InnerFEC { 
            [PreserveSig, DispId(2)] get; 
            [param: In] [PreserveSig, DispId(2)] set; 
        }

        [DispId(3)]
        BinaryConvolutionCodeRate InnerFECRate { 
            [PreserveSig, DispId(3)] get; 
            [param: In] [PreserveSig, DispId(3)] set; 
        }

        [DispId(4)]
        FECMethod OuterFEC { 
            [PreserveSig, DispId(4)] get; 
            [param: In] [PreserveSig, DispId(4)] set; 
        }

        [DispId(5)]
        BinaryConvolutionCodeRate OuterFECRate { 
            [PreserveSig, DispId(5)] get; 
            [param: In] [PreserveSig, DispId(5)] set; 
        }

        [DispId(6)]
        ModulationType Modulation { 
            [PreserveSig, DispId(6)] get; 
            [param: In] [PreserveSig, DispId(6)] set;
        }

        [DispId(7)]
        int SymbolRate { 
            [PreserveSig, DispId(7)] get; 
            [param: In] [PreserveSig, DispId(7)] set; 
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        [PreserveSig, DispId(8)]
        ILocator Clone();

        [DispId(0x191)]
        Polarisation SignalPolarisation { 
            [PreserveSig, DispId(0x191)] get; 
            [param: In] [PreserveSig, DispId(0x191)] set; 
        }

        [DispId(0x192)]
        bool WestPosition { 
            [PreserveSig, DispId(0x192)] get; 
            [param: In] [PreserveSig, DispId(0x192)] set; 
        }

        [DispId(0x193)]
        int OrbitalPosition { 
            [PreserveSig, DispId(0x193)] get; 
            [param: In] [PreserveSig, DispId(0x193)] set; 
        }

        [DispId(0x194)]
        int Azimuth { 
            [PreserveSig, DispId(0x194)] get; 
            [param: In] [PreserveSig, DispId(0x194)] set; 
        }

        [DispId(0x195)]
        int Elevation { 
            [PreserveSig, DispId(0x195)] get; 
            [param: In] [PreserveSig, DispId(0x195)] set; 
        }

    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("061C6E30-E622-11d2-9493-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITuningSpace
    {
        [PreserveSig]
        int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
        int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);

        [PreserveSig]
        int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        int Clone([Out] out ITuningSpace NewTS);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("28C52640-018A-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITuner
    {
        [PreserveSig]
        int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        int put_TuningSpace([In] ITuningSpace TuningSpace);

        [PreserveSig]
        int EnumTuningSpaces([Out] out IEnumTuningSpaces ppEnum);

        [PreserveSig]
        int get_TuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        int put_TuneRequest([In] ITuneRequest TuneRequest);

        [PreserveSig]
        int Validate([In] ITuneRequest TuneRequest);

        [PreserveSig]
        int get_PreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        int put_PreferredComponentTypes([In] IComponentTypes ComponentTypes);

        [PreserveSig]
        int get_SignalStrength([Out] out int Strength);

        [PreserveSig]
        int TriggerSignalEvents([In] int Interval);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("07DDC146-FC3D-11d2-9D8C-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITuneRequest
    {
        [PreserveSig]
        int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        int get_Components([Out] out IComponents Components);

        [PreserveSig]
        int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        int put_Locator([In] ILocator Locator);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("3D7C353C-0D04-45f1-A742-F97CC1188DC8"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBSLocator : IDigitalLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_SignalPolarisation([Out] out Polarisation PolarisationVal);

        [PreserveSig]
        int put_SignalPolarisation([In] Polarisation PolarisationVal);

        [PreserveSig]
        int get_WestPosition([Out, MarshalAs(UnmanagedType.VariantBool)] out bool WestLongitude);

        [PreserveSig]
        int put_WestPosition([In, MarshalAs(UnmanagedType.VariantBool)] bool WestLongitude);

        [PreserveSig]
        int get_OrbitalPosition([Out] out int longitude);

        [PreserveSig]
        int put_OrbitalPosition([In] int longitude);

        [PreserveSig]
        int get_Azimuth([Out] out int Azimuth);

        [PreserveSig]
        int put_Azimuth([In] int Azimuth);

        [PreserveSig]
        int get_Elevation([Out] out int Elevation);

        [PreserveSig]
        int put_Elevation([In] int Elevation);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("CDF7BE60-D954-42fd-A972-78971958E470"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBSTuningSpace : IDVBTuningSpace2
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IDVBTuningSpace Methods

        [PreserveSig]
        new int get_SystemType([Out] out DVBSystemType SysType);

        [PreserveSig]
        new int put_SystemType([In] DVBSystemType SysType);

        #endregion

        #region IDVBTuningSpace2 Methods

        [PreserveSig]
        new int get_NetworkID([Out] out int NetworkID);

        [PreserveSig]
        new int put_NetworkID([In] int NetworkID);

        #endregion

        [PreserveSig]
        int get_LowOscillator([Out] out int LowOscillator);

        [PreserveSig]
        int put_LowOscillator([In] int LowOscillator);

        [PreserveSig]
        int get_HighOscillator([Out] out int HighOscillator);

        [PreserveSig]
        int put_HighOscillator([In] int HighOscillator);

        [PreserveSig]
        int get_LNBSwitch([Out] out int LNBSwitch);

        [PreserveSig]
        int put_LNBSwitch([In] int LNBSwitch);

        [PreserveSig]
        int get_InputRange([Out, MarshalAs(UnmanagedType.BStr)] out string InputRange);

        [PreserveSig]
        int put_InputRange([Out, MarshalAs(UnmanagedType.BStr)] string InputRange);

        [PreserveSig]
        int get_SpectralInversion([Out] out SpectralInversion SpectralInversionVal);

        [PreserveSig]
        int put_SpectralInversion([In] SpectralInversion SpectralInversionVal);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("286D7F89-760C-4F89-80C4-66841D2507AA"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ILocator
    {
        [PreserveSig]
        int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        int put_SymbolRate([In] int Rate);

        [PreserveSig]
        int Clone([Out] out ILocator NewLocator);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("ADA0B268-3B19-4e5b-ACC4-49F852BE13BA"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTuningSpace : ITuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        [PreserveSig]
        int get_SystemType([Out] out DVBSystemType SysType);

        [PreserveSig]
        int put_SystemType([In] DVBSystemType SysType);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("843188B4-CE62-43db-966B-8145A094E040"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTuningSpace2 : IDVBTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IDVBTuningSpace Methods

        [PreserveSig]
        new int get_SystemType([Out] out DVBSystemType SysType);

        [PreserveSig]
        new int put_SystemType([In] DVBSystemType SysType);

        #endregion

        [PreserveSig]
        int get_NetworkID([Out] out int NetworkID);

        [PreserveSig]
        int put_NetworkID([In] int NetworkID);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("8B8EB248-FC2B-11d2-9D8C-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumTuningSpaces
    {
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ITuningSpace[] rgelt,
            [In] IntPtr pceltFetched
            );

        int Skip([In] int celt);

        int Reset();

        int Clone([Out] out IEnumTuningSpaces ppEnum);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0D6F567E-A636-42bb-83BA-CE4C1704AFA2"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTuneRequest : ITuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        [PreserveSig]
        int get_ONID([Out] out int ONID);

        [PreserveSig]
        int put_ONID([In] int ONID);

        [PreserveSig]
        int get_TSID([Out] out int TSID);

        [PreserveSig]
        int put_TSID([In] int TSID);

        [PreserveSig]
        int get_SID([Out] out int SID);

        [PreserveSig]
        int put_SID([In] int SID);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("8664DA16-DDA2-42ac-926A-C18F9127C302"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTLocator : IDigitalLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_Bandwidth([Out] out int BandwidthVal);

        [PreserveSig]
        int put_Bandwidth([In] int BandwidthVal);

        [PreserveSig]
        int get_LPInnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        int put_LPInnerFEC([In] FECMethod FEC);

        [PreserveSig]
        int get_LPInnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int put_LPInnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int get_HAlpha([Out] out HierarchyAlpha Alpha);

        [PreserveSig]
        int put_HAlpha([In] HierarchyAlpha Alpha);

        [PreserveSig]
        int get_Guard([Out] out GuardInterval GI);

        [PreserveSig]
        int put_Guard([In] GuardInterval GI);

        [PreserveSig]
        int get_Mode([Out] out TransmissionMode mode);

        [PreserveSig]
        int put_Mode([In] TransmissionMode mode);

        [PreserveSig]
        int get_OtherFrequencyInUse([Out, MarshalAs(UnmanagedType.VariantBool)] out bool OtherFrequencyInUseVal);

        [PreserveSig]
        int put_OtherFrequencyInUse([In, MarshalAs(UnmanagedType.VariantBool)] bool OtherFrequencyInUseVal);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("8A674B4A-1F63-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumComponentTypes
    {
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IComponentType[] rgelt,
            [In] IntPtr pceltFetched
            );

        int Skip([In] int celt);

        int Reset();

        int Clone([Out] out IEnumComponentTypes ppEnum);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("6A340DC0-0311-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponentType
    {
        [PreserveSig]
        int get_Category(
            [Out] out ComponentCategory Category
            );

        [PreserveSig]
        int put_Category(
            [In] ComponentCategory Category
            );

        [PreserveSig]
        int get_MediaMajorType(
            [Out, MarshalAs(UnmanagedType.BStr)] out string MediaMajorType
            );

        [PreserveSig]
        int put_MediaMajorType(
            [In, MarshalAs(UnmanagedType.BStr)] string MediaMajorType
            );

        [PreserveSig]
        int get__MediaMajorType(
            [Out] out Guid MediaMajorType
            );

        [PreserveSig]
        int put__MediaMajorType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaMajorType
            );

        [PreserveSig]
        int get_MediaSubType(
            [Out, MarshalAs(UnmanagedType.BStr)] out string MediaSubType
            );

        [PreserveSig]
        int put_MediaSubType(
            [In, MarshalAs(UnmanagedType.BStr)] string MediaSubType
            );

        [PreserveSig]
        int get__MediaSubType(
            [Out] out Guid MediaSubType
            );

        [PreserveSig]
        int put__MediaSubType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaSubType
            );

        [PreserveSig]
        int get_MediaFormatType(
            [Out, MarshalAs(UnmanagedType.BStr)] out string MediaFormatType
            );

        [PreserveSig]
        int put_MediaFormatType(
            [In, MarshalAs(UnmanagedType.BStr)] string MediaFormatType
            );

        [PreserveSig]
        int get__MediaFormatType(
            [Out] out Guid MediaFormatType
            );

        [PreserveSig]
        int put__MediaFormatType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaFormatType
            );

        [PreserveSig]
        int get_MediaType(
            [Out] AMMediaType MediaType
            );

        [PreserveSig]
        int put_MediaType(
            [In] AMMediaType MediaType
            );

        [PreserveSig]
        int Clone(
            [Out] out IComponentType NewCT
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0DC13D4A-0313-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponentTypes
    {
        [PreserveSig]
        int get_Count(
            [Out] out int Count
            );

        [PreserveSig]
        int get__NewEnum(
        [Out] out IEnumVARIANT ppNewEnum
            );

        [PreserveSig]
        int EnumComponentTypes(
            [Out] out IEnumComponentTypes ppNewEnum
            );

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out IComponentType TuningSpace
            );

        [PreserveSig]
        int put_Item(
            [In] object NewIndex,
            [In] IComponentType ComponentType
            );

        [PreserveSig]
        int Add(
            [In] IComponentType ComponentType,
            [Out] out object NewIndex
            );

        [PreserveSig]
        int Remove(
            [In] object Index
            );

        [PreserveSig]
        int Clone(
            [Out] out IComponentTypes NewList
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2A6E2939-2595-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumComponents
    {
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IComponent[] rgelt,
            [In] IntPtr pceltFetched
            );

        int Skip([In] int celt);

        int Reset();

        int Clone([Out] out IEnumComponents ppEnum);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("1A5576FC-0E19-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponent
    {
        [PreserveSig]
        int get_Type([Out] out IComponentType CT);

        [PreserveSig]
        int put_Type([In] IComponentType CT);

        [PreserveSig]
        int get_DescLangID([Out] out int LangID);

        [PreserveSig]
        int put_DescLangID([In] int LangID);

        [PreserveSig]
        int get_Status([Out] out ComponentStatus Status);

        [PreserveSig]
        int put_Status([In] ComponentStatus Status);

        [PreserveSig]
        int get_Description([Out, MarshalAs(UnmanagedType.BStr)] out string Description);

        [PreserveSig]
        int put_Description([In, MarshalAs(UnmanagedType.BStr)] string Description);

        [PreserveSig]
        int Clone([Out] out IComponent NewComponent);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("FCD01846-0E19-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponents
    {
        [PreserveSig]
        int get_Count(
            [Out] out int Count
            );

        [PreserveSig]
        int get__NewEnum(
        [Out] out IEnumVARIANT ppNewEnum
            );

        [PreserveSig]
        int EnumComponents(
            [Out] out IEnumComponents ppNewEnum
            );

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out IComponent TuningSpace
            );

        [PreserveSig]
        int Add(
            [In] IComponent Component,
            [Out] out object NewIndex
            );

        [PreserveSig]
        int Remove(
            [In] object Index
            );

        [PreserveSig]
        int Clone(
            [Out] out IComponents NewList
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2A6E293C-2595-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAnalogTVTuningSpace : ITuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        [PreserveSig]
        int get_MinChannel([Out] out int MinChannelVal);

        [PreserveSig]
        int put_MinChannel([In] int NewMinChannelVal);

        [PreserveSig]
        int get_MaxChannel([Out] out int MaxChannelVal);

        [PreserveSig]
        int put_MaxChannel([In] int NewMaxChannelVal);

        [PreserveSig]
        int get_InputType([Out] out TunerInputType InputTypeVal);

        [PreserveSig]
        int put_InputType([In] TunerInputType NewInputTypeVal);

        [PreserveSig]
        int get_CountryCode([Out] out int CountryCodeVal);

        [PreserveSig]
        int put_CountryCode([In] int NewCountryCodeVal);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0369B4E1-45B6-11d3-B650-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCChannelTuneRequest : IChannelTuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        #region IChannelTuneRequest Methods

        [PreserveSig]
        new int get_Channel([Out] out int Channel);

        [PreserveSig]
        new int put_Channel([In] int Channel);

        #endregion

        [PreserveSig]
        int get_MinorChannel([Out] out int MinorChannel);

        [PreserveSig]
        int put_MinorChannel([In] int MinorChannel);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("BF8D986F-8C2B-4131-94D7-4D3D9FCC21EF"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCLocator : IDigitalLocator
    {
        #region ILocator Methods
        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_PhysicalChannel([Out] out int PhysicalChannel);

        [PreserveSig]
        int put_PhysicalChannel([In] int PhysicalChannel);

        [PreserveSig]
        int get_TSID([Out] out int TSID);

        [PreserveSig]
        int put_TSID([In] int TSID);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0369B4E2-45B6-11d3-B650-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCTuningSpace : IAnalogTVTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IAnalogTVTuningSpace Methods

        [PreserveSig]
        new int get_MinChannel([Out] out int MinChannelVal);

        [PreserveSig]
        new int put_MinChannel([In] int NewMinChannelVal);

        [PreserveSig]
        new int get_MaxChannel([Out] out int MaxChannelVal);

        [PreserveSig]
        new int put_MaxChannel([In] int NewMaxChannelVal);

        [PreserveSig]
        new int get_InputType([Out] out TunerInputType InputTypeVal);

        [PreserveSig]
        new int put_InputType([In] TunerInputType NewInputTypeVal);

        [PreserveSig]
        new int get_CountryCode([Out] out int CountryCodeVal);

        [PreserveSig]
        new int put_CountryCode([In] int NewCountryCodeVal);

        #endregion

        [PreserveSig]
        int get_MinMinorChannel([Out] out int MinMinorChannelVal);

        [PreserveSig]
        int put_MinMinorChannel([In] int NewMinMinorChannelVal);

        [PreserveSig]
        int get_MaxMinorChannel([Out] out int MaxMinorChannelVal);

        [PreserveSig]
        int put_MaxMinorChannel([In] int NewMaxMinorChannelVal);

        [PreserveSig]
        int get_MinPhysicalChannel([Out] out int MinPhysicalChannelVal);

        [PreserveSig]
        int put_MinPhysicalChannel([In] int NewMinPhysicalChannelVal);

        [PreserveSig]
        int get_MaxPhysicalChannel([Out] out int MaxPhysicalChannelVal);

        [PreserveSig]
        int put_MaxPhysicalChannel([In] int NewMaxPhysicalChannelVal);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0369B4E0-45B6-11d3-B650-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IChannelTuneRequest : ITuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        [PreserveSig]
        int get_Channel([Out] out int Channel);

        [PreserveSig]
        int put_Channel([In] int Channel);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("6E42F36E-1DD2-43c4-9F78-69D25AE39034"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBCLocator : IDigitalLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("19B595D8-839A-47F0-96DF-4F194F3C768C"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalLocator : ILocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("612AA885-66CF-4090-BA0A-566F5312E4CA"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCLocator2 : IATSCLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        #region IATSCLocator Methods

        [PreserveSig]
        new int get_PhysicalChannel([Out] out int PhysicalChannel);

        [PreserveSig]
        new int put_PhysicalChannel([In] int PhysicalChannel);

        [PreserveSig]
        new int get_TSID([Out] out int TSID);

        [PreserveSig]
        new int put_TSID([In] int TSID);

        #endregion

        [PreserveSig]
        int get_ProgramNumber([Out] out int ProgramNumber);

        [PreserveSig]
        int put_ProgramNumber([In] int ProgramNumber);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("48F66A11-171A-419A-9525-BEEECD51584C"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalCableLocator : IATSCLocator2
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        #region IATSCLocator Methods

        [PreserveSig]
        new int get_PhysicalChannel([Out] out int PhysicalChannel);

        [PreserveSig]
        new int put_PhysicalChannel([In] int PhysicalChannel);

        [PreserveSig]
        new int get_TSID([Out] out int TSID);

        [PreserveSig]
        new int put_TSID([In] int TSID);

        #endregion

        #region IATSCLocator2 Methods

        [PreserveSig]
        new int get_ProgramNumber([Out] out int ProgramNumber);

        [PreserveSig]
        new int put_ProgramNumber([In] int ProgramNumber);

        #endregion
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("013F9F9C-B449-4ec7-A6D2-9D4F2FC70AE5"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalCableTuningSpace : IATSCTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IAnalogTVTuningSpace Methods

        [PreserveSig]
        new int get_MinChannel([Out] out int MinChannelVal);

        [PreserveSig]
        new int put_MinChannel([In] int NewMinChannelVal);

        [PreserveSig]
        new int get_MaxChannel([Out] out int MaxChannelVal);

        [PreserveSig]
        new int put_MaxChannel([In] int NewMaxChannelVal);

        [PreserveSig]
        new int get_InputType([Out] out TunerInputType InputTypeVal);

        [PreserveSig]
        new int put_InputType([In] TunerInputType NewInputTypeVal);

        [PreserveSig]
        new int get_CountryCode([Out] out int CountryCodeVal);

        [PreserveSig]
        new int put_CountryCode([In] int NewCountryCodeVal);

        #endregion

        #region IATSCTuningSpace Methods

        [PreserveSig]
        new int get_MinMinorChannel([Out] out int MinMinorChannelVal);

        [PreserveSig]
        new int put_MinMinorChannel([In] int NewMinMinorChannelVal);

        [PreserveSig]
        new int get_MaxMinorChannel([Out] out int MaxMinorChannelVal);

        [PreserveSig]
        new int put_MaxMinorChannel([In] int NewMaxMinorChannelVal);

        [PreserveSig]
        new int get_MinPhysicalChannel([Out] out int MinPhysicalChannelVal);

        [PreserveSig]
        new int put_MinPhysicalChannel([In] int NewMinPhysicalChannelVal);

        [PreserveSig]
        new int get_MaxPhysicalChannel([Out] out int MaxPhysicalChannelVal);

        [PreserveSig]
        new int put_MaxPhysicalChannel([In] int NewMaxPhysicalChannelVal);

        #endregion

        [PreserveSig]
        int get_MinMajorChannel([Out] out int MinMajorChannelVal);

        [PreserveSig]
        int put_MinMajorChannel([In] int NewMinMajorChannelVal);

        [PreserveSig]
        int get_MaxMajorChannel([Out] out int MaxMajorChannelVal);

        [PreserveSig]
        int put_MaxMajorChannel([In] int NewMaxMajorChannelVal);

        [PreserveSig]
        int get_MinSourceID([Out] out int MinSourceIDVal);

        [PreserveSig]
        int put_MinSourceID([In] int NewMinSourceIDVal);

        [PreserveSig]
        int get_MaxSourceID([Out] out int MaxSourceIDVal);

        [PreserveSig]
        int put_MaxSourceID([In] int NewMaxSourceIDVal);
    }    

    #endregion
}
