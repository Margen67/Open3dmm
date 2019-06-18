using Open3dmm.Classes;
using System;
using System.Runtime.InteropServices;

namespace Open3dmm
{
#if NATIVEDEP
    public static class VtableHelper
    {
        public static bool TryGetClassID(int vtable, out ClassID classID)
        {
            if (vtable != 0)
            {
                // Assumes that the vtable is valid
                classID = new ClassID(UnmanagedFunctionCall.ThisCall(Marshal.ReadIntPtr(new IntPtr(vtable), 4), IntPtr.Zero).ToInt32());
                return true;
            }
            classID = default;
            return false;
        }
    }
#endif
}
