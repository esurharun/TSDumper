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
using System.Threading;
using System.Reflection;

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a DirectShow graph.
    /// </summary>
    public abstract class DirectShowGraph
    {
        /// <summary>
        /// Get the full assembly version number.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
            }
        }

        /// <summary>
        /// Get the capture builder for the graph.
        /// </summary>
        public ICaptureGraphBuilder2 CaptureGraphBuilder { get { return (captureBuilder); } }
        /// <summary>
        /// Get the graph builder for the graph.
        /// </summary>
        public IFilterGraph2 GraphBuilder { get { return (graphBuilder); } }
        /// <summary>
        /// Get the media control for the graph.
        /// </summary>
        public IMediaControl MediaControl { get { return (mediaControl); } }

        /// <summary>
        /// Get the component name for the graph.
        /// </summary>
        public string ComponentName { get { return (componentName); } }

        private ICaptureGraphBuilder2 captureBuilder = null;
        private IFilterGraph2 graphBuilder = null;
        private IMediaControl mediaControl = null;

        private string componentName;        

        private int reply;

        /// <summary>
        /// Initialize a new instance of the DirectShowGraph class.
        /// </summary>
        /// <param name="componentName">The name of the component owning the graph.</param>
        protected DirectShowGraph(string componentName)
        {
            this.componentName = componentName;
        }

        /// <summary>
        /// Decompose the graph and release resources.
        /// </summary>
        public virtual void Dispose()
        {
            FilterState filterState;
            mediaControl.GetState(0, out filterState);
            if (filterState != FilterState.Stopped)
            {
                LogMessage("Stopping graph");
                reply = mediaControl.Stop();
                DsError.ThrowExceptionForHR(reply);
            }

            LogMessage("Removing filters");
            FilterGraphTools.RemoveAllFilters(graphBuilder);

            LogMessage("Releasing Graph Builder");
            Marshal.ReleaseComObject(graphBuilder);
            graphBuilder = null;

            LogMessage("Releasing Capture Builder");
            Marshal.ReleaseComObject(captureBuilder);
            captureBuilder = null;
        }

        /// <summary>
        /// Start the graph.
        /// </summary>
        /// <returns></returns>
        public virtual bool Play()
        {
            LogMessage("Graph playing");
            reply = mediaControl.Run();
            return (reply >= 0);
        }

        /// <summary>
        /// Stop the graph.
        /// </summary>
        public virtual void Stop()
        {
            LogMessage("Graph stopped");            
            reply = MediaControl.Stop();
            DsError.ThrowExceptionForHR(reply);
        }

        /// <summary>
        /// Pause or restart the graph.
        /// </summary>
        /// <param name="pause">True to pause the graph; false otherwise.</param>
        public void Pause(bool pause)
        {
            int reply;

            if (pause)
            {
                reply = mediaControl.Pause();
                Thread.Sleep(250);
            }
            else
                reply = mediaControl.Run();

            DsError.ThrowExceptionForHR(reply);
        }

        /// <summary>
        /// Build the graph.
        /// </summary>
        protected virtual void BuildGraph()
        {
            captureBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            graphBuilder = (IFilterGraph2)new FilterGraph();
            captureBuilder.SetFiltergraph(graphBuilder);

            mediaControl = graphBuilder as IMediaControl;
        }

        internal IPin FindPin(IBaseFilter filter, string name)
        {
            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinInfo pinInfo;
                reply = pins[0].QueryPinInfo(out pinInfo);
                if (reply != 0)
                {
                    FilterInfo filterInfo;
                    filter.QueryFilterInfo(out filterInfo);
                    throw (new InvalidOperationException("The pin '" + name + "' could not be located for filter '" + filterInfo.achName + "'"));
                }

                if (pinInfo.name.StartsWith(name))
                {
                    Marshal.ReleaseComObject(enumPins);
                    return (pins[0]);
                }
            }

            Marshal.ReleaseComObject(enumPins);

            return (null);
        }

        internal IPin FindPin(IBaseFilter filter, PinDirection direction)
        {
            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinDirection pinDirection;

                reply = pins[0].QueryDirection(out pinDirection);
                DsError.ThrowExceptionForHR(reply);

                if (pinDirection == direction)
                {
                    Marshal.ReleaseComObject(enumPins);
                    return (pins[0]);
                }
            }

            Marshal.ReleaseComObject(enumPins);

            return (null);
        }

        internal IPin FindPin(IBaseFilter filter, Guid mediaType, PinDirection direction)
        {
            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinDirection pinDirection;

                reply = pins[0].QueryDirection(out pinDirection);
                DsError.ThrowExceptionForHR(reply);

                if (pinDirection == direction)
                {
                    bool mediaReply = DirectShowGraph.checkMediaType(pins[0], mediaType);
                    if (mediaReply)
                    {
                        Marshal.ReleaseComObject(enumPins);
                        return (pins[0]);
                    }
                }
            }

            Marshal.ReleaseComObject(enumPins);

            return (null);
        }

        internal IPin FindPin(IBaseFilter filter, Guid mediaType, Guid mediaSubType, PinDirection direction)
        {
            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinDirection pinDirection;

                reply = pins[0].QueryDirection(out pinDirection);
                DsError.ThrowExceptionForHR(reply);

                if (pinDirection == direction)
                {
                    bool mediaReply = DirectShowGraph.checkMediaTypes(pins[0], mediaType, mediaSubType);
                    if (mediaReply)
                    {
                        Marshal.ReleaseComObject(enumPins);
                        return (pins[0]);
                    }
                }
            }

            Marshal.ReleaseComObject(enumPins);

            return (null);
        }

        internal static IPin LocatePin(IBaseFilter filter, PinDirection direction)
        {
            int reply = 0;

            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinDirection pinDirection;

                reply = pins[0].QueryDirection(out pinDirection);
                DsError.ThrowExceptionForHR(reply);

                if (pinDirection == direction)
                {
                    Marshal.ReleaseComObject(enumPins);
                    return (pins[0]);
                }
            }

            Marshal.ReleaseComObject(enumPins);

            return (null);
        }

        internal static IPin LocatePin(IBaseFilter filter, Guid mediaType, Guid mediaSubType, PinDirection direction)
        {
            int reply = 0;

            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinDirection pinDirection;

                reply = pins[0].QueryDirection(out pinDirection);
                DsError.ThrowExceptionForHR(reply);

                if (pinDirection == direction)
                {
                    bool mediaReply = DirectShowGraph.checkMediaTypes(pins[0], mediaType, mediaSubType);
                    if (mediaReply)
                    {
                        Marshal.ReleaseComObject(enumPins);
                        return (pins[0]);
                    }
                }
            }

            Marshal.ReleaseComObject(enumPins);

            return (null);
        }

        private static bool checkMediaType(IPin pin, Guid mediaType)
        {
            int reply = 0;

            IEnumMediaTypes enumMediaTypes = null;
            AMMediaType[] mediaTypes = new AMMediaType[1];

            reply = pin.EnumMediaTypes(out enumMediaTypes);
            DsError.ThrowExceptionForHR(reply);

            while (enumMediaTypes.Next(mediaTypes.Length, mediaTypes, IntPtr.Zero) == 0)
            {
                foreach (AMMediaType currentMediaType in mediaTypes)
                {
                    if (currentMediaType.majorType == mediaType)
                    {
                        Marshal.ReleaseComObject(enumMediaTypes);
                        return (true);
                    }
                }
            }

            Marshal.ReleaseComObject(enumMediaTypes);

            return (false);
        }

        private static bool checkMediaTypes(IPin pin, Guid mediaType, Guid mediaSubType)
        {
            int reply = 0;

            IEnumMediaTypes enumMediaTypes = null;
            AMMediaType[] mediaTypes = new AMMediaType[1];

            reply = pin.EnumMediaTypes(out enumMediaTypes);
            DsError.ThrowExceptionForHR(reply);

            while (enumMediaTypes.Next(mediaTypes.Length, mediaTypes, IntPtr.Zero) == 0)
            {
                foreach (AMMediaType currentMediaType in mediaTypes)
                {
                    if (currentMediaType != null)
                    {
                        if (currentMediaType.majorType == mediaType &&
                            currentMediaType.subType == mediaSubType)
                        {
                            Marshal.ReleaseComObject(enumMediaTypes);
                            return (true);
                        }
                    }
                }
            }

            Marshal.ReleaseComObject(enumMediaTypes);

            return (false);
        }

        /// <summary>
        /// Log the filters in the graph.
        /// </summary>
        protected void LogFilters()
        {
            LogMessage("Logging filters");

            IEnumFilters enumFilters = null;
            IBaseFilter[] filters = new IBaseFilter[1];

            reply = graphBuilder.EnumFilters(out enumFilters);
            DsError.ThrowExceptionForHR(reply);

            while (enumFilters.Next(filters.Length, filters, IntPtr.Zero) == 0)
            {
                FilterInfo filterInfo;

                reply = filters[0].QueryFilterInfo(out filterInfo);
                DsError.ThrowExceptionForHR(reply);

                LogMessage("Filter: " + filterInfo.achName);
                
                logPins(filters[0]);
                if (filterInfo.pGraph != null)
                    Marshal.ReleaseComObject(filterInfo.pGraph);
                Marshal.ReleaseComObject(filters[0]);
            }

            Marshal.ReleaseComObject(enumFilters);

            LogMessage("All filters logged");
        }

        private void logPins(IBaseFilter filter)
        {
            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1];

            reply = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(reply);
            
            if (enumPins == null)
                return;

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                PinInfo pinInfo;

                reply = pins[0].QueryPinInfo(out pinInfo);
                DsError.ThrowExceptionForHR(reply);
                
                IPin connectedPin = null;
                reply = pins[0].ConnectedTo(out connectedPin);
                if (connectedPin != null)
                {
                    PinInfo connectedPinInfo;
                    reply = connectedPin.QueryPinInfo(out connectedPinInfo);
                    DsError.ThrowExceptionForHR(reply);
                    
                    FilterInfo filterInfo;
                    reply = connectedPinInfo.filter.QueryFilterInfo(out filterInfo);
                    DsError.ThrowExceptionForHR(reply);

                    LogMessage("Pin: " + pinInfo.name + " Connected to: " + filterInfo.achName);
                    logMediaTypes(pins[0]);
                    if (filterInfo.pGraph != null)
                        Marshal.ReleaseComObject(filterInfo.pGraph);
                }
                else
                {
                    LogMessage("Pin: " + pinInfo.name + " not connected");
                    logMediaTypes(pins[0]);
                }

                Marshal.ReleaseComObject(pins[0]);
                if (connectedPin != null)
                    Marshal.ReleaseComObject(connectedPin);
            }

            Marshal.ReleaseComObject(enumPins);
        }

        private void logMediaTypes(IPin pin)
        {
            IEnumMediaTypes mediaTypes = null;
            AMMediaType[] mediaType = new AMMediaType[1];

            AMMediaType connectedMediaType = new AMMediaType();
            reply = pin.ConnectionMediaType(connectedMediaType);

            reply = pin.EnumMediaTypes(out mediaTypes);
            if (reply != 0)
            {
                LogMessage("Media types cannot be determined at this time (not connected yet?)");
                return;
            }

            while (mediaTypes.Next(mediaType.Length, mediaType, IntPtr.Zero) == 0)
            {
                foreach (AMMediaType currentMediaType in mediaType)
                {
                    PinInfo pinInfo;
                    reply = pin.QueryPinInfo(out pinInfo);
                    DsError.ThrowExceptionForHR(reply);                    

                    string majorType = TranslateMediaMajorType(currentMediaType.majorType);
                    string subType = TranslateMediaSubType(currentMediaType.subType);                    

                    string connectedComment;

                    if (currentMediaType.majorType == connectedMediaType.majorType && currentMediaType.subType == connectedMediaType.subType)
                        connectedComment = "** Connected **";
                    else
                        connectedComment = string.Empty;

                    LogMessage("Media type: " +
                        majorType + " ; " +
                        subType + " " +
                        currentMediaType.fixedSizeSamples + " " +
                        currentMediaType.sampleSize + " " +
                        connectedComment);
                }
            }
        }

        internal string TranslateMediaMajorType(Guid majorType)
        {
            if (majorType == MediaType.AnalogAudio)
                return ("Analog Audio");
            if (majorType == MediaType.AnalogVideo)
                return ("Analog Video");
            if (majorType == MediaType.Audio)
                return ("Audio");
            if (majorType == MediaType.AuxLine21Data)
                return ("Aux Line21 Data");
            if (majorType == MediaType.DTVCCData)
                return ("DTV CC Data");
            if (majorType == MediaType.File)
                return ("File");
            if (majorType == MediaType.Interleaved)
                return ("Interleaved");
            if (majorType == MediaType.LMRT)
                return ("LMRT");
            if (majorType == MediaType.Midi)
                return ("Midi");
            if (majorType == MediaType.Mpeg2Sections)
                return ("MPEG2 Sections");
            if (majorType == MediaType.MSTVCaption)
                return ("MSTV Caption");
            if (majorType == MediaType.ScriptCommand)
                return ("Script Command");
            if (majorType == MediaType.Stream)
                return ("Stream");
            if (majorType == MediaType.Timecode)
                return ("Timecode");
            if (majorType == MediaType.URLStream)
                return ("URL Stream");
            if (majorType == MediaType.VBI)
                return ("VBI");
            if (majorType == MediaType.Video)
                return ("Video");

            return (majorType.ToString());
        }

        internal string TranslateMediaSubType(Guid subType)
        {
            if (subType == MediaSubType.A2B10G10R10)
                return ("A2B10G10R10");
            if (subType == MediaSubType.A2R10G10B10)
                return ("A2B10G10B10");
            if (subType == MediaSubType.AI44)
                return ("AI44");
            if (subType == MediaSubType.AIFF)
                return ("AIFF");
            if (subType == MediaSubType.AnalogVideo_NTSC_M)
                return ("Analog Video NTSC M");
            if (subType == MediaSubType.AnalogVideo_PAL_B)
                return ("Analog Video PAL B");
            if (subType == MediaSubType.AnalogVideo_PAL_D)
                return ("Analog Video PAL D");
            if (subType == MediaSubType.AnalogVideo_PAL_G)
                return ("Analog Video PAL G");
            if (subType == MediaSubType.AnalogVideo_PAL_H)
                return ("Analog Video PAL H");
            if (subType == MediaSubType.AnalogVideo_PAL_I)
                return ("Analog Video PAL I");
            if (subType == MediaSubType.AnalogVideo_PAL_M)
                return ("Analog Video PAL M");
            if (subType == MediaSubType.AnalogVideo_PAL_N)
                return ("Analog Video PAL N");
            if (subType == MediaSubType.AnalogVideo_PAL_N_COMBO)
                return ("Analog Video PAL N Combo");
            if (subType == MediaSubType.AnalogVideo_SECAM_B)
                return ("Analog Video Secam B");
            if (subType == MediaSubType.AnalogVideo_SECAM_D)
                return ("Analog Video Secam D");
            if (subType == MediaSubType.AnalogVideo_SECAM_G)
                return ("Analog Video Secam G");
            if (subType == MediaSubType.AnalogVideo_SECAM_H)
                return ("Analog Video Secam H");
            if (subType == MediaSubType.AnalogVideo_SECAM_K)
                return ("Analog Video Secam K");
            if (subType == MediaSubType.AnalogVideo_SECAM_K1)
                return ("Analog Video Secam K1");
            if (subType == MediaSubType.AnalogVideo_SECAM_L)
                return ("Analog Video Secam L");
            if (subType == MediaSubType.ARGB1555)
                return ("ARGB1555");
            if (subType == MediaSubType.ARGB1555_D3D_DX7_RT)
                return ("ARGB1555 D3D DX7 RT");
            if (subType == MediaSubType.ARGB1555_D3D_DX9_RT)
                return ("ARGB1555 D3D DX9 RT");
            if (subType == MediaSubType.ARGB32)
                return ("ARGB32");
            if (subType == MediaSubType.ARGB32_D3D_DX7_RT)
                return ("ARGB32 D3D DX7 RT");
            if (subType == MediaSubType.ARGB32_D3D_DX9_RT)
                return ("ARGB32 D3D DX9 RT");
            if (subType == MediaSubType.ARGB4444)
                return ("ARGB4444");
            if (subType == MediaSubType.ARGB4444_D3D_DX7_RT)
                return ("ARGB4444 D3D DX7 RT");
            if (subType == MediaSubType.ARGB4444_D3D_DX9_RT)
                return ("ARGB4444 D3D DX9 RT");
            if (subType == MediaSubType.Asf)
                return ("ASF");
            if (subType == MediaSubType.AtscSI)
                return ("ATSC SI");
            if (subType == MediaSubType.AU)
                return ("AU");
            if (subType == MediaSubType.Avi)
                return ("AVI");
            if (subType == MediaSubType.AYUV)
                return ("AYUV");
            if (subType == MediaSubType.CFCC)
                return ("CFCC");
            if (subType == MediaSubType.CLPL)
                return ("ARGB 1555");
            if (subType == MediaSubType.CLJR)
                return ("CLPL");
            if (subType == MediaSubType.CPLA)
                return ("CPLA");
            if (subType == MediaSubType.Data708_608)
                return ("Data 708/608");
            if (subType == MediaSubType.DOLBY_AC3_SPDIF)
                return ("Dolby AC3 S/P DIF");
            if (subType == MediaSubType.DolbyAC3)
                return ("Dolby AC3");
            if (subType == MediaSubType.DRM_Audio)
                return ("DRM Audio");
            if (subType == MediaSubType.DssAudio)
                return ("DSS Audio");
            if (subType == MediaSubType.DssVideo)
                return ("DSS Video");
            if (subType == MediaSubType.DtvCcData)
                return ("DTV CC Data");
            if (subType == MediaSubType.dv25)
                return ("DV25");
            if (subType == MediaSubType.dv50)
                return ("DV50");
            if (subType == MediaSubType.DvbSI)
                return ("DVB SI");
            if (subType == MediaSubType.DVCS)
                return ("DVCS");
            if (subType == MediaSubType.dvh1)
                return ("DV H1");
            if (subType == MediaSubType.dvhd)
                return ("DV HD");
            if (subType == MediaSubType.DVSD)
                return ("DV SD");
            if (subType == MediaSubType.dvsl)
                return ("DV SL");
            if (subType == MediaSubType.H264)
                return ("H264");
            if (subType == MediaSubType.I420)
                return ("I420");
            if (subType == MediaSubType.IA44)
                return ("IA44");
            if (subType == MediaSubType.IEEE_FLOAT)
                return ("IEEE FLOAT");
            if (subType == MediaSubType.IF09)
                return ("IF09");
            if (subType == MediaSubType.IJPG)
                return ("IJPG");
            if (subType == MediaSubType.IMC1)
                return ("IMC1");
            if (subType == MediaSubType.IMC2)
                return ("IMC2");
            if (subType == MediaSubType.IMC3)
                return ("IMC3");
            if (subType == MediaSubType.IMC4)
                return ("IMC4");
            if (subType == MediaSubType.IYUV)
                return ("IYUV");
            if (subType == MediaSubType.Line21_BytePair)
                return ("Line21 Byte Pair");
            if (subType == MediaSubType.Line21_GOPPacket)
                return ("Line21 GOP Packet");
            if (subType == MediaSubType.Line21_VBIRawData)
                return ("Line21 VBI Raw Data");
            if (subType == MediaSubType.MDVF)
                return ("MDVF");
            if (subType == MediaSubType.MJPG)
                return ("MJPG");
            if (subType == MediaSubType.MPEG1Audio)
                return ("MPEG1 Audio");
            if (subType == MediaSubType.MPEG1AudioPayload)
                return ("MPEG1 Audio Payload");
            if (subType == MediaSubType.MPEG1Packet)
                return ("MPEG1 Packet");
            if (subType == MediaSubType.MPEG1Payload)
                return ("MPEG1 Payload");
            if (subType == MediaSubType.MPEG1System)
                return ("MPEG1 System");
            if (subType == MediaSubType.MPEG1SystemStream)
                return ("MPEG1 System Stream");
            if (subType == MediaSubType.MPEG1Video)
                return ("MPEG1 Video");
            if (subType == MediaSubType.MPEG1VideoCD)
                return ("MPEG1 Video CD");
            if (subType == MediaSubType.Mpeg2Audio)
                return ("MPEG2 Audio");
            if (subType == MediaSubType.Mpeg2Data)
                return ("MPEG2 Data");
            if (subType == MediaSubType.Mpeg2Program)
                return ("MPEG2 Program");
            if (subType == MediaSubType.Mpeg2Transport)
                return ("MPEG2 Transport");
            if (subType == MediaSubType.Mpeg2TransportStride)
                return ("MPEG2 Transport Stride");
            if (subType == MediaSubType.Mpeg2Video)
                return ("MPEG2 Video");
            if (subType == MediaSubType.None)
                return ("None");
            if (subType == MediaSubType.Null)
                return ("Null");
            if (subType == MediaSubType.NV12)
                return ("NV12");
            if (subType == MediaSubType.NV24)
                return ("NV24");
            if (subType == MediaSubType.Overlay)
                return ("Overlay");
            if (subType == MediaSubType.PCM)
                return ("PCM");
            if (subType == MediaSubType.PCMAudio_Obsolete)
                return ("PCM Audio Obsolete");
            if (subType == MediaSubType.PLUM)
                return ("PLUM");
            if (subType == MediaSubType.QTJpeg)
                return ("QT JPEG");
            if (subType == MediaSubType.QTMovie)
                return ("QT Movie");
            if (subType == MediaSubType.QTRle)
                return ("QT RLE");
            if (subType == MediaSubType.QTRpza)
                return ("QT RPZA");
            if (subType == MediaSubType.QTSmc)
                return ("QT SMC");
            if (subType == MediaSubType.RAW_SPORT)
                return ("RAW SPORT");
            if (subType == MediaSubType.RGB1)
                return ("RGB1");
            if (subType == MediaSubType.RGB16_D3D_DX7_RT)
                return ("RGB16 D3D DX7 RT");
            if (subType == MediaSubType.RGB16_D3D_DX9_RT)
                return ("RGB16 D3D DX9 RT");
            if (subType == MediaSubType.RGB24)
                return ("RGB24");
            if (subType == MediaSubType.RGB32)
                return ("RGB32");
            if (subType == MediaSubType.RGB32_D3D_DX7_RT)
                return ("RGB32 D3D DX7 RT");
            if (subType == MediaSubType.RGB32_D3D_DX9_RT)
                return ("RGB32 D3D DX9 RT");
            if (subType == MediaSubType.RGB4)
                return ("RGB4");
            if (subType == MediaSubType.RGB555)
                return ("RGB555");
            if (subType == MediaSubType.RGB565)
                return ("RGB565");
            if (subType == MediaSubType.RGB8)
                return ("RGB8");
            if (subType == MediaSubType.S340)
                return ("S340");
            if (subType == MediaSubType.S342)
                return ("S342");
            if (subType == MediaSubType.SPDIF_TAG_241h)
                return ("S/P DIF TAG 241H");
            if (subType == MediaSubType.TELETEXT)
                return ("Teletext");
            if (subType == MediaSubType.TVMJ)
                return ("TVMJ");
            if (subType == MediaSubType.UYVY)
                return ("UYVY");
            if (subType == MediaSubType.VideoImage)
                return ("Video Image");
            if (subType == MediaSubType.VPS)
                return ("VPS");
            if (subType == MediaSubType.VPVBI)
                return ("VP VBI");
            if (subType == MediaSubType.VPVideo)
                return ("VP Video");
            if (subType == MediaSubType.WAKE)
                return ("WAKE");
            if (subType == MediaSubType.WAVE)
                return ("WAVE");
            if (subType == MediaSubType.WebStream)
                return ("Web Stream");
            if (subType == MediaSubType.WSS)
                return ("WSS");
            if (subType == MediaSubType.Y211)
                return ("Y211");
            if (subType == MediaSubType.Y411)
                return ("Y411");
            if (subType == MediaSubType.Y41P)
                return ("Y41P");
            if (subType == MediaSubType.YUY2)
                return ("YUY2");
            if (subType == MediaSubType.YUYV)
                return ("YUYV");
            if (subType == MediaSubType.YV12)
                return ("YV12");
            if (subType == MediaSubType.YVU9)
                return ("YVU9");
            if (subType == MediaSubType.YVYU)
                return ("YVYU");

            return (subType.ToString());
        }

        /// <summary>
        /// Log a BDA message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        protected void LogMessage(string message)
        {
            if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                Logger.Instance.Write(componentName + " " + message);
        }
    }
}
