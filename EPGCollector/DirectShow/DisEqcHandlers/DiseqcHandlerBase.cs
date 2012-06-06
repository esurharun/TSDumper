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
    internal abstract class DiseqcHandlerBase
    {
        /// <summary>
        /// Get the description of the handler.
        /// </summary>
        internal abstract string Description { get; }

        /// <summary>
        /// Return true if the card is Diseqc capable.
        /// </summary>
        internal abstract bool CardCapable { get; }

        protected enum BdaDigitalModulator
        {
            MODULATION_TYPE = 0,
            INNER_FEC_TYPE,
            INNER_FEC_RATE,
            OUTER_FEC_TYPE,
            OUTER_FEC_RATE,
            SYMBOL_RATE,
            SPECTRAL_INVERSION,
            GUARD_INTERVAL,
            TRANSMISSION_MODE
        }
        
        protected enum BdaTunerExtension
        {
            KSPROPERTY_BDA_DISEQC = 0,
            KSPROPERTY_BDA_SCAN_FREQ,
            KSPROPERTY_BDA_CHANNEL_CHANGE,
            KSPROPERTY_BDA_EFFECTIVE_FREQ,
            KSPROPERTY_BDA_PILOT = 0x20,
            KSPROPERTY_BDA_ROLL_OFF = 0x21
        }

        protected enum DisEqcVersion
        {
            DISEQC_VER_1X = 1,
            DISEQC_VER_2
        }

        protected enum BurstModulationType
        {
            TONE_BURST_UNMODULATED = 0,
            TONE_BURST_MODULATED
        }

        protected enum RxMode
        {
            RXMODE_INTERROGATION = 1,
            RXMODE_QUICKREPLY,
            RXMODE_NOREPLY,
            RXMODE_DEFAULT = 0
        }

        internal static SwitchReply ProcessDisEqcSwitch(TuningSpec tuningSpec, Tuner tunerSpec, IBaseFilter tunerFilter)
        {
            DiseqcHandlerBase diseqcHandler = getDiseqcHandler(tunerSpec, tunerFilter);
            if (diseqcHandler == null)
            {
                Logger.Instance.Write("No DiSEqC handler available - switch request ignored");
                return (SwitchReply.NoHandler);
            }

            Logger.Instance.Write("Created " + diseqcHandler.Description + " DiSEqC handler");

            bool reply = diseqcHandler.SendDiseqcCommand(tuningSpec, ((SatelliteFrequency)tuningSpec.Frequency).SatelliteDish.DiseqcSwitch);
            if (reply)
                return (SwitchReply.OK);
            else
                return (SwitchReply.Failed);
        }

        private static DiseqcHandlerBase getDiseqcHandler(Tuner tuner, IBaseFilter tunerFilter)
        {
            if (RunParameters.Instance.DiseqcIdentity != null)
            {
                switch (RunParameters.Instance.DiseqcIdentity)
                {
                    case "HAUPPAUGE":
                        return (createHauppaugeHandler(tunerFilter, true));
                    case "TECHNOTREND":
                        return (createTechnoTrendHandler(tunerFilter, true));
                    case "GENPIX":
                        return (createGenPixHandler(tunerFilter, true));
                    case "CONEXANT":
                        return (createConexantHandler(tuner, tunerFilter, true));
                    case "TWINHAN":
                    case "AZUREWAVE":
                        return (createTwinhanHandler(tunerFilter, true));
                    case "TEVII":
                        return (createTeviiHandler(tuner, tunerFilter, true));
                    case "PROFRED":
                        return (createProfRedHandler(tunerFilter, true));
                    case "DIGITALEVERYWHERE":
                        return (createDigitalEverywhereHandler(tunerFilter, true));
                    case "WIN7API":
                        return (createWin7APIHandler(tunerFilter, true));
                    case "GENERIC":
                        return (createGenericHandler(tunerFilter, true));
                    default:
                        Logger.Instance.Write("DiSEqC handler not recognized");
                        return (null);
                }
            }

            DiseqcHandlerBase diseqcHandler = createHauppaugeHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using Hauppauge method");
                return (diseqcHandler);
            }

            diseqcHandler = createTechnoTrendHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using TechnoTrend method");
                return (diseqcHandler);
            }

            diseqcHandler = createTwinhanHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using Twinhan/TechniSat method");
                return (diseqcHandler);
            }

            diseqcHandler = createConexantHandler(tuner, tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using Conexant method");
                return (diseqcHandler);
            }

            diseqcHandler = createGenPixHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using GenPix method");
                return (diseqcHandler);
            }

            diseqcHandler = createTeviiHandler(tuner, tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using Tevii method");
                return (diseqcHandler);
            }

            diseqcHandler = createProfRedHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using ProfRed/TBS method");
                return (diseqcHandler);
            }

            diseqcHandler = createDigitalEverywhereHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using DigitalEverywhere method");
                return (diseqcHandler);
            }

            diseqcHandler = createWin7APIHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using Win7 API method");
                return (diseqcHandler);
            }

            diseqcHandler = createGenericHandler(tunerFilter, false);
            if (diseqcHandler != null)
            {
                Logger.Instance.Write("DiSEqC processing using Generic method");
                return (diseqcHandler);
            }

            return (null);
        }

        private static DiseqcHandlerBase createHauppaugeHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            HauppaugeDiseqcHandler hauppaugeHandler = new HauppaugeDiseqcHandler(tunerFilter);

            if (hauppaugeHandler.CardCapable)
                return (hauppaugeHandler);

            if (logMessage)
                Logger.Instance.Write("Hauppauge card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createTechnoTrendHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            TechnoTrendDiseqcHandler technotrendHandler = new TechnoTrendDiseqcHandler(tunerFilter);

            if (technotrendHandler.CardCapable)
                return (technotrendHandler);

            if (logMessage)
                Logger.Instance.Write("TechnoTrend card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createGenPixHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            GenPixDiseqcHandler genPixHandler = new GenPixDiseqcHandler(tunerFilter);

            if (genPixHandler.CardCapable)
                return (genPixHandler);

            if (logMessage)
                Logger.Instance.Write("GenPix card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createTwinhanHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            TwinhanDiseqcHandler twinhanHandler = new TwinhanDiseqcHandler(tunerFilter);

            if (twinhanHandler.CardCapable)
                return (twinhanHandler);

            if (logMessage)
                Logger.Instance.Write("Twinhan/Azurewave card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createConexantHandler(Tuner tuner, IBaseFilter tunerFilter, bool logMessage)
        {
            ConexantDiseqcHandler conexantHandler = new ConexantDiseqcHandler(tunerFilter, tuner);

            if (conexantHandler.CardCapable)
                return (conexantHandler);

            if (logMessage)
                Logger.Instance.Write("Conexant card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createTeviiHandler(Tuner tuner, IBaseFilter tunerFilter, bool logMessage)
        {
            TeviiDiseqcHandler teviiHandler = new TeviiDiseqcHandler(tunerFilter, tuner);

            if (teviiHandler.CardCapable)
                return (teviiHandler);

            if (logMessage)
                Logger.Instance.Write("Tevii card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createProfRedHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            ProfRedDiseqcHandler profRedHandler = new ProfRedDiseqcHandler(tunerFilter);

            if (profRedHandler.CardCapable)
                return (profRedHandler);

            if (logMessage)
                Logger.Instance.Write("ProfRed card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createDigitalEverywhereHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            DigitalEverywhereDiseqcHandler digitalEverywhereHandler = new DigitalEverywhereDiseqcHandler(tunerFilter);

            if (digitalEverywhereHandler.CardCapable)
                return (digitalEverywhereHandler);

            if (logMessage)
                Logger.Instance.Write("DigitalEverywhere card is not DiSEqC capable");

            return (null);
        }

        private static DiseqcHandlerBase createWin7APIHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            Win7APIDiseqcHandler win7APIHandler = new Win7APIDiseqcHandler(tunerFilter);

            if (win7APIHandler.CardCapable)
                return (win7APIHandler);

            if (logMessage)
                Logger.Instance.Write("Win 7 DiSEqC API is not available");

            return (null);
        }

        private static DiseqcHandlerBase createGenericHandler(IBaseFilter tunerFilter, bool logMessage)
        {
            GenericDiseqcHandler genericHandler = new GenericDiseqcHandler(tunerFilter);

            if (genericHandler.CardCapable)
                return (genericHandler);

            if (logMessage)
                Logger.Instance.Write("Generic card is not DiSEqC capable");

            return (null);
        }

        protected int GetLnbNumber(string port)
        {
            int lnbNumber = 0;

            switch (port)
            {
                case "A":
                    lnbNumber = 1;
                    break;
                case "B":
                    lnbNumber = 2;
                    break;
                case "AA":
                    lnbNumber = 1;
                    break;
                case "AB":
                    lnbNumber = 2;
                    break;
                case "BA":
                    lnbNumber = 3;
                    break;
                case "BB":
                    lnbNumber = 4;
                    break;
                case "PORT1":
                    lnbNumber = 5;
                    break;
                case "PORT2":
                    lnbNumber = 6;
                    break;
                case "PORT3":
                    lnbNumber = 7;
                    break;
                case "PORT4":
                    lnbNumber = 8;
                    break;
                case "PORT5":
                    lnbNumber = 9;
                    break;
                case "PORT6":
                    lnbNumber = 10;
                    break;
                case "PORT7":
                    lnbNumber = 11;
                    break;
                case "PORT8":
                    lnbNumber = 12;
                    break;
                case "PORT9":
                    lnbNumber = 13;
                    break;
                case "PORT10":
                    lnbNumber = 14;
                    break;
                case "PORT11":
                    lnbNumber = 15;
                    break;
                case "PORT12":
                    lnbNumber = 16;
                    break;
                case "PORT13":
                    lnbNumber = 17;
                    break;
                case "PORT14":
                    lnbNumber = 18;
                    break;
                case "PORT15":
                    lnbNumber = 19;
                    break;
                case "PORT16":
                    lnbNumber = 20;
                    break;
                case "AAPORT1":
                    lnbNumber = 21;
                    break;
                case "ABPORT1":
                    lnbNumber = 22;
                    break;
                case "BAPORT1":
                    lnbNumber = 23;
                    break;
                case "BBPORT1":
                    lnbNumber = 24;
                    break;
                case "AAPORT2":
                    lnbNumber = 25;
                    break;
                case "ABPORT2":
                    lnbNumber = 26;
                    break;
                case "BAPORT2":
                    lnbNumber = 27;
                    break;
                case "BBPORT2":
                    lnbNumber = 28;
                    break;
                case "AAPORT3":
                    lnbNumber = 29;
                    break;
                case "ABPORT3":
                    lnbNumber = 30;
                    break;
                case "BAPORT3":
                    lnbNumber = 31;
                    break;
                case "BBPORT3":
                    lnbNumber = 32;
                    break;
                case "AAPORT4":
                    lnbNumber = 33;
                    break;
                case "ABPORT4":
                    lnbNumber = 34;
                    break;
                case "BAPORT4":
                    lnbNumber = 35;
                    break;
                case "BBPORT4":
                    lnbNumber = 36;
                    break;
                default:
                    lnbNumber = -1;
                    break;
            }

            return (lnbNumber);
        }

        protected string ConvertToHex(byte[] byteData)
        {
            char[] outputChars = new char[byteData.Length * 2];
            int outputIndex = 0;

            for (int inputIndex = 0; inputIndex < byteData.Length; inputIndex++)
            {
                int hexByteLeft = byteData[inputIndex] >> 4;
                int hexByteRight = byteData[inputIndex] & 0x0f;

                outputChars[outputIndex] = getHex(hexByteLeft);
                outputChars[outputIndex + 1] = getHex(hexByteRight);

                outputIndex += 2;
            }

            return ("0x" + new string(outputChars));
        }

        private static char getHex(int value)
        {
            if (value < 10)
                return ((char)('0' + value));

            return ((char)('a' + (value - 10)));
        }

        protected byte[] GetCommand(string hexCommand)
        {
            string[] hexPairs = hexCommand.Split(new char[] { ' ' });

            byte[] hexBytes = new byte[hexPairs.Length];

            try
            {
                for (int index = 0; index < hexPairs.Length; index++)
                    hexBytes[index] = byte.Parse(hexPairs[index].Trim(), System.Globalization.NumberStyles.HexNumber);
            }
            catch (FormatException)
            {
                return(hexBytes);
            }
            catch (ArithmeticException)
            {
                return (hexBytes);
            }

            return (hexBytes);
        }

        protected byte[] GetCommand(int lnbNumber, TuningSpec tuningSpec)
        {
            byte[] commandBytes = new byte[4] { 0xe0, 0x10, 0x00, 0x00 };

            if (lnbNumber < 5)
            {
                commandBytes[2] = 0x38;
                commandBytes[3] = 0xf0;

                //bit 0	(1)	: 0=low band, 1 = hi band
                //bit 1 (2) : 0=vertical, 1 = horizontal
                //bit 3 (4) : 0=satellite position A, 1=satellite position B
                //bit 4 (8) : 0=switch option A, 1=switch option  B
                // LNB    option  position
                // 1        A         A
                // 2        A         B
                // 3        B         A
                // 4        B         B

                bool hiBand = (tuningSpec.Frequency.Frequency > ((SatelliteFrequency)tuningSpec.Frequency).SatelliteDish.LNBSwitchFrequency);
                bool isHorizontal = ((tuningSpec.NativeSignalPolarization == Polarisation.LinearH) || (tuningSpec.NativeSignalPolarization == Polarisation.CircularL));

                commandBytes[3] |= (byte)(hiBand ? 1 : 0);
                commandBytes[3] |= (byte)((isHorizontal) ? 2 : 0);
                commandBytes[3] |= (byte)((lnbNumber - 1) << 2);
            }
            else
            {
                if (lnbNumber < 21)
                {
                    commandBytes[2] = 0x39;
                    commandBytes[3] = (byte)(lnbNumber - 5);
                }
                else
                {
                    commandBytes[2] = 0x38;
                    commandBytes[3] = 0xf0;

                    bool hiBand = (tuningSpec.Frequency.Frequency > ((SatelliteFrequency)tuningSpec.Frequency).SatelliteDish.LNBSwitchFrequency);
                    bool isHorizontal = ((tuningSpec.NativeSignalPolarization == Polarisation.LinearH) || (tuningSpec.NativeSignalPolarization == Polarisation.CircularL));

                    commandBytes[3] |= (byte)(hiBand ? 1 : 0);
                    commandBytes[3] |= (byte)((isHorizontal) ? 2 : 0);

                    if (lnbNumber >= 21 && lnbNumber <= 24)
                        commandBytes[3] |= (byte)((lnbNumber - 21) << 2);
                    else
                    {
                        if (lnbNumber >= 25 && lnbNumber <= 28)
                            commandBytes[3] |= (byte)((lnbNumber - 25) << 2);
                        else
                        {
                            if (lnbNumber >= 29 && lnbNumber <= 32)
                                commandBytes[3] |= (byte)((lnbNumber - 29) << 2);
                            else
                                commandBytes[3] |= (byte)((lnbNumber - 33) << 2);
                        }
                    }
                }
            }

            return (commandBytes);
        }

        protected byte[] GetSecondCommand(int lnbNumber, TuningSpec tuningSpec)
        {
            byte[] commandBytes = new byte[4] { 0xe0, 0x10, 0x39, 0x00 };

            if (lnbNumber < 21 || lnbNumber > 36)
                return(null);
            else
            {
                if (lnbNumber >= 21 && lnbNumber <= 24)
                    commandBytes[3] = 0;
                else
                {
                    if (lnbNumber >= 25 && lnbNumber <= 28)
                        commandBytes[3] = 1;
                    else
                    {
                        if (lnbNumber >= 29 && lnbNumber <= 32)
                            commandBytes[3] = 2;
                        else
                            commandBytes[3] = 3;
                    }
                }
            }

            return (commandBytes);
        }

        internal abstract bool SendDiseqcCommand(TuningSpec tuningSpec, string port);        
    }
}
