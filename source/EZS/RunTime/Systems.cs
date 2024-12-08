using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using UnityEngine;
using Wargon.DI;

namespace Wargon.ezs {
    public class Systems {
        public bool Alive;
        public int id;
        private bool debugMode;
        private readonly World world;
        private readonly DynamicArray<DestroySystem> destroySystemsList = new DynamicArray<DestroySystem>(4);
        private readonly DynamicArray<InitSystem> initSystemsList = new DynamicArray<InitSystem>(4);
        private readonly DynamicArray<ReactiveSystem> reactiveSystmes = new DynamicArray<ReactiveSystem>(4);
        public readonly DynamicArray<UpdateSystem> updateSystemsList = new DynamicArray<UpdateSystem>(16);
        private readonly Dictionary<int, List<IOnAdd>> onAddSystems = new Dictionary<int, List<IOnAdd>>(4);
        private readonly Dictionary<int, List<IOnRemove>> onRemoveSystems = new Dictionary<int, List<IOnRemove>>(4);
        private readonly Dictionary<int, List<ReactiveSystem>> reactiveSystemsMap = new Dictionary<int, List<ReactiveSystem>>(4);
        private ISystemListener systemBigListener;
        private int updateSystemsCount;
        internal Unity.Jobs.JobHandle Dependency;
        public ref Unity.Jobs.JobHandle RefDependency => ref Dependency;
        public Systems(World world) {
            this.world = world;
            this.AddFeature(new StartFrameSystemGroup());
            world.AddSystems(this);
        }

        public void Init() {
            this.AddFeature(new EndFrameSystemGroup());

            for (var i = 0; i < initSystemsList.Count; i++)
                initSystemsList[i].Execute();
            for (int i = 0; i < updateSystemsCount; i++) {
                updateSystemsList.Items[i].OnCreateInternal();
            }
            for (int i = 0; i < updateSystemsCount; i++) {
                _SetupSystemFields(updateSystemsList.Items[i]);
            }
            Alive = true;
        }

        public void SetListener(ISystemListener listener) {
            systemBigListener = listener;
            debugMode = systemBigListener != null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add<T>(T system) where T : UpdateSystem {
            system.PreInit();
            Injector.ResolveObject(system);
            _IfRemoveBeforeThenAddRemoveSystem(system);
            _IfEventSystemThenAddClearSystem(system);
            _SetupPools(system);
            system.Init(world.Entities, world, this);
            system.ID = updateSystemsCount;
            system.TypeName = typeof(T).Name;
            updateSystemsList.Add(system);
            updateSystemsCount++;
            return this;
        }

        private void _SetupPools<T>(T system) where T : UpdateSystem {
            foreach (var fieldInfo in system.SystemType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if (fieldInfo.FieldType.GetInterface(nameof(IPool)) != null) {
                    var value = fieldInfo.GetValue(system);
                    if (value == null) {
                        var poolType = fieldInfo.FieldType.GetGenericArguments()[0];
                        var componentTypeIndex = ComponentType.GetID(poolType);
                        fieldInfo.SetValue(system, world.GetPoolByID(componentTypeIndex));
                    }
                }
            }
        }

        private void _SetupSystemFields<T>(T system) where T : UpdateSystem {
            foreach (var fieldInfo in system.SystemType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(UpdateSystem))) {
                    var value = fieldInfo.GetValue(system);
                    if (value == null) {
                        fieldInfo.SetValue(system, world.GetSystem(fieldInfo.FieldType));
                    }
                }
            }
        }
        private void _IfRemoveBeforeThenAddRemoveSystem<T>(T eventSystem) where T: UpdateSystem {
            var removeBeforeTypes = GetGenericType(eventSystem.SystemType, typeof(IRemoveBefore<>));
            foreach (var type in removeBeforeTypes) {
                var typeId = ComponentType.GetID(type);
                var system = CreateClearEventSystem(typeId);
                Add(system);
            }
            var clearEvent = GetGenericType(eventSystem.GetType(), typeof(IClearBuffer<>));
            foreach (var type in clearEvent) {
                Add(new ClearEventBufferSystem(world.GetBuffer(type)));
            }
        }
        
