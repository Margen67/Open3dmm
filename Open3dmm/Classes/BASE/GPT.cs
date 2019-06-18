using Open3dmm.WinApi;
using System;
using System.Runtime.InteropServices;

namespace Open3dmm.Classes
{
    [NativeClassSize(0x8c)]
    public unsafe class GPT : BASE
    {
        [NativeFieldOffset(0x08)]
        public extern ref Ref<REGN> Region { get; }
        [NativeFieldOffset(0x0C)]
        public extern ref RECTANGLE ClipRect { get; }
        [NativeFieldOffset(0x1C)]
        public extern ref int OffsetX { get; }
        [NativeFieldOffset(0x20)]
        public extern ref int OffsetY { get; }
        [NativeFieldOffset(0x24)]
        public extern ref IntPtr DC { get; }
        [NativeFieldOffset(0x28)]
        public extern ref IntPtr WindowHandle { get; }
        [NativeFieldOffset(0x2C)]
        public extern ref IntPtr DIBPointer { get; }
        [NativeFieldOffset(0x30)]
        public extern ref IntPtr PixelBuffer { get; }
        [NativeFieldOffset(0x3C)]
        public extern ref RECTANGLE Bounds { get; }
        [NativeFieldOffset(0x34)]
        public extern ref int BitDepth { get; }
        [NativeFieldOffset(0x38)]
        public extern ref int Stride { get; }
        [NativeFieldOffset(0x4C)]
        public extern ref int PaletteVersion { get; }
        [NativeFieldOffset(0x50)]
        public extern ref int FlushCounter { get; }
        [NativeFieldOffset(0x54)]
        public extern ref int Field_0x54 { get; }
        [NativeFieldOffset(0x68)]
        public extern ref int Field_0x68 { get; }
        [NativeFieldOffset(0x80)]
        public extern ref int Field_0x80 { get; }
        [NativeFieldOffset(0x84)]
        public extern ref int Field_0x84 { get; }
        [NativeFieldOffset(0x88)]
        public extern ref uint UnkFlags { get; }

        public int Width => Bounds.Right - Bounds.Left;
        public int Height => Bounds.Bottom - Bounds.Top;

        private GPT() : base()
        {
            Vtable = new VTABLE(0x4df278);
            Field_0x68 = 0;
        }

