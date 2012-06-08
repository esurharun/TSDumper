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
using System.IO;
using System.Text;

using DirectShowAPI;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a WMC recording attribute.
    /// </summary>
    public class RecordingAttribute
    {
        /// <summary>
        /// Get the collection of attributes.
        /// </summary>
        public static Collection<RecordingAttribute> Attributes { get { return (attributes); } }

        /// <summary>
        /// Get the attribute name.
        /// </summary>
        public string Name { get { return (name); } }
        /// <summary>
        /// Get the attribute type.
        /// </summary>
        public string AttributeType { get { return (attributeType); } }

        private string name;
        private object attributeValue;
        private string attributeType;

        private static Collection<RecordingAttribute> attributes;

        private RecordingAttribute() { }

        /// <summary>
        /// Initialize a new instance of the RecordingAttribute class.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <param name="attributeValue">The value of the attribute.</param>
        public RecordingAttribute(string name, string attributeType, object attributeValue)
        {
            this.name = name;
            this.attributeType = attributeType;
            this.attributeValue = attributeValue;            
        }

        /// <summary>
        /// Get the attribute value as an Int.
        /// </summary>
        /// <returns>The attribute value as an Int.</returns>
        public int GetValueAsInt()
        {
            return ((int)attributeValue);
        }

        /// <summary>
        /// Get the attribute value as an string.
        /// </summary>
        /// <returns>The attribute value as an string.</returns>
        public string GetValueAsString()
        {
            return ((string)attributeValue);
        }

        /// <summary>
        /// Get the attribute value as a byte array.
        /// </summary>
        /// <returns>The attribute value as a byte array.</returns>
        public byte[] GetValueAsByte()
        {
            return ((byte[])attributeValue);
        }

        /// <summary>
        /// Get the attribute value as a boolean.
        /// </summary>
        /// <returns>The attribute value as an boolean.</returns>
        public bool GetValueAsBool()
        {
            return ((bool)attributeValue);
        }

        /// <summary>
        /// Get the attribute value as a long.
        /// </summary>
        /// <returns>The attribute value as a long.</returns>
        public long GetValueAsLong()
        {
            return ((long)attributeValue);
        }

        /// <summary>
        /// Get the attribute value as a short.
        /// </summary>
        /// <returns>The attribute value as an Int.</returns>
        public short GetValueAsShort()
        {
            return ((short)attributeValue);
        }

        /// <summary>
        /// Get the attribute value as a GUID.
        /// </summary>
        /// <returns>The attribute value as an Int.</returns>
        public Guid GetValueAsGuid()
        {
            return ((Guid)attributeValue);
        }

        /// <summary>
        /// Find an attribute by name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The attribute or null if it cannot be located.</returns>
        public static RecordingAttribute FindAttribute(string name)
        {
            if (attributes == null)
                return (null);

            foreach (RecordingAttribute attribute in attributes)
            {
                if (attribute.Name == name)
                    return (attribute);
            }

            return (null);
        }

        /// <summary>
        /// Load the attribute collection from a recording file (dvr-ms or wtv).
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>An error message or null if the load succeeds.</returns>
        public static string Load(string fileName)
        {
            if (!File.Exists(fileName))
                return ("The recording file ' " + fileName + "' does not exist");

            StreamBufferRecordingAttributes recordingAttributes = new StreamBufferRecordingAttributes();
            
            int reply = ((IFileSourceFilter)recordingAttributes).Load(fileName, null);
            if (reply != 0)
                return ("The attributes cannot be loaded");

            int reserved = 0;
            short attributeCount;
            reply = ((IStreamBufferRecordingAttribute)recordingAttributes).GetAttributeCount(reserved, out attributeCount);

            attributes = new Collection<RecordingAttribute>();

            for (short index = 0; index < attributeCount; index++)
            {
                StreamBufferAttrDataType attributeType;
                StringBuilder name = null;
                byte[] attributeValue = null;
                short nameLength = 0;
                short valueLength = 0;
                
                reply = ((IStreamBufferRecordingAttribute)recordingAttributes).GetAttributeByIndex(index,
                    ref reserved,
                    name,
                    ref nameLength,
                    out attributeType,
                    attributeValue,
                    ref valueLength);

                name = new StringBuilder(nameLength);
                attributeValue = new byte[valueLength];

                reply = ((IStreamBufferRecordingAttribute)recordingAttributes).GetAttributeByIndex(index,
                    ref reserved,
                    name,
                    ref nameLength,
                    out attributeType,
                    attributeValue,
                    ref valueLength);

                if (name != null && name.Length != 0)
                {
                    RecordingAttribute attribute = getAttribute(name, attributeType, attributeValue);
                    if (attribute != null)
                        attributes.Add(attribute);
                }
            }

            return (null);
        }

        private static RecordingAttribute getAttribute(StringBuilder name, StreamBufferAttrDataType attributeType, byte[] attributeValue)
        {
            string attributeName = name.ToString().TrimEnd('\0');
            string type = attributeType.ToString();

            switch (attributeType)
            {
                case StreamBufferAttrDataType.Binary:
                    RecordingAttribute binaryAttribute = new RecordingAttribute(attributeName, type, attributeValue);
                    return (binaryAttribute);
                case StreamBufferAttrDataType.Bool:
                    RecordingAttribute boolAttribute = new RecordingAttribute(attributeName, type, attributeValue[0] == 0);
                    return (boolAttribute);
                case StreamBufferAttrDataType.DWord:
                    int intValue = attributeValue[0] << 24 | attributeValue[1] << 16 | attributeValue[2] << 8 | attributeValue[3];
                    RecordingAttribute intAttribute = new RecordingAttribute(attributeName, type, intValue);
                    return (intAttribute);                    
                case StreamBufferAttrDataType.Guid:
                    RecordingAttribute guidAttribute = new RecordingAttribute(attributeName, type, new Guid(attributeValue));
                    return (guidAttribute);
                case StreamBufferAttrDataType.QWord:
                    long longValue = attributeValue[0] << 56 | attributeValue[1] << 48 | attributeValue[2] << 40
                        | attributeValue[3] << 32 | attributeValue[4] << 24 | attributeValue[5] << 16
                        | attributeValue[6] << 8 | attributeValue[7];
                    RecordingAttribute longAttribute = new RecordingAttribute(attributeName, type, longValue);
                    return (longAttribute);
                case StreamBufferAttrDataType.String:
                    Encoding sourceEncoding = Encoding.GetEncoding("utf-16");
                    if (sourceEncoding == null)
                        return (null);
                    string encodedString = sourceEncoding.GetString(attributeValue,  0, attributeValue.Length);
                    RecordingAttribute stringAttribute = new RecordingAttribute(attributeName, type, encodedString);
                    return (stringAttribute);
                case StreamBufferAttrDataType.Word:
                    int shortValue = attributeValue[0] << 8 | attributeValue[1];
                    RecordingAttribute shortAttribute = new RecordingAttribute(attributeName, type, shortValue);
                    return (shortAttribute);                    
                default:
                    return (null);
            }
        }
    }
}
