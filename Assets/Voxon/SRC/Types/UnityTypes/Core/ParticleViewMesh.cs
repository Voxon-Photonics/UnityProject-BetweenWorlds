using System;
using UnityEngine;

namespace Voxon
{
    public class ParticleViewMesh : ParticleView
    {
        private RegisteredMesh _mesh;
        private poltex[] _transformedMesh;

        private int _drawFlags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer

        #region Constructors

        public ParticleViewMesh(ParticleModel particle, Mesh inMesh, GameObject parent = null) : base(particle, parent)
        {
            try
            {
                _mesh = MeshRegister.Instance.get_registed_mesh(ref inMesh);
                _transformedMesh = new poltex[_mesh.vertexCount];
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Loading Mesh: {inMesh.name}", e);
            }
        }
        #endregion

        #region drawing
        public override void Draw()
        {
            int particles = Model.ParticleCount;

            for (var idx = 0; idx < particles; ++idx)
            {

                // Unity Style
                Matrix4x4 matrix = VXProcess.Instance.Transform * Model.GetMatrix(idx);

                _mesh.compute_transform_cpu(matrix, ref _transformedMesh);

                for (int idy = _mesh.submeshCount - 1; idy >= 0; --idy)
                {
					VXProcess.Runtime.DrawUntexturedMesh(_transformedMesh, _mesh.vertexCount, _mesh.indices[idy], _mesh.indexCounts[idy], _drawFlags, Model.GetParticleColour(idx));
                }
            }
        }
        #endregion

    }
}
