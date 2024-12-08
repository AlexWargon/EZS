using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LD54;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;


namespace Wargon.ezs {
    public unsafe struct EntityCommandBuffer : IDisposable {
        [NativeDisableUnsafePtrRestriction]
        private readonly ECBInternal* ecb;
        public int Capacity => ecb->Capacity;
        public int Count => ecb->internalBuffer->Length;
        public bool IsCreated => ecb != null && ecb->isCreated == 1;
        [NativeSetThreadIndex] 
        internal int ThreadIndex;
        public EntityCommandBuffer(int startSize) {
            ecb = (ECBInternal*)UnsafeUtility.Malloc(sizeof(ECBInternal), UnsafeUtility.AlignOf<ECBInternal>(), Allocator.Persistent);
            *ecb = new ECBInternal();
            ThreadIndex = 0;
            //ecb->internalBuffer = UnsafeList<ECBCommand>.Create(startSize, Allocator.Persistent);
            ecb->perThreadBuffer = Chains(startSize);
            ecb->isCreated = 1;
        }

        private UnsafePtrList<UnsafeList<ECBCommand>>* Chains(int startSize)
        {
            var threads = JobsUtility.JobWorkerCount+1;
            UnsafePtrList<UnsafeList<ECBCommand>>* ptrList = UnsafePtrList<UnsafeList<ECBCommand>>.Create(threads, Allocator.Persistent);
            for (int i = 0; i < threads; i++)
            {
                var list = UnsafeList<ECBCommand>.Create(startSize, Allocator.Persistent);
                ptrList->Add(list);
            }

            return ptrList;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct ECBCommand {
            public void* Component;
            public int Entity;
            public Type EcbCommandType;
            public int ComponentType;
            public int ComponentSize;
            public float3 Position;
            public byte active;

            public enum Type : byte {
                AddComponent = 0,
                AddComponentNoData = 1,
                RemoveComponent = 2,
                SetComponent = 3,
                CreateEntity = 4,
                DestroyEntity = 5,
                ChangeTransformRefPosition = 6,
                SetActiveGameObject = 7,
                PlayParticleReference = 8,
                ReuseView = 9,
                SetActiveEntity = 10,
            }
        }
        public sealed partial class ECBCommandType {
            public const byte AddComponent = 0;
            public const byte RemoveComponent = 1;
            public const byte SetComponent = 2;
            public const byte CreateEntity = 3;
            public const byte DestroyEntity = 4;
            public const byte ChangeGOPosition = 5;
            public const byte SetActive = 6;
            public const byte PlayParticle = 7;
        }
        private struct ECBInternal {
            internal byte isCreated;
            //[NativeDisableUnsafePtrRestriction]
            //internal UnsafeList<ECBCommand>* internalBuffer;
            [NativeDisableUnsafePtrRestriction]
            internal UnsafePtrList<UnsafeList<ECBCommand>>* perThreadBuffer;



            internal UnsafeList<ECBCommand>* internalBuffer
            {
                get => perThreadBuffer->ElementAt(1);
            }
            public int Capacity => internalBuffer->Capacity;
            public bool IsCreated => isCreated == 1;
            public void ResizeAndClear(int newSize) {
                internalBuffer->Resize(newSize);
                internalBuffer->Clear();
            }

            public void Clear() {
                internalBuffer->Clear();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set<T>(int entity, int thread) where T : unmanaged {
                var cmd = new ECBCommand {
                    Entity = entity,
                    EcbCommandType = ECBCommand.Type.SetComponent,
                    ComponentType = CTS<T>.ID.Data
                };
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add<T>(int entity, T component, int thread) where T : unmanaged {
                //if(IsCreated== false) return;
                var size = UnsafeUtility.SizeOf<T>();
                var ptr = (T*)UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), Allocator.Temp);
                *ptr = component;
                var cmd = new ECBCommand {
                    Component = ptr,
                    Entity = entity,
                    EcbCommandType = ECBCommand.Type.AddComponent,
                    ComponentType = CTS<T>.ID.Data,
                    ComponentSize = size
                };
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add<T>(int entity, int thread) where T : unmanaged {
                var cmd = new ECBCommand {
                    Entity = entity,
                    EcbCommandType = ECBCommand.Type.AddComponentNoData,
                    ComponentType = CTS<T>.ID.Data,
                };
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Destroy(int entity, int thread) {
                // var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.DestroyEntity};
                // buffer.AddNoResize(cmd);
                Add<DestroyEntity>(entity, thread);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove<T>(int entity, int thread) where T: struct {
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.RemoveComponent, ComponentType = CTS<T>.ID.Data };
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetViewPosition(int entity, float3 pos, int thread) {
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.ChangeTransformRefPosition, Position = pos};
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnableGameObject(int entity, bool value, int thread) {
                byte v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.SetActiveGameObject, active = v};
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnableEntity(int entity, bool value, int thread) {
                byte v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.SetActiveGameObject, active = v};
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CreateEntity() {
                return 1;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void PlayParticleReference(int entity, bool value, int thread) {
                var v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.PlayParticleReference, active = v};
                var buffer = perThreadBuffer->ElementAt(thread);
                buffer->Add(cmd);
            }

            #if !UNITY_EDITOR
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            #endif
            public void Playback(World world) {
                //if(internalBuffer->IsEmpty) return;
                for (var i = 0; i < perThreadBuffer->Length; i++)
                {
                    var buffer = perThreadBuffer->ElementAt(i);
                    if(buffer->IsEmpty) continue;
                    
                    for (var cmdIndex = 0; cmdIndex < buffer->Length; cmdIndex++)
                    {
                        ref var cmd = ref buffer->ElementAt(cmdIndex);
                        switch (cmd.EcbCommandType) 
                        {
                            case ECBCommand.Type.AddComponent:
                                ref var entityToAdd = ref world.GetEntity(cmd.Entity);
                                entityToAdd.AddPtr(cmd.Component, cmd.ComponentType);
                                UnsafeUtility.Free(cmd.Component, Allocator.Temp);
                                break;
                            case ECBCommand.Type.AddComponentNoData:
                                world.GetEntity(cmd.Entity).AddByTypeID(cmd.ComponentType);
                                break;
                            case ECBCommand.Type.RemoveComponent:
                                world.GetEntity(cmd.Entity).RemoveByTypeID(cmd.ComponentType);
                                break;
                            case ECBCommand.Type.SetComponent:
                                
                                break;
                            case ECBCommand.Type.CreateEntity:
                                world.CreateEntity();
                                break;
                            case ECBCommand.Type.DestroyEntity:
                                world.GetEntity(cmd.Entity).Destroy();
                                break;
                            case ECBCommand.Type.ChangeTransformRefPosition:
                                world.GetEntity(cmd.Entity).Get<TransformRef>().value.position = new Vector3(cmd.Position.x, cmd.Position.y, cmd.Position.z);
                                break;
                            case ECBCommand.Type.SetActiveGameObject:
                                world.GetEntity(cmd.Entity).Get<Pooled>().SetActive(cmd.active == 1);
                                break;
                            case ECBCommand.Type.SetActiveEntity:
                                ref var e = ref world.GetEntity(cmd.Entity);
                                EntityPool.Back(e, e.Get<PooledEntity>());
                                break;
                            case ECBCommand.Type.PlayParticleReference:
                                if (cmd.active == 1) {
                                    world.GetEntity(cmd.Entity).Get<Particle>().value.Play();
                                }
                                else {
                                    world.GetEntity(cmd.Entity).Get<Particle>().value.Stop();
                                }
                                break;
                            case ECBCommand.Type.ReuseView:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    buffer->Clear();
                }
                
                
                // for (int i = 0; i < internalBuffer->Length; i++) {
                //     ref var cmd = ref internalBuffer->ElementAt(i);
                //     //Debug.Log(cmd.EcbCommandType);
                //     switch (cmd.EcbCommandType) {
                //         case ECBCommand.Type.AddComponent:
                //             ref var entityToAdd = ref world.GetEntity(cmd.Entity);
                //             entityToAdd.AddPtr(cmd.Component, cmd.ComponentType);
                //             UnsafeUtility.Free(cmd.Component, Allocator.Temp);
                //             break;
                //         case ECBCommand.Type.AddComponentNoData:
                //             world.GetEntity(cmd.Entity).AddByTypeID(cmd.ComponentType);
                //             break;
                //         case ECBCommand.Type.RemoveComponent:
                //             world.GetEntity(cmd.Entity).RemoveByTypeID(cmd.ComponentType);
                //             break;
                //         case ECBCommand.Type.SetComponent:
                //             
                //             break;
                //         case ECBCommand.Type.CreateEntity:
                //             world.CreateEntity();
                //             break;
                //         case ECBCommand.Type.DestroyEntity:
                //             world.GetEntity(cmd.Entity).Destroy();
                //             break;
                //         case ECBCommand.Type.ChangeTransformRefPosition:
                //             world.GetEntity(cmd.Entity).Get<TransformRef>().value.position = new Vector3(cmd.Position.x, cmd.Position.y, cmd.Position.z);
                //             break;
                //         case ECBCommand.Type.SetActiveGameObject:
                //             world.GetEntity(cmd.Entity).Get<Pooled>().SetActive(cmd.active == 1);
                //             break;
                //         case ECBCommand.Type.SetActiveEntity:
                //             ref var e = ref world.GetEntity(cmd.Entity);
                //             EntityPool.Back(e, e.Get<PooledEntity>());
                //             break;
                //         case ECBCommand.Type.PlayParticleReference:
                //             if (cmd.active == 1) {
                //                 world.GetEntity(cmd.Entity).Get<Particle>().value.Play();
                //             }
                //             else {
                //                 world.GetEntity(cmd.Entity).Get<Particle>().value.Stop();
                //             }
                //             break;
                //         case ECBCommand.Type.ReuseView:
                //
                //             break;
                //         default:
                //             throw new ArgumentOutOfRangeException();
                //     }
                // }

                //internalBuffer->Clear();
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() {
                for (var i = 0; i < perThreadBuffer->Length; i++)
                {
                    UnsafeList<ECBCommand>.Destroy(perThreadBuffer->ElementAt(i));
                }
                UnsafePtrList<UnsafeList<ECBCommand>>.Destroy(perThreadBuffer);
                //UnsafeList<ECBCommand>.Destroy(internalBuffer);
                isCreated = 0;
            }
        }
        [BurstCompile]
        public struct ParallelWriter {
            [WriteOnly]
            private UnsafeList<ECBCommand>.ParallelWriter buffer;
            public UnsafeList<ECBCommand>.ParallelWriter GetBuffer() => buffer;
            public ParallelWriter(UnsafeList<ECBCommand>.ParallelWriter parallelWriter) {
                buffer = parallelWriter;
            }
        
            private void Add(in ECBCommand cmd) {
                if (buffer.ListData->Length == buffer.ListData->Capacity - 1) {
                    buffer.ListData->Resize(buffer.ListData->Capacity * 2);
                }
        
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add<T>(int entity, T component) where T : unmanaged {
                var size = UnsafeUtility.SizeOf<T>();
                var ptr = (T*)UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), Allocator.Temp);
                *ptr = component;
                var cmd = new ECBCommand {
                    Component = ptr,
                    Entity = entity,
                    EcbCommandType = ECBCommand.Type.AddComponent,
                    ComponentType = CTS<T>.ID.Data,
                    ComponentSize = size
                };
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add<T>(int entity) where T : unmanaged {
                var cmd = new ECBCommand {
                    Entity = entity,
                    EcbCommandType = ECBCommand.Type.AddComponentNoData,
                    ComponentType = CTS<T>.ID.Data,
                };
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove<T>(int entity) where T: struct {
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.RemoveComponent, ComponentType = CTS<T>.ID.Data };
                buffer.AddNoResize(cmd);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Destroy(int entity) {
                // var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.DestroyEntity};
                // buffer.AddNoResize(cmd);
                Add<DestroyEntity>(entity);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetViewPosition(int entity, float3 pos) {
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.ChangeTransformRefPosition, Position = pos};
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnableGameObject(int entity, bool value) {
                byte v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.SetActiveGameObject, active = v};
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EnableEntity(int entity, bool value) {
                byte v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.SetActiveEntity, active = v};
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void PlayParticleReference(int entity, bool value) {
                byte v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.PlayParticleReference, active = v};
                buffer.AddNoResize(cmd);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void PlayParticle(int entity, bool value) {
                byte v = value ? (byte)1 : (byte)0;
                var cmd = new ECBCommand { Entity = entity, EcbCommandType = ECBCommand.Type.PlayParticleReference, active = v};
                buffer.AddNoResize(cmd);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParallelWriter AsParallelWriter() {
            return new ParallelWriter(ecb->internalBuffer->AsParallelWriter());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeAndClear(int newSize) {
            ecb->ResizeAndClear(newSize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            ecb->Clear();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<T>(int entity) where T : unmanaged {
            ecb->Set<T>(entity, ThreadIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>(int entity, T component) where T : unmanaged {
            ecb->Add(entity, component, ThreadIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>(int entity) where T : unmanaged {
            ecb->Add<T>(entity, ThreadIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>(int entity) where T: struct {
            ecb->Remove<T>(entity, ThreadIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnableGameObject(int entity, bool value) {
            ecb->EnableGameObject(entity, value, ThreadIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(int entity) {
            ecb->Destroy(entity, ThreadIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayParticleReference(int entity, bool value)
        {
            ecb->PlayParticleReference(entity, value, ThreadIndex);
        }
        
        public void PerformCommand(World world) {
            ecb->Playback(world);
        }
        public void Dispose() {
            ecb->Dispose();
            UnsafeUtility.Free(ecb, Allocator.Persistent);
        }
    }
    [SystemColor(DColor.cyan)]
    public partial class JobsCompliteSystem : UpdateSystem {
        public override void Update() {
            DependenciesAll.Complete();
        }
    }
    [SystemColor(DColor.blue)]
    public partial class EndFrameEntityBufferSystem : UpdateSystem {
        private UnsafeList<EntityCommandBuffer> buffers;
        private int lastBufferIndex;
        private int doubleEntities => world.totalEntitiesCount * 2;
        public ref EntityCommandBuffer CreateBuffer() {
            return ref CreateBuffer(1024);
        }
        public ref EntityCommandBuffer CreateBufferBIG() {
            if (buffers.IsCreated == false) {
                buffers = new UnsafeList<EntityCommandBuffer>(12, Allocator.Persistent);
            }
            if (lastBufferIndex >= buffers.Length) {
                buffers.Add(new EntityCommandBuffer(world.totalEntitiesCount > 1024 ? world.totalEntitiesCount : 1024));
            }
            ref var ecb = ref buffers.ElementAt(lastBufferIndex);
            if (ecb.Capacity < doubleEntities * 4) {
                ecb.ResizeAndClear(doubleEntities * 10);
            }
            lastBufferIndex++;
            return ref ecb;
        }
        public ref EntityCommandBuffer CreateBuffer(int size) {
            if (buffers.IsCreated == false) {
                buffers = new UnsafeList<EntityCommandBuffer>(12, Allocator.Persistent);
            }
            if (lastBufferIndex >= buffers.Length) {
                buffers.Add(new EntityCommandBuffer(size));
            }
            ref var ecb = ref buffers.ElementAt(lastBufferIndex);
            // if (ecb.Capacity < size) {
            //     ecb.ResizeAndClear(size);
            // }
            lastBufferIndex++;
            return ref ecb;
        }
        public override void Update() {
            //if(buffers.IsEmpty || lastBufferIndex == 0) return;
            
            for (var i = 0; i < lastBufferIndex; i++) {
                ref var ecb = ref buffers.ElementAt(i);
                ecb.PerformCommand(world);
            }
            lastBufferIndex = 0;
        }

        public override void OnDestroy() {
            for (var i = 0; i < buffers.Length; i++) {
                buffers.ElementAt(i).Dispose();
            }
            buffers.Dispose();
        }
    }
}