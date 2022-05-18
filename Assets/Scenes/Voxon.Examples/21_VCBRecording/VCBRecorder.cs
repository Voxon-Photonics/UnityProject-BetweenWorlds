using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCBRecorder : MonoBehaviour
{
	public string filename;
	bool is_recording = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Voxon.Input.GetKeyDown("VCBRecord"))
		{
			if (!is_recording)
			{
				Debug.Log($"Recorder: {filename}");
				Voxon.VXProcess.Runtime.StartRecording(filename, 15);
			}
			else
			{
				Voxon.VXProcess.Runtime.EndRecording();
			}
			is_recording = !is_recording;
		}
    }
}
