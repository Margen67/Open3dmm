using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open3dmm
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class NativeClassSizeAttribute : Attribute
    {
        public NativeClassSizeAttribute(int size)
        {
            Size = size;
        }

        public int Size { get; }
    }
}
