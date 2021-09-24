using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Wargon.ezs.Unity;

namespace Wargon.ezs
{
    [Serializable]
    public partial class Entity
    {
        public int id;
        public int generation;
        public World world;
    }
    
    public partial class Entity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>(A component) where A : class
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;

            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>(T component)");

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(component, id);
            pool.Add(this);
            world.OnAddComponent(typeId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>(A component, bool isClass = false) where A : struct
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;

            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>(T component)");

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(component, id);
            pool.Add(this);
            world.OnAddComponent(typeId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public A Get<A>()
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Get<A>()");

            if (data.componentTypes.Contains(typeId)) return world.GetPool<A>().items[id];

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(id);
            pool.Add(this);
            world.OnAddComponent(typeId);
            return pool.items[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A GetRef<A>()
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Get<A>()");

            if (data.componentTypes.Contains(typeId)) return ref world.GetPool<A>().items[id];

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(id);
            pool.Add(this);
            world.OnAddComponent(typeId);
            return ref pool.items[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Set<A>()");
            var pool = world.GetPool<A>();
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            pool.Set(id);
            pool.Add(this);
            world.OnAddComponent(typeId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Replace<A>(A component)
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Replace<A>(A component)");

            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId))
            {
                var pool = world.GetPool<A>();
                pool.Set(component, id);
            }
            else
            {
                data.componentTypes.Add(typeId);
                data.componentsCount++;
                var pool = world.GetPool<A>();
                pool.Set(component, id);
                pool.Add(this);
                world.OnAddComponent(typeId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
            {
                Debug.LogError("ENTITY NULL OR DESTROYED! Method: Entity.Remove<T>()");
                return;
            }
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Remove(typeId))
            {
                --data.componentsCount;
                world.GetPool<A>().Remove(this);
                world.OnRemoveComponent(typeId);
                if (data.componentsCount < 1)
                    Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveByTypeID(int typeId)
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.RemoveByTypeID(int typeId)");

            if (data.componentTypes.Remove(typeId))
            {
                --data.componentsCount;
                world.GetPoolByID(typeId).Remove(this);
                world.OnRemoveComponent(typeId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
            {
                Debug.LogError("ENTITY NULL OR DESTROYED! Method : Entity.Has<A>()");
                return false;
            }
            var typeId = ComponentType<A>.ID;
            return data.componentTypes.Contains(typeId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Copy()
        {
            var copy = world.CreateEntity();
            ref var copyData = ref world.GetEntityData(copy.id);
            ref var data = ref world.GetEntityData(id);

            foreach (var typeId in data.componentTypes)
            {
                var pool = world.GetPoolByID(typeId);

                copyData.componentTypes.Add(typeId);
                world.OnRemoveComponent(typeId);
                copyData.componentsCount++;
                pool.Set(copy.id);
            }

            return copy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref EntityData GetEntityData()
        {
            return ref world.GetEntityData(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            ref var data = ref world.GetEntityData(id);
            foreach (var dataComponentType in data.componentTypes)
                world.ComponentPools[dataComponentType].Default(id);
            data.componentsCount = 0;
            data.componentTypes.Clear();
            data.generation++;
            generation = -1;
            world.OnDestroyEntity(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActive(bool value)
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED");
            if (data.active == value) return;
            data.active = value;
            if (value)
                world.OnActivateEntity(this, data);
            else
                world.OnDeActivateEntity(this, data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDead()
        {
            if (world == null)
                return true;
            return generation == -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsActive()
        {
            if (world == null)
                return false;
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED");
            return data.active;
        }
    }

    [Serializable]
    public struct EntityData
    {
        public int id;
        public int componentsCount;
        public HashSet<int> componentTypes;
        public int generation;
        public bool active;
    }

    public static class EntityExtension
    {
        //private static readonly MethodInfo AddComponent = typeof(Entity).GetMethod("Add");
        public static void AddBoxed(this Entity entity, object component)
        {
            var type = component.GetType();
            var typeId = ComponentTypeMap.GetID(type);

            ref var data = ref entity.world.GetEntityData(entity.id);
            var pool = entity.world.GetPoolByID(typeId);
            if (data.generation != entity.generation)
                throw new Exception("ENTITY NULL OR DESTROYED. Method: Entity.AddBoxed()");

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            pool.Set(component, entity.id);
            pool.Add(entity);
            entity.world.OnAddComponent(typeId);
        }

        public static IPool GetPoolByID(this World world, int typeID, Type type)
        {
            if (world.ComponentPools.Length < typeID)
            {
                var length = world.ComponentPools.Length << 1;
                while (length <= typeID)
                    length <<= 1;
                Array.Resize(ref world.ComponentPools, length);
            }

            var pool = world.ComponentPools[typeID];
            if (pool == null)
            {
                var poolType = typeof(Pool<>);
                pool = (IPool) Activator.CreateInstance(poolType.MakeGenericType(type), world.ComponentCacheSize);
                world.ComponentPools[typeID] = pool;
            }

            return pool;
        }

        public static IPool GetPoolByID(this World world, int typeID)
        {
            if (world.ComponentPools.Length <= typeID)
            {
                var length = world.ComponentPools.Length << 1;
                while (length <= typeID + 2)
                    length <<= 1;
                Array.Resize(ref world.ComponentPools, length);
            }

            var pool = world.ComponentPools[typeID];
            if (pool == null)
            {
                var poolType = typeof(Pool<>);
                pool = (IPool) Activator.CreateInstance(poolType.MakeGenericType(ComponentTypeMap.GetValue(typeID)),
                    world.ComponentCacheSize);
                world.ComponentPools[typeID] = pool;
            }

            return pool;
        }

        public static MonoEntity SaveEntity(this Entity entity)
        {
            var monoEntity = entity.Get<View>().Value;
            var world = entity.world;
            ref var data = ref entity.GetEntityData();
            monoEntity.Components.Clear();
            foreach (var dataComponentType in data.componentTypes)
            {
                var pool = world.GetPoolByID(dataComponentType);
                var component = pool.Get(entity.id);
                monoEntity.Components.Add(component);
            }
            
            return monoEntity;
        }
    }

    public class SavedEntity
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public List<object> Components;
    }
}