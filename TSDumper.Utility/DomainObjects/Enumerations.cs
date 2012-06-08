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

namespace DomainObjects
{
    /// <summary>
    /// TV Station types.
    /// </summary>
    public enum TVStationType
    {
        /// <summary>
        /// The type is DVB.
        /// </summary>
        Dvb,
        /// <summary>
        /// The type is ATSC.
        /// </summary>
        Atsc
    }

    /// <summary>
    /// Tuner node types.
    /// </summary>
    public enum TunerNodeType
    {
        /// <summary>
        /// The tuner node type for satellite.
        /// </summary>
        Satellite,
        /// <summary>
        /// The tuner node type for terrestrial.
        /// </summary>
        Terrestrial,
        /// <summary>
        /// The tuner node type for cable.
        /// </summary>
        Cable,
        /// <summary>
        /// The tuner node type for ATSC.
        /// </summary>
        ATSC,
        /// <summary>
        /// The tuner node type for ISDB satellite.
        /// </summary>
        ISDBS,
        /// <summary>
        /// The tuner node type for ISDB terrestrial.
        /// </summary>
        ISDBT,
        /// <summary>
        /// The tuner node type for undefined types.
        /// </summary>
        Other
    }

    /// <summary>
    /// Tuner types.
    /// </summary>
    public enum TunerType
    {
        /// <summary>
        /// The tuner type for satellite.
        /// </summary>
        Satellite,
        /// <summary>
        /// The tuner type for terrestrial.
        /// </summary>
        Terrestrial,
        /// <summary>
        /// The tuner type for cable.
        /// </summary>
        Cable,
        /// <summary>
        /// The tuner type for ATSC terrestrial.
        /// </summary>
        ATSC,
        /// <summary>
        /// The tuner type for ATSC cable.
        /// </summary>
        ATSCCable,
        /// <summary>
        /// The tuner type for Clear QAM.
        /// </summary>
        ClearQAM,
        /// <summary>
        /// The tuner type for ISDB-S.
        /// </summary>
        ISDBS,
        /// <summary>
        /// The tuner type for ISDB-T.
        /// </summary>
        ISDBT,
        /// <summary>
        /// The tuner type for undefined types.
        /// </summary>        
        Other
    }

    /// <summary>
    /// Cable modulation values.
    /// </summary>
    public enum Modulation
    {
        /// <summary>
        /// The modulation value for QAM16.
        /// </summary>
        QAM16,
        /// <summary>
        /// The modulation value for QAM32.
        /// </summary>
        QAM32,
        /// <summary>
        /// The modulation value for QAM64.
        /// </summary>
        QAM64,
        /// <summary>
        /// The modulation value for QAM80.
        /// </summary>
        QAM80,
        /// <summary>
        /// The modulation value for QAM96.
        /// </summary>
        QAM96,
        /// <summary>
        /// The modulation value for QAM112.
        /// </summary>
        QAM112,
        /// <summary>
        /// The modulation value for QAM128.
        /// </summary>
        QAM128,
        /// <summary>
        /// The modulation value for QAM160.
        /// </summary>
        QAM160,
        /// <summary>
        /// The modulation value for QAM192.
        /// </summary>
        QAM192,
        /// <summary>
        /// The modulation value for QAM224.
        /// </summary>
        QAM224,
        /// <summary>
        /// The modulation value for QAM256.
        /// </summary>
        QAM256,
        /// <summary>
        /// The modulation value for QAM320.
        /// </summary>
        QAM320,
        /// <summary>
        /// The modulation value for QAM384.
        /// </summary>
        QAM384,
        /// <summary>
        /// The modulation value for QAM448.
        /// </summary>
        QAM448,
        /// <summary>
        /// The modulation value for QAM512.
        /// </summary>
        QAM512,
        /// <summary>
        /// The modulation value for QAM640.
        /// </summary>
        QAM640,
        /// <summary>
        /// The modulation value for QAM768.
        /// </summary>
        QAM768,
        /// <summary>
        /// The modulation value for QAM896.
        /// </summary>
        QAM896,
        /// <summary>
        /// The modulation value for QAM1024.
        /// </summary>
        QAM1024,
        /// <summary>
        /// The modulation value for QPSK.
        /// </summary>
        QPSK,
        /// <summary>
        /// The modulation value for BPSK.
        /// </summary>
        BPSK,
        /// <summary>
        /// The modulation value for OQPSK.
        /// </summary>
        OQPSK,
        /// <summary>
        /// The modulation value for VSB8.
        /// </summary>
        VSB8,
        /// <summary>
        /// The modulation value for VSB16.
        /// </summary>
        VSB16,
        /// <summary>
        /// The modulation value for AM radio.
        /// </summary>
        AMRadio,
        /// <summary>
        /// The modulation value for FM radio.
        /// </summary>
        FMRadio,
        /// <summary>
        /// The modulation value for PSK8.
        /// </summary>
        PSK8,
        /// <summary>
        /// The modulation value for RF.
        /// </summary>
        RF
    }

