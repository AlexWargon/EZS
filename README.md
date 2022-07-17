[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

![EZS_LOGO_22](https://user-images.githubusercontent.com/37613162/113684924-62cd8e80-96ce-11eb-8069-6923d4972dd1.png)
# EZS

C# Entity Component System (ECS) based on struct components

Inspired by [LeoECS](https://github.com/Leopotam/ecs) and [Entities](https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/index.html)

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
var debug = new DebugInfo(world); // Turn on debug
systems.Init();
systemsFixed.Init();
systemsLate.Init();

```
Call update of systems in your update loop
```C#
private void GameUpdateLoop()
{
    systems.OnUpdate();
}
private void GameFixedUpdateLoop()
{
    systemsFixed.OnUpdate();
}
private void GameLateUpdateLoop()
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
entity.Get<Health>()

//remove component
entity.Remove<Health>()

//check is entity alive or destroyed
entity.IsDead();

//destroy entity
entity.Destroy();
```
# Components:
Components are just structs or class with public fields
Examples:
```C#
[EcsComponent] // attribute for visual debugging and atachment components to entity from inspector
public struct Health 
{ 
    public int value;
}
[EcsComponent]
public class TransformRef
{
    public Transform value;
}
```
# Systems : 

1. Update systems
```C#
//Type of systems that will execute every time when you call systems.OnUpdate();
public class UpdateExampleSystem : UpdateSystem 
{
    public override void Update() 
    {
        entities.Each((ref Entity entity, ref Heal heal, ref Health health) => 
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
        entities.Each((ref Health heath, ref Damaged damage) => 
        {
            //some logic
        });
    }
}
```
# System Loops: Each, EachThreaded, Without:
1. entities.Each((..........)=>{....});
```C#
//execute logic for each entity that has the components specified in it

public class SingleThreadExampleUpdateSystem : UpdateSystem 
{
    public override void Update() 
    {
        entities.Each((ref Rigidbody rb, ref BoxCollider box) => 
        {
            //some logic
        });
    }
}

```
2. entities.EachThreaded((..........)=>{.....});
```C#
//same think like entities.Each(()=>), but use all threads of CPU.
//p.s. it won't work with unity object types like Transform, GameObject, Rigidbody and others :C

public class MultyThreadExampleUpdateSystem  : UpdateSystem 
{
    public override void Update() 
    {
        //ref keyword if you use struct as component
        entities.EachThreaded((ref Position pos, ref RayCast ray, ref Impact impact, ref CanReflect reflect, ref BossTag tag) => 
        {
            //some logic
        });
    }
}

```
3. You can filter entities not only by including components, but also by excluding
```C#
public class WithoutSystemExample  : UpdateSystem 
{
    public override void Update() 
    {
        //ref keyword if you use struct as component
        entities.Without<UnActive, PlayerTag>().EachThreaded((ref Position pos, ref RayCast ray, ref Impact impact, ref CanReflect reflect, ref BossTag tag) => 
        {
            //some logic
        });
        //ref keyword if you use struct as component
        entities.Without<UnActive, PlayerTag>().Each((Rigidbody rb, ref BoxCollider box) => 
        {
            //some logic
        });
    }
}
```

# Unity Integration:
Entity Wrapper

![image](https://user-images.githubusercontent.com/37613162/168055056-b42cb0d8-a9f5-44e6-bb6d-2f351bdd117c.png)

Systems

![image](https://user-images.githubusercontent.com/37613162/168055250-086ee037-642e-4c0f-a366-2218aeacb189.png)

# Example project
[Turn Base Game](https://github.com/AlexWargon/TurnBasedGameEcs)
