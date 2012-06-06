////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2011 nzsjb                                          //
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

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    internal abstract class DVBS2HandlerBase
    {
        /// <summary>
        /// Get the description of the handler.
        /// </summary>
        internal abstract string Description { get; }

        /// <summary>
        /// Return true if the card is DVB-S2 capable.
        /// </summary>
        internal abstract bool DVBS2Capable { get; }

        protected enum BdaTunerExtension
        {
            KSPROPERTY_BDA_DISEQC = 0,
            KSPROPERTY_BDA_SCAN_FREQ,
            KSPROPERTY_BDA_CHANNEL_CHANGE,
            KSPROPERTY_BDA_EFFECTIVE_FREQ,
            KSPROPERTY_BDA_NBC_PARAMS = 10,
            KSPROPERTY_BDA_PILOT = 0x20,
            KSPROPERTY_BDA_ROLL_OFF = 0x21
        }

        internal static DVBS2HandlerBase GetDVBS2Handler(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest, Tuner tuner)
        {
            DVBS2HandlerBase dvbs2Handler = getDVBS2Handler(tunerFilter, tuner);
            if (dvbs2Handler == null)
            {
                Logger.Instance.Write("No DVB-S2 handler available - parameters not set");
                return (null);
            }

            Logger.Instance.Write("Created " + dvbs2Handler.Description + " DVB-S2 handler");

            return(dvbs2Handler);            
        }

        private static DVBS2HandlerBase getDVBS2Handler(IBaseFilter tunerFilter, Tuner tuner)
        {
            DVBS2HandlerBase dvbs2Handler = createWin7APIHandler(tunerFilter, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using Win7 API method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createHauppaugeHandler(tunerFilter, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using Hauppauge method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createProfRedHandler(tunerFilter, tuner, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using TBS method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createDigitalEverywhereHandler(tunerFilter, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using DigitalEverywhere method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createTechnoTrendHandler(tunerFilter, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using TechnoTrend method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createTwinhanHandler(tunerFilter, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using Twinhan/TechniSat method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createGenPixHandler(tunerFilter, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using GenPix method");
                return (dvbs2Handler);
            }

            dvbs2Handler = createTBSHandler(tunerFilter, tuner, false);
            if (dvbs2Handler != null)
            {
                Logger.Instance.Write("DVB-S2 processing using TBS method");
                return (dvbs2Handler);
            }

            return (null);
        }

        private static DVBS2HandlerBase createHauppaugeHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            HauppaugeDVBS2Handler hauppaugeHandler = new HauppaugeDVBS2Handler(tunerFilter);

            if (hauppaugeHandler.DVBS2Capable)
                return (hauppaugeHandler);

            if (logMessage)
                Logger.Instance.Write("Hauppauge card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createProfRedHandler(IBaseFilter tunerFilter, Tuner tuner, bool logMessage)
        {
            ProfRedDVBS2Handler profRedHandler = new ProfRedDVBS2Handler(tunerFilter, tuner);

            if (profRedHandler.DVBS2Capable)
                return (profRedHandler);

            if (logMessage)
                Logger.Instance.Write("ProfRed card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createDigitalEverywhereHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            DigitalEverywhereDVBS2Handler digitalEverywhereHandler = new DigitalEverywhereDVBS2Handler(tunerFilter);

            if (digitalEverywhereHandler.DVBS2Capable)
                return (digitalEverywhereHandler);

            if (logMessage)
                Logger.Instance.Write("DigitalEverywhere card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createTechnoTrendHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            TechnoTrendDVBS2Handler technoTrendHandler = new TechnoTrendDVBS2Handler(tunerFilter);

            if (technoTrendHandler.DVBS2Capable)
                return (technoTrendHandler);

            if (logMessage)
                Logger.Instance.Write("TechnoTrend card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createTwinhanHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            TwinhanDVBS2Handler twinhanHandler = new TwinhanDVBS2Handler(tunerFilter);

            if (twinhanHandler.DVBS2Capable)
                return (twinhanHandler);

            if (logMessage)
                Logger.Instance.Write("Twinhan/TechniSat card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createGenPixHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            GenPixDVBS2Handler genPixHandler = new GenPixDVBS2Handler(tunerFilter);

            if (genPixHandler.DVBS2Capable)
                return (genPixHandler);

            if (logMessage)
                Logger.Instance.Write("GenPix card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createTBSHandler(IBaseFilter tunerFilter, Tuner tuner, bool logMessage)
        {
            TBSDVBS2Handler tbsHandler = new TBSDVBS2Handler(tunerFilter, tuner);

            if (tbsHandler.DVBS2Capable)
                return (tbsHandler);

            if (logMessage)
                Logger.Instance.Write("TBS card is not DVB-S2 capable");

            return (null);
        }

        private static DVBS2HandlerBase createWin7APIHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            Win7APIDVBS2Handler win7APIHandler = new Win7APIDVBS2Handler(tunerFilter);

            if (win7APIHandler.DVBS2Capable)
                return (win7APIHandler);

            if (logMessage)
                Logger.Instance.Write("Win7 API card is not DVB-S2 capable");

            return (null);
        }

        internal abstract bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest);
        internal virtual bool ClearDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            Logger.Instance.Write("No DVB-S2 processing required");
            return (true);
        }
                
        protected IBDA_DigitalDemodulator FindDemodulator(string identifier, IBaseFilter tunerFilter)
        {
            IBDA_Topology topology = tunerFilter as IBDA_Topology;
            if (topology == null)
                return (null);

            int nodeTypeCount = 0;
            int[] nodeTypes = new int[10];

            int reply = topology.GetNodeTypes(out nodeTypeCount, 10, nodeTypes);
            DsError.ThrowExceptionForHR(reply);

            for (int nodeTypeIndex = 0; nodeTypeIndex < nodeTypeCount; nodeTypeIndex++)
            {
                int interfaceCount;
                Guid[] interfaces = new Guid[32];
                reply = topology.GetNodeInterfaces(nodeTypes[nodeTypeIndex], out interfaceCount, 32, interfaces);
                DsError.ThrowExceptionForHR(reply);
            }

            for (int nodeTypeIndex = 0; nodeTypeIndex < nodeTypeCount; nodeTypeIndex++)
            {
                int interfaceCount;
                Guid[] interfaces = new Guid[32];
                reply = topology.GetNodeInterfaces(nodeTypes[nodeTypeIndex], out interfaceCount, 32, interfaces);
                DsError.ThrowExceptionForHR(reply);

                for (int searchIndex = 0; searchIndex < interfaceCount; searchIndex++)
                {
                    if (interfaces[searchIndex] == typeof(IBDA_DigitalDemodulator).GUID)
                    {
                        object controlNode;
                        reply = topology.GetControlNode(0, 1, nodeTypes[nodeTypeIndex], out controlNode);
                        DsError.ThrowExceptionForHR(reply);

                        IBDA_DigitalDemodulator demodulator = controlNode as IBDA_DigitalDemodulator;
                        if (demodulator == null)
                            Logger.Instance.Write(identifier + " DVB-S2 handler: Can't get demodulator: cast of control node failed");
                        else
                        {
                            Logger.Instance.Write(identifier + " DVB-S2 handler: Demodulator interface located");
                            return (demodulator);
                        }
                    }
                }
            }

            Logger.Instance.Write(identifier + " DVB-S2 handler: Failed to find demodulator interface");

            return (null);
        }
    }
}
