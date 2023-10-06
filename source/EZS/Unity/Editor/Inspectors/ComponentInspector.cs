using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wargon.ezs.Unity
{
    
    public interface IComponentInspector
    {
        Type ComponentType { get;}
        void OnCreate();
        object Draw(object component);
    }

    public abstract class ComponentInspector<T> : IComponentInspector
    {
        private Type componentType;
        protected T target;
        Type IComponentInspector.ComponentType => componentType;

        public ComponentInspector()
        {
            componentType = typeof(T);
        }

        public virtual void OnCreate(){}
        public object Draw(object component)
        {
            var obj = (T)component;
            return DrawInternal(ref obj);
        }

        protected abstract ref T DrawInternal(ref T component);
    }
}