using System;
using System.Runtime.CompilerServices;


namespace Wargon.ezs {
    public static class EntityExtension
    {
        public static void AddBoxed(this Entity entity, object component)
        {
            if (component == null)
            {
                UnityEngine.Debug.LogError($"Try add null component on entity {entity.id} " + Environment.NewLine +
                                           "Looks like some component on prefab was currupted :C");
                return;
            }
            var type = component.GetType();
            var typeId = ComponentType.GetID(type);
            ref var data = ref entity.GetEntityData();
            if (data.version != entity.version)
                throw new Exception("ENTITY NULL OR DESTROYED. Method: Entity.AddBoxed()");

            if (!data.Has(typeId)) {
                var pool = entity.World.GetPoolByID(typeId);
                pool.SetBoxed(component, entity.id);
                pool.Add(entity.id);
                data.archetype.TransferAdd(ref data, typeId);
                entity.World.OnAddComponent(typeId, in entity);
            }
        }
        /// <summary>
        /// Set new owner
        /// </summary>
        /// <param name="e"></param>
        /// <param name="newOwner"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetOwner(this ref Entity e, Entity newOwner) {
            if (newOwner.IsNULL()) {
                throw new Exception("You are trying set null reference entity as Owner");
            }
            ref var data = ref e.GetEntityData();
            var typeId = ComponentType<Owner>.ID;
            var pool = e.World.OwnerPool;
            var nativePool = e.World.OwnerNativePool;
            if (data.archetype.owner == -1) {
                pool.Set(new Owner{Value = newOwner}, e.id);
                pool.Add(e.id);
                nativePool.Set(new OwnerNative{id = newOwner.id}, e.id);
                nativePool.Add(e.id);
                data.archetype.TransferOwnerAdd(ref data, typeId, newOwner.id);
            }
            else {
                if(data.archetype.owner == newOwner.id) return;
                pool.Get(e.id).Value = newOwner;
                nativePool.Get(e.id).id = newOwner.id;
                data.archetype.TransferOwnerChange(ref data, newOwner.id);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasOwner(this ref Entity entity) {
            return !entity.IsNULL() && entity.GetEntityData().archetype.owner != -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetOwner(this ref Entity entity) {
            ref var world = ref entity.World;
            ref var data = ref entity.GetEntityData();
                if (data.archetype.owner != -1) return world.OwnerPool.Get(entity.id).Value;
            throw new Exception($"ENTITY {entity.id} HAS NO OWNER! Method: Entity.GetOwner()");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetOwnerDanger(this ref Entity entity) {
            ref var world = ref entity.World;
            return world.OwnerPool.Get(entity.id).Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity[] GetChildren(this Entity entity) {
            return entity.World.Entities.GetOwnerQuery(entity.id).GetEntityQuery();
        }

        public static Entity Copy(this ref Entity entity) {
            var w = entity.World;
            var e = w.CreateEntity();
            ref var data = ref entity.GetEntityData();
            foreach (var dataComponentType in data.archetype.mask) {
                var pool = w.GetPoolByID(dataComponentType);
                pool.Copy(entity.id, e.id);
                w.OnAddComponent(dataComponentType, in e);
            }

            e.GetEntityData().archetype = data.archetype;
            return e;
        }
        public static Entity Copy2(this ref Entity entity) {
            ref var data = ref entity.GetEntityData();
            var e = data.archetype.Copy(entity);
            return e;
        }
    }
}