using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Wargon.ezs.Unity {
    [DisallowMultipleComponent]
    public class MonoEntity : MonoBehaviour {
        public Entity Entity;

        [SerializeReference] public List<object> Components = new List<object>();

        public bool runTime;
        public bool destroyObject;
        public bool destroyComponent;
        public int id;
        private bool converted;
        private World world;
        public int ComponentsCount => runTime ? Entity.GetEntityData().componentTypes.Count : Components.Count;
        private IEnumerator Start() {
            yield return null;
            ConvertToEntity();
        }
#if UNITY_EDITOR
        private void OnEnable() {
            Enable();
        }
#endif
#if UNITY_EDITOR
        private void OnDisable() {
            Disable();
        }
#endif

        private void OnDestroy() {
            if (!destroyObject)
                if (world != null)
                    if (world.Alive)
                        if (!Entity.IsDead())
                            Entity.Destroy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ConvertToEntity() {
            if (converted) return;
            Entity = MonoConverter.GetWorld().CreateEntity();
            world = Entity.world;
#if UNITY_EDITOR
            gameObject.name = $"{gameObject.name} ID:{Entity.id.ToString()}";
#endif
            id = Entity.id;
            MonoConverter.Execute(Entity, Components);
            converted = true;
            if (destroyComponent) Destroy(this);
            if (destroyObject) Destroy(gameObject);
            runTime = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>() {
            return Entity.Get<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>() {
            Entity.Remove<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActive(bool state) {
            if (state)
                Enable();
            else
                Disable();
            gameObject.SetActive(state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Disable() {
            if (!converted) return;
            if (!world.Alive) return;
            if (Entity.IsDead()) return;
            Entity.Set<UnActive>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Enable() {
            if (!converted) return;
            if (Entity.Has<UnActive>())
                Entity.Remove<UnActive>();
        }

        public void DestroyWithoutEntity() {
            destroyObject = true;
            Destroy(gameObject);
        }
    }

    [EcsComponent]
    public class View {
        public MonoEntity value;
    }
}