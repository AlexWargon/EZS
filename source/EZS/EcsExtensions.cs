using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Wargon.ezs;
using Wargon.ezs.Unity;

public delegate void AddEntityToPoolDelegate<A>(int id) where A : unmanaged;

public delegate void RemoveEntityFormPoolDelegate<A>(int id) where A : unmanaged;
public static partial class EcsExtensions
{
    public static MonoEntity GetMonoEntity(this GameObject gameObject)
    {
        return gameObject.GetComponent<MonoEntity>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A>(this Entities ezs, ref AExecutor jobExecute)
        where A : unmanaged where AExecutor : unmanaged, IJobExecute<A>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePair<A>();
        var entities = entityType.entities;
        EachWithJob<A, AExecutor> job = default;
        job.Set(entities, ref entityType.poolA.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A, B>(this Entities ezs, ref AExecutor jobExecute) where A : unmanaged
        where B : unmanaged
        where AExecutor : unmanaged, IJobExecute<A, B>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePair<A, B>();
        var entities = entityType.entities;
        EachWithJob<A, B, AExecutor> job = default;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
// #if UNITY_EDITOR
//         job.Clear();
// #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A, B, NA>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor jobExecute)
        where A : unmanaged
        where B : unmanaged
        where AExecutor : unmanaged, IJobExecute<A, B>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        var entities = entityType.entities;
        EachWithJob<A, B, AExecutor> job = default;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
#if UNITY_EDITOR
        job.Clear();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A, B, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs,
        ref AExecutor jobExecute) where A : unmanaged
        where B : unmanaged
        where AExecutor : unmanaged, IJobExecute<A, B>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        var entities = entityType.entities;
        EachWithJob<A, B, AExecutor> job = default;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
#if UNITY_EDITOR
        job.Clear();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A, B, C>(this Entities ezs, ref AExecutor jobExecute) where A : unmanaged
        where B : unmanaged
        where C : unmanaged
        where AExecutor : unmanaged, IJobExecute<A, B, C>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePair<A, B, C>();
        var entities = entityType.entities;
        EachWithJob<A, B, C, AExecutor> job = default;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, entityType.poolС.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A, B, C, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs,
        ref AExecutor jobExecute) where A : unmanaged
        where B : unmanaged
        where C : unmanaged
        where AExecutor : unmanaged, IJobExecute<A, B, C>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C>();
        var entities = entityType.entities;
        EachWithJob<A, B, C, AExecutor> job = default;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, entityType.poolС.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
#if UNITY_EDITOR
        job.Clear();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EachWithJob<AExecutor, A, B, C, D>(this Entities ezs, ref AExecutor jobExecute)
        where A : unmanaged
        where B : unmanaged
        where C : unmanaged
        where D : unmanaged
        where AExecutor : unmanaged, IJobExecute<A, B, C, D>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePair<A, B, C, D>();
        var entities = entityType.entities;
        EachWithJob<A, B, C, D, AExecutor> job = default;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, entityType.poolС.items,
            entityType.poolD.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
    }

    public static void Each<A>(this Entities ezs, LambdaRef<A> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePair<A>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]]);
    }

    public static void Each<A, B, C, NA>(this Entities.EntitiesWithout<NA> ezs, LambdaRRC<A, B, C> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                c[entities[index]]);
    }

    public static void Each<A, B, NA>(this Entities.EntitiesWithout<NA> ezs, LambdaRef<A, B> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]]);
    }

    public static void Each<A, B, C, NA>(this Entities.EntitiesWithout<NA> ezs, LambdaRef<A, B, C> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                ref c[entities[index]]);
    }

    public static void Each<A, B, C, D, NA>(this Entities.EntitiesWithout<NA> ezs, LambdaRef<A, B, C, D> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C, D>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        var d = entityType.poolD.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                ref c[entities[index]],
                ref d[entities[index]]);
    }

    public static void Each<A, B, C, D, E, NA>(this Entities.EntitiesWithout<NA> ezs, LambdaRef<A, B, C, D, E> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        var d = entityType.poolD.items;
        var e = entityType.poolE.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                ref c[entities[index]],
                ref d[entities[index]],
                ref e[entities[index]]);
    }

    public static void Each<A, B, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs, LambdaRef<A, B> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]]);
    }

    public static void Each<A, B, C, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs, LambdaRef<A, B, C> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                ref c[entities[index]]);
    }

    public static void Each<A, B, C, D, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs, LambdaRef<A, B, C, D> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C, D>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        var d = entityType.poolD.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                ref c[entities[index]],
                ref d[entities[index]]);
    }

    public static void Each<A, B, C, D, E, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs,
        LambdaRef<A, B, C, D, E> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C, D, E>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        var d = entityType.poolD.items;
        var e = entityType.poolE.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(ref a[entities[index]],
                ref b[entities[index]],
                ref c[entities[index]],
                ref d[entities[index]],
                ref e[entities[index]]);
    }

    public static void EachWithJobs<A, NA>(this Entities.EntitiesWithout<NA> ezs, Lambda<A> lambda)
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A>();
        var entities = entityType.entities;
        var a = entityType.poolA.items;
        for (var index = 0; index < entityType.Count; index++)
            lambda(a[entities[index]]);
    }
}

[EcsComponent]
public struct TransformComponent
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public Vector3 right;
    public Vector3 forward;
}

