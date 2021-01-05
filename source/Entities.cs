using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wargon.ezs
{

    public partial class Entities
    {
        internal World world;
        internal TypeMap<Type, Entities> Withouts;
        internal TypeMap<Type, EntityType> EntityTypes;
        protected int[] excludedTypes;
        private int archetypesCount;
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
            archetypesCount = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entities WithOut<NA>()
        {
            var type = type<EntitiesWithout<NA>>.Value;
            if (!Withouts.HasKey(type))
            {
                Withouts.Add(type, NewWithout1(ComponentType<NA>.Value));
            }
            return Withouts.Get(type);
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
            var arch = GetEntityType<A>();
            var count = arch.Count;

            for (var i = 0; i < count; i++)
                action(ref arch.GetByIndexA(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B>(Lambda<A, B> action)
        {
            var arch = GetEntityType<A, B>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C>(Lambda<A, B, C> action)
        {
            var arch = GetEntityType<A, B, C>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D>(Lambda<A, B, C, D> action)
        {
            var arch = GetEntityType<A, B, C, D>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i),
                        ref arch.GetByIndexD(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
        {
            var arch = GetEntityType<A, B, C, D, E>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i),
                        ref arch.GetByIndexD(i),
                        ref arch.GetByIndexE(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A>(Lambda<Entity, A> action)
        {
            var arch = GetEntityType<A>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetEntityByIndex(i),
                        ref arch.GetByIndexA(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B>(Lambda<Entity, A, B> action)
        {
            var arch = GetEntityType<A, B>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetEntityByIndex(i),
                        ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var arch = GetEntityType<A, B, C>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetEntityByIndex(i),
                        ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var arch = GetEntityType<A, B, C, D>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetEntityByIndex(i),
                        ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i),
                        ref arch.GetByIndexD(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var arch = GetEntityType<A, B, C, D, E>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetEntityByIndex(i),
                        ref arch.GetByIndexA(i),
                        ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i),
                        ref arch.GetByIndexD(i),
                        ref arch.GetByIndexE(i));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A>(Lambda<A> lambda)
        {
            var enteties = GetEntityType<A>();
            var count = enteties.Count;

            Parallel.For(0, count, index =>
            {
                lambda(ref enteties.GetByIndexA(index));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B>(Lambda<A, B> lambda)
        {
            var enteties = GetEntityType<A, B>();
            var count = enteties.Count;

            Parallel.For(0, count, index =>
            {
                lambda(ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
        {
            var enteties = GetEntityType<A, B, C>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                lambda(ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index),
                    ref enteties.GetByIndexC(index));
            });

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
        {
            var enteties = GetEntityType<A, B, C, D>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                lambda(ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index),
                    ref enteties.GetByIndexC(index),
                    ref enteties.GetByIndexD(index));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
        {
            var enteties = GetEntityType<A, B, C, D, E>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                lambda(ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index),
                    ref enteties.GetByIndexC(index),
                    ref enteties.GetByIndexD(index),
                    ref enteties.GetByIndexE(index));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A>(Lambda<Entity, A> action)
        {
            var enteties = GetEntityType<A>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                action(ref enteties.GetEntityByIndex(index),
                    ref enteties.GetByIndexA(index));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B>(Lambda<Entity, A, B> action)
        {
            var enteties = GetEntityType<A, B>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                action(ref enteties.GetEntityByIndex(index),
                    ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index));
            });

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
        {
            var enteties = GetEntityType<A, B, C>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                action(ref enteties.GetEntityByIndex(index),
                    ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index),
                    ref enteties.GetByIndexC(index));
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
        {
            var enteties = GetEntityType<A, B, C, D>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                action(ref enteties.GetEntityByIndex(index),
                    ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index),
                    ref enteties.GetByIndexC(index),
                    ref enteties.GetByIndexD(index));
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
        {
            var enteties = GetEntityType<A, B, C, D, E>();
            var count = enteties.Count;
            Parallel.For(0, count, index =>
            {
                action(ref enteties.GetEntityByIndex(index),
                    ref enteties.GetByIndexA(index),
                    ref enteties.GetByIndexB(index),
                    ref enteties.GetByIndexC(index),
                    ref enteties.GetByIndexD(index),
                    ref enteties.GetByIndexE(index));
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
            if (!EntityTypes.HasKey(type<EntityType<A, B, C>>.Value))
                EntityTypes.Add(type<EntityType<A, B, C>>.Value, NewEntityType(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value));
            return (EntityType<A, B, C>)EntityTypes.Get(type<EntityType<A, B, C>>.Value);
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
        internal void InjectArchetype(Type typeA)
        {
            var archetype = NewArchetype(typeA);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }

        internal void InjectEntityType(Type typeA, Type typeB)
        {
            var archetype = NewEntityType(typeA, typeB);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }

        }
        internal void InjectEntityType(Type typeA, Type typeB, Type typeC)
        {
            var archetype = NewEntityType(typeA, typeB, typeC);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectEntityType(Type typeA, Type typeB, Type typeC, Type typeD)
        {

            var archetype = NewEntityType(typeA, typeB, typeC, typeD);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectEntityType(Type typeA, Type typeB, Type typeC, Type typeD, Type typeE)
        {

            var archetype = NewEntityType(typeA, typeB, typeC, typeD, typeE);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectArchetypeWithout1(Type typeA, Type na)
        {
            InjectWithout1(na);
            var archetype = NewEntityTypeWithout1(typeA, na);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }

        internal void InjectArchetypeWithout1(Type typeA, Type typeB, Type na)
        {
            InjectWithout1(na);
            var archetype = NewEntityTypeWithout1(typeA, typeB, na, na);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }

        }
        internal void InjectArchetypeWithout1(Type typeA, Type typeB, Type typeC, Type na)
        {
            InjectWithout1(na);
            var archetype = NewEntityTypeWithout1(typeA, typeB, typeC, na);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectArchetypeWithout1(Type typeA, Type typeB, Type typeC, Type typeD, Type na)
        {
            InjectWithout1(na);
            var archetype = NewEntityTypeWithout1(typeA, typeB, typeC, typeD, na);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectArchetypeWithout1(Type typeA, Type typeB, Type typeC, Type typeD, Type typeE, Type na)
        {
            InjectWithout1(na);
            var archetype = NewEntityTypeWithout1(typeA, typeB, typeC, typeD, typeE, na);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectArchetypeWithout2(Type typeA, Type na, Type nb)
        {
            InjectWithout2(na, nb);
            var archetype = NewEntityTypeWithout2(typeA, na, nb);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }

        internal void InjectArchetypeWithout2(Type typeA, Type typeB, Type na, Type nb)
        {
            InjectWithout2(na, nb);
            var archetype = NewEntityTypeWithout2(typeA, typeB, na, nb);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }

        }
        internal void InjectArchetypeWithout2(Type typeA, Type typeB, Type typeC, Type na, Type nb)
        {
            InjectWithout2(na, nb);
            var archetype = NewEntityTypeWithout2(typeA, typeB, typeC, na, nb);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectArchetypeWithout2(Type typeA, Type typeB, Type typeC, Type typeD, Type na, Type nb)
        {
            InjectWithout2(na, nb);
            var archetype = NewArchetypeWithout2(typeA, typeB, typeC, typeD, na, nb);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal void InjectArchetypeWithout2(Type typeA, Type typeB, Type typeC, Type typeD, Type typeE, Type na, Type nb)
        {
            InjectWithout2(na, nb);
            var archetype = NewEntityTypeWithout2(typeA, typeB, typeC, typeD, typeE, na, nb);
            var type = archetype.Type;
            if (!EntityTypes.HasKey(type))
            {
                EntityTypes.Add(type, archetype);
                archetypesCount++;
            }
        }
        internal EntityType NewArchetype(Type a)
        {
            var archetype = typeof(EntityType<>);
            return (EntityType)Activator.CreateInstance(archetype.MakeGenericType(a), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b)
        {
            var archetype = typeof(EntityType<,>);
            return (EntityType)Activator.CreateInstance(archetype.MakeGenericType(a, b), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b, Type c)
        {
            var archetype = typeof(EntityType<,,>);

            return (EntityType)Activator.CreateInstance(archetype.MakeGenericType(a, b, c), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d)
        {
            var archetype = typeof(EntityType<,,,>);
            return (EntityType)Activator.CreateInstance(archetype.MakeGenericType(a, b, c, d), new object[] { world });
        }
        internal EntityType NewEntityType(Type a, Type b, Type c, Type d, Type e)
        {
            var archetype = typeof(EntityType<,,,,>);
            return (EntityType)Activator.CreateInstance(archetype.MakeGenericType(a, b, c, d, e), new object[] { world });
        }
        internal EntityType NewEntityTypeWithout1(Type a, Type na)
        {
            Console.WriteLine(a.Name + " " + na.Name);
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
            var archetype = typeof(EntityType<,,,,>.WithOut<>).MakeGenericType(a, b, c, d, d, na);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
        internal EntityType NewEntityTypeWithout2(Type a, Type b, Type c, Type d, Type e, Type na, Type nb)
        {
            var archetype = typeof(EntityType<,,,,>.WithOut<,>).MakeGenericType(a, b, c, d, e, na, nb);
            return (EntityType)Activator.CreateInstance(archetype, new object[] { world });
        }
    }
    public partial class Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Each<ATag, B, C, D, E>(Lambda<B, C, D, E> action)
        {
            var arch = GetEntityType<ATag, B, C, D, E>();
            var count = arch.Count;
            for (var i = 0; i < count; i++)
                action(ref arch.GetByIndexB(i),
                        ref arch.GetByIndexC(i),
                        ref arch.GetByIndexD(i),
                        ref arch.GetByIndexE(i));
        }
    }
}
