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

using System.Collections.ObjectModel;
using System.Text;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a DVB tuner.
    /// </summary>
    public class Tuner
    {
        /// <summary>
        /// Get the current tuner.
        /// </summary>
        public static Tuner CurrentTuner { get { return (currentTuner); } }

        /// <summary>
        /// Get all the tuners with drivers on this machine.
        /// </summary>
        public static ObservableCollection<Tuner> TunerCollection
        {
            get
            {
                if (tunerCollection == null)
                    tunerCollection = new ObservableCollection<Tuner>();
                return (tunerCollection);
            }
            set { tunerCollection = value; }
        }

        /// <summary>
        /// Return true if there is at least one tuner installed; false otherwise.
        /// </summary>
        public static bool TunerPresent { get { return (tunerCollection.Count > 0); } }

        private static ObservableCollection<Tuner> tunerCollection;

        /// <summary>
        /// Get the Windows path for the tuner.
        /// </summary>
        public string Path { get { return(path); } }

        /// <summary>
        /// Get the name of the tuner.
        /// </summary>
        public string Name
        {
            get 
            {
                if (name != null)
                    return (name);
                else
                    return (string.Empty);
            }
            set { name = value; }
        }

        /// <summary>
        /// Get or set the collection of nodes defined by the tuner.
        /// </summary>
        public Collection<TunerNode> TunerNodes
        {
            get 
            {
                if (tunerNodes == null)
                    tunerNodes = new Collection<TunerNode>();
                return (tunerNodes); 
            }
            set { tunerNodes = value; }
        }

        /// <summary>
        /// Get or set the frequency being used by the tuner.
        /// </summary>
        public int Frequency
        {
            get { return (frequency); }
            set { frequency = value; }
        }

        /// <summary>
        /// Get or set the tuner tag.
        /// </summary>
        public object Tag
        {
            get { return (tag); }
            set { tag = value; }
        }

        /// <summary>
        /// Return true if the tuner is in use; false otherwise.
        /// </summary>
        public bool InUse 
        { 
            get { return (frequency != -1); }
            set
            {
                if (!value)
                    frequency = -1;
            }
        }

        /// <summary>
        /// Get the tuner node that supports DVB-S. 
        /// </summary>
        public TunerNode DVBSatelliteNode
        {
            get
            {
                if (TunerNodes == null)
                    return(null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.Satellite)
                            return(tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner node that supports DVB-T.
        /// </summary>
        public TunerNode DVBTerrestrialNode
        {
            get
            {
                if (TunerNodes == null)
                    return (null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.Terrestrial)
                            return (tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner node that supports DVB-C.
        /// </summary>
        public TunerNode DVBCableNode
        {
            get
            {
                if (TunerNodes == null)
                    return (null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.Cable)
                            return (tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner node that supports ATSC.
        /// </summary>
        public TunerNode AtscNode
        {
            get
            {
                if (TunerNodes == null)
                    return (null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.ATSC)
                            return (tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner node that supports Clear QAM.
        /// </summary>
        public TunerNode ClearQamNode
        {
            get
            {
                if (TunerNodes == null)
                    return (null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.Cable)
                            return (tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner node that supports ISDB satellite.
        /// </summary>
        public TunerNode ISDBSatellite
        {
            get
            {
                if (TunerNodes == null)
                    return (null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.ISDBS)
                            return (tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner node that supports ISDB terrestrial.
        /// </summary>
        public TunerNode ISDBTerrestrial
        {
            get
            {
                if (TunerNodes == null)
                    return (null);
                else
                {
                    foreach (TunerNode tunerNode in TunerNodes)
                    {
                        if (tunerNode.TunerNodeType == TunerNodeType.ISDBT)
                            return (tunerNode);
                    }
                    return (null);
                }
            }
        }

        /// <summary>
        /// Get the tuner that supports DVB-S.
        /// </summary>
        public static Tuner SatelliteTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.DVBSatelliteNode != null)
                        return (tuner);
                }
                return (null);
            }
        }

        /// <summary>
        /// Get the tuner that supports DVB-T.
        /// </summary>
        public static Tuner TerrestrialTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.DVBTerrestrialNode != null)
                        return (tuner);
                }
                return (null);
            }
        }

        /// <summary>
        /// Get the tuner that supports DVB-C.
        /// </summary>
        public static Tuner CableTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.DVBCableNode != null)
                        return (tuner);
                }
                return (null);
            }
        }

        /// <summary>
        /// Get the tuner that supports ATSC.
        /// </summary>
        public static Tuner AtscTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.AtscNode != null)
                        return (tuner);
                }
                return (null);
            }
        }

        /// <summary>
        /// Get the tuner that supports Clear QAM.
        /// </summary>
        public static Tuner ClearQamTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.ClearQamNode != null)
                        return (tuner);
                }
                return (null);
            }
        }

        /// <summary>
        /// Get the tuner that supports ISDB satellite.
        /// </summary>
        public static Tuner ISDBSatelliteTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.ISDBSatellite != null)
                        return (tuner);
                }
                return (null);
            }
        }

        /// <summary>
        /// Get the tuner that supports ISDB terrestrial.
        /// </summary>
        public static Tuner ISDBTerrestrialTuner
        {
            get
            {
                foreach (Tuner tuner in TunerCollection)
                {
                    if (tuner.ISDBTerrestrial != null)
                        return (tuner);
                }
                return (null);
            }
        }

        private string path;
        private string name;
        private Collection<TunerNode> tunerNodes;
        private int frequency = -1;

        private object tag;

        private static Tuner currentTuner;

        private Tuner() { }

        /// <summary>
        /// Initialise a new instance of the Tuner class.
        /// </summary>
        /// <param name="path">The Windows path for the tuner.</param>
        public Tuner(string path)
        {
            this.path = path;

            currentTuner = this;
        }

        /// <summary>
        /// Check if the tuner has a particular tuner node type.
        /// </summary>
        /// <param name="checkTunerNodeType">The tuner node type.</param>
        /// <returns>True if the node type is supported; false otherwise.</returns>
        public bool Supports(TunerNodeType checkTunerNodeType)
        {
            if (TunerNodes == null)
                return (false);

            foreach (TunerNode tunerNode in TunerNodes)
            {
                if (checkTunerNodeType == tunerNode.TunerNodeType)
                    return (true);
            }

            return (false);            
        }

        /// <summary>
        /// Check if the tuner is a particular tuner type.
        /// </summary>
        /// <param name="checkTunerType">The tuner type.</param>
        /// <returns>True if the tuner type is supported; false otherwise.</returns>
        public bool Supports(TunerType checkTunerType)
        {
            switch (checkTunerType)
            {
                case TunerType.Satellite:
                    return (Supports(TunerNodeType.Satellite));
                case TunerType.Terrestrial:
                    return (Supports(TunerNodeType.Terrestrial));
                case TunerType.Cable:
                    return (Supports(TunerNodeType.Cable));
                case TunerType.ATSC:
                    return (Supports(TunerNodeType.ATSC));
                case TunerType.ClearQAM:
                    return (Supports(TunerNodeType.Cable));
                case TunerType.ISDBS:
                    return (Supports(TunerNodeType.ISDBS));
                case TunerType.ISDBT:
                    return (Supports(TunerNodeType.ISDBT));
                default:
                    return (Supports(TunerNodeType.Satellite));
            }

        }

        /// <summary>
        /// Get a specific tuner node.
        /// </summary>
        /// <param name="checkTunerNodeType">The tuner node type requested.</param>
        /// <returns>The tuner node of the specified type or null if not supported.</returns>
        public TunerNode GetNode(TunerNodeType checkTunerNodeType)
        {
            if (TunerNodes == null)
                return (null);

            foreach (TunerNode tunerNode in TunerNodes)
            {
                if (checkTunerNodeType == tunerNode.TunerNodeType)
                return (tunerNode);
            }

            return (null);
        }

        /// <summary>
        /// Get a description of this tuner.
        /// </summary>
        /// <returns>A string describing the tuner.</returns>
        public override string ToString()
        {
            if (TunerNodes == null || TunerNodes.Count == 0)
                return (name + " (No tuner node type available)");

            StringBuilder stringBuilder = new StringBuilder();

            foreach (TunerNode tunerNode in TunerNodes)
            {
                if (tunerNode.TunerNodeType != TunerNodeType.Other)
                {
                    if (stringBuilder.Length != 0)
                        stringBuilder.Append(", ");
                    stringBuilder.Append(tunerNode.TunerNodeType.ToString());
                }
            }

            return (name + " (" + stringBuilder + ")");
        }
    }
}