public interface IJobExecute<T>
    where T : unmanaged
{
    void ForEach(ref T t);
}

public interface IJobExecute<T, T2>
    where T : unmanaged where T2 : unmanaged
{
    void ForEach(ref T t, ref T2 t2);
}

public interface IJobExecute<T, T2, T3>
    where T : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
{
    void ForEach(ref T t, ref T2 t2, ref T3 t3);
}

public interface IJobExecute<T, T2, T3, T4>
    where T : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
{
    void ForEach(ref T t, ref T2 t2, ref T3 t3, ref T4 t4);
}

public interface IJobExecute<T, T2, T3, T4, T5>
    where T : unmanaged
    where T2 : unmanaged
    where T3 : unmanaged
    where T4 : unmanaged
    where T5 : unmanaged
{
    void ForEach(ref T t, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5);
}

[BurstCompile(CompileSynchronously = true)]
public struct EachWithJob<A, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A>
    where A : unmanaged
{
    private Executor executionFunc;
    private NativeWrappedData<int> Entites;
    private NativeWrappedData<A> ItemsA;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int[] entites, ref A[] items, ref Executor action)
    {
        Entites = NativeMagic.WrapToNative(entites);
        ItemsA = NativeMagic.WrapToNative(items);
        executionFunc = action;
    }


    public void Execute(int index)
    {
        var entity = Entites.Array[index];
        var item = ItemsA.Array[entity];
        executionFunc.ForEach(ref item);
        ItemsA.Array[entity] = item;
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct EachWithJob<A, B, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A, B>
    where A : unmanaged
    where B : unmanaged
{
    private Executor executionFunc;
    private NativeWrappedData<int> Entites;
    private NativeWrappedData<A> ItemsA;
    private NativeWrappedData<B> ItemsB;

    public void Set(int[] entites, A[] itemsA, B[] itemsB, ref Executor action)
    {
        Entites = NativeMagic.WrapToNative(entites);
        ItemsA = NativeMagic.WrapToNative(itemsA);
        ItemsB = NativeMagic.WrapToNative(itemsB);
        executionFunc = action;
    }

    public void Execute(int index)
    {
        var entity = Entites.Array[index];
        var itemA = ItemsA.Array[entity];
        var itemB = ItemsB.Array[entity];
        executionFunc.ForEach(ref itemA, ref itemB);
        ItemsA.Array[entity] = itemA;
        ItemsB.Array[entity] = itemB;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
#if UNITY_EDITOR
        NativeMagic.UnwrapFromNative(Entites);
        NativeMagic.UnwrapFromNative(ItemsA);
        NativeMagic.UnwrapFromNative(ItemsB);
#endif
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct EachWithJob<A, B, C, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A, B, C>
    where A : unmanaged
    where B : unmanaged
    where C : unmanaged
{
    private Executor executionFunc;
    private NativeWrappedData<int> Entites;
    private NativeWrappedData<A> ItemsA;
    private NativeWrappedData<B> ItemsB;
    private NativeWrappedData<C> ItemsC;

    public void Set(int[] entites, A[] itemsA, B[] itemsB, C[] itemsC, ref Executor action)
    {
        Entites = NativeMagic.WrapToNative(entites);
        ItemsA = NativeMagic.WrapToNative(itemsA);
        ItemsB = NativeMagic.WrapToNative(itemsB);
        ItemsC = NativeMagic.WrapToNative(itemsC);
        executionFunc = action;
    }

    public void Execute(int index)
    {
        var entity = Entites.Array[index];
        var itemA = ItemsA.Array[entity];
        var itemB = ItemsB.Array[entity];
        var itemC = ItemsC.Array[entity];
        executionFunc.ForEach(ref itemA, ref itemB, ref itemC);
        ItemsA.Array[entity] = itemA;
        ItemsB.Array[entity] = itemB;
        ItemsC.Array[entity] = itemC;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
#if UNITY_EDITOR
        NativeMagic.UnwrapFromNative(Entites);
        NativeMagic.UnwrapFromNative(ItemsA);
        NativeMagic.UnwrapFromNative(ItemsB);
        NativeMagic.UnwrapFromNative(ItemsC);
#endif
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct EachWithJob<A, B, C, D, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A, B, C, D>
    where A : unmanaged
    where B : unmanaged
    where C : unmanaged
    where D : unmanaged
{
    private Executor executionFunc;
    private NativeWrappedData<int> Entites;
    private NativeWrappedData<A> ItemsA;
    private NativeWrappedData<B> ItemsB;
    private NativeWrappedData<C> ItemsC;
    private NativeWrappedData<D> ItemsD;

    public void Set(int[] entites, A[] itemsA, B[] itemsB, C[] itemsC, D[] itemsD, ref Executor action)
    {
        Entites = NativeMagic.WrapToNative(entites);
        ItemsA = NativeMagic.WrapToNative(itemsA);
        ItemsB = NativeMagic.WrapToNative(itemsB);
        ItemsC = NativeMagic.WrapToNative(itemsC);
        ItemsD = NativeMagic.WrapToNative(itemsD);
        executionFunc = action;
    }

    public void Execute(int index)
    {
        var entity = Entites.Array[index];
        var itemA = ItemsA.Array[entity];
        var itemB = ItemsB.Array[entity];
        var itemC = ItemsC.Array[entity];
        var itemD = ItemsD.Array[entity];

        executionFunc.ForEach(ref itemA, ref itemB, ref itemC, ref itemD);

        ItemsA.Array[entity] = itemA;
        ItemsB.Array[entity] = itemB;
        ItemsC.Array[entity] = itemC;
        ItemsD.Array[entity] = itemD;
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct EachWithJob<A, B, C, D, E, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A, B, C, D, E>
    where A : unmanaged
    where B : unmanaged
    where C : unmanaged
    where D : unmanaged
    where E : unmanaged
{
    private Executor executionFunc;
    private NativeWrappedData<int> Entites;
    private NativeWrappedData<A> ItemsA;
    private NativeWrappedData<B> ItemsB;
    private NativeWrappedData<C> ItemsC;
    private NativeWrappedData<D> ItemsD;
    private NativeWrappedData<E> ItemsE;

    public void Set(int[] entites, A[] itemsA, B[] itemsB, C[] itemsC, D[] itemsD, E[] itemsE, ref Executor action)
    {
        Entites = NativeMagic.WrapToNative(entites);
        ItemsA = NativeMagic.WrapToNative(itemsA);
        ItemsB = NativeMagic.WrapToNative(itemsB);
        ItemsC = NativeMagic.WrapToNative(itemsC);
        ItemsD = NativeMagic.WrapToNative(itemsD);
        ItemsE = NativeMagic.WrapToNative(itemsE);
        executionFunc = action;
    }

    public void Execute(int index)
    {
        var entity = Entites.Array[index];
        var itemA = ItemsA.Array[entity];
        var itemB = ItemsB.Array[entity];
        var itemC = ItemsC.Array[entity];
        var itemD = ItemsD.Array[entity];
        var itemE = ItemsE.Array[entity];

        executionFunc.ForEach(ref itemA, ref itemB, ref itemC, ref itemD, ref itemE);

        ItemsA.Array[entity] = itemA;
        ItemsB.Array[entity] = itemB;
        ItemsC.Array[entity] = itemC;
        ItemsD.Array[entity] = itemD;
        ItemsE.Array[entity] = itemE;
    }
}

public unsafe interface IJobForWithEntity<A, B> where A : unmanaged where B : unmanaged
{
    int MaxIndex { get; set; }
    void Execute(A* componentsPool1, B* componentsPool2, int* entities, ref EntityUnsafe entity, int currentIndex);
}
[BurstCompile(CompileSynchronously = true)]
public unsafe struct ForWithEntityJob<A, B, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobForWithEntity<A, B>
    where A : unmanaged
    where B : unmanaged
{
    public int ChangedEntitiesCount;

    [NativeDisableParallelForRestriction] public NativeArray<EntityUnsafe> entitiesUnsafe;
    
    [NativeDisableUnsafePtrRestriction] private int* entities;
    [NativeDisableUnsafePtrRestriction] private A* ItemsA;
    [NativeDisableUnsafePtrRestriction] private B* ItemsB;
    
    private Executor executionFunc;
    [BurstCompile(CompileSynchronously = true)]
    public void Set(int[] entites, A[] itemsA, B[] itemsB, ref Executor action, int count, int size)
    {
        entities = UnsafeHelp.ArrayToPtr(entites);
        ItemsA =   UnsafeHelp.ArrayToPtr(itemsA);
        ItemsB =   UnsafeHelp.ArrayToPtr(itemsB);
        entitiesUnsafe = new NativeArray<EntityUnsafe>(size, Allocator.TempJob);
        executionFunc = action;
        executionFunc.MaxIndex = count;
        ChangedEntitiesCount = 0;
    }

    public void Execute(int index)
    {
        var id = entities[index];
        var entity = entitiesUnsafe[id];
        entity.id = id;
        executionFunc.Execute(ItemsA, ItemsB, entities, ref entity, index);
        
        entitiesUnsafe[id] = entity;

    }

    public void Clear()
    {
        entitiesUnsafe.Dispose();
    }
}
public unsafe interface IJobForUnsafeExecute<A, B> where A : unmanaged where B : unmanaged
{
    int MaxIndex { get; set; }
    void Execute(A* componentsPool1, B* componentsPool2, int* entities, int currentIndex);
}

[BurstCompile(CompileSynchronously = true)]
public unsafe struct ForWithJob<A, B, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobForUnsafeExecute<A, B>
    where A : unmanaged
    where B : unmanaged
{
    private Executor executionFunc;
    [NativeDisableUnsafePtrRestriction]
    private int* Entites;

    [NativeDisableUnsafePtrRestriction]
    private A* ItemsA;
    [NativeDisableUnsafePtrRestriction]
    private B* ItemsB;

    public void Set(int[] entites, A[] itemsA, B[] itemsB, ref Executor action)
    {
        fixed (void* a = itemsA, b = itemsB, e = entites)
        {
            ItemsA = (A*) a;
            ItemsB = (B*) b;
            Entites = (int*)e;
        }
        
        executionFunc = action;
    }

    public void Execute(int index)
    {
        executionFunc.Execute(ItemsA, ItemsB, Entites, index);
    }
}

public static partial class EcsExtensions
{
    public static void ForWithJob<AExecutor, A, B>(this Entities ezs, ref AExecutor jobExecute)
        where AExecutor : unmanaged, 
        IJobForUnsafeExecute<A, B>
        where A : unmanaged
        where B : unmanaged
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePair<A,B>();
        var entities = entityType.entities;

        ForWithJob<A, B, AExecutor> job = default;
        jobExecute.MaxIndex = entityType.Count;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();

    }
    public static void ForWithJob<AExecutor, A, B, NA>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor jobExecute)
        where AExecutor : unmanaged, 
        IJobForUnsafeExecute<A, B>
        where A : unmanaged
        where B : unmanaged
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A,B>();
        var entities = entityType.entities;

        ForWithJob<A, B, AExecutor> job = default;
        jobExecute.MaxIndex = entityType.Count;
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute);
        job.Schedule(entityType.Count, 0).Complete();
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public static void ForWithJobWithEntity<AExecutor, A, B, NA>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor jobExecute)
        where AExecutor : unmanaged,
        IJobForWithEntity<A, B>
        where A : unmanaged
        where B : unmanaged
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A,B>();
        var entities = entityType.entities;

        
        ForWithEntityJob<A, B, AExecutor> job = default;
        jobExecute.MaxIndex = entityType.Count;
        
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute, entityType.Count, entities.Length);
        
        job.Schedule(entityType.Count, 0).Complete();

        for (int i = job.entitiesUnsafe.Length - 1; i > -1; i--)
        {
            var entity = job.entitiesUnsafe[i];
            var realEntity = ezs.GetWorld().GetEntity(entity.id);
            if (entity.addedComponent != 0)
            {
                if(!realEntity.Has(entity.addedComponent))
                    realEntity.AddBoxed(entity.GetAdded());
            }
        }
        job.Clear();
    }
    public static void EachWithJobRaycast<AExecutor, NA>(this Entities.EntitiesWithout<NA> ezs,
        ref AExecutor jobExecute, Vector3 direction)
        where AExecutor : unmanaged,
        IJobExecute<TransformComponent, RaycastHit>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<TransformComponent>();
        var transforms = entityType.poolA.items;
        var entities = entityType.entities;

        var results = new NativeArray<RaycastHit>(transforms.Length, Allocator.Persistent);
        var commands = new NativeArray<RaycastCommand>(transforms.Length, Allocator.Persistent);
        for (var i = 0; i < entityType.Count; i++)
        {
            var entity = entities[i];
            commands[entity] = new RaycastCommand(transforms[entity].position, direction);
        }
        
        RaycastCommand.ScheduleBatch(commands, results, 10).Complete();

        EachJobWithRaycast<TransformComponent, AExecutor> job = default;
        job.RaycastHits = results;
        job.ItemsA = NativeMagic.WrapToNative(transforms);
        job.Entities = NativeMagic.WrapToNative(entities);
        job.Action = jobExecute;
        job.Schedule(entityType.Count, 0).Complete();

        results.Dispose();
        commands.Dispose();
#if UNITY_EDITOR
        job.Clear();
#endif
    }

    
    [BurstCompile]
    public static void SetupCommands(ref NativeArray<RaycastCommand> commands, in NativeWrappedData<int> entities,
        in NativeWrappedData<TransformComponent> transforms, int count, in Vector3 offset, in Vector3 direction,
        in float distance, int mask)
    {
        for (var i = 0; i < count; i++)
        {
            var entity = entities.Array[i];
            commands[entity] =
                new RaycastCommand(transforms.Array[entity].position + offset, direction, distance, mask);
        }
    }

    public static void EachWithJobRaycast<AExecutor, NA, NB>(this Entities.EntitiesWithout<NA, NB> ezs,
        ref AExecutor jobExecute, in Vector3 direction, in Vector3 offset, LayerMask mask, float distance)
        where AExecutor : unmanaged, IJobExecute<TransformComponent, RaycastHit>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<TransformComponent>();
        var transforms = entityType.poolA.items;
        var entities = entityType.entities;

        var results = new NativeArray<RaycastHit>(entityType.Count, Allocator.Persistent);
        var commands = new NativeArray<RaycastCommand>(entityType.Count, Allocator.Persistent);

        var maskValue = mask.value;
        
        for (var i = 0; i < entityType.Count; i++)
        {
            var entity = entities[i];
            commands[i] = new RaycastCommand(transforms[entity].position + offset, direction, distance, maskValue);
        }

        RaycastCommand.ScheduleBatch(commands, results, 10).Complete();

        EachJobWithRaycast<TransformComponent, AExecutor> job;

        job.RaycastHits = results;
        job.ItemsA = NativeMagic.WrapToNative(transforms);
        job.Entities = NativeMagic.WrapToNative(entities);
        job.Action = jobExecute;
        job.Schedule(entityType.Count, 0).Complete();

        results.Dispose();
        commands.Dispose();
#if UNITY_EDITOR
        job.Clear();
#endif
    }
    public static void EachWithJobRaycast<AExecutor, NA, NB, B>(this Entities.EntitiesWithout<NA, NB> ezs,
        ref AExecutor jobExecute, in Vector3 direction, in Vector3 offset, LayerMask mask, float distance)
        where B : unmanaged
        where AExecutor : unmanaged, IJobExecute<TransformComponent,B, RaycastHit>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<TransformComponent,B>();
        var transforms = entityType.poolA.items;
        var b = entityType.poolB.items;

        var entities = entityType.entities;

        var results = new NativeArray<RaycastHit>(entityType.Count, Allocator.Persistent);
        var commands = new NativeArray<RaycastCommand>(entityType.Count, Allocator.Persistent);

        var maskValue = mask.value;
        
        for (var i = 0; i < entityType.Count; i++)
        {
            var entity = entities[i];
            commands[i] = new RaycastCommand(transforms[entity].position + offset, direction, distance, maskValue);
        }

        RaycastCommand.ScheduleBatch(commands, results, 10).Complete();

        EachJobWithRaycast<TransformComponent,B, AExecutor> job;

        job.RaycastHits = results;
        job.ItemsA = NativeMagic.WrapToNative(transforms);
        job.ItemsB = NativeMagic.WrapToNative(b);
        job.Entities = NativeMagic.WrapToNative(entities);
        job.Action = jobExecute;
        job.Schedule(entityType.Count, 0).Complete();

        results.Dispose();
        commands.Dispose();
