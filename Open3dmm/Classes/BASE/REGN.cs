using Open3dmm.WinApi;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Open3dmm.Classes
{
    public unsafe class REGN : BASE
    {
        [NativeFieldOffset(0x08)]
        public extern ref RECTANGLE Rectangle { get; }

        [NativeFieldOffset(0x18)]
        public extern ref int Unk1 { get; }

        [NativeFieldOffset(0x1C)]
        public extern ref Ref<GLB> ParentList { get; }

        [NativeFieldOffset(0x20)]
        public extern ref IntPtr GdiHandle { get; }

        [NativeFieldOffset(0x24)]
        public extern ref int OffsetX { get; }

        [NativeFieldOffset(0x28)]
        public extern ref int OffsetY { get; }

        [HookFunction(FunctionNames.REGN_CopyRectAndReturnInvalid, CallingConvention = CallingConvention.ThisCall)]
        public bool CopyRectAndReturnInvalid(RECTANGLE* dest)
        {
            var r = Rectangle;
            if (dest != null)
                *dest = r;
            return !(r.Top < r.Bottom && r.Left < r.Right);
        }

        [HookFunction(FunctionNames.REGN_FUN_00426380, CallingConvention = CallingConvention.ThisCall)]
        public unsafe bool FUN_00426380(RECTANGLE* dest)
        {
            if (dest != null)
                *dest = Rectangle;

            return ParentList.Value == null;
        }

        [HookFunction(FunctionNames.REGN_Offset, CallingConvention = CallingConvention.ThisCall)]
        public unsafe void Offset(int offsetX, int offsetY)
        {
            Rectangle.Offset(offsetX, offsetY);
            OffsetX += offsetX;
            OffsetY += offsetY;
        }

        [HookFunction(FunctionNames.REGN_FUN_004262A0, CallingConvention = CallingConvention.ThisCall)]
        public unsafe void FUN_004262A0(RECTANGLE* source)
        {
            if (source == null)
                Rectangle = default;
            else
                Rectangle = *source;
            if (ParentList.Value != null)
            {
                ParentList.Value.DecreaseReferenceCounter();
                ParentList.Value = null;
            }
            UnmanagedFunctionCall.StdCall(new IntPtr(0x0042b950), GdiHandle.AsPointer());
            Unk1 = 0;
        }

        [HookFunction(FunctionNames.REGN_GetOrCreateGdiHandle, CallingConvention = CallingConvention.ThisCall)]
        public unsafe IntPtr GetOrCreateGdiHandle()
        {
            IntPtr result;

            if (GdiHandle != IntPtr.Zero)
            {
                if ((OffsetX != 0) || (OffsetY != 0))
                {
                    PInvoke.Call(LibraryNames.GDI32, "OffsetRgn", GdiHandle, new IntPtr(OffsetX), new IntPtr(OffsetY));
                    OffsetY = 0;
                    OffsetX = 0;
                }
                return GdiHandle;
            }
            OffsetY = 0;
            OffsetX = 0;
            result = this.ThisCall(new IntPtr(0x00427aa0)); // FUN_00427aa0();
            return GdiHandle = result;
        }
    }
}
