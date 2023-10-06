using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wargon.ezs.Unity {
    [CreateAssetMenu(fileName = "EntityBlueprint", menuName = "EZS/EntityBlueprint", order = 1)]
    public class EntityBlueprint : ScriptableObject {
        [SerializeReference] public List<object> Components;
        private int[] types = Array.Empty<int>();
        private Entity entityToCopy;
        private bool initialized;
        public Entity CreateEntity() {
            if(!initialized) Initialize();
            var world = MonoConverter.GetWorld();
            var e = world.CreateEntity();
            if (entityToCopy.IsNULL()) {
                for (var i = 0; i < types.Length; i++) {
                    if (!AddComponent(e, Components[i], types[i])) {
                        Debug.LogError($"Cant add component {Components[i].GetType()} in index {i}");
                    }
                }
                entityToCopy = e;
            }
            else {
                e = entityToCopy.Copy();
            }
            return e;
        }
        
        private void Initialize() {
            types = new int[Components.Count];
            for (var i = types.Length - 1; i >= 0; i--) {
                types[i] = ComponentType.GetID(Components[i].GetType());
            }

            initialized = true;
        }
        private bool AddComponent(Entity entity, object component, int typeId) {
            ref var data = ref entity.GetEntityData();
            // if (data.componentTypes.Add(typeId)) {
            //     data.componentsCount++;
            //     var pool = entity.World.GetPoolByID(typeId);
            //     pool.SetBoxed(component, entity.id);
            //     pool.Add(entity.id);
            //     entity.World.OnAddComponent(typeId, in entity);
            //     return true;
            // }
            return false;
        }
        
    }
}