#if UNITY_EDITOR
        job.Clear();
#endif
    }
    
    public static void EachWithJobRaycast<AExecutor, NA, NB, B, C>(this Entities.EntitiesWithout<NA, NB> ezs,
        ref AExecutor jobExecute, in Vector3 direction, in Vector3 offset, LayerMask mask, float distance)
    where C : unmanaged where B : unmanaged
        where AExecutor : unmanaged, IJobExecute<TransformComponent,B,C, RaycastHit>
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<TransformComponent,B,C>();
        var transforms = entityType.poolA.items;
        var b = entityType.poolB.items;
        var c = entityType.poolС.items;
        var entities = entityType.entities;

        var results = new NativeArray<RaycastHit>(entityType.Count, Allocator.Persistent);
        var commands = new NativeArray<RaycastCommand>(entityType.Count, Allocator.Persistent);

        var maskValue = mask.value;
        
        for (var i = 0; i < entityType.Count; i++)
        {
            var entity = entities[i];
            commands[i] = new RaycastCommand(transforms[entity].position + offset, direction, distance, maskValue);
        }

        RaycastCommand.ScheduleBatch(commands, results, 10).Complete();

        EachJobWithRaycast<TransformComponent,B,C, AExecutor> job;

        job.RaycastHits = results;
        job.ItemsA = NativeMagic.WrapToNative(transforms);
        job.ItemsB = NativeMagic.WrapToNative(b);
        job.ItemsC = NativeMagic.WrapToNative(c);
        job.Entities = NativeMagic.WrapToNative(entities);
        job.Action = jobExecute;
        job.Schedule(entityType.Count, 0).Complete();

        results.Dispose();
        commands.Dispose();
