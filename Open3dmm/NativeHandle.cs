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

        public T ChangeType<T>() where T : NativeObject
        {
            var newObj = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
            managedObject = newObj;
            managedObject.SetHandle(this);
            return newObj;
        }

        public static bool TryDereference(IntPtr address, out NativeHandle handle)
        {
            if (address == IntPtr.Zero)
            {
                handle = default;
                return false;
            }
            if (!nativeDictionary.TryGetValue(address, out handle))
                handle = new NativeHandle(address);
            return true;
        }

        public static NativeHandle Dereference(IntPtr address)
        {
            if (!TryDereference(address, out var handle))
                throw new ArgumentException("No handle found at specified address");
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
            nativeDictionary[address] = this;
            Address = address;
            Size = 1024;
        }

        public IntPtr Address { get; }

        public int Size { get; }

        public bool IsDisposed => isDisposed;

        NativeObject managedObject;

        public T QueryInterface<T>() where T : NativeObject
        {
            if (!(managedObject is T))
                ChangeType<T>();

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

                nativeDictionary.Remove(Address);
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