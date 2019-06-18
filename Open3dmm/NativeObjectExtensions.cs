using Open3dmm.Classes;
using System;

namespace Open3dmm
{
#if NATIVEDEP
    public static class NativeObjectExtensions
    {
        #region VirtualCall
        public static IntPtr VirtualCall(this NativeObject obj, int offset)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }
        public static IntPtr VirtualCall(this NativeObject obj, int offset, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11, IntPtr arg12)
        {
            return UnmanagedFunctionCall.ThisCall(((BASE)obj).Vtable[offset / 0x4], obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        #endregion

        #region ThisCall
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }
        public static IntPtr ThisCall(this NativeObject obj, IntPtr address, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11, IntPtr arg12)
        {
            return UnmanagedFunctionCall.ThisCall(address, obj.NativeHandle.Address, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        #endregion  
    }
#endif
}
