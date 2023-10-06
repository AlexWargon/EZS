using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Wargon.ezs;
using Wargon.ezs.Unity;
using Object = UnityEngine.Object;

public class ObjectPool : MonoBehaviour {

    private const int POOL_MINIMUM_SIZE = 4;
    public const int POOL_INCREACE_SIZE = 32;
    private const int POOL_DEFAULT_SIZE = 128;
    public const int POOL_MAXIMUM_SIZE = 512;
    private static ObjectPool instance;
    public static Vector3 UnActivePos = new Vector3(-100000f, -100000f, 0);
    [SerializeField] private List<PoolContainer> poolContainers = new List<PoolContainer>();
    private readonly Dictionary<int, Queue<Entity>> entityPool = new Dictionary<int, Queue<Entity>>();

    public static void Clear() {
        Instance.entityPool.Clear();
        instance.poolContainers.Clear();
    }
    
    private static ObjectPool Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<ObjectPool>();

            if (instance == null)
            {
                instance = new GameObject().AddComponent<ObjectPool>();
                instance.transform.position = new Vector3(1000, 1000, 0);
                instance.name = "POOLS";
            }

            return instance;
        }
    }

    
    #region ENTITY POOL

    private int maxCallPerFrame = 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Queue<Entity> GetPool(int poolKey) {
        return instance.entityPool[poolKey];
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

            var poolHolder = new PoolContainer(new GameObject(),
                poolSize, prefab.name, poolKey, poolContainers.Count) {transform = {parent = transform}};
            poolContainers.Add(poolHolder);
            for (var i = 0; i < poolSize; i++)
            {
                var newEntity = Instantiate(prefab);
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
                var newEntity = Instantiate(prefab);
                newEntity.ConvertToEntity();
                ref var pooled = ref newEntity.Entity.Get<Pooled>();
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
    public static Entity ReuseEntity(MonoEntity prefab, Vector3 position)
    {
        return Instance.ReuseEntityNonStatic(prefab, position, Quaternion.identity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref Entity ReuseEntity(MonoEntity prefab, Vector3 position, Quaternion rotation)
    {
        return ref Instance.ReuseEntityNonStatic(prefab, position, rotation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Entity ReuseEntityNonStatic(MonoEntity prefab, Vector3 position, Quaternion rotation)
    {
        var poolKey = prefab.GetInstanceID();

        if (entityPool.ContainsKey(poolKey)) {
            var e = entityPool[poolKey].Dequeue();
            
            ref var poolObject = ref e.Get<Pooled>();
            
            poolObject.Reuse(position, rotation);
            if(entityPool[poolKey].Count < POOL_MINIMUM_SIZE)
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
                var newEntity = Instantiate(prefab);
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
            Destroy(poolContainers[entityPool[poolKey].Dequeue().Get<Pooled>().containerIndex].transform.gameObject);
            poolContainers.RemoveAt(entityPool[poolKey].Dequeue().Get<Pooled>().containerIndex);
            entityPool[poolKey].Clear();
            entityPool.Remove(poolKey);
        }
    }
    #endregion
}

public enum PoolObjectMode {
    GameObject,
    ChangePosition
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetActive(bool value)
    {
        IsActive = value;
        if (IsActive)
        {
            mono.Entity.Remove<Inactive>();
            mono.Entity.Set<PooledEvent>();
            CurrentLifeTime = LifeTime;
        }
        else
        {
            mono.Entity.Set<Inactive>();
            ObjectPool.GetPool(poolKey).Enqueue(mono.Entity);
        }

        if (Mode == PoolObjectMode.ChangePosition && !IsActive)
            transform.position = UnActivePosition;
        else
            mono.SetActive(IsActive);
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
            transformPure.scale = Vector3.one;
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