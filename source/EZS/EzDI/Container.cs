using System;

namespace Wargon.DI
{
    public class Container
    {
        private readonly Type containerType;
        private readonly object dependency;
        private readonly DiType diType;

        public Container(Type type, object dependency, DiType diType)
        {
            containerType = type;
            this.dependency = dependency;
            this.diType = diType;
        }

        public object Get()
        {
            return diType switch
            {
                DiType.Single => dependency,
                DiType.Global => DependencyContainer.Globals[containerType],
                DiType.New => Activator.CreateInstance(containerType),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}