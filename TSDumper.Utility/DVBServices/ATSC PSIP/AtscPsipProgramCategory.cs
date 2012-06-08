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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an ATSC PSIP program category.
    /// </summary>
    public class AtscPsipProgramCategory
    {
        /// <summary>
        /// Get the collection of program categories.
        /// </summary>
        public static Collection<AtscPsipProgramCategory> Categories
        {
            get
            {
                if (categories == null)
                    categories = new Collection<AtscPsipProgramCategory>();
                return (categories);
            }
        }

        /// <summary>
        /// Get the standard name of the configuration file.
        /// </summary>
        public static string FileName { get { return ("Atsc Psip Categories"); } }

        /// <summary>
        /// Get or set the category ID.
        /// </summary>
        public int CategoryID
        {
            get { return (number); }
            set { number = value; }
        }

        /// <summary>
        /// Get or set the category ID as a string.
        /// </summary>
        public string CategoryIDString
        {
            get { return (number.ToString()); }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    number = 0;
                else
                    number = Int32.Parse(value);
            }
        }

        /// <summary>
        /// Get or set the category description.
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
                        return (DVBViewerDescription);
                    case mode.maintenance:
                        return (description);
                    case mode.native:
                        return (PSIPDescription);
                    default:
                        return (null);
                }
            }
            set { description = value; }
        }

        /// <summary>
        /// Get or set the PSIP part of the description.
        /// </summary>
        public string PSIPDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] psipParts = description.Split(new char[] { '=' });
                string psipDescription = psipParts[0].Trim();
                if (psipDescription.Length == 0)
                    return (null);
                else
                    return (psipDescription);
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] psipParts = description.Split(new char[] { '=' });

                string newDescription;

                if (value == null || value.Trim().Length == 0)
                    newDescription = string.Empty;
                else
                    newDescription = value.Trim();

                StringBuilder descriptionParts = new StringBuilder();

                for (int index = 0; index < psipParts.Length; index++)
                {
                    if (index == 0)
                        descriptionParts.Append(newDescription);
                    else
                    {
                        if (descriptionParts.Length != 0)
                            descriptionParts.Append("=");
                        descriptionParts.Append(psipParts[index]);
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
        /// Get or set the usage count for the category.
        /// </summary>
        public int UsedCount
        {
            get { return (usedCount); }
            set { usedCount = value; }
        }

        private int number;
        private string description;
        private int usedCount;
        private string sampleEvent;

        private static Collection<AtscPsipProgramCategory> categories;
        private static Collection<AtscPsipProgramCategory> undefinedCategories;

        private enum mode
        {
            native,
            maintenance,
            wmc,
            dvbLogic,
            dvbViewer
        }

        private static mode currentMode;

        private const int maxParts = 4;

        /// <summary>
        /// Initialize a new instance of the AtscPsipProgramCategory class.
        /// </summary>
        public AtscPsipProgramCategory() { }

        /// <summary>
        /// Initialize a new instance of the AtscPsipProgramCategory class.
        /// </summary>
        /// <param name="number">The category ID.</param>
        /// <param name="description">The category description.</param>
        public AtscPsipProgramCategory(int number, string description)
        {
            this.number = number;
            this.description = description;
        }

        /// <summary>
        /// Find a category.
        /// </summary>
        /// <param name="number">The category ID.</param>
        /// <returns>A category instance or null if the category is undefined.</returns>
        public static AtscPsipProgramCategory FindCategory(int number)
        {
            if (categories == null)
                return (null);

            foreach (AtscPsipProgramCategory category in categories)
            {
                if (category.number == number)
                    return (category);

                if (category.number > number)
                    return (null);
            }

            return (null);
        }

        /// <summary>
        /// Find a category description.
        /// </summary>
        /// <param name="number">The category ID.</param>
        /// <returns>The category description or null if the category is undefined.</returns>
        public static string FindCategoryDescription(int number)
        {
            if (categories == null)
                return (null);

            foreach (AtscPsipProgramCategory category in categories)
            {
                if (category.number == number)
                    return (category.Description);

                if (category.number > number)
                    return (null);
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
                actualFileName = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", FileName + ".cfg"));

            return (Load(actualFileName));     
        }

        /// <summary>
        /// Load the category definitions.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load(string fileName)
        {
            Logger.Instance.Write("Loading AtscPsip Program Categories from " + fileName);

            FileStream fileStream = null;

            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException e)
            {
                Logger.Instance.Write("Program category file " + fileName + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            Categories.Clear();

            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();

                if (line != string.Empty && line[0] != '#')
                {
                    string[] parts = line.Split(new char[] { '=' });
                    if (parts.Length < 2)
                        Logger.Instance.Write("Program category line '" + line + "' format wrong - line ignored ");
                    else
                    {
                        try
                        {
                            int number = Int32.Parse(parts[0]);
                            AddCategory(number, line.Substring(parts[0].Length + 1));
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

            if (categories != null)
                Logger.Instance.Write("Loaded " + categories.Count + " program categories");
            else
                Logger.Instance.Write("No program categories loaded");

            return (true);
        }

        /// <summary>
        /// Add a category to the collection.
        /// </summary>
        /// <param name="number">The category number.</param>
        /// <param name="description">The category description.</param>
        public static void AddCategory(int number, string description)
        {
            foreach (AtscPsipProgramCategory category in Categories)
            {
                if (category.number == number)
                {
                    Logger.Instance.Write("Duplicate program category " + number + " - line ignored ");
                    return;
                }

                if (category.number > number)
                {
                    categories.Insert(categories.IndexOf(category), new AtscPsipProgramCategory(number, description));
                    return;
                }
            }

            categories.Add(new AtscPsipProgramCategory(number, description));
        }

        /// <summary>
        /// Save the categories to a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>Null if the operation succeeded; otherwise an error message.</returns>
        public static string Save(string fileName)
        {
            Logger.Instance.Write("Saving AtscPsip Program Categories to " + fileName);

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

                foreach (AtscPsipProgramCategory category in Categories)
                    streamWriter.WriteLine(category.CategoryID + "=" + category.FullDescription);

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
        /// Add an undefined category to the collection of undefined categories.
        /// </summary>
        /// <param name="number">The category ID.</param>
        /// <param name="description">The category description.</param>
        public static void AddUndefinedCategory(int number, string description)
        {
            if (undefinedCategories == null)
                undefinedCategories = new Collection<AtscPsipProgramCategory>();

            foreach (AtscPsipProgramCategory category in undefinedCategories)
            {
                if (category.number == number)
                    return;

                if (category.number > number)
                {
                    AtscPsipProgramCategory insertCategory = new AtscPsipProgramCategory(number, description);
                    insertCategory.usedCount = 1;
                    undefinedCategories.Insert(undefinedCategories.IndexOf(category), insertCategory);
                    return;
                }
            }

            AtscPsipProgramCategory addCategory = new AtscPsipProgramCategory(number, description);
            addCategory.usedCount = 1;
            undefinedCategories.Add(addCategory);
        }

        /// <summary>
        /// Log the category usage.
        /// </summary>

        public static void LogCategoryUsage()
        {
            if (categories != null)
            {
                Logger.Instance.WriteSeparator("Program Categories Used");

                foreach (AtscPsipProgramCategory category in categories)
                {
                    if (category.UsedCount != 0)
                    {
                        if (category.SampleEvent != null)
                            Logger.Instance.Write("Category " + category.CategoryID +
                                ": " + category.Description +
                                " Used: " + category.UsedCount +
                                " Sample Event: " + category.SampleEvent);
                        else
                            Logger.Instance.Write("Category " + category.CategoryID +
                                ": " + category.Description +
                                " ** No events **");
                    }
                }
            }

            if (undefinedCategories != null)
            {
                Logger.Instance.WriteSeparator("Program Categories Undefined");

                foreach (AtscPsipProgramCategory category in undefinedCategories)
                    Logger.Instance.Write("Category " + category.CategoryID +
                        ": " +
                        " Count: " + category.UsedCount +
                        " Sample Event: " + category.Description);
            }
        }

        /// <summary>
        /// Compare this instance with another for sorting purposes.
        /// </summary>
        /// <param name="category">The other instance.</param>
        /// <param name="keyName">The name of the key to compare on.</param>
        /// <returns>Zero if the instances are equal, Greater than 0 if this instance is greater; less than zero otherwise.</returns>
        public int CompareForSorting(AtscPsipProgramCategory category, string keyName)
        {
            switch (keyName)
            {
                case "CategoryID":
                    if (number == category.CategoryID)
                        return (PSIPDescription.CompareTo(category.PSIPDescription));
                    else
                        return (number.CompareTo(category.CategoryID));
                case "Description":
                    if (PSIPDescription == category.PSIPDescription)
                        return (number.CompareTo(category.CategoryID));
                    else
                        return (PSIPDescription.CompareTo(category.PSIPDescription));
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
                        return (number.CompareTo(category.CategoryID));
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
                        return (number.CompareTo(category.CategoryID));
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
                        return (number.CompareTo(category.CategoryID));
                    else
                        return (thisViewerDescription.CompareTo(otherViewerDescription)); 
                default:
                    return (0);
            }
        }

        /// <summary>
        /// Get a list of the categories for an event.
        /// </summary>
        /// <param name="eventEntry">The event to be processed.</param>
        /// <returns>A list of comma separated categories or null if none exist.</returns>
        public static string GetCategories(EventInformationTableEntry eventEntry)
        {
            if (eventEntry.Descriptors == null)
                return(null);

            StringBuilder categoryString = new StringBuilder(); 

            foreach (DescriptorBase descriptorBase in eventEntry.Descriptors)
            {
                GenreDescriptor genreDescriptor = descriptorBase as GenreDescriptor;
                if (genreDescriptor != null)
                {
                    if (genreDescriptor.Attributes != null)
                    {
                        foreach (GenreAttribute genreAttribute in genreDescriptor.Attributes)
                        {
                            AtscPsipProgramCategory category = FindCategory(genreAttribute.Attribute);
                            if (category != null)
                            {
                                if (categoryString.Length != 0)
                                    categoryString.Append(",");
                                categoryString.Append(category.Description);

                                if (category.SampleEvent == null)
                                    category.SampleEvent = eventEntry.EventName.ToString();
                                category.UsedCount++;
                            }
                            else
                                AtscPsipProgramCategory.AddUndefinedCategory(genreAttribute.Attribute, eventEntry.EventName.ToString());
                        }
                    }
                }
            }

            if (categoryString.Length != 0)
                return(categoryString.ToString());
            else
                return(null);                
        }
    }
}
