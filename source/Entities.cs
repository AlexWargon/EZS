using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
namespace Wargon.ezs
{

    public partial class Entities
    {
        internal World world;
        internal TypeMap<Type, Entities> Withouts;
        internal TypeMap<Type, EntityType> EntityTypes;

        protected int[] excludedTypes;
        protected Type Type;
        public delegate void Lambda<A>(ref A a);
        public delegate void Lambda<A, B>(ref A a, ref B b);
        public delegate void Lambda<A, B, C>(ref A a, ref B b, ref C c);
        public delegate void Lambda<A, B, C, D>(ref A a, ref B b, ref C c, ref D d);
        public delegate void Lambda<A, B, C, D, E>(ref A a, ref B b, ref C c, ref D d, ref E e);
        public delegate void Lambda<A, B, C, D, E, F>(ref A a, ref B b, ref C c, ref D d, ref E e, ref F f);
        public Entities(World world)
        {
            this.world = world;
            EntityTypes = new TypeMap<Type, EntityType>(world.EntityTypesCachSize);
            Withouts = new TypeMap<Type, Entities>(world.EntityTypesCachSize);
            Type = GetType();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<NA> WithOut<NA>()
        {
            var type = type<EntitiesWithout<NA>>.Value;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, NewWithout1(ComponentType<NA>.Value));
            }
            return (EntitiesWithout<NA>)Withouts.Get(type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntitiesWithout<NA, NB> WithOut<NA, NB>()
        {
            var type = type<EntitiesWithout<NA, NB>>.Value;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, NewWithout2(ComponentType<NA>.Value, ComponentType<NB>.Value));
            }
            return (EntitiesWithout<NA, NB>)Withouts.Get(type);
        }
        internal void InjectWithout1(Type na)
        {
            var without = NewWithout1(na);
            var type = without.Type;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, without);
            }
        }
        internal void InjectWithout2(Type na, Type nb)
        {
            var without = NewWithout2(na, nb);
            var type = without.Type;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, without);
            }
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
        public virtual void Each<A>(Lambda<A> action)
        {
            var enteties = GetEntityType<A>();
            
            if(enteties.Count < 1) return;
            for (var index = 0; index < enteties.Count; index++)
                action(ref enteties.poolA.items[enteties.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B>(Lambda<A, B> action)
        {
            var entities = GetEntityType<A, B>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action(ref  entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C>(Lambda<A, B, C> action)
        {
            var entities = GetEntityType<A, B, C>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D>(Lambda<A, B, C, D> action)
        {
            var entities = GetEntityType<A, B, C, D>();
            
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
        {
            var entities = GetEntityType<A, B, C, D, E>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]],
                        ref entities.poolE.items[entities.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A>(Lambda<Entity, A> action)
        {
            var entities = GetEntityType<A>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B>(Lambda<Entity, A, B> action)
        {
            var entities = GetEntityType<A, B>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action(ref  entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var entities = (EntityType<A, B, C>) GetEntityType(type<EntityType<A, B, C>>.Value);
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var entities = GetEntityType<A, B, C, D>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var entities = GetEntityType<A, B, C, D, E>();
            if(entities.Count < 1) return;
            for (var index = 0; index < entities.Count; index++)
                action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]],
                        ref entities.poolE.items[entities.entities[index]]);
        }

        public void Sss<A>(Action<A> sss)
        {
            
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A>(Lambda<A> lambda)
        {
            var entities = GetEntityType<A>();
            Parallel.For(0, entities.Count, i =>
            {
                lambda(ref entities.poolA.items[entities.entities[i]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B>(Lambda<A, B> lambda)
        {
            var entities = GetEntityType<A, B>();
            Parallel.For(0, entities.Count, index =>
            {
                lambda( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
        {
            var entities = GetEntityType<A, B, C>();
            Parallel.For(0, entities.Count, index =>
            {
                lambda( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
        {
            var entities = GetEntityType<A, B, C, D>();
            Parallel.For(0, entities.Count, index =>
            {
                lambda( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
        {
            var entities = GetEntityType<A, B, C, D, E>();
            Parallel.For(0, entities.Count, index =>
            {
                lambda( ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]],
                        ref entities.poolE.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A>(Lambda<Entity, A> action)
        {
            var entities = GetEntityType<A>();
            Parallel.For(0, entities.Count, i =>
            {
                action(ref entities.GetEntity(i),
                    ref entities.GetA(i));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B>(Lambda<Entity, A, B> action)
        {
            var entities = GetEntityType<A, B>();
            Parallel.For(0, entities.Count, index =>
            {
                action( ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]]);
            });

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var entities = GetEntityType<A, B, C>();
            Parallel.For(0, entities.Count, index =>
            {
                action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var entities = GetEntityType<A, B, C, D>();
            Parallel.For(0, entities.Count, index =>
            {
                action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]],
                        ref entities.poolD.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var entities = GetEntityType<A, B, C, D, E>();
            Parallel.For(0, entities.Count, index =>
            {
                action(ref entities.GetEntity(index),
                    ref entities.poolA.items[entities.entities[index]],
                    ref entities.poolB.items[entities.entities[index]],
                    ref entities.poolС.items[entities.entities[index]],
                    ref entities.poolD.items[entities.entities[index]],
                    ref entities.poolE.items[entities.entities[index]]);
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType<A> GetEntityType<A>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A>>.Value))
                EntityTypes.Add(type<EntityType<A>>.Value, NewArchetype(ComponentType<A>.Value));
            return (EntityType<A>)EntityTypes[type<EntityType<A>>.Value];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType<A, B> GetEntityType<A, B>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B>>.Value))
                EntityTypes.Add(type<EntityType<A, B>>.Value, NewEntityType(ComponentType<A>.Value, ComponentType<B>.Value));
            return (EntityType<A, B>)EntityTypes.Get(type<EntityType<A, B>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType<A, B, C> GetEntityType<A, B, C>()
        {
            return (EntityType<A, B, C>)GetEntityType(type<EntityType<A, B, C>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType<A, B, C, D> GetEntityType<A, B, C, D>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B, C, D>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C, D>>.Value, NewEntityType(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<D>.Value));
            return (EntityType<A, B, C, D>)EntityTypes.Get(type<EntityType<A, B, C, D>>.Value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType<A, B, C, D, E> GetEntityType<A, B, C, D, E>()
        {
            if (!EntityTypes.HasKey(type<EntityType<A, B, C, D, E>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C, D, E>>.Value, NewEntityType(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<D>.Value, ComponentType<E>.Value));
            return (EntityType<A, B, C, D, E>)EntityTypes.Get(type<EntityType<A, B, C, D, E>>.Value);
        }

        internal EntityType NewArchetype(Type a)
        {
            var entityType = typeof(EntityType<>);
            return (EntityType)Activator.CreateInstance(entityType.MakeGenericType(a), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b)
        {
            var entityType = typeof(EntityType<,>);
            return (EntityType)Activator.CreateInstance(entityType.MakeGenericType(a, b), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b, Type c)
        {
            var entityType = typeof(EntityType<,,>);

            return (EntityType)Activator.CreateInstance(entityType.MakeGenericType(a, b, c), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d)
        {
            var entityType = typeof(EntityType<,,,>);
            return (EntityType)Activator.CreateInstance(entityType.MakeGenericType(a, b, c, d), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d, Type e)
        {
            var entityType = typeof(EntityType<,,,,>);
            return (EntityType)Activator.CreateInstance(entityType.MakeGenericType(a, b, c, d, e), new object[] { world });
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type na)
        {
            var without = typeof(EntityType<>.WithOut<>).MakeGenericType(a, na);
            return (EntityType)Activator.CreateInstance(without, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type na, Type nb)
        {
            var without = typeof(EntityType<>.WithOut<,>).MakeGenericType(a, na, nb);
            return (EntityType)Activator.CreateInstance(without, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type na)
        {
            var without = typeof(EntityType<,>.WithOut<>).MakeGenericType(a, b, na);
            return (EntityType)Activator.CreateInstance(without, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type na, Type nb)
        {
            var without = typeof(EntityType<,>.WithOut<,>).MakeGenericType(a, b, na, nb);
            return (EntityType)Activator.CreateInstance(without, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type na)
        {
            var archetype = typeof(EntityType<,,>.Without<>).MakeGenericType(a, b, c, na);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,>.Without<,>).MakeGenericType(a, b, c, na, nb);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type d, Type na)
        {
            var archetype = typeof(EntityType<,,,>.WithOut<>).MakeGenericType(a, b, c, d, na);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        internal EntityType NewArchetypeWithout2(Type a, Type b, Type c, Type d, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,,>.WithOut<,>).MakeGenericType(a, b, c, d, na, nb);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type b, Type c, Type d, Type e, Type na)  
        {
            var archetype = typeof(EntityType<,,,,>.WithOut<>).MakeGenericType(a, b, c, d, e, na);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type d, Type e, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,,,>.WithOut<,>).MakeGenericType(a, b, c, d, e, na, nb);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntityType GetEntityType(Type type)
        {
            if(!EntityTypes.HasKey(type))
                EntityTypes.Add(type, (EntityType)Activator.CreateInstance(type,  new object[] { world }));
            return EntityTypes.Get(type);
        }
    }
}
