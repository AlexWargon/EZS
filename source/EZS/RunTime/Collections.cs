
using Unity.Collections.LowLevel.Unsafe;

namespace Wargon.ezs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Runtime.InteropServices;

    public interface IPool<T> : IPool where T : struct {
        void Set(T component, int id);
        ref T Get(int id);
    }
    public static class PoolExt{
        public static NativePool<T> AsNative<T>(this Pool<T> pool) where T : unmanaged {
            return new NativePool<T>(pool);
        }
    }

    public unsafe readonly struct NativePool<T> where T : unmanaged {
        [NativeDisableUnsafePtrRestriction] private readonly T* buffer;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NativePool(Pool<T> pool) {
            fixed (T* ptr = pool.items) buffer = ptr;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index) {
            return ref UnsafeUtility.ArrayElementAsRef<T>(buffer, index);
        }
    }
    public class Pool<T> : IPool<T> where T : struct
    {
        public T[] items;
        private int length;
        private readonly bool typeIsStruct;
        public event Action<int> OnRemove;
        public event Action<int> OnAdd;
        public event Action<Pool<T>> OnResize;
        private bool isTag;
        public Pool(int size) {
            isTag = ComponentType<T>.IsTag;
            items = new T[size];
            ItemType = typeof(T);
            TypeID = ComponentType<T>.ID;
            typeIsStruct = ComponentType<T>.IsStruct;
            length = size;
        }

        public int TypeID { [MethodImpl(MethodImplOptions.AggressiveInlining)]get; }
        public int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)]get; [MethodImpl(MethodImplOptions.AggressiveInlining)]set; }
        public Type ItemType { [MethodImpl(MethodImplOptions.AggressiveInlining)]get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int entity)
        {
            Count--;
            items[entity] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int entity)
        {
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Default(int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
                OnResize?.Invoke(this);
            }
            items[id] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int id) {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
                OnResize?.Invoke(this);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IPool.Get(int id)
        {
            return items[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IPool.SetBoxed(object component, int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
                OnResize?.Invoke(this);
            }
            items[id] = (T)component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(T component, int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
                OnResize?.Invoke(this);
            }
            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(in T component, int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
                OnResize?.Invoke(this);
            }
            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int id)
        {
            return ref items[id];
        }

        public override string ToString()
        {
            return $"{nameof(items)}: {items}";
        }

        int IPool.GetSize() => length;

        public unsafe void AddPtr(void* component, int id) {
            var comp = Marshal.PtrToStructure<T>((IntPtr)component);
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
                OnResize?.Invoke(this);
            }
            items[id] = comp;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Copy(int @from, int to) {
            Set(in items[@from], to);
            Count++;
        }
    }


    public interface IPool {
        event Action<int> OnAdd;
        event Action<int> OnRemove;
        int TypeID { get; }
        int Count { get; set; }
        Type ItemType { get; }

        /// <summary>
        ///     <para>Set component with entity ID to default value </para>
        /// </summary>
        void Default(int id);
        object Get(int id);
        void SetBoxed(object component, int id);
        void Set(int id);
        void Add(int entity);
        void Remove(int entity);
        int GetSize();
#if UNITY_2019_1_OR_NEWER
        unsafe void AddPtr(void* component, int id);
#endif
        void Copy(int from, int to);
    }

    public unsafe struct UnsafeSpan<T> where T: unmanaged {
        private T* buffer;
        public int len;
        public int Count => len;
        public ref T this[int index] => ref UnsafeUtility.AsRef<T>(buffer);
        public UnsafeSpan(T* data, int lenght) {
            buffer = data;
            len = lenght;
        }
    }

    public static class DynaucArrayEx {
        public unsafe static UnsafeSpan<T> AsSpan<T>(this DynamicArray<T> array) where T : unmanaged {
            fixed (T* ptr = array.Items) {
                return new UnsafeSpan<T>(ptr, array.Count);
            }
        }
    }
    public class DynamicArray<T>
    {
        public int Count;
        public T[] Items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DynamicArray(int capacity)
        {
            Items = new T[capacity];
            Count = 0;
        }
        public DynamicArray() {
            Items = Array.Empty<T>();
            Count = 0;
        }
        public static DynamicArray<T> Empty() {
            return new DynamicArray<T>();
        }
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Items[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (Items.Length == Count) Array.Resize(ref Items, Items.Length << 1);
            Items[Count] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetLast()
        {
            return ref Items[Count];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetLastWithIncrement()
        {
            if (Items.Length == Count) Array.Resize(ref Items, Items.Length << 1);
            return ref Items[Count++];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetByIndex(int index)
        {
            return ref Items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(int entityID)
        {
            return Items[entityID];
        }

        internal void Remove(int index)
        {
            Count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(Items, 0, Items.Length);
            Count = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearCount() {
            Count = 0;
        }
    }
    public interface ISharedComponent { }
    public readonly struct ComponentType<T>
    {
        public readonly static int ID;
        public readonly static bool IsStruct;
        public readonly static bool IsUnmanaged;
        public readonly static bool IsTag;
        static ComponentType()
        {
            var Value = typeof(T);
            IsTag = Value.GetFields().Length < 1;
            ComponentType.Add(Value);
            ID = ComponentType.GetID(Value);
            IsUnmanaged = Value.IsUnmanaged();
            IsStruct = Value.IsValueType;
        }
    }
    public static class ComponentTypes
    {
        internal static int Count;
        
    }
    
    public static class ComponentType
    {
        private static Dictionary<Type, int> TypeID;
        private static Dictionary<int, Type> TypeValue;
        private static bool inited;

        static ComponentType() {
            TypeID = new Dictionary<Type, int>();
            TypeValue = new Dictionary<int, Type>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(Type type, int id)
        {
            if (TypeID.ContainsKey(type)) return;
            TypeID.Add(type, id);
            TypeValue.Add(id, type);
        }

        public static void Add(Type type) {
            if (TypeID.ContainsKey(type)) return;
            var id = Interlocked.Increment(ref ComponentTypes.Count);
            TypeID.Add(type, id);
            TypeValue.Add(id, type);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(Type type)
        {
            return TypeID.ContainsKey(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetID(Type type)
        {
            if (!TypeID.ContainsKey(type))
            {
                var newID = Interlocked.Increment(ref ComponentTypes.Count);
                TypeID.Add(type, newID);
                TypeValue.Add(newID, type);
            }

            return TypeID[type];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetTypeValue(int id)
        {
            return TypeValue[id];
        }
    }
    
    internal struct TypePair<T1, T2>
    { 
        internal static int Id;
        static TypePair()
        {
            Id = TypePairCount<T1>.Value++;
        }
    }

    internal struct TypePairCount<T1>
    {
        internal static int Value;
    }
    
    public static class CollectionHelp
    {
        class UnamagedGeneric<T> where T : unmanaged { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnmanaged(this Type t)
        {
            try { typeof(UnamagedGeneric<>).MakeGenericType(t); return true; }
            catch (Exception){ return false; }
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEntityTypes(ref EntityType[] array, ref bool[] actives, int id)
        {
            if (array.Length == id - 1)
            {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateArray<T>(ref T[] array, int id)
        {
            if (array.Length == id - 1)
            {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEntities(ref Entities[] array, ref bool[] actives, int id)
        {
            if (array.Length == id - 1)
            {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }
    }
    
    public class IntMap {
        private int[] items;
        public int Capacity;

        public int this[int key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items[key] - 1;
        }
        public IntMap(int capacity) {
            items = new int[capacity];
            Capacity = capacity;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Resize(int newSize) {
            Array.Resize(ref items, newSize);
            Capacity = newSize;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Assert(int key) {
            if (Capacity <= key) {
                Resize(key + 16);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int key, int value) {
            Assert(key);
            items[key] = value + 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Get(int key) {
            return items[key] - 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int key) {
            Assert(key);
            items[key] = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int key) {
            return Capacity > key && items[key] != 0;
        }
    }
}