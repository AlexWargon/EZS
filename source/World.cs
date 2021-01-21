using System;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public class Configs
    {
        public static int ComponentCachSize = 1024;
        public static int EntityCachSize = 1024;
        public static int PoolsCachSize = 128;
        public static int EntityTypesCachSize = 128;
        public static int EntityComponentCount = 24;
    }
    public class World
    {
        public readonly int ComponentCachSize;
        public readonly int EntityCachSize;
        public readonly int PoolsCachSize;
        public readonly int EntityTypesCachSize;
        public readonly int EntityComponentCount;
        private Entity[] entities;
        private EntityData[] entityData;
        private GrowList<int> freeEntities;
        internal Entities Entities;

        private TypeMap<int, Systems> systems;
        private int systemsCount;
        /// <summary>
        /// public for unity extension
        /// </summary>
        public IPool[] ComponentPools;
        private int entitiesCount;
        private int freeEntitiesCount = 0;
        private int usedComponentsCount;
        public bool Alive;
        public World()
        {
            ComponentCachSize = Configs.ComponentCachSize;
            EntityCachSize = Configs.EntityCachSize;
            PoolsCachSize = Configs.PoolsCachSize;
            EntityTypesCachSize = Configs.EntityTypesCachSize;
            EntityComponentCount = Configs.EntityComponentCount;
            usedComponentsCount = 0;
            ComponentPools = new IPool[PoolsCachSize];
            entityData = new EntityData[EntityCachSize];
            entities = new Entity[EntityCachSize];
            freeEntities = new GrowList<int>(EntityCachSize);
            systems = new TypeMap<int, Systems>(12);
            Entities = new Entities(this);
            Alive = true;
        }

        public void AddSystems(Systems add)
        {
            if (systems.HasKey(add.id)) return;
            add.id = systemsCount;
            systems.Add(add.id, add);
            systemsCount++;
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
            entity.World = this;
            if (freeEntities.Count == 0)
            {
                if (entities.Length == entitiesCount)
                {
                    Array.Resize(ref entities, entities.Length << 1);
                    Array.Resize(ref entityData, entityData.Length << 1);
                }
                entity.id = entitiesCount;
                entityData[entity.id].Generation = 1;
                entity.Generation = entityData[entity.id].Generation;
                entities[entitiesCount] = entity;
                entityData[entity.id].id = entity.id;
                entityData[entity.id].ComponentsCount = 0;
                entityData[entity.id].ComponentTypes = new int[EntityComponentCount];
                entitiesCount++;
            }
            else
            {
                entity.id = freeEntities.Items[--freeEntities.Count];
                entity.Generation = entityData[entity.id].Generation;
                entityData[entity.id].ComponentsCount = 0;
            }
            return entity;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnDestroyEntity(in Entity entity)
        {
            for (var index = 0; index < Entities.EntityTypes.Count; index++)
            {
                var entityType = Entities.EntityTypes.Values.Items[index];
                entityType.Remove(entity);
            }
            
            for (var index = 0; index < Entities.Withouts.Count; index++)
            {
                var withOut = Entities.Withouts.Values.Items[index];
                for (var i = 0; i < withOut.EntityTypes.Values.Count; i++)
                {
                    var entityType = withOut.EntityTypes.Values.Items[i];
                    entityType.Remove(entity);
                }
            }
            entitiesCount--;
            freeEntities.Add(entity.id);
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
        public IPool GetPoolById(int typeIdx)
        {
            if (ComponentPools.Length < typeIdx)
            {
                var len = ComponentPools.Length << 1;
                while (len <= typeIdx)
                {
                    len <<= 1;
                }
                Array.Resize(ref ComponentPools, len);
            }
            var pool = ComponentPools[typeIdx];
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

        private int solves = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCompile(EntityType entityType, EntityData data)
        {
            solves = 0;
            for (int i = 0, iMax = entityType.ExludeCount; i < iMax; i++)
                for (var j = 0; j < data.ComponentsCount; j++)
                    if (entityType.ExcludeTypes[i] == data.ComponentTypes[j])
                        return false;

            for (int i = 0, iMax = entityType.IncludCount; i < iMax; i++)
            {
                for (var j = 0; j < data.ComponentsCount; j++)
                {
                    if (entityType.IncludTypes[i] != data.ComponentTypes[j]) continue;
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
                for (var j = 0; j < data.ComponentsCount; j++)
                {
                    if (entityType.IncludTypes[i] != data.ComponentTypes[j]) continue;
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
