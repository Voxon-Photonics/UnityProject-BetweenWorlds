using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Voxon
{
	class UnityBuildPostProcessor : IPostprocessBuildWithReport
	{
		public int callbackOrder => 0;

		public void OnPostprocessBuild(BuildReport report)
		{
			string fileName = "";
			string outputDirectory = "";

			foreach(BuildFile file in report.files)
			{
			
				if(file.role != "Executable") continue;
			
				fileName = Path.GetFileName(file.path);
				outputDirectory = Path.GetDirectoryName(file.path);
				Debug.Log($"File: {fileName}, Folder: {outputDirectory}");
			}

			// Generate Batch executable
			string batchContents = $"start \"\" \"{fileName}\" -batchmode";
			StreamWriter writer = new StreamWriter(outputDirectory + "\\VX.bat");

			try
			{
				writer.WriteLine(batchContents);
			}
			catch (Exception e)
			{
				Debug.LogError("Unable to write batch file");
				Debug.LogError(e.Message);
			}
			finally
			{
				writer.Close();
			}
		}
	}
}