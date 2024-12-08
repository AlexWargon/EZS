using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Wargon.ezs {
    public interface IPool<T> : IPool where T : struct {
        void Set(T component, int id);
        ref T Get(int id);
    }


    public unsafe readonly struct NativePool<T> where T : unmanaged {
        [NativeDisableUnsafePtrRestriction] private readonly T* buffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NativePool(Pool<T> pool) {
            fixed (T* ptr = pool.items) {
                buffer = ptr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index) {
            return ref UnsafeUtility.ArrayElementAsRef<T>(buffer, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Read(int index) {
            return UnsafeUtility.ReadArrayElement<T>(buffer, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* UnsafeGet(int index) {
            return (T*)UnsafeUtilityExtensions.AddressOf(in UnsafeUtility.ArrayElementAsRef<T>(buffer, index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, in T component) {
            UnsafeUtility.WriteArrayElement(buffer, index, component);
        }
    }

    public class Pool<T> : IPool<T> where T : struct {
        public T[] items;
        private int capacity;
        private readonly bool typeIsStruct;
        public event Action<Pool<T>> OnResize;
        private bool isTag;
        private readonly bool isOnAddToEntity;
        private readonly bool isDiposable;
        private readonly bool isCopyable;
        public event Action<int> OnRemove;
        public event Action<int> OnAdd;

        private delegate void OnAddToEntity(ref T componet);

        private delegate void DisposeComponent(ref T component);

        private delegate T CopyComponent(ref T component);
        private event OnAddToEntity OnAddToEntityFunc;
        private event DisposeComponent DisposeFunc;
        private event CopyComponent CopyFunc;
#if ENABLE_IL2CPP && !UNITY_EDITOR
        T _fakeInstance;
#endif
        public Pool(int size) {
            isTag = ComponentType<T>.IsTag;
            items = new T[size];
            ItemType = typeof(T);
            TypeID = ComponentType<T>.ID;
            typeIsStruct = ComponentType<T>.IsStruct;
            capacity = size;
            isOnAddToEntity = ComponentType<T>.IsOnAddToEntity;
            isDiposable = ComponentType<T>.IsDisposable;
            isCopyable = ComponentType<T>.IsCopyable;
            
            if (isOnAddToEntity) {
                var onAddToEntity = typeof(T).GetMethod(nameof(IOnAddToEntity<T>.OnAdd));
                OnAddToEntityFunc = (OnAddToEntity)Delegate.CreateDelegate(
                    typeof(OnAddToEntity),
#if ENABLE_IL2CPP && !UNITY_EDITOR
                    _fakeInstance,
#else
                    null,
#endif
                    onAddToEntity);
            }

            if (isDiposable) {
                var dispose = typeof(T).GetMethod(nameof(IDisposable<T>.Dispose));
                DisposeFunc = (DisposeComponent)Delegate.CreateDelegate(
                    typeof(DisposeComponent),
#if ENABLE_IL2CPP && !UNITY_EDITOR
                    _fakeInstance,
#else
                    null,
#endif
                    dispose);
            }
            
            if (isCopyable) {
                var copy = typeof(T).GetMethod(nameof(ICopyable<T>.Copy));
                CopyFunc = (CopyComponent)Delegate.CreateDelegate(
                    typeof(CopyComponent),
#if ENABLE_IL2CPP && !UNITY_EDITOR
                    _fakeInstance,
#else
                    null,
#endif
                    copy);
            }
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        public Type ItemType {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public int TypeID {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int entity) {
            Count--;
            ref var c = ref items[entity];
            if (isDiposable) DisposeFunc.Invoke(ref c);
            c = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int entity) {
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Default(int id) {
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }

            items[id] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int id) {
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }

            if (isOnAddToEntity) OnAddToEntityFunc.Invoke(ref items[id]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IPool.GetBoxed(int id) {
            return items[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IPool.SetBoxed(object component, int id) {
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }

            items[id] = (T)component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(T component, int id) {
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }

            items[id] = component;
            if (isOnAddToEntity) OnAddToEntityFunc.Invoke(ref items[id]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(in T component, int id) {
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }

            items[id] = component;
            if (isOnAddToEntity) OnAddToEntityFunc.Invoke(ref items[id]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int id) {
            return ref items[id];
        }

        public override string ToString() {
            return $"{nameof(items)}: {items}";
        }

        int IPool.GetSize() {
            return capacity;
        }

        public unsafe void AddPtr(void* componentPtr, int id) {
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }
            UnsafeUtility.CopyPtrToStructure(componentPtr, out T comp);
            items[id] = comp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Copy(int from, int to) {
            var id = to;
            if (capacity - 1 < id) {
                Array.Resize(ref items, id + 16);
                capacity = items.Length;
                OnResize?.Invoke(this);
            }
            if (isCopyable) {
                var copy = CopyFunc(ref items[from]);
                Set(in copy, to);
            }
            else {
                Set(items[from], to);
            }
            Count++;
        }

        public void Dispose() {
            if (isDiposable)
                for (var i = 0; i < capacity; i++)
                    DisposeFunc.Invoke(ref items[i]);
        }
    }

    public static class PoolExt {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> AsNative<T>(this T[] array, Allocator allocator) where T : unmanaged {
            return new NativeArray<T>(array, allocator);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* AsPointer<T>(this T[] array) where T : unmanaged {
            fixed (T* ptr = array) {
                return ptr;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativePool<T> AsNative<T>(this Pool<T> pool) where T : unmanaged {
            return new NativePool<T>(pool);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> AsNativeArray<T>(this DynamicArray<T> array, Allocator allocator)
            where T : unmanaged {
            return new NativeArray<T>(array.Items, allocator);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSortedPool<T> AsNativeSortedPool<T>(this Pool<T> pool) where T : unmanaged, IEquatable<T> {
            NativeArray<T> array = new(pool.items, Allocator.TempJob);
            UnsafeList<T> write = new(pool.items.Length, Allocator.TempJob);
            SortPool<T> job = new() {
                array = array,
                write = write
            };
            job.Schedule(pool.items.Length, 1).Complete();
            array.Dispose();
            NativeSortedPool<T> sortedPool = new() {
                data = write,
                count = write.m_length
            };
            return sortedPool;
        }
    }

    [BurstCompile]
    internal struct SortPool<T> : IJobParallelFor where T : unmanaged, IEquatable<T> {
        internal NativeArray<T> array;
        internal UnsafeList<T> write;

        public void Execute(int index) {
            var item = array[index];
            if (!item.Equals(default)) write.Add(item);
        }
    }

    public unsafe struct NativeSortedPool<T> : IDisposable where T : unmanaged {
        internal UnsafeList<T> data;
        internal int count;

        public ref T Get(int e) {
            return ref UnsafeUtility.ArrayElementAsRef<T>(data.Ptr, e);
        }

        public void Dispose() {
            data.Dispose();
        }
    }

    public interface IPool {
        int TypeID { get; }
        int Count { get; }
        Type ItemType { get; }
        event Action<int> OnAdd;
        event Action<int> OnRemove;

        /// <summary>
        ///     <para>Set component with entity ID to default value </para>
        /// </summary>
        void Default(int id);

        object GetBoxed(int id);
        void SetBoxed(object component, int id);
        void Set(int id);
        void Add(int entity);
        void Remove(int entity);
        int GetSize();
#if UNITY_2019_1_OR_NEWER
        unsafe void AddPtr(void* componentPtr, int id);
#endif
        void Copy(int from, int to);
        void Dispose();
    }

    public unsafe struct UnsafeSpan<T> where T : unmanaged {
        private readonly T* buffer;
        public int len;
        public int Count => len;
        public ref T this[int index] => ref UnsafeUtility.AsRef<T>(buffer);

        public UnsafeSpan(T* data, int lenght) {
            buffer = data;
            len = lenght;
        }
    }

    public static class DynaucArrayEx {
        public static unsafe UnsafeSpan<T> AsSpan<T>(this DynamicArray<T> array) where T : unmanaged {
            fixed (T* ptr = array.Items) {
                return new UnsafeSpan<T>(ptr, array.Count);
            }
        }
    }

    public class DynamicArray<T> {
        public int Count;
        public T[] Items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DynamicArray(int capacity) {
            Items = new T[capacity];
            Count = 0;
        }

        public DynamicArray() {
            Items = Array.Empty<T>();
            Count = 0;
        }

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Items[index] = value;
        }

        public static DynamicArray<T> Empty() {
            return new DynamicArray<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item) {
            if (Items.Length == Count) Array.Resize(ref Items, Items.Length * 2);
            Items[Count] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetLast() {
            return ref Items[^1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetLastWithIncrement() {
            if (Items.Length == Count) Array.Resize(ref Items, Items.Length * 2);
            return ref Items[Count++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetLastWithDecrement() {
            if (Items.Length == Count) Array.Resize(ref Items, Items.Length * 2);
            return ref Items[Count--];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetByIndex(int index) {
            return ref Items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(int entityID) {
            return Items[entityID];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T ElementAt(int index) {
            return ref Items[index];
        }

        internal void Remove(int index) {
            Count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            Array.Clear(Items, 0, Items.Length);
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearCount() {
            Count = 0;
        }

        public void RemoveSwapBack(int index, ref T last) {
            Items[index] = last;
            Count--;
        }

        public void RemoveSwapBack(int index) {
            if (index < 0 || Count == 0 || index > Count) return;
            var last = Count - 1;
            if (last != index) Items[index] = Items[last];
            Count--;
        }

        public void RemoveLast() {
            Count--;
        }
    }

    public interface ISharedComponent { }

    public interface IOnAddToEntity<T> {
        void OnAdd(ref T thisComponent);
    }

    public interface ICopyable<T> {
        T Copy(ref T thisComponent);
    }
    public interface IDisposable<T> {
        void Dispose(ref T thisComponent);
    }

    public readonly struct ComponentKey : IEquatable<ComponentKey> {
        public readonly int Value;

        public ComponentKey(int key) {
            Value = key;
        }

        public bool Equals(ComponentKey other) {
            return other.Value == Value;
        }

        public override int GetHashCode() {
            return Value;
        }
    }

    public readonly struct ComponentType<T> where T : struct {
        public static readonly int ID;
        public static readonly bool IsStruct;
        public static readonly bool IsUnmanaged;
        public static readonly bool IsTag;
        public static readonly bool IsOnAddToEntity;
        public static readonly bool IsDisposable;
        public static readonly bool IsCopyable;
        static ComponentType() {
            var type = typeof(T);
            IsTag = type.GetFields().Length < 1;
            ComponentType.Add(type);
            ID = ComponentType.GetID(type);
            IsUnmanaged = type.IsUnmanaged();
            IsStruct = type.IsValueType;
            IsOnAddToEntity = typeof(IOnAddToEntity<T>).IsAssignableFrom(type);
            IsDisposable = typeof(IDisposable<T>).IsAssignableFrom(type);
            IsCopyable = typeof(ICopyable<T>).IsAssignableFrom(type);
            CTS<T>.ID.Data = ID;
        }

        public static ComponentKey AsComponentKey() {
            return new(ID);
        }
    }

    /// <summary>
    ///     Component Type Shared
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CTS<T> where T : struct {
        public static readonly SharedStatic<int> ID;
        static CTS() {
            ID = SharedStatic<int>.GetOrCreate<CTS<T>>();
        }
    }

    public static class ComponentTypes {
        internal static int Count;
    }

    public static class ComponentType {
        private static readonly Dictionary<Type, int> TypeID;
        private static readonly Dictionary<int, Type> TypeValue;
        private static readonly Dictionary<Type, Type> SwapComponent;
        private static bool inited;

        static ComponentType() {
            TypeID = new Dictionary<Type, int>();
            TypeValue = new Dictionary<int, Type>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(Type type, int id) {
            if (TypeID.ContainsKey(type)) return;
            TypeID.Add(type, id);
            TypeValue.Add(id, type);
        }

        public static void Add(Type type) {
            if (TypeID.ContainsKey(type)) return;
            var id = Interlocked.Increment(ref ComponentTypes.Count);
            TypeID.Add(type, id);
            TypeValue.Add(id, type);
            // var swap = GetSwapComponent(type);
            // if(swap != null) {
            //     SwapComponent.Add(type, swap);
            // }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(Type type) {
            return TypeID.ContainsKey(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetID(Type type) {
            if (!TypeID.ContainsKey(type)) {
                var newID = Interlocked.Increment(ref ComponentTypes.Count);
                TypeID.Add(type, newID);
                TypeValue.Add(newID, type);
            }

            return TypeID[type];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetTypeValue(int id) {
#if UNITY_EDITOR || DEBUG
            if (!TypeValue.ContainsKey(id))
                Debug.LogError($"Component with ID:{id} isn't exist." +
                               " FirstcCreate Query or Pool with same component.");
#endif
            return TypeValue[id];
        }

        private static Type GetSwapComponent(Type type) {
            var attributes = type.GetCustomAttribute<SwapComponentAttribute>();
            if (attributes != null) {
                return attributes.swapTarget;
            }

            return null;
        }
    }

    internal struct TypePair<T1, T2> {
        internal static int Id;

        static TypePair() {
            Id = TypePairCount<T1>.Value++;
        }
    }

    internal struct TypePairCount<T1> {
        internal static int Value;
    }

    public static class CollectionHelp {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnmanaged(this Type t) {
            try {
                typeof(UnamagedGeneric<>).MakeGenericType(t);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEntityTypes(ref EntityType[] array, ref bool[] actives, int id) {
            if (array.Length == id - 1) {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateArray<T>(ref T[] array, int id) {
            if (array.Length == id - 1) {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEntities(ref Entities[] array, ref bool[] actives, int id) {
            if (array.Length == id - 1) {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }

        private class UnamagedGeneric<T> where T : unmanaged { }
    }

    public class IntMap {
        public int Capacity;
        private int[] items;

        public IntMap(int capacity) {
            items = new int[capacity];
            Capacity = capacity;
        }

        public int this[int key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items[key] - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Resize(int newSize) {
            Array.Resize(ref items, newSize);
            Capacity = newSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Assert(int key) {
            if (Capacity <= key) Resize(key + 16);
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

    public interface IEvent { }

    public interface IEventBuffer {
        void Clear();
    }

    public class EventBuffer<T> : IEventBuffer where T : struct, IEvent {
        public int count;
        private T[] data;
    
        public EventBuffer(int size) {
            data = new T[size];
            count = 0;
        }

        public int Count => count;
        public bool IsEmpty => count == 0;
        public void Clear() {
            count = 0;
        }

        public void Add(in T @event) {
            if (count == data.Length - 1) Array.Resize(ref data, count * 2);
            data[count++] = @event;
        }

        public ref T Get(int index) {
            return ref data[index];
        }
    }

    public struct DynamicBuffer<T> : IOnAddToEntity<DynamicBuffer<T>>, IDisposable<DynamicBuffer<T>> {
        private DynamicArray<T> buffer;
        private bool IsCreated;
        public int Count => buffer.Count;

        public DynamicBuffer(int capacity) {
            buffer = new DynamicArray<T>(capacity);
            IsCreated = true;
        }

        public void OnAdd(ref DynamicBuffer<T> thisComponent) {
            if (thisComponent.IsCreated == false) {
                thisComponent.buffer = new DynamicArray<T>(4);
                thisComponent.IsCreated = true;
            }
        }

        public void Dispose(ref DynamicBuffer<T> thisComponent) {
            if (thisComponent.IsCreated) thisComponent.buffer.ClearCount();
            thisComponent.IsCreated = false;
        }

        public ref T ElementAt(int index) {
            return ref buffer.Items[index];
        }

        public int Length => buffer.Count;

        public void Clear() {
            buffer.ClearCount();
        }

        public int Capacity => buffer.Items.Length;
        public bool IsEmpty => buffer.Count == 0;

        public T this[int index] {
            get => buffer.Items[index];
            set => buffer.Items[index] = value;
        }

        public void Add(T item) {
            buffer.Add(item);
        }

        public void RemoveAtSwapBack(int index) {
            buffer.RemoveSwapBack(index);
        }
    }

    [EcsComponent]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeDynamicBuffer<T> : IOnAddToEntity<NativeDynamicBuffer<T>>,
        IDisposable<NativeDynamicBuffer<T>>, INativeList<T> where T : unmanaged {
        [NativeDisableUnsafePtrRestriction] [NoAlias]
        private UnsafeList<T>* buffer;

        private bool IsCreated;

        public NativeDynamicBuffer(int initialCapacity) {
            buffer = UnsafeList<T>.Create(initialCapacity, Allocator.Persistent);
            IsCreated = true;
        }

        public NativeDynamicBuffer(int initialCapacity, Allocator allocator) {
            buffer = UnsafeList<T>.Create(initialCapacity, allocator);
            IsCreated = true;
        }

        public void OnAdd(ref NativeDynamicBuffer<T> thisComponent) {
            if (thisComponent.IsCreated == false) {
                thisComponent.buffer = UnsafeList<T>.Create(16, Allocator.Persistent);
                thisComponent.IsCreated = true;
            }
        }

        public void Dispose(ref NativeDynamicBuffer<T> thisComponent) {
            if (thisComponent.IsCreated == false) return;
            thisComponent.buffer->Dispose();
            thisComponent.buffer = null;
            thisComponent.IsCreated = false;
        }

        public ref T ElementAt(int index) {
            return ref buffer->ElementAt(index);
        }

        public int Length {
            get => buffer->Length;
            set => buffer->Length = value;
        }

        public void Clear() {
            buffer->Clear();
        }

        public int Capacity {
            get => buffer->Capacity;
            set => buffer->Capacity = value;
        }

        public bool IsEmpty => buffer->IsEmpty;

        public T this[int index] {
            get => buffer->ElementAt(index);
            set => buffer->ElementAt(index) = value;
        }

        public void Add(T item) {
            buffer->Add(item);
        }

        public void RemoveAt(int index) {
            buffer->RemoveAt(index);
        }

        public void RemoveAtSwapBack(int index) {
            buffer->RemoveAtSwapBack(index);
        }
    }

    public struct SequentalChunk { }
}