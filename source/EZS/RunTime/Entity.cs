using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>(A component)
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;
#if DEBUG
            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>(T component)");
#endif
            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(component, id);
            pool.Add(id);
            world.OnAddComponent(typeId, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>(A component, bool isClass = false) where A : struct
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;
#if DEBUG
            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED!");
#endif
            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(component, id);
            pool.Add(id);
            world.OnAddComponent(typeId, this);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public A Get<A>()
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
#if DEBUG
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Get<T>()");
#endif
            if (data.componentTypes.Contains(typeId)) return world.GetPool<A>().items[id];

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(id);
            pool.Add(id);
            world.OnAddComponent(typeId, this);
            return pool.items[id];
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A GetRef<A>()
        {
            ref var data = ref world.GetEntityData(id);
            var typeId = ComponentType<A>.ID;
#if DEBUG
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.GetRef<T>()");
#endif
            if (data.componentTypes.Contains(typeId)) return ref world.GetPool<A>().items[id];

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            var pool = world.GetPool<A>();
            pool.Set(id);
            pool.Add(id);
            world.OnAddComponent(typeId, this);
            return ref pool.items[id];
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<A>()
        {
            ref var data = ref world.GetEntityData(id);
#if DEBUG
            if (data.generation != generation) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Set<T>()");
#endif
            var pool = world.GetPool<A>();
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Contains(typeId)) return;

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            pool.Set(id);
            pool.Add(id);
            world.OnAddComponent(typeId, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Replace<A>(A component)
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED!");

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
                pool.Add(id);
                world.OnAddComponent(typeId, this);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
            {
                UnityEngine.Debug.LogError("ENTITY NULL OR DESTROYED!");
                return;
            }
            var typeId = ComponentType<A>.ID;
            if (data.componentTypes.Remove(typeId))
            {
                data.componentsCount--;
                world.GetPool<A>().Remove(id);
                world.OnRemoveComponent(typeId, this);
                if (data.componentsCount < 1) {
                    Destroy();
                    //UnityEngine.Debug.Log($"ENTITY WITH {typeof(A)} DESTTOYED");
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveByTypeID(int typeId)
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
                throw new Exception("ENTITY NULL OR DESTROYED!");

            if (data.componentTypes.Remove(typeId))
            {
                data.componentsCount--;
                world.GetPoolByID(typeId).Remove(id);
                world.OnRemoveComponent(typeId, this);
                if (data.componentsCount < 1)
                    Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<A>()
        {
            ref var data = ref world.GetEntityData(id);
            if (data.generation != generation)
            {
                UnityEngine.Debug.LogError("ENTITY NULL OR DESTROYED!");
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
                world.OnRemoveComponent(typeId, this);
                copyData.componentsCount++;
                pool.Set(copy.id);
            }

            return copy;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref EntityData GetEntityData()
        {
            return ref world.GetEntityData(id);
        }

        // ReSharper disable Unity.PerformanceAnalysis
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

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDead()
        {
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
            if (component == null)
            {
                UnityEngine.Debug.LogError($"Try add null component on entity {entity.id} " + Environment.NewLine +
                                "Looks like some component on prefab was currupted :C");
            }
            var type = component.GetType();
            var typeId = ComponentTypeMap.GetID(type);

            ref var data = ref entity.GetEntityData();
            var pool = entity.world.GetPoolByID(typeId);
            if (data.generation != entity.generation)
                throw new Exception("ENTITY NULL OR DESTROYED. Method: Entity.AddBoxed()");

            data.componentTypes.Add(typeId);
            data.componentsCount++;
            pool.Set(component, entity.id);
            pool.Add(entity.id);
            entity.world.OnAddComponent(typeId, in entity);
        }

        public static bool Has(this Entity entity, int type)
        {
            return entity.GetEntityData().componentTypes.Contains(type);
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
                pool = (IPool) Activator.CreateInstance(poolType.MakeGenericType(ComponentTypeMap.GetTypeByID(typeID)),
                    world.ComponentCacheSize);
                world.ComponentPools[typeID] = pool;
            }
            return pool;
        }
        
        public static MonoEntity SaveEntity(this Entity entity)
        {
            var monoEntity = entity.Get<View>().value;
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
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Rotation;
        public List<object> Components;
    }
}