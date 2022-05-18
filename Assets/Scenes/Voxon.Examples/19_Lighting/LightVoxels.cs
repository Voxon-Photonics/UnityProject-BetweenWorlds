using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = UnityEngine.Random;

public class LightVoxels
{
	private struct PmCache
	{
		public float X;
		public float Z;
		public float XRatio;
		public float ZRatio;
		public float XPos;
		public float ZPos;

		public PmCache(float x, float z, float xRatio, float zRatio, float xPos, float zPos)
		{
			X = x;
			Z = z;
			XRatio = xRatio;
			ZRatio = zRatio;
			XPos = xPos;
			ZPos = zPos;
		}
	}

	private uint _width = 1;
	private uint _height = 1;
	public int x_units;
	public int z_units;

	public Color32 colour = Color.white;
	public Color32[] colours;
	private Vector3[] positions;
	private int iCol;
	private VXVoxelBatch _voxelBatch;
	private VXVoxels _voxelVoxels;

	private float halfWidth;
	private float halfHeight;

	private int totalUnits;
	private PmCache[] _cache;

	public LightVoxels(int Twidth = 500, int Theight = 500)
	{
		x_units = Twidth;
		z_units = Theight;

		halfWidth = _width * .5f;
		halfHeight = _height * .5f;

		totalUnits = x_units * z_units;
		positions = new Vector3[totalUnits];
		_cache = new PmCache[totalUnits];

		for (var x = 0; x < x_units; ++x)
		{
			float xRatio = (float)x / (x_units - 1);
			float xPos = xRatio * _width - halfWidth;
			for (var z = 0; z < z_units; ++z)
			{
				float zRatio = (float)z / (z_units - 1);
				float zPos = zRatio * _height - halfHeight;
				long idx = x * z_units + z;
				positions[idx] = new Vector3()
				{
					x = zPos,
					y = 0,
					z = xPos,
				};

				_cache[idx] = new PmCache(x, z, xRatio, zRatio, xPos, zPos);
			}
		}

		colours = new Color32[totalUnits];
		for (int i = 0; i < totalUnits; i++)
		{
			colours[i] = colour;
		}
		// _voxelBatch = new VXVoxelBatch(ref positions, colour);
		_voxelVoxels = new VXVoxels(ref positions, ref colours);
	}

	public void Update(ref Color32[] data)
	{
		_voxelVoxels.set_colors(ref data);

		for (var x = 0; x < x_units; ++x)
		{
			for (var z = 0; z < z_units; ++z)
			{
				int idx = x * z_units + z;
				float y = -((float)data[idx].a - 128) / 256;
				// Translation mistake
				_voxelVoxels.set_posz(idx, y);
				
			}
		}

		

	}
}
