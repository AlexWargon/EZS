namespace Wargon.ezs
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    
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
            public new void Each<A>(Lambda<Entity, A> action)
            {
                var entityType = GetEntityTypeWithout<A>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                for (var index = 0; index < entityType.Count; index++)
                    action(entityType.GetEntity(index),
                        a[entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B>(Lambda<Entity, A, B> action)
            {
                var entityType = GetEntityTypeWithout<A, B>();
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
            public new void Each<A, B>(LambdaCR<A, B> action)
            {
                var entityType = GetEntityTypeWithout<A, B>();
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
            public new void Each<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C>();
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
            public new void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                for (var index = 0; index < entityType.Count; index++)
                {
                    var entity = entities[index];
                    action(entityType.GetEntity(index),
                        a[entity],
                        b[entity],
                        c[entity],
                        d[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                for (var index = 0; index < entityType.Count; index++)
                {
                    var entity = entities[index];
                    action(entityType.GetEntity(index),
                        a[entity],
                        b[entity],
                        c[entity],
                        d[entity],
                        e[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A>(Lambda<A> action)
            {
                var entityType = GetEntityTypeWithout<A>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                for (var index = 0; index < entityType.Count; index++)
                    action(a[entities[index]]);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B>(Lambda<A, B> action)
            {
                var entityType = GetEntityTypeWithout<A, B>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                for (var index = 0; index < entityType.Count; index++)
                    action( 
                        a[entities[index]],
                        b[entities[index]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C>(Lambda<A, B, C> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                for (var index = 0; index < entityType.Count; index++)
                    action( 
                        a[entities[index]],
                        b[entities[index]],
                        c[entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C>(LambdaCCR<A, B, C> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C>();
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
                        ref c[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C, D>(LambdaCCCR<A, B, C, D> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D>();
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
                        ref d[entity]);
                }
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C, D, E>(LambdaCCCRR<A, B, C, D, E> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                for (var index = 0; index < entityType.Count; index++)
                {
                    var entity = entities[index];
                    action( 
                        a[entity],
                        b[entity],
                        c[entity],
                        ref d[entity],
                        ref e[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C, D, E, F>(LambdaCCCCRR<A, B, C, D, E, F> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D, E, F>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                var f = entityType.poolF.items;
                for (var index = 0; index < entityType.Count; index++)
                {
                    var entity = entities[index];
                    action( 
                        a[entity],
                        b[entity],
                        c[entity],
                        d[entity],
                        ref e[entity],
                        ref f[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C, D, E>(LambdaCCCCRR<Entity,A, B, C, D, E> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                for (var index = 0; index < entityType.Count; index++)
                {
                    var entity = entities[index];
                    action( entityType.GetEntity(index),
                        a[entity],
                        b[entity],
                        c[entity],
                        ref d[entity],
                        ref e[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C, D>(Lambda<A, B, C, D> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                for (var index = 0; index < entityType.Count; index++)
                    action( 
                        a[entities[index]],
                        b[entities[index]],
                        c[entities[index]],
                        d[entities[index]]);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action)
            {
                var entityType = GetEntityTypeWithout<A, B, C, D, E>();
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
            public new void EachThreaded<A>(Lambda<A> lambda)
            {
                var entities = GetEntityTypeWithout<A>();
                //if (SetNesting(lambda, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(entities.GetA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B>(Lambda<A, B> lambda)
            {
                var entities = GetEntityTypeWithout<A, B>();
                //if (SetNesting(lambda, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda( entities.poolA.items[entities.entities[index]],
                            entities.poolB.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C>(Lambda<A, B, C> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
               // if (SetNesting(lambda, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(entities.poolA.items[entities.entities[index]],
                           entities.poolB.items[entities.entities[index]],
                           entities.poolС.items[entities.entities[index]]);
                });
            
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda)
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
            public new void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();
                //if (SetNesting(lambda, entities)) return;
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
            public new void EachThreaded<A>(Lambda<Entity, A> action)
            {
                var entities = GetEntityTypeWithout<A>();
                //if (SetNesting(action, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action( entities.GetEntity(index),
                         entities.poolA.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B>(Lambda<Entity, A, B> action)
            {
                var entities = GetEntityTypeWithout<A, B>();
                //if (SetNesting(action, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(entities.GetEntity(index),
                            entities.poolA.items[entities.entities[index]],
                            entities.poolB.items[entities.entities[index]]);
                });
            
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action)
            {
                var entities = GetEntityTypeWithout<A, B, C>();
                Parallel.For(0, entities.Count, index =>
                {
                    action(entities.GetEntity(index),
                            entities.poolA.items[entities.entities[index]],
                            entities.poolB.items[entities.entities[index]],
                            entities.poolС.items[entities.entities[index]]);
                });
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D>();
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
            public new void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action)
            {
                var entities = GetEntityTypeWithout<A, B, C, D, E>();

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
                        NewEntityTypeWithout2(ComponentType<A>.Value,
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
                        NewEntityTypeWithout2(
                        ComponentType<A>.Value,
                        ComponentType<B>.Value,
                        ComponentType<C>.Value,
                        ComponentType<D>.Value,
                        ComponentType<E>.Value,
                        ComponentType<NA>.Value,
                        ComponentType<NB>.Value));
                return (EntityType<A, B, C, D, E>.WithOut<NA, NB>)EntityTypes.Get(type<EntityType<A, B, C, D, E>.WithOut<NA, NB>>.Value);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D, E, F>.WithOut<NA, NB> GetEntityTypeWithout<A, B, C, D, E, F>()
            {
                if (!EntityTypes.HasKey(type<EntityType<A, B, C, D, E, F>.WithOut<NA, NB>>.Value))
                    EntityTypes.Add(type<EntityType<A, B, C, D, E, F>.WithOut<NA, NB>>.Value,
                        NewEntityTypeWithout2(ComponentType<A>.Value,
                            ComponentType<B>.Value,
                            ComponentType<C>.Value,
                            ComponentType<D>.Value,
                            ComponentType<E>.Value,
                            ComponentType<F>.Value,
                            ComponentType<NA>.Value,
                            ComponentType<NB>.Value));
                return (EntityType<A, B, C, D, E, F>.WithOut<NA, NB>)EntityTypes.Get(type<EntityType<A, B, C, D, E, F>.WithOut<NA, NB>>.Value);
            }
        }
    }
}
