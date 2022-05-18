using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples
{
    public class CharacterSpawner : MonoBehaviour {

        public GameObject spawnable;
        [FormerlySerializedAs("max_chars")] public int maxChars;
        private List<GameObject> _chars;
    
        // Use this for initialization
        private void Start () {
            _chars = new List<GameObject>();
        }
	
        // Update is called once per frame
        private void Update () {
            if(_chars.Count == 0 || (Time.frameCount % 173 == 0 && _chars.Count < maxChars))
            {
                try
                {
                    _chars.Add(Instantiate(spawnable, new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)), Quaternion.identity));
                    _chars[_chars.Count - 1].AddComponent<VXGameObject>();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while Spawning Models {gameObject.name}: {e.Message}");
                }
            }

            if (Time.frameCount % 180 != 0 || _chars.Count < maxChars) return;
            GameObject fatalChar = _chars[0];
            _chars.RemoveAt(0);
            Destroy(fatalChar);

        }
    }
}
