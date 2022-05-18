// https://github.com/przemyslawzaworski
// Set plane position (Y=0.0)
// Collider is generated from vertex shader, using RWStructuredBuffer to transfer data.

using UnityEngine;

public struct depth
{
	public float value;         // Depth value at this point
	public int data_index;      // Data index where depth related value is stored
};

namespace Voxon
{
	/* 
	 * The light buffer class operates on 2 primary buffers.
	 * A data buffer that can contain up to a full volumes worth of voxels (it never should, but that's our max). 
	 *	This data is organised in an array of running indices and is drawn
	 * A depth buffer that stores the depth value for each pixel in the render texture.
	 *	Each new fragment tests itself against the depth buffer using it's x, y and if depth is less than active depth, it will use the existing x,y,z (if they exist), 
	 *	and update the data buffer with the fragments issue, before updating the depth buffer to the new values. Deeper values will be discarded.
	 */
	public class lightbuffer : MonoBehaviour, IDrawable
	{


		ComputeBuffer light_buffer;
		ComputeBuffer depth_buffer;
		ComputeBuffer index_buffer;

		ComputeShader clearShader;

		Shader lightShader;

		VXCamera activeCamera;
		Camera cam;
		RenderTexture rb;
		int voxels = 0;
		int pixels = 0;
		public int resolution = 512;

		poltex[] poltex_data;
		depth[] depth_data;
		int[] default_index = new int[] { 0 };
		int[] current_index = new int[] { 0 };

		public float LightDistance = 5;
		public float sigma = 0.001f;

		poltex[] poltex_buffer;
		int group_size = (1024 * 1024) / 24; // How many poltex per Megabyte
		int group_count = 0;

		static readonly int
			resolutionId = Shader.PropertyToID("_Resolution"),
			indexId = Shader.PropertyToID("_Index"),
			cameraId = Shader.PropertyToID("_ActiveCamera"),
			dataId = Shader.PropertyToID("_Data"),
			depthId = Shader.PropertyToID("_Depth");

		void OnEnable()
		{

		}

		void Start()
		{
			cam = GetComponent<Camera>();
			cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
			rb = cam.targetTexture;

			resolution = rb.width;

			if(resolution == 0)
			{
				Debug.LogError("Render Texture resolution is 0; stopping light system");
				return;
			}

			clearShader = (ComputeShader)Resources.Load("LightClear");

			// Voxel Setup
			// voxels = xz_resolution * xz_resolution * y_resolution; // Default to 200 Y resolution
			voxels = resolution * resolution;

			// Determine Groups based on group size and set up buffer
			poltex_buffer = new poltex[group_size];
			group_count = Mathf.CeilToInt(voxels / (float)group_size);

			// Debug.Log($"Groups: {group_count} - Last Group Remainder {voxels % group_size}");

			poltex_data = new poltex[voxels];

			int poltex_stride = 4 /*32 bit*/ * (3 /* Vector3 */ + 2 /* Vector2 */ + 1 /* int */);
			light_buffer = new ComputeBuffer(voxels, poltex_stride, ComputeBufferType.Default);

			// Depth Setup
			// pixels = xz_resolution * xz_resolution;
			pixels = voxels;

			depth_data = new depth[pixels];
			int pixel_stride = 4 /* int */ + 4 /* float */;
			depth_buffer = new ComputeBuffer(pixels, pixel_stride, ComputeBufferType.Default);

			// Index Setup
			int index_count = 1;
			int index_stride = 4;
			index_buffer = new ComputeBuffer(index_count, index_stride, ComputeBufferType.Default);

			lightShader = Shader.Find("Voxon/LightShader");

			Graphics.ClearRandomWriteTargets();
			Shader.SetGlobalBuffer(dataId, light_buffer);
			Shader.SetGlobalBuffer(depthId, depth_buffer);
			Shader.SetGlobalBuffer(indexId, index_buffer);

			index_buffer.SetData(default_index);
			Shader.SetGlobalInt(resolutionId, resolution);
			
			Graphics.SetRandomWriteTarget(1, light_buffer, false);
			Graphics.SetRandomWriteTarget(2, depth_buffer, false);
			Graphics.SetRandomWriteTarget(3, index_buffer, false);


			VXProcess.Drawables.Add(this);

			int groups = Mathf.CeilToInt(resolution / 64);
			clearShader.Dispatch(0, groups, groups, 1);
		}

		void Update()
		{
			activeCamera = VXProcess.Instance.Camera;
			
			cam.transform.LookAt(activeCamera.transform);

			Vector3 lightToCamVector = (activeCamera.transform.position - transform.position);
			float lightToCamDist = Vector3.Magnitude(lightToCamVector);
			float offset = lightToCamDist - LightDistance;

			if (Mathf.Abs(offset) > sigma)
			{
				transform.position = transform.position + Vector3.Normalize(lightToCamVector) * offset;
			}

			Shader.SetGlobalMatrix(cameraId, activeCamera.transform.worldToLocalMatrix);
		}

		/*
		void OnDisable()
		{
			if (light_buffer != null) { 
				light_buffer.Release();
				light_buffer = null;
			}

			if (depth_buffer != null)
			{
				depth_buffer.Release();
				depth_buffer = null;
			}

			if (index_buffer != null)
			{
				index_buffer.Release();
				index_buffer = null;
			}
		}
		*/

		void OnApplicationQuit()
		{
			if (light_buffer != null)
			{
				light_buffer.Release();
				light_buffer = null;
			}

			if (depth_buffer != null)
			{
				depth_buffer.Release();
				depth_buffer = null;
			}

			if (index_buffer != null)
			{
				index_buffer.Release(); // or Dispose?
				index_buffer = null;
			}
		}

		void LateUpdate()
		{
			// Last Actions before Graphics calls
			int groups = Mathf.CeilToInt(resolution / 8);
			clearShader.Dispatch(0, groups, groups, 1);

			cam.Render(); 
		}

		public void Draw()
		{

			if (!gameObject.activeInHierarchy || !enabled)
			{
				// Debug.Log($"{gameObject.name}: Skipping");
				return;
			}

			light_buffer.GetData(poltex_data); // Can get native buffers

			index_buffer.GetData(current_index);

			if(current_index[0] > poltex_data.Length)
			{
				// Debug.Log($"Failed to Clear Index Buffer Correct. Recieved Count {current_index[0]}");
				current_index[0] = poltex_data.Length;
			}

			int voxel_count;
			int groups = Mathf.CeilToInt((float)current_index[0] / (float)group_size);

			// Debug.Log($"Indices({current_index[0]}) / Group_Size({group_size}) = Count({groups})");
			for(int idx = 0; idx < groups; idx++)
			{
				// Group size unless last group
				voxel_count = idx < (groups - 1) ? group_size : (current_index[0] % group_size);
				//Debug.Log(voxel_count);

				if((voxel_count + idx * group_size) > poltex_data.Length)
				{
					Debug.Log($"VoxelCount {voxel_count } : Total {(voxel_count + idx * group_size)} : Length {poltex_data.Length}");
					return;
				}

				System.Array.Copy(poltex_data, idx * group_size, poltex_buffer, 0, voxel_count);

				VXProcess.Runtime.DrawUntexturedMesh(poltex_buffer, voxel_count, null, 0, 0, 0xffffff);
			}
		}
	}
}