        [HookFunction(FunctionNames.GPT_BlitMBMP, CallingConvention = CallingConvention.ThisCall)]
        public void BlitMBMP(MBMP mbmp, RECTANGLE* dest, GNV_UnkStruct1* unkStruct)
        {
            RECTANGLE clip = default;

            if (unkStruct->Clip != null)
            {
                clip.Copy(unkStruct->Clip);
                if (!clip.CalculateIntersection(dest))
                    return;
            }
            else
            {
                clip.Copy(dest);
            }

            mbmp.GetRect(out var mbmpRect); // call 0x0043F7D0 
            if (mbmpRect.Top < mbmpRect.Bottom && mbmpRect.Left < mbmpRect.Right)
            {
                if (BitDepth == 8)
                {
                    if (dest->Left - dest->Right + mbmpRect.Right != mbmpRect.Left || dest->Top - dest->Bottom + mbmpRect.Bottom != mbmpRect.Top)
                    {
                        // 0042B451
                        PInvoke.Call(LibraryNames.GDI32, "SetTextColor", DC, new IntPtr(0x2FFFFFF));
                        PInvoke.Call(LibraryNames.GDI32, "SetBkColor", DC, new IntPtr(0x2000000));
                        mbmpRect.TopLeftOrigin();
                        if (FromPointer<GPT>(UnmanagedFunctionCall.StdCall((IntPtr)FunctionNames.AllocateGPT, new IntPtr(&mbmpRect), new IntPtr(1))) is GPT gptMaskMaybe)
                        {
                            mbmp.ThisCall((IntPtr)FunctionNames.MBMP_Method00425850, gptMaskMaybe.PixelBuffer, (IntPtr)gptMaskMaybe.Stride, new IntPtr(mbmpRect.Height), new IntPtr(-mbmpRect.Left), new IntPtr(-mbmpRect.Top), new IntPtr(&mbmpRect)); // call 0x00425850
                            FUN_0042a550(unkStruct->Clip);
                            PInvoke.Call(LibraryNames.GDI32, "StretchBlt", DC, new IntPtr(dest->Left), new IntPtr(dest->Top), new IntPtr(dest->Width), new IntPtr(dest->Height), gptMaskMaybe.DC, IntPtr.Zero, IntPtr.Zero, new IntPtr(mbmpRect.Width), new IntPtr(mbmpRect.Height), new IntPtr(0xEE0086));
                            gptMaskMaybe.DecreaseReferenceCounter();

                            if (FromPointer<GPT>(UnmanagedFunctionCall.StdCall((IntPtr)FunctionNames.AllocateGPT, new IntPtr(&mbmpRect), new IntPtr(8))) is GPT gptColor)
                            {
                                RECTANGLE fill = default;
                                mbmpRect.ToGDI(&fill);

                                PInvoke.Call(LibraryNames.USER32, "FillRect", gptColor.DC, new IntPtr(&fill), PInvoke.Call(LibraryNames.GDI32, "GetStockObject", IntPtr.Zero));
                                GDIHelper.FlushGdi();

                                mbmp.Blit(gptColor.PixelBuffer, gptColor.Stride, mbmpRect.Height, -mbmpRect.Left, -mbmpRect.Top, &mbmpRect, null);

                                PInvoke.Call(LibraryNames.GDI32, "StretchBlt", DC, new IntPtr(dest->Left), new IntPtr(dest->Top), new IntPtr(dest->Width), new IntPtr(dest->Height), gptColor.DC, IntPtr.Zero, IntPtr.Zero, new IntPtr(mbmpRect.Width), new IntPtr(mbmpRect.Height), new IntPtr(0x8800C6));
                                gptColor.DecreaseReferenceCounter();
                            }
                        }
                    }
                    else
                    {
                        // 0042B3F0
                        if (clip.CalculateIntersection((RECTANGLE*)Bounds.AsPointer()))
                        {
                            if (FlushCounter >= APP.GlobalFlushCounter)
                                GDIHelper.FlushGdi();
                            int offsetX = dest->Left - mbmpRect.Left;
                            int offsetY = dest->Top - mbmpRect.Top;
                            mbmp.Blit(PixelBuffer, Stride, Bounds.Height, offsetX, offsetY, &clip, Region);
                        }
                    }
                }
                else if (BitDepth == 32)
                {
                    if (clip.CalculateIntersection((RECTANGLE*)Bounds.AsPointer()))
                    {
                        if (FlushCounter >= APP.GlobalFlushCounter)
                            GDIHelper.FlushGdi();
                        // TODO: Implement 32 bit GPT
                    }
                }
            }
        }

