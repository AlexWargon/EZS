using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

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
        //internal readonly Dictionary<int, int> entitiesMap;

        protected readonly World world;
        public int Count;
        internal int[] entities;
        internal int[] ExcludeTypes;
        internal int[] IncludeTypes;
        internal readonly IntMap entitiesMap;
        internal int ExcludeCount;
        internal int IncludeCount;
        internal int currentIndex;
        public EntityType(World world)
        {
            this.world = world;
            Count = 0;
            entities = new int[world.EntityCacheSize];
            entitiesMap = new IntMap(world.EntityCacheSize);
            IncludeTypes = new int[10];
            ExcludeTypes = new int[6];
        }

        protected void SubWith<T>() where T : struct {
            var type = ComponentType<T>.ID;
            IncludeTypes[IncludeCount] = type;
            var pool1 = world.GetPool<T>();
            pool1.OnAdd += OnAddInclude;
            pool1.OnRemove += OnRemoveInclude;
            IncludeCount++;
        }
        protected void SubWith<T>(Pool<T> reference) where T : struct {
            var type = ComponentType<T>.ID;
            IncludeTypes[IncludeCount] = type;
            var pool1 = world.GetPool<T>();
            reference = pool1;
            reference.OnAdd += OnAddInclude;
            reference.OnRemove += OnRemoveInclude;
            IncludeCount++;
        }
        protected void SubWithout<T>() where T : struct {
            var type = ComponentType<T>.ID;
            ExcludeTypes[ExcludeCount] = type;
            var pool1 = world.GetPool<T>();
            pool1.OnAdd += OnAddInclude;
            pool1.OnRemove += OnRemoveInclude;
            IncludeCount++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity[] GetEntityQuery()
        {
            var result = new Entity[Count];
            for (var i = 0; i < result.Length; i++) result[i] = world.GetEntity(entities[i]);
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnAddIncludeRemoveExclude(int id) {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);
            // for (var i = 0; i < ExcludeCount; i++)
            //     if (data.componentTypes.Contains(ExcludeTypes[i]))
            //         return;
            //
            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnRemoveIncludeAddExclude(int id) {
            
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnAddInclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);
            // for (var i = 0; i < ExcludeCount; i++)
            //     if (data.componentTypes.Contains(ExcludeTypes[i]))
            //         return;
            //
            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, Count+16);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnRemoveInclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateOnAddWith(ref EntityData data) {
            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, Count+16);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateOnRemoveWith(int id) {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void UpdateOnAddWithout(int id) {

            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void UpdateOnRemoveWithout(in EntityData data) {
            if (HasEntity(data.id)) return;

            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnAddExclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnRemoveExclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);
            // for (var i = 0; i < ExcludeCount; i++)
            //     if (data.componentTypes.Contains(ExcludeTypes[i]))
            //         return;
            //
            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void Add(int id)
        {
            if (HasEntity(id)) return;
            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            Count++;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Entity GetEntity(int index)
        {
            return ref world.GetEntity(entities[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasEntity(int id) {
            if (Count < 1) return false;
            return entitiesMap.Has(id);
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
        public class WithOut<NA> : EntityType where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }
    
    public class EntityType<A> : EntityType where A : struct
    {
        internal readonly Pool<A> poolA; 

        public EntityType(World world) : base(world)
        {
            IncludeTypes = new[]
            {
                ComponentType<A>.ID
            };
            IncludeCount = 1;
            poolA = world.GetPool<A>();
            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal A GetA(int index)
        {
            return poolA.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A> where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // pool1.OnAddExclude.Add(this);
                // pool1.OnRemoveExclude.Add(this);
            }
        }

        public class WithOut<NA, NB> : EntityType<A> where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B> : EntityType  where A: struct where B : struct
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;

        public EntityType(World world) : base(world)
        {
            IncludeTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID
            };
            IncludeCount = 2;
            //
            poolA = world.GetPool<A>();
            poolB = world.GetPool<B>();
            
            poolA.OnAdd += OnAddInclude;
            poolA.OnRemove += OnRemoveInclude;
            poolB.OnAdd += OnAddInclude;
            poolB.OnRemove += OnRemoveInclude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A GetA(int index)
        {
            return ref poolA.items[entities[index]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref B GetB(int index)
        {
            return ref poolB.items[entities[index]];
        }

        public class WithOut<NA> : EntityType<A, B> where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B> where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C> : EntityType  where A: struct where B : struct where C : struct
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludeTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID
            };
            IncludeCount = 3;
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

        public class Without<NA> : EntityType<A, B, C> where NA : struct
        {
            public Without(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class Without<NA, NB> : EntityType<A, B, C> where NA : struct where NB : struct
        {
            public Without(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D> : EntityType  where A: struct where B : struct where C : struct where D : struct
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludeTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID
            };
            IncludeCount = 4;
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

        public class WithOut<NA> : EntityType<A, B, C, D> where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D> where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D, E> : EntityType where A: struct where B : struct where C : struct where D : struct where  E : struct
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<E> poolE;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludeTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID,
                ComponentType<E>.ID
            };
            IncludeCount = 5;
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

        public class WithOut<NA> : EntityType<A, B, C, D, E> where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D, E> where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D, E, F> : EntityType where A: struct where B : struct where C : struct where D : struct where  E : struct where  F : struct
    {
        internal readonly Pool<A> poolA;
        internal readonly Pool<B> poolB;
        internal readonly Pool<D> poolD;
        internal readonly Pool<E> poolE;
        internal readonly Pool<F> poolF;
        internal readonly Pool<C> poolС;

        public EntityType(World world) : base(world)
        {
            IncludeTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID,
                ComponentType<E>.ID,
                ComponentType<F>.ID
            };
            IncludeCount = 6;
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

        public class WithOut<NA> : EntityType<A, B, C, D, E, F> where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D, E, F> where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;
            }
        }
    }

    public class EntityType<A, B, C, D, E, F, G> : EntityType  where A: struct where B : struct where C : struct where D : struct where  E : struct where  F : struct where G : struct
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
            IncludeTypes = new[]
            {
                ComponentType<A>.ID,
                ComponentType<B>.ID,
                ComponentType<C>.ID,
                ComponentType<D>.ID,
                ComponentType<E>.ID,
                ComponentType<F>.ID,
                ComponentType<G>.ID
            };
            IncludeCount = 7;
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

        public class WithOut<NA> : EntityType<A, B, C, D, E, F, G> where NA : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 1;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
            }
        }

        public class WithOut<NA, NB> : EntityType<A, B, C, D, E, F, G> where NA : struct where NB : struct
        {
            public WithOut(World world) : base(world)
            {
                ExcludeCount = 2;
                ExcludeTypes = new[]
                {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
                // var pool1 = world.GetPool<NA>();
                // pool1.OnAdd += OnAddExclude;
                // pool1.OnRemove += OnRemoveExclude;
                // var pool2 = world.GetPool<NB>();
                // pool2.OnAdd += OnAddExclude;
                // pool2.OnRemove += OnRemoveExclude;

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
    
    public class OwnerQuery : EntityType {
    private int ownerID;
    private readonly Pool<Owner> pool;
    
    public OwnerQuery(World world) : base(world)
    {
        ExcludeCount = 0;
        IncludeCount = 0;
        IncludeTypes = new int[8];
        ExcludeTypes = new int[4];
        pool = world.GetPool<Owner>();
        IncludeTypes[IncludeCount] = pool.TypeID;
        pool.OnAdd += OnAddInclude;
        pool.OnRemove += OnRemoveInclude;
        IncludeCount++;
    }
    public OwnerQuery WithOwner(int id)
    {
        ownerID = id;
        return this;
    }
    public OwnerQuery With<T>() where T : struct
    {
        IncludeTypes[IncludeCount] = ComponentType<T>.ID;
        world.GetPool<T>().OnAdd += OnAddInclude;
        world.GetPool<T>().OnRemove += OnRemoveInclude;
        IncludeCount++;
        return this;
    }
    public OwnerQuery Without<T>() where T : struct
    {
        ExcludeTypes[ExcludeCount] = ComponentType<T>.ID;
        world.GetPool<T>().OnAdd += OnAddExclude;
        world.GetPool<T>().OnRemove += OnRemoveExclude;
        ExcludeCount++;
        return this;
    }
    public Entity GetOwner()
    {
        return world.GetEntity(ownerID);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override void Add(int id)
    {
        if (HasEntity(id)) return;
        if(pool.items[id].Value.id != ownerID) return;
        if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
        entities[Count] = id;
        entitiesMap.Add(id, Count);
        Count++;
    }
    private new void OnAddInclude(int id)
    {
        if (HasEntity(id)) return;
        
        ref var data = ref world.GetEntityData(id);
        // for (var i = 0; i < ExcludeCount; i++)
        //     if (data.componentTypes.Contains(ExcludeTypes[i]))
        //         return;
        //
        // for (var i = 0; i < IncludeCount; i++)
        //     if (!data.componentTypes.Contains(IncludeTypes[i]))
        //         return;
        if(pool.items[id].Value.id != ownerID) return;
        if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
        entities[Count] = data.id;
        entitiesMap.Add(data.id, Count);
        Count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private new void OnRemoveInclude(int id)
    {
        if (!HasEntity(id)) return;
        var lastEntityId = entities[Count - 1];
        var indexOfEntityId = entitiesMap[id];
        entitiesMap.Remove(id);
        Count--;
        if (Count > indexOfEntityId)
        {
            entities[indexOfEntityId] = lastEntityId;
            entitiesMap.Add(lastEntityId, indexOfEntityId);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private new void OnAddExclude(int id)
    {
        if (!HasEntity(id)) return;
        var lastEntityId = entities[Count - 1];
        var indexOfEntityId = entitiesMap[id];
        entitiesMap.Remove(id);
        Count--;
        if (Count > indexOfEntityId)
        {
            entities[indexOfEntityId] = lastEntityId;
            entitiesMap.Add(lastEntityId, indexOfEntityId);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private new void OnRemoveExclude(int id)
    {
        if (HasEntity(id)) return;
        
        ref var data = ref world.GetEntityData(id);
        // for (var i = 0; i < ExcludeCount; i++)
        //     if (data.componentTypes.Contains(ExcludeTypes[i]))
        //         return;
        //
        // for (var i = 0; i < IncludeCount; i++)
        //     if (!data.componentTypes.Contains(IncludeTypes[i]))
        //         return;
        if(pool.items[id].Value.id != ownerID) return;
        if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
        entities[Count] = data.id;
        entitiesMap.Add(data.id, Count);
        Count++;
    }
}
}