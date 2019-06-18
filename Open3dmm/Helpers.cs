using Open3dmm.BRender;
using System;
using System.Runtime.InteropServices;

namespace Open3dmm
{
    public static unsafe class Helpers
    {
        [HookFunction(0x004198c0, CallingConvention = CallingConvention.StdCall)]
        public static void ScaleCalculation(int value, BrScalar scale, int* hiResult, int* loResult)
        {
            ScaleCalculation(value, scale, out var result);
            *hiResult = (int)(result >> 0x20);
            *loResult = (int)result;
        }

        public static void ScaleCalculation(int value, BrScalar scale, out long result)
        {
            result = (long)value * scale.ToFixed();
        }

        [HookFunction(0x00462080, CallingConvention = CallingConvention.StdCall)]
        public static int timeGetTime()
        {
            return Environment.TickCount;
        }

        [HookFunction(FunctionNames.MemSwap, CallingConvention = CallingConvention.StdCall)]
        public static void MemSwap(void* a, void* b, int length)
        {
            byte current;
            var spanA = new Span<byte>(a, length);
            var spanB = new Span<byte>(b, length);
            while (--length >= 0)
            {
                current = spanA[length];
                spanA[length] = spanB[length];
                spanB[length] = current;
            }
        }
    }
}
