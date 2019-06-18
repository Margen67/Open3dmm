using System;
using System.Runtime.InteropServices;

namespace Open3dmm.Classes
{
#if NATIVEDEP
    // Only inherit from NativeObject when we have native dependency enabled.
    public partial class BASE : NativeObject { }
#endif

    [Vtable(0x4dd200)]
    [NativeClassSize(0x8)]
    public unsafe partial class BASE
    {
        private static readonly int classID = ClassID.ValueFromString("BASE");

        [NativeFieldOffset(0x0)]
        public extern ref VTABLE Vtable { get; }
        [NativeFieldOffset(0x4)]
        public extern ref int NumReferences { get; }

        public static Pointer<T> GetGlobal<T>(int address) where T : unmanaged
        {
            return new Pointer<T>(new IntPtr(address));
        }

        [HookFunction(FunctionNames.BASE_Init, CallingConvention = CallingConvention.ThisCall)]
        public BASE()
        {
            Vtable = new VTABLE(0x4dd200);
            NumReferences = 1;
        }

        [HookFunction(FunctionNames.BASE_DecreaseReferenceCounter, CallingConvention = CallingConvention.ThisCall)]
        [VirtualFunction(4)]
        public virtual void DecreaseReferenceCounter()
        {
            if (--NumReferences < 1 && this != null)
            {
                Free(1);
            }
        }

        [HookFunction(FunctionNames.BASE_Free, CallingConvention = CallingConvention.ThisCall)]
        [VirtualFunction(2)]
        public virtual void Free(byte free)
        {
            Vtable = new VTABLE(0x4dd200);
#if NATIVEDEP
            if ((free & 1) != 0)
                Program.Free(NativeHandle.Address);
#endif
        }

        [HookFunction(FunctionNames.BASE_GetClassID, CallingConvention = CallingConvention.ThisCall)]
        [VirtualFunction(1)]
        public virtual int GetClassID()
        {
            return classID;
        }

        [HookFunction(FunctionNames.BASE_IncreaseReferenceCounter, CallingConvention = CallingConvention.ThisCall)]
        [VirtualFunction(3)]
        public virtual void IncreaseReferenceCounter()
        {
            NumReferences++;
        }

        [HookFunction(FunctionNames.BASE_IsDerivedFrom, CallingConvention = CallingConvention.ThisCall)]
        [HookFunction(FunctionNames.BASE__IsDerivedFrom, CallingConvention = CallingConvention.ThisCall)]
        [VirtualFunction(0)]
        public virtual bool IsDerivedFrom(int classID)
        {
            return BASE.classID == classID;
        }
    }
}
