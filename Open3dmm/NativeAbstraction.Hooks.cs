using Open3dmm.Classes;
using System;

namespace Open3dmm
{
    partial class NativeAbstraction
    {
        static unsafe partial void SetNativeHooks()
        {
            NativeWeaver.Init();

            //Hook.Create<ThisCall0>(AddressOfFunction(FunctionNames.BWLD_Render), ctx =>
            //{
            //    return (ecx) =>
            //    {
            //        var bwld = NativeObject.FromPointer<BWLD>(ecx);
            //        ctx.CallOriginal(o => o(ecx));
            //        return new IntPtr(1);
            //    };
            //}).Initialize();
        }
    }
}