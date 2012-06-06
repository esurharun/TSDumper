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
    /// The class that describes a users custom program category.
    /// </summary>
    public class CustomProgramCategory
    {
        /// <summary>
        /// Get the collection of custom categories.
        /// </summary>
        public static Collection<CustomProgramCategory> Categories
        {
            get
            {
                if (categories == null)
                    categories = new Collection<CustomProgramCategory>();
                return (categories);
            }
        }

        /// <summary>
        /// Get the standard name of the configuration file.
        /// </summary>
        public static string FileName { get { return ("Custom Categories"); } }

        /// <summary>
        /// Get or set the category tag.
        /// </summary>
        public string CategoryTag
        {
            get { return (categoryTag); }
            set { categoryTag = value; }
        }

        /// <summary>
        /// Get or set the category description.
        /// </summary>
        public string Description
        {
            get
            {
                string[] descriptionParts = description.Split(new char[] { '=' });

                switch (currentMode)
                {
                    case mode.wmc:
                        return (WMCDescription);
                    case mode.dvbLogic:
                        return (DVBLogicDescription);
                    case mode.dvbViewer:
                        return (DVBViewerDescription);
                    case mode.maintenance:
                        return (description);
                    case mode.native:
                        return (CustomDescription);
                    default:
                        return (null);
                }
            }
            set { description = value; }
        }

        /// <summary>
        /// Get or set the custom part of the description.
        /// </summary>
        public string CustomDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] customParts = description.Split(new char[] { '=' });
                string customDescription = customParts[0].Trim();
                if (customDescription.Length == 0)
                    return (null);
                else
                    return (customDescription);
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] customParts = description.Split(new char[] { '=' });

                description = value.Trim();

                for (int index = 1; index < customParts.Length; index++)
                    description = description + "=" + customParts[index];
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

                description = descriptionParts.ToString().TrimEnd(new char[] { '=' } );
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

                description = descriptionParts.ToString().TrimEnd(new char[] { '=' } );
            }
        }

        /// <summary>
        /// Get or set the DVBViewer part of the description.
        /// </summary>
        public string DVBViewerDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] dvbViewerParts = description.Split(new char[] { '=' });
                if (dvbViewerParts.Length < 4)
                    return (null);
                else
                {
                    string dvbViewerDescription = dvbViewerParts[3].Trim();
                    if (dvbViewerDescription.Length == 0)
                        return (null);
                    else
                        return (dvbViewerDescription);
                }
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] dvbViewerParts = description.Split(new char[] { '=' });

                string newDescription;

                if (value == null || value.Trim().Length == 0)
                    newDescription = string.Empty;
                else
                    newDescription = "=" + value.Trim().ToLowerInvariant();

                StringBuilder descriptionParts = new StringBuilder();
                
                for (int index = 0; index < maxParts; index++)
                {
                    if (index == 3)
                        descriptionParts.Append(newDescription);
                    else
                    {
                        if (descriptionParts.Length != 0)
                            descriptionParts.Append("=");

                        if (index < dvbViewerParts.Length)
                            descriptionParts.Append(dvbViewerParts[index]);
                    }
                }

                description = descriptionParts.ToString().TrimEnd(new char[] { '=' } );
            }
        }

        /// <summary>
        /// Get the full description.
        /// </summary>
        public string FullDescription { get { return (description); } }

        private string categoryTag;
        private string description;

        private static Collection<CustomProgramCategory> categories;

        private enum mode
        {
            native,
            maintenance,
            wmc,
            dvbLogic,
            dvbViewer
        }

        private enum matchMode
        {
            all,
            start,
            end,
            anywhere
        }

        private static mode currentMode;

        private const int maxParts = 4;

        /// <summary>
        /// Initialize a new instance of the CustomProgramCategory class.
        /// </summary>
        public CustomProgramCategory() { }

        /// <summary>
        /// Initialize a new instance of the CustomProgramCategory class.
        /// </summary>
        /// <param name="categoryTag">The category tag.</param>
        /// <param name="description">The category description.</param>
        public CustomProgramCategory(string categoryTag, string description)
        {
            this.categoryTag = categoryTag;
            this.description = description;
        }

        /// <summary>
        /// Find a category.
        /// </summary>
        /// <param name="categoryTag">The category tag.</param>
        /// <returns>A category instance or null if the category is undefined.</returns>
        public static CustomProgramCategory FindCategory(string categoryTag)
        {
            if (categories == null)
                return (null);

            foreach (CustomProgramCategory category in categories)
            {
                if (category.categoryTag.ToLowerInvariant() == categoryTag.ToLowerInvariant())
                    return (category);
            }

            return (null);
        }

        /// <summary>
        /// Find a category description.
        /// </summary>
        /// <param name="inputString">The string that may contain the category tag.</param>
        /// <returns>The category description or null if the category is undefined.</returns>
        public static string FindCategoryDescription(string inputString)
        {
            if (inputString == null || inputString.Length == 0 || categories == null)
                return (null);

            string lowerCaseInputString = inputString.ToLowerInvariant();

            matchMode matchMethod;
            string matchString;            

            foreach (CustomProgramCategory category in categories)
            {
                if (category.CategoryTag.StartsWith("<"))
                {
                    if (category.CategoryTag.EndsWith(">"))
                    {
                        matchMethod = matchMode.anywhere;
                        matchString = category.CategoryTag.Substring(1, category.CategoryTag.Length - 2).ToLowerInvariant();
                    }
                    else
                    {
                        matchMethod = matchMode.start;
                        matchString = category.CategoryTag.Substring(1).ToLowerInvariant();
                    }
                }
                else
                {
                    if (category.CategoryTag.EndsWith(">"))
                    {
                        matchMethod = matchMode.end;
                        matchString = category.CategoryTag.Substring(0, category.CategoryTag.Length - 1).ToLowerInvariant();
                    }
                    else
                    {
                        matchMethod = matchMode.all;
                        matchString = category.CategoryTag.ToLowerInvariant();
                    }
                }

                switch (matchMethod)
                {
                    case matchMode.all:
                        if (lowerCaseInputString == matchString)
                            return (category.Description);
                        break;
                    case matchMode.anywhere:
                        if (lowerCaseInputString.Contains(matchString))
                            return (category.Description);
                        break;
                    case matchMode.start:
                        if (lowerCaseInputString.StartsWith(matchString))
                            return (category.Description);
                        break;
                    case matchMode.end:
                        if (lowerCaseInputString.EndsWith(matchString))
                            return (category.Description);
                        break;
                    default:
                        break;                    
                }
            }

            return (null);
        }

        /// <summary>
        /// Load the category definitions.
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
            {
                actualFileName = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", FileName + ".cfg"));
                if (!File.Exists(actualFileName))
                    return (true);
            }

            return (Load(actualFileName));     
        }

        /// <summary>
        /// Load the category definitions.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load(string fileName)
        {
            Logger.Instance.Write("Loading Custom Program Categories from " + fileName);

            FileStream fileStream = null;

            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException e)
            {
                Logger.Instance.Write("Program category file " + fileName + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            Categories.Clear();

            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();

                if (line != string.Empty && line[0] != '#')
                {
                    string[] parts = line.Split(new char[] { '=' });
                    if (parts.Length < 2)
                        Logger.Instance.Write("Program category line '" + line + "' format wrong - line ignored ");
                    else
                        AddCategory(parts[0].Trim(), line.Substring(parts[0].Length + 1));
                }
            }

            streamReader.Close();
            fileStream.Close();

            if (categories != null)
                Logger.Instance.Write("Loaded " + categories.Count + " program categories");
            else
                Logger.Instance.Write("No program categories loaded");

            return (true);
        }

        /// <summary>
        /// Add a category to the collection.
        /// </summary>
        /// <param name="categoryTag">The category number.</param>
        /// <param name="description">The category description.</param>
        public static void AddCategory(string categoryTag, string description)
        {
            foreach (CustomProgramCategory category in Categories)
            {
                if (category.categoryTag == categoryTag)
                {
                    Logger.Instance.Write("Duplicate program category '" + categoryTag + "' - line ignored ");
                    return;
                }
            }

            categories.Add(new CustomProgramCategory(categoryTag.Trim().ToLowerInvariant(), description));
        }

        /// <summary>
        /// Save the categories to a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>Null if the operation succeeded; otherwise an error message.</returns>
        public static string Save(string fileName)
        {
            Logger.Instance.Write("Saving Custom Program Categories to " + fileName);

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
                StreamWriter streamWriter = new StreamWriter(fileStream);

                foreach (CustomProgramCategory category in Categories)
                    streamWriter.WriteLine(category.CategoryTag + "=" + category.FullDescription);

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
        /// Compare this instance with another for sorting purposes.
        /// </summary>
        /// <param name="category">The other instance.</param>
        /// <param name="keyName">The name of the key to compare on.</param>
        /// <returns>Zero if the instances are equal, Greater than 0 if this instance is greater; less than zero otherwise.</returns>
        public int CompareForSorting(CustomProgramCategory category, string keyName)
        {
            switch (keyName)
            {
                case "CategoryTag":
                    if (categoryTag == category.CategoryTag)
                        return (CustomDescription.CompareTo(category.CustomDescription));
                    else
                        return (categoryTag.CompareTo(category.CategoryTag));
                case "Description":
                    if (CustomDescription == category.CustomDescription)
                        return (categoryTag.CompareTo(category.CategoryTag));
                    else
                        return (CustomDescription.CompareTo(category.CustomDescription));
                case "WMCDescription":
                    string thisWMCDescription;
                    string otherWMCDescription;

                    if (WMCDescription != null)
                        thisWMCDescription = WMCDescription;
                    else
                        thisWMCDescription = string.Empty;

                    if (category.WMCDescription != null)
                        otherWMCDescription = category.WMCDescription;
                    else
                        otherWMCDescription = string.Empty;

                    if (thisWMCDescription == otherWMCDescription)
                        return (categoryTag.CompareTo(category.CategoryTag));
                    else
                        return (thisWMCDescription.CompareTo(otherWMCDescription));
                case "DVBLogicDescription":
                    string thisLogicDescription;
                    string otherLogicDescription;

                    if (DVBLogicDescription != null)
                        thisLogicDescription = DVBLogicDescription;
                    else
                        thisLogicDescription = string.Empty;

                    if (category.DVBLogicDescription != null)
                        otherLogicDescription = category.DVBLogicDescription;
                    else
                        otherLogicDescription = string.Empty;

                    if (thisLogicDescription == otherLogicDescription)
                        return (categoryTag.CompareTo(category.CategoryTag));
                    else
                        return (thisLogicDescription.CompareTo(otherLogicDescription));
                case "DVBViewerDescription":
                    string thisViewerDescription;
                    string otherViewerDescription;

                    if (DVBViewerDescription != null)
                        thisViewerDescription = DVBViewerDescription;
                    else
                        thisViewerDescription = string.Empty;

                    if (category.DVBViewerDescription != null)
                        otherViewerDescription = category.DVBViewerDescription;
                    else
                        otherViewerDescription = string.Empty;

                    if (thisViewerDescription == otherViewerDescription)
                        return (categoryTag.CompareTo(category.CategoryTag));
                    else
                        return (thisViewerDescription.CompareTo(otherViewerDescription));
                default:
                    return (0);
            }
        }
    }
}
