using Open3dmm.Classes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Open3dmm
{
    public unsafe class NativeObject : IEquatable<NativeObject>
    {
        private NativeHandle nativeHandle;

        public NativeHandle NativeHandle {
            get {
                if (nativeHandle == null)
                    nativeHandle = NativeHandle.Alloc(GetNativeSize());
                return nativeHandle;
            }
        }

        [NativeFieldOffset(0x0)]
        public extern ref VTABLE Vtable { get; }
        [NativeFieldOffset(0x4)]
        public extern ref int NumReferences { get; }

        private int GetNativeSize()
        {
            return GetType().GetCustomAttribute<NativeClassSizeAttribute>()?.Size ?? throw new InvalidOperationException($"{nameof(NativeClassSizeAttribute)} not defined");
        }

        internal void SetHandle(NativeHandle nativeHandle)
        {
            if (this.nativeHandle != null)
                throw new InvalidOperationException("Native object is already associated with a native handle");
            this.nativeHandle = nativeHandle;
        }

        public static Pointer<T> GetGlobal<T>(int address) where T : unmanaged
        {
            return new Pointer<T>(new IntPtr(address));
        }

        public static T FromPointer<T>(IntPtr ptr) where T : NativeObject
        {
            if (ptr == IntPtr.Zero)
                return default;
            if (!NativeHandle.TryDereference(ptr, out var handle))
                throw new InvalidOperationException();
            return handle.QueryInterface<T>();
        }

        protected void EnsureNotDisposed()
        {
            if (nativeHandle.IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public ref byte GetPinnableReference()
        {
            EnsureNotDisposed();
            unsafe
            {
                return ref *(byte*)nativeHandle.Address;
            }
        }

        #region Equality Implementation

        public override bool Equals(object obj)
        {
            return Equals(obj as NativeObject);
        }

        public bool Equals(NativeObject other)
        {
            return other != null &&
                   EqualityComparer<IntPtr>.Default.Equals(this.nativeHandle.Address, other.nativeHandle.Address);
        }

        public override int GetHashCode()
        {
            return -2057323372 + EqualityComparer<IntPtr>.Default.GetHashCode(this.nativeHandle.Address);
        }

        public static bool operator ==(NativeObject left, NativeObject right)
        {
            return EqualityComparer<NativeObject>.Default.Equals(left, right);
        }

        public static bool operator !=(NativeObject left, NativeObject right)
        {
            return !(left == right);
        }

        #endregion
    }
}
