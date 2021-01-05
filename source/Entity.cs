using System;
using System.Runtime.CompilerServices;

namespace Wargon.ezs
{
    public partial struct Entity
    {
        public World World;
        public int id;
        public int Generation;
    }
    public partial struct Entity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>(A component)
        {
            if (Has<A>()) return;

            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            if (data.ComponentTypes.Length == data.ComponentsCount)
            {
                Array.Resize(ref data.ComponentTypes, data.ComponentsCount << 1);
            }
            var type = ComponentType<A>.ID;
            data.ComponentTypes[data.ComponentsCount] = type;
            data.ComponentsCount++;
            World.GetPool<A>().Set(component, id);
            World.OnAddComponent(this, ref data, type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref A Get<A>()
        {
            if (Has<A>())
                return ref World.GetPool<A>().Items[id];
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            if (data.ComponentTypes.Length == data.ComponentsCount)
            {
                Array.Resize(ref data.ComponentTypes, data.ComponentsCount << 1);
            }
            var type = ComponentType<A>.ID;
            data.ComponentTypes[data.ComponentsCount] = type;
            data.ComponentsCount++;
            var pool = World.GetPool<A>();
            pool.Set(default, id);
            World.OnAddComponent(this, ref data, type);

            return ref pool.Items[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<A>()
        {
            if (Has<A>()) return;
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            if (data.ComponentTypes.Length == data.ComponentsCount)
            {
                Array.Resize(ref data.ComponentTypes, data.ComponentsCount << 1);
            }
            var type = ComponentType<A>.ID;
            data.ComponentTypes[data.ComponentsCount] = type;
            data.ComponentsCount++;
            World.OnAddComponent(this, ref data, type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<A>()
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            var typeId = ComponentType<A>.ID;
            for (var i = 0; i < data.ComponentsCount; i++)
            {
                if (data.ComponentTypes[i] != typeId) continue;
                data.ComponentTypes[i] = data.ComponentTypes[data.ComponentsCount - 1];
                data.ComponentTypes[data.ComponentsCount - 1] = 0;
                data.ComponentsCount--;
                World.OnRemoveComponent(this, data, typeId);
                return;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveByTypeID(int index)
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            var typeId = data.ComponentTypes[index];
            for (var i = 0; i < data.ComponentsCount; i++)
            {
                if (data.ComponentTypes[i] != typeId) continue;
                data.ComponentTypes[i] = data.ComponentTypes[data.ComponentsCount - 1];
                data.ComponentTypes[data.ComponentsCount - 1] = 0;
                data.ComponentsCount--;
                World.OnRemoveComponent(this, data, typeId);
                return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetByTypeID(int typeID)
        {
            return World.GetPoolById(typeID);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<A>(A component)
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            var typeId = ComponentType<A>.ID;
            for (int i = 0, iMax = data.ComponentsCount; i < iMax; i++)
                if (data.ComponentTypes[i] == typeId)
                    return true;

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<A>()
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            var typeId = ComponentType<A>.ID;
            for (int i = 0, iMax = data.ComponentsCount; i < iMax; i++)
                if (data.ComponentTypes[i] == typeId)
                    return true;

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref EntityData GetEntityData()
        {
            return ref World.GetEntityData(id);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) return;
            for (var i = 0; i < data.ComponentsCount; i++)
            {
                World.ComponentPools[data.ComponentTypes[i]].Default(id);
                data.ComponentTypes[i] = 0;
            }
            data.ComponentsCount = 0;
            data.Generation++;
            World.OnDestroyEntity(this);
            Generation = -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetActive(bool value)
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            data.Active = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDead()
        {
            return World.GetEntityData(id).Generation != Generation;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsActive()
        {
            ref var data = ref World.GetEntityData(id);
            if (data.Generation != Generation) throw new Exception("ENTITY NULL OR DESTROYED");
            return data.Active;
        }
    }
    public struct EntityData
    {
        public int id;
        public short ComponentsCount;
        public int[] ComponentTypes;
        public int Generation;
        public bool Active;
    }
}
