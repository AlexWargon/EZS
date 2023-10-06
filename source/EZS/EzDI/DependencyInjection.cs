using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DependencyInjection : MonoBehaviour
{
    // public Dependency1 dependency1;
    // public Dependency2 dependency2;
    // public Dependency3 Dependency3;
    //
    // public TestDI testDi;

    public void Awake()
    {
        // var ezDi = new EzDI();
        // ezDi.AddAsSingle(dependency1);
        // ezDi.AddAsGlobal(dependency2);
        // ezDi.AddAsSingle(Dependency3);
        // ezDi.Add<Dependency4>();
        //
        // ezDi.Bind(testDi);
        // Timer t = Timer.New();
        // t.Action(() =>
        //     {
        //         ezDi.Instatiate(testDi,Vector3.one, Quaternion.identity);
        //     })
        //     .WaitFor(0.1f)
        //     .Repeat()
        //     .Start();
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class InjectAttribute : Attribute
{
    public InjectAttribute()
    {
        
    }
}