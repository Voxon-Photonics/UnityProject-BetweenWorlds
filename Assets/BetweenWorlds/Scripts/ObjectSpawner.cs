using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples
{
    public class ObjectSpawner : MonoBehaviour {

        [FormerlySerializedAs("Mats")] public Material[] mats;
		public static int frameSpawn = 25;
		public static float upwardForce = 5f;

		public void Start()
		{
			// Invert Gravity
			Physics.gravity*= -1;
		}

		public static void IncreaseSpawnRate()
		{
			frameSpawn--;
			if (frameSpawn < 1) frameSpawn = 1;
		}

		public static void DecreaseSpawnRate()
		{
			frameSpawn++;
		}

		public static void IncreaseGravity()
		{
			Physics.gravity += new Vector3(0, 0.1f, 0);
			// upwardForce -= 1;
		}

		public static void DecreaseGravity()
		{
			Physics.gravity -= new Vector3(0, 0.1f, 0);
			// upwardForce += 1;
		}


		// Update is called once per frame
		private void Update () {
            if (Time.frameCount % frameSpawn != 0) return;
        
            int type = Random.Range(0, 5);

            while (type == 4)
            {
                type = Random.Range(0, 5);
            }
            
            GameObject h = GameObject.CreatePrimitive((PrimitiveType)type);
            
            if (h.GetComponent<Renderer>() == null)
            {
                h.AddComponent<Renderer>();
            }

            h.GetComponent<Renderer>().sharedMaterial = mats[Random.Range(0, mats.Length-1)];

            h.transform.SetPositionAndRotation((transform.position + new Vector3(Random.Range(-3.5f,3.5f), 0, Random.Range(-3.5f, 3.5f))), new Quaternion (Random.Range(-10f, 10f), Random.Range(-10f, 3.5f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
            var bod = h.AddComponent<Rigidbody>();
            bod.AddForce(new Vector3(Random.Range(-10.0f, 10.0f), -upwardForce, Random.Range(-10.0f, 10.0f)));
            h.AddComponent<VXGameObject>();
        }
    }
}
