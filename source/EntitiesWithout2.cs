using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wargon.ezs
{

    public partial class Entities
    {
        public class EntitiesWithout<NA, NB> : Entities
        {
            public EntitiesWithout(World world) : base(world)
            {
                excludedTypes = new [] {
                    ComponentType<NA>.ID,
                    ComponentType<NB>.ID
                };
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A>(Lambda<Entity, A> action)
            {
                var entities = GetEntityTypeWithout<A>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[i]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B>(Lambda<Entity, A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[i],
                            ref entities.poolB.items[i]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]],
                            ref entities.poolС.items[entities.entities[i]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]],
                            ref entities.poolС.items[entities.entities[i]],
                            ref entities.poolD.items[entities.entities[i]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]],
                            ref entities.poolС.items[entities.entities[i]],
                            ref entities.poolD.items[entities.entities[i]],
                            ref entities.poolE.items[entities.entities[i]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A>(Lambda<A> action)
            {
                var entities = GetEntityTypeWithout<A>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.poolA.items[entities.entities[i]]);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B>(Lambda<A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C>(Lambda<A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                for (var i = 0; i < entities.Count; i++)
                    action( ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]],
                            ref entities.poolС.items[entities.entities[i]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D>(Lambda<A, B, C, D> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
                for (var i = 0; i < entities.Count; i++)
                    action( ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]],
                            ref entities.poolС.items[entities.entities[i]],
                            ref entities.poolD.items[entities.entities[i]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                for (var i = 0; i < entities.Count; i++)
                    action( ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]],
                            ref entities.poolС.items[entities.entities[i]],
                            ref entities.poolD.items[entities.entities[i]],
                            ref entities.poolE.items[entities.entities[i]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A>(Lambda<A> lambda)
            {
                var entities = GetEntityTypeWithout<A>();
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref entities.GetA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B>(Lambda<A, B> lambda)
            {
                var entities = GetEntityTypeWithout<A, B>();
                Parallel.For(0, entities.Count, index =>
                {
                    lambda( ref entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref  entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]],
                            ref entities.poolС.items[entities.entities[index]]);
                });

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref  entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]],
                            ref entities.poolС.items[entities.entities[index]],
                            ref entities.poolD.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref  entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]],
                            ref entities.poolС.items[entities.entities[index]],
                            ref entities.poolD.items[entities.entities[index]],
                            ref entities.poolE.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A>(Lambda<Entity, A> action)
            {
                var entities = GetEntityTypeWithout<A>();
                var count = entities.Count;
                Parallel.For(0, count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.GetA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B>(Lambda<Entity, A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                var count = entities.Count;
                Parallel.For(0, count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]]);
                });

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                Parallel.For(0, entities.Count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.poolA.items[entities.entities[index]],
                        ref entities.poolB.items[entities.entities[index]],
                        ref entities.poolС.items[entities.entities[index]]);
                });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
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
            public override void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
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
            internal EntityType<A>.WithOut<NA, NB> GetEntityTypeWithout<A>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A>.WithOut<NA, NB>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A>.WithOut<NA, NB>>.Value, NewEntityTypeWithout2(ComponentType<A>.Value, ComponentType<NA>.Value, ComponentType<NB>.Value));
                }
                return (EntityType<A>.WithOut<NA, NB>)EntityTypes.Get(type<EntityType<A>.WithOut<NA, NB>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B>.WithOut<NA, NB> GetEntityTypeWithout<A, B>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B>.WithOut<NA, NB>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B>.WithOut<NA, NB>>.Value, NewEntityTypeWithout2(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<NA>.Value, ComponentType<NB>.Value));
                }
                return (EntityType<A, B>.WithOut<NA, NB>)EntityTypes.Get(type<EntityType<A, B>.WithOut<NA, NB>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C>.Without<NA, NB> GetEntityTypeWithout<A, B, C>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C>.Without<NA, NB>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C>.Without<NA, NB>>.Value, NewEntityTypeWithout2(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<NA>.Value, ComponentType<NB>.Value));
                }
                return (EntityType<A, B, C>.Without<NA, NB>)EntityTypes.Get(type<EntityType<A, B, C>.Without<NA, NB>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D>.WithOut<NA, NB> GetEntityTypeWithout<A, B, C, D>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C, D>.WithOut<NA, NB>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C, D>.WithOut<NA, NB>>.Value,
                        NewArchetypeWithout2(ComponentType<A>.Value,
                                                ComponentType<B>.Value,
                                                ComponentType<C>.Value,
                                                ComponentType<D>.Value,
                                                ComponentType<NA>.Value,
                                                ComponentType<NB>.Value));
                }
                return (EntityType<A, B, C, D>.WithOut<NA, NB>)EntityTypes.Get(type<EntityType<A, B, C, D>.WithOut<NA, NB>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D, E>.WithOut<NA, NB> GetEntityTypeWithout<A, B, C, D, E>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C, D, E>.WithOut<NA, NB>>.Value))
                    EntityTypes.Add(type<EntityType<A, B, C, D, E>.WithOut<NA, NB>>.Value,
                        NewEntityTypeWithout2(ComponentType<A>.Value,
                                             ComponentType<B>.Value,
                                             ComponentType<C>.Value,
                                             ComponentType<D>.Value,
                                             ComponentType<E>.Value,
                                             ComponentType<NA>.Value,
                                             ComponentType<NB>.Value));
                return (EntityType<A, B, C, D, E>.WithOut<NA, NB>)EntityTypes.Get(type<EntityType<A, B, C, D, E>.WithOut<NA, NB>>.Value);
            }
        }
    }
}
