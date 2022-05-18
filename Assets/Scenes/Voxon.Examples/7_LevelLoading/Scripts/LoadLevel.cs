using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Voxon.Examples
{
	public class LoadLevel : MonoBehaviour
	{
		[FormerlySerializedAs("LevelName")] public string levelName;

		[FormerlySerializedAs("LoadTime")] [Tooltip("Seconds")]
		public float loadTime;
		// Start is called before the first frame update
		private void Start()
		{
			Invoke(nameof(DelayedStart),loadTime);
		}

		private void DelayedStart()
		{
			StartCoroutine(routine: LoadScene());
		}

		private IEnumerator LoadScene()
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}
}
