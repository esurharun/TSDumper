using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DomainObjects;

namespace DVBServices
{
    internal class BellShortEventDescriptor : DVBShortEventDescriptor
    {
        /// <summary>
        /// Get the event name.
        /// </summary>
        public override string EventName { get { return (getEventName()); } }
        /// <summary>
        /// Get the short description.
        /// </summary>
        public override string ShortDescription { get { return (getShortDescription()); } }
        /// <summary>
        /// Get the short description.
        /// </summary>
        public bool HighDefinition { get { return (base.EventName.StartsWith("HD-") || base.EventName.StartsWith("HD - ")); } }

        internal string getEventName()
        {
            string eventName = base.EventName;

            if (eventName.StartsWith("HD-"))
                return (eventName.Substring(3));
            else
            {
                if (eventName.StartsWith("HD - "))
                    return(eventName.Substring(5));
                else
                    return (eventName);
            }
        }

        internal string getShortDescription()
        {
            return (base.ShortDescription);
        }
    }
}
