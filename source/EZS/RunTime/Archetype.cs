using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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

        internal EntityQuery GetQueryById(int id) {
            return queries[id];
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
        internal static int GetHashCode(HashSet<int> mask, int owner) {
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
                hash += hash >> owner;
                return hash;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetHashCode(int[] mask, int owner) {
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
                hash += hash >> owner;
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
        private readonly ComponentEdge CreateEdge;
        internal readonly int owner;
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
            this.owner = -1;
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
            this.CreateEdge = new ComponentEdge(new Edge(this), new Edge(null));
            for (int i = 0; i < queries.Count; i++) {
                this.CreateEdge.add.AddToAddEntity(queries[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity() {
            var entity = world.CreateEntity(this);
            foreach (var i in mask) {
                var pool = world.GetPoolByID(i);
                pool.Add(entity.id);
                pool.Set(entity.id);
            }
            entity.GetEntityData().archetype = this;
            world.AddDirtyEntity(entity.id, CreateEdge.add);
            return entity;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Copy(Entity e) {
            var entity = world.CreateEntity(this);
            foreach (var i in mask) {
                var pool = world.GetPoolByID(i);
                pool.Copy(e.id, entity.id);
            }
            entity.GetEntityData().archetype = this;
            world.AddDirtyEntity(entity.id, CreateEdge.add);
            return entity;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateEntity(List<object> components) {
            var entity = world.CreateEntity(this);
            var index = 0;
            foreach (var i in mask) {
                var pool = world.GetPoolByID(i);
                pool.SetBoxed(components[index],entity.id);
                pool.Add(entity.id);
            }
            entity.GetEntityData().archetype = this;
            world.AddDirtyEntity(entity.id, CreateEdge.add);
            return entity;
        }


        internal void TryAddQ(EntityQuery query) {
            if(HasQuery(query)) return;
            if (IsQueryMatch(query)) {
                queries.Add(query);
            }
        }

        internal void UpdateEdges(EntityQuery query) {
            foreach (var edge in componentEdges.Values) {
                var add = edge.add;
                var remove = edge.remove;
                
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

            foreach (var ownerEdge in OwnerEdges.Values) {
                for (var i = 0; i < queries.Count; i++) {
                    if (!ownerEdge.buffer.toMove.HasQuery(query))
                        ownerEdge.buffer.AddToRemoveEntity(query);
                }

                for (var i = 0; i < ownerEdge.buffer.toMove.queries.Count; i++) {
                    if (!HasQuery(query))
                        ownerEdge.buffer.AddToAddEntity(query);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsQueryMatch(EntityQuery q) {
            if (owner != -1 && q.owner != -1 && owner != q.owner) {
                return false;
            }

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
        internal void Remove(int entity) {
            for (int i = 0; i < queries.Count; i++) {
                queries.Items[i].Remove(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void TransferAdd(ref EntityData entity, int component) {
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
        internal void TransferRemove(ref EntityData entity, int component) {
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
        internal void TransferOwnerChange(ref EntityData entity, int newOwner) {
            if(this.owner == newOwner) return;
            if (OwnerEdges.TryGetValue(newOwner, out OwnerEdge edge)) {
                entity.archetype = edge.buffer.toMove;
                world.AddDirtyEntity(entity.id, edge.buffer);
                return;
            }

            CreateOwnerEdge(-1, newOwner, out edge);
            world.AddDirtyEntity(entity.id, edge.buffer);
            entity.archetype = edge.buffer.toMove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferOwnerAdd(ref EntityData entity, int component, int newOwner) {
            if (OwnerEdges.TryGetValue(newOwner, out OwnerEdge edge)) {
                world.AddDirtyEntity(entity.id, edge.buffer);
                entity.archetype = edge.buffer.toMove;
                return;
            }
            CreateOwnerEdge(component, newOwner, out edge);
            world.AddDirtyEntity(entity.id, edge.buffer);
            entity.archetype = edge.buffer.toMove;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TransferDestroy(ref EntityData entity) {
            world.AddDirtyEntity(entity.id, DestroyEdge.remove);
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
            
            edge = new OwnerEdge(GetOrCreateMigration(world.GetOrCreateArchetype(maskAdd, owmer)));
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
            internal Edge add;
            internal Edge remove;

            public ComponentEdge(Edge add, Edge remove) {
                this.add = add;
                this.remove = remove;
            }
        }

        private struct OwnerEdge {
            internal Edge buffer;

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
        [NativeDisableUnsafePtrRestriction] private readonly int* entities;
        [NativeDisableUnsafePtrRestriction] private readonly int* entitiesMap;
        private readonly int count;
        private readonly int entitiesLen;
        public int Count => count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal NativeEntityQuery(EntityQuery query) {
            fixed (int* ptr = query.entities) entities = ptr;
            fixed (int* ptr = query.entitiesMap) entitiesMap = ptr;
            count = query.count;
            entitiesLen = query.entities.Length;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntity(int index) => entities[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int entity) {
            if (entitiesLen <= entity) return false;
            return entitiesMap[entity] > 0;
        }
    }

    public unsafe struct NEntityQuery : IDisposable {
        internal Internal* ptr;

        public NEntityQuery(World world, int id) {
            ptr = (Internal*)UnsafeUtility.Malloc(sizeof(Internal), UnsafeUtility.AlignOf<Internal>(),
                Allocator.Persistent);
            ptr->entities = new UnsafeList<int>(128, Allocator.Persistent);
            ptr->entitiesMap = new UnsafeList<int>(128, Allocator.Persistent);
            ptr->worldID = world.ID;
            ptr->id = id;
            ptr->owner = -1;
            ptr->withCount = 0;
            ptr->noneCount = 0;
        }
        
        public void Dispose() {
            ptr->Clear();
            UnsafeUtility.Free(ptr, Allocator.Persistent);
        }
        internal ref Internal Ptr => ref *ptr;
        internal struct Internal {
            internal fixed int with[12];
            internal int withCount;
            internal fixed int none[8];
            internal int noneCount;
            internal int count;
            internal UnsafeList<int> entities;
            internal UnsafeList<int> entitiesMap;
            internal int owner;
            internal int id;
            internal int worldID;
            
            internal void Clear() {
                entities.Dispose();
                entitiesMap.Dispose();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal unsafe void Add(int entity) {
                if (entities.Length - 1 <= count) {
                    entities.Resize(count+16);
                }
                if (entitiesMap.Length - 1 <= entity) {
                    entitiesMap.Resize(count+16);
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
            public void With<T>() where T: struct {
                with[withCount++] = ComponentType<T>.ID;
                fixed (int* p = with) {
                    var s = p->GetHashCode();
                }
            }

            public void Without<T>()  where T: struct {
                none[noneCount++] = ComponentType<T>.ID;
            }
        }
        
        public NEntityQuery With<T>()  where T : struct {
            ptr->With<T>();
            return this;
        }

        public NEntityQuery Without<T>() where T : struct {
            ptr->Without<T>();
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEntityIndex(int index) {
            return ptr->entities[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Entity GetEntity(int index) {
            return ref Worlds.GetWorld(ptr->worldID).GetEntity(ptr->entities[index]);
        }

    }
    
    public class EntityQuery {
        internal readonly DynamicArray<int> with;
        internal readonly DynamicArray<int> none;
        /// <summary>
        /// Is there no entities in query
        /// </summary>
        public bool IsEmpty {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count == 0;
        }
        /// <summary>
        /// Entities amount in query
        /// </summary>
        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get {
                return count;
            }   
        }
        internal int count;
        internal int[] entities;
        internal int[] entitiesMap;
        internal int owner;
        public Entity Owner => world.GetEntity(owner);
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
            lockedEntities = new DynamicArray<LockedEntity>(64);
            owner = -1;
        }

        public EntityQuery SetCapacity(int capacity) {
            Array.Resize(ref entities, capacity);
            Array.Resize(ref entitiesMap, capacity);
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(int entity) {
            if (locked) {
                AddLocked(entity);
                return;
            }
            if (entities.Length - 1 <= count) {
                Array.Resize(ref entities, count + 16);
            }
            if (entitiesMap.Length - 1 <= entity) {
                Array.Resize(ref entitiesMap, entity + 16);
            }
            entities[count++] = entity;
            entitiesMap[entity] = count;
            if (onAddNotNull)
                onAdd(entity, world);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int entity) {
            if (locked) {
                RemoveLocked(entity);
                return;
            }
            if (Has(entity) == false) return;
            var index = entitiesMap[entity] - 1;
            entitiesMap[entity] = 0;
            count--;
            if (count > index) {
                entities[index] = entities[count];
                entitiesMap[entities[index]] = index + 1;
            }
            if (onRemoveNotNull)
                onRemove(entity, world);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int entity) {
            if (entitiesMap.Length <= entity) return false;
            return entitiesMap[entity] > 0;
        }
        
        public EntityQuery With<T>() where T: struct {
            with.Add(ComponentType<T>.ID);
            return this;
        }

        public EntityQuery Without<T>()  where T: struct {
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
        /// <summary>
        /// Sub for invoke when entity added to query
        /// </summary>
        /// <param name="func">Action(int entityID, Wolrd world)</param>
        /// <returns></returns>
        public unsafe EntityQuery OnAdd(Action<int, World> func) {
            onAdd = func;
            onAddNotNull = true;
            return this;
        }
        /// <summary>
        /// Sub for invoke when entity removed from query
        /// </summary>
        /// <param name="func">Action(int entityID, Wolrd world)</param>
        /// <returns></returns>
        public unsafe EntityQuery OnRemove(Action<int, World> func) {
            onRemove = func;
            onRemoveNotNull = true;
            return this;
        }
        private bool onAddNotNull;
        private bool onRemoveNotNull;
        private unsafe Action<int, World> onAdd;
        private unsafe Action<int, World> onRemove;

        struct LockedEntity {
            public int entity;
            public bool add;
        }

        private readonly DynamicArray<LockedEntity> lockedEntities;
        private bool locked;
        
        private int chunkIndex;
        private int splitedFrames;
        private int lockedFramesCounter;
        private int lockedCount;
        private int lastFrameLockedCount;
        public bool Locked => locked;
        public int LockedCount {
            get {
                return lockedCount;
            }
        }

        public int SplitedFrames => splitedFrames;
        public void SpliteFor(int value) {
            splitedFrames = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Entity GetLockedEntity(int index) {
            return ref world.GetEntity(entities[chunkIndex + index]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Lock() {
            locked = true;
            lockedFramesCounter = splitedFrames;
            chunkIndex = 0;
            GetLockedEntitiesCount();
            //Debug.Log("LOCKED");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetLockedEntitiesCount() {
            lockedCount = count / splitedFrames;
            lastFrameLockedCount = count - lockedCount * splitedFrames;
            //Debug.Log(count/splitedFrames);
            //Debug.Log($"count = {count} | lockedCount = {lockedCount} | lastFrameLockedCount = {lastFrameLockedCount}" );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryLock() {
            if (locked) return false;
            if (lockedFramesCounter == 0) {
                Lock();
                return true;
            }

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryUnlock() {
            lockedFramesCounter--;
            
            if (lockedFramesCounter == 1) {
                lockedCount = lastFrameLockedCount;
                //Debug.Log($"lockedCount : {lockedCount}");
                return false;
            }
            chunkIndex += lockedCount;
            //Debug.Log($"count = {count} | lockedCount = {lockedCount} | chunkIndex = {chunkIndex} | lockedFramesCounter = {lockedFramesCounter}  | lastFrameLockedCount = {lastFrameLockedCount}" );
            if (lockedFramesCounter == 0) {
                Unlock();
                //Debug.Log("UNLOCKED");
                return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Unlock() {
            for (var i = 0; i < lockedEntities.Count; i++) {
                ref var e = ref lockedEntities.Items[i];
                if (e.add) {
                    if (entities.Length - 1 <= count) {
                        Array.Resize(ref entities, count + 16);
                    }
                    if (entitiesMap.Length - 1 <= e.entity) {
                        Array.Resize(ref entitiesMap, e.entity + 16);
                    }
                    entities[count++] = e.entity;
                    entitiesMap[e.entity] = count;
                }
                else {
                    if (!Has(e.entity)) continue;
                    var index = entitiesMap[e.entity] - 1;
                    entitiesMap[e.entity] = 0;
                    count--;
                    if (count > index) {
                        entities[index] = entities[count];
                        entitiesMap[entities[index]] = index + 1;
                    }
                }
            }
            lockedEntities.ClearCount();
            locked = false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddLocked(int entity) {
            lockedEntities.Add(new LockedEntity{entity = entity, add = true});
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveLocked(int entity) {
            lockedEntities.Add(new LockedEntity{entity = entity, add = false});
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