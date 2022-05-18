﻿using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voxon
{
    [InitializeOnLoad]
    public class EditorHandler: MonoBehaviour {

        static EditorHandler()
        {
            #if !UNITY_2018_4
                Debug.LogWarning("Voxon Plugin may not be compatible with this version of Unity. Please use 2018.4.X (LTS)");
            #endif
            
            EditorApplication.playModeStateChanged += PlayStateChange;
            try
            {
                if (FindObjectOfType<VXProcess>() == null)
                {
                    // Force creation of VXProcess
                    VXProcess a = VXProcess.Instance;
                }

                if(AssetDatabase.IsValidFolder("Assets/StreamingAssets") == false)
                {
                    Directory.CreateDirectory("Assets\\StreamingAssets");
                }

                if(InputController.GetKey("Quit") == 0)
                {
                    InputController.LoadData();
                }

                Debug.Assert(InputController.GetKey("Quit") != 0, "No 'Quit' keybinding found. Add to Input Manager");
            }
            catch(System.InvalidOperationException e)
            {
                Debug.Log(e.Message);
            }

            DefaultPlayerSettings();
        }

        [MenuItem("Voxon/Tools/Reimport Textures")]
        static void ReimportMaterials()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2d", null);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (texImporter != null) texImporter.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Texture2D tex = AssetDatabase.LoadAssetAtPath (path, typeof(Texture2D)) as Texture2D;
            }
            EditorUtility.DisplayDialog("Reimported Textures", "Textures Reimported.", "Ok");
        }

        [MenuItem("Voxon/Tools/Reimport Meshes")]
        static void ReimportMeshes()
        {
            string[] guids = AssetDatabase.FindAssets("t:Mesh", null);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var meshImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (meshImporter != null)
                {
                    meshImporter.isReadable = true;
                }

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            EditorUtility.DisplayDialog("Reimported Meshes", "Meshes Reimported.", "Ok");
        }

        [MenuItem("Voxon/Tools/Prebuild Mesh")]
        public static void PrebuildMesh()
        {
			// Prebuild 
			string scene_path = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
			string scene_directory = Path.GetDirectoryName(scene_path).Replace("Assets/","");
			string scene_filename = Path.GetFileNameWithoutExtension(scene_path);

			Debug.Log("Prebuilding Meshes for Scene");
            // string[] guids = AssetDatabase.FindAssets("t:Mesh", null);

            MeshRegister meshRegister = MeshRegister.Instance;
			meshRegister.Clear();

			/* All Meshes in Scene */
			MeshFilter[] meshes = FindObjectsOfType<MeshFilter>();

			Debug.Log($"{meshes.Length} mesh filters in scene");
			VXComponent vXBuffer;

			for (uint idx = 0; idx < meshes.Length; idx++)
			{
				Mesh sharedmesh = meshes[idx].sharedMesh;

				string path = UnityEditor.AssetDatabase.GetAssetPath(sharedmesh);

				// We don't rename default resources
				if (!path.StartsWith("Library"))
				{
					meshes[idx].name = path;

					vXBuffer = meshes[idx].gameObject.GetComponent<VXComponent>();
					if (vXBuffer)
					{
						vXBuffer.MeshPath = path;
					}
				}

				meshRegister.get_registed_mesh(ref sharedmesh);
			}

			SkinnedMeshRenderer[] skinned_meshes = FindObjectsOfType<SkinnedMeshRenderer>();

			Debug.Log($"{skinned_meshes.Length} skinned meshes in scene");

			for (uint idx = 0; idx < skinned_meshes.Length; idx++)
			{
				Mesh mesh = skinned_meshes[idx].sharedMesh;

				string path = UnityEditor.AssetDatabase.GetAssetPath(skinned_meshes[idx].sharedMesh);

				path += $":{mesh.name}";
				Debug.Log($"Path: { path }");

				// We don't rename default resources
				if (!path.StartsWith("Library"))
				{
					// skinned_meshes[idx].name = path;

					vXBuffer = skinned_meshes[idx].gameObject.GetComponent<VXComponent>();
					if (vXBuffer)
					{
						vXBuffer.MeshPath = path;
					}
				}

				meshRegister.get_registed_mesh(ref mesh);
			}

			// Create an instance of the type and serialize it.
			IFormatter formatter = new BinaryFormatter();

            if (!AssetDatabase.IsValidFolder($"{Application.dataPath}/StreamingAssets/{scene_directory}"))
            {
                Directory.CreateDirectory($"{Application.dataPath}/StreamingAssets/{scene_directory}");
            }

			string SerializedRegisterPath = $"{Application.dataPath}/StreamingAssets/{scene_directory}/{scene_filename}-Meshes.bin";
			// Debug.Log(SerializedRegisterPath);

			using (var s = new FileStream(SerializedRegisterPath, FileMode.Create))
            {
				try
				{
					// THIS APPROACH WONT WORK (We don't have points for de-serialisation).
					MeshData[] allData = meshRegister.PackMeshes();
					int mdCount = allData.Length;

					s.Write(System.BitConverter.GetBytes(mdCount), 0, sizeof(int));
					
					foreach (MeshData md in allData)
					{
						byte[] bytes = md.toByteArray();
						s.Write(bytes, 0, bytes.Length);
					}

				}
				catch (SerializationException e)
				{
					Debug.Log("Failed to serialize. Reason: " + e.Message);
					throw;
				}
				
            }

			UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
		}

		[MenuItem("Voxon/Tools/Prebuild Textures")]
		public static void PrebuildTextures()
		{
			// Prebuild 
			string scene_path = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
			string scene_directory = Path.GetDirectoryName(scene_path).Replace("Assets/", "");
			string scene_filename = Path.GetFileNameWithoutExtension(scene_path);

			Debug.Log("Prebuilding Meshes for Scene");
			string[] guids = AssetDatabase.FindAssets("t:Texture", null);

			TextureRegister textureRegister = TextureRegister.Instance;
			textureRegister.ClearRegister();
			/* All Meshes in Scene */
			MeshRenderer[] meshes = FindObjectsOfType<MeshRenderer>();

			Debug.Log($"{meshes.Length} mesh renderers in scene");

			for (uint idx = 0; idx < meshes.Length; idx++)
			{
				Material[] materials = meshes[idx].sharedMaterials;
				Material mat = meshes[idx].sharedMaterial;

				for (uint m_idx = 0; m_idx < materials.Length; m_idx++)
				{
					Texture2D tex = (Texture2D)materials[m_idx].mainTexture;

					if (tex == null) continue;
					

					string path = UnityEditor.AssetDatabase.GetAssetPath(tex);
					// Debug.Log($"{tex.name}, {path}");

					// We don't rename default resources
					if (!path.StartsWith("Library"))
					{
						// Debug.Log(tex.name);
						tex.name = path;
					}

					textureRegister.get_tile(ref tex);
				}
			}

			SkinnedMeshRenderer[] skinned_meshes = FindObjectsOfType<SkinnedMeshRenderer>();

			Debug.Log($"{skinned_meshes.Length} skinned meshes in scene");

			for (uint idx = 0; idx < skinned_meshes.Length; idx++)
			{
				Material[] materials = skinned_meshes[idx].sharedMaterials;

				for (uint m_idx = 0; m_idx < materials.Length; m_idx++)
				{
					Texture2D tex = (Texture2D)materials[m_idx].mainTexture;

					if (tex == null) continue;

					// Debug.Log(tex.name);

					string path = UnityEditor.AssetDatabase.GetAssetPath(tex);

					// We don't rename default resources
					if (!path.StartsWith("Library"))
					{
						tex.name = path;
					}

					textureRegister.get_tile(ref tex);
				}
			}

			/* All Meshes in project 
			foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == "") continue;

                var t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));

				// We don't rename default resources
				if (!path.StartsWith("Library"))
				{
					t.name = path;
				}

                meshRegister.get_registed_mesh(ref t);
            }
			*/

			// Create an instance of the type and serialize it.
			IFormatter formatter = new BinaryFormatter();


			if (!AssetDatabase.IsValidFolder($"{Application.dataPath}/StreamingAssets/{scene_directory}"))
			{
				Directory.CreateDirectory($"{Application.dataPath}/StreamingAssets/{scene_directory}");
			}

			string SerializedRegisterPath = $"{Application.dataPath}/StreamingAssets/{scene_directory}/{scene_filename}-Textures.bin";
			// Debug.Log(SerializedRegisterPath);

			using (var s = new FileStream(SerializedRegisterPath, FileMode.Create))
			{
				try
				{
					TextureData[] allData = textureRegister.PackMeshes();
					int mdCount = allData.Length;

					s.Write(System.BitConverter.GetBytes(mdCount), 0, sizeof(int));

					foreach (TextureData md in allData)
					{
						byte[] bytes = md.toByteArray();
						s.Write(bytes, 0, bytes.Length);
					}

				}
				catch (SerializationException e)
				{
					Debug.Log("Failed to serialize. Reason: " + e.Message);
					throw;
				}

			}


		}



		private static void PlayStateChange(PlayModeStateChange state)
        {
            // Handle Editor play states (block Play when Input disabled / close VX when Play stopped)
            if (state != PlayModeStateChange.ExitingPlayMode || VXProcess.Runtime == null) return;
        
            Debug.Log("Editor Play Stopping : Shutting down VX1 Simulator");
            VXProcess.Runtime.Shutdown();
        }

        private static void DefaultPlayerSettings()
        {
#if UNITY_2017
		PlayerSettings.displayResolutionDialog = UnityEditor.ResolutionDialogSetting.Disabled;
		PlayerSettings.defaultIsFullScreen = true;
#else
            PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
#endif

            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
            PlayerSettings.allowFullscreenSwitch = false;        
            PlayerSettings.defaultScreenHeight = 480;
            PlayerSettings.defaultScreenWidth = 640;
            PlayerSettings.forceSingleInstance = true;
            PlayerSettings.resizableWindow = false;
            PlayerSettings.runInBackground = true;
            PlayerSettings.usePlayerLog = true;
            PlayerSettings.visibleInBackground = true;
        }
    }
}
