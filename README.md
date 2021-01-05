# EZS
C# Entity Component System (ECS) based on struct components

Inspired by LeoECS https://github.com/Leopotam/ecs and UnityECS


# How it works:

Init World and Systems
```
world = new World();
systems = new Systems(world);
systems .Add(new PlayerMoveSystem())
        .AddReactive(new HealSystem())
        .AddReactive(new DamageSystem());
systems.Init();
```
Call update of systems in your update loop
```
private void GameUpdateLoop(){
    systems.OnUpdate();
}
```
Work with entities
```
//create new entity
var entity = world.CreateEntity()

//add component
entity.Add(new Health{ value = 100 });

// check is entity has type of component
// it's return bool so you can use it at if statement or other logic on bool
entity.Has<Health>()

// get component
entity.Get<Health>()

//remove component
entity.Remove<Health>()
```
# Components:
Use component only with some cind of public data.
Examples:
```
public struct Health { 
    public int value;
}
public struct TransformRef{
    public Transform value;
}
```
# You can use two cinds of systems : 

# 1. Update systems
```
public class UpdateExampleSystem : UpdateSystem {
    public override void Update() {
        Entities.Each((ref Entity entity, ref Heal heal, ref Health health) => {
            health.value += heal.value;
            heal.value--;
            if(heal.value <= 0)
                entity.Remove<Heal>();
        });
    }
}
```
# 2. Reactive systems
a) Call system when component added to some entity   
```
public class DamageSystem : OnAdd<Damaged> {
    public override void Execute() {
        Entities.Each((ref Health heath, ref Damaged damage) => {
            heath.value -= damage.value;
        });
    }
}
```

b) Call system when component removed from some entity
```
public class OnRemoveSystem : OnRemove<SomeComponent> {
    public override void Execute() {
        //some logic
    }
}
```
