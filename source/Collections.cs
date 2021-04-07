using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Wargon.ezs
{
    public class Delay
    {
        public static void Run(float time, Action callBack)
        {
            if (callBack == null) return;
            Task.Delay((int)time / 1000).ContinueWith(t => callBack?.Invoke());
        }
    }
    public class Pool<T> : IPool
    {
        public T[] items;
        public Type ItemType { get; }
        public Pool(int size)
        {
            items = new T[size];
            ItemType = ComponentType<T>.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Default(int id)
        {            
            if (items.Length - 1 < id)
            {
                var length = items.Length;
                var dif = length > id ? length - id : id - length;
                Array.Resize(ref items, 1 + length + dif);
            }
            items[id] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(ref T component, int id)
        {
            if (items.Length - 1 < id)
            {
                var length = items.Length;
                var dif = length > id ? length - id : id - length;
                Array.Resize(ref items, 1 + length + dif);
            }
            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(T component, int id)
        {
            if (items.Length - 1 < id)
            {
                var length = items.Length;
                var dif = length > id ? length - id : id - length;
                Array.Resize(ref items, 1 + length + dif);
            }
            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetItem(int id)
        {
            return ref items[id];
        }

        public override string ToString()
        {
            return $"{nameof(items)}: {items}";
        }

        /// <summary>
        /// <para>Get boxed component from pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IPool.GetItem(int id)
        {
            return items[id];
        }

        /// <summary>
        /// <para>Set boxed component to pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetItem(object component ,int id)
        {
            if (items.Length - 1 < id)
            {
                var length = items.Length;
                var dif = length > id ? length - id : id - length;
                Array.Resize(ref items, 1 + length + dif);
            }
            items[id] = (T) component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResizePool(ref T[] pool)
        {
            var length = items.Length;
            Array.Resize(ref items, length + 16);
        }
    }
    public interface IPool
    {
        Type ItemType { get; }
        /// <summary>
        /// <para>Set component with entity ID to default value </para>
        /// </summary>
        void Default(int id);
        /// <summary>
        /// <para>Get boxed component from pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        object GetItem(int id);
        /// <summary>
        /// <para>Set boxed component to pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        void SetItem(object component ,int id);
    }
    public struct TypeMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, int> indexMap;
        public readonly GrowList<TValue> Values;
        public int Count => Values.Count;
        public TValue this[TKey key] => Values[indexMap[key]];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypeMap(int size)
        {
            indexMap = new Dictionary<TKey, int>();
            Values = new GrowList<TValue>(size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue item)
        {
            if (indexMap.ContainsKey(key)) return;
            indexMap.Add(key, Values.Count);
            Values.Add(item);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(TKey key)
        {
            return indexMap.ContainsKey(key);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Get(TKey key)
        {
            return Values.Items[indexMap[key]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(Values.Items, 0, Values.Items.Length);
            indexMap.Clear();
        }

    }
    public interface IGrowList { }

    public class GrowList<T> : IGrowList
    {

        public T[] Items;
        public int Count;

        public T this[int index] => Items[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GrowList(int capacity)
        {
            Items = new T[capacity];
            Count = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(T item)
        {
            if (Items.Length == Count)
            {
                Array.Resize(ref Items, Items.Length << 1);
            }
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
            Array.Clear(Items,0,Items.Length);
            Count = 0;
        }
    }
    public static class ComponentType<T>
    {
        public static readonly int ID;
        public static readonly Type Value;
        static ComponentType()
        {
            ID = Interlocked.Increment(ref ComponentTypes.Count);
            Value = typeof(T);
            ComponentTypeMap.Add(Value,ID);
        }
    }
    public static class ComponentTypes
    {
        internal static int Count;
    }
    public static class type<T>
    {
        public static readonly Type Value;
        static type()
        {
            Value = typeof(T);
        }
    }

    public static class ComponentTypeMap
    {
        public static Dictionary<Type, int> TypeID = new Dictionary<Type, int>();
        public static World World;
        public static void Add(Type type, int id)
        {
            if(TypeID.ContainsKey(type)) return;
            TypeID.Add(type, id);
        }

        public static int GetID(Type type)
        {
            if (!TypeID.ContainsKey(type))
            {
                var newID = Interlocked.Increment(ref ComponentTypes.Count);
                TypeID.Add(type, newID);
            }
            return TypeID[type];
        }
    }
    

    
}