    /// <summary>
    /// EPG sources.
    /// </summary>
    public enum EPGSource
    {
        /// <summary>
        /// The EPG originated from the MHEG5 protocol.
        /// </summary>
        MHEG5,
        /// <summary>
        /// The EPG originated from the DVB EIT protocol.
        /// </summary>
        EIT,
        /// <summary>
        /// The EPG originated from the OpenTV protocol.
        /// </summary>
        OpenTV,
        /// <summary>
        /// The EPG originated from the FreeSat protocol.
        /// </summary>
        FreeSat,
        /// <summary>
        /// The EPG originated from the MediaHighway1 protocol.
        /// </summary>
        MediaHighway1,
        /// <summary>
        /// The EPG originated from the MediaHighway2 protocol.
        /// </summary>
        MediaHighway2,
        /// <summary>
        /// The EPG originated from the ATSC PSIP protocol.
        /// </summary>        
        PSIP,
        /// <summary>
        /// The EPG originated from the Dish Network EEPG protocol.
        /// </summary>
        DishNetwork,
        /// <summary>
        /// The EPG originated from the Bell TV EEPG protocol.
        /// </summary>
        BellTV,
        /// <summary>
        /// The EPG originated from the Siehfern Info protocol.
        /// </summary>
        SiehfernInfo
    }

    /// <summary>
    /// The types of EPG collection.
    /// </summary>
    public enum CollectionType
    {
        /// <summary>
        /// The collection is for the MHEG5 protocol.
        /// </summary>
        MHEG5,
        /// <summary>
        /// The collection is for the DVB EIT protocol.
        /// </summary>
        EIT,
        /// <summary>
        /// The collection is for the OpenTV protocol.
        /// </summary>
        OpenTV,
        /// <summary>
        /// The collection is for the FreeSat protocol.
        /// </summary>
        FreeSat,
        /// <summary>
        /// The collection is for the MediaHighway1 protocol.
        /// </summary>
        MediaHighway1,
        /// <summary>
        /// The collection is for the MediaHighway2 protocol.
        /// </summary>
        MediaHighway2,        
        /// <summary>
        /// The collection is for the ATSC PSIP protocol.
        /// </summary>
        PSIP,
        /// <summary>
        /// The collection is for the Dish Network EEPG protocol.
        /// </summary>
        DishNetwork,
        /// <summary>
        /// The collection is for the Bell TV EEPG protocol.
        /// </summary>
        BellTV,
        /// <summary>
        /// The collection is for the Siehfern Info protocol.
        /// </summary>
        SiehfernInfo
    }

