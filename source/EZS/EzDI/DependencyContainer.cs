using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wargon.DI
{
    public class DependencyContainer : IDisposable
    {
        private static readonly Dictionary<Type, object> globals = new Dictionary<Type, object>();
        public static Dictionary<Type, object> Globals => globals;
        private readonly Dictionary<Type, Container> containers = new Dictionary<Type, Container>();
        private readonly Dictionary<Type, Context> contexts = new Dictionary<Type, Context>();

        public void Dispose() {
            containers.Clear();
            contexts.Clear();
        }
        public void DisposeAll() {
            containers.Clear();
            contexts.Clear();
            globals.Clear();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T New<T>() where T : class, new()
        {
            var newObj = new T();
            var type = typeof(T);
            if (!contexts.ContainsKey(type)) contexts.Add(type, new Context(type, this));
            contexts[type].Inject(newObj);
            return newObj;
        }

        public void Resolve(Type type, object item)
        {
            if (!contexts.ContainsKey(type)) contexts.Add(type, new Context(type, this));
            contexts[type].Inject(item);
        }
        

        public void Resolve<T>(T item) where T : class
        {
            var type = typeof(T);
            if (!contexts.ContainsKey(type)) contexts.Add(type, new Context(type, this));
            contexts[type].Inject(item);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResolveObject(object item)
        {
            var type = item.GetType();
            if (!contexts.ContainsKey(type)) contexts.Add(type, new Context(type, this));
            contexts[type].Inject(item);
        }
        public T Instatiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            var component = Object.Instantiate(prefab, position, rotation);
            Resolve(component);
            return component;
        }

        public T Instatiate<T>() where T : MonoBehaviour
        {
            var newObj = new GameObject();
            var newComponent = newObj.AddComponent<T>();
            Resolve(newComponent);
            newObj.name = newComponent.GetType().Name;
            return newComponent;
        }

        public T Instatiate<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            var newObj = new GameObject();
            var newComponent = newObj.AddComponent<T>();
            Resolve(newComponent);
            newObj.name = newComponent.GetType().Name;
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            return newComponent;
        }

        public void AddAsSingle<T>(T toInject) where T : class
        {
            containers.Add(typeof(T), new Container(typeof(T), toInject, DiType.Single));
        }

        public T GetSingle<T>() where T : class {
            return (T)containers[typeof(T)].Get();
        }
        public void Add<T>() where T : class
        {
            containers.Add(typeof(T), new Container(typeof(T), null, DiType.New));
        }

        public void AddAsGlobal<T>(T toInject) where T : class
        {
            if (!globals.ContainsKey(typeof(T))) {
                globals.Add(typeof(T), toInject);
            }
            else {
                if(toInject is MonoBehaviour m)
                    Object.Destroy(m.gameObject);
            }
        }

        public bool HasGlobal(Type type)
        {
            return globals.ContainsKey(type);
        }

        public bool HasSingle(Type type)
        {
            return containers.ContainsKey(type);
        }

        public Container GetContainer<T>()
        {
            return containers[typeof(T)];
        }

        public Container GetContainer(Type type)
        {
            return containers[type];
        }
        
    }
    public enum DiType
    {
        New,
        Single,
        Global
    }
}