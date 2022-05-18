using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Voxon.Examples;

public class ScreenDetails : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VXProcess.add_log_line("Between Worlds : Moving between dimensions");
        VXProcess.add_log_line("2D->3D Demo by Ben Weatherall");
        VXProcess.add_log_line("");
        VXProcess.add_log_line("Controls:");
        VXProcess.add_log_line("    Left Arrow: Reduce Gravity");
        VXProcess.add_log_line("    Right Arrow: Increase Gravity");
        VXProcess.add_log_line("    Up Arrow: Increase Spawn Rate");
        VXProcess.add_log_line("    Down Arrow: Reduced Spawn Rate");
        VXProcess.add_log_line("    Space: Reset Level");
        VXProcess.add_log_line("Gravity (m/s): "+ Physics.gravity.y + "    Spawn Per Second: " + ((1 / Time.deltaTime) / ObjectSpawner.frameSpawn));
    }
}
