using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StaticSpawner : MonoBehaviour
{
    public int objCount = 100;

    public float min_size = 0.1f;
    public float max_size = 3.0f;

    public Vector3 minVector = new Vector3();

    public Vector3 maxVector = new Vector3();

    public GameObject spawnable; 
    // Start is called before the first frame update
    void Start()
    {
        GameObject original =GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject go;
        for (int x = 0; x < objCount; ++x)
        {
            go = Instantiate(spawnable,
                new Vector3(
                    Random.Range(minVector.x, maxVector.x),
                    0.5f,
                    Random.Range(minVector.x, maxVector.x)),
                Quaternion.identity
            );
            float value = Random.Range(min_size, max_size);
            go.transform.localScale = new Vector3(value, value, value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