        private void _IfEventSystemThenAddClearSystem<T>(T eventSystem) where T: UpdateSystem {
            var fields = eventSystem.SystemType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields) {
                if (typeof(IEventBuffer).IsAssignableFrom(fieldInfo.FieldType)) {
                    var arguments = fieldInfo.FieldType.GenericTypeArguments;
                    if (arguments.Length > 0) {
                        var gen = fieldInfo.FieldType.GenericTypeArguments[0];
                        if (gen != null) {
                            var buffer = world.GetBuffer(gen);
                            fieldInfo.SetValue(eventSystem, buffer);
                        }
                    }
                }
            }
        }
        
        private static UpdateSystem CreateClearEventSystem(int eventTypeID) {
            return new RemoveComponentSystem(eventTypeID);
        }
        private static List<Type> GetGenericType(Type system, Type @interface) {
            var types = new List<Type>();
            foreach(var type in system.GetInterfaces()) {
                if(type.IsGenericType && type.GetGenericTypeDefinition() == @interface) {
                    types.Add(type.GetGenericArguments()[0]);
                }
            }

            return types;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add(InitSystem system) {
            Injector.ResolveObject(system);
            initSystemsList.Add(system);
            if (system is IInject inject) {
                inject.Inject(world);
            }
            system.Init(world.Entities, world);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add(DestroySystem system) {
            destroySystemsList.Add(system);
            if (system is IInject inject) {
                inject.Inject(world);
            }
            system.Init(world.Entities, world);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems AddFeature(SystemsGroup systemsGroup) {
            var systems = systemsGroup.GetSystems();
            for (var i = 0; i < systems.Count; i++) Add(systems.Items[i]);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems AddReactive(IReactive system) {
            if (system is IInject inject) {
                inject.Inject(world);
            }
            if (system is IOnAdd add) {
                var triggerType = add.TriggerType;
                if (!onAddSystems.ContainsKey(triggerType))
                    onAddSystems.Add(triggerType, new List<IOnAdd>());
                onAddSystems[triggerType].Add(add);
                Injector.ResolveObject(add);
                add.Init(world.Entities, world);
            }
            else if (system is IOnRemove remove) {
                var triggerType = remove.TriggerType;
                if (!onRemoveSystems.ContainsKey(triggerType))
                    onRemoveSystems.Add(triggerType, new List<IOnRemove>());
                onRemoveSystems[triggerType].Add(remove);
                Injector.ResolveObject(remove);
                remove.Init(world.Entities, world);
            }
            else if (system is ReactiveSystem reactive) {
                var triggerType = reactive.TriggerType;
                if (!reactiveSystemsMap.ContainsKey(triggerType))
                    reactiveSystemsMap.Add(triggerType, new List<ReactiveSystem>());
                reactiveSystemsMap[triggerType].Add(reactive);
                reactiveSystmes.Add(reactive);
                reactive.Init(world.Entities, world);
                Injector.ResolveObject(reactive);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAdd(int type, in Entity entity) {
            if (!Alive) return;
            if (onAddSystems.TryGetValue(type, out var list)) {
                var cnt = list.Count;
                for (var index = 0; index < cnt; index++) list[index].Execute(in entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemove(int type, in Entity entity) {
            if (!Alive) return;
            if (onRemoveSystems.TryGetValue(type, out var list)) {
                for (var index = 0; index < list.Count; index++) {
                    var system = list[index];
                    system.Execute(in entity);
                }
            }
        }

        internal void AddReactiveTrigger<T>(T item) where T: struct{
            foreach (var reactiveSystem in reactiveSystemsMap[ComponentType<T>.ID]) {
                ((Trigger<T>) reactiveSystem.Rx).Add(item);
            }
        }

        internal void Destroy() {
            Alive = false;
            for (var i = 0; i < destroySystemsList.Count; i++)
                destroySystemsList[i].Execute();
            for (var i = 0; i < updateSystemsCount; i++)
                updateSystemsList[i].OnDestroy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUpdate() {
            if (!Alive) return;

            if (debugMode) {
                systemBigListener.StartCheck();
                for (var i = 0; i < updateSystemsCount; i++) {
                    systemBigListener.StartCheck(i);
                    if (systemBigListener.Active(i)) {
                        updateSystemsList.Items[i].OnUpdate();
                        world.UpdateWorld();
                    }
                    systemBigListener.StopCheck(i);
                }

                systemBigListener.StopCheck();
            }
            else {
                for (var i = 0; i < updateSystemsCount; i++) {
                    updateSystemsList.Items[i].OnUpdate();
                    world.UpdateWorld();
                }
            }
        }

        public void Stop() {
            Alive = false;
            Dependency.Complete();
        }
    }
    public static class Generic {
        public static object New(Type genericType, Type elementsType, params object[] parameters) {
            return Activator.CreateInstance(genericType.MakeGenericType(elementsType), parameters);
        }
    }
    public class SystemsGroup {
        public string name;
        public readonly DynamicArray<UpdateSystem> systems = new DynamicArray<UpdateSystem>(4);

        public SystemsGroup() {
            name = "NO_NAME";
        }

        public SystemsGroup(string name) {
            this.name = name;
        }

        public SystemsGroup Add<T>(T system) where T: UpdateSystem{
            systems.Add(system);
            return this;
        }

        public DynamicArray<UpdateSystem> GetSystems() {
            return systems;
        }

        public string GetName() {
            return name;
        }
    }


    public interface IReactive {
        int TriggerType { get; }
        void Init(Entities entities, World world);
    }

    internal interface IOnAdd : IReactive {
        void Execute(in Entity entity);
    }

    internal interface IOnRemove : IReactive {
        void Execute(in Entity entity);
    }

    public struct Reactive<T> {
        private T trigger;
        private T valueInternal;

        public Reactive(T trigger) {
            this.trigger = trigger;
            valueInternal = default;
        }

        public T Value {
            get => valueInternal;
            set {
                valueInternal = value;
                if (Compare(valueInternal, trigger))
                    Trigger();
            }
        }

        public void Trigger() { }

        private static bool Compare<T>(T x, T y) {
            return EqualityComparer<T>.Default.Equals(x, y);
        }
    }

    public interface ITrigger {
        int TriggerType { get; }
        bool Check();
    }

    public class Trigger<T> : ITrigger where T: struct {
        public int TriggerType => ComponentType<T>.ID;
        private Func<T, bool> predicate;
        private T[] items;
        private int count;

        public static Trigger<T> Match(Func<T, bool> predicate) {
            var n = new Trigger<T>();
            n.items = new T[64];
            n.predicate = predicate;
            n.count = 0;
            return n;
        }

        public void Add(T addNew) {
            items[count] = addNew;
            count++;
        }

        public bool Check() {
            for (var i = 0; i < count; i++) {
                return predicate.Invoke(items[i]);
            }

            return false;
        }

    }

    public abstract class ReactiveSystem : IReactive {
        private int triggerType;
        public int TriggerType { get; }
        protected Entities entities;
        protected World world;
        public ITrigger Rx;

        public void Init(Entities e, World w) {
            entities = e;
            world = w;
            Rx = GetTrigger();
            triggerType = Rx.TriggerType;
        }

        protected abstract ITrigger GetTrigger();
        public abstract void Execute();
    }

    public abstract class OnAdd<A> : IOnAdd where A : struct{
        protected Entities entities;
        protected World world;
        public int TriggerType => ComponentType<A>.ID;

        public void Init(Entities entities, World world) {
            this.entities = entities;
            this.world = world;
            OnCreate(world);
        }
        protected virtual void OnCreate(World world){}
        public abstract void Execute(in Entity entity);
    }

    public abstract class OnRemove<A> : IOnRemove where A : struct {
        protected Entities entities;
        protected World world;
        public int TriggerType => ComponentType<A>.ID;

        public void Init(Entities entities, World world) {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute(in Entity entity);
    }

    public abstract class InitSystem {
        protected Entities entities;
        protected World world;

        public void Init(Entities entities, World world) {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute();
    }

    public abstract class DestroySystem {
        protected Entities entities;
        protected World world;

        public void Init(Entities entities, World world) {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute();
    }

    public interface IInject {
        void Inject(World world);
    }
    public interface IClearBuffer<T> where T: struct, IEvent { }
    public interface IRemoveBefore<T>  where T: struct { }
    public abstract class UpdateSystem {
        public Unity.Jobs.JobHandle Dependencies;
        protected ref Unity.Jobs.JobHandle DependenciesAll => ref Root.Dependency;
        protected EntitiesEach entities;
        internal int ID;
        protected World world;
        internal Systems Root;
        internal string TypeName;
        internal Type SystemType;
        internal void PreInit() {
            SystemType = GetType();
            TypeName = SystemType.Name;
        }
        internal void Init(Entities newEntities, World newWorld, Systems systems) {
            world = newWorld;
            Root = systems;
            execute = new Default(this);
            OnInit();
        }

        public void SkipFrames(int value) {
            execute = new FrameSkip(value, this);
        }
        
        internal void OnCreateInternal() {
            OnCreate();
        }
        protected virtual void OnCreate() { }
        public virtual void OnInit(){ }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Update();
        public virtual void OnDestroy() {}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void UpdateN(){}

        private IUpdateExecute execute;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUpdate() {
            execute.Execute();
        }

        private interface IUpdateExecute {
            void Execute();
        }
        private class FrameSkip : IUpdateExecute {
            private UpdateSystem system;
            private int _framesToSkipCounter;
            private readonly int _framesToSkip;
            public FrameSkip(int frames, UpdateSystem system) {
                this.system = system;
                _framesToSkip = frames;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Execute() {
                _framesToSkipCounter++;
                if (_framesToSkipCounter != _framesToSkip) return;
                system.UpdateN();
                _framesToSkipCounter = 0;
            }
        }
        private class Default : IUpdateExecute {
            private readonly UpdateSystem system;
            public Default(UpdateSystem system) {
                this.system = system;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Execute() {
                system.UpdateN();
            }
        }
    }
    public struct DestroyEntityEvent { }
    
    public interface ISystemListener {
        void StartCheck();
        void StopCheck();
        void StartCheck(int index);
        void StopCheck(int index);
        bool Active(int index);
    }
    public partial class RemoveComponentSystem : UpdateSystem {
        private readonly int typeID;
        private EntityQuery query;
        public RemoveComponentSystem(int type) => typeID = type;
        public RemoveComponentSystem(Type type) => typeID = ComponentType.GetID(type);
        protected override void OnCreate() {
            query = world.GetQuery().With(typeID);
        }
        public override void Update() {
            for (int index = 0; index < query.Count; index++) {
                ref var e = ref query.GetEntity(index);
                e.RemoveByTypeID(typeID);
            }
        }
    }

    public partial class ClearEventBufferSystem : UpdateSystem {
        private IEventBuffer buffer;

        public ClearEventBufferSystem(IEventBuffer buffer) {
            this.buffer = buffer;
        }
        public override void Update() {
            buffer.Clear();
        }
    }

    public partial class DestroyEntitySystem : UpdateSystem {
        private EntityQuery Query;
        protected override void OnCreate() {
            Query = world.GetQuery().With<DestroyEntity>();
        }

        public override void Update() {
            for (var i = 0; i < Query.count; i++) {
                ref var e = ref Query.GetEntity(i);
                e.Destroy();
            }
        }
    }
    public class StartFrameSystemGroup : SystemsGroup {
        public StartFrameSystemGroup() : base() {
            name = nameof(StartFrameSystemGroup);
        }
    }

    public class EndFrameSystemGroup : SystemsGroup {
        public EndFrameSystemGroup() : base() {
            name = nameof(EndFrameSystemGroup);
        }
    }
    
    public interface ISwapComponent<TComponent, TSwap> {
        void Swap(ref Entity entity, ref TComponent component);
    }
    public partial class SwapComponentSystem : UpdateSystem {
        private int tcomponent;
        private int swap;
        private EntityQuery Query;
        
        protected override void OnCreate() {
            Query = world.GetQuery().With(tcomponent).Without(swap);
        }

        public override void Update() {
            
        }
    }
}
