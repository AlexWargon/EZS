
using Unity.Collections;
using UnityEngine;

namespace Wargon.ezs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    public class Configs
    {
        public static int ComponentCacheSize = 512;
        public static int EntityCacheSize = 512;
        public static int PoolsCacheSize = 64;
        public static int EntityTypesCacheSize = 64;
    }

    public static class Worlds
    {
        private static World[] WorldsPool;
        private static int WorldsCount;

        static Worlds()
        {
            WorldsPool = new World[4];
            WorldsCount = 0;
        }
        public static World New()
        {
            var world = new World();
            WorldsPool[WorldsCount] = world;
            world.ID = WorldsCount;
            WorldsCount++;
            return world;
        }
        public static World GetWorld(int id)
        {
            return WorldsPool[id];
        }
    }
    public struct DestroyEntity{}
    [EcsComponent] 
    public struct Owner {
        public Entity Value;
    }
    
    public struct OwnerNative {
        public int id;
    }
    public struct EntityConvertedEvent {}
    public partial class World
    {
        public int ID;
        public readonly int ComponentCacheSize;
        public readonly int EntityCacheSize;
        public readonly int PoolsCacheSize;
        public readonly int EntityTypesCachSize;
        public Entity[] entities;
        public Entities Entities;
        private IPool[] ComponentPools;
        private int poolsAmount;
        private Dictionary<Type, IEventBuffer> EventBuffers;
        private int eventBuffersAmount;
        public int totalEntitiesCount;
        public int aliveEntitiesCount;
        public bool Alive;
        private EntityData[] entitiesData;
        private DynamicArray<int> freeEntities;
        private Systems[] systems;
        private int systemsCount;
        private int freeEntitiesCount = 0;
        private event Action OnDestroy;

        public int GetFreeEntitiesCount() => freeEntities.Count;
        public World()
        {
            ComponentCacheSize = Configs.ComponentCacheSize;
            EntityCacheSize = Configs.EntityCacheSize;
            PoolsCacheSize = Configs.PoolsCacheSize;
            EntityTypesCachSize = Configs.EntityTypesCacheSize;
            ComponentPools = new IPool[PoolsCacheSize];
            entitiesData = new EntityData[EntityCacheSize];
            entities = new Entity[EntityCacheSize];
            freeEntities = new DynamicArray<int>(EntityCacheSize);
            systems = new Systems[4];
            Entities = new Entities(this);
            Alive = true;

            EventBuffers = new Dictionary<Type, IEventBuffer>(8);
            eventBuffersAmount = 0;
            
            GetPool<DestroyEntity>();
            ownerPool = GetPool<Owner>();
            ownerNativePool = GetPool<OwnerNative>();
            CreateFirstArchetype();
            var zeroE = CreateEntity();
            zeroE.Add(new Owner(){Value = new Entity(){id = -99999, version = -99999, World = this}});

        }

        private readonly Pool<Owner> ownerPool;
        private readonly Pool<OwnerNative> ownerNativePool;
        internal Pool<Owner> OwnerPool => ownerPool;
        internal Pool<OwnerNative> OwnerNativePool => ownerNativePool;

        public int GetSystemsCount() => systemsCount;
        public void AddSystems(Systems add)
        {
            add.id = systemsCount;
            if (systems[add.id]!=null) return;
            systems[systemsCount] = add;
            systemsCount++;
        }

        public Systems[] GetAllSystems()
        {
            return systems;
        }

        public T GetSystem<T>() where T : UpdateSystem {
            for (int i = 0; i < systemsCount; i++) {
                ref var s = ref systems[i];
                for (int j = 0; j < s.updateSystemsList.Count; j++) {
                    ref var system = ref s.updateSystemsList.Items[j];
                    if (system is T TS)
                        return TS;
                }
            }

            throw new Exception($"System of type {typeof(T).Name} is not initialized on not exist");
        }
        internal object GetSystem(Type type) {
            for (int i = 0; i < systemsCount; i++) {
                ref var s = ref systems[i];
                for (int j = 0; j < s.updateSystemsList.Count; j++) {
                    ref var system = ref s.updateSystemsList.Items[j];
                    if (system.GetType().Name == type.Name)
                        return system;
                }
            }

            throw new Exception($"System of type {type.Name} is not initialized on not exist");
        }
        public void Destroy()
        {
            for (var i = 0; i < systemsCount; i++)
            {
                systems[i].Destroy();
            }
            for (int i = 0; i < poolsAmount-1; i++) {
                ref var pool = ref ComponentPools[i];
                pool?.Dispose();
            }
            OnDestroy?.Invoke();
            OnDestroy = null;
            Array.Clear(entities, 0, totalEntitiesCount);
            Array.Clear(entitiesData, 0, totalEntitiesCount);
            Array.Clear(freeEntities.Items, 0, freeEntitiesCount);
            Array.Clear(ComponentPools, 0, ComponentPools.Length);
            Entities.Clear();
            Array.Clear(systems,0, systemsCount);
            totalEntitiesCount = 0;

            Alive = false;
        }
        
        public void SubOnDestroy(Action callback)
        {
            OnDestroy += callback;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref EntityData GetEntityData(int entityId)
        {
            return ref entitiesData[entityId];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity()
        {
            Entity entity;
            entity.World = this;
            if (freeEntities.Count == 0)
            {
                if (entitiesData.Length == totalEntitiesCount)
                {
                    Array.Resize(ref entities, entities.Length + 256);
                    Array.Resize(ref entitiesData, entitiesData.Length + 256);
                }
                
                entity.id = totalEntitiesCount;
                ref var data = ref entitiesData[entity.id];
                data.version = 1;
                entity.version = data.version;
                entities[entity.id] = entity;
                data.id = entity.id;
                data.archetype = emptyArchetype;
                totalEntitiesCount++;
            }
            else
            {
                entity.id = freeEntities.Items[--freeEntities.Count];
                ref var data = ref entitiesData[entity.id];
                entity.version = data.version;
                data.id = entity.id;
                data.archetype = emptyArchetype;
                entities[entity.id] = entity;
            }
            aliveEntitiesCount++;
            return entity;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity(Archetype archetype)
        {
            Entity entity;
            entity.World = this;
            if (freeEntities.Count == 0)
            {
                if (entitiesData.Length == totalEntitiesCount)
                {
                    Array.Resize(ref entities, entities.Length + 256);
                    Array.Resize(ref entitiesData, entitiesData.Length + 256);
                }
                
                entity.id = totalEntitiesCount;
                ref var data = ref entitiesData[entity.id];
                data.version = 1;
                entity.version = data.version;
                entities[entity.id] = entity;
                data.id = entity.id;
                data.archetype = archetype;
                totalEntitiesCount++;
            }
            else
            {
                entity.id = freeEntities.Items[--freeEntities.Count];
                ref var data = ref entitiesData[entity.id];
                entity.version = data.version;
                data.id = entity.id;
                data.archetype = emptyArchetype;
                entities[entity.id] = entity;
            }
            aliveEntitiesCount++;
            return entity;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnDestroyEntity(in Entity entity)
        {
            freeEntities.Add(entity.id);
            aliveEntitiesCount--;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetTotalEntitiesCount()
        {
            return totalEntitiesCount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAliveEntntiesCount() {
            return aliveEntitiesCount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pool<T> GetPool<T>()  where T: struct
        {
            var typeID = ComponentType<T>.ID;
            if (ComponentPools.Length <= typeID)
            {
                var length = ComponentPools.Length << 1;
                while (length <= typeID)
                    length <<= 1;
                Array.Resize(ref ComponentPools, length);
            }
            var pool = (Pool<T>)ComponentPools[typeID];

            if (pool == null)
            {
                pool = new Pool<T>(ComponentCacheSize);
                ComponentPools[typeID] = pool;
                poolsAmount++;
            }
            return pool;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IPool GetPoolByID(int typeID)
        {
            if (ComponentPools.Length <= typeID)
            {
                var length = ComponentPools.Length << 1;
                while (length <= typeID + 2)
                    length <<= 1;
                Array.Resize(ref ComponentPools, length);
            }
            var pool = ComponentPools[typeID];
            if (pool == null) {
                var poolType = typeof(Pool<>);
                pool = (IPool) Activator.CreateInstance(poolType.MakeGenericType(ComponentType.GetTypeValue(typeID)), ComponentCacheSize);
                ComponentPools[typeID] = pool;
                poolsAmount++;
            }
            return pool;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IPool[] GetAllPoolsInternal() {
            return ComponentPools;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventBuffer<T> GetBuffer<T>()  where T: struct, IEvent {
            var type = typeof(T);
            if (EventBuffers.TryGetValue(type, out var buffer))
                return (EventBuffer<T>)buffer;
            buffer = new EventBuffer<T>(128);
            EventBuffers.Add(type, buffer);
            return (EventBuffer<T>)buffer;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IEventBuffer GetBuffer(Type type) {
            if (EventBuffers.TryGetValue(type, out var buffer))
                return buffer;
            var bufferType = typeof(EventBuffer<>);
            var fullType = bufferType.MakeGenericType(type);
            buffer = (IEventBuffer) Activator.CreateInstance(fullType, 128);
            EventBuffers.Add(type, buffer);
            return buffer;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Entity GetEntity(int id)
        {
            return ref entities[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAddComponent(int type, in Entity entity)
        {
            for (var i = 0; i < systemsCount; i++)
                systems[i].OnAdd(type, in entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveComponent(int type, in Entity entity)
        {
            for (var i = 0; i < systemsCount; i++)
                systems[i].OnRemove(type, in entity);
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCreateEntityType(EntityType entityType)
        {
            // for (var i = 0; i < entitiesCount; i++)
            // {
            //     ref var entityData = ref entitiesData[i];
            //     if(IsCompile(entityType, entityData))
            //         entityType.Add(entityData.id);
            // }
        }
    }
}