#if UNITY_EDITOR
        job.Clear();
#endif
    }
    
    [BurstCompile]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetColliderID(this RaycastHit hit)
    {
        unsafe
        {
            var h = *(RaycastHitPublic*) &hit;
            return h.m_ColliderID;
        }
    }
}

[EcsComponent]
[StructLayout(LayoutKind.Sequential)]
public struct RaycastHitPublic
{
    public Vector3 m_Point;
    public Vector3 m_Normal;
    public int m_FaceID;
    public float m_Distance;
    public Vector2 m_UV;
    public int m_ColliderID;
}

[BurstCompile(CompileSynchronously = true)]
public struct EachJobWithRaycast<A, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A, RaycastHit>
    where A : unmanaged
{
    [ReadOnly] public NativeArray<RaycastHit> RaycastHits;
    public NativeWrappedData<A> ItemsA;
    public NativeWrappedData<int> Entities;
    public Executor Action;

    public void Execute(int index)
    {
        var entity = Entities.Array[index];
        var itemA = ItemsA.Array[entity];
        var raycast = RaycastHits[index];
        Action.ForEach(ref itemA, ref raycast);
        ItemsA.Array[entity] = itemA;
    }
#if UNITY_EDITOR
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        NativeMagic.UnwrapFromNative(ItemsA);
        NativeMagic.UnwrapFromNative(Entities);
    }
