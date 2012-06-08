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
using System.Threading;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a protected resource.
    /// </summary>
    public abstract class ProtectedResource
    {
        /// <summary>
        /// Get the thread ID that owns the resource.
        /// </summary>
        public int OwnerThreadID { get { return (ownerThreadID); } }
        /// <summary>
        /// Get the name of the thread that owns the resource.
        /// </summary>
        public string OwnerThreadName { get { return (ownerThreadName); } }
        /// <summary>
        /// Return true if the resource is currently locked; false otherwise.
        /// </summary>
        public bool IsLocked { get { return (ownerThreadID != 0); } }

        private Mutex resourceMutex;
        private int ownerThreadID;
        private string ownerThreadName;
        private string identity = string.Empty;

        /// <summary>
        /// Initialize a new instance of the ProtectedResource class.
        /// </summary>
        public ProtectedResource() 
        {
            resourceMutex = new Mutex();
        }

        /// <summary>
        /// Lock the resource.
        /// </summary>
        /// <param name="identity">The identity of the function locking the resource.</param>
        public void Lock(string identity)
        {
            if (Thread.CurrentThread.ManagedThreadId == ownerThreadID)
                return;

            bool reply = resourceMutex.WaitOne(15000, true);
            if (!reply)
                throw (new InvalidOperationException("Resource requested by " + Thread.CurrentThread.Name + "." + identity + " not released by " + getOwnerThreadName() + "." + this.identity));
            
            ownerThreadID = Thread.CurrentThread.ManagedThreadId;
            ownerThreadName = Thread.CurrentThread.Name;
            this.identity = identity;
        }

        /// <summary>
        /// Release the resource.
        /// </summary>
        /// <param name="identity">The identity of the function releasing the resource.</param>
        public void Release(string identity)
        {
            if (ownerThreadID == 0)
                throw (new InvalidOperationException("Thread " + Thread.CurrentThread.Name + "." + identity + " has attempted to release a protected resource not currently locked")); 
            
            if (Thread.CurrentThread.ManagedThreadId != ownerThreadID)
                throw (new InvalidOperationException("Thread " + Thread.CurrentThread.Name + "." + identity + " has attempted to release a protected resource currently owned by " + getOwnerThreadName() + "." + this.identity)); 
                
            ownerThreadID = 0;
            ownerThreadName = null;
            identity = string.Empty;

            resourceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Check ownersip of the resource.
        /// </summary>
        public void CheckOwnership()
        {
            if (ownerThreadID == 0)
                throw (new InvalidOperationException("Type " + this.GetType().Name + " Thread " + Thread.CurrentThread.Name + " has attempted to access a protected resource that is not locked"));

            if (Thread.CurrentThread.ManagedThreadId != ownerThreadID)
                throw (new InvalidOperationException("Type " + this.GetType().Name + " Thread " + Thread.CurrentThread.Name + " has attempted to access a protected resource currently owned by " + getOwnerThreadName()));  
        }

        private string getOwnerThreadName()
        {
            if (ownerThreadName == null)
                return ("Unknown");

            if (ownerThreadName == string.Empty)
                return ("Undefined");

            return (ownerThreadName);
        }
    }
}
