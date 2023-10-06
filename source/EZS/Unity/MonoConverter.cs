using System.Collections.Generic;

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
        public static void Execute(Entity entity, List<object> components, MonoEntity monoEntity = null) {
            foreach (var component in components) {
                if(component!=null)
                    entity.AddBoxed(component);
            }
            entity.Add(new EntityConvertedEvent());
        }
    }
}


