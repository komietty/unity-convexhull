using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2.animatedquickhull {
    using f2 = float2;
    using f3 = float3;
    using V2 = Vector2;
    using V3 = Vector3;

    public class AnimatedQuickhull2D {
        public Transform[] bones { get; protected set; }
        public Convex convex { get; protected set; }
        public float[] distPerBone { get; }

        public AnimatedQuickhull2D(SkinnedMeshRenderer skin) {
            this.bones = skin.bones;
            this.distPerBone = new float[bones.Length];
            var mesh = skin.sharedMesh;
            var vtcs = mesh.vertices;
            var bspv = mesh.GetBonesPerVertex();
            var wgts = mesh.GetAllBoneWeights();
            var bwid = 0;
            var bindposes = mesh.bindposes;

            for (var vi = 0; vi < vtcs.Length; vi++) {
                var bw = wgts[bwid];
                if (bw.weight > 0.1f) {
                    var i = bw.boneIndex;
                    var d1 = distPerBone[i];
                    var d2 = length(vtcs[vi] - (V3)(bindposes[i].inverse * new Vector4(0, 0, 0, 1)));
                    if (d2 > d1) distPerBone[i] = d2;
                    bwid += bspv[vi];
                }
            }
        }

        public void Execute() {
            var bps = new List<f2>();
            for (var i = 0; i < bones.Length; i++) {
                var b = bones[i];
                var d = distPerBone[i];
                if (d > 0.3f) {
                    bps.Add((V2)b.position + new V2(-d, -d));
                    bps.Add((V2)b.position + new V2(+d, -d));
                    bps.Add((V2)b.position + new V2(-d, +d));
                    bps.Add((V2)b.position + new V2(+d, +d));
                } else { 
                    bps.Add((V2)b.position);
                }
            }
            convex = new Convex(bps);
            convex.ExpandLoop();
        }
    }
}
