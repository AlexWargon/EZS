namespace Wargon.ezs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
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
        internal readonly Dictionary<int, int> entitiesMap;
        public int[] IncludTypes;
        public int[] ExcludeTypes;
        public int IncludCount;
        public int ExludeCount;
        public int Count;
        private readonly World world;
        
        internal EntityType(World world)
        {
            this.world = world;
            Count = 0;
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
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = world.GetEntity(entities[i]);
            }
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAddInclude(Entity entity)
        {
            if (HasEntity(entity)) return;
            ref var data = ref entity.GetEntityData();

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;

            for (var i = 0; i < IncludCount; i++)
                if(!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count)
            {
                Array.Resize(ref entities, entities.Length << 1);
            }
            entities[Count] = entity.id;
            entitiesMap.Add(entity.id, Count);
            Count++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemoveInclude(Entity entity)
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
        internal void OnAddExclude(Entity entity)
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
        internal void OnRemoveExclude(Entity entity)
        {
            if (HasEntity(entity)) return;
            ref var data = ref entity.GetEntityData();

            for (var i = 0; i < ExludeCount; i++)
                if(data.componentTypes.Contains(ExcludeTypes[i]))
                    return;
            for (var i = 0; i < IncludCount; i++)
                if(!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count)
            {
                Array.Resize(ref entities, entities.Length << 1);
            }
            entities[Count] = entity.id;
            entitiesMap.Add(entity.id, Count);
            Count++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(Entity entity)
        {
            if (entitiesMap.ContainsKey(entity.id)) return;
            if (entities.Length == Count)
            {
                Array.Resize(ref entities, entities.Length << 1);
            }
            entities[Count] = entity.id;
            entitiesMap.Add(entity.id, Count);
            Count++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(int id)
        {
            if (entitiesMap.ContainsKey(id)) return;
            if (entities.Length == Count)
            {
                Array.Resize(ref entities, entities.Length << 1);
            }
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            Count++;
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
        internal Entity GetEntity(int index)
        {
            return world.GetEntity(entities[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasEntity(Entity entity)
        {
            return entitiesMap.ContainsKey(entity.id);
        }

        internal ref EntityData GetEnitiyDataByIndex(int index)
        {
            return ref world.GetEntityData(entities[index]);
        }

        internal virtual void Clear()
        {

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
            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A>
        {
            public WithOut(World world) : base(world)
            {
                ExludeCount = 1;
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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

            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
        internal Pool<A> poolA;
        internal Pool<B> poolB;
        internal Pool<C> poolС;
        internal Pool<D> poolD;
        internal Pool<E> poolE;
        internal Pool<F> poolF;
        internal Pool<G> poolG;
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
                ExcludeTypes = new[] {
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
                ExcludeTypes = new[] {
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
}
