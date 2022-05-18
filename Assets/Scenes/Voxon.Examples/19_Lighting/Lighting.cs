using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
	[RequireComponent(typeof(Camera))]
	public class Lighting : MonoBehaviour
	{
		private Camera cam;
		private RenderTexture rt;
		private int PixelCount = 0;
		private int width = 0;
		private int height = 0;

		LightVoxels lv;
		Rect lightWindow;
		Texture2D lightTexture;
		Color32 [] colours;
		
		// Start is called before the first frame update
		void Start()
		{
			cam = GetComponent<Camera>();
			rt = cam.targetTexture;
			width = rt.width;
			height = rt.height;
			PixelCount = width * height;

			lightWindow = new Rect(0, 0, width, height);
			lightTexture = new Texture2D(width, height);
			lv = new LightVoxels(width, height);

		}

		// Update is called once per frame
		void Update()
		{
			cam.Render();
			RenderTexture.active = rt;
			lightTexture.ReadPixels(lightWindow, 0, 0);
			lightTexture.Apply();
			RenderTexture.active = null;

			colours = lightTexture.GetPixels32();
			lv.Update(ref colours);
		}
	}
}