using System;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace Wargon.ezs {
    public static class JobsAndNativeExtensions {
        public static bool TryComplete(this Unity.Jobs.JobHandle @this)
        {
            if (!@this.IsCompleted) return false;
            @this.Complete();    return true;
        }



        
        public static Unity.Collections.NativeParallelMultiHashMap<TKey, TValue> Clone<TKey, TValue>(this ref Unity.Collections.NativeParallelMultiHashMap<TKey, TValue> @this, Unity.Collections.Allocator alloc)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {

            Unity.Collections.NativeParallelMultiHashMap<TKey, TValue> cloneHashMap = new Unity.Collections.NativeParallelMultiHashMap<TKey, TValue>(@this.Count(), alloc);

            Unity.Collections.NativeParallelMultiHashMapIterator<TKey> it;
            Unity.Collections.NativeArray<TKey> keys = @this.GetKeyArray(Unity.Collections.Allocator.Temp);
            TKey key;
            TValue value;

            for (int k = 0, count = keys.Length; k < count; k++)
            {
                key = keys[k];
                if (@this.TryGetFirstValue(key, out value, out it))
                {
                    cloneHashMap.Add(key, value);
                    while (@this.TryGetNextValue(out value, ref it))
                    {
                        cloneHashMap.Add(key, value);
                    }
                }
            }

            keys.Dispose();

            return cloneHashMap;
        }
    }
    public static class IJobForExtensions {
        public static ref JobHandle ScheduleInSystem<TJob>(this ref TJob job, int count, UpdateSystem system) where TJob : struct, IJobFor {
            system.Dependencies = job.Schedule(count, system.Root.Dependency);
            system.Root.Dependency = system.Dependencies;
            return ref system.Root.Dependency;
        }
        public static ref JobHandle ScheduleParalleInSystem<TJob>(this ref TJob job, int count, UpdateSystem system) where TJob : struct, IJobFor {
            system.Dependencies = job.ScheduleParallel(count, 1, system.Root.Dependency);
            system.Root.Dependency = system.Dependencies;
            return ref system.Root.Dependency;
        }
    }
    public static class IJobParallelForExtensions {
        public static ref JobHandle ScheduleInSystem<TJob>(this ref TJob job, int count, UpdateSystem system) where TJob : struct, IJobParallelFor {
            system.Dependencies = job.Schedule(count, 1, system.Root.Dependency);
            system.Root.Dependency = system.Dependencies;
            return ref system.Root.Dependency;
        }
    }
    public static class IJobParallelForTransformExtensions {
        public static ref JobHandle ScheduleInSystem<TJob>(this ref TJob job, TransformAccessArray array, UpdateSystem system) where TJob : struct, IJobParallelForTransform {
            system.Dependencies = job.Schedule(array, system.Root.Dependency);
            system.Root.Dependency = system.Dependencies;
            return ref system.Root.Dependency;
        }
    }
}