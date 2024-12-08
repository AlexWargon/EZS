namespace Wargon.ezs.Unity {
    public partial class SyncTransformSystem2 : UpdateSystem {
        private Pool<TransformRef> transforms;
        private Pool<TransformComponent> transformPure;
        private EntityQuery query;
        protected override void OnCreate() {
            query = world.GetQuery().With<TransformRef>().With<TransformComponent>().Without<Inactive>().Without<StaticTag>();
            transforms = world.GetPool<TransformRef>();
            transformPure = world.GetPool<TransformComponent>();
        }

        public override void Update() {
            //Span<TransformComponent> spanT = new Span<TransformComponent>(transformPure.items);
            //Span<TransformRef> spanR = new Span<TransformRef>(transforms.items);
            
            for (var i = 0; i < query.Count; i++) {
                var index = query.GetEntityIndex(i);
                ref var transformRef = ref transforms.items[index];
                ref var transformComponent = ref transformPure.items[index];
                // transformComponent.right = transformComponent.rotation * UnityEngine.Vector3.right;
                // transformComponent.forward = transformComponent.rotation * UnityEngine.Vector3.forward;
                transformRef.value.localPosition = transformComponent.position;
                transformRef.value.rotation = transformComponent.rotation;
                transformRef.value.localScale = transformComponent.scale;
                
            }
        }
    }
}