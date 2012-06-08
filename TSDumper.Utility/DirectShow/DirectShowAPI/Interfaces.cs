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
    /// From BDA_CHANGE_STATE
    /// </summary>
    public enum BDAChangeState
    {
        ChangesComplete = 0,
        ChangesPending
    }

    /// <summary>
    /// From BDANODE_DESCRIPTOR
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BDANodeDescriptor
    {
        public int ulBdaNodeType;
        public Guid guidFunction;
        public Guid guidName;
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaControl
    {
        [PreserveSig]
        int Run();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int GetState([In] int msTimeout, [Out] out FilterState pfs);

        [PreserveSig]
        int StopWhenReady();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("0000010c-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        int GetClassID(
            [Out] out Guid pClassID);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("b61178d1-a2d9-11cf-9e53-00aa00a216a1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsPin
    {
        /// <summary>
        /// The caller must free the returned structures, using the CoTaskMemFree function
        /// </summary>
        [PreserveSig]
        int KsQueryMediums(
            out IntPtr ip);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyBag
    {
        [PreserveSig]
        int Read(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [Out, MarshalAs(UnmanagedType.Struct)] out object pVar,
            [In] IErrorLog pErrorLog
            );

        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In, MarshalAs(UnmanagedType.Struct)] ref object pVar
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IErrorLog
    {
        [PreserveSig]
        int AddError(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("F84E2AB0-3C6B-45E3-A0FC-8669D4B81F11"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DiseqCommand
    {
        [PreserveSig]
        int put_EnableDiseqCommands(
            [In] byte bEnable
            );

        [PreserveSig]
        int put_DiseqLNBSource(
            [In] int ulLNBSource
            );

        [PreserveSig]
        int put_DiseqUseToneBurst(
            [In] byte bUseToneBurst
            );

        [PreserveSig]
        int put_DiseqRepeats(
            [In] int ulRepeats
            );

        [PreserveSig]
        int put_DiseqSendCommand(
            [In] int ulRequestId,
            [In] int ulcbCommandLen,
            [In] ref byte pbCommand
            );

        [PreserveSig]
        int get_DiseqResponse(
            [In] int ulRequestId,
            [In, Out] ref int pulcbResponseLen,
            [In, Out] ref byte pbResponse
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("EF30F379-985B-4d10-B640-A79D5E04E1E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DigitalDemodulator
    {
        [PreserveSig]
        int put_ModulationType([In] ref ModulationType pModulationType);

        [PreserveSig]
        int get_ModulationType([Out] out ModulationType pModulationType);

        [PreserveSig]
        int put_InnerFECMethod([In] ref FECMethod pFECMethod);

        [PreserveSig]
        int get_InnerFECMethod([Out] out FECMethod pFECMethod);

        [PreserveSig]
        int put_InnerFECRate([In] ref BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int get_InnerFECRate([Out] out BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int put_OuterFECMethod([In] ref FECMethod pFECMethod);

        [PreserveSig]
        int get_OuterFECMethod([Out] out FECMethod pFECMethod);

        [PreserveSig]
        int put_OuterFECRate([In] ref BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int get_OuterFECRate([Out] out BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int put_SymbolRate([In] ref int pSymbolRate);

        [PreserveSig]
        int get_SymbolRate([Out] out int pSymbolRate);

        [PreserveSig]
        int put_SpectralInversion([In] ref SpectralInversion pSpectralInversion);

        [PreserveSig]
        int get_SpectralInversion([Out] out SpectralInversion pSpectralInversion);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("525ED3EE-5CF3-4E1E-9A06-5368A84F9A6E")]
    public interface IBDA_DigitalDemodulator2 : IBDA_DigitalDemodulator
    {
        #region IBDA_DigitalDemodulator Methods

        [PreserveSig]
        new int put_ModulationType(
            [In] ref ModulationType pModulationType
            );

        [PreserveSig]
        new int get_ModulationType(
            [Out] out ModulationType pModulationType
            );

        [PreserveSig]
        new int put_InnerFECMethod(
            [In] ref FECMethod pFECMethod
            );

        [PreserveSig]
        new int get_InnerFECMethod(
            [Out] out FECMethod pFECMethod
            );

        [PreserveSig]
        new int put_InnerFECRate(
            [In] ref BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int get_InnerFECRate(
            [Out] out BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int put_OuterFECMethod(
            [In] ref FECMethod pFECMethod
            );

        [PreserveSig]
        new int get_OuterFECMethod(
            [Out] out FECMethod pFECMethod
            );

        [PreserveSig]
        new int put_OuterFECRate(
            [In] ref BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int get_OuterFECRate(
            [Out] out BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int put_SymbolRate(
            [In] ref int pSymbolRate
            );

        [PreserveSig]
        new int get_SymbolRate(
            [Out] out int pSymbolRate
            );

        [PreserveSig]
        new int put_SpectralInversion(
            [In] ref SpectralInversion pSpectralInversion
            );

        [PreserveSig]
        new int get_SpectralInversion(
            [Out] out SpectralInversion pSpectralInversion
            );

        #endregion

        [PreserveSig]
        int put_GuardInterval(
            [In] ref GuardInterval pGuardInterval
            );

        [PreserveSig]
        int get_GuardInterval(
            [In, Out] ref GuardInterval pGuardInterval
            );

        [PreserveSig]
        int put_TransmissionMode(
            [In] ref TransmissionMode pTransmissionMode
            );

        [PreserveSig]
        int get_TransmissionMode(
            [In, Out] ref TransmissionMode pTransmissionMode
            );

        [PreserveSig]
        int put_RollOff(
            [In] ref RollOff pRollOff
            );

        [PreserveSig]
        int get_RollOff(
            [In, Out] ref RollOff pRollOff
            );

        [PreserveSig]
        int put_Pilot(
            [In] ref Pilot pPilot
            );

        [PreserveSig]
        int get_Pilot(
            [In, Out] ref Pilot pPilot
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("1347D106-CF3A-428a-A5CB-AC0D9A2A4338"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_SignalStatistics
    {
        [PreserveSig]
        int put_SignalStrength([In] int lDbStrength);

        [PreserveSig]
        int get_SignalStrength([Out] out int plDbStrength);

        [PreserveSig]
        int put_SignalQuality([In] int lPercentQuality);

        [PreserveSig]
        int get_SignalQuality([Out] out int plPercentQuality);

        [PreserveSig]
        int put_SignalPresent([In, MarshalAs(UnmanagedType.U1)] bool fPresent);

        [PreserveSig]
        int get_SignalPresent([Out, MarshalAs(UnmanagedType.U1)] out bool pfPresent);

        [PreserveSig]
        int put_SignalLocked([In, MarshalAs(UnmanagedType.U1)] bool fLocked);

        [PreserveSig]
        int get_SignalLocked([Out, MarshalAs(UnmanagedType.U1)] out bool pfLocked);

        [PreserveSig]
        int put_SampleTime([In] int lmsSampleTime);

        [PreserveSig]
        int get_SampleTime([Out] out int plmsSampleTime);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("79B56888-7FEA-4690-B45D-38FD3C7849BE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_Topology
    {
        [PreserveSig]
        int GetNodeTypes(
            [Out] out int pulcNodeTypes,
            [In] int ulcNodeTypesMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 1)] int[] rgulNodeTypes
            );

        [PreserveSig]
        int GetNodeDescriptors(
            [Out] out int ulcNodeDescriptors,
            [In] int ulcNodeDescriptorsMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 1)] BDANodeDescriptor[] rgNodeDescriptors
            );

        [PreserveSig]
        int GetNodeInterfaces(
            [In] int ulNodeType,
            [Out] out int pulcInterfaces,
            [In] int ulcInterfacesMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 2)] Guid[] rgguidInterfaces
            );

        [PreserveSig]
        int GetPinTypes(
            [Out] out int pulcPinTypes,
            [In] int ulcPinTypesMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 1)] int[] rgulPinTypes
            );

        [PreserveSig]
        int GetTemplateConnections(
            [Out] out int pulcConnections,
            [In] int ulcConnectionsMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 1)] BDATemplateConnection[] rgConnections
            );

        [PreserveSig]
        int CreatePin(
            [In] int ulPinType,
            [Out] out int pulPinId
            );

        [PreserveSig]
        int DeletePin([In] int ulPinId);

        [PreserveSig]
        int SetMediaType(
            [In] int ulPinId,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType
            );

        [PreserveSig]
        int SetMedium(
            [In] int ulPinId,
            [In] RegPinMedium pMedium
            );

        [PreserveSig]
        int CreateTopology(
            [In] int ulInputPinId,
            [In] int ulOutputPinId
            );

        [PreserveSig]
        int GetControlNode(
            [In] int ulInputPinId,
            [In] int ulOutputPinId,
            [In] int ulNodeType,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppControlNode // IUnknown
            );

    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("FD0A5AF3-B41D-11d2-9C95-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DeviceControl
    {
        [PreserveSig]
        int StartChanges();

        [PreserveSig]
        int CheckChanges();

        [PreserveSig]
        int CommitChanges();

        [PreserveSig]
        int GetChangeState([Out] out BDAChangeState pState);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("71985F47-1CA1-11d3-9CC8-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_FrequencyFilter
    {
        [PreserveSig]
        int put_Autotune([In] int ulTransponder);

        [PreserveSig]
        int get_Autotune([Out] out int pulTransponder);

        [PreserveSig]
        int put_Frequency([In] int ulFrequency);

        [PreserveSig]
        int get_Frequency([Out] out int pulFrequency);

        [PreserveSig]
        int put_Polarity([In] Polarisation Polarity);

        [PreserveSig]
        int get_Polarity([Out] out Polarisation pPolarity);

        [PreserveSig]
        int put_Range([In] int ulRange);

        [PreserveSig]
        int get_Range([Out] out int pulRange);

        [PreserveSig]
        int put_Bandwidth([In] int ulBandwidth);

        [PreserveSig]
        int get_Bandwidth([Out] out int pulBandwidth);

        [PreserveSig]
        int put_FrequencyMultiplier([In] int ulMultiplier);

        [PreserveSig]
        int get_FrequencyMultiplier([Out] out int pulMultiplier);
    }

    #endregion
}
