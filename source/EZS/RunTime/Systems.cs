﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs {
    public class Systems {
        private readonly GrowList<DestroySystem> destroySystemsList = new GrowList<DestroySystem>(4);
        private readonly GrowList<InitSystem> initSystemsList = new GrowList<InitSystem>(4);
        private readonly World world;

        public bool Alive;
        private bool debugMode;
        public int id;
        private TypeMap<int, List<IOnAdd>> onAddSystems = new TypeMap<int, List<IOnAdd>>(4);
        private TypeMap<int, List<IOnRemove>> onRemoveSystems = new TypeMap<int, List<IOnRemove>>(4);
        private ISystemListener systemBigListener;
        private int updateSystemsCount;
        public GrowList<UpdateSystem> updateSystemsList = new GrowList<UpdateSystem>(16);

        public Systems(World world) {
            this.world = world;
            world.AddSystems(this);
        }

        public void Init() {
            for (var i = 0; i < initSystemsList.Count; i++)
                initSystemsList[i].Execute();
            Alive = true;
        }

        public void SetListener(ISystemListener listener) {
            systemBigListener = listener;
            debugMode = systemBigListener != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add(UpdateSystem system) {
            updateSystemsList.Add(system);
            system.Init(world.Entities, world);
            system.ID = updateSystemsCount;
            system.Update();

            //Injector.InjectEntityType(system, world);
            updateSystemsCount++;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add(InitSystem system) {
            initSystemsList.Add(system);
            system.Init(world.Entities, world);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add(DestroySystem system) {
            destroySystemsList.Add(system);
            system.Init(world.Entities, world);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems Add(SystemsGroup systemsGroup) {
            var systems = systemsGroup.GetSystems();
            for (var i = 0; i < systemsGroup.Count; i++) Add(systems.Items[i]);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems AddReactive(IReactive system) {
            if (system is IOnAdd add) {
                var triggerType = add.TriggerType;
                if (!onAddSystems.HasKey(triggerType))
                    onAddSystems.Add(triggerType, new List<IOnAdd>());
                onAddSystems[triggerType].Add(add);
                add.Init(world.Entities, world);
            }
            else if (system is IOnRemove remove) {
                var triggerType = remove.TriggerType;
                if (!onRemoveSystems.HasKey(triggerType))
                    onRemoveSystems.Add(triggerType, new List<IOnRemove>());
                onRemoveSystems[triggerType].Add(remove);
                remove.Init(world.Entities, world);
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAdd(int type, in Entity entity) {
            if (!Alive) return;
            if (!onAddSystems.HasKey(type)) return;
            var cnt = onAddSystems[type].Count;
            for (var index = 0; index < cnt; index++) onAddSystems[type][index].Execute(in entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemove(int type, in Entity entity) {
            if (!Alive) return;
            if (!onRemoveSystems.HasKey(type)) return;
            for (var index = 0; index < onRemoveSystems[type].Count; index++) {
                var system = onRemoveSystems[type][index];
                system.Execute(in entity);
            }
        }

        internal void Kill() {
            Alive = false;
            for (var i = 0; i < destroySystemsList.Count; i++)
                destroySystemsList[i].Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUpdate() {
            if (!Alive) return;
            if (debugMode) {
                systemBigListener.StartCheck();
                for (var i = 0; i < updateSystemsCount; i++) {
                    systemBigListener.StartCheck(i);
                    updateSystemsList.Items[i].Update();
                    systemBigListener.StopCheck(i);
                }

                systemBigListener.StopCheck();
            }
            else
                for (var i = 0; i < updateSystemsCount; i++)
                    updateSystemsList.Items[i].Update();
        }
    }

    public class SystemsGroup {
        private readonly string name;
        private readonly GrowList<UpdateSystem> systems = new GrowList<UpdateSystem>(4);
        internal int Count;

        public SystemsGroup() {
            name = "NO_NAME";
        }

        public SystemsGroup(string name) {
            this.name = name;
        }

        public SystemsGroup Add(UpdateSystem system) {
            systems.Add(system);
            Count++;
            return this;
        }

        public GrowList<UpdateSystem> GetSystems() {
            return systems;
        }

        public string GetName() {
            return name;
        }
    }

    internal interface IOnAdd : IReactive {
        void Execute(in Entity entity);
    }

    internal interface IOnRemove : IReactive {
        void Execute(in Entity entity);
    }

    public interface IReactive {
        int TriggerType { get; }
        void Init(Entities entities, World world);
    }

    public abstract class OnAdd<A> : IOnAdd {
        protected Entities entities;
        protected World world;
        public int TriggerType => ComponentType<A>.ID;

        public void Init(Entities entities, World world) {
            this.entities = entities;
            this.world = world;
            OnInit();
        }

        public virtual void OnInit(){}
        public abstract void Execute(in Entity entity);
    }

    public abstract class OnRemove<A> : IOnRemove {
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

    public abstract class UpdateSystem {
        protected Entities entities;

        internal int ID;
        protected World world;

        public virtual void Init(Entities newEntities, World newWorld) {
            entities = newEntities;
            world = newWorld;
            OnInit();
        }

        public virtual void OnInit() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Update();
    }

    public interface ISystemListener {
        void StartCheck();
        void StopCheck();
        void StartCheck(int index);
        void StopCheck(int index);
    }

    public struct SubedEntityTypes {
        private GrowList<EntityType> items;

        public static SubedEntityTypes New() {
            SubedEntityTypes newObj;
            newObj.items = new GrowList<EntityType>(16);
            return newObj;
        }

        public void Add(EntityType entityType) {
            items.Add(entityType);
        }

        public void OnAdd(int id) {
            for (var i = 0; i < items.Items.Length; i++) items.Items[i].OnAddExclude(id);
        }

        public void OnRemoveInclude(int id) {
            for (var i = 0; i < items.Items.Length; i++) items.Items[i].OnRemoveInclude(id);
        }

        public void OnAddInclude(int id) {
            for (var i = 0; i < items.Items.Length; i++) items.Items[i].OnAddInclude(id);
        }

        public void OnRemoveExclude(int id) {
            for (var i = 0; i < items.Items.Length; i++) items.Items[i].OnRemoveExclude(id);
        }
    }
}