using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wargon.ezs;
using Object = UnityEngine.Object;

namespace Wargon.ezs.Unity
{
    public class SystemsDebugMono : MonoBehaviour
    {
        public SystemsDebug Systems { get; private set; }
        public void Init(SystemsDebug systems)
        {
            Systems = systems;
        }
    }

    public sealed class SystemsDebug : ISystemListener
    {
        public int index;
        public double executeTime;
        public readonly double[] executeTimes;
        public readonly bool[] active;
        private readonly Stopwatch stopwatch;
        private readonly Stopwatch[] stopwatches;
        
        public SystemsDebug(Systems systems, World world)
        {
            Systems = systems;
            //Systems.SetListener(this);
            var newDebugMono = new GameObject().AddComponent<SystemsDebugMono>();
            world.SubOnDestroy(() =>
            {
                if(newDebugMono)
                    Object.Destroy(newDebugMono.gameObject);
            });

            Object.DontDestroyOnLoad(newDebugMono);
            newDebugMono.gameObject.name = "Systems Debug";
            newDebugMono.Init(this);
            newDebugMono.transform.SetSiblingIndex(0);
            stopwatch = new Stopwatch();
            stopwatches = new Stopwatch[systems.updateSystemsList.Count];
            active = new bool[systems.updateSystemsList.Count];
            for (var i = 0; i < stopwatches.Length; i++) {
                stopwatches[i] = new Stopwatch();
                active[i] = true;
            }
            executeTimes = new double[systems.updateSystemsList.Count];
        }

        public Systems Systems { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Active(int index) {
            return active[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StartCheck()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopCheck()
        {
            stopwatch.Stop();
            executeTime = stopwatch.Elapsed.TotalMilliseconds;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StartCheck(int index)
        {
            stopwatches[index].Reset();
            stopwatches[index].Start();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopCheck(int index)
        {
            stopwatches[index].Stop();
            executeTimes[index] = stopwatches[index].Elapsed.TotalMilliseconds;
        }
    }
}