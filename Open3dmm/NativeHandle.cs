using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Open3dmm
{
#if NATIVEDEP
    public unsafe class NativeHandle : IDisposable
    {
        private static readonly Dictionary<IntPtr, NativeHandle> nativeDictionary = new Dictionary<IntPtr, NativeHandle>();

        public static NativeHandle[] GetObjects()
        {
            return nativeDictionary.Values.ToArray();
        }

        public static NativeHandle Dereference(IntPtr address)
        {
            if (!nativeDictionary.TryGetValue(address, out var handle))
                handle = new NativeHandle(address);
            return handle;
        }

        public static bool Free(IntPtr address)
        {
            if (!nativeDictionary.TryGetValue(address, out var obj))
                return false;
            obj.Dispose();
            return true;
        }

        public static NativeHandle Alloc(int size)
        {
            return new NativeHandle(size);
        }

        internal int lastVtable;

        private NativeHandle(int size)
        {
            var address = Marshal.AllocHGlobal(size);
            nativeDictionary[address] = this;
            Address = address;
            Size = size;
            while (--size >= 0)
                Marshal.WriteByte(address + size, 0);
        }

        private NativeHandle(IntPtr address)
        {
            if (address.ToPointer() > NativeAbstraction.ModuleHandle.ToPointer())
                nativeDictionary[address] = this;
            lastVtable = *(int*)address;
            Address = address;
            Size = 1024;
        }

        public IntPtr Address { get; }

        public int Size { get; }

        public bool IsDisposed => isDisposed;

        NativeObject managedObject;

        public T QueryInterface<T>() where T : NativeObject
        {
            Type nativeInstanceType;
            int vtable = *(int*)Address;

            if (vtable != lastVtable && VtableHelper.TryGetClassID(vtable, out var classID))
            {
                // Native class is initialized, so we can assume the real instance type.
                nativeInstanceType = Type.GetType($"Open3dmm.Classes.{classID.ToString().ToUpperInvariant()}", true);
                if (managedObject != null && nativeInstanceType != managedObject.GetType())
                {
                    // The managed object has been assigned previously but it doesn't match the native type.

                    if (nativeInstanceType.IsAssignableFrom(managedObject.GetType()))
                    {
                        // The native instance type is a base of the current type.

                        // This only really happens when the object is in
                        // the process of being freed.

                        // Since the object can simply be cast, we reuse the
                        // managed instance.

                        goto ReturnCurrent;
                    }
                    //else
                    //{
                    //    Managed code was called before initialization could complete.
                    //    We need to replace the managedObject with a new one of the new type.
                    //}
                }
                managedObject = (NativeObject)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(nativeInstanceType);
                managedObject.SetHandle(this);
                lastVtable = vtable;
            }
            else if (managedObject == null)
            {
                // Provide a temporary means of accessing the object in managed code.
                var tempInterface = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                tempInterface.SetHandle(this);
                return tempInterface;
            }

        ReturnCurrent:
            nativeInstanceType = managedObject.GetType();
            if (!typeof(T).IsAssignableFrom(nativeInstanceType))
                throw new InvalidOperationException($"Expected type '{typeof(T)}' but instance is of type '{nativeInstanceType}'");
            return (T)managedObject;
        }

        public event Action Disposed;

        #region IDisposable Support
        private bool isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                }

                if (nativeDictionary.Remove(Address))
                    Marshal.FreeHGlobal(Address);
                isDisposed = true;
                Disposed?.Invoke();
            }
        }

        ~NativeHandle()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif