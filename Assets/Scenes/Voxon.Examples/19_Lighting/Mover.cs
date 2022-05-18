using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
	Vector3 initial_position;
    // Start is called before the first frame update
    void Start()
    {
		initial_position = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
		if (Voxon.Input.GetKey("Left"))
		{
			transform.position += new Vector3(0.1f, 0, 0);
		}

		if (Voxon.Input.GetKey("Right"))
		{
			transform.position -= new Vector3(0.1f, 0, 0);
		}

		if (Voxon.Input.GetKey("Up"))
		{
			transform.position += new Vector3(0, 0, 00.1f);
		}

		if (Voxon.Input.GetKey("Down"))
		{
			transform.position -= new Vector3(0, 0, 00.1f);
		}

		if (Voxon.Input.GetKey("RotLeft"))
		{
			transform.position += new Vector3(0, 0.1f, 0);
		}

		if (Voxon.Input.GetKey("RotRight"))
		{
			transform.position -= new Vector3(0, 0.1f, 0);
		}

		if (Voxon.Input.GetKey("ToggleComponent"))
		{
			transform.position = initial_position;
		}
	}
}
