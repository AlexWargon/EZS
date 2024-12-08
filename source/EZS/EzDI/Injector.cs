using System.Runtime.CompilerServices;

namespace Wargon.DI
{
    public static class Injector {
        private static DependencyContainer _dependencyContainerInstance;
        public static DependencyContainer GetOrCreate()
        {
            if(_dependencyContainerInstance == null) 
                _dependencyContainerInstance = new DependencyContainer();
            return _dependencyContainerInstance;
        }
        public static T New<T>() where T : class, new() => GetOrCreate().New<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Resolve<T>(T item) where T : class => GetOrCreate().Resolve(item);

        public static void ResolveObject(object item) => GetOrCreate().ResolveObject(item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAsSingle<T>(T toInject) where T : class => GetOrCreate().AddAsSingle(toInject);

        public static T GetAsSignle<T>() where T : class => GetOrCreate().GetSingle<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAsGlobal<T>(T toInject) where T : class => GetOrCreate().AddAsGlobal(toInject);
        public static void Dispose() => GetOrCreate().Dispose();
        public static void DisposeAll() => GetOrCreate().DisposeAll();
    }
}