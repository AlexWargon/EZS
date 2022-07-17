using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Wargon.ezs.Unity {

    [DisallowMultipleComponent]
    public class MonoEntity : MonoBehaviour {
        public Entity Entity;
        private World world;
        [SerializeReference]
        public List<object> Components = new List<object>();
        public int ComponentsCount => runTime ? Entity.GetEntityData().componentTypes.Count : Components.Count;
        public bool runTime;
        public bool destroyObject;
        public bool destroyComponent;
        public int id;
        private bool converted;
        private IEnumerator Start()
        {
            yield return null;
            ConvertToEntity();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ConvertToEntity() {
            if(converted) return;
            Entity = MonoConverter.GetWorld().CreateEntity();
            world = Entity.world;
            MonoConverter.Execute(Entity, Components);
            id = Entity.id;
            converted = true;
            if (destroyComponent) Destroy(this);
            if (destroyObject) Destroy(gameObject);
            runTime = true;
#if UNITY_EDITOR
            gameObject.name = $"{gameObject.name} ID:{Entity.id.ToString()}";
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>()
        {
            return Entity.Get<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<T>()
        {
            Entity.Remove<T>();
        }
        
        private void OnDestroy() {
            if(!destroyObject)
                if(world!= null)
                    if(world.Alive)
                        if(!Entity.IsDead())
                            Entity.Destroy();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActive(bool state)
        {
            if(state)
                Enable();
            else
                Disable();
            gameObject.SetActive(state);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Disable()
        {
            if(!converted) return;
            if (!world.Alive) return;
            if(Entity.IsDead()) return;
            Entity.Set<UnActive>();
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Enable()
        {
            if(!converted) return;
            if(Entity.Has<UnActive>())
                Entity.Remove<UnActive>();

        }
#if UNITY_EDITOR
        private void OnDisable()
        {
            Disable();
        }
#endif
#if UNITY_EDITOR      
        private void OnEnable()
        {
            Enable();
        }
#endif
        public void DestroyWithoutEntity()
        {
            destroyObject = true;
            Destroy(this.gameObject);
        }
    }
    [EcsComponent]
    public class View {
        public MonoEntity Value;
    }
    [EcsComponent] 
    public class UnActive { }
}