#endif
}
[BurstCompile(CompileSynchronously = true)]
public struct EachJobWithRaycast<A,B, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A , B, RaycastHit>
    where A : unmanaged
    where B : unmanaged

{
    [ReadOnly] public NativeArray<RaycastHit> RaycastHits;
    public NativeWrappedData<A> ItemsA;
    public NativeWrappedData<B> ItemsB;
    public NativeWrappedData<int> Entities;
    public Executor Action;

    public void Execute(int index)
    {
        var entity = Entities.Array[index];
        var itemA = ItemsA.Array[entity];
        var itemB = ItemsB.Array[entity];

        var raycast = RaycastHits[index];
        Action.ForEach(ref itemA, ref itemB,ref raycast);
        ItemsA.Array[entity] = itemA;
        ItemsB.Array[entity] = itemB;

    }
#if UNITY_EDITOR
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        NativeMagic.UnwrapFromNative(ItemsA);
        NativeMagic.UnwrapFromNative(ItemsB);

        NativeMagic.UnwrapFromNative(Entities);
    }
#endif
}
[BurstCompile(CompileSynchronously = true)]
public struct EachJobWithRaycast<A,B,C, Executor> : IJobParallelFor
    where Executor : unmanaged, IJobExecute<A , B, C, RaycastHit>
    where A : unmanaged
    where B : unmanaged
    where C : unmanaged
{
    [ReadOnly] public NativeArray<RaycastHit> RaycastHits;
    public NativeWrappedData<A> ItemsA;
    public NativeWrappedData<B> ItemsB;
    public NativeWrappedData<C> ItemsC;
    public NativeWrappedData<int> Entities;
    public Executor Action;

    public void Execute(int index)
    {
        var entity = Entities.Array[index];
        var itemA = ItemsA.Array[entity];
        var itemB = ItemsB.Array[entity];
        var itemC = ItemsC.Array[entity];
        var raycast = RaycastHits[index];
        Action.ForEach(ref itemA, ref itemB,ref itemC,ref raycast);
        ItemsA.Array[entity] = itemA;
        ItemsB.Array[entity] = itemB;
        ItemsC.Array[entity] = itemC;
    }
#if UNITY_EDITOR
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        NativeMagic.UnwrapFromNative(ItemsA);
        NativeMagic.UnwrapFromNative(ItemsB);
        NativeMagic.UnwrapFromNative(ItemsC);
        NativeMagic.UnwrapFromNative(Entities);
    }
