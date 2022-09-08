using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    [Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct Entity : IEquatable<Entity>
    {
        public int id;
        public int generation;
        public World World;
        public static Entity Null => new Entity();
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool operator == (in Entity lhs, in Entity rhs) {
            return lhs.id == rhs.id && lhs.generation == rhs.generation;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Entity lhs, in Entity rhs) {
            return lhs.id != rhs.id || lhs.generation != rhs.generation;
        }
        public bool Equals(Entity other) {
            return id == other.id && generation == other.generation && World == other.World;
        }
        public override int GetHashCode() {
            unchecked {
                var hashCode = id;
                hashCode = (hashCode * 397) ^ generation;
                // ReSharper disable once NonReadonlyMemberInGetHashCode

                hashCode = (hashCode * 397) ^ (World != null ? World.GetHashCode() : 0);
                return hashCode;
            }
        }
        public override bool Equals(object obj) {
            return obj is Entity other && Equals(other);
        }
    }

    public static class EntityExtensionMethods
    {
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<A>(in this Entity e, in A component) where A : new()
        {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
#if DEBUG
            if (data.generation != e.generation)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>(T component)");
#endif
            if (data.componentTypes.Add(typeId)) {
                data.componentsCount++;
                var pool = e.World.GetPool<A>();
                pool.Set(component, e.id);
                pool.Add(e.id);
                e.World.OnAddComponent(typeId, in e);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref A Get<A>(in this Entity e) where A : new()
        {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
#if DEBUG
            if (data.generation != e.generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Get<T>()");
#endif
            if (data.componentTypes.Contains(typeId)) return ref e.World.GetPool<A>().items[e.id];

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = e.World.GetPool<A>();
            pool.Set(e.id);
            pool.Add(e.id);
            e.World.OnAddComponent(typeId, in e);
            return ref pool.items[e.id];
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref A GetRef<A>(in this Entity e) where A : new()
        {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
#if DEBUG
            if (data.generation != e.generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.GetRef<T>()");
#endif
            if (data.componentTypes.Contains(typeId)) return ref e.World.GetPool<A>().items[e.id];

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = e.World.GetPool<A>();
            pool.Set(e.id);
            pool.Add(e.id);
            e.World.OnAddComponent(typeId, in e);
            return ref pool.items[e.id];
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<A>(in this Entity e) where A : new()
        {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG
            if (data.generation != e.generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Set<T>()");
#endif
            
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;
            
            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = e.World.GetPool<A>();
            pool.Set(e.id);
            pool.Add(e.id);
            e.World.OnAddComponent(typeId, in e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Replace<A>(in this Entity e, A component) where A : new()
        {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.generation != e.generation)
                throw new Exception("ENTITY NULL OR DESTROYED!");

            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId))
            {
                var pool = e.World.GetPool<A>();
                pool.Set(component, e.id);
            }
            else
            {
                data.componentTypes.Add(typeId);
                data.componentsCount++;
                var pool = e.World.GetPool<A>();
                pool.Set(component, e.id);
                pool.Add(e.id);
                e.World.OnAddComponent(typeId, in e);
            }
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<A>(in this Entity e) where A : new()
        {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.generation != e.generation)
            {
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Remove<T>()");
            }
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Remove(typeId))
            {
                data.componentsCount--;
                e.World.GetPool<A>().Remove(e.id);
                e.World.OnRemoveComponent(typeId, in e);
                if (data.componentsCount < 1) {
                    e.Destroy();
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveByTypeID(in this Entity e, int typeId)
        {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.generation != e.generation)
                throw new Exception("ENTITY NULL OR DESTROYED!");

            if (data.componentTypes.Remove(typeId))
            {
                data.componentsCount--;
                e.World.GetPoolByID(typeId).Remove(e.id);
                e.World.OnRemoveComponent(typeId, in e);
                if (data.componentsCount < 1)
                    e.Destroy();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<A>(in this Entity e)
        {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.generation != e.generation)
            {
                //UnityEngine.Debug.LogError("ENTITY NULL OR DESTROYED!");
                return false;
            }
            var typeId = ComponentType<A>.ID;
            return data.componentTypes.Contains(typeId);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref EntityData GetEntityData(in this Entity e)
        {
            return ref e.World.GetEntityData(e.id);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in this Entity e)
        {
            ref var data = ref e.World.GetEntityData(e.id);
            foreach (var dataComponentType in data.componentTypes) {
                e.World.GetPoolByID(dataComponentType).Remove(e.id);
            }
            data.componentsCount = 0;
            data.componentTypes.Clear();
            data.generation++;
            e.World.OnDestroyEntity(in e);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Check is entity destroyed or alive
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNULL(in this Entity e) {
            if (e.World == null) return true;
            ref var data = ref e.World.GetEntityData(e.id);
            return data.generation != e.generation;
        }
    }

    [Serializable]
    public struct EntityData
    {
        public int id;
        public int componentsCount;
        public HashSet<int> componentTypes;
        public int generation;
    }

    public class SavedEntity
    {
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Rotation;
        public List<object> Components;
    }
}