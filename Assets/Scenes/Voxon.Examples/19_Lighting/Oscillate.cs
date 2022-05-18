using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 pos = transform.position;
		pos.x = (Mathf.Sin(Time.time) * 0.5f);
		transform.position = pos;

	}
}
