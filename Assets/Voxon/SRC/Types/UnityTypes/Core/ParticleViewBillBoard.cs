using UnityEngine;

namespace Voxon
{
    public class ParticleViewBillBoard : ParticleView
    {
        #region Constructors

        public ParticleViewBillBoard(ParticleModel particle, GameObject parent = null) : base(particle, parent) { }
        #endregion

        #region drawing
        public override void Draw()
        {
            int particles = Model.ParticleCount;

            for (var idx = 0; idx < particles; ++idx)
            {
                float size = Model.GetParticleSize(idx);
                point3d point = Model.GetParticle(idx);

                point3d min;
                min.x = point.x - size;
                min.y = point.y;
                min.z = point.z - size;

                point3d max;
                max.x = point.x + size;
                max.y = point.y;
                max.z = point.z + size;

				VXProcess.Runtime.DrawBox(ref min, ref max, 2, Model.GetParticleColour(idx));
            }
        }
        #endregion

    }
}
