using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    [Serializable]
    public class MeshRegister : Singleton<MeshRegister> {
		[FormerlySerializedAs("Register")][SerializeField]
		private Dictionary<string, RegisteredMesh> register;

		[FormerlySerializedAs("cshader_main")] public ComputeShader cshaderMain;
        public int kernelHandle;

        public static bool Active;

		public void Enable()
		{
			this.OnEnable();
		}

        private void OnEnable()
        {
            Init();

            IFormatter formatter = new BinaryFormatter();


			string scene_path = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
			string scene_directory = Path.GetDirectoryName(scene_path).Replace("Assets/","");
			string scene_filename = Path.GetFileNameWithoutExtension(scene_path);

			string meshRegisterPath = $"{Application.dataPath}/StreamingAssets/{scene_directory}/{scene_filename}-Meshes.bin";


			if (!File.Exists(meshRegisterPath))
			{
				Active = true;
				Debug.Log("No pre-generated meshes found");
				return;
			}
			// Debug.Log("Loading MeshRegistered");

			using (var s = new FileStream(meshRegisterPath, FileMode.Open))
			{
				try
				{
					// Prep for all meshes;
					byte[] md_count_count_buf = new byte[sizeof(int)];
					s.Read(md_count_count_buf, 0, sizeof(int));
					int md_count = BitConverter.ToInt32(md_count_count_buf, 0);

					MeshData loaded_mesh;

					byte[] md_size_buf, md_buffer;
					int packet_size;

					for (int i = 0; i < md_count; i++) {
						md_size_buf = new byte[sizeof(int)];
						s.Read(md_size_buf, 0, sizeof(int));

						/*
						{ // Debug
							int start = 0;
							int end = sizeof(int);
							System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
							for (int idx = start; idx < end; idx++)
							{
								hex.AppendFormat("{0:x2}", md_size_buf[idx]);
							}
							Debug.Log($"\tSizeBytes: \t{hex.ToString()}");
						}
						*/

						packet_size = System.BitConverter.ToInt32(md_size_buf, 0);
						// Debug.Log(packet_size);
						md_buffer = new byte[packet_size];
						s.Read(md_buffer, 0, packet_size);

						/*
						{ // Debug
							int start = 0;
							int end = packet_size;
							System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
							for (int idx = start; idx < end; idx++)
							{
								hex.AppendFormat("{0:x2}", md_buffer[idx]);
							}
							Debug.Log($"Packet_bytes: \t{hex.ToString()}");
						}
						*/

						loaded_mesh = MeshData.fromByteArray(md_buffer);
						register.Add(loaded_mesh.name, new RegisteredMesh(ref loaded_mesh));
					}
				}
				catch (SerializationException e)
				{
					Debug.Log("Failed to serialize. Reason: " + e.Message);
					throw;
				}
			}

            Active = true;

        }

        private void Init()
        {
            register = new Dictionary<string, RegisteredMesh>();

            if (!Resources.Load("VCS"))
                Debug.Log("Failed to load VCS");

            cshaderMain = (ComputeShader)Resources.Load("VCS");
            kernelHandle = cshaderMain.FindKernel("CSMain");
        }

		public RegisteredMesh get_registed_mesh(string mesh_name)
		{
			if (register.ContainsKey(mesh_name))
			{
				return register[(mesh_name)];
			}

			return null;
		}


		public RegisteredMesh get_registed_mesh(ref Mesh mesh)
        {
#if UNITY_EDITOR
			if(!mesh.name.StartsWith("Assets/"))
			{
				string path = UnityEditor.AssetDatabase.GetAssetPath(mesh);
				path += $":{mesh.name}";

				if (!path.StartsWith("Library"))
				{
					// Debug.LogWarning($"({mesh.name}){path} is not preprocessed!");
					mesh.name = path;
				}
				
			}
#endif
			if (register == null)
            {
				Debug.Log("Initialising Register");
                Init();
            }

            if (register.ContainsKey(mesh.name))
            {
				// Debug.Log($"Looking Up Mesh: {mesh.name}");
				RegisteredMesh rm = register[(mesh.name)];
                rm.Increment();

                return rm;
            }
            else
            {
				Debug.Log($"Building Mesh: {mesh.name}");
				var rm = new RegisteredMesh(ref mesh);

                register.Add(mesh.name, rm);

                return rm;
            }
        }

        public bool drop_mesh(ref Mesh mesh)
        {
            if (register == null || !register.ContainsKey(mesh.name)) return false;
        
            RegisteredMesh rt = register[mesh.name];
            rt.Decrement();

            /*
        if (!rt.isactive() && false)
        {
            register.Remove(mesh.name);
            rt.destroy();
        }
        */
			return true;
        }

        public int Length()
        {
            return register.Count;
        }

        public void Clear()
        {
            if (register == null)
                return;

            while (register.Count > 0)
            {
                RemoveRegister(register.ElementAt(0).Key);
            }
        }

		public string[] Keys()
		{
			return register.Keys.ToArray();
		}

        new void OnApplicationQuit()
        {
            Active = false;
            Clear();
            base.OnApplicationQuit();
        }

        new void OnDestroy()
        {
            Active = false;
            Clear();
            base.OnDestroy();
        }

        private void RemoveRegister(string meshName)
        {
            if (!register.ContainsKey(meshName)) return;
        
            RegisteredMesh rt = register[meshName];
            register.Remove(meshName);
            rt.Destroy();
        }

#if UNITY_EDITOR
        public MeshData[] PackMeshes()
        {
            var rMs = new MeshData[register.Count];
            var idx = 0;
            foreach(RegisteredMesh rm in register.Values)
            {
                rMs[idx] = rm.GetMeshData();
                idx++;
            }

			Debug.Log($"RM Count: {rMs.Length}");

            return rMs;
        }
#endif
    }
}
