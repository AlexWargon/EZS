using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
        public T[] Items;
        public Type ItemType { get; }
        internal Pool(int size)
        {
            Items = new T[size];
            ItemType = ComponentType<T>.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Default(int id)
        {
            Items[id] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(T component, int id)
        {
            if (Items.Length - 1 < id)
            {
                var length = Items.Length;
                var dif = length > id ? length - id : id - length;
                Array.Resize(ref Items, 1 + length + dif);
            }
            Items[id] = component;
        }
    }
    public interface IPool
    {
        Type ItemType { get; }
        void Default(int id);
    }
    internal class TypeMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, int> _indexMap;
        public readonly GrowList<TValue> Values;
        public int Count => Values.Count;
        public TValue this[TKey key] => Values[_indexMap[key]];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypeMap(int size)
        {
            _indexMap = new Dictionary<TKey, int>();
            Values = new GrowList<TValue>(size);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue item)
        {
            if (_indexMap.ContainsKey(key)) return;
            _indexMap.Add(key, Values.Count);
            Values.Add(item);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasKey(TKey key)
        {
            return _indexMap.ContainsKey(key);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Get(TKey key)
        {
            return Values.Items[_indexMap[key]];
        }

        public void Clear()
        {
            Array.Clear(Values.Items, 0, Values.Items.Length);
            _indexMap.Clear();
        }

    }
    public interface IGrowList { }
    internal class GrowList<T> : IGrowList
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
        }
    }
    public static class type<T>
    {
        public static readonly Type Value;
        static type()
        {
            Value = typeof(T);
        }
    }

    public static class ComponentTypes
    {
        internal static int Count;
    }

}