using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableComponents : MonoBehaviour
{
	Voxon.VXComponent[] components;

	public GameObject LitObject;
	public GameObject UnlitObject;
	public new Voxon.lightbuffer light;

	new public bool enabled = false;
    // Start is called before the first frame update
    void Start()
    {
		Toggle();
    }

	void Toggle()
	{
		UnlitObject.active = enabled;
		LitObject.active = !enabled;
		light.enabled = !enabled;
	}

    // Update is called once per frame
    void Update()
    {
		if (Voxon.Input.GetKeyDown("ToggleComponent"))
		{
			enabled = !enabled;
			Toggle();
		}
    }
}
