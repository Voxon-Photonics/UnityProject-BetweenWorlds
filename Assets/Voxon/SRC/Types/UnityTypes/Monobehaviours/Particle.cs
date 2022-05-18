using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Voxon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particle : MonoBehaviour
    {
        public enum ParticleStyle
        {
            BILLBOARD,
            BOX,
            MESH,
            SPHERE
        };

        // Editor View (Won't use after initialisation)
        [FormerlySerializedAs("particle_style")] public ParticleStyle particleStyle = ParticleStyle.BILLBOARD;

        // Associated Models
        private ParticleModel _particleModel;

        // Associated Views
        private ParticleView _particle;

        // Use this for initialization
        private void Start()
        {
            try
            {
                var ps = GetComponent<ParticleSystem>();
                _particleModel = new ParticleModel {ParticleSystem = ps};
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"({name}) Failed to load suitable Line", e);
                Destroy(this);
            }

            switch(particleStyle)
            {
                case ParticleStyle.BILLBOARD:
                    _particle = new ParticleViewBillBoard(_particleModel);
                    break;
                case ParticleStyle.BOX:
                    _particle = new ParticleViewBox(_particleModel);
                    break;
                case ParticleStyle.SPHERE:
                    _particle = new ParticleViewSphere(_particleModel);
                    break;
                case ParticleStyle.MESH:
                    _particle = new ParticleViewMesh(_particleModel, GetComponent<ParticleSystemRenderer>().mesh);
                    break;
            }

            if (_particle != null && _particleModel != null) return;
            
            Debug.LogError($"Particle? {(_particle != null)} ParticleModel? {(_particleModel != null)}");
            Destroy(this);
        }
    }
}