using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class SpaceNavigator : MonoBehaviour
{
    public float movement_speed = 1f;
    public float zoom_speed = 1f;
    private Vector3 original_size = Vector3.zero;
    private Vector3 original_pos = Vector3.zero;
    
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (original_size == Vector3.zero)
        {
            original_size = VXProcess.Instance.Camera.transform.localScale;
        }

        if (original_pos == Vector3.zero)
        {
            original_pos = VXProcess.Instance.Camera.transform.position;
        }
        
        if (Voxon.Input.GetSpaceNavButton("LeftButton"))
        {
            VXProcess.Instance.Camera.transform.localScale *= (1 + zoom_speed/10);
        }
        
        if(Voxon.Input.GetSpaceNavButton("RightButton"))
        {
            VXProcess.Instance.Camera.transform.localScale *= (1 - zoom_speed/10);
        }
        
        var position = VXProcess.Runtime.GetSpaceNavPosition();
        var rotation = VXProcess.Runtime.GetSpaceNavRotation();
        
        if (rotation != null)
        {
            var v3rot = new Vector3(0,rotation[2]/70,0);
            transform.Rotate(v3rot);    
        }
        
        var v3pos = transform.position;
        if (position != null)
        {
            v3pos.x += movement_speed*(position[0]/35.0f);
            v3pos.y += movement_speed*(position[2]/35.0f);
            v3pos.z -= movement_speed*(position[1]/35.0f);
            VXProcess.Instance.Camera.transform.position = v3pos;
        }

        if (Voxon.Input.GetSpaceNavButton("LeftButton") && Voxon.Input.GetSpaceNavButton("RightButton"))
        {
            VXProcess.Instance.Camera.transform.position = original_pos;
            VXProcess.Instance.Camera.transform.rotation = Quaternion.identity;
            VXProcess.Instance.Camera.transform.localScale = original_size;
        }
    }
}