        [HookFunction(FunctionNames.GPT_FUN_0042a550, CallingConvention = CallingConvention.ThisCall)]
        public void FUN_0042a550(RECTANGLE* clipRect)
        {
            RECTANGLE clip;
            RECTANGLE local_20;

            if (clipRect == null)
            {
                clip = new RECTANGLE(-0x3fffffff, -0x3fffffff, 0x3fffffff, 0x3fffffff);
            }
            else
            {
                clip = *clipRect;
            }
            if ((UnkFlags & 2) != 0)
            {
                var bounds = Bounds;
                clip.CalculateIntersection(&bounds);
            }
            if ((UnkFlags & 1) == 0)
            {
                var current = ClipRect;
                if (!clip.OneValidAndBothNotSame(&current)) goto Finally;
                if ((UnkFlags & 1) == 0 && ClipRect.Left == -0x3fffffff && ClipRect.Top == -0x3fffffff && ClipRect.Right == 0x3fffffff && ClipRect.Bottom == 0x3fffffff)
                    goto DefaultClip;
            }

            IntPtr hrgn;
            if (Region.Value == null)
            {
                hrgn = IntPtr.Zero;
                goto SelectHRGN;
            }
            var iVar2 = Region.Value.ThisCall(new IntPtr(0x00426380), new IntPtr(&local_20)).ToInt32();
            if (iVar2 == 0)
            {
                hrgn = Region.Value.ThisCall(new IntPtr(0x00427d10));
                if (hrgn != IntPtr.Zero)
                    goto SelectHRGN;
            }
            local_20.CalculateIntersection(&clip);
            PInvoke.Call(LibraryNames.GDI32, "SelectClipRgn", DC, IntPtr.Zero);
            PInvoke.Call(LibraryNames.GDI32, "IntersectClipRect", DC, new IntPtr(local_20.Left), new IntPtr(local_20.Top), new IntPtr(local_20.Right), new IntPtr(local_20.Bottom));
            goto From_DoClip;

        SelectHRGN:
            PInvoke.Call(LibraryNames.GDI32, "SelectClipRgn", DC, hrgn);
            goto DefaultClip;

        DefaultClip:
            if ((clip.Left != -0x3fffffff) || (clip.Top != -0x3fffffff) ||
                (clip.Right != 0x3fffffff) || (clip.Bottom != 0x3fffffff))
            {
                PInvoke.Call(LibraryNames.GDI32, "IntersectClipRect", DC, new IntPtr(clip.Left), new IntPtr(clip.Top), new IntPtr(clip.Right), new IntPtr(clip.Bottom));
            }
            goto From_DefaultClip;

        From_DefaultClip:
        From_DoClip:
            UnkFlags &= 0xfffffffe;
            ClipRect = clip;

        Finally:
            this.ThisCall(new IntPtr(0x0042a700));
            return;
        }

        [HookFunction(FunctionNames.GPT_SwapRegion, CallingConvention = CallingConvention.ThisCall)]
        internal void SwapRegion(void** swap)
        {
            if ((OffsetX != 0) || (OffsetY != 0))
            {
                if (*swap != null)
                {
                    FromPointer<REGN>(new IntPtr(*swap)).Offset(-OffsetX, -OffsetY);
                }
                if (Region.Value != null)
                {
                    Region.Value.Offset(OffsetX, OffsetY);
                }
            }
            Helpers.MemSwap((void*)Region.AsPointer(), swap, 4);
            UnkFlags = UnkFlags | 1;
            return;
        }

        [HookFunction(FunctionNames.GPT_SetOffset, CallingConvention = CallingConvention.ThisCall)]
        public void SetOffset(POINT* offset)
        {
            OffsetX = offset->X;
            OffsetY = offset->Y;
        }


        [HookFunction(FunctionNames.GPT_GetOffset, CallingConvention = CallingConvention.ThisCall)]
        public void GetOffset(POINT* dest)
        {
            dest->X = OffsetX;
            dest->Y = OffsetY;
        }

        [HookFunction(FunctionNames.GPT_FromWindow, CallingConvention = CallingConvention.StdCall)]
        public static GPT FromWindow(IntPtr hwnd)
        {
            IntPtr dc;
            GPT result = null;

            if (hwnd != default)
            {
                dc = PInvoke.Call(LibraryNames.USER32, "GetDC", hwnd);
                if (dc != default)
                {
                    result = FromHDC(dc);
                    if (result == null)
                    {
                        PInvoke.Call(LibraryNames.USER32, "ReleaseDC", hwnd, dc);
                    }
                    else
                    {
                        result.WindowHandle = hwnd;
                    }
                }
            }
            else
            {
                PTR_ERS_004e39a8.Value.VirtualCall(0x14, new IntPtr(10000)); // VirtualFunc1
            }
            return result;
        }

        [HookFunction(FunctionNames.GPT_FromHDC, CallingConvention = CallingConvention.StdCall)]
        public static GPT FromHDC(IntPtr dc)
        {
            GPT @this = null;

            if (dc != default)
            {
                @this = new GPT();
                if (!@this.SetDC(dc))
                {
                    @this.DecreaseReferenceCounter();
                    @this = null;
                }
            }
            return @this;
        }

