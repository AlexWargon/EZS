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
                excludedTypes = new [] {
                    ComponentType<NA>.ID
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A>(Lambda<Entity, A> action)
            {
                var entities = GetEntityTypeWithout<A>();
                if(entities.Count < 1) return;
                var count = entities.Count;
                for (var i = 0; i < count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[entities.entities[i]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B>(Lambda<Entity, A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                if(entities.Count < 1) return;
                for (var i = 0; i < entities.Count; i++)
                    action(ref entities.GetEntity(i),
                            ref entities.poolA.items[entities.entities[i]],
                            ref entities.poolB.items[entities.entities[i]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                if(entities.Count < 1) return;
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
                if(entities.Count < 1) return;
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
                if(entities.Count < 1) return;
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
                if(entities.Count < 1) return;
                for (var index = 0; index < entities.Count; index++)
                    action(ref entities.poolA.items[entities.entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B>(Lambda<A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                var count = entities.Count;
                for (var index = 0; index < entities.Count; index++)
                    action( ref entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C>(Lambda<A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                if(entities.Count < 1) return;
                for (var index = 0; index < entities.Count; index++)
                    action( ref entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]],
                            ref entities.poolС.items[entities.entities[index]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D>(Lambda<A, B, C, D> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
                if(entities.Count < 1) return;
                for (var index = 0; index < entities.Count; index++)
                    action( ref entities.poolA.items[entities.entities[index]],
                            ref entities.poolB.items[entities.entities[index]],
                            ref entities.poolС.items[entities.entities[index]],
                            ref entities.poolD.items[entities.entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                if(entities.Count < 1) return;
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
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref entities.poolA.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B>(Lambda<A, B> lambda)
            {
                var entities = GetEntityTypeWithout<A, B>();

                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref entities.poolA.items[entities.entities[index]],
                           ref entities.poolB.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref entities.GetA(index),
                           ref entities.GetB(index),
                           ref entities.GetC(index));
                });

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref entities.GetA(index),
                        ref entities.GetB(index),
                        ref entities.GetC(index),
                        ref entities.GetD(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(ref entities.GetA(index),
                        ref entities.GetB(index),
                        ref entities.GetC(index),
                        ref entities.GetD(index),
                        ref entities.GetE(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A>(Lambda<Entity, A> action)
            {
                var entities = GetEntityTypeWithout<A>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.GetA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B>(Lambda<Entity, A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.GetA(index),
                        ref entities.GetB(index));
                });

            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.GetA(index),
                        ref entities.GetB(index),
                        ref entities.GetC(index));
                });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.GetA(index),
                        ref entities.GetB(index),
                        ref entities.GetC(index),
                        ref entities.GetD(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                if(entities.Count < 1) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(ref entities.GetEntity(index),
                        ref entities.GetA(index),
                        ref entities.GetB(index),
                        ref entities.GetC(index),
                        ref entities.GetD(index),
                        ref entities.GetE(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A>.WithOut<NA> GetEntityTypeWithout<A>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A>.WithOut<NA>)EntityTypes.Get(type<EntityType<A>.WithOut<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B>.WithOut<NA> GetEntityTypeWithout<A, B>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B>.WithOut<NA>)EntityTypes.Get(type<EntityType<A, B>.WithOut<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C>.Without<NA> GetEntityTypeWithout<A, B, C>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C>.Without<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C>.Without<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B, C>.Without<NA>)EntityTypes.Get(type<EntityType<A, B, C>.Without<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D>.WithOut<NA> GetEntityTypeWithout<A, B, C, D>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C, D>.WithOut<NA>>.Value))
                {
                    EntityTypes.Add(type<EntityType<A, B, C, D>.WithOut<NA>>.Value,
                        NewEntityTypeWithout1(ComponentType<A>.Value, ComponentType<B>.Value, ComponentType<C>.Value, ComponentType<D>.Value, ComponentType<NA>.Value));
                }
                return (EntityType<A, B, C, D>.WithOut<NA>)EntityTypes.Get(type<EntityType<A, B, C, D>.WithOut<NA>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D, E>.WithOut<NA> GetEntityTypeWithout<A, B, C, D, E>()
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
