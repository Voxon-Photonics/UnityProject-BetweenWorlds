#if UNITY_EDITOR
using UnityEditor;

namespace Voxon
{
    public class ProcessManager : EditorWindow
    {
        private static ProcessManager _processManager;

        [MenuItem("Voxon/Process")]
        private static void Init()
        {
            _processManager = (ProcessManager)GetWindow(typeof(ProcessManager));
            // Unnecessary but it shuts up Unity's warnings
            _processManager.Show();
        }

        private void OnGUI()
        {
            Editor editor = Editor.CreateEditor(VXProcess.Instance);
            editor.OnInspectorGUI();
        }
    }
}
#endif