        [HookFunction(FunctionNames.GPT_SetDC, CallingConvention = CallingConvention.ThisCall)]
        public bool SetDC(IntPtr dc)
        {
            IntPtr hdc;
            IntPtr h;

            DC = dc;
            h = PInvoke.Call(LibraryNames.GDI32, "GetStockObject", new IntPtr(8));
            h = PInvoke.Call(LibraryNames.GDI32, "SelectObject", DC, h);
            if (h != default)
            {
                PInvoke.Call(LibraryNames.GDI32, "DeleteObject", h);
            }
            h = PInvoke.Call(LibraryNames.GDI32, "GetStockObject", IntPtr.Zero);
            h = PInvoke.Call(LibraryNames.GDI32, "SelectObject", DC, h);
            if (h != default)
            {
                PInvoke.Call(LibraryNames.GDI32, "DeleteObject", h);
            }
            h = PInvoke.Call(LibraryNames.GDI32, "GetStockObject", new IntPtr(0xd));
            h = PInvoke.Call(LibraryNames.GDI32, "SelectObject", DC, h);
            if (h != default)
            {
                PInvoke.Call(LibraryNames.GDI32, "DeleteObject", h);
            }
            Field_0x80 = 3;
            Field_0x84 = 4;
            hdc = DC;
            ClipRect = new RECTANGLE(-0x3fffffff, -0x3fffffff, 0x3fffffff, 0x3fffffff);
            PInvoke.Call(LibraryNames.GDI32, "SelectClipRgn", hdc, IntPtr.Zero);
            PInvoke.Call(LibraryNames.GDI32, "SetPolyFillMode", DC, new IntPtr(2));
            return true;
        }

        [HookFunction(FunctionNames.GPT_FUN_00429e60, CallingConvention = CallingConvention.ThisCall)]
        public uint FUN_00429e60(uint param_1)
        {
            uint uVar1;
            int iVar2;

            if (((UnkFlags & 4) == 0) && APP.GlobalIsPalettedDevice)
            {
                uVar1 = FUN_00429600(&param_1);
                return uVar1;
            }
            uVar1 = FUN_00429600(&param_1);
            if (((sbyte)(uVar1 >> 0x18) == 1) && APP.GlobalPalette.ToPointer() != null)
            {
                iVar2 = (short)uVar1;
                if (-1 < iVar2 && iVar2 < APP.GlobalColorEntries)
                {
                    uVar1 = *(uint*)(APP.GlobalPalette.ToPointer() + iVar2);
                    uVar1 = (uVar1 & 0xff) << 0x10 | (uVar1 >> 8 & 0xff) << 8 | uVar1 >> 0x10 & 0xff;
                }
            }
            return uVar1;
        }

        [HookFunction(FunctionNames.GPT_FUN_00429600, CallingConvention = CallingConvention.StdCall)]
        public static uint FUN_00429600(uint* param_1)
        {
            uint uVar1;

            uVar1 = *param_1;
            if ((sbyte)(uVar1 >> 0x18) != -2)
            {
                return (uint)((uVar1 & 0xff | 0x200) << 0x10 | (uVar1 >> 8 & 0xff) << 8 | (int)uVar1 >> 0x10 & 0xffU);
            }
            return uVar1 & 0xff | 0x1000000;
        }

        [HookFunction(FunctionNames.GPT_FUN_0042a2d0, CallingConvention = CallingConvention.ThisCall)]
        public void* FUN_0042a2d0(RECTANGLE* dest)
        {
            if (DIBPointer == IntPtr.Zero)
            {
                return null;
            }
            if (APP.GlobalFlushCounter <= FlushCounter)
            {
                GDIHelper.FlushGdi();
            }
            if (dest != null)
            {
                *dest = Bounds;
            }
            FUN_0042a380();
            return PixelBuffer.ToPointer();
        }


