using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace Wargon.ezs {
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Entity : IEquatable<Entity> {
        public int id;
        internal int version;
        public World World;
        public static Entity Null => new();

        public bool Equals(Entity other) {
            return id == other.id && version == other.version && World == other.World;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetVersion() {
            return version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator == (in Entity lhs, in Entity rhs) {
            return lhs.id == rhs.id && lhs.version == rhs.version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator != (in Entity lhs, in Entity rhs) {
            return lhs.id != rhs.id || lhs.version != rhs.version;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = id;
                hashCode = (hashCode * 397) ^ version;
                // ReSharper disable once NonReadonlyMemberInGetHashCode

                hashCode = (hashCode * 397) ^ (World != null ? World.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj) {
            return obj is Entity other && Equals(other);
        }
    }

    public static class EntityExtensionMethods {
        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<A>(in this Entity e, in A component) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version)
                throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>(T component)");
#endif
            if (!data.Has(typeId)) {
                var pool = e.World.GetPool<A>();
                pool.Set(in component, e.id);
                pool.Add(e.id);
                data.archetype.TransferAdd(ref data, typeId);
                e.World.OnAddComponent(typeId, in e);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void AddPtr(in this Entity e, void* component, int typeId) {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method: Entity.AddPtr<{ComponentType.GetTypeValue(typeId).Name}>()");
#endif
            if (data.Has(typeId) == false) {
                var pool = e.World.GetPoolByID(typeId);
                pool.AddPtr(component, e.id);
                pool.Add(e.id);
                data.archetype.TransferAdd(ref data, typeId);
                e.World.OnAddComponent(typeId, in e);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref A Get<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Get<T>()");
#endif
            if (data.Has(typeId)) return ref e.World.GetPool<A>().items[e.id];
            var pool = e.World.GetPool<A>();
            pool.Set(e.id);
            pool.Add(e.id);
            data.archetype.TransferAdd(ref data, typeId);
            e.World.OnAddComponent(typeId, in e);
            return ref pool.items[e.id];
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref A GetRef<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.GetRef<T>()");
#endif
            if (data.Has(typeId)) return ref e.World.GetPool<A>().items[e.id];
            var pool = e.World.GetPool<A>();
            pool.Set(e.id);
            pool.Add(e.id);
            data.archetype.TransferAdd(ref data, typeId);
            e.World.OnAddComponent(typeId, in e);
            return ref pool.items[e.id];
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Add<T>()");
#endif
            var typeId = ComponentType<A>.ID;
            if (data.Has(typeId)) return;
            var pool = e.World.GetPoolByID(typeId);
            pool.Set(e.id);
            pool.Add(e.id);
            data.archetype.TransferAdd(ref data, typeId);
            e.World.OnAddComponent(typeId, in e);
        }
        public static void AddByTypeID(in this Entity e, int typeId) {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.AddByTypeID<T>()");
#endif
            if (data.Has(typeId)) return;
            var pool = e.World.GetPoolByID(typeId);
            pool.Set(e.id);
            pool.Add(e.id);
            data.archetype.TransferAdd(ref data, typeId);
            e.World.OnAddComponent(typeId, in e);
        }
        /// <summary>
        ///     Set component data to entity, if entity already has component of type;
        /// </summary>
        /// <param name="e">Entity</param>
        /// <param name="component">Component data to set</param>
        /// <typeparam name="A">Component Type </typeparam>
        /// <exception cref="Exception"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<A>(in this Entity e, in A component) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Set<T>()");
#endif
            var typeId = ComponentType<A>.ID;
            if (data.Has(typeId)) {
                var pool = e.World.GetPool<A>();
                pool.Set(e.id);
                pool.Set(in component, e.id);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! Method: Entity.Set<T>()");
#endif
            var typeId = ComponentType<A>.ID;
            if (data.Has(typeId)) {
                var pool = e.World.GetPoolByID(typeId);
                pool.Set(e.id);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBoxed(in this Entity e, object component) {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception("ENTITY NULL OR DESTROYED! : Entity.SetBoxed");
#endif
            var typeId = ComponentType.GetID(component.GetType());
            if (data.Has(typeId)) {
                var pool = e.World.GetPoolByID(typeId);
                pool.SetBoxed(component, e.id);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Replace<A>(in this Entity e, A component) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.version != e.version)
                throw new Exception("ENTITY NULL OR DESTROYED! : Entity.Replace");

            var typeId = ComponentType<A>.ID;
            if (data.Has(typeId)) {
                var pool = e.World.GetPool<A>();
                pool.Set(component, e.id);
            }
            else {
                var pool = e.World.GetPool<A>();
                pool.Set(component, e.id);
                pool.Add(e.id);
                e.World.OnAddComponent(typeId, in e);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) throw new Exception($"ENTITY NULL OR DESTROYED! : Entity.Remove<{typeof(A).Name}>()");
#endif
            var typeId = ComponentType<A>.ID;
            if (data.Has(typeId)) {
                e.World.GetPoolByID(typeId).Remove(e.id);
                data.archetype.TransferRemove(ref data, typeId);
                e.World.OnRemoveComponent(typeId, in e);
                if (data.archetype.IsEmpty)
                    e.DestroyLate();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveByTypeID(in this Entity e, int typeId) {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version)
                throw new Exception($"ENTITY NULL OR DESTROYED! : Entity.RemoveByTypeID({ComponentType.GetTypeValue(typeId).Name})");
#endif
            if (data.Has(typeId)) {
                e.World.GetPoolByID(typeId).Remove(e.id);
                data.archetype.TransferRemove(ref data, typeId);
                e.World.OnRemoveComponent(typeId, in e);
                if (data.archetype.IsEmpty)
                    e.DestroyLate();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
#if DEBUG && ENTITY_NULL_CHECK
            if (data.version != e.version) {
                UnityEngine.Debug.LogError($"ENTITY NULL OR DESTROYED! : Entity.Has<{typeof(A).Name}>");
                return false;
            }
#endif
            var typeId = ComponentType<A>.ID;
            return data.Has(typeId);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasDanger<A>(in this Entity e) where A : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<A>.ID;
            return data.Has(typeId);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref EntityData GetEntityData(in this Entity e) {
            return ref e.World.GetEntityData(e.id);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(in this Entity e) {
            ref var data = ref e.World.GetEntityData(e.id);

            foreach (var dataComponentType in data.archetype.mask) e.World.GetPoolByID(dataComponentType).Remove(e.id);

            data.version++;
            data.archetype.TransferDestroy(ref data);
            e.World.OnDestroyEntity(in e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyLate(in this Entity e) {
            e.Add<DestroyEntity>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        ///     Check is entity destroyed or alive
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNULL(in this Entity e) {
            if (e.World == null) return true;
            ref var data = ref e.World.GetEntityData(e.id);
            return data.version != e.version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InternalGetGeneration(in this Entity entity) {
            return entity.version;
        }
    }

    public static class EntityBufferExtensions {
        public static NativeDynamicBuffer<TBufferElement> AddNativeBuffer<TBufferElement>(in this Entity e, int initialCapacity)
            where TBufferElement : unmanaged {
            var component = new NativeDynamicBuffer<TBufferElement>(initialCapacity);
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<NativeDynamicBuffer<TBufferElement>>.ID;
#if DEBUG
            if (data.version != e.version)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method: AddBuffer<{typeof(TBufferElement).Name}>()");
#endif
            if (!data.Has(typeId)) {
                var pool = e.World.GetPool<NativeDynamicBuffer<TBufferElement>>();
                pool.Set(in component, e.id);
                pool.Add(e.id);
                data.archetype.TransferAdd(ref data, typeId);
                e.World.OnAddComponent(typeId, in e);
            }
            return component;
        }

        public static NativeDynamicBuffer<TBufferElement> AddNativeBuffer<TBufferElement>(in this Entity e, int initialCapacity,
            Unity.Collections.Allocator allocator)
            where TBufferElement : unmanaged {
            var component = new NativeDynamicBuffer<TBufferElement>(initialCapacity, allocator);
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<NativeDynamicBuffer<TBufferElement>>.ID;
#if DEBUG
            if (data.version != e.version)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method: AddBuffer<{typeof(TBufferElement).Name}>()");
#endif
            if (!data.Has(typeId)) {
                var pool = e.World.GetPool<NativeDynamicBuffer<TBufferElement>>();
                pool.Set(in component, e.id);
                pool.Add(e.id);
                data.archetype.TransferAdd(ref data, typeId);
                e.World.OnAddComponent(typeId, in e);
            }

            return component;
        }

        public static void RemoveNativeBuffer<TBufferElement>(in this Entity e) where TBufferElement : unmanaged {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.version != e.version)
                throw new Exception(
                    $"ENTITY NULL OR DESTROYED! : Entity.RemoveBuffer<{typeof(TBufferElement).Name}>()");
            var typeId = ComponentType<NativeDynamicBuffer<TBufferElement>>.ID;
            if (data.Has(typeId)) {
                e.World.GetPoolByID(typeId).Remove(e.id);
                data.archetype.TransferRemove(ref data, typeId);
                e.World.OnRemoveComponent(typeId, in e);
                if (data.archetype.IsEmpty)
                    e.DestroyLate();
            }
        }

        public static ref NativeDynamicBuffer<TBufferElement> GetNativeBuffer<TBufferElement>(in this Entity e)
            where TBufferElement : unmanaged {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<NativeDynamicBuffer<TBufferElement>>.ID;
#if DEBUG
            if (data.version != e.version)
                throw new Exception(
                    $"ENTITY NULL OR DESTROYED! Method: Entity.GetBuffer<{typeof(TBufferElement).Name}>()");
#endif
            if (data.Has(typeId)) return ref e.World.GetPool<NativeDynamicBuffer<TBufferElement>>().items[e.id];
            throw new Exception(
                $"ENTITY ISN'T CONTAIN DUNAMIC BUFFER! Method: Entity.GetBuffer<{typeof(TBufferElement).Name}>()");
        }

        public static DynamicBuffer<TBufferElement> AddBuffer<TBufferElement>(in this Entity e, int initialCapacity)
            where TBufferElement : struct {
            var component = new DynamicBuffer<TBufferElement>(initialCapacity);
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<DynamicBuffer<TBufferElement>>.ID;
#if DEBUG
            if (data.version != e.version)
                throw new Exception($"ENTITY NULL OR DESTROYED! Method: AddBuffer<{typeof(TBufferElement).Name}>()");
#endif
            if (!data.Has(typeId)) {
                var pool = e.World.GetPool<DynamicBuffer<TBufferElement>>();
                pool.Set(in component, e.id);
                pool.Add(e.id);
                data.archetype.TransferAdd(ref data, typeId);
                e.World.OnAddComponent(typeId, in e);
            }
            return component;
        }

        public static void RemoveBuffer<TBufferElement>(in this Entity e) where TBufferElement : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            if (data.version != e.version)
                throw new Exception(
                    $"ENTITY NULL OR DESTROYED! : Entity.RemoveBuffer<{typeof(TBufferElement).Name}>()");
            var typeId = ComponentType<DynamicBuffer<TBufferElement>>.ID;
            if (data.Has(typeId)) {
                e.World.GetPoolByID(typeId).Remove(e.id);
                data.archetype.TransferRemove(ref data, typeId);
                e.World.OnRemoveComponent(typeId, in e);
                if (data.archetype.IsEmpty)
                    e.DestroyLate();
            }
        }

        public static ref DynamicBuffer<TBufferElement> GetBuffer<TBufferElement>(in this Entity e)
            where TBufferElement : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<DynamicBuffer<TBufferElement>>.ID;
#if DEBUG
            if (data.version != e.version)
                throw new Exception(
                    $"ENTITY NULL OR DESTROYED! Method: Entity.GetBuffer<{typeof(TBufferElement).Name}>()");
#endif
            if (data.Has(typeId)) return ref e.World.GetPool<DynamicBuffer<TBufferElement>>().items[e.id];
            throw new Exception(
                $"ENTITY ISN'T CONTAIN DUNAMIC BUFFER! Method: Entity.GetBuffer<{typeof(TBufferElement).Name}>()");
        }
        
        public static ref DynamicBuffer<TBufferElement> GetOrCreateBuffer<TBufferElement>(in this Entity e)
            where TBufferElement : struct {
            ref var data = ref e.World.GetEntityData(e.id);
            var typeId = ComponentType<DynamicBuffer<TBufferElement>>.ID;
#if DEBUG
            if (data.version != e.version)
                throw new Exception(
                    $"ENTITY NULL OR DESTROYED! Method: Entity.GetBuffer<{typeof(TBufferElement).Name}>()");
#endif
            if (data.Has(typeId)) return ref e.World.GetPool<DynamicBuffer<TBufferElement>>().items[e.id];
            var pool = e.World.GetPool<DynamicBuffer<TBufferElement>>();
            var component = new DynamicBuffer<TBufferElement>(8);
            pool.Set(in component, e.id);
            pool.Add(e.id);
            data.archetype.TransferAdd(ref data, typeId);
            e.World.OnAddComponent(typeId, in e);
            return ref pool.items[e.id];
        }
    }

    [Serializable]
    public struct EntityData {
        public int id;
        public int version;
        public Archetype archetype;

        public int ComponentsCount {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => archetype.mask.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int type) {
            return archetype.mask.Contains(type);
        }
    }
}