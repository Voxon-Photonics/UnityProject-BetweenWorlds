using UnityEngine;
using UnityEngine.SceneManagement;
using Voxon;

public class HelpMessage_7 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VXProcess.add_log_line("-");
        VXProcess.add_log_line("Ensure 7_LevelLoading_1 and 7_LevelLoading_2 are in 'File\\Build Settings\\Scenes in Build'");
        VXProcess.add_log_line("Current Level:" + SceneManager.GetActiveScene().name);
        VXProcess.add_log_line("Loading next scene...");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
