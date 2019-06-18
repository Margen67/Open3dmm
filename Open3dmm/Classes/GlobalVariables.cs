using System;
using System.Runtime.InteropServices;

namespace Open3dmm.Classes
{
    public partial class BASE
    {
        public static readonly FixedArray<RGBQUAD> GlobalPalette = new FixedArray<RGBQUAD>(new IntPtr(0x004E3CA4));
        public static APP GlobalAPPInstance => FromPointer<APP>(GetGlobal<IntPtr>(0x004d5568));
        public static USAC GlobalUSAC => FromPointer<USAC>(GetGlobal<IntPtr>(0x004e39a4));
        public static Pointer<int> GlobalPaletteVersion => GetGlobal<int>(0x004E3CA8);
        public static Pointer<int> GlobalFlushCounter => GetGlobal<int>(0x004E3CAC);
        public static Pointer<bool> GlobalIsPalettedDevice => GetGlobal<bool>(0x004E3CB0);
        public static Pointer<uint> GlobalColorEntries => GetGlobal<uint>(0x004E3CA0);
        public static Pointer<IntPtr> Global004e3c98 => GetGlobal<IntPtr>(0x004e3c98);
        public static GOB GlobalRootGOB => FromPointer<GOB>(GetGlobal<IntPtr>(0x004d585c));
        public static Ref<ERS> PTR_ERS_004e39a8 => Marshal.PtrToStructure<Ref<ERS>>(new IntPtr(0x004e39a8));
    }
}
