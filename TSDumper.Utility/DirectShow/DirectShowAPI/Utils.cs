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
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Security;
using System.Runtime.InteropServices.ComTypes;

namespace DirectShowAPI
{
    #region Declarations

    /// <summary>
    /// From CDEF_CLASS_* defines
    /// </summary>
    [Flags]
    public enum CDef
    {
        None = 0,
        ClassDefault = 0x0001,
        BypassClassManager = 0x0002,
        ClassLegacy = 0x0004,
        MeritAboveDoNotUse = 0x0008,
        DevmonCMGRDevice = 0x0010,
        DevmonDMO = 0x0020,
        DevmonPNPDevice = 0x0040,
        DevmonFilter = 0x0080,
        DevmonSelectiveMask = 0x00f0
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
            [Out] out IEnumMoniker ppEnumMoniker,
            [In] CDef dwFlags);
    }

    /// <summary>
    /// The connected status of a pin. These are not DirectShow values.
    /// </summary>
    public enum PinConnectedStatus
    {
        Unconnected,
        Connected
    }    

    /// <summary>
    /// DsLong is a wrapper class around a <see cref="System.Int64"/> value type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class DsLong
    {
        private long Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DsLong with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsLong(long Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DsLong Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DsLong and Int64.
        /// </summary>
        /// <param name="l">DsLong to be cast.</param>
        /// <returns>A casted System.Int64.</returns>
        public static implicit operator long(DsLong l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int64 and DsLong.
        /// </summary>
        /// <param name="l">System.Int64 to be cast.</param>
        /// <returns>A casted DsLong.</returns>
        public static implicit operator DsLong(long l)
        {
            return new DsLong(l);
        }

        /// <summary>
        /// Get the System.Int64 equivalent to this  instance.
        /// </summary>
        /// <returns>A System.Int64.</returns>
        public long ToInt64()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DsLong instance for a given System.Int64
        /// </summary>
        /// <param name="l">The System.Int64 to wrap into a DsLong.</param>
        /// <returns>A new instance of DsLong.</returns>
        public static DsLong FromInt64(long l)
        {
            return new DsLong(l);
        }
    }

    /// <summary>
    /// DirectShowLib.DsGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class DsGuid
    {
        [FieldOffset(0)]
        private Guid guid;

        public static readonly DsGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor. 
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public DsGuid()
        {
            this.guid = Guid.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="g">A valid System.Guid as string</param>
        public DsGuid(string g)
        {
            this.guid = new Guid(g);
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="g">A System.Guid value type</param>
        public DsGuid(Guid g)
        {
            this.guid = g;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="System.Guid.ToString"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return this.guid.ToString(format);
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new DsGuid instance
        ///   DsGuid dsG = new DsGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between DsGuid and Guid
        ///   Guid g = dsG;
        ///
        ///   Console.WriteLine(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(DsGuid g)
        {
            return g.guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and DirectShowLib.DsGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and DsGuid
        ///   DsGuid dsG = g;
        ///
        ///   Console.WriteLine(dsG.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Guid to be cast</param>
        /// <returns>A casted DirectShowLib.DsGuid</returns>
        public static implicit operator DsGuid(Guid g)
        {
            return new DsGuid(g);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this DirectShowLib.DsGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return this.guid;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsGuid instance for a given System.Guid
        /// </summary>
        /// <param name="g">The System.Guid to wrap into a DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsGuid FromGuid(Guid g)
        {
            return new DsGuid(g);
        }
    }

    /// <summary>
    /// DirectShowLib.DsInt is a wrapper class around a <see cref="System.Int32"/> value type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class DsInt
    {
        private int Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsInt with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsInt(int Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsInt Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsInt and System.Int64 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsInt.ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsInt instance
        ///   DsInt dsI = new DsInt(0x12345678);
        ///   // Do implicit cast between DsInt and Int32
        ///   int i = dsI;
        ///
        ///   Console.WriteLine(i.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsInt to be cast</param>
        /// <returns>A casted System.Int32</returns>
        public static implicit operator int(DsInt l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int32 and DirectShowLib.DsInt for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt32"/> for similar functionality.
        /// <code>
        ///   // Define a new Int32 instance
        ///   int i = 0x12345678;
        ///   // Do implicit cast between Int64 and DsInt
        ///   DsInt dsI = i;
        ///
        ///   Console.WriteLine(dsI.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Int32 to be cast</param>
        /// <returns>A casted DirectShowLib.DsInt</returns>
        public static implicit operator DsInt(int l)
        {
            return new DsInt(l);
        }

        /// <summary>
        /// Get the System.Int32 equivalent to this DirectShowLib.DsInt instance.
        /// </summary>
        /// <returns>A System.Int32</returns>
        public int ToInt32()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsInt instance for a given System.Int32
        /// </summary>
        /// <param name="g">The System.Int32 to wrap into a DirectShowLib.DsInt</param>
        /// <returns>A new instance of DirectShowLib.DsInt</returns>
        public static DsInt FromInt32(int l)
        {
            return new DsInt(l);
        }
    }

    /// <summary>
    /// DirectShowLib.DsShort is a wrapper class around a <see cref="System.Int16"/> value type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class DsShort
    {
        private short Value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsShort with the Value parameter
        /// </summary>
        /// <param name="Value">Value to assign to this new instance</param>
        public DsShort(short Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsShort Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsShort and System.Int16 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsShort.ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsShort instance
        ///   DsShort dsS = new DsShort(0x1234);
        ///   // Do implicit cast between DsShort and Int16
        ///   short s = dsS;
        ///
        ///   Console.WriteLine(s.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">DirectShowLib.DsShort to be cast</param>
        /// <returns>A casted System.Int16</returns>
        public static implicit operator short(DsShort l)
        {
            return l.Value;
        }

        /// <summary>
        /// Define implicit cast between System.Int16 and DirectShowLib.DsShort for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DirectShowLib.DsGuid.FromInt16"/> for similar functionality.
        /// <code>
        ///   // Define a new Int16 instance
        ///   short s = 0x1234;
        ///   // Do implicit cast between Int64 and DsShort
        ///   DsShort dsS = s;
        ///
        ///   Console.WriteLine(dsS.ToString());
        /// </code>
        /// </summary>
        /// <param name="g">System.Int16 to be cast</param>
        /// <returns>A casted DirectShowLib.DsShort</returns>
        public static implicit operator DsShort(short l)
        {
            return new DsShort(l);
        }

        /// <summary>
        /// Get the System.Int16 equivalent to this DirectShowLib.DsShort instance.
        /// </summary>
        /// <returns>A System.Int16</returns>
        public short ToInt16()
        {
            return this.Value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsShort instance for a given System.Int64
        /// </summary>
        /// <param name="g">The System.Int16 to wrap into a DirectShowLib.DsShort</param>
        /// <returns>A new instance of DirectShowLib.DsShort</returns>
        public static DsShort FromInt16(short l)
        {
            return new DsShort(l);
        }
    }

    #endregion

    #region Utility Classes

    public static class DsError
    {
        [DllImport("quartz.dll", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "AMGetErrorTextW"),
        SuppressUnmanagedCodeSecurity]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If DirectShow error text
        /// is available, it is used to build the exception, otherwise a generic com error
        /// is thrown.
        /// </summary>
        /// <param name="hr">The HRESULT to check</param>
        public static void ThrowExceptionForHR(int hr)
        {
            // If a severe error has occurred
            if (hr < 0)
            {
                string s = GetErrorText(hr);

                // If a string is returned, build a com error from it
                if (s != null)
                {
                    throw new COMException(s, hr);
                }
                else
                {
                    // No string, just use standard com error
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        /// <summary>
        /// Returns a string describing a DS error.  Works for both error codes
        /// (values < 0) and Status codes (values >= 0)
        /// </summary>
        /// <param name="errorCode">HRESULT for which to get description</param>
        /// <returns>The string, or null if no error text can be found</returns>
        public static string GetErrorText(int errorCode)
        {
            const int maxTextLength = 160;

            StringBuilder messageBuffer = new StringBuilder(maxTextLength, maxTextLength);

            if (AMGetErrorText(errorCode, messageBuffer, maxTextLength) > 0)
                return messageBuffer.ToString();

            return null;
        }
    }
     
    public static class DsUtils
    {
        /// <summary>
        /// Convert an EPG Collector DVB-S2 pilot value to a BDA value.
        /// </summary>
        /// <param name="pilot">The EPG Collector pilot value.</param>
        /// <returns>The BDA pilot value.</returns>
        public static Pilot GetNativePilot(DomainObjects.Pilot pilot)
        {
            switch (pilot)
            {
                case DomainObjects.Pilot.NotDefined:
                    return (Pilot.NotDefined);
                case DomainObjects.Pilot.NotSet:
                    return (Pilot.NotSet);
                case DomainObjects.Pilot.Off:
                    return (Pilot.Off);
                case DomainObjects.Pilot.On:
                    return (Pilot.On);
                default:
                    return (Pilot.NotSet);
            }
        }

        /// <summary>
        /// Convert an EPG Collector DVB-S2 rolloff value to a BDA value.
        /// </summary>
        /// <param name="rollOff">The EPG Collector rolloff value.</param>
        /// <returns>The BDA rolloff value.</returns>
        public static RollOff GetNativeRollOff(DomainObjects.RollOff rollOff)
        {
            switch (rollOff)
            {
                case DomainObjects.RollOff.NotDefined:
                    return (RollOff.NotDefined);
                case DomainObjects.RollOff.NotSet:
                    return (RollOff.NotSet);
                case DomainObjects.RollOff.RollOff20:
                    return (RollOff.Twenty);
                case DomainObjects.RollOff.RollOff25:
                    return (RollOff.TwentyFive);
                default:
                    return (RollOff.NotSet);
            }
        }

        /// <summary>
        ///  Free the nested structures and release any
        ///  COM objects within an AMMediaType struct.
        /// </summary>
        public static void FreeAMMediaType(AMMediaType mediaType)
        {
            if (mediaType != null)
            {
                if (mediaType.formatSize != 0)
                {
                    Marshal.FreeCoTaskMem(mediaType.formatPtr);
                    mediaType.formatSize = 0;
                    mediaType.formatPtr = IntPtr.Zero;
                }
                if (mediaType.unkPtr != IntPtr.Zero)
                {
                    Marshal.Release(mediaType.unkPtr);
                    mediaType.unkPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        ///  Free the nested interfaces within a PinInfo struct.
        /// </summary>
        public static void FreePinInfo(PinInfo pinInfo)
        {
            if (pinInfo.filter != null)
            {
                Marshal.ReleaseComObject(pinInfo.filter);
                pinInfo.filter = null;
            }
        }

    }

    public class DsDevice : IDisposable
    {
        public IMoniker Moniker { get { return moniker; } }

        public string Name
        {
            get
            {
                if (name == null)
                    name = getPropBagValue("FriendlyName");
                return (name);
            }
        }

        /// <summary>
        /// Returns a unique identifier for a device
        /// </summary>
        public string DevicePath
        {
            get
            {
                string devicePath = null;

                try
                {
                    moniker.GetDisplayName(null, null, out devicePath);
                }
                catch { }

                return (devicePath);
            }
        }

        /// <summary>
        /// Returns the ClassID for a device
        /// </summary>
        public Guid ClassID
        {
            get
            {
                Guid guid;

                moniker.GetClassID(out guid);

                return (guid);
            }
        }

        private IMoniker moniker;
        private string name;

        public DsDevice(IMoniker moniker)
        {
            this.moniker = moniker;
            name = null;
        }

        /// <summary>
        /// Returns an array of DsDevices for a filter category.
        /// </summary>
        /// <param name="filterCategory">Any one of FilterCategory</param>
        public static DsDevice[] GetDevicesOfCat(Guid FilterCategory)
        {
            int hr;

            // Use arrayList to build the return list since it is easily resizable
            DsDevice[] devret;
            ArrayList devs = new ArrayList();
            IEnumMoniker enumMon;

            ICreateDevEnum enumDev = (ICreateDevEnum)new CreateDevEnum();
            hr = enumDev.CreateClassEnumerator(FilterCategory, out enumMon, 0);
            DsError.ThrowExceptionForHR(hr);

            // CreateClassEnumerator returns null for enumMon if there are no entries
            if (hr != 1)
            {
                try
                {
                    try
                    {
                        IMoniker[] mon = new IMoniker[1];

                        while ((enumMon.Next(1, mon, IntPtr.Zero) == 0))
                        {
                            try
                            {
                                // The devs array now owns this object.  Don't
                                // release it if we are going to be successfully
                                // returning the devret array
                                devs.Add(new DsDevice(mon[0]));
                            }
                            catch
                            {
                                Marshal.ReleaseComObject(mon[0]);
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(enumMon);
                    }

                    // Copy the ArrayList to the DsDevice[]
                    devret = new DsDevice[devs.Count];
                    devs.CopyTo(devret);
                }
                catch
                {
                    foreach (DsDevice d in devs)
                    {
                        d.Dispose();
                    }
                    throw;
                }
            }
            else
            {
                devret = new DsDevice[0];
            }

            return devret;
        }

        /// <summary>
        /// Get a specific PropertyBag value from a moniker
        /// </summary>
        /// <param name="sPropName">The name of the value to retrieve</param>
        /// <returns>String or null on error</returns>
        private string getPropBagValue(string sPropName)
        {
            IPropertyBag bag = null;
            string returnValue = null;
            object bagObj = null;
            object val = null;

            try
            {
                Guid bagId = typeof(IPropertyBag).GUID;
                moniker.BindToStorage(null, null, ref bagId, out bagObj);

                bag = (IPropertyBag)bagObj;

                int hr = bag.Read(sPropName, out val, null);
                DsError.ThrowExceptionForHR(hr);

                returnValue = val as string;
            }
            catch
            {
                returnValue = null;
            }
            finally
            {
                bag = null;
                if (bagObj != null)
                {
                    Marshal.ReleaseComObject(bagObj);
                    bagObj = null;
                }
            }

            return (returnValue);
        }

        public void Dispose()
        {
            if (moniker != null)
            {
                Marshal.ReleaseComObject(moniker);
                moniker = null;
            }

            name = null;
        }
    }


    public static class DsFindPin
    {
        /// <summary>
        /// Scans a filter's pins looking for a pin in the specified direction
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vDir">The direction to find</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin in the specified direction)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByDirection(IBaseFilter vSource, PinDirection vDir, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            PinDirection ppindir;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                while (ppEnum.Next(1, pPins, IntPtr.Zero) == 0)
                {
                    // Read the direction
                    hr = pPins[0].QueryDirection(out ppindir);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right direction?
                    if (ppindir == vDir)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    Marshal.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified name
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vPinName">The pin name to find</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByName(IBaseFilter vSource, string vPinName)
        {
            int hr;
            IEnumPins ppEnum;
            PinInfo ppinfo;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                while (ppEnum.Next(1, pPins, IntPtr.Zero) == 0)
                {
                    // Read the info
                    hr = pPins[0].QueryPinInfo(out ppinfo);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right name?
                    if (ppinfo.name == vPinName)
                    {
                        DsUtils.FreePinInfo(ppinfo);
                        pRet = pPins[0];
                        break;
                    }
                    Marshal.ReleaseComObject(pPins[0]);
                    DsUtils.FreePinInfo(ppinfo);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(ppEnum);
            }

            return pRet;
        }
    }

    // This abstract class contains definitions for use in implementing a custom marshaler.
    //
    // MarshalManagedToNative() gets called before the COM method, and MarshalNativeToManaged() gets
    // called after.  This allows for allocating a correctly sized memory block for the COM call,
    // then to break up the memory block and build an object that c# can digest.

    abstract internal class DsMarshaler : ICustomMarshaler
    {
        #region Data Members
        // The cookie isn't currently being used.
        protected string m_cookie;

        // The managed object passed in to MarshalManagedToNative, and modified in MarshalNativeToManaged
        protected object m_obj;
        #endregion

        // The constructor.  This is called from GetInstance (below)
        public DsMarshaler(string cookie)
        {
            // If we get a cookie, save it.
            m_cookie = cookie;
        }

        // Called just before invoking the COM method.  The returned IntPtr is what goes on the stack
        // for the COM call.  The input arg is the parameter that was passed to the method.
        virtual public IntPtr MarshalManagedToNative(object managedObj)
        {
            // Save off the passed-in value.  Safe since we just checked the type.
            m_obj = managedObj;

            // Create an appropriately sized buffer, blank it, and send it to the marshaler to
            // make the COM call with.
            int iSize = GetNativeDataSize() + 3;
            IntPtr p = Marshal.AllocCoTaskMem(iSize);

            for (int x = 0; x < iSize / 4; x++)
            {
                Marshal.WriteInt32(p, x * 4, 0);
            }

            return p;
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        virtual public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return m_obj;
        }

        // Release the (now unused) buffer
        virtual public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (pNativeData != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pNativeData);
            }
        }

        // Release the (now unused) managed object
        virtual public void CleanUpManagedData(object managedObj)
        {
            m_obj = null;
        }

        // This routine is (apparently) never called by the marshaler.  However it can be useful.
        abstract public int GetNativeDataSize();

        // GetInstance is called by the marshaler in preparation to doing custom marshaling.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.

        // It is commented out in this abstract class, but MUST be implemented in derived classes
        //public static ICustomMarshaler GetInstance(string cookie)
    }

    // c# does not correctly marshal arrays of pointers.
    //

    internal class EMTMarshaler : DsMarshaler
    {
        public EMTMarshaler(string cookie) : base(cookie)
        {
        }

        // Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        // from MarshalManagedToNative.  The return value is unused.
        override public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            AMMediaType[] emt = m_obj as AMMediaType[];

            for (int x = 0; x < emt.Length; x++)
            {
                // Copy in the value, and advance the pointer
                IntPtr p = Marshal.ReadIntPtr(pNativeData, x * IntPtr.Size);
                if (p != IntPtr.Zero)
                {
                    emt[x] = (AMMediaType)Marshal.PtrToStructure(p, typeof(AMMediaType));
                }
                else
                {
                    emt[x] = null;
                }
            }

            return null;
        }

        // The number of bytes to marshal out
        override public int GetNativeDataSize()
        {
            // Get the array size
            int i = ((Array)m_obj).Length;

            // Multiply that times the size of a pointer
            int j = i * IntPtr.Size;

            return j;
        }

        // This method is called by interop to create the custom marshaler.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new EMTMarshaler(cookie);
        }
    }

    #endregion
}