    /// <summary>
    /// The values that a diseqc switch can have.
    /// </summary>
    public enum DiseqcSettings
    {
        /// <summary>
        /// The switch is not used
        /// </summary>
        None,
        /// <summary>
        /// Simple A.
        /// </summary>
        A,
        /// <summary>
        /// Simple B.
        /// </summary>
        B,
        /// <summary>
        /// Use satellite A port A (Disqec 1.0 committed switch)
        /// </summary>
        AA,
        /// <summary>
        /// Use satellite A port B (Disqec 1.0 committed switch)
        /// </summary>
        AB,
        /// <summary>
        /// Use satellite B port A (Disqec 1.0 committed switch)
        /// </summary>
        BA,
        /// <summary>
        /// Use satellite B port B (Disqec 1.0 committed switch)
        /// </summary>
        BB,
        /// <summary>
        /// Use port 1 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT1,
        /// <summary>
        /// Use port 2 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT2,
        /// <summary>
        /// Use port 3 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT3,
        /// <summary>
        /// Use port 4 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT4,
        /// <summary>
        /// Use port 5 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT5,
        /// <summary>
        /// Use port 6 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT6,
        /// <summary>
        /// Use port 7 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT7,
        /// <summary>
        /// Use port 8 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT8,
        /// <summary>
        /// Use port 9 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT9,
        /// <summary>
        /// Use port 10 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT10,
        /// <summary>
        /// Use port 11 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT11,
        /// <summary>
        /// Use port 12 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT12,
        /// <summary>
        /// Use port 13 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT13,
        /// <summary>
        /// Use port 14 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT14,
        /// <summary>
        /// Use port 15 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT15,
        /// <summary>
        /// Use port 16 (Disqec 1.1 uncommitted switch)
        /// </summary>
        PORT16,
        /// <summary>
        /// Use committed port AA uncommitted port 1 (Combination committed/uncommited switch)
        /// </summary>
        AAPORT1,
        /// <summary>
        /// Use committed port AB uncommitted port 1 (Combination committed/uncommited switch)
        /// </summary>
        ABPORT1,
        /// <summary>
        /// Use committed port BA uncommitted port 1 (Combination committed/uncommited switch)
        /// </summary>
        BAPORT1,
        /// <summary>
        /// Use committed port BB uncommitted port 1 (Combination committed/uncommited switch)
        /// </summary>
        BBPORT1,
        /// <summary>
        /// Use committed port AA uncommitted port 2 (Combination committed/uncommited switch)
        /// </summary>
        AAPORT2,
        /// <summary>
        /// Use committed port AB uncommitted port 2 (Combination committed/uncommited switch)
        /// </summary>
        ABPORT2,
        /// <summary>
        /// Use committed port BA uncommitted port 2 (Combination committed/uncommited switch)
        /// </summary>
        BAPORT2,
        /// <summary>
        /// Use committed port BB uncommitted port 2 (Combination committed/uncommited switch)
        /// </summary>
        BBPORT2,
        /// <summary>
        /// Use committed port AA uncommitted port 3 (Combination committed/uncommited switch)
        /// </summary>
        AAPORT3,
        /// <summary>
        /// Use committed port AB uncommitted port 3 (Combination committed/uncommited switch)
        /// </summary>
        ABPORT3,
        /// <summary>
        /// Use committed port BA uncommitted port 3 (Combination committed/uncommited switch)
        /// </summary>
        BAPORT3,
        /// <summary>
        /// Use committed port BB uncommitted port 3 (Combination committed/uncommited switch)
        /// </summary>
        BBPORT3,
        /// <summary>
        /// Use committed port AA uncommitted port 4 (Combination committed/uncommited switch)
        /// </summary>
        AAPORT4,
        /// <summary>
        /// Use committed port AB uncommitted port 4 (Combination committed/uncommited switch)
        /// </summary>
        ABPORT4,
        /// <summary>
        /// Use committed port BA uncommitted port 4 (Combination committed/uncommited switch)
        /// </summary>
        BAPORT4,
        /// <summary>
        /// Use committed port BB uncommitted port 4 (Combination committed/uncommited switch)
        /// </summary>
        BBPORT4

    }

    /// <summary>
    /// The state of a data update control.
    /// </summary>
    public enum DataState
    {
        /// <summary>
        /// There are unresolved errors.
        /// </summary>
        HasErrors,
        /// <summary>
        /// The data does not need saving.
        /// </summary>
        NotChanged,
        /// <summary>
        /// The data needs saving.
        /// </summary>
        Changed
    }

    /// <summary>
    /// The function of the parameters.
    /// </summary>
    public enum ParameterSet
    {
        /// <summary>
        /// The parameters are used by the Collector.
        /// </summary>
        Collector,
        /// <summary>
        /// The parameters are used by the DVBLogic plugin.
        /// </summary>
        Plugin
    }

    /// <summary>
    /// The DVB-S2 pilot values.
    /// </summary>
    public enum Pilot
    {
        /// <summary>
        /// The value is not set.
        /// </summary>
        NotSet,
        /// <summary>
        /// The value is not defined.
        /// </summary>
        NotDefined,
        /// <summary>
        /// The pilot is off.
        /// </summary>
        Off,
        /// <summary>
        /// The pilot is on.
        /// </summary>
        On
    }

    /// <summary>
    /// The DVB-S2 roll-off values.
    /// </summary>
    public enum RollOff
    {
        /// <summary>
        /// The value is not set.
        /// </summary>
        NotSet,
        /// <summary>
        /// The value is not defined.
        /// </summary>
        NotDefined,
        /// <summary>
        /// Roll off factor is 20%
        /// </summary>
        RollOff20,
        /// <summary>
        /// Roll off factor is 25%
        /// </summary>
        RollOff25,
        /// <summary>
        /// Roll off factor is 35%
        /// </summary>
        RollOff35
    }
}
