using Open3dmm.BRender;
using System;

namespace Open3dmm.Classes
{
    [NativeClassSize(0x198)]
    [Vtable(0x4e2278)]
    public class BWLD : BASE
    {
        [NativeFieldOffset(0x8)]
        public extern ref RECTANGLE Rectangle1 { get; }
        [NativeFieldOffset(0x18)]
        public extern ref RECTANGLE Rectangle2 { get; }

        [NativeFieldOffset(0x30)]
        public extern ref IntPtr RenderHandlers { get; }

        [NativeFieldOffset(0x003C)]
        public extern ref Ref<BWLD> Field003C { get; }

        [NativeFieldOffset(0x0100)]
        public extern ref Ref<GPT> Bitmap1 { get; }

        [NativeFieldOffset(0x0104)]
        public extern ref Ref<GPT> Bitmap2 { get; }

        [NativeFieldOffset(0x10C)]
        public extern ref int Color { get; }

        [NativeFieldOffset(0x0130)]
        public extern ref Ref<ZBMP> Field0130 { get; }

        [NativeFieldOffset(0x0134)]
        public extern ref Ref<ZBMP> Field0134 { get; }

        [NativeFieldOffset(0x138)]
        public extern ref int Depth { get; }

        [NativeFieldOffset(0x015C)]
        public extern ref Ref<REGN> Field015C { get; }

        [NativeFieldOffset(0x0160)]
        public extern ref Ref<REGN> Field0160 { get; }

        [NativeFieldOffset(0x16C)]
        public extern ref bool DirtyFlag { get; }
        [NativeFieldOffset(0x170)]
        public extern ref bool SkipHandlers { get; }

        [NativeFieldOffset(0x0184)]
        public extern ref Ref<CRF> Field0184 { get; }

        [NativeFieldOffset(0x28)]
        public extern ref BrActor World { get; }
        [NativeFieldOffset(0x84)]
        public extern ref BrActor Camera { get; }
        [NativeFieldOffset(0x84)]
        public extern ref BrPixelMap PixelMap1 { get; }
        [NativeFieldOffset(0x50)]
        public extern ref BrMatrix34 Matrix { get; }

        public BWLD(int width, int height, IntPtr param_3, IntPtr param_4)
        {
            Vtable = new VTABLE(0x4e2278);
            if (!FUN_00473a70(width, height, param_3, param_4))
                DecreaseReferenceCounter();
        }

#if NATIVEDEP
        [HookFunction(0x00473a10, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        private static BWLD NativeConstructor(int width, int height, IntPtr param_3, IntPtr param_4)
        {
            return new BWLD(width, height, param_3, param_4);
        }
#endif

        public bool FUN_00473a70(int width, int height, IntPtr param_3, IntPtr param_4)
        {
            return UnmanagedFunctionCall.ThisCall(new IntPtr(0x00473a70), NativeHandle.Address, new IntPtr(width), new IntPtr(height), param_3, param_4) != IntPtr.Zero;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    renderer = new BrWorldRenderer(this);
        //    NativeAbstraction.GameTimer.Draw += OnRender;
        //}

        //private void OnRender(GameTime gameTime)
        //{
        //    if (NativeHandle.IsDisposed)
        //        NativeAbstraction.GameTimer.Draw -= OnRender;
        //    else
        //        renderer.Render();
        //}
    }
}
