using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wargon.ezs
{

    public partial class Entities
    {
        public class EntitiesWithout<NA> : Entities
        {
            public EntitiesWithout(World world) : base(world)
            {
                excludedTypes = new int[1] {
                    ComponentType<NA>.ID
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A>(Lambda<Entity, A> action)
            {
                var arch = GetEntityTypeWithout<A>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetEntityByIndex(i),
                            ref arch.GetByIndexA(i));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B>(Lambda<Entity, A, B> action)
            {
                var arch = GetEntityTypeWithout<A, B>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetEntityByIndex(i),
                            ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var arch = GetEntityTypeWithout<A, B, C>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetEntityByIndex(i),
                            ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i),
                            ref arch.GetByIndexC(i));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var arch = GetEntityTypeWithout<A, B, C, D>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetEntityByIndex(i),
                            ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i),
                            ref arch.GetByIndexC(i),
                            ref arch.GetByIndexD(i));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var arch = GetEntityTypeWithout<A, B, C, D, E>();
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
            public override void Each<A>(Lambda<A> action)
            {
                var arch = GetEntityTypeWithout<A>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetByIndexA(i));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B>(Lambda<A, B> action)
            {
                var arch = GetEntityTypeWithout<A, B>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C>(Lambda<A, B, C> action)
            {
                var arch = GetEntityTypeWithout<A, B, C>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i),
                            ref arch.GetByIndexC(i));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D>(Lambda<A, B, C, D> action)
            {
                var arch = GetEntityTypeWithout<A, B, C, D>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i),
                            ref arch.GetByIndexC(i),
                            ref arch.GetByIndexD(i));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
            {
                var arch = GetEntityTypeWithout<A, B, C, D, E>();
                var count = arch.Count;
                for (var i = 0; i < count; i++)
                    action(ref arch.GetByIndexA(i),
                            ref arch.GetByIndexB(i),
                            ref arch.GetByIndexC(i),
                            ref arch.GetByIndexD(i),
                            ref arch.GetByIndexE(i));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A>(Lambda<A> lambda)
            {
                var enteties = GetEntityTypeWithout<A>();
                var count = enteties.Count;

                Parallel.For(0, count, index =>
                {
                    lambda(ref enteties.GetByIndexA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B>(Lambda<A, B> lambda)
            {
                var enteties = GetEntityTypeWithout<A, B>();
                var count = enteties.Count;

                Parallel.For(0, count, index =>
                {
                    lambda(ref enteties.GetByIndexA(index),
                        ref enteties.GetByIndexB(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
            {
                var enteties = GetEntityTypeWithout<A, B, C>();
                var count = enteties.Count;
                Parallel.For(0, count, index =>
                {
                    lambda(ref enteties.GetByIndexA(index),
                        ref enteties.GetByIndexB(index),
                        ref enteties.GetByIndexC(index));
                });

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
            {
                var enteties = GetEntityTypeWithout<A, B, C, D>();
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
            public override void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
            {
                var enteties = GetEntityTypeWithout<A, B, C, D, E>();
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
            public override void EachThreaded<A>(Lambda<Entity, A> action)
            {
                var enteties = GetEntityTypeWithout<A>();
                var count = enteties.Count;
                Parallel.For(0, count, index =>
                {
                    action(ref enteties.GetEntityByIndex(index),
                        ref enteties.GetByIndexA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B>(Lambda<Entity, A, B> action)
            {
                var enteties = GetEntityTypeWithout<A, B>();
                var count = enteties.Count;
                Parallel.For(0, count, index =>
                {
                    action(ref enteties.GetEntityByIndex(index),
                        ref enteties.GetByIndexA(index),
                        ref enteties.GetByIndexB(index));
                });

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var enteties = GetEntityTypeWithout<A, B, C>();
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
            public override void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var enteties = GetEntityTypeWithout<A, B, C, D>();
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
            public override void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var enteties = GetEntityTypeWithout<A, B, C, D, E>();
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
            private EntityType<A>.WithOut<NA> GetEntityTypeWithout<A>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A>.WithOut<NA>)EntityTypes.Get(type<EntityType<A>.WithOut<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private EntityType<A, B>.WithOut<NA> GetEntityTypeWithout<A, B>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B>.WithOut<NA>)EntityTypes.Get(type<EntityType<A, B>.WithOut<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private EntityType<A, B, C>.Without<NA> GetEntityTypeWithout<A, B, C>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C>.Without<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C>.Without<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B, C>.Without<NA>)EntityTypes.Get(type<EntityType<A, B, C>.Without<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private EntityType<A, B, C, D>.WithOut<NA> GetEntityTypeWithout<A, B, C, D>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C, D>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C, D>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<D>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B, C, D>.WithOut<NA>)EntityTypes.Get(type<EntityType<A, B, C, D>.WithOut<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private EntityType<A, B, C, D, E>.WithOut<NA> GetEntityTypeWithout<A, B, C, D, E>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C, D, E>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C, D, E>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<D>.Value, ComponentType<E>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B, C, D, E>.WithOut<NA>)EntityTypes.Get(type<EntityType<A, B, C, D, E>.WithOut<NA>>.Value);
            }
        }
    }
}
