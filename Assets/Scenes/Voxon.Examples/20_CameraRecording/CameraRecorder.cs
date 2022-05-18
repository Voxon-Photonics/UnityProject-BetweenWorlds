using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraAnimation))]
public class CameraRecorder : MonoBehaviour
{
	public string filename;
	bool is_recording = false;
	CameraAnimation camAn;

	// Start is called before the first frame update
	void Start()
    {
		camAn = GetComponent<CameraAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Voxon.Input.GetKeyDown("Cam_Record"))
		{
			if (!is_recording)
			{
				Debug.Log($"Camera Animation Recording: {filename}");
				camAn.recording_file = filename;
				camAn.SetFrame(0);
				camAn.BeginRecording();
			}
			else
			{
				Debug.Log($"Camera Animation Recording Stopped");
				camAn.StopRecording();
			}
			is_recording = !is_recording;
		}
	}
}
