using System;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class ParticleModel
    {
        private const float SizeModifier = 0.05f;

        private GameObject _parent;

        private ParticleSystem _mParticleSystem;
        private ParticleSystem.Particle[] _mParticles;
        private int _nParticles;

        private Matrix4x4 _mat = Matrix4x4.identity;

        #region data_manipulation
        public void Update()
        {        }
        #endregion

        #region getters_setters
        public ParticleSystem ParticleSystem
        {
            set {
                _mParticleSystem = value;
                ParticleSystem.MainModule main = _mParticleSystem.main;
                _mParticles = new ParticleSystem.Particle[main.maxParticles];

                // Use worldspace
                main.simulationSpace = ParticleSystemSimulationSpace.World;
            }
        }
        public int ParticleCount => (_mParticleSystem != null) ? _mParticleSystem.GetParticles(_mParticles) : 0;

        public float GetParticleSize(int particleIndex)
        {
            return _mParticles[particleIndex].GetCurrentSize(_mParticleSystem) * SizeModifier;
        }

        public point3d GetParticle(int particleIndex)
        {
            return (VXProcess.Instance.Transform * _mParticles[particleIndex].position).ToPoint3d();
        }

        public int GetParticleColour(int particleIndex)
        {
            return ((_mParticles[particleIndex].GetCurrentColor(_mParticleSystem)).ToInt() & 0xffffff) >> 0;
        }

        public GameObject Parent
        {
            get => _parent;
            set { _parent = value; Update(); }
        }
        
        public Matrix4x4 GetMatrix(int particleIndex)
        {
            _mat.SetTRS(_mParticles[particleIndex].position, Quaternion.Euler(_mParticles[particleIndex].rotation3D), _mParticles[particleIndex].GetCurrentSize3D(_mParticleSystem)*5);
            return _mat;
        }
        #endregion
    }
}
