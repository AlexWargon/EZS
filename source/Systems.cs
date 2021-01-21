using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public class Systems
    {
        public int id;
        private World world;
        private GrowList<InitSystem> initSystemsList = new GrowList<InitSystem>(4);
        private GrowList<UpdateSystem> updateSystemsList = new GrowList<UpdateSystem>(16);
        private GrowList<DestroySystem> destroySystemsList = new GrowList<DestroySystem>(4);
        private TypeMap<int, List<IOnAdd>> OnAddSystems = new TypeMap<int, List<IOnAdd>>(4);
        private TypeMap<int, List<IOnRemove>> OnRemoveSystems = new TypeMap<int, List<IOnRemove>>(4);
        private int updateSystemsCount;
        public bool Alive;
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
        public Systems Add(UpdateSystem system)
        {
            updateSystemsList.Add(system);
            system.Init(world.Entities, world);
            system.Update();
            updateSystemsCount++;
            //injector.InjectArchetype(system, world);
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
            if (system is IOnAdd add)
            {
                var triggerType = add.TriggerType;
                if (!OnAddSystems.HasKey(triggerType))
                    OnAddSystems.Add(triggerType, new List<IOnAdd>());
                OnAddSystems[triggerType].Add(add);
                add.Init(world.Entities, world);
                add.Execute();
            }
            else
            if (system is IOnRemove remove)
            {
                var triggerType = remove.TriggerType;
                if (!OnRemoveSystems.HasKey(triggerType))
                    OnRemoveSystems.Add(triggerType, new List<IOnRemove>());
                OnRemoveSystems[triggerType].Add(remove);
                remove.Init(world.Entities, world);
                remove.Execute();
            }
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnAdd(int type)
        {
            if(!Alive) return;;
            if (OnAddSystems.HasKey(type))
            {
                foreach (var system in OnAddSystems[type])
                {
                    system.Execute();
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemove(int type)
        {
            if(!Alive) return;;
            if (OnRemoveSystems.HasKey(type))
                foreach (var system in OnRemoveSystems[type])
                    system.Execute();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void OnRemove<T>()
        {
            if(!Alive) return;;
            if (OnRemoveSystems.HasKey(ComponentType<T>.ID))
                foreach (var system in OnRemoveSystems[ComponentType<T>.ID])
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
            if(!Alive) return;;
            for (var i = 0; i < updateSystemsCount; i++)
                updateSystemsList.Items[i].Update();
        }
    }
    public interface IUpdateSystem
    {
        void Update();
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
        void Init(Entities entities, World world);
        int TriggerType { get; }
    }
    public abstract class OnAdd<A> : IOnAdd
    {
        public int TriggerType => ComponentType<A>.ID;
        protected World world;
        protected Entities entities;
        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }
        public abstract void Execute();

    }
    public abstract class OnRemove<A> : IOnRemove
    {
        public int TriggerType => ComponentType<A>.ID;
        protected World world;
        protected Entities entities;
        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }
        public abstract void Execute();
    }
    public abstract class InitSystem
    {
        protected World world;
        protected Entities entities;
        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }
        public abstract void Execute();
    }
    public abstract class DestroySystem
    {
        protected World world;
        protected Entities entities;
        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }
        public abstract void Execute();
    }
    public abstract class UpdateSystem : IUpdateSystem
    {
        protected World world;
        protected Entities entities;
        public void Init(Entities entities, World world)
        {
            this.entities = entities;
            this.world = world;
        }
        public abstract void Update();
    }
    
}
