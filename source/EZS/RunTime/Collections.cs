using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using UnityEngine;

namespace Wargon.ezs
{
    public interface ICustomPool
    {
        int PoolType { get; set; }

        void Clear();
    }
    
    // [Il2CppSetOption(Option.NullChecks, false)]
    // [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    // [Il2CppSetOption(Option.DivideByZeroChecks, false)]

    public struct Observers {
        public EntityType[] observers;
        public int count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(EntityType entityType) {
            if (observers.Length <= count) {
                Array.Resize(ref observers, observers.Length<<1);
            }
            observers[count] = entityType;
            count++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnAddInclude(int id) {
            for (var i = 0; i < count; i++) {
                observers[i].OnAddInclude(id);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnRemoveInclude(int id) {
            for (var i = 0; i < count; i++) {
                observers[i].OnRemoveInclude(id);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnAddExclude(int id) {
            for (var i = 0; i < count; i++) {
                observers[i].OnAddExclude(id);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnRemoveExclude(int id) {
            for (var i = 0; i < count; i++) {
                observers[i].OnRemoveExclude(id);
            }
        }
    }
    
    public class Pool<T> : IPool where T : new()
    {
        public T[] items;
        public int length;
        private bool typeIsStruct;
        public event Action<int> OnRemove;
        public event Action<int> OnAdd;

        private bool isTag;
        // public Observers OnAddExclude;
        // public Observers OnAddInclude;
        // public Observers OnRemoveExclude;
        // public Observers OnRemoveInclude;
        
        public Pool(int size) {
            isTag = ComponentType<T>.IsTag;
            items = new T[size];
            ItemType = typeof(T);
            TypeID = ComponentType<T>.ID;
            typeIsStruct = ComponentType<T>.IsStruct;
            length = size;
            // OnAddExclude = new Observers {
            //     observers = new EntityType[4],
            //     count = 0
            // };
            // OnAddInclude = new Observers {
            //     observers = new EntityType[4],
            //     count = 0
            // };
            // OnRemoveExclude = new Observers {
            //     observers = new EntityType[4],
            //     count = 0
            // };
            // OnRemoveInclude = new Observers {
            //     observers = new EntityType[4],
            //     count = 0
            // };
        }

        public int TypeID { get; }
        public int Count { get; set; }
        public Type ItemType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int entity)
        {
            OnRemove?.Invoke(entity);
            // OnRemoveExclude.OnRemoveExclude(entity);
            // OnRemoveInclude.OnRemoveInclude(entity);
            Count--;
            Default(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int entity)
        {
            OnAdd?.Invoke(entity);
            // OnAddExclude.OnAddExclude(entity);
            // OnAddInclude.OnAddInclude(entity);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Default(int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
            }
            if (typeIsStruct) {
                items[id] = default;
            }
            else
                items[id] = new T();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int id) {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
            }

            if (!typeIsStruct) {
                items[id] = new T();
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
                // var dif = length > id ? length - id : id - length;
                // Array.Resize(ref items, 1 + length + dif);
                // length = items.Length;
                Array.Resize(ref items, id + 16);
                length = items.Length;
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
            }
            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(ref T component, int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 16);
                length = items.Length;
            }
            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(int id)
        {
            return items[id];
        }

        public override string ToString()
        {
            return $"{nameof(items)}: {items}";
        }

        int IPool.GetSize() => length;
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

        /// <summary>
        ///     <para>Get boxed component from pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        object Get(int id);

        /// <summary>
        ///     <para>Set boxed component to pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        void SetBoxed(object component, int id);

        /// <summary>
        ///     <para>Set boxed component to pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        void Set(int id);

        void Add(int entity);
        void Remove(int entity);
        int GetSize();
    }

    public class GrowList<T>
    {
        public int Count;
        public T[] Items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GrowList(int capacity)
        {
            Items = new T[capacity];
            Count = 0;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Items[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(T item)
        {
            if (Items.Length == Count) Array.Resize(ref Items, Items.Length << 1);
            Items[Count] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T GetLast()
        {
            return ref Items[Count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetByIndex(int index)
        {
            return Items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T Get(int entityID)
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
    }


    public struct ComponentType<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public readonly static int ID;
        // ReSharper disable once StaticMemberInGenericType
        public readonly static bool IsStruct;
        // ReSharper disable once StaticMemberInGenericType
        public readonly static bool IsUnmanaged;
        // ReSharper disable once StaticMemberInGenericType
        public readonly static bool IsTag;
        static ComponentType()
        {
            var Value = typeof(T);
            IsTag = Value.GetFields().Length < 1;
            if (ComponentTypeMap.Has(Value))
            {
                ID = ComponentTypeMap.GetID(Value);
            }
            else
            {
                ID = Interlocked.Increment(ref ComponentTypes.Count);
                ComponentTypeMap.Add(Value, ID);
            }

            IsStruct = Value.IsValueType;
        }
    }

    [EcsComponent]
    public struct TransformRef
    {
        public UnityEngine.Transform Value;
    }
    public static class ComponentTypes
    {
        internal static int Count;
    }
    
    public static class ComponentTypeMap
    {
        private static Dictionary<Type, int> TypeID;
        private static Dictionary<int, Type> TypeValue;
        private static bool inited;

        static ComponentTypeMap() {
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
        public static Type GetTypeByID(int id)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEntityTypes(ref fEntities[] array, ref bool[] actives, int id)
        {
            if (array.Length == id - 1)
            {
                var newL = array.Length + 16;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }
    }

    internal class IntKeyMap<TItem> {
        private TItem[] items;
        private int[] keys;
        public int Capacity;
        public int Count;
        public ref TItem this[int key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref items[keys[key]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Assert(int key) {
            if (Capacity <= key) {
                Resize(key + 16);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Resize(int newSize) {
            Array.Resize(ref items, newSize);
            Capacity = newSize;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int key, TItem value) {
            Assert(key);
            keys[Count] = key;
            items[key] = value;
        }
    }
    
    internal class IntMap {
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