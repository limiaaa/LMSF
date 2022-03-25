using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestClass
{
    public int id;
    public float age;
}
public class LinqExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SelectList()
    {
        Debug.Log("Start Linq***************************************************");
        List<TestClass> test1 = new List<TestClass>();
        List<TestClass> test2 = new List<TestClass>();
        TestClass t1 = new TestClass();
        t1.id = 1;
        t1.age = 10;
        TestClass t2 = new TestClass();
        t2.id = 2;
        t2.age = 20;
        TestClass t3 = new TestClass();
        t3.id = 3;
        t3.age = 30;
        test1.Add(t1);
        test1.Add(t2);
        test1.Add(t3);

        test2 = (from item in test1 where item.age > 10 select item).ToList();
        Debug.Log("End Linq***************************************************");
    }
}