        [HookFunction(FunctionNames.GPT_FUN_0042a380, CallingConvention = CallingConvention.ThisCall)]
        public void FUN_0042a380()
        {
            Field_0x54++;
        }

        [HookFunction(FunctionNames.GPT_GetStride, CallingConvention = CallingConvention.ThisCall)]
        public int GetStride()
        {
            return DIBPointer == IntPtr.Zero ? 0 : Stride;
        }

        [HookFunction(FunctionNames.GPT_GetBitDepth, CallingConvention = CallingConvention.ThisCall)]
        public int GetBitDepth()

        {
            IntPtr hdc;
            uint rasterCaps;
            int bitDepth;
            int planes;

            if (DIBPointer == IntPtr.Zero && BitDepth == 0)
            {
                rasterCaps = (uint)PInvoke.Call(LibraryNames.GDI32, "GetDeviceCaps", DC, new IntPtr(0x26));
                if ((rasterCaps & 0x100) != 0)
                {
                    hdc = DC;
                    bitDepth = (int)PInvoke.Call(LibraryNames.GDI32, "GetDeviceCaps", hdc, new IntPtr(0xc));
                    planes = (int)PInvoke.Call(LibraryNames.GDI32, "GetDeviceCaps", hdc, new IntPtr(0xe));
                    BitDepth = bitDepth * planes;
                }
            }
            return BitDepth;
        }

        // Used for text highlighting
        [HookFunction(FunctionNames.GPT_FUN_0042a3b0, CallingConvention = CallingConvention.ThisCall)]
        public void FUN_0042a3b0(RECTANGLE* param_1, GNV_UnkStruct1* param_2)
        {
            FUN_0042a550(param_2->Clip);
            PInvoke.Call(LibraryNames.USER32, "InvertRect", DC, new IntPtr(param_1));
            FlushCounter = APP.GlobalFlushCounter;
        }


        [HookFunction(FunctionNames.GPT_DrawRectangle, CallingConvention = CallingConvention.ThisCall)]
        public void DrawRectangle(RECTANGLE* param_1)

        {
            PInvoke.Call(LibraryNames.GDI32, "Rectangle", DC, new IntPtr(param_1->Left), new IntPtr(param_1->Top), new IntPtr(param_1->Right + 1), new IntPtr(param_1->Bottom + 1));
            FlushCounter = APP.GlobalFlushCounter;
        }


        [HookFunction(FunctionNames.GPT_FUN_0042a700, CallingConvention = CallingConvention.ThisCall)]
        public void FUN_0042a700()

        {
            IntPtr hPalette;
            uint cEntries;
            int bitDepth;

            if ((PaletteVersion != APP.GlobalPaletteVersion) && (APP.GlobalPalette.ToPointer() != null))
            {
                if (PixelBuffer == IntPtr.Zero)
                {
                    if (((UnkFlags & 4) == 0) && (APP.Global004e3c98 != IntPtr.Zero))
                    {
                        hPalette = PInvoke.Call(LibraryNames.GDI32, "SelectPalette", DC, APP.Global004e3c98, IntPtr.Zero);
                        if (hPalette == IntPtr.Zero)
                        {
                            UnkFlags |= 4;
                            PaletteVersion = APP.GlobalPaletteVersion;
                        }
                        PInvoke.Call(LibraryNames.GDI32, "RealizePalette", DC);
                    }
                }
                else
                {
                    if ((UnkFlags & 8) == 0)
                    {
                        bitDepth = BitDepth;
                        if (bitDepth > 1 && bitDepth < 9)
                        {
                            cEntries = 1u << ((byte)bitDepth & 0x1f);
                            if (APP.GlobalColorEntries < cEntries)
                            {
                                cEntries = APP.GlobalColorEntries;
                            }
                            PInvoke.Call(LibraryNames.GDI32, "SetDIBColorTable", DC, IntPtr.Zero, new IntPtr(cEntries), new IntPtr(APP.GlobalPalette.ToPointer()));
                        }
                    }
                }
                PaletteVersion = APP.GlobalPaletteVersion;
            }
        }
    }
}
