namespace Wargon.ezs.Delegates
{
      public delegate void gLambda<T1>(T1 t1);
      public delegate void gLambda<T1,T2>(T1 t1,T2 t2);
      public delegate void gLambda<T1,T2,T3>(T1 t1,T2 t2,T3 t3);
      public delegate void gLambda<T1,T2,T3,T4>(T1 t1,T2 t2,T3 t3,T4 t4);
      public delegate void gLambda<T1,T2,T3,T4,T5>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5);
      public delegate void gLambda<T1,T2,T3,T4,T5,T6>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6);
      public delegate void gLambda<T1,T2,T3,T4,T5,T6,T7>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7);
      public delegate void gLambda<T1,T2,T3,T4,T5,T6,T7,T8>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7,T8 t8);
      public delegate void gLambda<T1,T2,T3,T4,T5,T6,T7,T8,T9>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7,T8 t8,T9 t9);
      public delegate void gLambda<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10);
      public delegate void gLambda<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7,T8 t8,T9 t9,T10 t10,T11 t11);

}

namespace Wargon.ezs
{
    public static partial class EntitiesExtensions 
    {
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1>(this EntitiesEach entities, Delegates.gLambda<T1> callback) 
        where T1: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2>(this EntitiesEach entities, Delegates.gLambda<T1,T2> callback) 
        where T1: new()
        where T2: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4,T5> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4,T5,T6> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4,T5,T6,T7> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7,T8>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4,T5,T6,T7,T8> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        where T8: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7,T8,T9>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4,T5,T6,T7,T8,T9> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        where T8: new()
        where T9: new()
        {
            return entities;
        }


            
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(this EntitiesEach entities, Delegates.gLambda<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        where T8: new()
        where T9: new()
        where T10: new()
        {
            return entities;
        }


            
    }
}
            


namespace Wargon.ezs
{
    public static partial class EntitiesExtensions 
    {
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1>(this EntitiesEach entities,Delegates.gLambda<Entity,T1> callback) 
        where T1: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2> callback) 
        where T1: new()
        where T2: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4,T5> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4,T5,T6> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4,T5,T6,T7> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7,T8>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4,T5,T6,T7,T8> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        where T8: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7,T8,T9>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4,T5,T6,T7,T8,T9> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        where T8: new()
        where T9: new()
        {
            return entities;
        }
                        
        // ReSharper disable Unity.PerformanceAnalysis
        public static EntitiesEach Each<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(this EntitiesEach entities,Delegates.gLambda<Entity,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> callback) 
        where T1: new()
        where T2: new()
        where T3: new()
        where T4: new()
        where T5: new()
        where T6: new()
        where T7: new()
        where T8: new()
        where T9: new()
        where T10: new()
        {
            return entities;
        }
                        
    }
}
            
