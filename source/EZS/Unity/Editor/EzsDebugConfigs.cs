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
            Debug.Log("RELOADED");
            ComponentTypesList.Init();
            ComponentInspectorInternal.Init();
        }
    }
    [Serializable]
    public class Context
    {
        public string TypeName;
        public GUIStyle Style;
    }
}