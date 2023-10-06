using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs {

    public struct EntitiesEach {
        
    }

    public static partial class EntitiesExtensions {
        public static EntitiesEach Without<T1>(this EntitiesEach entities) where T1 : new() {
            return entities;
        }
        public static EntitiesEach Without<T1, T2>(this EntitiesEach entities) where T1 : new() where T2 : new() {
            return entities;
        }
        public static EntitiesEach Without<T1, T2, T3>(this EntitiesEach entities) where T1 : new() where T2 : new() where T3 : new() {
            return entities;
        }

        public static EntitiesEach WithOwner(this EntitiesEach entities, int id) {
            return entities;
        }
        public static EntitiesEach WithOwner(this EntitiesEach entities, Entity entity) {
            return entities;
        }
        public static EntitiesEach WithJob(this EntitiesEach entities) {
            return entities;
        }
        public static EntitiesEach WithJobParallel(this EntitiesEach entities) {
            return entities;
        }
    }

    #region FILTERS

    public interface IFilter {
        public void Setup(EntityType entityQuery, World world);
    }
    public interface IChacheID {
        void Cache(ref int[] cache);
    }

    public struct With<T1> : IFilter
        where T1 : struct {
        public Pool<T1> pool1;
        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2> : IFilter
        where T1 : struct
        where T2 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        private EntityType query;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
            query = entityQuery as EntityQuery<With<T1, T2>>;
        }
    }

    public struct With<T1, T2, T3> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        private EntityType query;
        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
            query = entityQuery;
        }
    }

    public struct With<T1, T2, T3, T4> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2, T3, T4, T5> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;
        public Pool<T5> pool5;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2, T3, T4, T5, T6> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;
        public Pool<T5> pool5;
        public Pool<T6> pool6;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T6>.ID;
            pool6 = world.GetPool<T6>();
            pool6.OnAdd += entityQuery.OnAddInclude;
            pool6.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2, T3, T4, T5, T6, T7> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;
        public Pool<T5> pool5;
        public Pool<T6> pool6;
        public Pool<T7> pool7;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T6>.ID;
            pool6 = world.GetPool<T6>();
            pool6.OnAdd += entityQuery.OnAddInclude;
            pool6.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T7>.ID;
            pool7 = world.GetPool<T7>();
            pool7.OnAdd += entityQuery.OnAddInclude;
            pool7.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2, T3, T4, T5, T6, T7, T8> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;
        public Pool<T5> pool5;
        public Pool<T6> pool6;
        public Pool<T7> pool7;
        public Pool<T8> pool8;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T6>.ID;
            pool6 = world.GetPool<T6>();
            pool6.OnAdd += entityQuery.OnAddInclude;
            pool6.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T7>.ID;
            pool7 = world.GetPool<T7>();
            pool7.OnAdd += entityQuery.OnAddInclude;
            pool7.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T8>.ID;
            pool8 = world.GetPool<T8>();
            pool8.OnAdd += entityQuery.OnAddInclude;
            pool8.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;
        public Pool<T5> pool5;
        public Pool<T6> pool6;
        public Pool<T7> pool7;
        public Pool<T8> pool8;
        public Pool<T9> pool9;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T6>.ID;
            pool6 = world.GetPool<T6>();
            pool6.OnAdd += entityQuery.OnAddInclude;
            pool6.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T7>.ID;
            pool7 = world.GetPool<T7>();
            pool7.OnAdd += entityQuery.OnAddInclude;
            pool7.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T8>.ID;
            pool8 = world.GetPool<T8>();
            pool8.OnAdd += entityQuery.OnAddInclude;
            pool8.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T9>.ID;
            pool9 = world.GetPool<T9>();
            pool9.OnAdd += entityQuery.OnAddInclude;
            pool9.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IFilter
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
        where T5 : struct
        where T6 : struct
        where T7 : struct
        where T8 : struct
        where T9 : struct
        where T10 : struct {
        public Pool<T1> pool1;
        public Pool<T2> pool2;
        public Pool<T3> pool3;
        public Pool<T4> pool4;
        public Pool<T5> pool5;
        public Pool<T6> pool6;
        public Pool<T7> pool7;
        public Pool<T8> pool8;
        public Pool<T9> pool9;
        public Pool<T10> pool10;

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T1>.ID;
            pool1 = world.GetPool<T1>();
            pool1.OnAdd += entityQuery.OnAddInclude;
            pool1.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T2>.ID;
            pool2 = world.GetPool<T2>();
            pool2.OnAdd += entityQuery.OnAddInclude;
            pool2.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T3>.ID;
            pool3 = world.GetPool<T3>();
            pool3.OnAdd += entityQuery.OnAddInclude;
            pool3.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T4>.ID;
            pool4 = world.GetPool<T4>();
            pool4.OnAdd += entityQuery.OnAddInclude;
            pool4.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T5>.ID;
            pool5 = world.GetPool<T5>();
            pool5.OnAdd += entityQuery.OnAddInclude;
            pool5.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T6>.ID;
            pool6 = world.GetPool<T6>();
            pool6.OnAdd += entityQuery.OnAddInclude;
            pool6.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T7>.ID;
            pool7 = world.GetPool<T7>();
            pool7.OnAdd += entityQuery.OnAddInclude;
            pool7.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T8>.ID;
            pool8 = world.GetPool<T8>();
            pool8.OnAdd += entityQuery.OnAddInclude;
            pool8.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T9>.ID;
            pool9 = world.GetPool<T9>();
            pool9.OnAdd += entityQuery.OnAddInclude;
            pool9.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;

            entityQuery.IncludeTypes[entityQuery.IncludeCount] = ComponentType<T10>.ID;
            pool10 = world.GetPool<T10>();
            pool10.OnAdd += entityQuery.OnAddInclude;
            pool10.OnRemove += entityQuery.OnRemoveInclude;
            entityQuery.IncludeCount++;
        }
    }

    public struct Empty : IFilter {
        public void Setup(EntityType entityQuery, World world) { }
    }

    public struct Without<T1> : IFilter, IChacheID
        where T1 : new() {
        public void Setup(EntityType entityQuery, World world) {
            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T1>.ID;
            // var pool1 = world.GetPool<T1>();
            // pool1.OnAdd += entityQuery.OnAddExclude;
            // pool1.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;
        }

        public void Cache(ref int[] cache) {
            cache = new[] {ComponentType<T1>.ID};
        }
    }

    public struct Without<T1, T2> : IFilter, IChacheID
        where T1 : new()
        where T2 : new() {
        public void Cache(ref int[] cache) {
            cache = new[] {
                ComponentType<T1>.ID,
                ComponentType<T2>.ID
            };
        }

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T1>.ID;
            // var pool1 = world.GetPool<T1>();
            // pool1.OnAdd += entityQuery.OnAddExclude;
            // pool1.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;

            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T2>.ID;
            // var pool2 = world.GetPool<T2>();
            // pool2.OnAdd += entityQuery.OnAddExclude;
            // pool2.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;
        }
    }

    public struct Without<T1, T2, T3> : IFilter, IChacheID where T1 : new() where T2 : new() where T3 : new() {
        public void Cache(ref int[] cache) {
            cache = new[] {
                ComponentType<T1>.ID,
                ComponentType<T2>.ID,
                ComponentType<T3>.ID
            };
        }

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T1>.ID;
            // var pool1 = world.GetPool<T1>();
            // pool1.OnAdd += entityQuery.OnAddExclude;
            // pool1.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;

            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T2>.ID;
            // var pool2 = world.GetPool<T2>();
            // pool2.OnAdd += entityQuery.OnAddExclude;
            // pool2.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;

            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T3>.ID;
            // var pool3 = world.GetPool<T3>();
            // pool3.OnAdd += entityQuery.OnAddExclude;
            // pool3.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;
        }
    }

    public struct Without<T1, T2, T3, T4> : IFilter, IChacheID
        where T1 : new() where T2 : new() where T3 : new() where T4 : new() {
        public void Cache(ref int[] cache) {
            cache = new[] {
                ComponentType<T1>.ID,
                ComponentType<T2>.ID,
                ComponentType<T3>.ID,
                ComponentType<T4>.ID
            };
        }

        public void Setup(EntityType entityQuery, World world) {
            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T1>.ID;
            // var pool1 = world.GetPool<T1>();
            // pool1.OnAdd += entityQuery.OnAddExclude;
            // pool1.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;

            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T2>.ID;
            // var pool2 = world.GetPool<T2>();
            // pool2.OnAdd += entityQuery.OnAddExclude;
            // pool2.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;

            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T3>.ID;
            // var pool3 = world.GetPool<T3>();
            // pool3.OnAdd += entityQuery.OnAddExclude;
            // pool3.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;

            entityQuery.ExcludeTypes[entityQuery.ExcludeCount] = ComponentType<T4>.ID;
            // var pool4 = world.GetPool<T4>();
            // pool4.OnAdd += entityQuery.OnAddExclude;
            // pool4.OnRemove += entityQuery.OnRemoveExclude;
            entityQuery.ExcludeCount++;
        }
    }

    #endregion

    public class Query : EntityType {
        public Query(World world) : base(world) { }

        public Query With(int id) {

            IncludeTypes[IncludeCount] = id;
            var pool1 = world.GetPoolByID(id);
            pool1.OnAdd += OnAddInclude;
            pool1.OnRemove += OnRemoveInclude;
            IncludeCount++;
            return this;
        }

        public Query Without(int id) {
            var pool1 = world.GetPoolByID(id);
            pool1.OnAdd += OnAddExclude;
            pool1.OnRemove += OnRemoveExclude;
            ExcludeCount++;
            return this;
        }

        public Query Push() {
            world.OnCreateEntityType(this);
            return this;
        }
    } 
    public class EntityQuery<TWith> : EntityType where TWith : struct, IFilter {
        protected TWith with;
        public EntityQuery(World world) : base(world) {
            with = new TWith();
            with.Setup(this, world);
        }

        public EntityQuery<TWith> Without<T>() where T : struct {
            ExcludeTypes[ExcludeCount] = ComponentType<T>.ID;
            var pool1 = world.GetPool<T>();
            pool1.OnAdd += OnAddExclude;
            pool1.OnRemove += OnRemoveExclude;
            ExcludeCount++;
            return this;
        }

        public void Without(int type) {
            ExcludeTypes[ExcludeCount] = type;
            var pool1 = world.GetPoolByID(type);
            pool1.OnAdd += OnAddExclude;
            pool1.OnRemove += OnRemoveExclude;
            ExcludeCount++;
        }
        public void Push(){
            world.OnCreateEntityType(this);
        }

    }

    public class OwnerQuery<TWith> : EntityQuery<TWith> where TWith : struct, IFilter {
        private readonly int ownerID;
        private readonly Pool<Owner> pool;
        public OwnerQuery(World world, int id) : base(world) {
            ExcludeCount = 0;
            IncludeCount = 0;
            IncludeTypes = new int[8];
            ExcludeTypes = new int[4];
            ownerID = id;
            pool = world.GetPool<Owner>();
            IncludeTypes[IncludeCount] = pool.TypeID;
            pool.OnAdd += OnAddInclude;
            pool.OnRemove += OnRemoveInclude;
            IncludeCount++;
            with = new TWith();
            with.Setup(this, world);
        }
        
        public new OwnerQuery<TWith> Without<T>() where T : struct {
            ExcludeTypes[ExcludeCount] = ComponentType<T>.ID;
            var pool1 = world.GetPool<T>();
            pool1.OnAdd += OnAddExclude;
            pool1.OnRemove += OnRemoveExclude;
            ExcludeCount++;
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Add(int id)
        {
            if (HasEntity(id)) return;
            if(pool.items[id].Value.id != ownerID) return;
            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            Count++;
        }
        public override void OnAddInclude(int id)
        {
            if (HasEntity(id)) return;
            if(pool.items[id].Value.id != ownerID) return;
            ref var data = ref world.GetEntityData(id);
            // for (var i = 0; i < ExcludeCount; i++)
            //     if (data.componentTypes.Contains(ExcludeTypes[i]))
            //         return;
            //
            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void OnRemoveInclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void OnAddExclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap.Add(lastEntityId, indexOfEntityId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void OnRemoveExclude(int id)
        {
            if (HasEntity(id)) return;
            if(pool.items[id].Value.id != ownerID) return;
            ref var data = ref world.GetEntityData(id);
            // for (var i = 0; i < ExcludeCount; i++)
            //     if (data.componentTypes.Contains(ExcludeTypes[i]))
            //         return;
            //
            // for (var i = 0; i < IncludeCount; i++)
            //     if (!data.componentTypes.Contains(IncludeTypes[i]))
            //         return;

            if (entities.Length == Count) Array.Resize(ref entities, world.totalEntitiesCount+2);
            entities[Count] = data.id;
            entitiesMap.Add(data.id, Count);
            Count++;
        }
    }
}