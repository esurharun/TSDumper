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
    /// The class that describes an Bell TV program category entry.
    /// </summary>
    public class BellTVProgramCategory
    {
        /// <summary>
        /// Get the collection of program categories.
        /// </summary>
        public static Collection<BellTVProgramCategory> Categories
        {
            get
            {
                if (categories == null)
                    categories = new Collection<BellTVProgramCategory>();
                return (categories);
            }
        }

        /// <summary>
        /// Get the standard name of the configuration file.
        /// </summary>
        public static string FileName { get { return ("Bell TV Categories"); } }

        /// <summary>
        /// Get or set the category ID.
        /// </summary>
        public int CategoryID
        {
            get { return (categoryID); }
            set { categoryID = value; }
        }

        /// <summary>
        /// Get or set the category ID as a string.
        /// </summary>
        public string CategoryIDString
        {
            get { return (categoryID.ToString()); }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    categoryID = 0;
                else
                    categoryID = Int32.Parse(value);
            }
        }

        /// <summary>
        /// Get or set the sub-category ID.
        /// </summary>
        public int SubCategoryID
        {
            get { return (subCategoryID); }
            set { subCategoryID = value; }
        }

        /// <summary>
        /// Get or set the sub-category ID as a string.
        /// </summary>
        public string SubCategoryIDString
        {
            get { return (subCategoryID.ToString()); }
            set
            {
                if (value == null || value.Trim() == string.Empty)
                    subCategoryID = 0;
                else
                    subCategoryID = Int32.Parse(value);
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
                        return (DVBViewerDescription);
                    case mode.maintenance:
                        return (description);
                    case mode.native:
                        return (BellTVDescription);
                    default:
                        return (null);
                }
            }
            set { description = value; }
        }

        /// <summary>
        /// Get or set the Dish Network part of the description.
        /// </summary>
        public string BellTVDescription
        {
            get
            {
                if (description == null)
                    return (null);

                string[] dishNetworkParts = description.Split(new char[] { '=' });
                string dishNetworkDescription = dishNetworkParts[0].Trim();
                if (dishNetworkDescription.Length == 0)
                    return (null);
                else
                    return (dishNetworkDescription);
            }
            set
            {
                if (description == null)
                    description = string.Empty;

                string[] dishNetworkParts = description.Split(new char[] { '=' });

                string newDescription;

                if (value == null || value.Trim().Length == 0)
                    newDescription = string.Empty;
                else
                    newDescription = value.Trim();

                StringBuilder descriptionParts = new StringBuilder();

                for (int index = 0; index < dishNetworkParts.Length; index++)
                {
                    if (index == 0)
                        descriptionParts.Append(newDescription);
                    else
                    {
                        if (descriptionParts.Length != 0)
                            descriptionParts.Append("=");
                        descriptionParts.Append(dishNetworkParts[index]);
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

        private int categoryID;
        private int subCategoryID;
        private string description;
        private int usedCount;
        private string sampleEvent;

        private static Collection<BellTVProgramCategory> categories;
        private static Collection<BellTVProgramCategory> undefinedCategories;

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
        /// Initialize a new instance of the BellTVProgramCategory class.
        /// </summary>
        public BellTVProgramCategory() { }

        /// <summary>
        /// Initialize a new instance of the BellTVProgramCategory class.
        /// </summary>
        /// <param name="categoryID">The category ID.</param>
        /// <param name="subCategoryID">The sub-category ID.</param>
        /// <param name="description">The category description.</param>
        public BellTVProgramCategory(int categoryID, int subCategoryID, string description)
        {
            this.categoryID = categoryID;
            this.subCategoryID = subCategoryID;
            this.description = description;
        }

        /// <summary>
        /// Find a program category.
        /// </summary>
        /// <param name="categoryID">The category ID.</param>
        /// <param name="subCategoryID">The sub-category ID.</param>
        /// <returns>A categoryt instance or null if the category is undefined.</returns>
        public static BellTVProgramCategory FindCategory(int categoryID, int subCategoryID)
        {
            if (categories == null)
                return (null);

            foreach (BellTVProgramCategory category in categories)
            {
                if (category.CategoryID == categoryID && category.SubCategoryID == subCategoryID)
                    return (category);
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
        /// Load the category definitions from a specified file.
        /// </summary>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load(string fileName)
        {
            Logger.Instance.Write("Loading Bell TV Program Categories from " + fileName);

            FileStream fileStream = null;

            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException e)
            {
                Logger.Instance.Write("Program categories file " + fileName + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            Categories.Clear();
            undefinedCategories = null;

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
                                int categoryID = Int32.Parse(numbers[0].Trim());
                                int subCategoryID = Int32.Parse(numbers[1].Trim());
                                AddCategory(categoryID, subCategoryID, line.Substring(parts[0].Length + 1));
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

            if (categories != null)
                Logger.Instance.Write("Loaded " + categories.Count + " program category entries");
            else
                Logger.Instance.Write("No program category entries loaded");

            return (true);
        }

        /// <summary>
        /// Add a new category definition to the collection.
        /// </summary>
        /// <param name="categoryID">The category ID.</param>
        /// <param name="subCategoryID">The sub-category ID.</param>
        /// <param name="description">The description of the category.</param>
        public static void AddCategory(int categoryID, int subCategoryID, string description)
        {
            foreach (BellTVProgramCategory category in Categories)
            {
                if (category.CategoryID == categoryID)
                {
                    if (category.SubCategoryID == subCategoryID)
                    {
                        Logger.Instance.Write("Duplicate program category '" + categoryID + "," + subCategoryID + "' ignored ");
                        return;
                    }
                    else
                    {
                        if (category.SubCategoryID == subCategoryID)
                        {
                            categories.Insert(categories.IndexOf(category), new BellTVProgramCategory(categoryID, subCategoryID, description));
                            return;
                        }
                    }
                }
                else
                {
                    if (category.CategoryID > categoryID)
                    {
                        categories.Insert(categories.IndexOf(category), new BellTVProgramCategory(categoryID, subCategoryID, description));
                        return;
                    }
                }

            }

            categories.Add(new BellTVProgramCategory(categoryID, subCategoryID, description));
        }

        /// <summary>
        /// Save the categories to a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>Null if the operation succeeded; otherwise an error message.</returns>
        public static string Save(string fileName)
        {
            Logger.Instance.Write("Saving Bell TV Program Categories to " + fileName);

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

                foreach (BellTVProgramCategory category in Categories)
                    streamWriter.WriteLine(category.CategoryID + "," + category.SubCategoryID + "=" + category.FullDescription);

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
        /// <param name="categoryID">The category ID.</param>
        /// <param name="subCategoryID">The sub-category ID.</param>
        /// <param name="description">The category description.</param>
        /// <param name="sampleEvent">The description of a sample event.</param>
        public static void AddUndefinedCategory(int categoryID, int subCategoryID, string description, string sampleEvent)
        {
            if (undefinedCategories == null)
                undefinedCategories = new Collection<BellTVProgramCategory>();

            foreach (BellTVProgramCategory category in undefinedCategories)
            {
                if (category.CategoryID == categoryID)
                {
                    if (category.SubCategoryID == subCategoryID)
                    {
                        category.UsedCount++;
                        return;
                    }
                    else
                    {
                        if (category.SubCategoryID == subCategoryID)
                        {
                            undefinedCategories.Insert(undefinedCategories.IndexOf(category), new BellTVProgramCategory(categoryID, subCategoryID, description));
                            undefinedCategories[undefinedCategories.Count - 1].SampleEvent = sampleEvent;
                            undefinedCategories[undefinedCategories.Count - 1].UsedCount++;
                            return;
                        }
                    }
                }
                else
                {
                    if (category.CategoryID > categoryID)
                    {
                        undefinedCategories.Insert(undefinedCategories.IndexOf(category), new BellTVProgramCategory(categoryID, subCategoryID, description));
                        undefinedCategories[undefinedCategories.Count - 1].SampleEvent = sampleEvent;
                        undefinedCategories[undefinedCategories.Count - 1].UsedCount++;
                        return;
                    }
                }
            }

            undefinedCategories.Add(new BellTVProgramCategory(categoryID, subCategoryID, description));
            undefinedCategories[undefinedCategories.Count - 1].SampleEvent = sampleEvent;
            undefinedCategories[undefinedCategories.Count - 1].UsedCount++;
        }

        /// <summary>
        /// Log the category usage.
        /// </summary>
        public static void LogCategoryUsage()
        {
            if (categories != null)
            {
                Logger.Instance.WriteSeparator("Program Categories Used");

                foreach (BellTVProgramCategory category in categories)
                {
                    if (category.UsedCount != 0)
                    {
                        if (category.SampleEvent != null)
                            Logger.Instance.Write("Content " + category.CategoryID + "," + category.SubCategoryID +
                                ": " + category.Description +
                                " Used: " + category.UsedCount +
                                " Sample Event: " + category.SampleEvent);
                        else
                            Logger.Instance.Write("Content " + category.CategoryID + "," + category.SubCategoryID +
                                ": " + category.Description +
                                " Used: " + category.UsedCount);
                    }
                }
            }

            if (undefinedCategories != null)
            {
                Logger.Instance.WriteSeparator("Program Categories Undefined");

                foreach (BellTVProgramCategory category in undefinedCategories)
                {
                    if (category.SampleEvent != null)
                        Logger.Instance.Write("Content " + category.CategoryID + "," + category.SubCategoryID +
                            " Used: " + category.UsedCount +
                            " Sample Event: " + category.SampleEvent);
                    else
                        Logger.Instance.Write("Content " + category.CategoryID + "," + category.SubCategoryID +
                            " Used: " + category.UsedCount);
                }
            }
        }

        /// <summary>
        /// Compare this instance with another for sorting purposes.
        /// </summary>
        /// <param name="category">The other instance.</param>
        /// <param name="keyName">The name of the key to compare on.</param>
        /// <returns>Zero if the instances are equal, Greater than 0 if this instance is greater; less than zero otherwise.</returns>
        public int CompareForSorting(BellTVProgramCategory category, string keyName)
        {
            switch (keyName)
            {
                case "CategoryID":
                    if (categoryID == category.CategoryID)
                    {
                        if (subCategoryID == category.SubCategoryID)
                            return (BellTVDescription.CompareTo(category.BellTVDescription));
                        else
                            return (subCategoryID.CompareTo(category.SubCategoryID));
                    }
                    else
                        return (categoryID.CompareTo(category.CategoryID));
                case "SubCategoryID":
                    if (subCategoryID == category.SubCategoryID)
                    {
                        if (categoryID == category.CategoryID)
                            return (BellTVDescription.CompareTo(category.BellTVDescription));
                        else
                            return (categoryID.CompareTo(category.CategoryID));
                    }
                    else
                        return (subCategoryID.CompareTo(category.SubCategoryID));
                case "Description":
                    if (BellTVDescription == category.BellTVDescription)
                    {
                        if (categoryID == category.CategoryID)
                            return (subCategoryID.CompareTo(category.SubCategoryID));
                        else
                            return (categoryID.CompareTo(category.CategoryID));
                    }
                    else
                        return (BellTVDescription.CompareTo(category.BellTVDescription));
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
                    {
                        if (categoryID == category.CategoryID)
                            return (subCategoryID.CompareTo(category.SubCategoryID));
                        else
                            return (categoryID.CompareTo(category.CategoryID));
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

                    if (category.DVBLogicDescription != null)
                        otherDVBLogicDescription = category.DVBLogicDescription;
                    else
                        otherDVBLogicDescription = string.Empty;

                    if (thisDVBLogicDescription == otherDVBLogicDescription)
                    {
                        if (categoryID == category.CategoryID)
                            return (subCategoryID.CompareTo(category.SubCategoryID));
                        else
                            return (categoryID.CompareTo(category.CategoryID));
                    }
                    else
                        return (thisDVBLogicDescription.CompareTo(otherDVBLogicDescription));
                default:
                    return (0);
            }
        }
    }
}
