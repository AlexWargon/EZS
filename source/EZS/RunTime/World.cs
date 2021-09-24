using System.Linq;
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

    public class World
    {
        public readonly int ComponentCacheSize;
        public readonly int EntityCacheSize;
        public readonly int PoolsCacheSize;
        public readonly int EntityTypesCachSize;
        public Entity[] entities;
        public Entities Entities;
        public IPool[] ComponentPools;
        public int entitiesCount;
        public bool Alive;
        private EntityData[] entitiesData;
        private GrowList<int> freeEntities;
        private TypeMap<int, Systems> systems;
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
            systems = new TypeMap<int, Systems>(4);
            Entities = new Entities(this);

            ComponentTypeMap.Init(this);
            Alive = true;
        }

        public void AddSystems(Systems add)
        {
            add.id = systemsCount;
            if (systems.HasKey(add.id)) return;
            systems.Add(add.id, add);
            systemsCount++;
        }

        public GrowList<Systems> GetAllSystems()
        {
            return systems.Values;
        }
        public void Destroy()
        {
            for (var i = 0; i < systems.Count; i++)
            {
                systems[i].Kill();
            }
            OnDestroy?.Invoke();
            OnDestroy = null;
            Array.Clear(entities, 0, entitiesCount);
            Array.Clear(entitiesData, 0, entitiesCount);
            Array.Clear(freeEntities.Items, 0, freeEntitiesCount);
            Array.Clear(ComponentPools, 0, ComponentPools.Length);
            Entities.Clear();
            systems.Clear();
            entitiesCount = 0;
            Alive = false;
        }

        public void SubOnDestoy(Action callback)
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
            var entity = new Entity();
            entity.world = this;
            if (freeEntities.Count == 0)
            {
                if (entitiesData.Length == entitiesCount)
                {
                    // Array.Resize(ref entities, entities.Length << 1);
                    // Array.Resize(ref entitiesData, entitiesData.Length << 1);
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
            for (var index = 0; index < Entities.EntityTypes.Count; index++)
                Entities.EntityTypes.Values.Items[index].Remove(entity);
            for (var index = 0; index < Entities.Withouts.Count; index++)
            {
                var withOut = Entities.Withouts.Values.Items[index];
                for (var i = 0; i < withOut.EntityTypes.Values.Count; i++)
                    withOut.EntityTypes.Values.Items[i].Remove(entity);
            }
            freeEntities.Add(entity.id);
            //entitiesCount--;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntitiesCount()
        {
            return entitiesCount;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Pool<T> GetPool<T>()
        {
            var typeIdx = ComponentType<T>.ID;
            if (ComponentPools.Length <= typeIdx)
            {
                var length = ComponentPools.Length << 1;
                while (length <= typeIdx)
                    length <<= 1;
                Array.Resize(ref ComponentPools, length);
            }
            var pool = (Pool<T>)ComponentPools[typeIdx];

            if (pool == null)
            {
                pool = new Pool<T>(ComponentCacheSize);
                ComponentPools[typeIdx] = pool;
            }
            return pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity GetEntity(int id)
        {
            return entities[id];
        }
        
        public Entity GetEntityIn(int id)
        {
            return entities[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAddComponent(int type)
        {
            for (var i = 0; i < systems.Count; i++)
                systems[i].OnAdd(type);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveComponent(int type)
        {
            for (var i = 0; i < systems.Count; i++)
                systems[i].OnRemove(type);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnDeActivateEntity(in Entity entity, EntityData data)
        {
            for (int index = 0, iMax = Entities.EntityTypes.Values.Count; index < iMax; index++)
            {
                var entityType = Entities.EntityTypes.Values[index];
                    entityType.Remove(entity);
            }
            
            for (int index = 0, indexMax = Entities.Withouts.Values.Count; index < indexMax; index++)
            {
                var withOut = Entities.Withouts.Values[index];
                for (int i = 0, iMax = withOut.EntityTypes.Values.Count; i < iMax; i++)
                {
                    var entityType = withOut.EntityTypes.Values.Items[i];
                    entityType.Remove(entity);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnActivateEntity(in Entity entity, EntityData data)
        {
            for (int index = 0, iMax = Entities.EntityTypes.Values.Count; index < iMax; index++)
            {
                var entityType = Entities.EntityTypes.Values[index];
                if (IsCompileByInclude(entityType, data))
                    entityType.Add(entity);
            }

            for (int index = 0, iMax = Entities.Withouts.Values.Count; index < iMax; index++)
            {
                var withOut = Entities.Withouts.Values[index];
                for (var i = 0; i < withOut.EntityTypes.Values.Count; i++)
                {
                    var entityType = withOut.EntityTypes.Values.Items[i];
                    if (IsCompile(entityType, data))
                        entityType.Add(entity);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnCreateEntityType(EntityType entityType)
        {
            for (var i = 0; i < entitiesCount; i++)
            {
                ref var entitiyData = ref entitiesData[i];
                if(IsCompile(entityType, entitiyData))
                    entityType.Add(entitiyData.id);
            }
        }
        
        private int solves = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCompile(EntityType entityType, in EntityData data)
        {
            solves = 0;
            for (int i = 0, iMax = entityType.ExludeCount; i < iMax; i++)
                if(data.componentTypes.Contains(entityType.ExcludeTypes[i]))
                    return false;

            for (int i = 0, iMax = entityType.IncludCount; i < iMax; i++)
            {
                if (data.componentTypes.Contains(entityType.IncludTypes[i]))
                {
                    solves++;
                    if (solves == entityType.IncludCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCompileByInclude(EntityType entityType, in EntityData data)
        {
            solves = 0;
            for (int i = 0, iMax = entityType.IncludCount; i < iMax; i++)
            {
                if (data.componentTypes.Contains(entityType.IncludTypes[i]))
                {
                    solves++;
                    if (solves == entityType.IncludCount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
