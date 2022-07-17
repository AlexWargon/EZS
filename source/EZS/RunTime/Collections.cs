using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;

namespace Wargon.ezs
{
    public interface ICustomPool
    {
        int PoolType { get; set; }

        void Clear();
    }
    
    [Serializable]
    public class Pool<T> : IPool
    {
        public T[] items;
        public int length;

        public Pool(int size)
        {
            items = new T[size];
            ItemType = typeof(T);
            TypeID = ComponentType<T>.ID;
            length = size;
        }

        public int TypeID { get; }
        public Type ItemType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int entity)
        {
            OnRemove?.Invoke(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int entity)
        {
            OnAdd?.Invoke(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Default(int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 256);
                length = items.Length;
            }

            items[id] = (T) Activator.CreateInstance(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int id)
        {
            if (length - 1 < id)
            {
                Array.Resize(ref items, id + 256);
                length = items.Length;
            }
            if(items[id] == null)
                items[id] = (T) Activator.CreateInstance(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IPool.Get(int id)
        {
            return items[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IPool.Set(object component, int id)
        {
            if (length - 1 < id)
            {
                // var dif = length > id ? length - id : id - length;
                // Array.Resize(ref items, 1 + length + dif);
                // length = items.Length;
                Array.Resize(ref items, id + 256);
                length = items.Length;
            }
            items[id] = (T)component;
        }

        public event Action<int> OnRemove;
        public event Action<int> OnAdd;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(T component, int id)
        {
            if (length - 1 < id)
            {
                // var dif = length > id ? length - id : id - length;
                // Array.Resize(ref items, 1 + length + dif);
                // length = items.Length;
                Array.Resize(ref items, id + 256);
                length = items.Length;
            }

            items[id] = component;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(ref T component, int id)
        {
            if (length - 1 < id)
            {
                // var dif = length > id ? length - id : id - length;
                // Array.Resize(ref items, 1 + length + dif);
                // length = items.Length;
                Array.Resize(ref items, id + 256);
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

    public interface IPool
    {
        int TypeID { get; }
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
        void Set(object component, int id);

        /// <summary>
        ///     <para>Set boxed component to pool (NEED FOR UNITY EXTENSION)</para>
        /// </summary>
        void Set(int id);

        void Add(int entity);
        void Remove(int entity);
        int GetSize();
    }

    public struct TypeMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, int> indexMap;
        public readonly GrowList<TValue> Values;
        public int Count => Values.Count;

        public TValue this[TKey key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values[indexMap[key]];
        }

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

    public interface IGrowList
    {
    }

    public class GrowList<T> : IGrowList
    {
        public int Count;
        public T[] Items;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GrowList(int capacity)
        {
            Items = new T[capacity];
            Count = 0;
        }

        public T this[int index] => Items[index];

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


    public static class ComponentType<T>
    {
        public readonly static int ID;

        static ComponentType()
        {
            var Value = typeof(T);
            if (ComponentTypeMap.Has(Value))
            {
                ID = ComponentTypeMap.GetID(Value);
            }
            else
            {
                ID = Interlocked.Increment(ref ComponentTypes.Count);
                ComponentTypeMap.Add<T>(Value, ID);
            }

            if(Value.IsUnmanaged())
                ComponentTypeStruct<T>.SetID(ID);

        }
    }
    public struct ComponentTypeStruct<T>
    {
        private readonly static SharedStatic<int> typeID;
        public static int ID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => typeID.Data;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetID(int ids) => typeID.Data = ids;
        static ComponentTypeStruct()
        {
            typeID = SharedStatic<int>.GetOrCreate<ComponentTypeStruct<T>>();
        }
    }
    public static class ComponentTypes
    {
        internal static int Count;
    }

    internal static class CustomPoolsCount
    {
        public static int Value;
    }
    internal static class CustomPoolID<T>
    {
        public static int Value;
        static CustomPoolID()
        {
            Value = CustomPoolsCount.Value++;
        }
    }
    
    public static class ComponentTypeMap
    {
        private static Dictionary<Type, int> TypeID;
        private static Dictionary<int, Type> TypeValue;
        private static World World;
        private static bool inited;

        public static void Init(World world)
        {
            if (inited) return;
            World = world;
            TypeID = new Dictionary<Type, int>();
            TypeValue = new Dictionary<int, Type>();
            inited = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<A>(Type type, int id)
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

    public static class Component<T>
    {
        private static readonly Func<T> fastActivator;

        static Component()
        {
            fastActivator = FastActivator.Generate<T>();
        }

        public static T New()
        {
            return fastActivator();
        }
    }
    internal static class TypePair<T1, T2>
    { 
        internal static int Id;
        static TypePair()
        {
            Id = Counts<T1>.Value++;
        }
    }

    internal static class Counts<T1>
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
                var newL = array.Length + 256;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEntities(ref Entities[] array, ref bool[] actives, int id)
        {
            if (array.Length == id - 1)
            {
                var newL = array.Length + 256;
                Array.Resize(ref array, newL);
                Array.Resize(ref actives, newL);
            }
        }
    }
    public static class FastActivator
    {
        public static Func<TResult> Generate<TResult>()
        {
            var constructorInfo = typeof(TResult).GetConstructor(Type.EmptyTypes);

            var expression = Expression.Lambda<Func<TResult>>(
                Expression.New(constructorInfo, null), null);
            var functor = expression.Compile();
            return functor;
        }
    }
}