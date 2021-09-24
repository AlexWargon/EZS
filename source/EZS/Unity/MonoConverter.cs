using System.Collections.Generic;
using System.Reflection;

namespace Wargon.ezs.Unity {
    public static class MonoConverter {
        private static World world;
        public static bool HasWorld => world != null;
        public static void Init(World ecsWorld) 
        {
            world = ecsWorld;
        }

        public static World GetWorld()
        {
            return world;
        }
        public static void Execute(Entity entity, List<object> components) {
            foreach (var component in components) {
                entity.AddBoxed(component);
            }
        }
    }
}


