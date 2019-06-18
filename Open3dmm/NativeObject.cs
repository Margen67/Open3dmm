#if NATIVEDEP
using Open3dmm.Classes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Open3dmm
{
    public unsafe class NativeObject
    {
        private NativeHandle nativeHandle;

        public NativeHandle NativeHandle => nativeHandle;

        public NativeObject()
        {
            if (nativeHandle == null)
                SetHandle(NativeHandle.Alloc(GetNativeSize()));
        }

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

        public static T FromPointer<T>(IntPtr ptr) where T : NativeObject
        {
            if (ptr == IntPtr.Zero)
                return default;
            return NativeHandle.Dereference(ptr).QueryInterface<T>();
        }
    }
}
#endif
