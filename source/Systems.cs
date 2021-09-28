using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public class Systems
    {
        public bool Alive;
        private readonly GrowList<DestroySystem> destroySystemsList = new GrowList<DestroySystem>(4);
        public int id;
        private readonly GrowList<InitSystem> initSystemsList = new GrowList<InitSystem>(4);
        private readonly TypeMap<int, List<IOnAdd>> onAddSystems = new TypeMap<int, List<IOnAdd>>(4);
        private readonly TypeMap<int, List<IOnRemove>> onRemoveSystems = new TypeMap<int, List<IOnRemove>>(4);
        public GrowList<UpdateSystem> updateSystemsList = new GrowList<UpdateSystem>(16);
        private int updateSystemsCount;
        private readonly World world;
        private ISystemListener systemBigListener;
        private bool debugMode;
        public Systems(World world)
        {
            this.world = world;
            world.AddSystems(this);
        }

        public void Init()
        {
            for (var i = 0; i < initSystemsList.Count; i++)
                initSystemsList[i].Execute();
            Alive = true;
        }

        internal void SetListener(ISystemListener listener)
        {
            systemBigListener = listener;
            debugMode = systemBigListener != null;
        }

        public Systems Add(UpdateSystem system)
        {
            updateSystemsList.Add(system);
            system.Init(world.Entities, world);
            InjectEntityTypesToSystem(system);
            system.Update();
            updateSystemsCount++;
            return this;
        }

        public Systems Add(InitSystem system)
        {
            initSystemsList.Add(system);
            system.Init(world.Entities, world);
            return this;
        }

        public Systems Add(DestroySystem system)
        {
            destroySystemsList.Add(system);
            system.Init(world.Entities, world);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Systems AddReactive(IReactive system)
        {
            InjectEntityTypesToSystem(system);
            if (system is IOnAdd add)
            {
                var triggerType = add.TriggerType;
                if (!onAddSystems.HasKey(triggerType))
                    onAddSystems.Add(triggerType, new List<IOnAdd>());
                onAddSystems[triggerType].Add(add);
                add.Init(world.Entities, world);
                add.Execute();
            }
            else if (system is IOnRemove remove)
            {
                var triggerType = remove.TriggerType;
                if (!onRemoveSystems.HasKey(triggerType))
                    onRemoveSystems.Add(triggerType, new List<IOnRemove>());
                onRemoveSystems[triggerType].Add(remove);
                remove.Init(world.Entities, world);
                remove.Execute();
            }

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAdd(int type)
        {
            if (!Alive) return;
            ;
            if (onAddSystems.HasKey(type))
                foreach (var system in onAddSystems[type])
                    system.Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemove(int type)
        {
            if (!Alive) return;
            if (onRemoveSystems.HasKey(type))
                foreach (var system in onRemoveSystems[type])
                    system.Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemove<T>()
        {
            if (!Alive) return;
            ;
            if (onRemoveSystems.HasKey(ComponentType<T>.ID))
                foreach (var system in onRemoveSystems[ComponentType<T>.ID])
                    system.Execute();
        }

        internal void Kill()
        {
            Alive = false;
            for (var i = 0; i < destroySystemsList.Count; i++)
                destroySystemsList[i].Execute();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnUpdate()
        {
            if (!Alive) return;
            if (debugMode)
            {
                systemBigListener?.StartCheck();
                for (var i = 0; i < updateSystemsCount; i++)
                {
                    systemBigListener?.StartCheck(i);
                    updateSystemsList.Items[i].Update();
                    systemBigListener?.StopCheck(i);
                }
                systemBigListener?.StopCheck();
            }
            else
            {
                for (var i = 0; i < updateSystemsCount; i++)
                    updateSystemsList.Items[i].Update();
            }
        }

        private void InjectEntityTypesToSystem(object system)
        {
            var systemType = system.GetType();
            var entityType = typeof(EntityType);
            var fields = systemType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
                if (f.FieldType.IsSubclassOf(entityType))
                    f.SetValue(system, world.Entities.GetEntityType(f.FieldType));
            
        }
    }

    internal interface IOnAdd : IReactive
    {
        void Execute();
    }

    internal interface IOnRemove : IReactive
    {
        void Execute();
    }

    public interface IReactive
    {
        int TriggerType { get; }
        void Init(Entities entities, World world);
    }

    public abstract class OnAdd<A> : IOnAdd
    {
        protected Entities entities;
        protected World world;
        public int TriggerType => ComponentType<A>.ID;

        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute();
    }

    public abstract class OnRemove<A> : IOnRemove
    {
        protected Entities entities;
        protected World world;
        public int TriggerType => ComponentType<A>.ID;

        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute();
    }

    public abstract class InitSystem
    {
        protected Entities entities;
        protected World world;

        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute();
    }

    public abstract class DestroySystem
    {
        protected Entities entities;
        protected World world;

        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }

        public abstract void Execute();
    }

    public abstract class UpdateSystem
    {
        protected Entities entities;
        protected World world;

        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Update();
    }

    public interface ISystemListener
    {
        void StartCheck();
        void StopCheck();
        void StartCheck(int index);
        void StopCheck(int index);
    }
}