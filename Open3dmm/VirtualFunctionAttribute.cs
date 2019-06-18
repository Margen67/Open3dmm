using System;

namespace Open3dmm
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    sealed class VirtualFunctionAttribute : Attribute
    {
        readonly int vtableIndex;
        public VirtualFunctionAttribute(int vtableIndex)
        {
            this.vtableIndex = vtableIndex;
        }

        public int VtableIndex => this.vtableIndex;
    }
}
