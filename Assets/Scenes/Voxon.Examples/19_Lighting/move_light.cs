using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_light : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
		Vector3 pos = gameObject.transform.position;

		pos.x +=  Mathf.Sin(Time.fixedTime / 5) * 10;
		pos.y += 10 + (Mathf.Sin(Time.fixedTime / 5) + Mathf.Cos(Time.fixedTime))/2 * 10;
		pos.z += Mathf.Cos(Time.fixedTime / 5) * 10;

		gameObject.transform.position = pos;
	}
}
