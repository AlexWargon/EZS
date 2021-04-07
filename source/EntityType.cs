using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public struct Enumerator : IDisposable {
        
        readonly int count;
        int index;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        internal Enumerator (EntityType entityType) {

            count = entityType.Count;
            index = -1;
        }
        public int Current {
            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            get => index;
        }
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void Dispose () {
            
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool MoveNext () {
            return ++index < count;
        }
    }

    public abstract class EntityType
    {
        internal int[] entities;
        private readonly Dictionary<int, int> entitiesMap;
        public int[] IncludTypes;
        public int[] ExcludeTypes;
        public int IncludCount;
        public int ExludeCount;
        public int Count;
        public readonly Type Type;
        private readonly World world;
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
            entitiesMap.Add(entity.id, Count);
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
            foreach (var t in ExcludeTypes)
                if (typeID == t)
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
        internal ref Entity GetEntity(int index)
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
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator () {
            return new Enumerator (this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => Count < 1;
    }

    public class EntityType<A> : EntityType
    {
        internal Pool<A> poolA;
        public EntityType(World world) : base(world)
        {
            IncludTypes = new[] {
                ComponentType<A>.ID
            };
            IncludCount = 1;
            poolA = world.GetPool<A>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
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
        internal Pool<A> poolA;
        internal Pool<B> poolB;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetB(int index)
        {
            return ref poolB.items[entities[index]];
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
        internal Pool<A> poolA;
        internal Pool<B> poolB;
        internal Pool<C> poolС;
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
        internal ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetB(int index)
        {
            return ref poolB.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetC(int index)
        {
            return ref poolС.items[entities[index]];
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
        internal Pool<A> poolA;
        internal Pool<B> poolB;
        internal Pool<C> poolС;
        internal Pool<D> poolD;
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
        internal ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetB(int index)
        {
            return ref poolB.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetC(int index)
        {
            return ref poolС.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref D GetD(int index)
        {
            return ref poolD.items[entities[index]];
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
        internal Pool<A> poolA;
        internal Pool<B> poolB;
        internal Pool<C> poolС;
        internal Pool<D> poolD;
        internal Pool<E> poolE;
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
        internal ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetB(int index)
        {
            return ref poolB.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetC(int index)
        {
            return ref poolС.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref D GetD(int index)
        {
            return ref poolD.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref E GetE(int index)
        {
            return ref poolE.items[entities[index]];
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
    public class EntityType<A, B, C, D, E, F> : EntityType
    {
        internal Pool<A> poolA;
        internal Pool<B> poolB;
        internal Pool<C> poolС;
        internal Pool<D> poolD;
        internal Pool<E> poolE;
        internal Pool<F> poolF;
        public EntityType(World world) : base(world)
        {
            IncludTypes = new[] {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID,
                ComponentType<E>.ID,
                ComponentType<F>.ID
            };
            IncludCount = 6;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            poolС = world.GetPool<C>();
            poolD = world.GetPool<D>();
            poolE = world.GetPool<E>();
            poolF = world.GetPool<F>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref B GetB(int index)
        {
            return ref poolB.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref C GetC(int index)
        {
            return ref poolС.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref D GetD(int index)
        {
            return ref poolD.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref E GetE(int index)
        {
            return ref poolE.items[entities[index]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref F GetF(int index)
        {
            return ref poolF.items[entities[index]];
        }
        public class WithOut<NA> : EntityType<A, B, C, D, E, F>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[] {
                    ComponentType<NA>.ID
                };
            }
        }
        public class WithOut<NA, NB> : EntityType<A, B, C, D, E, F>
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
