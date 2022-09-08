using UnityEngine.UIElements;

namespace Wargon.ezs
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    
    public partial class Entities
    {
        public class EntitiesWithout<NA, NB> : Entities where NA : new() where NB : new()
        {
            public EntitiesWithout(World world) : base(world)
            {
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A>(int ownerID, Lambda<A> action) where A: new()
            {
                var entityType = GetOwnerQuery<A>(ownerID);
                if (entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = world.GetPool<A>().items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action(a[entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B>(int ownerID, Lambda<A, B> action) where A : new() where B : new()
            {
                var entityType = GetOwnerQuery<A,B>(ownerID);
                if (entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = world.GetPool<A>().items;
                var b = world.GetPool<B>().items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action(a[entities[index]], b[entities[index]]);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private new OwnerQuery GetOwnerQuery<T1>(int id) where T1 : new()
            {
                var key = (id,TypeWithInt<T1>.ID);
                if (!ownerQueries.ContainsKey(key))
                {
                    var q = new OwnerQuery(world).WithOwner(id).With<T1>().Without<NA>().Without<NB>();
                    world.OnCreateEntityType(q);
                    ownerQueries.Add(key, q);
                }
                return ownerQueries[key];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private new OwnerQuery GetOwnerQuery<T1,T2>(int id) where T1 : new() where T2 : new()
            {
                var key = (id,TypeWithInt<T1,T2>.ID);
                if (!ownerQueries.ContainsKey(key))
                {
                    var q = new OwnerQuery(world).WithOwner(id).With<T1>().With<T2>().Without<NA>().Without<NB>();
                    world.OnCreateEntityType(q);
                    ownerQueries.Add(key, q);
                }
                return ownerQueries[key];
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A>(Lambda<Entity, A> action) where A : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action(entityType.GetEntity(index),
                        a[entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B>(Lambda<Entity, A, B> action) where A : new() where B : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action(entityType.GetEntity(index),
                        a[entities[index]],
                        b[entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B>(LambdaCR<A, B> action) where A : new() where B : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B>();
                if(entityType.Count < 1) return;
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C>(Lambda<Entity, A, B, C> action) where A : new() where B : new() where C : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C>();
                if(entityType.Count < 1) return;
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C, D>(Lambda<Entity, A, B, C, D> action) where A : new() where B : new() where C : new() where D : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D>();
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
            public new void Each<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action) where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                for (var index = entityType.Count-1; index >= 0; index--)
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
            public new void Each<A>(Lambda<A> action) where A : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action(a[entities[index]]);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B>(Lambda<A, B> action) where A : new() where B : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action( 
                        a[entities[index]],
                        b[entities[index]]);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C>(Lambda<A, B, C> action) where A : new() where B : new() where C : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action( 
                        a[entities[index]],
                        b[entities[index]],
                        c[entities[index]]);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C>(LambdaCCR<A, B, C> action) where A : new() where B : new() where C : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C>();
                if(entityType.Count < 1) return;
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
            public void Each<A, B, C, D>(LambdaCCCR<A, B, C, D> action) where A : new() where B : new() where C : new() where D : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D>();
                if(entityType.Count < 1) return;
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
            public void Each<A, B, C, D, E>(LambdaCCCRR<A, B, C, D, E> action) where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
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
                        ref d[entity],
                        ref e[entity]);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Each<A, B, C, D, E, F>(LambdaCCCCRR<A, B, C, D, E, F> action) where A : new() where B : new() where C : new() where D : new() where E : new() where F : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E, F>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                var f = entityType.poolF.items;
                for (var index = entityType.Count-1; index >= 0; index--)
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
            public void Each<A, B, C, D, E>(LambdaCCCCRR<Entity,A, B, C, D, E> action) where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                var e = entityType.poolE.items;
                for (var index = entityType.Count-1; index >= 0; index--)
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
            public new void Each<A, B, C, D>(Lambda<A, B, C, D> action) where A : new() where B : new() where C : new() where D : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D>();
                if(entityType.Count < 1) return;
                var entities = entityType.entities;
                var a = entityType.poolA.items;
                var b = entityType.poolB.items;
                var c = entityType.poolС.items;
                var d = entityType.poolD.items;
                for (var index = entityType.Count-1; index >= 0; index--)
                    action( 
                        a[entities[index]],
                        b[entities[index]],
                        c[entities[index]],
                        d[entities[index]]);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void Each<A, B, C, D, E>(Lambda<A, B, C, D, E> action) where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var entityType = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
                if(entityType.Count < 1) return;
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A>(Lambda<A> lambda) where A : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A>();
                //if (SetNesting(lambda, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(entities.GetA(index));
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B>(Lambda<A, B> lambda) where A : new() where B : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B>();
                //if (SetNesting(lambda, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda( entities.poolA.items[entities.entities[index]],
                            entities.poolB.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C>(Lambda<A, B, C> lambda) where A : new() where B : new() where C : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B, C>();
               // if (SetNesting(lambda, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    lambda(entities.poolA.items[entities.entities[index]],
                           entities.poolB.items[entities.entities[index]],
                           entities.poolС.items[entities.entities[index]]);
                });
            
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C, D>(Lambda<A, B, C, D> lambda) where A : new() where B : new() where C : new() where D : new()
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
            public new void EachThreaded<A, B, C, D, E>(Lambda<A, B, C, D, E> lambda) where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
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
            public new void EachThreaded<A>(Lambda<Entity, A> action) where A : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A>();
                //if (SetNesting(action, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action( entities.GetEntity(index),
                         entities.poolA.items[entities.entities[index]]);
                });
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B>(Lambda<Entity, A, B> action) where A : new() where B : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B>();
                //if (SetNesting(action, entities)) return;
                Parallel.For(0, entities.Count, index =>
                {
                    action(entities.GetEntity(index),
                            entities.poolA.items[entities.entities[index]],
                            entities.poolB.items[entities.entities[index]]);
                });
            
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C>(Lambda<Entity, A, B, C> action) where A : new() where B : new() where C : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B, C>();
                Parallel.For(0, entities.Count, index =>
                {
                    action(entities.GetEntity(index),
                            entities.poolA.items[entities.entities[index]],
                            entities.poolB.items[entities.entities[index]],
                            entities.poolС.items[entities.entities[index]]);
                });
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public new void EachThreaded<A, B, C, D>(Lambda<Entity, A, B, C, D> action) where A : new() where B : new() where C : new() where D : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B, C, D>();
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
            public new void EachThreaded<A, B, C, D, E>(Lambda<Entity, A, B, C, D, E> action) where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var entities = GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();

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
            internal EntityType<A>.WithOut<NA, NB> GetEntityTypeFromArrayTypePairWithout<A>() where A : new()
            {
                var id = TypePair<EntitiesWithout<NA,NB>,EntityType<A>.WithOut<NA, NB>>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    var newEntityType = new EntityType<A>.WithOut<NA, NB>(world);
                    world.OnCreateEntityType(newEntityType);
                    entityTypesArray[id] = newEntityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
                return (EntityType<A>.WithOut<NA, NB>)entityTypesArray[id];
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B>.WithOut<NA, NB> GetEntityTypeFromArrayTypePairWithout<A, B>() where A : new() where B : new()
            {
                var id = TypePair<EntitiesWithout<NA,NB>,EntityType<A, B>.WithOut<NA, NB>>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    var newEntityType = new EntityType<A, B>.WithOut<NA, NB>(world);
                    world.OnCreateEntityType(newEntityType);
                    entityTypesArray[id] = newEntityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
                return (EntityType<A, B>.WithOut<NA, NB>)entityTypesArray[id];
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C>.Without<NA, NB> GetEntityTypeFromArrayTypePairWithout<A, B, C>() where A : new() where B : new() where C : new()
            {
                var id = TypePair<EntitiesWithout<NA,NB>,EntityType<A, B, C>.Without<NA, NB>>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    var newEntityType = new EntityType<A, B, C>.Without<NA, NB>(world);
                    world.OnCreateEntityType(newEntityType);
                    entityTypesArray[id] = newEntityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
                return (EntityType<A, B, C>.Without<NA, NB>)entityTypesArray[id];
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D>.WithOut<NA, NB> GetEntityTypeFromArrayTypePairWithout<A, B, C, D>() where A : new() where B : new() where C : new() where D : new()
            {
                var id = TypePair<EntitiesWithout<NA,NB>,EntityType<A, B, C, D>.WithOut<NA, NB>>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    var newEntityType = new EntityType<A, B, C, D>.WithOut<NA, NB>(world);
                    world.OnCreateEntityType(newEntityType);
                    entityTypesArray[id] = newEntityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
                return (EntityType<A, B, C, D>.WithOut<NA, NB>)entityTypesArray[id];
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D, E>.WithOut<NA, NB> GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>() where A : new() where B : new() where C : new() where D : new() where E : new()
            {
                var id = TypePair<EntitiesWithout<NA,NB>,EntityType<A, B, C, D, E>.WithOut<NA, NB>>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    var newEntityType = new EntityType<A, B, C, D, E>.WithOut<NA, NB>(world);
                    world.OnCreateEntityType(newEntityType);
                    entityTypesArray[id] = newEntityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
                return (EntityType<A, B, C, D, E>.WithOut<NA, NB>)entityTypesArray[id];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal EntityType<A, B, C, D, E, F>.WithOut<NA, NB> GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E, F>() where A : new() where B : new() where C : new() where D : new() where E : new() where F : new()
            {
                var id = TypePair<EntitiesWithout<NA,NB>,EntityType<A, B, C, D, E, F>.WithOut<NA, NB>>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    var newEntityType = new EntityType<A, B, C, D, E, F>.WithOut<NA, NB>(world);
                    world.OnCreateEntityType(newEntityType);
                    entityTypesArray[id] = newEntityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
                return (EntityType<A, B, C, D, E, F>.WithOut<NA, NB>)entityTypesArray[id];
            }
            
            public override void AddNewEntityType<A>(A entityType)
            {
                var id = TypePair<EntitiesWithout<NA, NB>, A>.Id;
                CollectionHelp.ValidateEntityTypes(ref entityTypesArray, ref entityTypesActives, id);
                if (entityTypesActives[id] == false)
                {
                    world.OnCreateEntityType(entityType);
                    entityTypesArray[id] = entityType;
                    entityTypesActives[id] = true;
                    entityTypesCount++;
                }
            }
        }
    }
}
