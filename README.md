[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

![EZS_LOGO_22](https://user-images.githubusercontent.com/37613162/113684924-62cd8e80-96ce-11eb-8069-6923d4972dd1.png)
# EZS

C# Entity Component System (ECS) based on struct components

Inspired by [Entities](https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/index.html)

# How install:
Just copy source folder in assets folder of your game

# How it works:

Init World and Systems
```C#
var world = new World();
MonoConverter.Init(world); // Turn on unity integration
var systems = new Systems(world);
systems .Add(new PlayerMoveSystem())  //Add update/init/destroy system
        .AddReactive(new HealSystem())  //Add reactive system
        .AddReactive(new DamageSystem()); //Add reactive system
        
//You can use multiple Systems objects with one world
var systemsFixed = new Systems(world);
var systemsLate = new Systems(world);
#if UNITY_EDITOR
var debug = new DebugInfo(world); // Turn on debug
#endif
systems.Init();
systemsFixed.Init();
systemsLate.Init();

```
Call update of systems in your update loop
```C#
private void Update()
{
    systems.OnUpdate();
}
private void FixedUpdate()
{
    systemsFixed.OnUpdate();
}
private void LateUpdate()
{
    systemsLate.OnUpdate();
}
```


Work with entities
```C#
//create new entity
var entity = world.CreateEntity()

//add component
entity.Add(new Health{ value = 100 });

//check is entity has type of component
entity.Has<Health>()

//get component
ref var health = ref entity.Get<Health>()

//remove component
entity.Remove<Health>()

//check is entity alive or destroyed
entity.IsNull();

//destroy entity
entity.Destroy();
```
# Components:
Components are just struct or class with public fields\
(better use structs, most likely in the future framework will support only structs)
Examples:
```C#
[EcsComponent] // attribute for visual debugging and attachment components to entity from inspector
public struct Health 
{ 
    public int value;
}
```
# Systems : 

1. Update systems
```C#
//Type of systems that will execute every time when you call systems.OnUpdate();
//Update systems must be partial, because the framework uses a source generator now.
public partial class UpdateExampleSystem : UpdateSystem 
{
        public override void Update() 
        {
                entities.Each((Entity entity, Heal heal, Health health) => 
                {
                        //some logic
                });
        }
}
```

2. Reactive systems 
```C#
//Call system when component added to some entity
public class DamageSystem : OnAdd<Damaged> 
{
        public override void Execute(in Enitity entity) 
        {
                //some logic
        }
}
```
```C#
//Call system when component removed from some entity
public class OnRemoveSystem : OnRemove<SomeComponent> 
{
        public override void Execute(in Enitity entity) 
        {
                //some logic
        }
}
```

3. Init systems
```C#
//Call system at start of world live
public class InitExampleSystem : InitSystem
{
        public override void Execute() 
        {
                entities.Each((Entity entity, ref Health heath, ref Damaged damage) => 
                {
                    //some logic
                });
        }
}
```

4. Destroy systems
```C#
//Call system during the destruction of the world
public class DestroyExampleSystem : DestroySystem
{
        public override void Execute() 
        {
                entities.Each((Health heath, Damaged damage) => 
                {
                        //some logic
                });
        }
}
```
# System Loops: Each, Jobs, Without:
1. entities.Each((..........)=>{....});
```C#
//execute logic for each entity that has the components specified in it

public partial class SingleThreadExampleUpdateSystem : UpdateSystem 
{
        public override void Update() 
        {
                entities.Each((Rigidbody rb, BoxCollider box) => 
                {
                        //some logic
                });
        }
}

```
2. Jobs
```C#

public partial class MultiThreadExampleUpdateSystem  : UpdateSystem 
{
        private EntityQuery query;
        private Pool<Data> pool;
        protected override void OnCreate() {
                query = world.GetQuery().With<Data>();
                pool = world.GetPool<Data>();
        }
        public override void Update() 
        {
                var job = new TestJob
                {
                        nativeQuery = query.AsNative(),
                        nativePool = pool.AsNative()
                };
                job.Schedule(query.Count, 1).Complete();
        }
        [BurstCompile]
        struct TestJob : IJobParallelFor
        {
                public NativeQuery nativeQuery;
                public NativePool<Data> nativePool;
                public void Execute(int index)
                {
                        //when we work with entities from Jobs, entities representing as integer (Int32)
                        var entity = nativeQuery.GetEntity(index);
                        ref var data = ref nativePool.Get(entity);
                }
        }
}

```
3. You can filter entities not only by including components, but also by excluding
```C#
public partial class WithoutSystemExample  : UpdateSystem 
{
        public override void Update() 
        {
                //ref keyword if you use struct as component
                entities.Without<UnActive, PlayerTag>().EachThreaded((Position pos, RayCast ray, Impact impact, CanReflect reflect, BossTag tag) => 
                {
                        //some logic
                });
                
                entities.Without<UnActive, PlayerTag>().Each((Rigidbody rb, BoxCollider box) => 
                {
                        //some logic
                });
        }
}
```
4. WithOwner
```C#
public partial class AbilityExampleSystem  : UpdateSystem 
{
        public override void Update() 
        {
                entities.Each((Bullet bullet, CollisionEvent collision, Owner owner) => 
                {
                        // WithOwner take as parameter id of owner entity
                        entities.WithOwner(owner.id).Each((ExplosionOnCollisionAbility ability) => 
                        {
                                //some logic
                        });
                });
        }
}
```
# How systems filter entities:
Each entity has its own archetype. As many components as an entity has, there are as many components in an archetype. When we add/remove component form entity or change it's owner, the entity changes its archetype and marking as dirty. After execution of system it will be placed in matching queries, and removed from that are not matching.

```C#
var query = world.GetQuery().With<EnemyTag>().Without<PlayerTag>();
var entity = world.CreateEntity(); // empty archetype
entity.Add(new EnemyTag()) // archetype(EnemyTag). Our entity match with our query, after system executin it will be added to our query.
entity.Add(new PlayerTag()) // archetype(EnemyTag,PlayerTag). Now our entity not matching our query. and entity won't be added to it. 

```

# Unity Integration:
Entity Wrapper

![image](https://user-images.githubusercontent.com/37613162/168055056-b42cb0d8-a9f5-44e6-bb6d-2f351bdd117c.png)

Systems

![image](https://user-images.githubusercontent.com/37613162/168055250-086ee037-642e-4c0f-a366-2218aeacb189.png)

# Example project
[Turn Base Game](https://github.com/AlexWargon/TurnBasedGameEcs)
