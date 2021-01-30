[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
# EZS
C# Entity Component System (ECS) based on struct components

Inspired by LeoECS https://github.com/Leopotam/ecs and UnityECS

# How install:
Just copy source folder in assets folder of your game

# How it works:

Init World and Systems
```C#
var world = new World();
var systems = new Systems(world);
systems .Add(new PlayerMoveSystem())  //Add update system
        .AddReactive(new HealSystem())  //Add reactive system
        .AddReactive(new DamageSystem()); //Add reactive system
        
//You can use multiple Systems objects with one world
var systemsFixed = new Systems(world);
var systemsLate = new Systems(world);

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
Components are just structs with public fields
Examples:
```C#
public struct Health 
{ 
    public int value;
}
public struct TransformRef
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
    public override void Execute() 
    {
        entities.Each((ref Health heath, ref Damaged damage) => 
        {
                //some logic
        });
    }
}
```
```C#
//Call system when component removed from some entity
public class OnRemoveSystem : OnRemove<SomeComponent> 
{
    public override void Execute() 
    {
        //some logic
    }
}
```

3. Init systems
```C#
//Call system at start of world live
public class DamageSystem : InitSystem
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

3. Destroy systems
```C#
//Call system during the destruction of the world
public class DamageSystem : DestroySystem
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

public class UpdateSystemSingleThreadExample : UpdateSystem 
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
//same think like entities.Each(()=>), but use all threads of CPU!!!
//p.s. it won't work with unity object types like Transform, GameObject, Rigidbody and others :C

public class UpdateSystemMultyThreadExample  : UpdateSystem 
{
    public override void Update() 
    {
        entities.EachThreaded((ref Position pos, ref RayCast ray, ref Impact impact, ref CanReflect reflect, ref BossTag tag) => 
        {
            //some logic
        });
    }
}

```
3. You can filter entities not only by including components, but also by exluding
```C#
public class WithoutSystemExample  : UpdateSystem 
{
    public override void Update() 
    {
        entities.Without<UnActive, PlayerTag>().EachThreaded((ref Position pos, ref RayCast ray, ref Impact impact, ref CanReflect reflect, ref BossTag tag) => 
        {
            //some logic
        });
        entities.Without<UnActive, PlayerTag>().Each((ref Rigidbody rb, ref BoxCollider box) => 
        {
            //some logic
        });
    }
}
```
