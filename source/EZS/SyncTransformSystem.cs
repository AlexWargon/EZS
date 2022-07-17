using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Wargon.ezs;
using Wargon.ezs.Unity;

[EcsComponent]
public class TransformRef
{
    public Transform Value;
}
public class SyncTransformSystem : UpdateSystem, IJobSystemTag
{
    private SyncTransforms syncTransforms;

    public override void Init(Entities entities, World world)
    {
        base.Init(entities, world);
        syncTransforms = new SyncTransforms(world);
        entities.Without<UnActive, NoBurst>().AddNewEntityType<SyncTransforms>(syncTransforms);
    }

    public override void Update()
    {
        syncTransforms.Synchronize();
    }

    private class SyncTransforms : EntityType, IDisposable
    {
        private readonly Pool<TransformRef> classComponents;
        private readonly Pool<TransformComponent> structComponents;
        private bool disposed;
        private TransformSynchronizeJob job;
        private TransformAccessArray transformAccessArray;
        private readonly World world;

        public SyncTransforms(World world) : base(world)
        {
            this.world = world;
            IncludCount = 2;
            IncludTypes = new[]
            {
                ComponentType<TransformComponent>.ID,
                ComponentType<TransformRef>.ID
            };
            ExludeCount = 2;
            ExcludeTypes = new[]
            {
                ComponentType<UnActive>.ID,
                ComponentType<NoBurst>.ID
            };
            structComponents = world.GetPool<TransformComponent>();
            classComponents = world.GetPool<TransformRef>();
            transformAccessArray = new TransformAccessArray(world.EntityCacheSize);
            Count = 0;
            structComponents.OnAdd += OnAddInclude;
            structComponents.OnRemove += OnRemoveInclude;
            classComponents.OnAdd += OnAddInclude;
            classComponents.OnRemove += OnRemoveInclude;
            var PoolEx = world.GetPool<UnActive>();
            PoolEx.OnAdd += OnAddExclude;
            PoolEx.OnRemove += OnRemoveExclude;
            var PoolEx2 = world.GetPool<NoBurst>();
            PoolEx2.OnAdd += OnAddExclude;
            PoolEx2.OnRemove += OnRemoveExclude;
            world.OnCreateEntityType(this);
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            transformAccessArray.Dispose();
            disposed = true;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal new void OnAddInclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;

            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            transformAccessArray.Add(classComponents.items[id].Value);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnRemoveExclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;
            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            transformAccessArray.Add(classComponents.items[id].Value);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnRemoveInclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnAddExclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Remove(Entity entity)
        {
            if (!entitiesMap.ContainsKey(entity.id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[entity.id];
            entitiesMap.Remove(entity.id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Synchronize()
        {
            if (disposed) return;
            if (Count < 1) return;
            var data = NativeMagic.WrapToNative(structComponents.items);
            TransformSynchronizeJob job = default;
            job.transformComponents = data;
            job.entities = NativeMagic.WrapToNative(entities);
            job.Schedule(transformAccessArray).Complete();
#if UNITY_EDITOR
            job.Clear();
#endif
        }

        internal override void Clear()
        {
#if UNITY_EDITOR
            Debug.Log("TRANSFORMS DISPOSED");
            transformAccessArray.Dispose();
            disposed = true;
#endif
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct TransformSynchronizeJob : IJobParallelForTransform
        {
            [NativeDisableParallelForRestriction] public NativeWrappedData<TransformComponent> transformComponents;
            [NativeDisableParallelForRestriction] public NativeWrappedData<int> entities;

            public void Execute(int index, TransformAccess transform)
            {
                var entity = entities.Array[index];

                var transformComponent = transformComponents.Array[entity];
                transformComponent.right = transformComponent.rotation * Vector3.right;
                transformComponent.forward = transformComponent.rotation * Vector3.forward;
                transform.position = transformComponent.position;
                transform.rotation = transformComponent.rotation;
                transform.localScale = transformComponent.scale;


                transformComponent.position = transform.position;
                transformComponents.Array[entity] = transformComponent;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear()
            {
#if UNITY_EDITOR
                NativeMagic.UnwrapFromNative(transformComponents);
                NativeMagic.UnwrapFromNative(entities);
#endif
            }
        }
    }
}

[EcsComponent]
public class SyncBackTransformTag
{
}

public class SyncBackTransformSystem : UpdateSystem, IJobSystemTag
{
    private Transforms transforms;

    public override void Init(Entities entities, World world)
    {
        base.Init(entities, world);
        transforms = new Transforms(world);
        entities.Without<UnActive>().AddNewEntityType<Transforms>(transforms);
    }

    public override void Update()
    {
        transforms.Synchronize();
    }

    private class Transforms : EntityType
    {
        private readonly Pool<TransformRef> classComponents;
        private readonly Pool<TransformComponent> structComponents;
        private bool disposed;
        private TransformSynchronizeJob job;
        private TransformAccessArray transformAccessArray;
        private readonly World world;

        public Transforms(World world) : base(world)
        {
            this.world = world;
            IncludCount = 3;
            IncludTypes = new[]
            {
                ComponentType<TransformComponent>.ID,
                ComponentType<TransformRef>.ID,
                ComponentType<SyncBackTransformTag>.ID
            };
            ExludeCount = 1;
            ExcludeTypes = new[]
            {
                ComponentType<UnActive>.ID
            };
            structComponents = world.GetPool<TransformComponent>();
            classComponents = world.GetPool<TransformRef>();
            transformAccessArray = new TransformAccessArray(world.EntityCacheSize);
            Count = 0;
            structComponents.OnAdd += OnAddInclude;
            structComponents.OnRemove += OnRemoveInclude;
            classComponents.OnAdd += OnAddInclude;
            classComponents.OnRemove += OnRemoveInclude;

            var poolIn = world.GetPool<SyncBackTransformTag>();
            poolIn.OnAdd += OnAddInclude;
            poolIn.OnRemove += OnRemoveInclude;

            var PoolEx = world.GetPool<UnActive>();
            PoolEx.OnAdd += OnAddExclude;
            PoolEx.OnRemove += OnRemoveExclude;
            var PoolEx2 = world.GetPool<NoBurst>();
            PoolEx2.OnAdd += OnAddExclude;
            PoolEx2.OnRemove += OnRemoveExclude;
            world.OnCreateEntityType(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal new void OnAddInclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;

            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            transformAccessArray.Add(classComponents.items[id].Value);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnRemoveExclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;
            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            transformAccessArray.Add(classComponents.items[id].Value);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnRemoveInclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnAddExclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Remove(Entity entity)
        {
            if (!entitiesMap.ContainsKey(entity.id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[entity.id];
            entitiesMap.Remove(entity.id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Synchronize()
        {
            if (disposed) return;
            if (Count < 1) return;
            var data = NativeMagic.WrapToNative(structComponents.items);
            job = default;
            job.transformComponents = data;
            job.entities = NativeMagic.WrapToNative(entities);
            job.Schedule(transformAccessArray).Complete();
#if UNITY_EDITOR
            job.Clear();
#endif
        }

        internal override void Clear()
        {
#if UNITY_EDITOR
            Debug.Log("TRANSFORMS DISPOSED");
            transformAccessArray.Dispose();
            disposed = true;
#endif
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct TransformSynchronizeJob : IJobParallelForTransform
        {
            [NativeDisableParallelForRestriction] public NativeWrappedData<TransformComponent> transformComponents;
            [NativeDisableParallelForRestriction] public NativeWrappedData<int> entities;

            public void Execute(int index, TransformAccess transform)
            {
                var entity = entities.Array[index];

                var transformComponent = transformComponents.Array[entity];
                transformComponent.right = transformComponent.rotation * Vector3.right;
                transformComponent.forward = transformComponent.rotation * Vector3.forward;
                transformComponent.position = transform.position;
                transformComponent.rotation = transform.rotation;
                transformComponent.scale = transform.localScale;
                transformComponent.position = transform.position;
                transformComponents.Array[entity] = transformComponent;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear()
            {
#if UNITY_EDITOR
                NativeMagic.UnwrapFromNative(transformComponents);
                NativeMagic.UnwrapFromNative(entities);
#endif
            }
        }
    }
}

[EcsComponent]
public struct NoBurst
{
}

public class Collision2DTriggerSystem : UpdateSystem, IJobSystemTag
{
    private Transforms transforms;

    public override void Init(Entities entities, World world)
    {
        base.Init(entities, world);
        transforms = new Transforms(world);
        entities.Without<UnActive, NoBurst>().AddNewEntityType<Transforms>(transforms);
        // transform = new Transforms<TransformComponent>.WithOut<NoBurst>(world);
        // entities.Without<NoBurst>().EntityTypes.Add(type<Transforms<TransformComponent>.WithOut<NoBurst>>.Value, transform);
    }

    public override void Update()
    {
        transforms.Synchronize();
        //transform.OnUpdate(ref jobTransform);
    }

    private class Transforms : EntityType, IDisposable
    {
        private readonly Pool<TransformRef> classComponents;
        private readonly Pool<TransformComponent> structComponents;
        private bool disposed;
        private TransformSynchronizeJob job;
        private TransformAccessArray transformAccessArray;
        private readonly World world;

        public Transforms(World world) : base(world)
        {
            this.world = world;
            IncludCount = 2;
            IncludTypes = new[]
            {
                ComponentType<TransformComponent>.ID,
                ComponentType<TransformRef>.ID
            };
            ExludeCount = 2;
            ExcludeTypes = new[]
            {
                ComponentType<UnActive>.ID,
                ComponentType<NoBurst>.ID
            };
            structComponents = world.GetPool<TransformComponent>();
            classComponents = world.GetPool<TransformRef>();
            transformAccessArray = new TransformAccessArray(world.EntityCacheSize);
            Count = 0;
            structComponents.OnAdd += OnAddInclude;
            structComponents.OnRemove += OnRemoveInclude;
            classComponents.OnAdd += OnAddInclude;
            classComponents.OnRemove += OnRemoveInclude;
            var PoolEx = world.GetPool<UnActive>();
            PoolEx.OnAdd += OnAddExclude;
            PoolEx.OnRemove += OnRemoveExclude;
            var PoolEx2 = world.GetPool<NoBurst>();
            PoolEx2.OnAdd += OnAddExclude;
            PoolEx2.OnRemove += OnRemoveExclude;
            world.OnCreateEntityType(this);
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            transformAccessArray.Dispose();
            disposed = true;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal new void OnAddInclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;

            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            transformAccessArray.Add(classComponents.items[id].Value);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnRemoveExclude(int id)
        {
            if (HasEntity(id)) return;
            ref var data = ref world.GetEntityData(id);

            for (var i = 0; i < ExludeCount; i++)
                if (data.componentTypes.Contains(ExcludeTypes[i]))
                    return;
            for (var i = 0; i < IncludCount; i++)
                if (!data.componentTypes.Contains(IncludTypes[i]))
                    return;

            if (entities.Length == Count) Array.Resize(ref entities, entities.Length << 1);
            entities[Count] = id;
            entitiesMap.Add(id, Count);
            transformAccessArray.Add(classComponents.items[id].Value);
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnRemoveInclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private new void OnAddExclude(int id)
        {
            if (!HasEntity(id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[id];
            entitiesMap.Remove(id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Remove(Entity entity)
        {
            if (!entitiesMap.ContainsKey(entity.id)) return;
            var lastEntityId = entities[Count - 1];
            var indexOfEntityId = entitiesMap[entity.id];
            entitiesMap.Remove(entity.id);
            transformAccessArray.RemoveAtSwapBack(indexOfEntityId);
            Count--;
            if (Count > indexOfEntityId)
            {
                entities[indexOfEntityId] = lastEntityId;
                entitiesMap[lastEntityId] = indexOfEntityId;
            }
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Synchronize()
        {
            if (disposed) return;
            var data = NativeMagic.WrapToNative(structComponents.items);
            job = default;
            job.transformComponents = data;
            job.entities = NativeMagic.WrapToNative(entities);
            job.Schedule(transformAccessArray).Complete();
#if UNITY_EDITOR
            job.Clear();
#endif
        }

        internal override void Clear()
        {
#if UNITY_EDITOR
            Debug.Log("TRANSFORMS DISPOSED");
            transformAccessArray.Dispose();
            disposed = true;
#endif
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct TransformSynchronizeJob : IJobParallelForTransform
        {
            public NativeWrappedData<TransformComponent> transformComponents;
            public NativeWrappedData<int> entities;

            public void Execute(int index, TransformAccess transform)
            {
                var entity = entities.Array[index];

                var transformComponent = transformComponents.Array[entity];
                transformComponent.right = transformComponent.rotation * Vector3.right;
                transformComponent.forward = transformComponent.rotation * Vector3.forward;
                transform.position = transformComponent.position;
                transform.rotation = transformComponent.rotation;
                transform.localScale = transformComponent.scale;


                transformComponent.position = transform.position;
                transformComponents.Array[entity] = transformComponent;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear()
            {
#if UNITY_EDITOR
                NativeMagic.UnwrapFromNative(transformComponents);
                NativeMagic.UnwrapFromNative(entities);
#endif
            }
        }

        private struct MyStruct
        {
        }
    }
}