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
    /// DVB Component descriptor class.
    /// </summary>
    internal class DVBComponentDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the video component type.
        /// </summary>
        public int ComponentTypeVideo { get { return (componentTypeVideo); } }
        
        /// <summary>
        /// Get the audio component type.
        /// </summary>
        public int ComponentTypeAudio { get { return (componentTypeAudio); } }
        
        /// <summary>
        /// Get the subtitles component type.
        /// </summary>
        public int ComponentTypeSubtitles { get { return (componentTypeSubtitles); } }

        /// <summary>
        /// Get the index of the next byte in the EIT section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ComponentDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int streamContent = -1;
        private int componentTypeVideo = -1;
        private int componentTypeAudio = -1;
        private int componentTypeSubtitles = -1;
        private int componentTag;
        private byte[] languageCode;
        private string text;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBComponentDescriptor class.
        /// </summary>
        internal DVBComponentDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                streamContent = (int)byteData[lastIndex] & 0x0f;
                lastIndex++;

                switch (streamContent)
                {
                    case 1:
                    case 5:
                        componentTypeVideo = (int)byteData[lastIndex];
                        break;
                    case 2:
                        componentTypeAudio = (int)byteData[lastIndex];
                        break;
                    case 3:
                        componentTypeSubtitles = (int)byteData[lastIndex];
                        break;
                    default:
                        break;
                }

                lastIndex++;

                componentTag = (int)byteData[lastIndex];
                lastIndex++;

                languageCode = Utils.GetBytes(byteData, lastIndex, 3);
                lastIndex += 3;

                int textLength = Length - (lastIndex - index);
                if (textLength != 0)
                {
                    text = Utils.GetString(byteData, lastIndex, textLength);
                    lastIndex += textLength;
                }

                Validate();
            }
            catch (OverflowException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Component Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            string textString;
            if (text != null)
                textString = text;
            else
                textString = "?";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB COMPONENT DESCRIPTOR: Stream content: " + streamContent +
                " Comp type video: " + componentTypeVideo +
                " Comp type audio: " + componentTypeAudio +
                " Comp type subtitles: " + componentTypeSubtitles +
                " Comp tag: " + componentTag +
                " Lang code: " + Utils.ConvertToHex(languageCode) +
                " Text: " + textString);
        }

        internal static string Decode(int streamContent, int componentTypeVideo, int componentTypeAudio)
        {
            switch (streamContent)
            {
                case 1:
                    switch (componentTypeVideo)
                    {
                        case 1:
                            return ("MPEG2 video, 4:3, 25Hz");
                        case 2:
                        case 3:
                            return ("MPEG2 video, 16:9, 25Hz");
                        case 4:
                            return ("MPEG2 video, >16:9, 25Hz");
                        case 5:
                            return ("MPEG2 video, 4:3, 30Hz");
                        case 6:
                        case 7:
                            return ("MPEG2 video, 16:9, 30Hz");
                        case 8:
                            return ("MPEG2 video, >16:9, 30Hz");
                        case 9:
                            return ("MPEG2 video, 4:3, 25Hz");
                        case 0x0a:
                        case 0x0b:
                            return ("MPEG2 HD video, 16:9, 25Hz");
                        case 0x0c:
                            return ("MPEG2 HD video, >16:9, 25Hz");
                        case 0x0d:
                            return ("MPEG2 HD video, 4:3, 30Hz");
                        case 0x0e:
                        case 0x0f:
                            return ("MPEG2 HD video, 16:9, 30Hz");
                        case 0x10:
                            return ("MPEG2 HD video, >16:9, 30Hz");
                        default:
                            if (componentTypeVideo > 0xaf && componentTypeVideo < 0xff)
                                return ("User defined");
                            else
                                return ("Reserved for future use");
                    }
                case 2:
                    switch (componentTypeAudio)
                    {
                        case 1:
                        case 2:
                            return ("MPEG1 audio, mono");
                        case 3:
                            return ("MPEG1 audio, stereo");
                        case 4:
                            return ("MPEG1 audio, multi-lingual, multi-channel");
                        case 5:
                            return ("MPEG1 audio, surround sound");
                        case 0x40:
                            return ("MPEG1 audio, for visually impaired");
                        case 0x41:
                            return ("MPEG1 audio, for hard of hearing");
                        case 0x42:
                            return ("Supplementary audio");
                        default:
                            if (componentTypeAudio > 0xaf && componentTypeAudio < 0xff)
                                return ("User defined");
                            else
                                return ("Reserved for future use");
                    }
                case 3:
                    switch (componentTypeVideo)
                    {
                        case 1:
                            return ("EBU Teletext subtitles");
                        case 2:
                            return ("Associated EBU Teletext");
                        case 3:
                            return ("VBI data");
                        case 0x10:
                            return ("DVB subtitles");
                        case 0x11:
                            return ("DVB subtitles, 4:3");
                        case 0x12:
                            return ("DVB subtitles, 16:9");
                        case 0x13:
                            return ("DVB subtitles, 2.21:1");
                        case 0x14:
                            return ("DVB subtitles, HD");
                        case 0x20:
                            return ("DVB subtitles for hard of hearing");
                        case 0x21:
                            return ("DVB subtitles for hard of hearing, 4:3");
                        case 0x22:
                            return ("DVB subtitles for hard of hearing, 16:9");
                        case 0x23:
                            return ("DVB subtitles for hard of hearing, 2.21:1");
                        case 0x24:
                            return ("DVB subtitles for hard of hearing, HD");
                        case 0x30:
                            return ("Open sign language interpretation");
                        case 0x31:
                            return ("Closed sign language interpretation");
                        default:
                            if (componentTypeVideo > 0xAF && componentTypeVideo < 0xFF)
                                return ("User defined");
                            else
                                return ("Reserved for future use");
                    }
                case 4:
                    if (componentTypeAudio < 0x080)
                        return ("AC3 audio");
                    else
                        return ("Enhanced AC3 audio");
                case 5:
                    switch (componentTypeVideo)
                    {
                        case 1:
                            return ("H264/AVC SD, 4:3, 25Hz");
                        case 3:
                            return ("H264/AVC SD, 16:9, 25Hz");
                        case 4:
                            return ("H264/AVC SD, >16:9, 25Hz");
                        case 5:
                            return ("H264/AVC SD, 4:3, 30Hz");
                        case 7:
                            return ("H264/AVC SD, 16:9, 30Hz");
                        case 8:
                            return ("H264/AVC SD, >16:9, 30Hz");
                        case 0x0B:
                            return ("H264/AVC HD, 16:9, 25Hz");
                        case 0x0C:
                            return ("H264/AVC HD, >16:9, 25Hz");
                        case 0x0F:
                            return ("H264/AVC HD, 16:9, 30Hz");
                        case 0x10:
                            return ("H264/AVC HD, >16:9, 30Hz");
                        default:
                            if (componentTypeVideo > 0xAF && componentTypeVideo < 0xFF)
                                return ("User defined");
                            else
                                return ("Reserved for future use");
                    }
                case 6:
                    switch (componentTypeAudio)
                    {
                        case 1:
                            return ("HE-AAC audio, mono");
                        case 3:
                            return ("HE-AAC audio, stereo");
                        case 5:
                            return ("HE-AAC audio, surround sound");
                        case 0x40:
                            return ("HE-AAC audio for visually impaired");
                        case 0x41:
                            return ("HE-AAC audio for hard of hearing");
                        case 0x42:
                            return ("HE-AAC supplementary audio");
                        case 0x43:
                            return ("HE-AAC V2 audio, stero");
                        case 0x44:
                            return ("HE-AAC V2 audio for visually impaired");
                        case 0x45:
                            return ("HE-AAC V2 audio for hard of hearing");
                        case 0x46:
                            return ("HE-AAC V2 supplementary audio");
                        default:
                            if (componentTypeAudio > 0x0AF && componentTypeAudio < 0x0FF)
                                return ("User defined");
                            else
                                return ("Reserved for future use");
                    }
                case 7:
                    if (componentTypeAudio < 0x80)
                        return ("DTS audio mode");
                    else
                        return ("Reserved for future use");
                default:
                    if (streamContent > 0x0B)
                        return ("User defined");
                    else
                        return ("Reserved for future use");
            }
        }
    }
}
