using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a program category for the category analysis.
    /// </summary>
    public class CategoryEntry
    {
        /// <summary>
        /// Get the network ID.
        /// </summary>
        public int NetworkID { get { return (networkID); } }
        /// <summary>
        /// Get the transport stream ID.
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }
        /// <summary>
        /// Get the service ID.
        /// </summary>
        public int ServiceID { get { return (serviceID); } }
        /// <summary>
        /// Get the program start time.
        /// </summary>
        public DateTime StartTime { get { return (startTime); } }
        /// <summary>
        /// Get the title of the program.
        /// </summary>
        public string EventName { get { return (eventName); } }
        /// <summary>
        /// Get the category of the program.
        /// </summary>
        public int Category { get { return (category); } }
        /// <summary>
        /// Get the subcategory of the program.
        /// </summary>
        public int SubCategory { get { return (subCategory); } }

        private int networkID;
        private int transportStreamID;
        private int serviceID;

        private DateTime startTime;
        private string eventName;

        private int category;
        private int subCategory;

        private CategoryEntry() { }

        /// <summary>
        /// Initialize a new instance of the CategoryEntry class.
        /// </summary>
        /// <param name="networkID">The network ID carrying the program.</param>
        /// <param name="transportStreamID">The transport stream ID carrying the program.</param>
        /// <param name="serviceID">The service ID carrying the program.</param>
        /// <param name="startTime">The start time of the program.</param>
        /// <param name="eventName">The title of the program.</param>
        /// <param name="category">The category of the program.</param>
        public CategoryEntry(int networkID, int transportStreamID, int serviceID, DateTime startTime, string eventName, int category)
        {
            this.networkID = networkID;
            this.transportStreamID = transportStreamID;
            this.serviceID = serviceID;

            this.eventName = eventName;
            this.startTime = startTime;
            this.category = category;            
        }

        /// <summary>
        /// Initialize a new instance of the CategoryEntry class.
        /// </summary>
        /// <param name="networkID">The network ID carrying the program.</param>
        /// <param name="transportStreamID">The transport stream ID carrying the program.</param>
        /// <param name="serviceID">The service ID carrying the program.</param>
        /// <param name="startTime">The start time of the program.</param>
        /// <param name="eventName">The title of the program.</param>
        /// <param name="category">The category of the program.</param>
        /// <param name="subCategory">The subcategory of the program.</param>
        public CategoryEntry(int networkID, int transportStreamID, int serviceID, DateTime startTime, string eventName, int category, int subCategory) : this(networkID, transportStreamID, serviceID, startTime, eventName, category)
        {
            this.subCategory = subCategory;
        }
    }
}