#endif
}


[BurstCompile(CompileSynchronously = true)]
public struct SetupRaycastComadsJob : IJobParallelFor
{
    public NativeArray<RaycastCommand> Commands;
    public NativeArray<TransformComponent> Transforms;
    public NativeArray<int> Entities;
    public Vector3 Offset;
    public Vector3 Diraction;
    public float Distance;
    public int mask;

    public void Execute(int index)
    {
        var entity = Entities[index];
        var command = Commands[entity];
        command.from = Transforms[entity].position + Offset;
        command.direction = Diraction;
        command.distance = Distance;
        command.layerMask = mask;
        Commands[entity] = command;
    }
}
public interface IJobSystemTag{}


internal static class NativeMagic
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T* GetArrayPtr<T>(T[] data) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            return ptr;
        }
    }
    /// <summary>
    ///     Transform C# Array to NativeArray.
    ///     BIG THANKS FOR LEOPOTAM "https://github.com/Leopotam/ecslite-threads-unity"
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe NativeWrappedData<T> WrapToNative<T>(T[] managedData) where T : unmanaged
    {
        fixed (void* ptr = managedData)
        {
#if UNITY_EDITOR
            var nativeData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, managedData.Length, Allocator.TempJob);
            var sh = AtomicSafetyHandle.Create();
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeData, sh);
            return new NativeWrappedData<T> {Array = nativeData, SafetyHandle = sh};
#else
            return new NativeWrappedData<T> { Array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (ptr, managedData.Length, Allocator.None) };
#endif
        }
    }
#if UNITY_EDITOR
    public static void UnwrapFromNative<T>(NativeWrappedData<T> sh) where T : unmanaged
    {
        AtomicSafetyHandle.CheckDeallocateAndThrow(sh.SafetyHandle);
        AtomicSafetyHandle.Release(sh.SafetyHandle);
    }
#endif
}


public struct NativeWrappedData<TT> where TT : unmanaged
{
    [NativeDisableParallelForRestriction] public NativeArray<TT> Array;
#if UNITY_EDITOR
    public AtomicSafetyHandle SafetyHandle;
#endif
}



public static class UnsafeHelp
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe A* ArrayToPtr<A>(A[] array) where A : unmanaged
    {
        fixed (A* ptr = array)
        {
            return ptr;
        }
    }

    public static unsafe void* ToVoidPtr<A>(this A[] array) where A : unmanaged
    {
        fixed (A* ptr = array)
        {
            return ptr;
        }
    }
    public static unsafe A* ToPointer<A>(this A[] array) where A : unmanaged
    {
        fixed (A* ptr = array)
        {
            return ptr;
        }
    }

    public static unsafe A* GetPtr<A>(this IntPtr ptr) where A : unmanaged
    {
        return (A*)ptr;
    }

    public unsafe static NativeArray<A> ToNative<A>(this A[] array) where A : unmanaged
    {
        fixed (void* ptr = array)
        {
#if UNITY_EDITOR
            var nativeData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<A>(ptr, array.Length, Allocator.TempJob);
            return nativeData;
#else
            return new NativeWrappedData<T> { Array =
 NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (ptr, managedData.Length, Allocator.None) };
#endif
        }
    }
}

public interface IUnsafePool
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe void* Get();
}
public unsafe struct PoolBuffer
{
    [NativeDisableUnsafePtrRestriction] public void* buffer;
}

public interface IJobEntityQueue
{
    EntityQueue Queue { get; set; }
    void Execute(int index);
}

