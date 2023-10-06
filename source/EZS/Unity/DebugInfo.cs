using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wargon.ezs;

namespace Wargon.ezs.Unity
{
    public class DebugInfo
    {
        private List<ISystemListener> systemListeners;
    
        public DebugInfo(World world)
        {
            systemListeners = new List<ISystemListener>();
            var systemsPool = world.GetAllSystems();
            var worldDebug = new GameObject("ECS World Debug").AddComponent<WorldDebug>();
            SceneManager.sceneUnloaded += scnene =>
            {
                if (!worldDebug) return;
                if(worldDebug.gameObject!=null)
                    Object.Destroy(worldDebug.gameObject);
            };
            Object.DontDestroyOnLoad(worldDebug);
            worldDebug.world = world;
            worldDebug.transform.SetSiblingIndex(0);
            //Debug.Log($"systems count {world.GetSystemsCount()}");
            for (var i = 0; i < world.GetSystemsCount(); i++)
            {
                var systems = systemsPool[i];
                var newListener = new SystemsDebug(systems, world);
                systems.SetListener(newListener);
                systemListeners.Add(newListener);
            }
        }

    }
}


