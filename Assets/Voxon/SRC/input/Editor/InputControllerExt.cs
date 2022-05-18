using UnityEditor;
using UnityEngine;

namespace Voxon.Editor
{
    [CustomEditor(typeof(InputController))]
    public class InputControllerExt : UnityEditor.Editor {
        public static Vector2 ScrollPosition;
        public override void OnInspectorGUI()
        {
			GUILayout.MaxHeight(600); GUILayout.MinHeight(600);
			GUILayout.MaxWidth(800); GUILayout.MinWidth(800);

			ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(800), GUILayout.Height(600));

            base.OnInspectorGUI();
        
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New"))
            {
                InputController.Instance.keyboard.Clear();
                InputController.Instance.mouse.Clear();
                InputController.Instance.spacenav.Clear();
                InputController.Instance.j1Axis.Clear();
                InputController.Instance.j1Buttons.Clear();
                InputController.Instance.j2Axis.Clear();
                InputController.Instance.j2Buttons.Clear();
                InputController.Instance.j3Axis.Clear();
                InputController.Instance.j3Buttons.Clear();
                InputController.Instance.j4Axis.Clear();
                InputController.Instance.j4Buttons.Clear();
            }

            if (GUILayout.Button("Save"))
            {
                InputController.SaveData();
            }
            if (GUILayout.Button("Load"))
            {
                InputController.LoadData();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
    }
}
