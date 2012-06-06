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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an EIT program content entry.
    /// </summary>
    public class EITProgramContent
    {
        /// <summary>
        /// Get the collection of program contents.
        /// </summary>
        public static Collection<EITProgramContent> Contents 
        { 
            get 
            {
                if (contents == null)
                    contents = new Collection<EITProgramContent>();
                return (contents); 
            } 
        }

        /// <summary>
        /// Get the standard name of the configuration file.
        /// </summary>
        public static string FileName { get { return ("EIT Categories"); } }

        /// <summary>
        /// Get or set the content ID.
        /// </summary>
        public int ContentID 
        { 
            get { return (contentID); }
            set { contentID = value; }
        }

        /// <summary>
        /// Get or set the content ID as a string.
        /// </summary>
        public string ContentIDString
        {
            get { return (contentID.ToString()); }
            set 
            {
                if (value == null || value.Trim() == string.Empty)
                    contentID = 0;
                else
                    contentID = Int32.Parse(value); 
            }
        }

        /// <summary>
        /// Get or set the sub-content ID.
        /// </summary>
        public int SubContentID 
        { 
            get { return (subContentID); }
            set { subContentID = value; }
        }

        /// <summary>
        /// Get or set the sub-content ID as a string.
        /// </summary>
        public string SubContentIDString
        {
            get { return (subContentID.ToString()); }
            set 
            {
                if (value == null || value.Trim() == string.Empty)
                    subContentID = 0;
                else
                    subContentID = Int32.Parse(value); 
            }
        }

        /// <summary>
        /// Get or set the  description.
        /// </summary>
        public string Description 
        { 
            get 
            {
                switch (currentMode)
                {
                    case mode.wmc:
                        return (WMCDescription);
                    case mode.dvbLogic:
                        return (DVBLogicDescription);
                    case mode.dvbViewer:
                        return (((contentID * 16) + subContentID).ToString());
                    case mode.maintenance:
                        return (description);
                    case mode.native:
                        return (EITDescription);
                    default:
                        return (null);
                }
            }
            set { description = value; }
        }

        /// <summary>
        /// Get or set the EIT part of the description.
        /// </summary>
        public string EITDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] eitParts = description.Split(new char[] { '=' });
                string eitDescription = eitParts[0].Trim();
                if (eitDescription.Length == 0)
                    return (null);
                else
                    return (eitDescription);
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] eitParts = description.Split(new char[] { '=' });

                string newDescription;

                if (value == null || value.Trim().Length == 0)
                    newDescription = string.Empty;
                else
                    newDescription = value.Trim();

                StringBuilder descriptionParts = new StringBuilder();

                for (int index = 0; index < eitParts.Length; index++)
                {
                    if (index == 0)
                        descriptionParts.Append(newDescription);
                    else
                    {
                        if (descriptionParts.Length != 0)
                            descriptionParts.Append("=");
                        descriptionParts.Append(eitParts[index]);
                    }
                }

                description = descriptionParts.ToString();
            }
        }

        /// <summary>
        /// Get or set the Windows Media Centre part of the description.
        /// </summary>
        public string WMCDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] wmcParts = description.Split(new char[] { '=' });
                if (wmcParts.Length < 2)
                    return (null);
                else
                {
                    string wmcDescription = wmcParts[1].Trim();
                    if (wmcDescription.Length == 0)
                        return (null);
                    else
                        return (wmcDescription);
                }
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] wmcParts = description.Split(new char[] { '=' });

                string newDescription;

                if (value == null || value.Trim().Length == 0)
                    newDescription = string.Empty;
                else
                    newDescription = "=" + value.Trim();

                StringBuilder descriptionParts = new StringBuilder();

                for (int index = 0; index < maxParts; index++)
                {
                    if (index == 1)
                        descriptionParts.Append(newDescription);
                    else
                    {
                        if (descriptionParts.Length != 0)
                            descriptionParts.Append("=");

                        if (index < wmcParts.Length)
                            descriptionParts.Append(wmcParts[index]);
                    }
                }

                description = descriptionParts.ToString().TrimEnd(new char[] { '=' });
            }
        }

        /// <summary>
        /// Get or set the DVBLogic part of the description.
        /// </summary>
        public string DVBLogicDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] dvbLogicParts = description.Split(new char[] { '=' });
                if (dvbLogicParts.Length < 3)
                    return (null);
                else
                {
                    string dvbLogicDescription = dvbLogicParts[2].Trim();
                    if (dvbLogicDescription.Length == 0)
                        return (null);
                    else
                        return (dvbLogicDescription);
                }
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] dvbLogicParts = description.Split(new char[] { '=' });

                string newDescription;

                if (value == null || value.Trim().Length == 0)
                    newDescription = string.Empty;
                else
                    newDescription = "=" + value.Trim().ToLowerInvariant();

                StringBuilder descriptionParts = new StringBuilder();

                for (int index = 0; index < maxParts; index++)
                {
                    if (index == 2)
                        descriptionParts.Append(newDescription);
                    else
                    {
                        if (descriptionParts.Length != 0)
                            descriptionParts.Append("=");

                        if (index < dvbLogicParts.Length)
                            descriptionParts.Append(dvbLogicParts[index]);
                    }
                }

                description = descriptionParts.ToString().TrimEnd(new char[] { '=' });
            }
        }

        /// <summary>
        /// Get the full description.
        /// </summary>
        public string FullDescription { get { return (description); } }

        /// <summary>
        /// Get or set a description of the sample event for the category.
        /// </summary>
        public string SampleEvent
        {
            get { return (sampleEvent); }
            set { sampleEvent = value; }
        }

        /// <summary>
        /// Get or set the usage count for the content.
        /// </summary>
        public int UsedCount
        {
            get { return (usedCount); }
            set { usedCount = value; }
        }

        private int contentID;
        private int subContentID;
        private string description;
        private int usedCount;
        private string sampleEvent;
        
        private static Collection<EITProgramContent> contents;
        private static Collection<EITProgramContent> undefinedContents;

        private enum mode
        {
            native,
            maintenance,
            wmc,
            dvbLogic,
            dvbViewer
        }

        private static mode currentMode;

        private const int maxParts = 3;

        /// <summary>
        /// Initialize a new instance of the EITProgramContent class.
        /// </summary>
        public EITProgramContent() { }

        /// <summary>
        /// Initialize a new instance of the EITProgramContent class.
        /// </summary>
        /// <param name="contentID">The content ID.</param>
        /// <param name="subContentID">The sub-content ID.</param>
        /// <param name="description">The content description.</param>
        public EITProgramContent(int contentID, int subContentID, string description)
        {
            this.contentID = contentID;
            this.subContentID = subContentID;
            this.description = description;
        }

        /// <summary>
        /// Find a program content.
        /// </summary>
        /// <param name="contentID">The content ID.</param>
        /// <param name="subContentID">The sub-content ID.</param>
        /// <returns>A content instance or null if the content is undefined.</returns>
        public static EITProgramContent FindContent(int contentID, int subContentID)
        {
            if (contents == null)
                return (null);

            foreach (EITProgramContent content in contents)
            {
                if (content.ContentID == contentID && content.SubContentID == subContentID)
                    return (content);
            }

            return (null);
        }

        /// <summary>
        /// Load the content definitions.
        /// </summary>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load()
        {            
            if (RunParameters.Instance.Options.Contains("WMCIMPORT"))
                currentMode = mode.wmc;
            else
            {
                if (RunParameters.Instance.Options.Contains("DVBVIEWERIMPORT") || RunParameters.Instance.Options.Contains("DVBVIEWERRECSVCIMPORT"))
                    currentMode = mode.dvbViewer;
                else
                {
                    if (CommandLine.PluginMode && !RunParameters.Instance.OutputFileSet)
                        currentMode = mode.dvbLogic;
                    else
                        currentMode = mode.native;
                }
            }

            string actualFileName = Path.Combine(RunParameters.DataDirectory, FileName + ".cfg");
            if (!File.Exists(actualFileName))
                actualFileName = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", FileName + ".cfg"));
            
            return(Load(actualFileName));            
        }

        /// <summary>
        /// Load the category definitions from a specified file.
        /// </summary>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load(string fileName)
        {
            Logger.Instance.Write("Loading EIT Program Categories from " + fileName);

            FileStream fileStream = null;

            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException e)
            {
                Logger.Instance.Write("Program categories file " + fileName + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            Contents.Clear();
            undefinedContents = null;

            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();

                if (line.Trim() != string.Empty && line[0] != '#')
                {
                    string[] parts = line.Split(new char[] { '=' });
                    if (parts.Length < 2)
                        Logger.Instance.Write("Program category line '" + line + "' format wrong - line ignored ");
                    else
                    {
                        try
                        {
                            string[] numbers = parts[0].Split(new char[] { ',' });
                            if (numbers.Length != 2)
                                Logger.Instance.Write("Program category line '" + line + "' format wrong - line ignored ");
                            else
                            {
                                int contentID = Int32.Parse(numbers[0].Trim());
                                int subContentID = Int32.Parse(numbers[1].Trim());
                                AddContent(contentID, subContentID, line.Substring(parts[0].Length + 1));
                            }
                        }
                        catch (FormatException)
                        {
                            Logger.Instance.Write("Program category line '" + line + "' number wrong - line ignored ");
                        }
                        catch (ArithmeticException)
                        {
                            Logger.Instance.Write("Program category line '" + line + "' number wrong - line ignored ");
                        }
                    }
                }
            }

            streamReader.Close();
            fileStream.Close();

            if (contents != null)
                Logger.Instance.Write("Loaded " + contents.Count + " program category entries");
            else
                Logger.Instance.Write("No program category entries loaded");

            return (true);
        }

        /// <summary>
        /// Add a new content definition to the collection.
        /// </summary>
        /// <param name="contentID">The content ID.</param>
        /// <param name="subContentID">The sub-content ID.</param>
        /// <param name="description">The description of the content.</param>
        public static void AddContent(int contentID, int subContentID, string description)
        {
            foreach (EITProgramContent content in Contents)
            {
                if (content.ContentID == contentID)
                {
                    if (content.SubContentID == subContentID)
                    {
                        Logger.Instance.Write("Duplicate program category '" + contentID + "," + subContentID + "' ignored ");
                        return;
                    }
                    else
                    {
                        if (content.SubContentID == subContentID)
                        {
                            contents.Insert(contents.IndexOf(content), new EITProgramContent(contentID, subContentID, description));
                            return;
                        }
                    }
                }
                else
                {
                    if (content.ContentID > contentID)
                    {
                        contents.Insert(contents.IndexOf(content), new EITProgramContent(contentID, subContentID, description));
                        return;
                    }
                }

            }

            contents.Add(new EITProgramContent(contentID, subContentID, description));
        }

        /// <summary>
        /// Save the categories to a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>Null if the operation succeeded; otherwise an error message.</returns>
        public static string Save(string fileName)
        {
            Logger.Instance.Write("Saving EIT Program Categories to " + fileName);

            try
            {
                if (File.Exists(fileName))
                {
                    if (File.Exists(fileName + ".bak"))
                    {
                        File.SetAttributes(fileName + ".bak", FileAttributes.Normal);
                        File.Delete(fileName + ".bak");
                    }

                    File.Copy(fileName, fileName + ".bak");
                    File.SetAttributes(fileName + ".bak", FileAttributes.ReadOnly);

                    File.SetAttributes(fileName, FileAttributes.Normal);
                }

                FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

                foreach (EITProgramContent content in Contents)
                    streamWriter.WriteLine(content.ContentID + "," + content.SubContentID + "=" + content.FullDescription);

                streamWriter.Close();
                fileStream.Close();

                File.SetAttributes(fileName, FileAttributes.ReadOnly);

                return (null);
            }
            catch (IOException e)
            {
                return (e.Message);
            }
        }


        /// <summary>
        /// Add an undefined content to the collection of undefined contents.
        /// </summary>
        /// <param name="contentID">The content ID.</param>
        /// <param name="subContentID">The sub-content ID.</param>
        /// <param name="description">The content description.</param>
        /// <param name="sampleEvent">The description of a sample event.</param>
        public static void AddUndefinedContent(int contentID, int subContentID, string description, string sampleEvent)
        {
            if (undefinedContents == null)
                undefinedContents = new Collection<EITProgramContent>();

            foreach (EITProgramContent content in undefinedContents)
            {
                if (content.ContentID == contentID)
                {
                    if (content.SubContentID == subContentID)
                    {
                        content.UsedCount++;
                        return;
                    }
                    else
                    {
                        if (content.SubContentID == subContentID)
                        {
                            undefinedContents.Insert(undefinedContents.IndexOf(content), new EITProgramContent(contentID, subContentID, description));
                            undefinedContents[undefinedContents.Count - 1].SampleEvent = sampleEvent;
                            undefinedContents[undefinedContents.Count - 1].UsedCount++;
                            return;
                        }
                    }
                }
                else
                {
                    if (content.ContentID > contentID)
                    {
                        undefinedContents.Insert(undefinedContents.IndexOf(content), new EITProgramContent(contentID, subContentID, description));
                        undefinedContents[undefinedContents.Count - 1].SampleEvent = sampleEvent;
                        undefinedContents[undefinedContents.Count - 1].UsedCount++;
                        return;
                    }
                }
            }

            undefinedContents.Add(new EITProgramContent(contentID, subContentID, description));
            undefinedContents[undefinedContents.Count - 1].SampleEvent = sampleEvent;
            undefinedContents[undefinedContents.Count - 1].UsedCount++;
        }

        /// <summary>
        /// Log the content usage.
        /// </summary>
        public static void LogContentUsage()
        {
            if (contents != null)
            {
                Logger.Instance.WriteSeparator("Program Categories Used");

                foreach (EITProgramContent content in contents)
                {
                    if (content.UsedCount != 0)
                    {
                        if (content.SampleEvent != null)
                            Logger.Instance.Write("Content " + content.ContentID + "," + content.SubContentID +
                                ": " + content.Description +
                                " Used: " + content.UsedCount +
                                " Sample Event: " + content.SampleEvent);
                        else
                            Logger.Instance.Write("Content " + content.ContentID + "," + content.SubContentID +
                                ": " + content.Description +
                                " Used: " + content.UsedCount);
                    }
                }
            }

            if (undefinedContents != null)
            {
                Logger.Instance.WriteSeparator("Program Categories Undefined");

                foreach (EITProgramContent content in undefinedContents)
                {
                    if (content.SampleEvent != null)
                        Logger.Instance.Write("Content " + content.ContentID + "," + content.SubContentID +
                            " Used: " + content.UsedCount +
                            " Sample Event: " + content.SampleEvent);
                    else
                        Logger.Instance.Write("Content " + content.ContentID + "," + content.SubContentID +
                            " Used: " + content.UsedCount);
                }
            }
        }

        /// <summary>
        /// Compare this instance with another for sorting purposes.
        /// </summary>
        /// <param name="content">The other instance.</param>
        /// <param name="keyName">The name of the key to compare on.</param>
        /// <returns>Zero if the instances are equal, Greater than 0 if this instance is greater; less than zero otherwise.</returns>
        public int CompareForSorting(EITProgramContent content, string keyName)
        {
            switch (keyName)
            {
                case "ContentID":
                    if (contentID == content.ContentID)
                    {
                        if (subContentID == content.SubContentID)
                            return (EITDescription.CompareTo(content.EITDescription));
                        else
                            return (subContentID.CompareTo(content.SubContentID));
                    }
                    else
                        return (contentID.CompareTo(content.ContentID));
                case "SubContentID":
                    if (subContentID == content.SubContentID)
                    {
                        if (contentID == content.ContentID)
                            return (EITDescription.CompareTo(content.EITDescription));
                        else
                            return (contentID.CompareTo(content.ContentID));
                    }
                    else
                        return (subContentID.CompareTo(content.SubContentID));
                case "Description":
                    if (EITDescription == content.EITDescription)
                    {
                        if (contentID == content.ContentID)
                            return (subContentID.CompareTo(content.SubContentID));
                        else
                            return (contentID.CompareTo(content.ContentID));
                    }
                    else
                        return (EITDescription.CompareTo(content.EITDescription));
                case "WMCDescription":
                    string thisWMCDescription;
                    string otherWMCDescription;

                    if (WMCDescription != null)
                        thisWMCDescription = WMCDescription;
                    else
                        thisWMCDescription = string.Empty;

                    if (content.WMCDescription != null)
                        otherWMCDescription = content.WMCDescription;
                    else
                        otherWMCDescription = string.Empty;

                    if (thisWMCDescription == otherWMCDescription)
                    {
                        if (contentID == content.ContentID)
                            return (subContentID.CompareTo(content.SubContentID));
                        else
                            return (contentID.CompareTo(content.ContentID));
                    }
                    else
                        return (thisWMCDescription.CompareTo(otherWMCDescription));
                case "DVBLogicDescription":
                    string thisDVBLogicDescription;
                    string otherDVBLogicDescription;

                    if (DVBLogicDescription != null)
                        thisDVBLogicDescription = DVBLogicDescription;
                    else
                        thisDVBLogicDescription = string.Empty;

                    if (content.DVBLogicDescription != null)
                        otherDVBLogicDescription = content.DVBLogicDescription;
                    else
                        otherDVBLogicDescription = string.Empty;

                    if (thisDVBLogicDescription == otherDVBLogicDescription)
                    {
                        if (contentID == content.ContentID)
                            return (subContentID.CompareTo(content.SubContentID));
                        else
                            return (contentID.CompareTo(content.ContentID));
                    }
                    else
                        return (thisDVBLogicDescription.CompareTo(otherDVBLogicDescription));
                default:
                    return (0);
            }
        }        
    }
}
