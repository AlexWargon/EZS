using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

    public delegate void LambdaCCR<A, B, C>(A a, B b, ref C c);

    public delegate void LambdaCCCR<A, B, C, D>(A a, B b, C c, ref D d);

    public delegate void LambdaCCCCR<A, B, C, D, E>(A a, B b, C c, D d, ref E e);

    public delegate void LambdaCCCCCR<A, B, C, D, E, F>(A a, B b, C c, D d, E e, ref F f);

    public delegate void LambdaCCCRR<A, B, C, D, E>(A a, B b, C c, ref D d, ref E e);

    public delegate void LambdaCCCCRR<A, B, C, D, E, F>(A a, B b, C c, D d, ref E e, ref F f);
    public delegate void LambdaCCCRRR<A, B, C, D, E, F>(A a, B b, C c, ref D d, ref E e, ref F f);
    public delegate void LambdaCRCCC<A, B, C, D, E>(A a, ref B b, C c, D d, E e) where B : unmanaged;

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
        internal Dictionary<(int,int), OwnerQuery> ownerQueries;
        internal int withoutsCount;
        internal int entityTypesCount;
        internal int[] WITHOUT_TYPES;
        private readonly World world;
        private int CountEX;
        public Entities(World world)
        {
            this.world = world;
            entityTypesArray = new EntityType[world.EntityTypesCachSize];
            entityTypesActives = new bool[world.EntityTypesCachSize];
            entitiesWithoutArray = new Entities[world.EntityTypesCachSize];
            ownerQueries = new Dictionary<(int, int), OwnerQuery>();
            entitiesWithoutActives = new bool[world.EntityTypesCachSize];
        }
        private void OnAddWithoutOnEntity(int id) {

            for (var i = 0; i < entityTypesCount; i++) {
                entityTypesArray[i].UpdateOnAddWithout(id);
            }
        }

        private void OnRemoveWithoutOnEntity(int id) {
            ref var data = ref world.GetEntityData(id);
            // for (var i = 0; i < CountEX; i++) {
            //     if(data.componentTypes.Contains(WITHOUT_TYPES[i])) {
            //         return;
            //     }
            // }
            // for (var i = 0; i < entityTypesCount; i++) {
            //     entityTypesArray[i].UpdateOnRemoveWithout(in data);
            // }
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
        public EntitiesWithout<TWithout> Without<TWithout>() where TWithout : struct
        {
            var id = EntitiesWithoutID<EntitiesWithout<TWithout>>.ID;
            CollectionHelp.ValidateEntities(ref entitiesWithoutArray, ref entityTypesActives, id);
            
            if (!entitiesWithoutActives[id]) {
                var newWithouts = new EntitiesWithout<TWithout>(world) {
                    WITHOUT_TYPES = new [] {
                        ComponentType<TWithout>.ID
                    }, 
                    CountEX = 1
                };
                var pool = world.GetPoolByID(newWithouts.WITHOUT_TYPES[0]);
                pool.OnAdd += newWithouts.OnAddWithoutOnEntity;
                pool.OnRemove += newWithouts.OnRemoveWithoutOnEntity;
                
                entitiesWithoutArray[id] = newWithouts;
                entitiesWithoutActives[id] = true;
                withoutsCount++;
            }
            
            return (EntitiesWithout<TWithout>) entitiesWithoutArray[id];
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<TWithout1, TWithout2> Without<TWithout1, TWithout2>() where TWithout1 : struct where TWithout2 : struct
        {
            var id = EntitiesWithoutID<EntitiesWithout<TWithout1, TWithout2>>.ID;
            CollectionHelp.ValidateEntities(ref entitiesWithoutArray, ref entityTypesActives, id);
            
            if (!entitiesWithoutActives[id]) {
                var newWithouts = new EntitiesWithout<TWithout1, TWithout2>(world) {
                    WITHOUT_TYPES = new [] {
                    ComponentType<TWithout1>.ID,
                    ComponentType<TWithout2>.ID
                    
                    }, 
                    CountEX = 2
                };
                var pool1 = world.GetPoolByID(newWithouts.WITHOUT_TYPES[0]);
                pool1.OnAdd += newWithouts.OnAddWithoutOnEntity;
                pool1.OnRemove += newWithouts.OnRemoveWithoutOnEntity;
                
                var pool2 = world.GetPoolByID(newWithouts.WITHOUT_TYPES[1]);
                pool2.OnAdd += newWithouts.OnAddWithoutOnEntity;
                pool2.OnRemove += newWithouts.OnRemoveWithoutOnEntity;
                
                entitiesWithoutArray[id] = newWithouts;
                entitiesWithoutActives[id] = true;
                withoutsCount++;
            }

            return (EntitiesWithout<TWithout1, TWithout2>) entitiesWithoutArray[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear<A>()  where A: struct
        {
            var entityType = GetEntityTypeFromArrayTypePair<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            for (var index = entityType.Count-1; index >= 0; index--)
                entityType.GetEntity(entities[index]).Remove<A>();
        }

        public Entities WithOwner(int id) {
            return this;
        }
        
        public Entities WithJob() {
            return this;
        }

        public Entities WithJobParallel() {
            return this;
        }

        public Entities BurstOff() {
            return this;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entities Each<A>(Lambda<A> action)  where A: struct
        {
            var entityType = GetEntityTypeFromArrayTypePair<A>();
            if (entityType.Count < 1) return this;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(a[entities[index]]);
            return this;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entities Each<A, B>(Lambda<A, B> action)  where A: struct where B : struct
        {
            var entityType = GetEntityTypeFromArrayTypePair<A, B>();
            if (entityType.Count < 1) return this;
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
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(LambdaCR<A, B> action)  where A: struct where B : struct
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
        public void Each<A, B, C>(Lambda<A, B, C> action)  where A: struct where B : struct where C : struct
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
        public void Each<A, B, C>(LambdaCCR<A, B, C> action)  where A: struct where B : struct where C : struct
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
        public void Each<A, B, C>(LambdaCRR<A, B, C> action)  where A: struct where B : struct where C : struct
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
                    ref b[entity],
                    ref c[entity]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(LambdaCCR<Entity, A, B> action)  where A: struct where B : struct
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
        public void Each<A, B>(LambdaCRR<Entity, A, B> action)  where A: struct where B : struct
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
                    ref a[entity],
                    ref b[entity]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(LambdaCCCR<Entity, A, B, C> action)  where A: struct where B : struct where C : struct
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
        public void Each<A, B, C, D>(LambdaCCCCR<Entity, A, B, C, D> queue)  where A: struct where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D>(LambdaCCCRR<Entity, A, B, C, D> action)  where A: struct where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D, E>(LambdaCCCCR<A, B, C, D, E> action)  where A: struct where B : struct where C : struct where D : struct where  E : struct
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
        public void Each<A, B, C, D, E>(LambdaCCCCCR<Entity, A, B, C, D, E> action)  where A: struct where B : struct where C : struct where D : struct where  E : struct
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
        public void Each<A, B, C, D>(Lambda<A, B, C, D> action)  where A: struct where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D>(LambdaRCCC<A, B, C, D> action)  where A: unmanaged where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D>(LambdaCCCR<A, B, C, D> action)  where A: struct where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)  where A: struct where B : struct where C : struct where D : struct where  E : struct
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
        public void Each<A>(Lambda<Entity, A> action)  where A: struct
        {
            var entityType = GetEntityTypeFromArrayTypePair<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    a[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaCR<Entity, A> action)  where A: struct
        {
            var entityType = GetEntityTypeFromArrayTypePair<A>();
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(entityType.GetEntity(index),
                    ref a[entities[index]]);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(Lambda<Entity, A, B> action)  where A: struct where B : struct
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
        public void Each<A, B, C>(Lambda<Entity, A, B, C> action)  where A: struct where B : struct where C : struct
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
        public void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action) where A: struct where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D>(LambdaCRCCC<Entity, A, B, C, D> action) where A: unmanaged where B : struct where C : struct where D : struct
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
        public void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action) where A: struct where B : struct where C : struct where D : struct where  E : struct
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
        public void Each<A, B, C, D, E, F>(Lambda<Entity, A, B, C, D, E, F> action) where A: struct where B : struct where C : struct where D : struct where  E : struct where  F : struct
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
        public void EachThreaded<A>(Lambda<A> lambda) where A: struct
        {
            var entities = GetEntityTypeFromArrayTypePair<A>();

            Parallel.For(0, entities.Count, i => { lambda(entities.poolA.items[entities.entities[i]]); });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B>(Lambda<A, B> lambda) where A: struct where B : struct
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
        public void EachThreaded<A, B, C>(Lambda<A, B, C> lambda) where A: struct where B : struct where C : struct
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
        public void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)  where A: struct where B : struct where C : struct where D : struct
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
        public void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)  where A: struct where B : struct where C : struct where D : struct where  E : struct
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
        public void EachThreaded<A>(Lambda<Entity, A> action)  where A: struct
        {
            var entities = GetEntityTypeFromArrayTypePair<A>();

            Parallel.For(0, entities.Count, i =>
            {
                action(entities.GetEntity(i),
                    entities.poolA.items[entities.entities[i]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B>(Lambda<Entity, A, B> action)  where A: struct where B : struct
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
        public void EachThreaded<A, B>(LambdaRef<A, B> action)  where A: struct where B : struct
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
        public void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)  where A: struct where B : struct where C : struct
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
        public void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)  where A: struct where B : struct where C : struct where D : struct
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
        public void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)  where A: struct where B : struct where C : struct where D : struct where  E : struct
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
        public EntityType<A> GetEntityTypeFromArrayTypePair<A>()  where A: struct
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
        public EntityType GetEntityTypeFromArrayTypePairAbstract<A>()  where A: struct
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
        public EntityType GetEntityTypeFromArrayTypePairAbstractAndAddSystem<A>(UpdateSystem updateSystem)  where A: struct
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
        public EntityType<A, B> GetEntityTypeFromArrayTypePair<A, B>()  where A: struct where B : struct
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
        public EntityType<A, B, C> GetEntityTypeFromArrayTypePair<A, B, C>()  where A: struct where B : struct where C : struct
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
        public EntityType<A, B, C, D> GetEntityTypeFromArrayTypePair<A, B, C, D>()  where A: struct where B : struct where C : struct where D : struct
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
            where A:  struct 
            where B : struct 
            where C : struct 
            where D : struct 
            where E : struct
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
        public EntityType<A, B, C, D, E, F> GetEntityTypeFromArrayTypePair<A, B, C, D, E, F>()  where A: struct where B : struct where C : struct where D : struct where  E : struct where  F : struct
        {
            ref var id = ref TypePair<Entities, EntityType<A, B, C, D, E, F>>.Id;
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

        public virtual void AddNewEntityType<A>(A entityType)  where A: EntityType
        {
            var id = TypePair<Entities, A>.Id;
            CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
            if (entityTypesActives[id] == false)
            {
                world.OnCreateEntityType(entityType);
                entityTypesArray[id] = entityType;
                entityTypesActives[id] = true;
                entityTypesCount++;
            }
        }
        public void Each<A, B>(LambdaRef<A, B> action) where A: struct where B : struct
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
        public void Each<A, B, C>(LambdaRef<A, B, C> action) where A: struct where B : struct where C : struct
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal OwnerQuery GetOwnerQuery(int id)
        {
            var key = (id,-1);
            if (!ownerQueries.ContainsKey(key))
            {
                var q = new OwnerQuery(world).WithOwner(id);
                world.OnCreateEntityType(q);
                ownerQueries.Add(key, q);
            }
            return ownerQueries[key];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OwnerQuery GetOwnerQuery<T1>(int id) where T1 : struct
        {
            var key = (id,TypeWithInt<T1>.ID);
            if (!ownerQueries.ContainsKey(key))
            {
                var q = new OwnerQuery(world).WithOwner(id).With<T1>();
                world.OnCreateEntityType(q);
                ownerQueries.Add(key, q);
            }
            return ownerQueries[key];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OwnerQuery GetOwnerQuery<T1,T2>(int id) where T1 : struct where T2 : struct
        {
            var key = (id,TypeWithInt<T1,T2>.ID);
            if (!ownerQueries.ContainsKey(key))
            {
                var q = new OwnerQuery(world).WithOwner(id).With<T1>().With<T2>();
                world.OnCreateEntityType(q);
                ownerQueries.Add(key, q);
            }
            return ownerQueries[key];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachWithOwner<A>(int ownerID, Lambda<A> action) where A: struct
        {
            var entityType = GetOwnerQuery<A>(ownerID);
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = world.GetPool<A>().items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(a[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachWithOwner<A, B>(int ownerID, Lambda<A, B> action) where A: struct where B : struct {
            var entityType = GetOwnerQuery<A,B>(ownerID);
            if (entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = world.GetPool<A>().items;
            var b = world.GetPool<B>().items;
            for (var index = entityType.Count-1; index >= 0; index--)
                action(a[entities[index]], b[entities[index]]);
        }
        private struct EntitiesWithoutID<T>
        {
            public static readonly int ID;
            static EntitiesWithoutID()
            {
                ID = EntitiesWithoutCount.Count++;
            }
        }

        private struct EntitiesWithoutCount
        {
            public static int Count;
        }

        private struct TypeWithInt<A>
        {
            public static readonly int ID;
            static TypeWithInt()
            {
                ID = TypeWithIntCount.Value++;
            }
        }
        private struct TypeWithInt<A, B>
        {
            public static readonly int ID;
            static TypeWithInt()
            {
                ID = TypeWithIntCount.Value++;
            }
        }
        public static class TypeWithIntCount
        {
            public static int Value;
        }
    }

    public struct EntitiesForEach {

        public EntitiesForEach Without<T1>() where T1 : new() {
            return this;
        }

        public EntitiesForEach Without<T1, T2>() where T1 : new() where T2 : new() {
            return this;
        }

        public EntitiesForEach Without<T1, T2, T3>() where T1 : new() where T2 : new() where T3 : new() {
            return this;
        }
    }
}