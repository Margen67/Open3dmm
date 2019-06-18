namespace Open3dmm.Classes
{
    public unsafe class APPB : CMH
    {
        [NativeFieldOffset(0x00C)]
        public extern ref int Flags00C { get; }

        [NativeFieldOffset(0x05C)]
        public extern ref int RenderMode { get; }
        [NativeFieldOffset(0x01C)]
        public extern ref Ref<GPT> GPT { get; }

        [NativeFieldOffset(0x08C)]
        public extern ref Ref<STIO> STIO { get; }
        [NativeFieldOffset(0x01BC)]
        public extern ref Ref<KWA> KWA { get; }

        [NativeFieldOffset(0x68)]
        public extern ref Ref<GL> ListSometimes { get; }
    }
}
