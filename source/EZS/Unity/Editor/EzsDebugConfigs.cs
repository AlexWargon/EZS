using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    [InitializeOnLoad]
    public class EzsDebugConfigs
    {
        public bool Colored;
        
        static EzsDebugConfigs()
        {
            //Debug.Log("RELOADED");
            
            ComponentTypesList.Init();
            ComponentInspector.Init();
        }
    }
    [CreateAssetMenu(fileName = "EzsDebugStyles", menuName = "EZS/EzsDebugStyles", order = 1)]
    public class EzsDebugStyles : ScriptableObject
    {
        public List<Context> styles;
        private static EzsDebugStyles instance;
        public static EzsDebugStyles Isntance
        {
            get
            {
                if (instance == null)
                {
                    EzsDebugStyles[] results = Resources.FindObjectsOfTypeAll<EzsDebugStyles>();
                    instance = results[0];
                    instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }

                return instance;
            }
        }
    }
    [Serializable]
    public class Context
    {
        public string TypeName;
        public GUIStyle Style;
    }
}