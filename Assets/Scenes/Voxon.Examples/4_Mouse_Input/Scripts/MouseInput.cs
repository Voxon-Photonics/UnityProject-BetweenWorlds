using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples
{
    public class MouseInput : MonoBehaviour {
        private Vector3 _minLimits = new Vector3(-4.5f, -1, -4.5f);
        private Vector3 _maxLimits = new Vector3(4.5f, -1, 4.5f);

        [FormerlySerializedAs("Meshes")] public Mesh[] meshes = new Mesh[4];

        // Update is called once per frame
        private void FixedUpdate () {
            MousePosition mp = Voxon.Input.GetMousePos();

            Vector3 position = gameObject.transform.position;
            position = new Vector3(Mathf.Clamp(position.x + mp.X, _minLimits.x, _maxLimits.x),
                Mathf.Clamp(position.y + mp.Z, _minLimits.y, _maxLimits.y),
                Mathf.Clamp(position.z - mp.Y, _minLimits.z, _maxLimits.z));
            gameObject.transform.position = position;

            if(Voxon.Input.GetMouseButtonDown("Left"))
            {
                gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
            }

            if (Voxon.Input.GetMouseButtonDown("Right"))
            {
                transform.localScale = new Vector3(Random.Range(0.25f, 3f), Random.Range(0.25f, 3f), Random.Range(0.25f, 3f));
            }

        }
    }
}
