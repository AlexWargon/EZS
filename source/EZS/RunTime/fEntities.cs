using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs {

    public struct EntitiesEach { }

    public static partial class EntitiesExtensions {

        public static EntitiesEach With<T1>(this EntitiesEach entities) where T1 : struct {
            return entities;
        }

        public static EntitiesEach WithAll<T1, T2>(this EntitiesEach entities) where T1 : struct where T2 : struct {
            return entities;
        }

        public static EntitiesEach WithAll<T1, T2, T3>(this EntitiesEach entities)
            where T1 : struct where T2 : struct where T3 : struct {
            return entities;
        }

        public static EntitiesEach Without<T1>(this EntitiesEach entities) where T1 : struct {
            return entities;
        }

        public static EntitiesEach Without<T1, T2>(this EntitiesEach entities) where T1 : struct where T2 : struct {
            return entities;
        }

        public static EntitiesEach Without<T1, T2, T3>(this EntitiesEach entities)
            where T1 : struct where T2 : struct where T3 : struct {
            return entities;
        }

        public static EntitiesEach Any<T1, T2>(this EntitiesEach entities) where T1 : struct where T2 : struct {
            return entities;
        }

        public static EntitiesEach Any<T1, T2, T3>(this EntitiesEach entities)
            where T1 : struct where T2 : struct where T3 : struct {
            return entities;
        }

        public static EntitiesEach WithOwner(this EntitiesEach entities, int id) {
            return entities;
        }

        // public static EntitiesEach WithOwner(this EntitiesEach entities, Entity entity) {
        //     return entities;
        // }

        public static EntitiesEach WithJob(this EntitiesEach entities) {
            return entities;
        }

        public static EntitiesEach WithJobParallel(this EntitiesEach entities) {
            return entities;
        }
    }
}