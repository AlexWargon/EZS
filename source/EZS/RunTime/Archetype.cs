using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Wargon.ezs {

    public partial class World {
        internal DynamicArray<EntityQuery> queries = new DynamicArray<EntityQuery>(64);
        internal int queriesCount = 0;
        public EntityQuery GetQuery() {
            var q = new EntityQuery(this);
            queries.Add(q);
            queriesCount++;
            return q;
        }

        public DynamicArray<EntityQuery> GetAllQueries() => queries;
    }

    public partial class World {
        private readonly Dictionary<int, Archetype> archetypesGet = new Dictionary<int, Archetype>();
        private readonly DynamicArray<Archetype> archetypesArray = new DynamicArray<Archetype>(32);
        private Archetype emptyArchetype;
        private int archetypesCount = 0;
        public int ArchetypesCount() => archetypesCount;
        public DynamicArray<Archetype> GetAllArchetypes() => archetypesArray;
        private Archetype CreateFirstArchetype() {
            var archetype = new Archetype(this);
            archetypesGet.Add(0, archetype);
            archetypesArray.Add(archetype);
            emptyArchetype = archetype;
            archetypesCount++;
            return archetype;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Archetype GetOrCreateArchetype(HashSet<int> mask, int owner) {
            var id = GetHashCode(mask, owner);
            if (archetypesGet.TryGetValue(id, out var archetype))
                return archetype;

            archetype = new Archetype(this, mask, id, owner);
            archetypesGet.Add(id, archetype);
            archetypesArray.Add(archetype);
            return archetype;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Archetype GetOrCreateArchetype(params Type[] param) {
            var mask = new HashSet<int>(param.Length);
            foreach (var type in param) {
                mask.Add(ComponentType.GetID(type));
            }
            var id = GetHashCode(mask, -1);
            if (archetypesGet.TryGetValue(id, out var archetype))
                return archetype;
            
            archetype = new Archetype(this, mask, id, -1);
            archetypesGet.Add(id, archetype);
            archetypesArray.Add(archetype);
            return archetype;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Archetype GetOrCreateArchetype(params int[] param) {
            var id = GetHashCode(param, -1);
            if (archetypesGet.TryGetValue(id, out var archetype))
                return archetype;
            var mask = new HashSet<int>(param);
            archetype = new Archetype(this, mask, id, -1);
            archetypesGet.Add(id, archetype);
            archetypesArray.Add(archetype);
            return archetype;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(HashSet<int> mask, int owner) {
            unchecked {
                int hash = (int) 2166136261;
                const int p = 16777619;
                foreach (int i in mask) {
                    hash = (hash ^ i) * p;
                }
                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                hash ^= hash >> owner;
                return hash;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(int[] mask, int owner) {
            unchecked {
                int hash = (int) 2166136261;
                const int p = 16777619;
                for (var index = 0; index < mask.Length; index++) {
                    int i = mask[index];
                    hash = (hash ^ i) * p;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                hash ^= hash >> owner;
                return hash;
            }
        }
    }
    
    internal struct DirtyEntity {
        internal int id;
        internal Archetype.Edge edge;
    }
    
    public partial class World {
        private readonly DynamicArray<DirtyEntity> dirtyEntities = new DynamicArray<DirtyEntity>(Configs.EntityCacheSize);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddDirtyEntity(int entity, Archetype.Edge migrationEdge) {
            ref var e = ref dirtyEntities.GetLastWithIncrement();
            e.edge = migrationEdge;
            e.id = entity;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateWorld() {
            for (int i = 0; i < dirtyEntities.Count; i++) {
                ref var e = ref dirtyEntities.GetByIndex(i);
                e.edge.Execute(e.id);
            }
            dirtyEntities.ClearCount();
        }
    }

    public sealed class Archetype {
        private readonly World world;
        internal readonly HashSet<int> mask;
        public HashSet<int> Mask => mask;
        private readonly DynamicArray<EntityQuery> queries;
        private readonly Dictionary<int, ComponentEdge> componentEdges;
        private readonly Dictionary<int, OwnerEdge> OwnerEdges;
        private readonly ComponentEdge DestroyEdge;
        internal readonly int owner = -1;
        private readonly int id;
        public int ID => id;
        internal readonly bool IsEmpty;
        public Archetype(World world) {
            this.world = world;
            this.mask = new HashSet<int>();
            this.queries = DynamicArray<EntityQuery>.Empty();
            this.componentEdges = new Dictionary<int, ComponentEdge>();
            this.OwnerEdges = new Dictionary<int, OwnerEdge>();
            this.IsEmpty = true;
            this.id = 0;
        }

        public Archetype(World world, HashSet<int> mask, int id, int owner) {
            this.world = world;
            this.mask = mask;
            this.id = id;
            this.componentEdges = new Dictionary<int, ComponentEdge>();
            this.OwnerEdges = new Dictionary<int, OwnerEdge>();
            this.queries = new DynamicArray<EntityQuery>(4);
            this.owner = owner;
            for (int i = 0; i < world.queriesCount; i++) {
                var q = world.queries.Items[i];
                if (IsQueryMatch(q)) {
                    queries.Add(q);
                }
            }

            this.DestroyEdge = new ComponentEdge(new Edge(null), new Edge(null));
            for (int i = 0; i < queries.Count; i++) {
                this.DestroyEdge.remove.AddToRemoveEntity(queries[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() {
            var e = world.CreateEntity(this);
            return e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity(List<object> components) {
            var e = world.CreateEntity(this);
            var index = 0;
            foreach (var i in mask) {
                var pool = world.GetPoolByID(i);
                pool.SetBoxed(components[index],e.id);
                pool.Add(e.id);
            }
            
            for (var i = 0; i < queries.Count; i++) {
                queries[i].Add(e.id);
            }
            return e;
        }


        internal void TryAddQ(EntityQuery query) {
            if(HasQuery(query)) return;
            if (IsQueryMatch(query)) {
                queries.Add(query);
            }
        }

        internal void UpdateEdges(EntityQuery query) {
            foreach (var ComponentEdge in componentEdges.Values) {
                var add = ComponentEdge.add;
                var remove = ComponentEdge.remove;
                
                for (var i = 0; i < queries.Count; i++) {
                    if (!add.toMove.HasQuery(query))
                        add.AddToRemoveEntity(query);
                }

                for (var i = 0; i < add.toMove.queries.Count; i++) {
                    if (!HasQuery(query))
                        add.AddToAddEntity(query);
                }
                
                for (var i = 0; i < queries.Count; i++) {
                    if (!remove.toMove.HasQuery(query))
                        remove.AddToRemoveEntity(query);
                }

                for (var i = 0; i < remove.toMove.queries.Count; i++) {
                    if (!HasQuery(query))
                        remove.AddToAddEntity(query);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsQueryMatch(EntityQuery q) {
            if (owner != -1 && q.owner != -1 && owner != q.owner) return false;

            for (var i = 0; i < q.none.Count; i++) {
                if (mask.Contains(q.none.Items[i]))
                    return false;
            }

            var match = 0;
            for (var i = 0; i < q.with.Count; i++) {
                if (mask.Contains(q.with.Items[i])) {
                    match++;
                    if (match == q.with.Count) {
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int entity) {
            for (int i = 0; i < queries.Count; i++) {
                queries.Items[i].Remove(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferAdd(ref EntityData entity, int component) {
            if (componentEdges.TryGetValue(component, out ComponentEdge edge)) {
                world.AddDirtyEntity(entity.id, edge.add);
                entity.archetype = edge.add.toMove;
                return;
            }

            CreateComponentEdge(component, this.owner, out edge);
            world.AddDirtyEntity(entity.id, edge.add);
            entity.archetype = edge.add.toMove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferRemove(ref EntityData entity, int component) {
            if (componentEdges.TryGetValue(component, out ComponentEdge edge)) {
                entity.archetype = edge.remove.toMove;
                world.AddDirtyEntity(entity.id, edge.remove);
                return;
            }
            if(component == ComponentType<Owner>.ID)
                CreateComponentEdge(component, -1, out edge);
            else
                CreateComponentEdge(component, this.owner, out edge);
            world.AddDirtyEntity(entity.id, edge.remove);
            entity.archetype = edge.remove.toMove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferOwnerChange(ref EntityData entity, int component, int owner) {
            if(this.owner==owner) return;
            if (OwnerEdges.TryGetValue(owner, out OwnerEdge edge)) {
                entity.archetype = edge.buffer.toMove;
                world.AddDirtyEntity(entity.id, edge.buffer);
                return;
            }

            CreateOwnerEdge(component, owner, out edge);
            world.AddDirtyEntity(entity.id, edge.buffer);
            entity.archetype = edge.buffer.toMove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferOwnerAdd(ref EntityData entity, int component, int owner) {
            if (OwnerEdges.TryGetValue(owner, out OwnerEdge edge)) {
                world.AddDirtyEntity(entity.id, edge.buffer);
                entity.archetype = edge.buffer.toMove;
                return;
            }
            CreateOwnerEdge(component, owner, out edge);
            world.AddDirtyEntity(entity.id, edge.buffer);
            entity.archetype = edge.buffer.toMove;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferDestroy(ref EntityData entity) {
            world.AddDirtyEntity(entity.id, DestroyEdge.remove);
            entity.version++;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CreateComponentEdge(int component, int owmer, out ComponentEdge edge) {
            var maskAdd = new HashSet<int>(mask) { component };
            var maskRemove = new HashSet<int>(mask);
            maskRemove.Remove(component);
            
            edge = new ComponentEdge(
                GetOrCreateMigration(world.GetOrCreateArchetype(maskAdd, owmer))
                , GetOrCreateMigration(world.GetOrCreateArchetype(maskRemove, owmer)));
            componentEdges.Add(component,edge);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CreateOwnerEdge(int component, int owmer, out OwnerEdge edge) {
            
            var maskAdd = new HashSet<int>(mask);
            if (component != -1) maskAdd.Add(component);
            
            edge = new OwnerEdge(
                GetOrCreateMigration(world.GetOrCreateArchetype(maskAdd, owmer)));
            OwnerEdges.Add(owmer,edge);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Edge GetOrCreateMigration(Archetype archetypeNext) {
            Edge migrationEdge = new(archetypeNext);
            for (var i = 0; i < queries.Count; i++) {
                var query = queries[i];
                if (!archetypeNext.HasQuery(query))
                    migrationEdge.AddToRemoveEntity(query);
            }

            for (var i = 0; i < archetypeNext.queries.Count; i++) {
                var query = archetypeNext.queries[i];
                if (!HasQuery(query))
                    migrationEdge.AddToAddEntity(query);
            }
            return migrationEdge;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasQuery(EntityQuery query) {
            for (var i = 0; i < queries.Count; i++)
                if (queries[i].id == query.id)
                    return true;
            return false;
        }

        private struct ComponentEdge {
            internal readonly Edge add;
            internal Edge remove;

            public ComponentEdge(Edge add, Edge remove) {
                this.add = add;
                this.remove = remove;
            }
        }

        private readonly struct OwnerEdge {
            internal readonly Edge buffer;

            public OwnerEdge(Edge buffer) {
                this.buffer = buffer;
            }
        }

        internal struct Edge {
            private readonly DynamicArray<EntityQuery> addEntity;
            private readonly DynamicArray<EntityQuery> removeEntity;
            internal readonly Archetype toMove;
            private bool IsEmpty;
            public Edge(Archetype toMove) {
                this.toMove = toMove;
                this.addEntity = new DynamicArray<EntityQuery>(4);
                this.removeEntity = new DynamicArray<EntityQuery>(4);
                IsEmpty = true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Execute(int entity) {
                if(IsEmpty) return;
                for (int i = 0; i < removeEntity.Count; i++) {
                    removeEntity.Items[i].Remove(entity);
                }
                for (int i = 0; i < addEntity.Count; i++) {
                    addEntity.Items[i].Add(entity);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void AddToAddEntity(EntityQuery q) {
                addEntity.Add(q);
                IsEmpty = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void AddToRemoveEntity(EntityQuery q) {
                removeEntity.Add(q);
                IsEmpty = false;
            }
        }
    }
    public unsafe readonly struct NativeEntityQuery {
        [NativeDisableUnsafePtrRestriction] private readonly int* entnties;
        [NativeDisableUnsafePtrRestriction] private readonly int* entitiesMap;
        private readonly int count;
        public int Count => count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NativeEntityQuery(EntityQuery query) {
            fixed (int* ptr = query.entities) entnties = ptr;
            fixed (int* ptr = query.entitiesMap) entitiesMap = ptr;
            count = query.count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntity(int index) => entnties[index];
    }
    public class EntityQuery {
        internal readonly DynamicArray<int> with;
        internal readonly DynamicArray<int> none;
        public int Count => count;
        internal int count;
        internal int[] entities;
        internal int[] entitiesMap;
        internal int owner;
        internal readonly int id;
        internal readonly World world;

        public EntityQuery(World world) {
            this.world = world;
            with = new DynamicArray<int>(12);
            none = new DynamicArray<int>(8);
            id = world.queriesCount;
            count = 0;
            entities = new int[Configs.EntityCacheSize];
            entitiesMap = new int[Configs.EntityCacheSize];
            owner = -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(int entity) {
            if (entities.Length - 1 <= count) {
                Array.Resize(ref entities, count + 16);
            }
            if (entitiesMap.Length - 1 <= entity) {
                Array.Resize(ref entitiesMap, entity + 16);
            }
            entities[count++] = entity;
            entitiesMap[entity] = count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int entity) {
            if (!Has(entity)) return;
            var index = entitiesMap[entity] - 1;
            entitiesMap[entity] = 0;
            count--;
            if (count > index) {
                entities[index] = entities[count];
                entitiesMap[entities[index]] = index + 1;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Has(int entity) {
            if (entitiesMap.Length <= entity) return false;
            return entitiesMap[entity] > 0;
        }
        
        public EntityQuery With<T>() {
            with.Add(ComponentType<T>.ID);
            return this;
        }

        public EntityQuery Without<T>() {
            none.Add(ComponentType<T>.ID);
            return this;
        }

        internal EntityQuery With(int componentType) {
            with.Add(componentType);
            return this;
        }

        internal EntityQuery Without(int componentType) {
            none.Add(componentType);
            return this;
        }
        
        public EntityQuery WithOwner(int entity) {
            owner = entity;
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntityIndex(int index) {
            return entities[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Entity GetEntity(int index) {
            return ref world.GetEntity(entities[index]);
        }

        public EntityQuery Push() {
            // var archetypes = world.GetAllArchetypes();
            // for (int i = 0; i < archetypes.Count; i++) {
            //     archetypes[i].TryAddQ(this);
            // }
            // for (int i = 0; i < archetypes.Count; i++) {
            //     archetypes[i].UpdateEdges(this);
            // }
            return this;
        }

        public override string ToString() {
            var types = string.Empty;
            for (int i = 0; i < with.Count; i++) {
                types += ComponentType.GetTypeValue(with[i]).Name;
                types += ",";
            }

            var notypes = string.Empty;
            for (int i = 0; i < none.Count; i++) {
                notypes += ComponentType.GetTypeValue(none[i]).Name;
                notypes += ",";
            }
            return $"EntityQuery.With[{types}]_Without[{notypes}]";
        }
    }

    public static class EcsExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Archetype GetArchetype(this ref Entity entity) {
            return entity.World.GetEntityData(entity.id).archetype;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeEntityQuery AsNative(this EntityQuery query) {
            return new NativeEntityQuery(query);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(this EntityQuery query, in Entity entity) {
            return query.Has(entity.id);
        }
    }

    public static class EzAllocator {
        public static int allocated;
        public static int released;

        public static unsafe T* Allocate<T>(int items) where T : unmanaged {
            if (UnsafeUtility.IsValidAllocator(Allocator.Persistent)) {
                allocated++;
                return (T*)UnsafeUtility.Malloc(sizeof(T) * items, UnsafeUtility.AlignOf<T>(), Allocator.Persistent);
            }
            throw new Exception($"{Allocator.Persistent} not valide");
        }
        public static unsafe void FreeDisposable<T>(T* disposable) where T : unmanaged, IDisposable {
            if (disposable == null) return;
            disposable->Dispose();
            if (UnsafeUtility.IsValidAllocator(Allocator.Persistent)) {
                UnsafeUtility.Free(disposable, Allocator.Persistent);
                released++;
            }
            else {
                throw new Exception($"{Allocator.Persistent} not valide");
            }
        }
        public static unsafe void Free<T>(T* disposable) where T : unmanaged {
            if (UnsafeUtility.IsValidAllocator(Allocator.Persistent)) {
                UnsafeUtility.Free(disposable, Allocator.Persistent);
                released++;
            }
            else {
                throw new Exception($"{Allocator.Persistent} not valide");
            }
        }
        public static void Clear() {
            released = 0;
            allocated = 0;
        }
        
    }
}