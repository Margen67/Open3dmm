using Open3dmm.Classes;
using Open3dmm.WinApi;
using System;
using System.Runtime.InteropServices;

namespace Open3dmm
{
    internal static class GDIHelper
    {
        public static short MinExtent => Marshal.ReadInt16(new IntPtr(0x4D5328));

        public static void LimitFunction(in int input, out int output)
        {
            output = 0x7fff;
            if (input < 0x8000)
            {
                output = input;
                if (input < MinExtent)
                    output = MinExtent;
            }
        }

        [HookFunction(FunctionNames.FlushGDI, CallingConvention = CallingConvention.StdCall)]
        public static void FlushGdi()
        {
            PInvoke.Call(LibraryNames.GDI32, "GdiFlush");
            BASE.GlobalFlushCounter.Value++;
            return;
        }
    }
}
