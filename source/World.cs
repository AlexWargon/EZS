using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public class Configs
    {
        public static int ComponentCachSize = 1024;
        public static int EntityCachSize = 1024;
        public static int PoolsCachSize = 128;
        public static int EntityTypesCachSize = 256;
    }
    public class World
    {
        public readonly int ComponentCachSize;
        public readonly int EntityCachSize;
        public readonly int PoolsCachSize;
        public readonly int EntityTypesCachSize;
        public Entity[] entities;
        private EntityData[] entityData;
        private GrowList<int> freeEntities;
        public Entities Entities;

        private TypeMap<int, Systems> systems;
        private int systemsCount;
        /// <summary>
        /// public for unity extension
        /// </summary>
        public IPool[] ComponentPools;
        private int entitiesCount;
        private int freeEntitiesCount = 0;
        internal int usedComponentsCount;
        public bool Alive;
        public int GetFreeEntitiesCount() => freeEntities.Count;
        public World()
        {
            ComponentCachSize = Configs.ComponentCachSize;
            EntityCachSize = Configs.EntityCachSize;
            PoolsCachSize = Configs.PoolsCachSize;
            EntityTypesCachSize = Configs.EntityTypesCachSize;
            usedComponentsCount = 0;
            ComponentPools = new IPool[PoolsCachSize];
            entityData = new EntityData[EntityCachSize];
            entities = new Entity[EntityCachSize];
            freeEntities = new GrowList<int>(EntityCachSize);
            systems = new TypeMap<int, Systems>(4);
            Entities = new Entities(this);
            for (var index = 0; index < entities.Length; index++)
            {
                var entity = entities[index];
                entity.world = this;
            }
            ComponentTypeMap.World = this;
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
            Array.Clear(entities, 0, entitiesCount);
            Array.Clear(entityData, 0, entitiesCount);
            Array.Clear(freeEntities.Items, 0, freeEntitiesCount);
            Array.Clear(ComponentPools, 0, ComponentPools.Length);
            systems.Clear();
            entitiesCount = 0;
            Alive = false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref EntityData GetEntityData(int entityId)
        {
            return ref entityData[entityId];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity()
        {
            Entity entity;
            entity.world = this;
            if (freeEntities.Count == 0)
            {
                if (entityData.Length == entitiesCount)
                {
                    Array.Resize(ref entities, entities.Length << 1);
                    Array.Resize(ref entityData, entityData.Length << 1);
                }
                entity.id = entitiesCount;
                ref var data = ref entityData[entity.id];
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
                entity.generation = entityData[entity.id].generation;
                entityData[entity.id].componentsCount = 0;
                
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
            if (ComponentPools.Length < typeIdx)
            {
                var length = ComponentPools.Length << 1;
                while (length <= typeIdx)
                    length <<= 1;
                Array.Resize(ref ComponentPools, length);
            }
            var pool = (Pool<T>)ComponentPools[typeIdx];
            if (pool == null)
            {
                pool = new Pool<T>(ComponentCachSize);
                ComponentPools[typeIdx] = pool;
                usedComponentsCount++;
            }
            return pool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Entity GetEntity(int id)
        {
            return ref entities[id];
        }
        public Entity GetEntityIn(int id)
        {
            return entities[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAddComponent(in Entity entity, ref EntityData data, int typeId)
        {
            for (int index = 0, iMax = Entities.EntityTypes.Values.Count; index < iMax; index++)
            {
                var entityType = Entities.EntityTypes.Values[index];
                if (IsCompileByInclude(entityType, data))
                    entityType.Add(entity);
            }

            for (int index = 0, indexMax = Entities.Withouts.Values.Count; index < indexMax; index++)
            {
                var withOut = Entities.Withouts.Values[index];
                for (int i = 0, iMax = withOut.EntityTypes.Values.Count; i < iMax; i++)
                {
                    var entityType = withOut.EntityTypes.Values.Items[i];
                    if (IsCompile(entityType, data))
                        entityType.Add(entity);
                    else
                        entityType.Remove(entity);
                }
            }
            for (var i = 0; i < systemsCount; i++)
                systems[i].OnAdd(typeId);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveComponent<A>(in Entity entity, EntityData data)
        {
            for (int index = 0, iMax = Entities.EntityTypes.Values.Count; index < iMax; index++)
            {
                var entityType = Entities.EntityTypes.Values[index];
                if (!IsCompileByInclude(entityType, data))
                    entityType.Remove(entity);
            }
            
            for (int index = 0, indexMax = Entities.Withouts.Values.Count; index < indexMax; index++)
            {
                var withOut = Entities.Withouts.Values[index];
                for (int i = 0, iMax = withOut.EntityTypes.Values.Count; i < iMax; i++)
                {
                    var entityType = withOut.EntityTypes.Values.Items[i];
                    if (IsCompile(entityType, data))
                        entityType.Add(entity);
                    else
                        entityType.Remove(entity);
                }
            }
            for (var i = 0; i < systemsCount; i++)
                systems[i].OnRemove(ComponentType<A>.ID);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveComponent(in Entity entity, in EntityData data, int typeID)
        {
            for (int index = 0, iMax = Entities.EntityTypes.Values.Count; index < iMax; index++)
            {
                var entityType = Entities.EntityTypes.Values[index];
                if (!IsCompileByInclude(entityType, data))
                    entityType.Remove(entity);
            }

            for (int index = 0, iMax = Entities.Withouts.Values.Count; index < iMax; index++)
            {
                var withOut = Entities.Withouts.Values[index];
                for (var i = 0; i < withOut.EntityTypes.Values.Count; i++)
                {
                    var entityType = withOut.EntityTypes.Values.Items[i];
                    if (IsCompile(entityType, data))
                        entityType.Add(entity);
                    else
                        entityType.Remove(entity);
                }
            }
            for (var i = 0; i < systemsCount; i++)
                systems[i].OnRemove(typeID);
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
        private int solves = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCompile(EntityType entityType, EntityData data)
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
        private bool IsCompileByInclude(EntityType entityType, EntityData data)
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
