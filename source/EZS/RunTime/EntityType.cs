using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public struct Enumerator : IDisposable
    {
        private readonly int count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(EntityType entityType)
        {
            count = entityType.Count;
            Current = -1;
        }

        public int Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Current = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++Current < count;
        }
    }

    public abstract class EntityType
    {
        internal readonly Dictionary<int, int> entitiesMap;
        private readonly World world;
        private int[] addedLocked;
        public int Count;
        internal int[] entities;
        
        public int[] ExcludeTypes;
        public int ExludeCount;
        public int IncludCount;
        public int[] IncludTypes;

        private int[] removedLocked;


        private SparseSet sparseSet;
        public EntityType(World world)
        {
            this.world = world;
            Count = 0;
            sparseSet = new SparseSet(world.EntityTypesCachSize);
            entities = new int[world.EntityCacheSize];
            entitiesMap = new Dictionary<int, int>();
        }

        public int[] GetIDQuery()
        {
            var result = new int[Count];
            Array.Copy(entities, 0, result, 0, Count);
            return result;
        }

        public Entity[] GetEntityQuery()
        {
            var result = new Entity[Count];
            for (var i = 0; i < result.Length; i++) result[i] = world.GetEntity(entities[i]);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAddInclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;

            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            Count++;
            sparseSet.Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveInclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
            sparseSet.Remove(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAddExclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
            sparseSet.Remove(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveExclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;
            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            Count++;
            sparseSet.Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(Entity entity)
        {
            if (entitiesMap.ContainsKey(entity.id)) return;
            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = entity.id;
            entitiesMap.Add(entity.id, Count);
            Count++;
            sparseSet.Add(entity.id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(int id)
        {
            if (entitiesMap.ContainsKey(id)) return;
            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            Count++;
            sparseSet.Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void Remove(Entity entity)
        {
            if (!entitiesMap.ContainsKey(entity.id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[entity.id];
            entitiesMap.Remove(entity.id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
            sparseSet.Remove(entity.id);
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
        public Entity GetEntity(int index)
        {
            return world.GetEntity(entities[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasEntity(int id)
        {
            return entitiesMap.ContainsKey(id);
        }

        internal ref EntityData GetEnitiyDataByIndex(int index)
        {
            return ref world.GetEntityData(entities[index]);
        }

        internal virtual void Clear()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return Count < 1;
        }
        public class WithOut<NA> : EntityType
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A> : EntityType
    {
        internal readonly Pool<A> poolA;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
                ComponentType<A>.ID
            };
            IncludCount = 1;
            poolA = world.GetPool<A>();
            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B> : EntityType
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID
            };
            IncludCount = 2;

            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();

            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B GetB(int index)
        {
            return poolB.items[entities[index]];
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
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C> : EntityType
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID
            };
            IncludCount = 3;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            poolС = world.GetPool<C>();

            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
            poolС.OnAdd += OnAddInclude;
            poolС.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal B GetB(int index)
        {
            return poolB.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C GetC(int index)
        {
            return poolС.items[entities[index]];
        }

        public class Without<NA> : EntityType<A, B, C>
        {
            public Without(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class Without<NA, NB> : EntityType<A, B, C>
        {
            public Without(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D> : EntityType
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
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

            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
            poolС.OnAdd += OnAddInclude;
            poolС.OnRemove += OnRemoveInclude;
            poolD.OnAdd += OnAddInclude;
            poolD.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal B GetB(int index)
        {
            return poolB.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C GetC(int index)
        {
            return poolС.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal D GetD(int index)
        {
            return poolD.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A, B, C, D>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D, E> : EntityType
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<E> poolE;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
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

            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
            poolС.OnAdd += OnAddInclude;
            poolС.OnRemove += OnRemoveInclude;
            poolD.OnAdd += OnAddInclude;
            poolD.OnRemove += OnRemoveInclude;
            poolE.OnAdd += OnAddInclude;
            poolE.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal B GetB(int index)
        {
            return poolB.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C GetC(int index)
        {
            return poolС.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal D GetD(int index)
        {
            return poolD.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal E GetE(int index)
        {
            return poolE.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A, B, C, D, E>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D, E>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D, E, F> : EntityType
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<E> poolE;
        internal readonly Pool<F> poolF;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
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

            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
            poolС.OnAdd += OnAddInclude;
            poolС.OnRemove += OnRemoveInclude;
            poolD.OnAdd += OnAddInclude;
            poolD.OnRemove += OnRemoveInclude;
            poolE.OnAdd += OnAddInclude;
            poolE.OnRemove += OnRemoveInclude;
            poolF.OnAdd += OnAddInclude;
            poolF.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal B GetB(int index)
        {
            return poolB.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C GetC(int index)
        {
            return poolС.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal D GetD(int index)
        {
            return poolD.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal E GetE(int index)
        {
            return poolE.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal F GetF(int index)
        {
            return poolF.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A, B, C, D, E, F>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D, E, F>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D, E, F, G> : EntityType
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<E> poolE;
        internal readonly Pool<F> poolF;
        internal readonly Pool<G> poolG;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID,
                ComponentType<E>.ID,
                ComponentType<F>.ID,
                ComponentType<G>.ID
            };
            IncludCount = 6;
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            poolС = world.GetPool<C>();
            poolD = world.GetPool<D>();
            poolE = world.GetPool<E>();
            poolF = world.GetPool<F>();
            poolG = world.GetPool<G>();


            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
            poolС.OnAdd += OnAddInclude;
            poolС.OnRemove += OnRemoveInclude;
            poolD.OnAdd += OnAddInclude;
            poolD.OnRemove += OnRemoveInclude;
            poolE.OnAdd += OnAddInclude;
            poolE.OnRemove += OnRemoveInclude;
            poolF.OnAdd += OnAddInclude;
            poolF.OnRemove += OnRemoveInclude;
            poolG.OnAdd += OnAddInclude;
            poolG.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal B GetB(int index)
        {
            return poolB.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal C GetC(int index)
        {
            return poolС.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal D GetD(int index)
        {
            return poolD.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal E GetE(int index)
        {
            return poolE.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal F GetF(int index)
        {
            return poolF.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal G GetG(int index)
        {
            return poolG.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A, B, C, D, E, F, G>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D, E, F, G>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                var pool1 = world.GetPool<NA>();
                pool1.OnAdd += OnAddExclude;
                pool1.OnRemove += OnRemoveExclude;
                var pool2 = world.GetPool<NB>();
                pool2.OnAdd += OnAddExclude;
                pool2.OnRemove += OnRemoveExclude;
            }
        }
    }
    public struct SparseSet : IEnumerable<int>
    {
        private int _max;      // maximal value the set can contain
                               // _max = 100; implies a range of [0..99]
        private int _n;                 // current size of the set
        public int[] entities;      // dense array
        private int[] target;      // sparse array

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseSet"/> class.
        /// </summary>
        /// <param name="maxValue">The maximal value the set can contain.</param>
        public SparseSet(int maxValue)
        {
            _max = maxValue + 1;
            _n = 0;
            entities = new int[_max];
            target = new int[_max];
        }

        /// <summary>
        /// Adds the given value.
        /// If the value already exists in the set it will be ignored.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(int value)
        {

            if(value > _max)
            {
                _max = value + 16;
                Array.Resize(ref entities, _max);
                Array.Resize(ref target, _max);
            }

            if(value >= 0 && value < _max && !Contains(value))
            {
                entities[_n] = value;     // insert new value in the dense array...
                target[value] = _n;     // ...and link it to the sparse array
                _n++;
            }
        }

        /// <summary>
        /// Removes the given value in case it exists.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(int value)
        {
            if(Contains(value))
            {
                entities[target[value]] = entities[_n - 1];     // put the value at the end of the dense array
                                                // into the slot of the removed value
                target[entities[_n - 1]] = target[value];     // put the link to the removed value in the slot
                                                // of the replaced value
                _n--;
            }
        }

        /// <summary>
        /// Determines whether the set contains the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the set contains the given value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int value)
        {
            if(value >= _max || value < 0)
                return false;
            else
                return target[value] < _n && entities[target[value]] == value;    // value must meet two conditions:
                                                                    // 1. link value from the sparse array
                                                                    // must point to the current used range
                                                                    // in the dense array
                                                                    // 2. there must be a valid two-way link
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public void Clear()
        {
            _n = 0;     // simply set n to 0 to clear the set; no re-initialization is required
        }

        /// <summary>
        /// Gets the number of elements in the set.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all elements in the set.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<int> GetEnumerator()
        {
            var i = 0;
            while(i < _n)
            {
                yield return entities[i];
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}