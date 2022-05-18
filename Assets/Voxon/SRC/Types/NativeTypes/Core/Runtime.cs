using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UnityEngine;

namespace Voxon
{
	public enum ColorMode
	{
		// Dual Color - Currently Disabled
		BG = 4, 
		RG  = 3,
		RB = 2,
		// Full Color
		RGB = 1,
		// Monochrome
		WHITE = 0,
		RED = -1,
		GREEN = -2,
		YELLOW = -3,
		BLUE = -4,
		MAGENTA = -5,
		CYAN = -6
	};

	public enum MENUPOSITION
	{
		SINGLE = 3,
		FIRST = 1,
		MIDDLE = 0,
		END = 2
	}
	
	public class Runtime : IRuntimePromise
	{
		private string _pluginFilePath = "";
		private const string PluginFileName = "C#-Runtime.dll";
		
		private const string PluginTypeName = "Voxon.Runtime";

		public string ActiveRuntime;
		
		private static Type _tClassType;
		private static object _runtime;

		private static Dictionary<string, MethodInfo> _features;

		public Runtime()
		{
			_features = new Dictionary<string, MethodInfo>();
			FindDll();

			if (!System.IO.File.Exists(_pluginFilePath))
			{
				Debug.LogError("C#-Runtime.dll not found in Runtime directory.\nPlease ensure Voxon Runtime is correctly installed");
				Windows.Error("C#-Runtime.dll not found in Runtime directory.\nPlease ensure Voxon Runtime is correctly installed");
				_runtime = null;
				Application.Quit();
			}

			Assembly asm = Assembly.LoadFrom(_pluginFilePath);
			_tClassType = asm.GetType(PluginTypeName);
			ActiveRuntime = PluginTypeName;
			if (_tClassType == null)
			{
				Debug.LogError("Voxon Runtime failed to load from local C#-Runtime.dll.");
				Windows.Error("Voxon Runtime failed to load from local C#-Runtime.dll.");
				_runtime = null;
				Application.Quit();
			}

			try
			{
				_runtime = Activator.CreateInstance(_tClassType);

				MethodInfo makeRequestMethod = _tClassType.GetMethod("GetFeatures");
				if (makeRequestMethod == null) return;

				var featureNames = (HashSet<string>) makeRequestMethod.Invoke(_runtime, null);

				foreach (string feature in featureNames)
				{
					_features.Add(feature, _tClassType.GetMethod(feature));
				}
			}
			catch (Exception e)
			{
				Windows.Alert($"Voxon Runtime Bridge failed to load.\nCheck your version of C#-bridge-interface.dll\n{e}");
				Application.Quit();
			}
			
			
		}

		private void FindDll()
		{
			if (_pluginFilePath != "") return;
			
			if (File.Exists(PluginFileName))
			{
				_pluginFilePath = PluginFileName;
				return;
			}

#if NET_4_6
			RegistryKey dll = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon");
			if (dll != null)
			{
				_pluginFilePath =
					$"{(string) Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon")?.GetValue("Path")}{PluginFileName}";
				return;
			}
#endif

			string voxon_path = Environment.GetEnvironmentVariable("VOXON_RUNTIME", EnvironmentVariableTarget.User);
			
			if (File.Exists($"{voxon_path}{PluginFileName}"))
			{
				_pluginFilePath = $"{voxon_path}{PluginFileName}";
				return;
			}
			else
			{
				Debug.Log($"{voxon_path}{PluginFileName} Doesn't Exist");
				
			}
			
