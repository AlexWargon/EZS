using UnityEditor;

namespace Wargon.ezs.Unity
{
    [InitializeOnLoad]
    public class EzsDebugConfigs
    {
        public bool Colored;
        
        static EzsDebugConfigs()
        {
            ComponentTypesList.Init();
            ComponentInspectorInternal.Init();
        }
    }
}