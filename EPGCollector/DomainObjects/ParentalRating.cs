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
using System.Xml;
using System.IO;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a parental rating.
    /// </summary>
    public class ParentalRating
    {
        private string location;
        private string system;
        private string protocol;
        private string code;
        private string rating;        
        private string mpaaRating;

        private static Collection<ParentalRating> parentalRatings;

        private static string fileName = "Parental Ratings.cfg";

        private ParentalRating() { }

        /// <summary>
        /// Create a new instance of the ParentalRating class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="system">The name of the rating system.</param>
        /// <param name="protocol">The collection protocol.</param>
        /// <param name="code">The broadcasters code for the rating.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="mpaaRating">The MPAA equivalent.</param>
        public ParentalRating(string location, string system, string protocol, string code, string rating, string mpaaRating)
        {
            this.location = location;
            this.system = system;
            this.protocol = protocol;
            this.code = code;
            this.rating = rating;
            this.mpaaRating = mpaaRating;
        }

        /// <summary>
        /// Load the parental rating collection from the configuration file.
        /// </summary>
        public static int Load()
        {
            string actualFileName = Path.Combine(RunParameters.DataDirectory, fileName);
            if (!File.Exists(actualFileName))
                actualFileName = Path.Combine(RunParameters.ConfigDirectory, fileName);

            Logger.Instance.Write("Loading Parental Ratings from " + actualFileName);

            parentalRatings = new Collection<ParentalRating>();
            
            XmlReader reader = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            try
            {
                reader = XmlReader.Create(actualFileName, settings);
            }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + actualFileName);
                return (0);
            }

            try
            {
                string currentLocation = null;
                string currentSystem = null;
                string currentProtocol = null;

                while (!reader.EOF)
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Location":
                                currentLocation = reader.GetAttribute("code").Trim().ToUpperInvariant();
                                try
                                {
                                    currentSystem = reader.GetAttribute("system").Trim().ToUpperInvariant();
                                }
                                catch (NullReferenceException)
                                {
                                    currentSystem = null;
                                }
                                break;
                            case "Protocol":
                                currentProtocol = reader.GetAttribute("name").Trim().ToUpperInvariant();
                                break;
                            case "ParentalRating":
                                ParentalRating parentalRating = new ParentalRating(currentLocation,
                                    currentSystem,
                                    currentProtocol,
                                    reader.GetAttribute("code").Trim(),
                                    reader.GetAttribute("rating").Trim(),
                                    reader.GetAttribute("mpaaRating").Trim());
                                parentalRatings.Add(parentalRating);
                                break;
                            default:
                                break;
                        }
                    }
                }

                Logger.Instance.Write("Parental ratings loaded");
            }
            catch (XmlException e)
            {
                Logger.Instance.Write("Failed to load file " + actualFileName);
                Logger.Instance.Write("Data exception: " + e.Message);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Failed to load file " + actualFileName);
                Logger.Instance.Write("I/O exception: " + e.Message);
            }

            if (reader != null)
                reader.Close();

            return (parentalRatings.Count);
        }

        /// <summary>
        /// Find the rating system for a rating.
        /// </summary>
        /// <param name="location">The location of the rating.</param>
        /// <param name="protocol">The collection protocol.</param>
        /// <param name="code">The broadcasters code for the rating.</param>
        /// <returns>The system or null if it cannot be located.</returns>
        public static string FindSystem(string location, string protocol, string code)
        {
            if (parentalRatings == null)
                return (null);

            foreach (ParentalRating parentalRating in parentalRatings)
            {
                if (parentalRating.location == location && parentalRating.protocol == protocol && parentalRating.code == code)
                    return (parentalRating.system);
            }

            return (null);
        }

        /// <summary>
        /// Find the rating.
        /// </summary>
        /// <param name="location">The location of the rating.</param>
        /// <param name="protocol">The collection protocol.</param>
        /// <param name="code">The broadcasters code for the rating.</param>
        /// <returns>The rating or null if it cannot be located.</returns>
        public static string FindRating(string location, string protocol, string code)
        {
            if (parentalRatings == null)
                return (null);

            foreach (ParentalRating parentalRating in parentalRatings)
            {
                if (parentalRating.location == location && parentalRating.protocol == protocol && parentalRating.code == code)
                    return (parentalRating.rating);
            }

            return (null);
        }

        /// <summary>
        /// Find the equivalent MPAA rating.
        /// </summary>
        /// <param name="location">The location of the rating.</param>
        /// <param name="protocol">The collection protocol.</param>
        /// <param name="code">The broadcasters code for the rating.</param>
        /// <returns>The MPPA rating or null if it cannot be located.</returns>
        public static string FindMpaaRating(string location, string protocol, string code)
        {
            if (parentalRatings == null)
                return (null);

            foreach (ParentalRating parentalRating in parentalRatings)
            {
                if (parentalRating.location == location && parentalRating.protocol == protocol && parentalRating.code == code)
                    return (parentalRating.mpaaRating);
            }

            return (null);
        }
    }
}
