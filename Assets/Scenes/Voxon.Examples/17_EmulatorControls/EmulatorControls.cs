using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class EmulatorControls : MonoBehaviour
{
    public float VerticleAngle;
    public float HorizontalAngle;
    public float Distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        int horizontalDisplacement =    (Voxon.Input.GetKey("Right") ? -1 : 0) +
                                        (Voxon.Input.GetKey("Left") ? 1 : 0);
        if (horizontalDisplacement != 0)
        {
            HorizontalAngle = VXProcess.Runtime.GetEmulatorHorizontalAngle();
            HorizontalAngle += horizontalDisplacement *  Time.deltaTime;
            HorizontalAngle = VXProcess.Runtime.SetEmulatorHorizontalAngle(HorizontalAngle);
        }

        int verticalDisplacement =    (Voxon.Input.GetKey("Down") ? -1 : 0) +
                                      (Voxon.Input.GetKey("Up") ? 1 : 0);
        if (verticalDisplacement != 0)
        {
            VerticleAngle = VXProcess.Runtime.GetEmulatorVerticalAngle();
            VerticleAngle += verticalDisplacement *  Time.deltaTime;
            VerticleAngle = VXProcess.Runtime.SetEmulatorVerticalAngle(VerticleAngle);
        }
        
        int distanceDisplacement =(Voxon.Input.GetKey("In") ? -1 : 0) +
                                  (Voxon.Input.GetKey("Out") ? 1 : 0);
        
        if (distanceDisplacement != 0)
        {
            Distance = VXProcess.Runtime.GetEmulatorDistance();
            Distance += distanceDisplacement *  Time.deltaTime * 1000;
            Distance = VXProcess.Runtime.SetEmulatorDistance(Distance);
        }
    }
}
