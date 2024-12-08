using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Wargon.ezs;
using Wargon.ezs.Unity;
using Object = UnityEngine.Object;

public class ObjectPool {

    private const int POOL_MINIMUM_SIZE = 4;
    public const int POOL_INCREACE_SIZE = 32;
    private const int POOL_DEFAULT_SIZE = 32;
    public const int POOL_MAXIMUM_SIZE = 512;
    
    public static Vector3 UnActivePos = new Vector3(-100000f, -100000f, 0);
    private readonly List<PoolContainer> poolContainers = new List<PoolContainer>();
    private readonly Dictionary<int, Queue<Entity>> entityPool = new Dictionary<int, Queue<Entity>>();
    public List<MonoEntity> poolObj;
    private GameObject root;
    private static ObjectPool instance;
    private static ObjectPool Instance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (instance != null) return instance;
            instance = new ObjectPool {
                root = new GameObject("POOLS_ROOT") {
                    transform = {
                        position = new Vector3(1000, 1000, 0)
                    }
                }
            };
            // instance = FindObjectOfType<ObjectPool>();
            // if (instance == null && !clearing)
            // {
            //     instance = new GameObject().AddComponent<ObjectPool>();
            //     instance.transform.position = new Vector3(1000, 1000, 0);
            //     instance.name = "POOLS";
            // }
            return instance;
        }
    }
    
    #region ENTITY POOL

    public static void Clear() {
        if(instance is null) return;
        Instance.entityPool.Clear();
        Instance.poolContainers.Clear();
        instance = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Queue<Entity> GetPool(int poolKey) {
        return Instance.entityPool[poolKey];
    }

    private World _world;
    private Pool<Lifetime> lifeTimePool;
    private Pool<Pooled> pooledPool;
    public static void Create(World world) {
        Instance._world = world;
        instance.lifeTimePool = world.GetPool<Lifetime>();
        instance.pooledPool = world.GetPool<Pooled>();
    }

    private PoolCommand Command;
    private const int MAX_INSTATIATE_PER_FRAME = 64;
    private int instatiatePerFrameCounter = 0;
    private ref Entity ReuseEntityAsyncInternal(MonoEntity prefab, Vector3 position, Quaternion rotation) {
        var poolKey = prefab.GetInstanceID();
        if (entityPool.ContainsKey(poolKey)) {
            var e = entityPool[poolKey].Dequeue();
            var command = new PoolCommand.Command { entity = e, prefab = poolKey, pos = position, rotation = rotation, CommandType = PoolCommand.CommandType.Pool};
            Command.Buffer.Add(command);
            ref var poolObject = ref e.GetRef<Pooled>();
            poolObject.Reuse(position, rotation);
            if(entityPool[poolKey].Count < 0)
                AddPoolSizeAsync(prefab, 16, poolObject.containerIndex);
            return ref Command.Buffer.Items[Command.Buffer.Count-1].entity;
        }
        CreateEntityPool(prefab, POOL_DEFAULT_SIZE);
        return ref ReuseEntityNonStatic(prefab, position, rotation);
    }
    private void AddPoolSizeAsync(MonoEntity prefab, int addPoolSize, int containerIndex)
    {
        var poolKey = prefab.GetInstanceID();

        if (entityPool.ContainsKey(poolKey))
        {
            
            var poolHolder = poolContainers[containerIndex];
            for (var i = 0; i < addPoolSize; i++)
            {
                var newEntity = Object.Instantiate(prefab);
                newEntity.ConvertToEntity();
                ref var newObject = ref pooledPool.Get(newEntity.id);
                newObject.containerIndex = containerIndex;
                newObject.poolKey = poolKey;
                newObject.transform = newObject.mono.transform;
                newObject.gameObject = newObject.mono.gameObject;
                newObject.SetParent(poolHolder.transform);
#if UNITY_EDITOR
                newObject.SetName($"[PoolObject] {prefab.name} ID:{newObject.mono.id.ToString()}");
                //Log.Show(Color.yellow, "Pool added");
#endif
                newObject.SetActive(false);
                poolHolder.Add();
                instatiatePerFrameCounter++;
            }
        }
    }
    private void ExecuteBuffer() {
        for (var i = 0; i < instatiatePerFrameCounter; i++) {
            ref var cmd = ref Command.Buffer.GetLastWithDecrement();
            ref var poolObject = ref pooledPool.Get(cmd.entity.id);
            poolObject.Reuse(cmd.pos, cmd.rotation);
        }

    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateEntityPool(MonoEntity prefab, int poolSize)
    {
        Instance.CreateEntityPoolNonStatic(prefab, poolSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateEntityPoolNonStatic(MonoEntity prefab, int poolSize)
    {
        var poolKey = prefab.GetInstanceID();

        if (!entityPool.ContainsKey(poolKey))
        {
            entityPool.Add(poolKey, new Queue<Entity>());

            var poolHolder = new PoolContainer(new GameObject(), poolSize, prefab.name, poolKey, poolContainers.Count)
            {
                transform = {parent = root.transform},
            };
            poolContainers.Add(poolHolder);
            for (var i = 0; i < poolSize; i++)
            {
                var newEntity = Object.Instantiate(prefab);
                newEntity.ConvertToEntity();
                ref var pooled = ref newEntity.Entity.Get<Pooled>();
                pooled.poolKey = poolKey;
                pooled.transform = pooled.mono.transform;
                pooled.gameObject = pooled.mono.gameObject;
                pooled.containerIndex = poolHolder.index;
                pooled.SetActive(false);
                pooled.SetParent(poolHolder.transform);
                pooled.SetName($"[PoolObject] {prefab.name} ID:{pooled.mono.id.ToString()}");
            }

            //print($"<color=yellow> POOL OF {poolSize} [{poolHolder.Name}] CREATED </color>");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateEntityPoolNonStatic(MonoEntity prefab, int poolSize, Transform parent)
    {
        var poolKey = prefab.GetInstanceID();

        if (!entityPool.ContainsKey(poolKey))
        {
            entityPool.Add(poolKey, new Queue<Entity>());

            var poolHolder = new PoolContainer(new GameObject(), poolSize, prefab.name, poolKey, poolContainers.Count);
            poolHolder.transform.SetParent(parent);
            poolContainers.Add(poolHolder);
            for (var i = 0; i < poolSize; i++)
            {
                var newEntity = Object.Instantiate(prefab);
                var e = newEntity.ConvertToEntity();
                ref var pooled = ref e.Get<Pooled>();
                pooled.poolKey = poolKey;
                pooled.transform = pooled.mono.transform;
                pooled.gameObject = pooled.mono.gameObject;
                pooled.containerIndex = poolHolder.index;
                pooled.SetActive(false);
                pooled.SetParent(poolHolder.transform);
#if UNITY_EDITOR
                pooled.SetName($"[PoolObject] {prefab.name} ID:{pooled.mono.id.ToString()}");
#endif
            }

            //print($"<color=yellow> POOL OF {poolSize} [{poolHolder.Name}] CREATED </color>");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Entity ReuseView(MonoEntity prefab, Vector3 position)
    {
        return Instance.ReuseEntityNonStatic(prefab, position, Quaternion.identity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Entity ReuseView(MonoEntity prefab, Vector3 position, Quaternion rotation)
    {
        return ref Instance.ReuseEntityNonStatic(prefab, position, rotation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Entity ReuseEntityNonStatic(MonoEntity prefab, Vector3 position, Quaternion rotation)
    {
        var poolKey = prefab.GetInstanceID();

        if (entityPool.ContainsKey(poolKey)) {
            var e = entityPool[poolKey].Dequeue();
            
            ref var poolObject = ref e.GetRef<Pooled>();
            poolObject.Reuse(position, rotation);
            if(entityPool[poolKey].Count < 2)
                AddPoolSize(prefab, 16, poolObject.containerIndex);
            return ref poolObject.mono.Entity;
        }

        CreateEntityPool(prefab, POOL_DEFAULT_SIZE);
        return ref ReuseEntityNonStatic(prefab, position, rotation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Entity ReuseEntity(MonoEntity prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instance.ReuseEntityNonStatic(prefab, position, rotation, parent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Entity ReuseEntityNonStatic(MonoEntity prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        var poolKey = prefab.GetInstanceID();

        if (entityPool.ContainsKey(poolKey))
        {
            ref var poolObject = ref entityPool[poolKey].Dequeue().Get<Pooled>();
            if(entityPool[poolKey].Count < POOL_MINIMUM_SIZE)
                AddPoolSize(prefab, 16, poolObject.containerIndex);
            poolObject.Reuse(position, rotation);

            return poolObject.mono.Entity;
        }

        CreateEntityPoolNonStatic(prefab, POOL_DEFAULT_SIZE, parent);
        return ReuseEntityNonStatic(prefab, position, rotation, parent);
    }

    private const int MAX_POOL_SIZE = 512;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddPoolSize(MonoEntity prefab, int addPoolSize, int containerIndex)
    {
        var poolKey = prefab.GetInstanceID();

        if (entityPool.ContainsKey(poolKey))
        {
            
            var poolHolder = poolContainers[containerIndex];
            for (var i = 0; i < addPoolSize; i++)
            {
                var newEntity = Object.Instantiate(prefab);
                newEntity.ConvertToEntity();
                ref var newObject = ref newEntity.Entity.Get<Pooled>();
                newObject.containerIndex = containerIndex;
                newObject.poolKey = poolKey;
                newObject.transform = newObject.mono.transform;
                newObject.gameObject = newObject.mono.gameObject;
                newObject.SetParent(poolHolder.transform);
#if UNITY_EDITOR
                newObject.SetName($"[PoolObject] {prefab.name} ID:{newObject.mono.id.ToString()}");
                //Log.Show(Color.yellow, "Pool added");
#endif
                newObject.SetActive(false);
                poolHolder.Add();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Destroy(MonoEntity prefab)
    {
        Instance.DestroyNonStatic(prefab);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyNonStatic(MonoEntity prefab)
    {
        var poolKey = prefab.GetInstanceID();
        if (Instance.entityPool.ContainsKey(poolKey))
        {
            Object.Destroy(poolContainers[entityPool[poolKey].Dequeue().Get<Pooled>().containerIndex].transform.gameObject);
            poolContainers.RemoveAt(entityPool[poolKey].Dequeue().Get<Pooled>().containerIndex);
            entityPool[poolKey].Clear();
            entityPool.Remove(poolKey);
        }
    }
    #endregion
}

public enum PoolObjectMode {
    GameObject,
    ChangePosition,
    Pure
}

public struct Lifetime {
    public float value;
}
[EcsComponent]
public struct Pooled {
    public PoolObjectMode Mode;
    public int poolKey;
    public int containerIndex;
    public float CurrentLifeTime;
    public bool IsActive;
    public float LifeTime;
    public MonoEntity mono;
    public GameObject gameObject;
    public Transform transform;
    private static readonly Vector2 UnActivePosition = new Vector2(-100000, -100000);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetName(string name)
    {
        gameObject.name = name;
    }

    public void SetActiveOnlyObject(bool value) {
        mono.gameObject.SetActive(value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetActive(bool value)
    {
        IsActive = value;
        if (IsActive)
        {
            mono.Entity.Remove<Inactive>();
            
            mono.Entity.Add<PooledEvent>();
        }
        else
        {
            mono.Entity.Add<Inactive>();
            mono.Entity.Get<Lifetime>().value = LifeTime;
            if(poolKey != 0)
                ObjectPool.GetPool(poolKey).Enqueue(mono.Entity);
        }

        if (Mode == PoolObjectMode.ChangePosition && !IsActive) {
            transform.position = UnActivePosition;
        }
        else if (Mode == PoolObjectMode.GameObject) {
            mono.SetActive(IsActive);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetActive(bool value, ref Lifetime lifetime)
    {
        IsActive = value;
        if (IsActive)
        {
            mono.Entity.Remove<Inactive>();
            mono.Entity.Add<PooledEvent>();
            lifetime.value = LifeTime;
        }
        else
        {
            mono.Entity.Add<Inactive>();
            ObjectPool.GetPool(poolKey).Enqueue(mono.Entity);
        }

        if (Mode == PoolObjectMode.ChangePosition && !IsActive)
            transform.position = UnActivePosition;
        else
            mono.gameObject.SetActive(IsActive);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reuse(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        SetActive(true);
        if (mono.Entity.Has<TransformComponent>())
        {
            ref var transformPure = ref mono.Entity.Get<TransformComponent>();
            transformPure.position = position;
            transformPure.rotation = rotation;
        }
    }
}

[Serializable]
public class PoolContainer {
    public string Name;
    [HideInInspector] public Transform transform;
    public int size;
    public int index;
    public int poolKey;
    
    public PoolContainer(GameObject gameObject, int size, string prefabName, int poolKey, int index)
    {
        transform = gameObject.transform;
        this.size = size;
        Name = prefabName;
        this.poolKey = poolKey;
        this.index = index;
        Set();
    }

    private void Set()
    {
        transform.name = $"[Pool] {Name} [KeyID:{poolKey}]  Size:{size}";
    }

    public void Add()
    {
        size++;
        transform.name = $"[Pool] {Name} [KeyID:{poolKey}]  Size:{size}";
    }
}



public class PoolCommand {
    public enum CommandType {
        Pool,
        Instatiate
    }
    public struct Command {
        public Entity entity;
        public int prefab;
        public Vector3 pos;
        public Quaternion rotation;
        public CommandType CommandType;
    }
    public DynamicArray<Command> Buffer;

    public PoolCommand() {
        Buffer = new DynamicArray<Command>(64);
    }
}
