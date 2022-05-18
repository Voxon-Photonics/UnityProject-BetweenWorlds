using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Voxon.Examples.Input
{
	public class InputReporter : MonoBehaviour {

		// Update is called once per frame
		void Update () {

			if (VXProcess.Instance.active)
			{
				// Left Arrow: Reduce Gravity
				if (Voxon.Input.GetKey("ReduceGravity"))
				{
					ObjectSpawner.DecreaseGravity();
				}

				// Right Arrow: Increase Gravity
				if (Voxon.Input.GetKey("IncreaseGravity"))
				{
					ObjectSpawner.IncreaseGravity();
				}

				// Up Arrow: Increase Spawn Rate
				if (Voxon.Input.GetKey("IncreaseSpawnRate"))
				{
					ObjectSpawner.IncreaseSpawnRate();
				}

				// Down Arrow: Reduced Spawn Rate
				if (Voxon.Input.GetKey("ReduceSpawnRate"))
				{
					ObjectSpawner.DecreaseSpawnRate();
				}

				if(Voxon.Input.GetKeyDown("ResetLevel"))
				{
					StartCoroutine(routine: LoadScene());
				}
			}
		}

		private IEnumerator LoadScene()
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BetweenWorlds");

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}
}