			string[] paths = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)?.Split(';');

			if (paths == null) return;
			
			foreach (string path in paths)
			{
				if (!File.Exists($"{path}\\{PluginFileName}")) continue;
				
				_pluginFilePath = $"{path}\\{PluginFileName}";
			}
		}

		public void DrawBox(ref point3d min, ref point3d max, int fill, int colour)
		{
			_features["DrawBox"].Invoke(_runtime, parameters: new object[] { min, max, fill, colour });
		}

		public void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int flags, int col)
		{
			_features["DrawCube"].Invoke(_runtime, new object[] { pp, pr, pd, pf, flags, col });
		}

		public void DrawGuidelines()
		{
			_features["DrawGuidelines"].Invoke(_runtime, null);
		}

		public void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int colorkey, int minHeight, int flags)
		{
			var paras = new object[] { texture, pp, pr, pd, pf, colorkey, minHeight, flags};
			_features["DrawHeightmap"].Invoke(_runtime, paras);
		}

		public void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, int col, byte[] text)
		{
			var paras = new object[] { pp, pr, pd, col, text};
			_features["DrawLetters"].Invoke(_runtime, paras);
		}

		public void DrawLine(ref point3d min, ref point3d max, int col)
		{
			var paras = new object[] { min, max, col };
			_features["DrawLine"].Invoke(_runtime, paras);
		}

		public void DrawPolygon(pol_t[] pt, int ptCount, int col)
		{
			var paras = new object[] { pt, ptCount, col };
			_features["DrawPolygon"].Invoke(_runtime, paras);
		}

		public void DrawSphere(ref point3d position, float radius, int issol, int colour)
		{
			var paras = new object[] { position, radius, issol, colour };
			_features["DrawSphere"].Invoke(_runtime, paras);
		}

		public void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags)
		{
			var paras = new object[] { texture, vertices, verticeCount, indices, indiceCount, flags };
			_features["DrawTexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawLitTexturedMesh(ref tiletype texture, poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags, int ambient_color = 0x040404)
		{
			var paras = new object[] { texture, vertices, verticeCount, indices, indiceCount, flags, ambient_color };
			_features["DrawLitTexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawUntexturedMesh(poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags, int colour)
		{
			var paras = new object[] { vertices, verticeCount, indices, indiceCount, flags, colour };
			_features["DrawUntexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawVoxel(ref point3d position, int col)
		{
			var paras = new object[] { position, col };
			_features["DrawVoxel"].Invoke(_runtime, paras);
		}

		public void DrawVoxelBatch(ref point3d[] positions, int voxel_count, int colour)
		{
			// colour
			var paras = new object[] { positions, voxel_count, colour };
			_features["DrawVoxelBatch"].Invoke(_runtime, paras);
		}

		public void DrawVoxels(ref point3d[] positions, int voxel_count, ref int[] colours)
		{
			var paras = new object[] { positions, voxel_count, colours };
			_features["DrawVoxels"].Invoke(_runtime, paras);
		}

		public void FrameEnd()
		{
			_features["FrameEnd"].Invoke(_runtime, null);
		}

		public bool FrameStart()
		{
			return (bool)_features["FrameStart"].Invoke(_runtime, null);
		}

		public float[] GetAspectRatio()
		{
			return (float[]) _features["GetAspectRatio"].Invoke(_runtime, null);

		}

		public float GetAxis(int axis, int player)
		{
			var paras = new object[] { axis, player };
			return (float)_features["GetAxis"].Invoke(_runtime, paras);
		}

		public bool GetButton(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButton"].Invoke(_runtime, paras);

		}

		public bool GetButtonDown(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButtonDown"].Invoke(_runtime, paras);
		}

		public bool GetButtonUp(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButtonUp"].Invoke(_runtime, paras);
		}

		public HashSet<string> GetFeatures()
		{
			return (HashSet<string>)_features["DrawPolygon"].Invoke(_runtime, null);
		}

		public bool GetKey(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKey"].Invoke(_runtime, paras);
		}

		public bool GetKeyDown(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKeyDown"].Invoke(_runtime, paras);
		}

		public int GetKeyState(int keycode)
		{
			var paras = new object[] { keycode };
			return (int)_features["GetKeyState"].Invoke(_runtime, paras);
		}

		public bool GetKeyUp(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKeyUp"].Invoke(_runtime, paras);
		}

		public bool GetMouseButton(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetMouseButton"].Invoke(_runtime, paras);
		}

		public bool GetMouseButtonDown(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetMouseButtonDown"].Invoke(_runtime, paras);
		}

		public float[] GetMousePosition()
		{
			return (float[])_features["GetMousePosition"].Invoke(_runtime, null);
		}

		public float[] GetSpaceNavPosition()
		{
			return (float[])_features["GetSpaceNavPosition"].Invoke(_runtime, null);
		}
		
		public float[] GetSpaceNavRotation()
		{
			return (float[])_features["GetSpaceNavRotation"].Invoke(_runtime, null);
		}
		
		public bool GetSpaceNavButton(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetSpaceNavButton"].Invoke(_runtime, paras);
		}

		public float GetVolume()
		{
			return (float)_features["GetVolume"].Invoke(_runtime, null);
		}

		public void Initialise()
		{
			_features["Initialise"].Invoke(_runtime, null);
		}

		public bool isInitialised()
		{
			return (bool)_features["isInitialised"].Invoke(_runtime, null);
		}

		public bool isLoaded()
		{
			return (bool)_features["isLoaded"].Invoke(_runtime, null);
		}

		public void Load()
		{
			_features["Load"].Invoke(_runtime, null);
		}

		public void LogToFile(string msg)
		{
			var paras = new object[] { msg };
			_features["LogToFile"].Invoke(_runtime, paras);
		}

		public void LogToScreen(int x, int y, string text)
		{
			var paras = new object[] { x, y, text };
			_features["LogToScreen"].Invoke(_runtime, paras);
		}

		public void SetAspectRatio(float aspx, float aspy, float aspz)
		{
			var paras = new object[] { aspx, aspy, aspz };
			_features["SetAspectRatio"].Invoke(_runtime, paras);
		}

		public void SetColorMode(int colour)
		{
			var paras = new object[] { colour };
			_features["SetColorMode"].Invoke(_runtime, paras);
		}
		
		public int GetColorMode()
		{
			return (int)_features["isInitialised"].Invoke(_runtime, null);
		}

		public void SetDisplayColor(ColorMode color)
		{
			SetColorMode((int)color);
		}
		
		public ColorMode GetDisplayColor()
		{
			return (ColorMode) GetColorMode();
		}

		public void Shutdown()
		{
			_features["Shutdown"].Invoke(_runtime, null);
		}

		public void Unload()
		{
			_features["Unload"].Invoke(_runtime, null);
		}

		public long GetDLLVersion()
		{
			return (long)_features["GetDLLVersion"].Invoke(_runtime, null);
		}

		public string GetSDKVersion()
		{
			return (string) _features["GetSDKVersion"].Invoke(_runtime, null);
		}
		
		#region Helix Controls
		public bool GetHelixMode()
		{
			return _features.ContainsKey("GetHelixMode") && (bool) _features["GetHelixMode"].Invoke(_runtime, null);
		}

		public void SetSimulatorHelixMode(bool helix)
		{
			if (_features.ContainsKey("SetSimulatorHelixMode"))
			{
				var paras = new object[] { helix };
				_features["SetSimulatorHelixMode"].Invoke(_runtime, paras);
			}
		}

		public float GetExternalRadius()
		{
			if (_features.ContainsKey("GetExternalRadius"))
			{
				return (float) _features["GetExternalRadius"].Invoke(_runtime, null);
			}

			return 0.0f;
		}
		
		public void SetExternalRadius(float radius)
		{
			if (_features.ContainsKey("SetExternalRadius"))
			{
				var paras = new object[] { radius };
				_features["SetExternalRadius"].Invoke(_runtime, paras);
			}
		}
		
		public float GetInternalRadius()
		{
			if (_features.ContainsKey("GetInternalRadius"))
			{
				return (float) _features["GetInternalRadius"].Invoke(_runtime, null);
			}

			return 0.0f;
		}
		
		public void SetInternalRadius(float radius)
		{
			if (_features.ContainsKey("SetInternalRadius"))
			{
				var paras = new object[] { radius };
				_features["SetInternalRadius"].Invoke(_runtime, paras);
			}
		}
		
		#endregion

		#region Menu Controls
		public void MenuReset(MenuUpdateHandler menuUpdate, IntPtr userdata)
		{
			var paras = new object[] { menuUpdate, userdata };
			_features["MenuReset"].Invoke(_runtime, paras);
		}

		public void MenuAddTab(string text, int x, int y, int width, int height)
		{
			var paras = new object[] { text, x, y, width, height };
			_features["MenuAddTab"].Invoke(_runtime, paras);
		}

		public void MenuAddText(int id, string text, int x, int y, int width, int height, int colour)
		{
			var paras = new object[] { id, text, x, y, width, height, colour };
			_features["MenuAddText"].Invoke(_runtime, paras);
		}

		public void MenuAddButton(int id, string text, int x, int y, int width, int height, int colour, int position)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, position };
			_features["MenuAddButton"].Invoke(_runtime, paras);
		}

		public void MenuAddVerticleSlider(int id, string text, int x, int y, int width, int height, int colour, double initial_value,
			double min, double max, double minor_step, double major_step)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, initial_value, min, max, minor_step, major_step };
			_features["MenuAddVerticleSlider"].Invoke(_runtime, paras);
		}

		public void MenuAddHorizontalSlider(int id, string text, int x, int y, int width, int height, int colour, double initial_value,
			double min, double max, double minor_step, double major_step)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, initial_value, min, max, minor_step, major_step };
			_features["MenuAddHorizontalSlider"].Invoke(_runtime, paras);
		}

		public void MenuAddEdit(int id, string text, int x, int y, int width, int height, int colour, bool hasFollowupButton = false)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, hasFollowupButton };
			_features["MenuAddEdit"].Invoke(_runtime, paras);
		}

		public void MenuUpdateItem(int id, string st, int down, double v)
		{
			var paras = new object[] { id, st, down, v };
			_features["MenuReset"].Invoke(_runtime, paras);
		}
		#endregion
		
		#region Emulator
		public float SetEmulatorHorizontalAngle(float radians)
		{
			var paras = new object[] { radians };
			return (float) _features["SetEmulatorHorizontalAngle"].Invoke(_runtime, paras);
		}

		public float SetEmulatorVerticalAngle(float radians)
		{
			var paras = new object[] { radians };
			return (float)_features["SetEmulatorVerticalAngle"].Invoke(_runtime, paras);
		}

		public float SetEmulatorDistance(float distance)
		{
			var paras = new object[] { distance };
			return (float)_features["SetEmulatorDistance"].Invoke(_runtime, paras);
		}
		
		public float GetEmulatorHorizontalAngle()
		{
			return (float) _features["GetEmulatorHorizontalAngle"].Invoke(_runtime, null);
		}

		public float GetEmulatorVerticalAngle()
		{
			return (float)_features["GetEmulatorVerticalAngle"].Invoke(_runtime, null);
		}

		public float GetEmulatorDistance()
		{
			return (float)_features["GetEmulatorDistance"].Invoke(_runtime, null);
		}

		public void StartRecording(string filename, int vps)
		{
			Debug.Log($"StartRecording: {filename}");
			var paras = new object[] { filename, vps };
			_features["StartRecording"].Invoke(_runtime, paras);
		}

		public void EndRecording()
		{
			Debug.Log("EndRecording");
			_features["EndRecording"].Invoke(_runtime, null);
		}

		public void GetVCB(string filename, int vps)
		{
			var paras = new object[] { filename, vps };
			_features["GetVCB"].Invoke(_runtime, paras);
		}

		#endregion
	}
}
