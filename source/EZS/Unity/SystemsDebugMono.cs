using System;
using System.Diagnostics;
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
        private readonly Stopwatch stopwatch;
        private readonly Stopwatch[] stopwatches;
        
        public SystemsDebug(Systems systems, World world)
        {
            Systems = systems;
            //Systems.SetListener(this);
            var newDebugMono = new GameObject().AddComponent<SystemsDebugMono>();
            world.SubOnDestoy(() =>
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
            for (var i = 0; i < stopwatches.Length; i++)
                stopwatches[i] = new Stopwatch();
            executeTimes = new double[systems.updateSystemsList.Count];
        }

        public Systems Systems { get; }

        public void StartCheck()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public void StopCheck()
        {
            stopwatch.Stop();
            executeTime = stopwatch.Elapsed.TotalMilliseconds;
        }

        public void StartCheck(int index)
        {
            stopwatches[index].Reset();
            stopwatches[index].Start();
        }

        public void StopCheck(int index)
        {
            stopwatches[index].Stop();
            executeTimes[index] = stopwatches[index].Elapsed.TotalMilliseconds;
        }
    }
}