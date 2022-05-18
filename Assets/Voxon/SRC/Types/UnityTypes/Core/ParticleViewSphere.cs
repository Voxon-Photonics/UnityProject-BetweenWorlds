using UnityEngine;

namespace Voxon
{
    public class ParticleViewSphere : ParticleView
    {
        #region Constructors

        public ParticleViewSphere(ParticleModel particle, GameObject parent = null) : base(particle, parent) { }
        #endregion

        #region drawing
        public override void Draw()
        {
            int particles = Model.ParticleCount;

            for (var idx = 0; idx < particles; ++idx)
            {
                float size = Model.GetParticleSize(idx);
                point3d point = Model.GetParticle(idx);

				VXProcess.Runtime.DrawSphere(ref point, size, 0, Model.GetParticleColour(idx));

            }
        }
        #endregion

    }
}
