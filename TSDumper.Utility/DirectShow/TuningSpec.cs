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

using DomainObjects;

using DirectShowAPI;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a tuning spec.
    /// </summary>
    public class TuningSpec
    {
        /// <summary>
        /// Get or set the frequency.
        /// </summary>
        public TuningFrequency Frequency
        {
            get { return (frequency); }
            set { frequency = value; }
        }

        /// <summary>
        /// Get or set the satellite.
        /// </summary>
        public Satellite Satellite
        {
            get { return (satellite); }
            set { satellite = value; }
        }

        /// <summary>
        /// Get or set the symbol rate.
        /// </summary>
        public int SymbolRate
        {
            get { return (symbolRate); }
            set { symbolRate = value; }
        }

        /// <summary>
        /// Get or set the FEC rate.
        /// </summary>
        public FECRate FECRate
        {
            get { return (fec); }
            set { fec = value; }
        }

        /// <summary>
        /// Get the DirectShow FEC rate.
        /// </summary>
        public BinaryConvolutionCodeRate NativeFECRate
        {
            get
            {
                if (fec.Rate == FECRate.FECRate12)
                    return (BinaryConvolutionCodeRate.Rate1_2);
                if (fec.Rate == FECRate.FECRate13)
                    return (BinaryConvolutionCodeRate.Rate1_3);
                if (fec.Rate == FECRate.FECRate14)
                    return (BinaryConvolutionCodeRate.Rate1_4);
                if (fec.Rate == FECRate.FECRate23)
                    return (BinaryConvolutionCodeRate.Rate2_3);
                if (fec.Rate == FECRate.FECRate25)
                    return (BinaryConvolutionCodeRate.Rate2_5);
                if (fec.Rate == FECRate.FECRate34)
                    return (BinaryConvolutionCodeRate.Rate3_4);
                if (fec.Rate == FECRate.FECRate35)
                    return (BinaryConvolutionCodeRate.Rate3_5);
                if (fec.Rate == FECRate.FECRate45)
                    return (BinaryConvolutionCodeRate.Rate4_5);
                if (fec.Rate == FECRate.FECRate511)
                    return (BinaryConvolutionCodeRate.Rate5_11);
                if (fec.Rate == FECRate.FECRate56)
                    return (BinaryConvolutionCodeRate.Rate5_6);
                if (fec.Rate == FECRate.FECRate67)
                    return (BinaryConvolutionCodeRate.Rate6_7);
                if (fec.Rate == FECRate.FECRate78)
                    return (BinaryConvolutionCodeRate.Rate7_8);
                if (fec.Rate == FECRate.FECRate89)
                    return (BinaryConvolutionCodeRate.Rate8_9);
                if (fec.Rate == FECRate.FECRate910)
                    return (BinaryConvolutionCodeRate.Rate9_10);
                if (fec.Rate == FECRate.FECRateMax)
                    return (BinaryConvolutionCodeRate.RateMax);
                return (BinaryConvolutionCodeRate.Rate3_4);
            }
        }

        /// <summary>
        /// Get or set the signal polarization.
        /// </summary>
        public SignalPolarization SignalPolarization
        {
            get { return (signalPolarization); }
            set { signalPolarization = value; }
        }

        /// <summary>
        /// Get the DirectShow signal polarization.
        /// </summary>
        public Polarisation NativeSignalPolarization
        {
            get
            {
                if (signalPolarization.Polarization == SignalPolarization.LinearHorizontal)
                    return (Polarisation.LinearH);
                if (signalPolarization.Polarization == SignalPolarization.LinearVertical)
                    return (Polarisation.LinearV);
                if (signalPolarization.Polarization == SignalPolarization.CircularLeft)
                    return (Polarisation.CircularL);
                if (signalPolarization.Polarization == SignalPolarization.CircularRight)
                    return (Polarisation.CircularR);
                return (Polarisation.LinearH);
            }
        }

        /// <summary>
        /// Get or set the bandwidth.
        /// </summary>
        public int Bandwidth
        {
            get { return (bandwidth); }
            set { bandwidth = value; }
        }

        /// <summary>
        /// Get or set the physical channel number.
        /// </summary>
        public int ChannelNumber
        {
            get { return (channelNumber); }
            set { channelNumber = value; }
        }

        /// <summary>
        /// Get or set the Modulation.
        /// </summary>
        public Modulation Modulation
        {
            get { return (modulation); }
            set { modulation = value; }
        }

        /// <summary>
        /// Get the DirectShow modulation.
        /// </summary>
        public ModulationType NativeModulation
        {
            get
            {
                switch (modulation)
                {
                    case Modulation.AMRadio:
                        return (ModulationType.ModAnalogAmplitude);
                    case Modulation.BPSK:
                        return (ModulationType.ModBpsk);
                    case Modulation.FMRadio:
                        return (ModulationType.ModAnalogFrequency);
                    case Modulation.OQPSK:
                        return (ModulationType.ModOqpsk);
                    case Modulation.PSK8:
                        return (ModulationType.Mod8Psk);
                    case Modulation.QAM1024:
                        return (ModulationType.Mod1024Qam);
                    case Modulation.QAM112:
                        return (ModulationType.Mod112Qam);
                    case Modulation.QAM128:
                        return (ModulationType.Mod128Qam);
                    case Modulation.QAM16:
                        return (ModulationType.Mod16Qam);
                    case Modulation.QAM160:
                        return (ModulationType.Mod160Qam);
                    case Modulation.QAM192:
                        return (ModulationType.Mod192Qam);
                    case Modulation.QAM224:
                        return (ModulationType.Mod224Qam);
                    case Modulation.QAM256:
                        return (ModulationType.Mod256Qam);
                    case Modulation.QAM32:
                        return (ModulationType.Mod32Qam);
                    case Modulation.QAM320:
                        return (ModulationType.Mod320Qam);
                    case Modulation.QAM384:
                        return (ModulationType.Mod384Qam);
                    case Modulation.QAM448:
                        return (ModulationType.Mod448Qam);
                    case Modulation.QAM512:
                        return (ModulationType.Mod512Qam);
                    case Modulation.QAM64:
                        return (ModulationType.Mod64Qam);
                    case Modulation.QAM768:
                        return (ModulationType.Mod768Qam);
                    case Modulation.QAM80:
                        return (ModulationType.Mod80Qam);
                    case Modulation.QAM896:
                        return (ModulationType.Mod896Qam);
                    case Modulation.QAM96:
                        return (ModulationType.Mod96Qam);
                    case Modulation.QPSK:
                        return (ModulationType.ModQpsk);
                    case Modulation.RF:
                        return (ModulationType.ModRF);
                    case Modulation.VSB16:
                        return (ModulationType.Mod16Vsb);
                    case Modulation.VSB8:
                        return (ModulationType.Mod8Vsb);
                    default:
                        return (ModulationType.ModQpsk);
                }
            }
        }

        private Satellite satellite;
        private TuningFrequency frequency;
        private int symbolRate;
        private FECRate fec = new FECRate("3/4");
        private SignalPolarization signalPolarization = new SignalPolarization('H');
        private Modulation modulation = Modulation.QPSK;

        private int bandwidth;
        private int channelNumber;

        /// <summary>
        /// Initialize a new instance of the TuningSpec class.
        /// </summary>
        public TuningSpec() { }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for a DVB errestrial frequency.
        /// </summary>
        /// <param name="frequency">The terrestrial frequency to tune to.</param>
        public TuningSpec(TerrestrialFrequency frequency)
        {
             this.frequency = frequency;
             bandwidth = frequency.Bandwidth;   
        }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for a DVB satellite frequency.
        /// </summary>
        /// <param name="satellite">The satellite to tune to.</param>
        /// <param name="frequency">The frequency to tune to.</param>
        public TuningSpec(Satellite satellite, SatelliteFrequency frequency)
        {
            this.frequency = frequency;
            this.satellite = satellite;
            symbolRate = frequency.SymbolRate;
            fec = frequency.FEC;
            signalPolarization = frequency.Polarization;
            modulation = frequency.Modulation;
        }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for a DVB cable frequency.
        /// </summary>
        /// <param name="frequency">The frequency to tune to.</param>
        public TuningSpec(CableFrequency frequency)
        {
            this.frequency = frequency;
            symbolRate = frequency.SymbolRate;
            fec = frequency.FEC;
            modulation = frequency.Modulation;
        }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for an ATSC frequency.
        /// </summary>
        /// <param name="frequency">The frequency to tune to.</param>
        public TuningSpec(AtscFrequency frequency)
        {
            this.frequency = frequency;
            symbolRate = frequency.SymbolRate;
            fec = frequency.FEC;
            modulation = frequency.Modulation;
            channelNumber = frequency.ChannelNumber;
        }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for a Clear QAM frequency.
        /// </summary>
        /// <param name="frequency">The frequency to tune to.</param>
        public TuningSpec(ClearQamFrequency frequency)
        {
            this.frequency = frequency;
            symbolRate = frequency.SymbolRate;
            fec = frequency.FEC;
            modulation = frequency.Modulation;
            channelNumber = frequency.ChannelNumber;
        }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for a ISDB satellite frequency.
        /// </summary>
        /// <param name="satellite">The satellite to tune to.</param>
        /// <param name="frequency">The frequency to tune to.</param>
        public TuningSpec(Satellite satellite, ISDBSatelliteFrequency frequency)
        {
            this.frequency = frequency;
            this.satellite = satellite;
            symbolRate = frequency.SymbolRate;
            fec = frequency.FEC;
            signalPolarization = frequency.Polarization;
        }

        /// <summary>
        /// Initialize a new instance of the TuningSpec class for a ISDB errestrial frequency.
        /// </summary>
        /// <param name="frequency">The terrestrial frequency to tune to.</param>
        public TuningSpec(ISDBTerrestrialFrequency frequency)
        {
            this.frequency = frequency;
            bandwidth = frequency.Bandwidth;
        }
    }
}
