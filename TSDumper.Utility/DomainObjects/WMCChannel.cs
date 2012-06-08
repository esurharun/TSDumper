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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a WMC merged channel.
    /// </summary>
    public class WMCChannel
    {
        /// <summary>
        /// Get the collection of channels.
        /// </summary>
        public static Collection<WMCChannel> Channels { get { return(channels); } }

        /// <summary>
        /// Get the channel number.
        /// </summary>
        public decimal ChannelNumber { get { return(channelNumber); } }
        /// <summary>
        /// Get the channel call sign.
        /// </summary>
        public string CallSign { get { return (callSign); } }
        /// <summary>
        /// Get the match name.
        /// </summary>
        public string MatchName { get { return (matchName); } }
        /// <summary>
        /// Get the universal ID.
        /// </summary>
        public string Uid { get { return (uid); } }
        /// <summary>
        /// Get the collection of tuning infos.
        /// </summary>
        public Collection<WMCTuningInfo> TuningInfos
        {
            get
            {
                if (tuningInfos == null)
                    tuningInfos = new Collection<WMCTuningInfo>();
                return(tuningInfos);
            }
        }
    
        private decimal channelNumber;
        private string callSign;
        private string matchName;
        private string uid;

        private Collection<WMCTuningInfo> tuningInfos;

        private static Collection<WMCChannel> channels;

        /// <summary>
        /// Initialize a new instance of the WMCChannel class.
        /// </summary>
        public WMCChannel() { }

        /// <summary>
        /// Load the xml data.
        /// </summary>
        /// <param name="reader">The xml reader for the file.</param>
        public void Load(XmlReader reader)
        {
            uid = reader.GetAttribute("uid");
            matchName = reader.GetAttribute("matchName");
            callSign = reader.GetAttribute("callSign");
            channelNumber = Decimal.Parse(reader.GetAttribute("channelNumber"), CultureInfo.InvariantCulture);

            if (channels == null)
                channels = new Collection<WMCChannel>();

            channels.Add(this);  
        }

        /// <summary>
        /// Load the DVB tuning information.
        /// </summary>
        /// <param name="reader"></param>
        public void LoadDVBTuningInfo(XmlReader reader)
        {
            WMCDVBTuningInfo tuningInfo = new WMCDVBTuningInfo(
                Int32.Parse(reader.GetAttribute("frequency")),
                Int32.Parse(reader.GetAttribute("onid")),
                Int32.Parse(reader.GetAttribute("tsid")),
                Int32.Parse(reader.GetAttribute("sid")));

            TuningInfos.Add(tuningInfo);
        }

        /// <summary>
        /// Load the ATSC tuning information.
        /// </summary>
        /// <param name="reader"></param>
        public void LoadATSCTuningInfo(XmlReader reader)
        {
            WMCATSCTuningInfo tuningInfo = new WMCATSCTuningInfo(
                Int32.Parse(reader.GetAttribute("frequency")),
                Int32.Parse(reader.GetAttribute("majorChannel")),
                Int32.Parse(reader.GetAttribute("minorChannel")));

            TuningInfos.Add(tuningInfo);
        }
    }
}
