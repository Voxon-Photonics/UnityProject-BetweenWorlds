using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnCat : MonoBehaviour
{
	public int cat_count;
	public GameObject kitty;
	GameObject[] cat_array;
	public float spawn_radius = 3;
	public float despawn_radius = 3;

	float y = -0.255f;

	// Start is called before the first frame update
	void Start()
    {
		cat_array = new GameObject[cat_count];
    }

    // Update is called once per frame
    void Update()
    {
        for(int idx = 0; idx < cat_array.Length; idx++)
		{
			if(cat_array[idx] == null)
			{
				MakeKitty(idx);
			}
			else if (Vector3.Distance(cat_array[idx].transform.position, transform.position) > despawn_radius)
			{
				Destroy(cat_array[idx]);
				cat_array[idx] = null;

				MakeKitty(idx);
			}
		}
    }

	void MakeKitty(int idx)
	{
		float angle = UnityEngine.Random.Range(0.0f, 1f) * (float)Math.PI * 2f;
		float x = (float)Math.Cos(angle) * spawn_radius;
		float z = (float)Math.Sin(angle) * spawn_radius;

		cat_array[idx] = GameObject.Instantiate(kitty);
		cat_array[idx].transform.localScale = new Vector3(
			UnityEngine.Random.Range(0.8f, 1.2f),
			UnityEngine.Random.Range(0.8f, 1.2f),
			UnityEngine.Random.Range(0.8f, 1.2f));
			
		cat_array[idx].transform.position = new Vector3(x, y, z);
		Vector3 dir = transform.position;
		dir.x += UnityEngine.Random.Range(-1f, 1f);
		dir.y = y;
		dir.z += UnityEngine.Random.Range(-1f, 1f);

		cat_array[idx].transform.LookAt(dir);
		cat_array[idx].transform.right = -cat_array[idx].transform.forward;

		float speed = UnityEngine.Random.Range(-2f, -1f);
		cat_array[idx].GetComponent<forward_kitty>().speed = speed;
		cat_array[idx].GetComponent<Animator>().SetFloat("Kitty_Speed", -speed);
	}
}