[BurstCompile(CompileSynchronously = true)]
public struct JobEntitiesQueueParallelFor<AExecutor> : IJobParallelFor where AExecutor : IJobEntityQueue
{
    public AExecutor executor;
    public void Execute(int index)
    {
        executor.Execute(index);
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct JobEntitiesQueueFor<AExecutor> : IJobFor where AExecutor : IJobEntityQueue
{
    public AExecutor executor;
    public void Execute(int index)
    {
        executor.Execute(index);
    }
}

public static partial class EcsExtensions
{
    public static void QueueJobParallel<AExecutor,NA, A, B>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor executor) 
        where AExecutor :  IJobEntityQueue 
        where A : unmanaged 
        where B : unmanaged
    {

        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        if(entityType.Count < 1) return;

        EntityQueue q = default;
        q.Init(entityType.Count, EntityQueue.Mode.Parallel);
        q.Add(ref entityType.poolA.items);
        q.Add(ref entityType.poolB.items);
        q.SetEntities(entityType.entities);
        executor.Queue = q;
        JobEntitiesQueueParallelFor<AExecutor> job = default;
        job.executor = executor;
        job.Schedule(entityType.Count, 0).Complete();
        executor.Queue.Dispose();
    }
    
    public static void QueueJobParallel<AExecutor,NA, A, B , C>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor executor) 
        where AExecutor :  IJobEntityQueue 
        where A : unmanaged 
        where B : unmanaged
        where C : unmanaged 
    {

        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B, C>();
        if(entityType.Count < 1) return;

        EntityQueue q = default;
        q.Init(entityType.Count, EntityQueue.Mode.Parallel);
        q.Add(ref entityType.poolA.items);
        q.Add(ref entityType.poolB.items);
        q.Add(ref entityType.poolС.items);
        q.SetEntities(entityType.entities);
        executor.Queue = q;
        JobEntitiesQueueParallelFor<AExecutor> job = default;
        job.executor = executor;
        job.Schedule(entityType.Count, 0).Complete();
        executor.Queue.Dispose();
    }
    
    public static void QueueJobParallel<AExecutor,NA,NB, A,B>(this Entities.EntitiesWithout<NA,NB> ezs, ref AExecutor executor) 
        where AExecutor :  IJobEntityQueue 
        where A : unmanaged 
        where B : unmanaged
    {

        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        if(entityType.Count < 1) return;

        EntityQueue q = default;
        q.Init(entityType.Count, EntityQueue.Mode.Parallel);
        q.Add(ref entityType.poolA.items);
        q.Add(ref entityType.poolB.items);
        q.SetEntities(entityType.entities);
        executor.Queue = q;
        JobEntitiesQueueParallelFor<AExecutor> job = default;
        job.executor = executor;
        job.Schedule(entityType.Count, 0).Complete();
        executor.Queue.Dispose();
    }

    public static void QueueJob<AExecutor,NA, A,B>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor executor) 
        where AExecutor :  IJobEntityQueue 
        where A : unmanaged 
        where B : unmanaged
    {

        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        if(entityType.Count < 1) return;

        EntityQueue q = default;
        q.Init(entityType.Count, EntityQueue.Mode.SingleCore);
        q.Add(ref entityType.poolA.items);
        q.Add(ref entityType.poolB.items);
        q.SetEntities(entityType.entities);
        executor.Queue = q;
        JobEntitiesQueueFor<AExecutor> job = default;
        job.executor = executor;
        job.Run(entityType.Count);

        q.Dispose();
    }
    public static void QueueJob<AExecutor,NA,NB, A,B>(this Entities.EntitiesWithout<NA,NB> ezs, ref AExecutor executor) 
        where AExecutor :  IJobEntityQueue 
        where A : unmanaged 
        where B : unmanaged
    {

        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A, B>();
        if(entityType.Count < 1) return;

        EntityQueue q = default;
        q.Init(entityType.Count, EntityQueue.Mode.SingleCore);
        q.Add(ref entityType.poolA.items);
        q.Add(ref entityType.poolB.items);
        q.SetEntities(entityType.entities);
        executor.Queue = q;
        JobEntitiesQueueFor<AExecutor> job = default;
        job.executor = executor;
        job.Run(entityType.Count);
        executor.Queue.Dispose();
    }
    
    
    public static void ForWithJobWithEntity2<AExecutor, A, B, NA>(this Entities.EntitiesWithout<NA> ezs, ref AExecutor jobExecute)
        where AExecutor : unmanaged,
        IJobForWithEntity<A, B>
        where A : unmanaged
        where B : unmanaged
    {
        var entityType = ezs.GetEntityTypeFromArrayTypePairWithout<A,B>();
        var entities = entityType.entities;

        
        ForWithEntityJob<A, B, AExecutor> job = default;
        jobExecute.MaxIndex = entityType.Count;
        
        job.Set(entities, entityType.poolA.items, entityType.poolB.items, ref jobExecute, entityType.Count, entities.Length);
        
        job.Schedule(entityType.Count, 0).Complete();

        for (int i = job.entitiesUnsafe.Length - 1; i > -1; i--)
        {
            var entity = job.entitiesUnsafe[i];
            var realEntity = ezs.GetWorld().GetEntity(entity.id);
            if (entity.addedComponent != 0)
            {
                if(!realEntity.Has(entity.addedComponent))
                    realEntity.AddBoxed(entity.GetAdded());
            }
        }
        job.Clear();
    }
    
}

