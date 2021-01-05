using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public abstract class EntityType
    {
        protected int[] entities;
        protected Dictionary<int, int> entitiesMap;
        public int[] IncludTypes;
        public int[] ExcludeTypes;
        public int IncludCount;
        public int ExludeCount;
        public int Count;
        public Type Type;
        protected World world;
        internal EntityType(World world)
        {
            this.world = world;
            Count = 0;
            entities = new int[world.EntityCachSize];
            entitiesMap = new Dictionary<int, int>();
            Type = GetType();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(in Entity entity)
        {
            if (HasEntity(entity)) return;
            if (entities.Length == Count)
            {
                Array.Resize(ref entities, entities.Length << 1);
            }
            entities[Count] = entity.id;
            entitiesMap[entity.id] = Count;
            Count++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(in Entity entity)
        {
            if (!HasEntity(entity)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[entity.id];
            entitiesMap.Remove(entity.id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasType<A>()
        {
            var typeID = ComponentType<A>.ID;
            for (int i = 0, iMax = IncludTypes.Length; i < iMax; i++)
                if (typeID == IncludTypes[i])
                    return true;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasType(int typeID)
        {
            for (int i = 0, iMax = IncludTypes.Length; i < iMax; i++)
                if (typeID == IncludTypes[i])
                    return true;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasExcludeType<A>()
        {
            var typeID = ComponentType<A>.ID;
            for (var i = 0; i < ExcludeTypes.Length; i++)
                if (typeID == ExcludeTypes[i])
                    return true;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasExcludeType(int typeID)
        {
            for (var i = 0; i < ExcludeTypes.Length; i++)
                if (typeID == ExcludeTypes[i])
                    return true;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref Entity GetEntityByIndex(int index)
        {
            return ref world.GetEntity(entities[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasEntity(in Entity entity)
        {
            return entitiesMap.ContainsKey(entity.id);
        }

        internal ref EntityData GetEnitiyDataByIndex(int index)
        {
            return ref world.GetEntityData(entities[index]);
        }
    }

    public class EntityType<A> : EntityType
    {
        private Pool<A> pool;
        public EntityType(World world) : base(world)
        {
            IncludTypes = new[] {
                ComponentType<A>.ID
            };
            IncludCount = 1;
            pool = world.GetPool<A>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A Get(int entityID)
        {
            return pool.Items[entityID];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetByIndexA(int index)
        {
            return ref pool.Items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID
                };
            }
        }
        public class WithOut<NA, NB> : EntityType<A>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
            }
        }
    }

    public class EntityType<A, B> : EntityType
    {
        private Pool<A> poolA;
        private Pool<B> poolB;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[] {
                ComponentType<A>.ID,
                ComponentType<B>.ID
            };
            IncludCount = 2;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();

        }

        public void ForEach<A, B>(Entities.Lambda<A, B> lambda)
        {

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetA(int entityID)
        {
            return ref poolA.Items[entityID];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetB(int entityID)
        {
            return ref poolB.Items[entityID];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetByIndexA(int index)
        {
            return ref poolA.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetByIndexB(int index)
        {
            return ref poolB.Items[entities[index]];
        }
        public class WithOut<NA> : EntityType<A, B>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
            }
        }
        public class WithOut<NA, NB> : EntityType<A, B>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
            }
        }
    }

    public class EntityType<A, B, C> : EntityType
    {
        private Pool<A> poolA;
        private Pool<B> poolB;
        private Pool<C> poolС;
        public EntityType(World world) : base(world)
        {
            IncludTypes = new[] {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID
            };
            IncludCount = 3;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            poolС = world.GetPool<C>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetByIndexA(int index)
        {
            return ref poolA.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetByIndexB(int index)
        {
            return ref poolB.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetByIndexC(int index)
        {
            return ref poolС.Items[entities[index]];
        }
        public class Without<NA> : EntityType<A, B, C>
        {
            public Without(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID
                };
            }
        }
        public class Without<NA, NB> : EntityType<A, B, C>
        {
            public Without(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
            }
        }
    }
    public class EntityType<A, B, C, D> : EntityType
    {
        private Pool<A> poolA;
        private Pool<B> poolB;
        private Pool<C> poolС;
        private Pool<D> poolD;
        public EntityType(World world) : base(world)
        {
            IncludTypes = new int[] {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID
            };
            IncludCount = 4;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            poolС = world.GetPool<C>();
            poolD = world.GetPool<D>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetByIndexA(int index)
        {
            return ref poolA.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetByIndexB(int index)
        {
            return ref poolB.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetByIndexC(int index)
        {
            return ref poolС.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref D GetByIndexD(int index)
        {
            return ref poolD.Items[entities[index]];
        }
        public class WithOut<NA> : EntityType<A, B, C, D>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID
                };
            }
        }
        public class WithOut<NA, NB> : EntityType<A, B, C, D>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
            }
        }
    }

    public class EntityType<A, B, C, D, E> : EntityType
    {
        private Pool<A> poolA;
        private Pool<B> poolB;
        private Pool<C> poolС;
        private Pool<D> poolD;
        private Pool<E> poolE;
        public EntityType(World world) : base(world)
        {
            IncludTypes = new[] {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID,
                ComponentType<E>.ID
            };
            IncludCount = 5;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            poolС = world.GetPool<C>();
            poolD = world.GetPool<D>();
            poolE = world.GetPool<E>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetByIndexA(int index)
        {
            return ref poolA.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetByIndexB(int index)
        {
            return ref poolB.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetByIndexC(int index)
        {
            return ref poolС.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref D GetByIndexD(int index)
        {
            return ref poolD.Items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref E GetByIndexE(int index)
        {
            return ref poolE.Items[entities[index]];
        }
        public class WithOut<NA> : EntityType<A, B, C, D, E>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID
                };
            }
        }
        public class WithOut<NA, NB> : EntityType<A, B, C, D, E>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
            }
        }
    }
}
