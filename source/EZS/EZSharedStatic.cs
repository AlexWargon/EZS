using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public unsafe struct EZSharedStatic<T> where T : unmanaged
    {
        public void* _buffer;
        public ref T Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ref *(T*) _buffer; }
        }
        public void* UnsafeDataPointer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _buffer; }
        }
        public static EZSharedStatic<T> GetOrCreate()
        {
            var newData = new EZSharedStatic<T>();
            newData._buffer = (void*) Marshal.AllocCoTaskMem(sizeof(T));
            return newData;
        }
    }

    public unsafe readonly struct EZSharedStaticInt
    {
        private readonly int* _buffer;
        public ref int Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ref *_buffer; }
        }
        public EZSharedStaticInt(int* buffer)
        {
            _buffer = buffer;
        }
        
    }
    public static class EZSharedStatic
    {
        class U<T> where T : unmanaged { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnmanaged(this Type t)
        {
            try { typeof(U<>).MakeGenericType(t); return true; }
            catch (Exception){ return false; }
        }
        
        public static unsafe EZSharedStatic<T> GetOrCreate<T>() where T : unmanaged
        {
            var newData = new EZSharedStatic<T>();
            newData._buffer = (void*) Marshal.AllocCoTaskMem(sizeof(T));
            return newData;
        }
        public static unsafe EZSharedStaticInt GetOrCreate()
        {
            int p = new int();
            int* ptr = &p;
            return new EZSharedStaticInt(ptr);
        }
    }
}

