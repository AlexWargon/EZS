using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using UnityEngine;

namespace Wargon.ezs
{
    public partial struct Entity
    {
        public World world;
        public int id;
        public int generation;
    }

    public partial struct Entity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>(A component)
        {
            ref var data = ref world.GetEntityData(id);
            var type = ComponentType<A>.ID;
            if (data.componentTypes.Contains(type)) return;
            
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>(T component)");

            data.componentTypes.Add(type);
            data.componentsCount++;
            world.GetPool<A>().Set(ref component, id);
            world.OnAddComponent(this, ref data, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A Get<A>()
        {
            if (Has<A>()) return ref world.GetPool<A>().items[id];

            ref var data = ref world.GetEntityData(id);
            var type = ComponentType<A>.ID;
            data.componentTypes.Add(type);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(default, id);
            world.OnAddComponent(this, ref data, type);

            return ref pool.items[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A Add<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED");
            var type = ComponentType<A>.ID;
            data.componentTypes.Add(type);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(default, id);
            world.OnAddComponent(this, ref data, type);

            return ref pool.items[id];
        }
        /// <summary>
        /// Remove component of type
        /// </summary>
        /// <typeparam name="A">Component</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method: Entity.Remove<T>()");
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Remove(typeId))
            {
                --data.componentsCount;
                world.OnRemoveComponent(in this, in data, typeId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveByTypeID(int typeId)
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method: Entity.RemoveByTypeID(int typeId)");

            if (data.componentTypes.Remove(typeId))
            {
                --data.componentsCount;
                world.OnRemoveComponent(in this, in data, typeId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method : Entity.Has<A>()");
            var typeId = ComponentType<A>.ID;
            return data.componentTypes.Contains(typeId);
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
            if(data.active == value) return;
            data.active = value;
            if (value)
                world.OnActivateEntity(this, data);
            else
                world.OnDeActivateEntity(this, data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDead()
        {
            return generation == -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsActive()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED");
            return data.active;
        }
    }

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
        public static void AddBoxed(this ref Entity entity, object component)
        {
            var type = component.GetType();
            var typeID = ComponentTypeMap.GetID(type);
            ref var data = ref entity.world.GetEntityData(entity.id);
            var pool = entity.world.GetPoolByID(typeID, type);
            if (data.generation != entity.generation) throw new Exception("ENTITY NULL OR DESTROYED. Method: Entity.AddBoxed()");

            data.componentTypes.Add(typeID);
            data.componentsCount++;
            pool.SetItem(component, entity.id);
            entity.world.OnAddComponent(entity, ref data, typeID);
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
                pool = (IPool) Activator.CreateInstance(poolType.MakeGenericType(type), new object[] { world.ComponentCachSize });
                world.ComponentPools[typeID] = pool;
            }
            return pool;
        }
    }
}