public struct EntityQueue : IDisposable
{
    public int Count;
    [NativeDisableUnsafePtrRestriction] private unsafe PoolBuffer* pools;
    [ReadOnly] private NativeHashMap<int, int> poolsMap;
    [ReadOnly] private NativeArray<int> entitiesBuffer;
    private int lastPoolIndex;
    private Mode mode;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe void Init(int size, Mode mode)
    {
        poolsMap = new NativeHashMap<int, int>(8, Allocator.TempJob);
        lastPoolIndex = 0;
        Count = size;
        this.mode = mode;
        //pools = (UnsafePool*)Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(UnsafePool)) * 8);
        pools = (PoolBuffer*) UnsafeUtility.Malloc(Marshal.SizeOf(typeof(PoolBuffer)) * 8, 0, Allocator.TempJob);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetEntities(int[] entities)
    {
        entitiesBuffer = NativeMagic.WrapToNative(entities).Array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Dispose()
    {
        poolsMap.Dispose();
        UnsafeUtility.Free(pools, Allocator.TempJob);
        //Marshal.FreeCoTaskMem((IntPtr)pools);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstCompile]
    internal unsafe void Add<A>(ref A[] pool) where A : unmanaged
    {
        if (!poolsMap.TryAdd(ComponentTypeStruct<A>.ID, lastPoolIndex)) return;
        pools[lastPoolIndex].buffer = NativeMagic.WrapToNative(pool).Array.GetUnsafePtr();
        lastPoolIndex++;
    }
    /// <summary>
    /// <para>Get component from queue by ref in index</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstCompile]
    public unsafe ref A GetAsRef<A>(int index) where A : unmanaged
    {
        var poolIndex = poolsMap[ComponentTypeStruct<A>.ID];
        var idx = entitiesBuffer[index];
        //return ref UnsafeUtility.ArrayElementAsRef<A>(pools[poolIndex].buffer, idx);
        return ref ((A*) pools[poolIndex].buffer)[idx];
    }
    /// <summary>
    /// <para>Get component from queue in index</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstCompile]
    public unsafe A Get<A>(int index) where A : unmanaged
    {
        var poolIndex = poolsMap[ComponentTypeStruct<A>.ID];
        var idx = entitiesBuffer[index];
        return ((A*)pools[poolIndex].buffer)[idx];
    }
    /// <summary>
    /// <para>Set component to queue in index</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstCompile]
    public unsafe void Set<A>(A item, int index) where A : unmanaged
    {
        var poolIndex = poolsMap[ComponentTypeStruct<A>.ID];
        var idx = entitiesBuffer[index];
        ((A*)pools[poolIndex].buffer)[idx] = item;
        //UnsafeUtility.WriteArrayElement(pools[poolIndex].buffer, idx, item);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstCompile]
    public unsafe A* GetPtr<A>(int index) where A : unmanaged
    {
        var poolIndex = poolsMap[ComponentTypeStruct<A>.ID];
        var idx = entitiesBuffer[index];
        return &((A*)pools[poolIndex].buffer)[idx];
    }
    /// <summary>
    /// <para>Get entity id in index</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstCompile]
    public int GetEntity(int index)
    {
        return entitiesBuffer[index];
    }
    /// <summary>
    /// <para>Add component to entity in index in queue</para>
    /// <para>#IMPORTANT# Don't use with QueueJobParallel</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstDiscard]
    public void AddComponent<T>(int index, T component)
    {
        if (mode == Mode.Parallel) throw new Exception($"Can't use AddComponent in {mode} mode");
        MonoConverter.GetWorld().GetEntity(entitiesBuffer[index]).Add(component);
    }
    /// <summary>
    /// <para>Add empty tag component to entity in index in queue</para>
    /// <para>#IMPORTANT# Don't use with QueueJobParallel</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstDiscard]
    public void AddTag<T>(int index)
    {
        if (mode == Mode.Parallel) throw new Exception($"Can't use AddTag in {mode} mode");
        MonoConverter.GetWorld().GetEntity(entitiesBuffer[index]).Set<T>();
    }
    /// <summary>
    /// <para>Crate new entity with tag component</para>
    /// <para>#IMPORTANT# Don't use with QueueJobParallel</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstDiscard]
    public void CreateEntityWith<T>()
    {
        if (mode == Mode.Parallel) throw new Exception($"Can't use CreateEntityWith in {mode} mode");
        MonoConverter.GetWorld().CreateEntity().Set<T>();
    }
    /// <summary>
    /// <para>Crate new entity with component</para>
    /// <para>#IMPORTANT# Don't use with QueueJobParallel</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)][BurstDiscard]
    public void CreateEntityWith<T>(T component)
    {
        if (mode == Mode.Parallel) throw new Exception($"Can't use CreateEntityWith in {mode} mode");
        MonoConverter.GetWorld().CreateEntity().Add(component);
    }
    public enum Mode
    {
        SingleCore,
        Parallel
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EntityUnsafe
{
    public int id;
    public int addedComponent;
    private void* addedComponentPtr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add<A>(A component) where A: unmanaged
    {
        addedComponent = ComponentTypeStruct<A>.ID;
        addedComponentPtr = &component;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTag<A>() where A: unmanaged
    {
        A component = default;
        addedComponent = ComponentTypeStruct<A>.ID;
        addedComponentPtr = &component;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object GetAdded()
    {
        return Marshal.PtrToStructure((IntPtr)addedComponentPtr, ComponentTypeMap.GetTypeByID(addedComponent)); 
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public A GetAdded<A>() where A: unmanaged
    {
        return UnsafeUtility.AsRef<A>(addedComponentPtr);
        //return Marshal.PtrToStructure<A>((IntPtr)addedComponentPtr); 
    }
}
