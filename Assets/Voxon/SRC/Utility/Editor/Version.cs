﻿using UnityEditor;
using UnityEngine;

namespace Voxon
{
    public class VersionDisplay : EditorWindow
    {
        [MenuItem("Voxon/Version")]
        public static void Init()
        {
            VersionDisplay window = ScriptableObject.CreateInstance<VersionDisplay>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 75);
            window.ShowPopup();
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Voxon Unity Plugin Build Date: " + VXProcess.BuildDate);
            EditorGUILayout.LabelField("For the latest version visit: ");
            EditorGUILayout.LabelField("https://github.com/Voxon-Photonics/Content-Developers-Kit");
            if (GUILayout.Button("Ok")) this.Close();
        }
    }
}