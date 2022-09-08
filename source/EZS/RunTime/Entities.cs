using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Wargon.ezs
{
    public delegate void Lambda<A>(A a);

    public delegate void Lambda<A, B>(A a, B b);
    
    public delegate void Lambda<A, B, C>(A a, B b, C c);

    public delegate void Lambda<A, B, C, D>(A a, B b, C c, D d);

    public delegate void Lambda<A, B, C, D, E>(A a, B b, C c, D d, E e);

    public delegate void Lambda<A, B, C, D, E, F>(A a, B b, C c, D d, E e, F f);

    public delegate void Lambda<A, B, C, D, E, F, G>(A a, B b, C c, D d, E e, F f, G g);


    public delegate void LambdaRef<A>(ref A a);
    public delegate void LambdaI<A>(in A a);
    public delegate void LambdaRef<A, B>(ref A a, ref B b);

    public delegate void LambdaRef<A, B, C>(ref A a, ref B b, ref C c);

    public delegate void LambdaRef<A, B, C, D>(ref A a, ref B b, ref C c, ref D d);

    public delegate void LambdaRef<A, B, C, D, E>(ref A a, ref B b, ref C c, ref D d, ref E e);

    public delegate void LambdaRef<A, B, C, D, E, F>(ref A a, ref B b, ref C c, ref D d, ref E e, ref F f);

    public delegate void LambdaRef<A, B, C, D, E, F, G>(ref A a, ref B b, ref C c, ref D d, ref E e, ref F f, ref G g);

    public delegate void LambdaRRC<A, B, C>(ref A a, ref B b, C c);
    public delegate void LambdaCRR<A, B, C>(A a, ref B b, ref C c);

    public delegate void LambdaRCCC<A, B, C, D>(ref A a, B b, C c, D d) where A : unmanaged;

    public delegate void LambdaCR<A, B>(A a, ref B b);
    public delegate void LambdaCI<A, B>(A a, in B b);

    public delegate void LambdaCCR<A, B, C>(A a, B b, ref C c);

    public delegate void LambdaCCCR<A, B, C, D>(A a, B b, C c, ref D d);

    public delegate void LambdaCCCCR<A, B, C, D, E>(A a, B b, C c, D d, ref E e);

    public delegate void LambdaCCCCCR<A, B, C, D, E, F>(A a, B b, C c, D d, E e, ref F f);

    public delegate void LambdaCCCRR<A, B, C, D, E>(A a, B b, C c, ref D d, ref E e);

    public delegate void LambdaCCCCRR<A, B, C, D, E, F>(A a, B b, C c, D d, ref E e, ref F f);

    public delegate void LambdaCRefCCC<A, B, C, D, E>(A a, ref B b, C c, D d, E e) where B : unmanaged;

    public delegate void LambdaRefIn<A>(ref A a);

    public delegate void LambdaRefIn<A, B>(ref A a, in B b);

    public delegate void LambdaInIn<A, B>(in A a, in B b);

    public delegate void LambdaRefRefIn<A, B, C>(ref A a, ref B b, in C c);

    public delegate void LambdaRefRefRefRef<A, B, C, D>(ref A a, ref B b, ref C c, ref D d);

    public partial class Entities
    {
        internal Entities[] entitiesWithoutArray;
        private bool[] entitiesWithoutActives;
        internal EntityType[] entityTypesArray;
        internal bool[] entityTypesActives;
        private int[] excludedTypes;
        internal int withoutsCount;
        internal int entityTypesCount;
        private readonly World world;

        public Entities(World world)
        {
            this.world = world;
            entityTypesArray = new EntityType[world.EntityTypesCachSize];
            entityTypesActives = new bool[world.EntityTypesCachSize];
            entitiesWithoutArray = new Entities[world.EntityTypesCachSize];
            entitiesWithoutActives = new bool[world.EntityTypesCachSize];
        }


        internal void Clear()
        {
            for (var i = 0; i < entityTypesCount; i++) entityTypesArray[i].Clear();
            for (var i = 0; i < withoutsCount; i++) entitiesWithoutArray?[i].Clear();
            
            if (entitiesWithoutArray != null)
                Array.Clear(entitiesWithoutArray ?? Array.Empty<Entities>(), 0, entitiesWithoutArray.Length);
            if (entityTypesArray != null)
                Array.Clear(entityTypesArray ?? Array.Empty<EntityType>(), 0, entityTypesArray.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal World GetWorld() => world;
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<NA> Without<NA>()
        {
            var id = EntitiesWithoutID<EntitiesWithout<NA>>.ID;
            CollectionHelp.ValidateEntities(ref entitiesWithoutArray, ref entityTypesActives, id);
            if (!entitiesWithoutActives[id])
            {
                entitiesWithoutArray[id] = new EntitiesWithout<NA>(world);
                entitiesWithoutActives[id] = true;
                withoutsCount++;
                return (EntitiesWithout<NA>) entitiesWithoutArray[id];
            }

            return (EntitiesWithout<NA>) entitiesWithoutArray[id];
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<NA, NB> Without<NA, NB>()
        {
            var id = EntitiesWithoutID<EntitiesWithout<NA, NB>>.ID;
            CollectionHelp.ValidateEntities(ref entitiesWithoutArray, ref entityTypesActives, id);
            if (!entitiesWithoutActives[id])
            {
                entitiesWithoutArray[id] = new EntitiesWithout<NA, NB>(world);
                entitiesWithoutActives[id] = true;
                withoutsCount++;
            }

            return (EntitiesWithout<NA, NB>) entitiesWithoutArray[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear<A>()
        {
            var entityType = GetEntityTypeFromArrayTypePairAbstract<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            for (var index = entityType.Count-1; index >= 0; index--)
                entityType.GetEntity(entities[index]).Remove<A>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(Lambda<A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(a[entities[index]]);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaRef<A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(ref a[entities[index]]);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaI<A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(in a[entities[index]]);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(Lambda<A, B> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    b[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(LambdaCR<A, B> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    ref b[entity]);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(Lambda<A, B, C> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    b[entity],
                    c[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(LambdaCCR<A, B, C> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    b[entity],
                    ref c[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(LambdaCCR<Entity, A, B> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    world.GetEntity(entity),
                    a[entity],
                    ref b[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(LambdaCCCR<Entity, A, B, C> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    world.GetEntity(entity),
                    a[entity],
                    b[entity],
                    ref c[entity]);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(LambdaCCCCR<Entity, A, B, C, D> queue)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                queue(
                    world.GetEntity(entity),
                    a[entity],
                    b[entity],
                    c[entity],
                    ref d[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(LambdaCCCRR<Entity, A, B, C, D> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    world.GetEntity(entity),
                    a[entity],
                    b[entity],
                    ref c[entity],
                    ref d[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D, E>(LambdaCCCCR<A, B, C, D, E> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D, E>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    b[entity],
                    c[entity],
                    d[entity],
                    ref e[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D, E>(LambdaCCCCCR<Entity, A, B, C, D, E> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D, E>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    world.GetEntity(entity),
                    a[entity],
                    b[entity],
                    c[entity],
                    d[entity],
                    ref e[entity]);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(Lambda<A, B, C, D> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    b[entity],
                    c[entity],
                    d[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(LambdaRCCC<A, B, C, D> action) where A : unmanaged
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    ref a[entity],
                    b[entity],
                    c[entity],
                    d[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(LambdaCCCR<A, B, C, D> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action(
                    a[entity],
                    b[entity],
                    c[entity],
                    ref d[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D, E>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]],
                    e[entities[index]]);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(Lambda<Entity, A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaCR<Entity, A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    ref a[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaCI<Entity, A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    in a[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaCI<int, A> action)
        {
            var entityType = GetEntityType<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entities[index],
                    in a[entities[index]]);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(Lambda<Entity, A, B> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            if (entityType.Count < 1)
            {
                return;
            }
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]],
                    b[entities[index]]);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]]);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(LambdaCRefCCC<Entity, A, B, C, D> action) where A : unmanaged
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    ref a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]]);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D, E>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]],
                    e[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D, E, F>(Lambda<Entity, A, B, C, D, E, F> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D, E, F>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            var f = entityType.poolF.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]],
                    e[entities[index]],
                    f[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A>(Lambda<A> lambda)
        {
            var entities = GetEntityType<A>();

            Parallel.For(0, entities.Count, i => { lambda(entities.poolA.items[entities.entities[i]]); });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B>(Lambda<A, B> lambda)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            Parallel.For(0, entityType.Count, index =>
            {
                lambda(a[entities[index]],
                    b[entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
        {
            var entities = GetEntityTypeFromArrayTypePair<A, B, C>();

            Parallel.For(0, entities.Count, index =>
            {
                lambda(entities.poolA.items[entities.entities[index]],
                    entities.poolB.items[entities.entities[index]],
                    entities.poolС.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C, D>();
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            Parallel.For(0, entityType.Count, index =>
            {
                lambda(a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
        {
            var entities = GetEntityTypeFromArrayTypePair<A, B, C, D, E>();

            Parallel.For(0, entities.Count, index =>
            {
                lambda(entities.poolA.items[entities.entities[index]],
                    entities.poolB.items[entities.entities[index]],
                    entities.poolС.items[entities.entities[index]],
                    entities.poolD.items[entities.entities[index]],
                    entities.poolE.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A>(Lambda<Entity, A> action)
        {
            var entities = GetEntityType<A>();

            Parallel.For(0, entities.Count, i =>
            {
                action(entities.GetEntity(i),
                    entities.poolA.items[entities.entities[i]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B>(Lambda<Entity, A, B> action)
        {
            var entities = GetEntityTypeFromArrayTypePair<A, B>();

            Parallel.For(0, entities.Count, index =>
            {
                action(entities.GetEntity(index),
                    entities.poolA.items[entities.entities[index]],
                    entities.poolB.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B>(LambdaRef<A, B> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            Parallel.For(0, entityType.Count, index =>
            {
                action(ref a[entities[index]],
                    ref b[entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var entities = GetEntityTypeFromArrayTypePair<A, B, C>();

            Parallel.For(0, entities.Count, index =>
            {
                action(entities.GetEntity(index),
                    entities.poolA.items[entities.entities[index]],
                    entities.poolB.items[entities.entities[index]],
                    entities.poolС.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var entities = GetEntityTypeFromArrayTypePair<A, B, C, D>();

            Parallel.For(0, entities.Count, index =>
            {
                action(entities.GetEntity(index),
                    entities.poolA.items[entities.entities[index]],
                    entities.poolB.items[entities.entities[index]],
                    entities.poolС.items[entities.entities[index]],
                    entities.poolD.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var entities = GetEntityTypeFromArrayTypePair<A, B, C, D, E>();

            Parallel.For(0, entities.Count, index =>
            {
                action(entities.GetEntity(index),
                    entities.poolA.items[entities.entities[index]],
                    entities.poolB.items[entities.entities[index]],
                    entities.poolС.items[entities.entities[index]],
                    entities.poolD.items[entities.entities[index]],
                    entities.poolE.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A> GetEntityType<A>()
        {
            var id = TypePair<Entities, EntityType<A>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return (EntityType<A>) entityTypesArray[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType GetEntityTypeFromArrayTypePairAbstract<A>()
        {
            var id = TypePair<Entities, EntityType<A>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return entityTypesArray[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType GetEntityTypeFromArrayTypePairAbstractAndAddSystem<A>(UpdateSystem updateSystem)
        {
            var id = TypePair<Entities, EntityType<A>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }
            //entityTypesArray[id].AddSystem(updateSystem);
            return entityTypesArray[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B> GetEntityTypeFromArrayTypePair<A, B>()
        {
            var id = TypePair<Entities, EntityType<A, B>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A, B>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return (EntityType<A, B>) entityTypesArray[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C> GetEntityTypeFromArrayTypePair<A, B, C>()
        {
            var id = TypePair<Entities, EntityType<A, B, C>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A, B, C>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return (EntityType<A, B, C>) entityTypesArray[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C, D> GetEntityTypeFromArrayTypePair<A, B, C, D>()
        {
            var id = TypePair<Entities, EntityType<A, B, C, D>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A, B, C, D>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return (EntityType<A, B, C, D>) entityTypesArray[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C, D, E> GetEntityTypeFromArrayTypePair<A, B, C, D, E>()
        {
            var id = TypePair<Entities, EntityType<A, B, C, D, E>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A, B, C, D, E>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return (EntityType<A, B, C, D, E>) entityTypesArray[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C, D, E, F> GetEntityTypeFromArrayTypePair<A, B, C, D, E, F>()
        {
            var id = TypePair<Entities, EntityType<A, B, C, D, E, F>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                var newEntityType = new EntityType<A, B, C, D, E, F>(world);
                world.OnCreateEntityType(newEntityType);
                entityTypesArray[id] = newEntityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }

            return (EntityType<A, B, C, D, E, F>) entityTypesArray[id];
        }

        public virtual void AddNewEntityType<A>(EntityType entityType)
        {
            var id = TypePair<Entities, EntityType<A>>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                world.OnCreateEntityType(entityType);
                entityTypesArray[id] = entityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }
        }
        public void Each<A, B>(LambdaRef<A, B> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action( 
                    ref a[entity],
                    ref b[entity]);
            }
        }
        public void Each<A, B, C>(LambdaRef<A, B, C> action)
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B, C>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = entityType.Count-1; index >= 0; index--)
            {
                var entity = entities[index];
                action( 
                    ref a[entity],
                    ref b[entity],
                    ref c[entity]);
            }
        }
        
        private static class EntitiesWithoutID<T>
        {
            public static readonly int ID;

            static EntitiesWithoutID()
            {
                ID = EntitiesWithoutCount.Count++;
            }
        }

        private static class EntitiesWithoutCount
        {
            public static int Count;
        }
        
        
        
    }

    internal struct EntityForEach
    {
        public EntityForEach Without<A>()
        {
            return this;
        }
    }
}