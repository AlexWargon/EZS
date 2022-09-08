
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
    public class World
    {
        public int ID;
        public readonly int ComponentCacheSize;
        public readonly int EntityCacheSize;
        public readonly int PoolsCacheSize;
        public readonly int EntityTypesCachSize;
        public Entity[] entities;
        public Entities Entities;
        internal readonly fEntities fEntities;
        private IPool[] ComponentPools;

        private int poolsAmount;
        public int entitiesCount;
        public bool Alive;
        private EntityData[] entitiesData;
        private GrowList<int> freeEntities;
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
            freeEntities = new GrowList<int>(EntityCacheSize);
            systems = new Systems[4];
            Entities = new Entities(this);
            fEntities = new fEntities(this);
            Alive = true;
        }

        // public void AddDirtyEntityType(EntityType added) {
        //     dirtyEntityTypes[dirtyCount] = added;
        //     dirtyCount++;
        // }
        //
        // public void UpdateEntityTypes() {
        //     for (var i = 0; i < dirtyCount; i++) {
        //         dirtyEntityTypes[i]
        //     }
        // }
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
        public void Destroy()
        {
            for (var i = 0; i < systemsCount; i++)
            {
                systems[i].Destroy();
            }
            OnDestroy?.Invoke();
            OnDestroy = null;
            Array.Clear(entities, 0, entitiesCount);
            Array.Clear(entitiesData, 0, entitiesCount);
            Array.Clear(freeEntities.Items, 0, freeEntitiesCount);
            Array.Clear(ComponentPools, 0, ComponentPools.Length);
            Entities.Clear();
            Array.Clear(systems,0, systemsCount);
            entitiesCount = 0;
            Alive = false;
        }
        
        public void SubOnDestroy(Action callback)
        {
            OnDestroy += callback;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref EntityData GetEntityData(int entityId)
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
                if (entitiesData.Length == entitiesCount)
                {
                    Array.Resize(ref entities, entities.Length + 256);
                    Array.Resize(ref entitiesData, entitiesData.Length + 256);
                }
                
                entity.id = entitiesCount;
                ref var data = ref entitiesData[entity.id];
                data.generation = 1;
                entity.generation = data.generation;
                
                entities[entity.id] = entity;
                data.componentTypes = new HashSet<int>();
                data.componentsCount = 0;
                data.id = entity.id;
                //data.componentTypes = new int[EntityComponentCount];
                entitiesCount++;
            }
            else
            {
                entity.id = freeEntities.Items[--freeEntities.Count];
                entity.generation = entitiesData[entity.id].generation;
                entitiesData[entity.id].componentsCount = 0;
                entities[entity.id] = entity;
            }
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnDestroyEntity(in Entity entity)
        {
            freeEntities.Add(entity.id);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntitiesCount()
        {
            return entitiesCount;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pool<T> GetPool<T>()  where T: new()
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
                pool = (IPool) Activator.CreateInstance(poolType.MakeGenericType(ComponentTypeMap.GetTypeByID(typeID)), ComponentCacheSize);
                ComponentPools[typeID] = pool;
                poolsAmount++;
                return pool;
            }

            return pool;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IPool[] GetAllPoolsInternal() {
            return ComponentPools;
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
            for (var i = 0; i < entitiesCount; i++)
            {
                ref var entityData = ref entitiesData[i];
                if(IsCompile(entityType, entityData))
                    entityType.Add(entityData.id);
            }
        }
        
        private int solves = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCompile(EntityType entityType, in EntityData data)
        {
            solves = 0;
            for (int i = 0, iMax = entityType.ExcludeCount; i < iMax; i++)
                if(data.componentTypes.Contains(entityType.ExcludeTypes[i]))
                    return false;

            for (int i = 0, iMax = entityType.IncludeCount; i < iMax; i++)
            {
                if (data.componentTypes.Contains(entityType.IncludeTypes[i]))
                {
                    solves++;
                    if (solves == entityType.IncludeCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
