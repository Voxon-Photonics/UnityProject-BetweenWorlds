using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forward_kitty : MonoBehaviour
{
	public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
		Mesh m = gameObject.GetComponent<Mesh>();
	}

    // Update is called once per frame
    void Update()
    {
		transform.position = transform.position + (transform.right * Time.deltaTime * speed);
	}
}
