using UnityEngine;

namespace  Voxon.Examples
{
    public class Destroyer : MonoBehaviour {
    
        private void OnCollisionEnter(Collision collision)
        {
            Destroy(collision.gameObject);
        }
    }
}

