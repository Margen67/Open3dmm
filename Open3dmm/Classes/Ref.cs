using System;

namespace Open3dmm.Classes
{
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public unsafe struct Ref<T> where T : NativeObject
    {
        private string DebuggerDisplay => Value.ToString();

#if NATIVEDEP
        private IntPtr address;

        public T Value {
            get => NativeObject.FromPointer<T>(address);
            set => address = value?.NativeHandle.Address ?? IntPtr.Zero;
        }

        public Ref(T obj)
        {
            this.address = obj.NativeHandle.Address;
        }

        public Ref(IntPtr address)
        {
            this.address = address;
        }
#else
        public T Value { get; set; }

        public Ref(T obj)
        {
            Value = obj;
        }

        public Ref(IntPtr address)
        {
            throw new NotSupportedException();
        }
#endif
        public static implicit operator Ref<T>(T obj)
        {
            return new Ref<T>(obj);
        }

        public static implicit operator T(Ref<T> reference)
        {
            return reference.Value;
        }
    }
}
