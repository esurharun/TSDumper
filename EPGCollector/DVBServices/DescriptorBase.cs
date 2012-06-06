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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The base descriptor class.
    /// </summary>
    internal class DescriptorBase
    {
        /// <summary>
        /// Get the table ID containing the descriptor data (Dish/Bell descriptors only).
        /// </summary>
        internal int Table { get { return (table); } }
        /// <summary>
        /// Get the tag of the record.
        /// </summary>
        internal int Tag { get { return (tag); } }
        /// <summary>
        /// Get the length of the descriptor data.
        /// </summary>
        internal int Length { get { return (length); } }
        /// <summary>
        /// Get the total length of the descriptor.
        /// </summary>
        internal int TotalLength { get { return (Length + 2); } }
        /// <summary>
        /// Get the record data.
        /// </summary>
        internal byte[] Data { get { return (data); } }

        /// <summary>
        /// Return true if the descriptor is undefined; false otherwise.
        /// </summary>
        internal bool IsUndefined { get { return (isUndefined); } }

        /// <summary>
        /// Return true if the descriptor is empty; false otherwise.
        /// </summary>
        internal bool IsEmpty { get { return (Length == 0); } }

        /// <summary>
        /// Return the minimum descriptor length in bytes.
        /// </summary>
        internal static int MinimumDescriptorLength { get { return (2); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public virtual int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DescriptorBase: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int lastIndex = -1;

        internal const int CompressedModuleDescriptorTag = 0x09;

        internal const int MetaDataPointerDescriptorTag = 0x25;
        internal const int MetaDataDescriptorTag = 0x26;

        internal const int NetworkNameDescriptorTag = 0x40;
        internal const int ServiceListDescriptorTag = 0x41;
        internal const int StuffingDescriptorTag = 0x42;
        internal const int SatelliteDeliverySystemDescriptorTag = 0x43;
        internal const int CableSystemDeliveryDescriptorTag = 0x44;
        internal const int VbiDataDescriptorTag = 0x45;
        internal const int VbiTeletextDescriptorTag = 0x46;
        internal const int BouquetNameDescriptorTag = 0x47;
        internal const int ServiceDescriptorTag = 0x48;
        internal const int CountryAvailabilityDescriptorTag = 0x49;
        internal const int LinkageDescriptorTag = 0x4a;
        internal const int NvodReferenceDescriptorTag = 0x4b;
        internal const int TimeShiftedServiceDescriptorTag = 0x4c;
        internal const int ShortEventDescriptorTag = 0x4d;
        internal const int ExtendedEventDescriptorTag = 0x4e;
        internal const int TimeShiftedEventDescriptorTag = 0x4f;
        internal const int ComponentDescriptorTag = 0x50;
        internal const int MosaicDescriptorTag = 0x51;
        internal const int StreamIdentifierDescriptorTag = 0x52;
        internal const int CaIdentifierDescriptorTag = 0x53;
        internal const int ContentDescriptorTag = 0x54;
        internal const int ParentalRatingDescriptorTag = 0x55;
        internal const int TeletextDescriptorTag = 0x56;
        internal const int TelephoneDescriptorTag = 0x57;
        internal const int LocalTimeOffsetDescriptorTag = 0x58;
        internal const int SubtitlingDescriptorTag = 0x59;
        internal const int TerrestrialDeliverySystemDescriptorTag = 0x5a;
        internal const int MultilingualNetworkNameDescriptorTag = 0x5b;
        internal const int MultilingualBouquetNameDescriptorTag = 0x5c;
        internal const int MultilingualServiceNameDescriptorTag = 0x5d;
        internal const int MultilingualComponentDescriptorTag = 0x5e;
        internal const int InternalDataSpecifierDescriptorTag = 0x5f;
        internal const int ServiceMoveDescriptorTag = 0x60;
        internal const int ShortSmoothingBufferDescriptorTag = 0x61;
        internal const int FrequenctListDescriptorTag = 0x62;
        internal const int PartialTransportStreamDescriptorTag = 0x63;
        internal const int DataBroadcastDescriptorTag = 0x64;
        internal const int ScramblingDescriptorTag = 0x65;
        internal const int DataBroadcastIDDescriptorTag = 0x66;
        internal const int TransportStreamDescriptorTag = 0x67;
        internal const int DsngDescriptorTag = 0x68;
        internal const int PdcDescriptorTag = 0x69;
        internal const int Ac3DescriptorTag = 0x6a;
        internal const int AncillaryDataDescriptorTag = 0x6b;
        internal const int CellListDescriptorTag = 0x6c;
        internal const int CellFrequencyLinkDescriptorTag = 0x6d;
        internal const int AnnouncementSupportDescriptorTag = 0x6e;
        internal const int ApplicationSignallingDescriptorTag = 0x6f;
        internal const int AdaptionFieldDataDescriptorTag = 0x70;
        internal const int ServiceIdentifierDescriptorTag = 0x71;
        internal const int ServiceAvailabilityDescriptorTag = 0x72;
        internal const int DefaultAuthorityDescriptorTag = 0x73;
        internal const int RelatedContentDescriptorTag = 0x74;
        internal const int TvaIDDescriptorTag = 0x75;
        internal const int ContentIdentifierDescriptorTag = 0x76;
        internal const int TimeSliceFECIdentifierDescriptorTag = 0x77;
        internal const int EcmRepetionRateDescriptorTag = 0x78;
        internal const int S2SatelliteDeliverySystemDescriptorTag = 0x79;
        internal const int EnhancedAC3DescriptorTag = 0x7a;
        internal const int DtsDescriptorTag = 0x7b;
        internal const int AacDescriptorTag = 0x7c;
        internal const int XaitLocationDescriptorTag = 0x7d;
        internal const int FtaContentManagementDescriptorTag = 0x7e;
        internal const int ExtensionDescriptorTag = 0x7f;
        internal const int UserDefinedDescriptorTag = 0x80;

        internal const int TurkeyChannelInfoDescriptorTag = 0x81;
        internal const int FreeviewChannelInfoDescriptorTag = 0x83;
        internal const int GenericChannelInfoDescriptorTag = 0x93;
        internal const int OpenTVChannelInfoDescriptorTag = 0xb1;
        internal const int FreeSatChannelInfoDescriptorTag = 0xd3;
        internal const int E2ChannelInfoDescriptorTag = 0xe2;

        // ATSC PSIP descriptors

        internal const int AtscAC3AudioDescriptorTag = 0x81;
        internal const int AtscCaptionServiceDescriptorTag = 0x86;
        internal const int AtscContentAdvisoryDescriptorTag = 0x87;
        internal const int AtscExtendedChannelNameDescriptorTag = 0xa0;
        internal const int AtscServiceLocationDescriptorTag = 0xa1;
        internal const int AtscGenreDescriptorTag = 0xab;

        // Dish Network descriptors 

        internal const int DishNetworkRatingDescriptorTag = 0x89;
        internal const int DishNetworkShortEventDescriptorTag = 0x91;
        internal const int DishNetworkExtendedEventDescriptorTag = 0x92;
        internal const int DishNetworkSupplementaryDescriptorTag = 0x94;
        internal const int DishNetworkVCHIPDescriptorTag = 0x95;
        internal const int DishNetworkSeriesDescriptorTag = 0x96;

        // Bell TV descriptors 

        internal const int BellTVExtendedEventDescriptorTag = 0x4e;
        internal const int BellTVRatingDescriptorTag = 0x89;        
        internal const int BellTVSeriesDescriptorTag = 0x96;

        private int table;
        private int tag;
        private int length;
        private byte[] data;

        private bool isUndefined;

        /// <summary>
        /// Create an instance of the descriptor class.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">The index of the tag byte of the descriptor.</param>
        /// <returns>A descriptor instance.</returns>
        internal static DescriptorBase Instance(byte[] byteData, int index)
        {
            return (Instance(byteData, index, Scope.All));
        }

        /// <summary>
        /// Create an instance of the descriptor class.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">The index of the tag byte of the descriptor.</param>
        /// <param name="scope">The current scope.</param>
        /// <returns>A descriptor instance.</returns>
        internal static DescriptorBase Instance(byte[] byteData, int index, Scope scope)
        {
            DescriptorBase descriptor;

            switch ((int)byteData[index])
            {
                case NetworkNameDescriptorTag:
                    descriptor = new DVBNetworkNameDescriptor();
                    break;
                case SatelliteDeliverySystemDescriptorTag:
                    descriptor = new DVBSatelliteDeliverySystemDescriptor();
                    break;
                case ServiceDescriptorTag:
                    descriptor = new DVBServiceDescriptor();
                    break;
                case ServiceListDescriptorTag:
                    descriptor = new DVBServiceListDescriptor();
                    break;
                case ShortEventDescriptorTag:
                    descriptor = new DVBShortEventDescriptor();
                    break;
                case ExtendedEventDescriptorTag:
                    descriptor = new DVBExtendedEventDescriptor();
                    break;
                case ComponentDescriptorTag:
                    descriptor = new DVBComponentDescriptor();
                    break;
                case ContentDescriptorTag:
                    descriptor = new DVBContentDescriptor();
                    break;
                case ParentalRatingDescriptorTag:
                    descriptor = new DVBParentalRatingDescriptor();
                    break;                
                case BouquetNameDescriptorTag:
                    descriptor = new DVBBouquetNameDescriptor();
                    break;
                case LocalTimeOffsetDescriptorTag:
                    descriptor = new DVBLocalTimeOffsetDescriptor();
                    break;
                case OpenTVChannelInfoDescriptorTag:
                    descriptor = new OpenTVChannelInfoDescriptor();
                    break;
                case GenericChannelInfoDescriptorTag:
                    switch (scope)
                    {
                        case Scope.Bouquet:
                            descriptor = new FreeviewChannelInfoDescriptor();
                            break;
                        case Scope.ServiceDescripton:
                            descriptor = new ServiceChannelDescriptor();
                            break;
                        default:
                            descriptor = new DescriptorBase();
                            break;
                    }
                    break;
                case FreeviewChannelInfoDescriptorTag:
                case TurkeyChannelInfoDescriptorTag:
                case E2ChannelInfoDescriptorTag:
                    if (scope == Scope.Bouquet)
                        descriptor = new FreeviewChannelInfoDescriptor();
                    else
                        descriptor = new DescriptorBase();  
                    break;
                case FreeSatChannelInfoDescriptorTag:
                    descriptor = new FreeSatChannelInfoDescriptor();
                    break;
                case ContentIdentifierDescriptorTag:
                    descriptor = new DVBContentIdentifierDescriptor();
                    break;
                default:
                    descriptor = new DescriptorBase();                    
                    break;
            }

            descriptor.tag = (int)byteData[index];
            index++;

            descriptor.length = (int)byteData[index];
            index++;

            if (descriptor.Length != 0)
                descriptor.Process(byteData, index);

            return (descriptor);
        }

        /// <summary>
        /// Create an instance of the descriptor class for ATSC descriptors.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">The index of the tag byte of the descriptor.</param>
        /// <returns>A descriptor instance.</returns>
        internal static DescriptorBase AtscInstance(byte[] byteData, int index)
        {
            DescriptorBase descriptor;

            switch ((int)byteData[index])
            {
                case AtscAC3AudioDescriptorTag:
                    descriptor = new AC3AudioDescriptor();
                    break;
                case AtscCaptionServiceDescriptorTag:
                    descriptor = new CaptionServiceDescriptor();
                    break;
                case AtscContentAdvisoryDescriptorTag:
                    descriptor = new ContentAdvisoryDescriptor();
                    break;
                case AtscExtendedChannelNameDescriptorTag:
                    descriptor = new ExtendedChannelNameDescriptor();
                    break;
                case AtscServiceLocationDescriptorTag:
                    descriptor = new ServiceLocationDescriptor();
                    break;
                case AtscGenreDescriptorTag:
                    descriptor = new GenreDescriptor();
                    break;
                default:
                    descriptor = new DescriptorBase();
                    break;
            }

            descriptor.tag = (int)byteData[index];
            index++;

            descriptor.length = (int)byteData[index];
            index++;

            descriptor.Process(byteData, index);

            return (descriptor);
        }

        /// <summary>
        /// Create an instance of the descriptor class for Dish Network descriptors.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">The index of the tag byte of the descriptor.</param>
        /// <param name="table">The table ID containing this descriptor.</param>
        /// <returns>A descriptor instance.</returns>
        internal static DescriptorBase DishNetworkInstance(byte[] byteData, int index, int table)
        {
            DescriptorBase descriptor;

            switch ((int)byteData[index])
            {
                case DishNetworkRatingDescriptorTag:
                    descriptor = new DishNetworkRatingDescriptor();
                    break;
                case DishNetworkShortEventDescriptorTag:
                    descriptor = new DishNetworkShortEventDescriptor();
                    break;
                case DishNetworkExtendedEventDescriptorTag:
                    descriptor = new DishNetworkExtendedEventDescriptor();
                    break;
                case DishNetworkSupplementaryDescriptorTag:
                    descriptor = new DishNetworkSupplementaryDescriptor();
                    break;
                case DishNetworkVCHIPDescriptorTag:
                    descriptor = new DishNetworkVCHIPDescriptor();
                    break;
                case DishNetworkSeriesDescriptorTag:
                    descriptor = new DishNetworkSeriesDescriptor();
                    break;
                case ContentDescriptorTag:
                    descriptor = new DVBContentDescriptor();
                    break;
                default:
                    descriptor = new DescriptorBase();
                    break;
            }

            descriptor.table = table;

            descriptor.tag = (int)byteData[index];
            index++;

            descriptor.length = (int)byteData[index];
            index++;

            descriptor.Process(byteData, index);

            return (descriptor);
        }

        /// <summary>
        /// Create an instance of the descriptor class for Bell TV descriptors.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">The index of the tag byte of the descriptor.</param>
        /// <param name="table">The table ID containing this descriptor.</param>
        /// <returns>A descriptor instance.</returns>
        internal static DescriptorBase BellTVInstance(byte[] byteData, int index, int table)
        {
            DescriptorBase descriptor;

            switch ((int)byteData[index])
            {
                case ShortEventDescriptorTag:
                    descriptor = new BellShortEventDescriptor();
                    break;
                case BellTVExtendedEventDescriptorTag:
                    descriptor = new BellTVExtendedEventDescriptor();
                    break;
                case BellTVRatingDescriptorTag:
                    descriptor = new BellTVRatingDescriptor();
                    break;                
                case BellTVSeriesDescriptorTag:
                    descriptor = new BellTVSeriesDescriptor();
                    break;
                case ContentDescriptorTag:
                    descriptor = new DVBContentDescriptor();
                    break;
                default:
                    descriptor = new DescriptorBase();
                    break;
            }

            descriptor.table = table;

            descriptor.tag = (int)byteData[index];
            index++;

            descriptor.length = (int)byteData[index];
            index++;

            descriptor.Process(byteData, index);

            return (descriptor);
        }

        /// <summary>
        /// Initialize a new instance of the DescriptorBase class.
        /// </summary>
        internal DescriptorBase() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal virtual void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            if (Length != 0)
            {
                data = Utils.GetBytes(byteData, index, Length);
                lastIndex += Length;
            }

            isUndefined = true;
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal virtual void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal virtual void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            if (!RunParameters.Instance.TraceIDs.Contains("GENERICDESCRIPTOR"))
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB GENERIC DESCRIPTOR: Tag: " + Utils.ConvertToHex(tag) +
                " Length: " + length);

            if (length != 0)
                Logger.ProtocolLogger.Dump("Generic Descriptor Data", data, data.Length); 
        }
    }
}
