using System;
using System.Runtime.CompilerServices;
using Wargon.ezs.Unity;

namespace Wargon.ezs {
    public static class EntityExtension
    {
        public static void AddBoxed(this Entity entity, object component)
        {
            if (component == null)
            {
                UnityEngine.Debug.LogError($"Try add null component on entity {entity.id} " + Environment.NewLine +
                                           "Looks like some component on prefab was currupted :C");
            }

            if (component != null) {
                var type = component.GetType();
                var typeId = ComponentTypeMap.GetID(type);

                ref var data = ref entity.GetEntityData();
                var pool = entity.World.GetPoolByID(typeId);
                if (data.generation != entity.generation)
                    throw new Exception("ENTITY NULL OR DESTROYED. Method: Entity.AddBoxed()");

                data.componentTypes.Add(typeId);
                data.componentsCount++;
                pool.SetBoxed(component, entity.id);
                pool.Add(entity.id);
                entity.World.OnAddComponent(typeId, in entity);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(this Entity entity, int type)
        {
            return entity.GetEntityData().componentTypes.Contains(type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(this Entity entity, Type type)
        {
            return entity.GetEntityData().componentTypes.Contains(ComponentTypeMap.GetID(type));
        }


        
        public static MonoEntity SaveEntity(this Entity entity)
        {
            var monoEntity = entity.Get<View>().Value;
            var world = entity.World;
            ref var data = ref entity.GetEntityData();
            monoEntity.Components.Clear();
            foreach (var dataComponentType in data.componentTypes)
            {
                var pool = world.GetPoolByID(dataComponentType);
                var component = pool.Get(entity.id);
                monoEntity.Components.Add(component);
            }
            
            return monoEntity;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetOwner(this Entity entity) {
            ref var world = ref entity.World;
            ref var data = ref entity.GetEntityData();
            var componentID = ComponentType<Owner>.ID;
            if (data.componentTypes.Contains(componentID)) return world.GetPool<Owner>().Get(entity.id).value;
            throw new Exception($"ENTITY {entity.id} HAS NO OWNER! Method: Entity.GetOwner()");
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity[] GetChildren(this Entity entity) {
            return entity.World.Entities.GetOwnerQuery(entity.id).GetEntityQuery();
        }

        public static ref T GetIfHas<T>(this Entity entity) where T: new() {
            ref var data = ref entity.GetEntityData();
            var typeId = ComponentType<T>.ID;
            try {
                if (data.componentTypes.Contains(typeId)) return ref entity.World.GetPool<T>().items[entity.id];
            }
            catch {
                // ignored
            }
            throw new Exception($"SSSS");
        }
    }
}