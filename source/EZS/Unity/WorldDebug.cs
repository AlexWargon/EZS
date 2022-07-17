using System;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    public class WorldDebug : MonoBehaviour
    {
        public World world;
        private void LateUpdate()
        {
            DebugUpdate.Redraw?.Invoke();
        }
    }

    public static class DebugUpdate
    {
        public static Action Redraw;
    }
}
