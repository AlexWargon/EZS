using System;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class WorldDebug : MonoBehaviour
    {
        public World world;
        private void Update()
        {
            DebugUpdate.Redraw?.Invoke();
        }

        private void OnDestroy()
        {
            DebugUpdate.Redraw = null;
        }
    }

    public static class DebugUpdate
    {
        public static Action Redraw;
    }
}
