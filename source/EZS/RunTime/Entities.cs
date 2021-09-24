namespace Wargon.ezs
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public delegate void Lambda<A>(A a);
    public delegate void Lambda<A, B>(A a, B b);
    public delegate void Lambda<A, B, C>(A a, B b, C c);
    public delegate void Lambda<A, B, C, D>(A a, B b, C c, D d);
    public delegate void Lambda<A, B, C, D, E>(A a, B b, C c, D d, E e);
    public delegate void Lambda<A, B, C, D, E, F>(A a, B b, C c, D d, E e, F f);
    public delegate void Lambda<A, B, C, D, E, F, G>(A a, B b, C c, D d, E e, F f, G g);


    public delegate void LambdaRef<A>(ref A a);
    public delegate void LambdaRef<A, B>(ref A a,ref B b);
    public delegate void LambdaRef<A, B, C>(ref A a, ref B b,ref C c);
    public delegate void LambdaRef<A, B, C, D>(ref A a, ref B b, ref C c, ref D d);
    public delegate void LambdaRef<A, B, C, D, E>(ref A a, ref B b, ref C c, ref D d, ref E e);
    public delegate void LambdaRef<A, B, C, D, E, F>(ref A a, ref B b, ref C c, ref D d, ref E e, ref F f);
    public delegate void LambdaRef<A, B, C, D, E, F, G>(ref A a, ref B b, ref C c, ref D d, ref E e, ref F f, ref G g);

    public delegate void LambdaRCCC<A, B, C, D>(ref A a, B b, C c, D d) where A : unmanaged;

    public delegate void LambdaCR<A, B>(A a, ref B b);
    public delegate void LambdaCCR<A, B, C>(A a, B b, ref C c);
    public delegate void LambdaCCCR<A, B, C, D>(A a, B b, C c, ref D d);
    public delegate void LambdaCCCRR<A, B, C, D, E>(A a, B b, C c, ref D d, ref E e);
    public delegate void LambdaCCCCRR<A, B, C, D, E, F>(A a, B b, C c, D d, ref E e, ref F f);
    public delegate void LambdaCRefCCC<A, B, C, D, E>(A a, ref B b, C c, D d, E e) where B : unmanaged;
    public delegate void LambdaRefIn<A>(ref A a);
    public delegate void LambdaRefIn<A, B>(ref A a, in B b);
    public delegate void LambdaInIn<A, B>(in A a, in B b);
    public delegate void LambdaRefRefIn<A, B, C>(ref A a, ref  B b, in C c);
    public delegate void LambdaRefRefRefRef<A, B, C, D>(ref A a, ref B b, ref C c, ref D d);
    public partial class Entities
    {
        private World world;
        internal TypeMap<Type, Entities> Withouts;
        internal TypeMap<Type, EntityType> EntityTypes;

        protected int[] excludedTypes;
        public Entities(World world)
        {
            this.world = world;
            EntityTypes = new TypeMap<Type, EntityType>(world.EntityTypesCachSize);
            Withouts = new TypeMap<Type, Entities>(world.EntityTypesCachSize);
        }

        internal void Clear()
        {
            Withouts.Clear();
            for (var i = 0; i < EntityTypes.Count; i++)
                EntityTypes.Values[i].Clear();
            EntityTypes.Clear();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<NA> Without<NA>()
        {
            var type = type<EntitiesWithout<NA>>.Value;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, NewWithout1(ComponentType<NA>.Value));
            }
            return (EntitiesWithout<NA>)Withouts.Get(type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<NA,NB> Without<NA, NB>()
        {
            var type = type<EntitiesWithout<NA, NB>>.Value;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, NewWithout2(ComponentType<NA>.Value, ComponentType<NB>.Value));
            }
            return (EntitiesWithout<NA,NB>)Withouts.Get(type);
        }

        internal Entities NewWithout1(Type na)
        {
            var without = typeof(EntitiesWithout<>);
            return (Entities)Activator.CreateInstance(without.MakeGenericType(na), new object[] { world });
        }

        internal Entities NewWithout2(Type na, Type nb)
        {
            var without = typeof(EntitiesWithout<,>);
            return (Entities)Activator.CreateInstance(without.MakeGenericType(na, nb), new object[] { world });
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear<A>()
        {
            var entityType = GetEntityType<A>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = 0; index < entityType.Count; index++)
                entityType.GetEntity(entities[index]).Remove<A>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(Lambda<A> action)
        {
            var entityType = GetEntityType<A>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = 0; index < entityType.Count; index++)
                action(a[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(Lambda<A, B> action)
        {
            var entityType = GetEntityType<A, B>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = 0; index < entityType.Count; index++)
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
            var entityType = GetEntityType<A, B>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = 0; index < entityType.Count; index++)
            {
                var entity = entities[index];
                action( 
                    a[entity],
                    ref b[entity]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(Lambda<A, B, C> action)
        {
            var entityType = GetEntityType<A, B, C>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = 0; index < entityType.Count; index++)
            {
                var entity = entities[index];
                action( 
                    a[entity],
                    b[entity],
                    c[entity]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(Lambda<A, B, C, D> action)
        {
            var entityType = GetEntityType<A, B, C, D>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = 0; index < entityType.Count; index++)
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
            var entityType = GetEntityType<A, B, C, D>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = 0; index < entityType.Count; index++)
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
        public void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
        {
            var entityType = GetEntityType<A, B, C, D, E>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            for (var index = 0; index < entityType.Count; index++)
                action( 
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]],
                    e[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(Lambda<Entity, A> action)
        {
            var entityType = GetEntityType<A>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                    a[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A>(LambdaCR<Entity, A> action)
        {
            var entityType = GetEntityType<A>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                    ref a[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B>(Lambda<Entity, A, B> action)
        {
            var entityType = GetEntityType<A, B>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                        a[entities[index]],
                        b[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var entityType = GetEntityType<A, B, C>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                        a[entities[index]],
                        b[entities[index]],
                        c[entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var entityType = GetEntityType<A, B, C, D>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                        a[entities[index]],
                        b[entities[index]],
                        c[entities[index]],
                        d[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D>(LambdaCRefCCC<Entity, A, B, C, D> action) where A : unmanaged
        {
            var entityType = GetEntityType<A, B, C, D>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                    ref a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var entityType = GetEntityType<A, B, C, D, E>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            for (var index = 0; index < entityType.Count; index++)
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
            var entityType = GetEntityType<A, B, C, D, E, F>();
            if(entityType.Count < 1) return;
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            var c = entityType.poolС.items;
            var d = entityType.poolD.items;
            var e = entityType.poolE.items;
            var f = entityType.poolF.items;
            for (var index = 0; index < entityType.Count; index++)
                action(entityType.GetEntity(index),
                    a[entities[index]],
                    b[entities[index]],
                    c[entities[index]],
                    d[entities[index]],
                    e[entities[index]],
                    f[entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A>(Lambda<A> lambda)
        {
            var entities = GetEntityType<A>();

            Parallel.For(0, entities.Count, i =>
            {
                lambda(entities.poolA.items[entities.entities[i]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A, B>(Lambda<A, B> lambda)
        {
            var entityType = GetEntityType<A, B>();
            var entities = entityType.entities;
            var a = entityType.poolA.items;
            var b = entityType.poolB.items;
            Parallel.For(0, entityType.Count, index =>
            {
                lambda( a[entities[index]],
                        b[entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
        {
            var entities = GetEntityType<A, B, C>();

            Parallel.For(0, entities.Count, index =>
            {
                lambda( entities.poolA.items[entities.entities[index]],
                        entities.poolB.items[entities.entities[index]],
                        entities.poolС.items[entities.entities[index]]);
            });
            
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
        {
            var entityType = GetEntityType<A, B, C, D>();
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
        public  void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
        {
            var entities = GetEntityType<A, B, C, D, E>();

            Parallel.For(0, entities.Count, index =>
            {
                lambda( entities.poolA.items[entities.entities[index]],
                        entities.poolB.items[entities.entities[index]],
                        entities.poolС.items[entities.entities[index]],
                        entities.poolD.items[entities.entities[index]],
                        entities.poolE.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A>(Lambda<Entity, A> action)
        {
            var entities = GetEntityType<A>();

            Parallel.For(0, entities.Count, i =>
            {
                action(entities.GetEntity(i),
                    entities.poolA.items[entities.entities[i]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A, B>(Lambda<Entity, A, B> action)
        {
            var entities = GetEntityType<A, B>();

            Parallel.For(0, entities.Count, index =>
            {
                action( entities.GetEntity(index),
                        entities.poolA.items[entities.entities[index]],
                        entities.poolB.items[entities.entities[index]]);
            });
        
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EachThreaded<A, B>(LambdaRef<A, B> action)
        {
            var entityType = GetEntityType<A, B>();
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
        public  void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var entities = GetEntityType<A, B, C>();

            Parallel.For(0, entities.Count, index =>
            {
                action(entities.GetEntity(index),
                        entities.poolA.items[entities.entities[index]],
                        entities.poolB.items[entities.entities[index]],
                        entities.poolС.items[entities.entities[index]]);
            });
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var entities = GetEntityType<A, B, C, D>();

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
        public  void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var entities = GetEntityType<A, B, C, D, E>();

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
            if (!EntityTypes.HasKey(type<EntityType<A>>.Value))
                EntityTypes.Add(type<EntityType<A>>.Value, 
                    NewEntityType(ComponentType<A>.Value));
            return (EntityType<A>)EntityTypes[type<EntityType<A>>.Value];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B> GetEntityType<A, B>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B>>.Value))
                EntityTypes.Add(type<EntityType<A, B>>.Value, 
                    NewEntityType(
                        ComponentType<A>.Value, 
                        ComponentType<B>.Value));
            return (EntityType<A, B>)EntityTypes.Get(type<EntityType<A, B>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C> GetEntityType<A, B, C>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B, C>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C>>.Value, 
                    NewEntityType(
                        ComponentType<A>.Value, 
                        ComponentType<B>.Value, 
                        ComponentType<C>.Value));
            return (EntityType<A, B, C>)EntityTypes.Get(type<EntityType<A, B, C>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C, D> GetEntityType<A, B, C, D>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B, C, D>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C, D>>.Value, 
                    NewEntityType(
                        ComponentType<A>.Value, 
                        ComponentType<B>.Value, 
                        ComponentType<C>.Value, 
                        ComponentType<D>.Value));
            return (EntityType<A, B, C, D>)EntityTypes.Get(type<EntityType<A, B, C, D>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C, D, E> GetEntityType<A, B, C, D, E>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B, C, D, E>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C, D, E>>.Value, 
                    NewEntityType(
                        ComponentType<A>.Value, 
                        ComponentType<B>.Value, 
                        ComponentType<C>.Value, 
                        ComponentType<D>.Value, 
                        ComponentType<E>.Value));
            
            return (EntityType<A, B, C, D, E>)EntityTypes.Get(type<EntityType<A, B, C, D, E>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityType<A, B, C, D, E, F> GetEntityType<A, B, C, D, E, F>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B, C, D, E ,F>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C, D, E, F>>.Value, 
                    NewEntityType(
                        ComponentType<A>.Value, 
                        ComponentType<B>.Value, 
                        ComponentType<C>.Value, 
                        ComponentType<D>.Value, 
                        ComponentType<E>.Value,
                        ComponentType<F>.Value));
            
            return (EntityType<A, B, C, D, E, F>)EntityTypes.Get(type<EntityType<A, B, C, D, E, F>>.Value);
        }
        internal EntityType NewEntityType(Type a)
        {
            var entityType = typeof(EntityType<>);
            var et = (EntityType) Activator.CreateInstance(entityType.MakeGenericType(a), new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityType(Type a, Type b)
        {
            var entityType = typeof(EntityType<,>);
            var et = (EntityType) Activator.CreateInstance(entityType.MakeGenericType(a, b), new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityType(Type a, Type b, Type c)
        {
            var entityType = typeof(EntityType<,,>);
            var et = (EntityType) Activator.CreateInstance(entityType.MakeGenericType(a, b, c), new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d)
        {
            var entityType = typeof(EntityType<,,,>);
            var et = (EntityType) Activator.CreateInstance(entityType.MakeGenericType(a, b, c, d), new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d, Type e)
        {
            var entityType = typeof(EntityType<,,,,>);
            var et = (EntityType) Activator.CreateInstance(entityType.MakeGenericType(a, b, c, d, e),
                new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d, Type e, Type f)
        {
            var entityType = typeof(EntityType<,,,,,>);
            var et = (EntityType) Activator.CreateInstance(entityType.MakeGenericType(a, b, c, d, e, f),
                new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type na)
        {
            var without = typeof(EntityType<>.WithOut<>).MakeGenericType(a, na);
            var et = (EntityType) Activator.CreateInstance(without, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }

        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type na)
        {
            var without = typeof(EntityType<,>.WithOut<>).MakeGenericType(a, b, na);
            var et = (EntityType) Activator.CreateInstance(without, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }

        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type na)
        {
            var archetype = typeof(EntityType<,,>.Without<>).MakeGenericType(a, b, c, na);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }

        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type d, Type na)
        {
            var archetype = typeof(EntityType<,,,>.WithOut<>).MakeGenericType(a, b, c, d, na);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }

        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type d, Type e, Type na)  
        {
            var archetype = typeof(EntityType<,,,,>.WithOut<>).MakeGenericType(a, b, c, d, e, na);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type d, Type e, Type f, Type na)
        {
            var archetype = typeof(EntityType<,,,,,>.WithOut<>).MakeGenericType(a, b, c, d, e, f, na);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type na, Type nb)
        {
            var without = typeof(EntityType<>.WithOut<,>).MakeGenericType(a, na, nb);
            var et = (EntityType) Activator.CreateInstance(without, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type na, Type nb)
        {
            var without = typeof(EntityType<,>.WithOut<,>).MakeGenericType(a, b, na, nb);
            var et = (EntityType) Activator.CreateInstance(without, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,>.Without<,>).MakeGenericType(a, b, c, na, nb);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type d, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,,>.WithOut<,>).MakeGenericType(a, b, c, d, na, nb);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type d, Type e, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,,,>.WithOut<,>).MakeGenericType(a, b, c, d, e, na, nb);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type d, Type e, Type f, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,,,,>.WithOut<,>).MakeGenericType(a, b, c, d, e, f, na, nb);
            var et = (EntityType) Activator.CreateInstance(archetype, new object[] {world});
            world.OnCreateEntityType(et);
            return et;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType GetEntityType(Type type)
        {
            if(!EntityTypes.HasKey(type))
                EntityTypes.Add(type, (EntityType)Activator.CreateInstance(type,  new object[] { world }));
            
            return EntityTypes.Get(type);
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
