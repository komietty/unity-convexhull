using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d2.animatedquickhull {
    using f2 = float2;
    using V2 = Vector2;
    using V3 = Vector3;

    public abstract class AnimatedQuickhull2D {
        public Transform[] bones { get; protected set; }
        public Convex convex { get; protected set; }
        public abstract void Execute();
    }

    public class AnimatedQuickhull2DIndex: AnimatedQuickhull2D {
        protected IEnumerable<int> vids;
        protected Mesh mesh;
        protected V3[] vrts;
        protected int[] tris;

        public AnimatedQuickhull2DIndex(SkinnedMeshRenderer skin, IEnumerable<int> tids) {
            this.bones = skin.bones;
            this.mesh = skin.sharedMesh;
            this.vrts = mesh.vertices; 
            this.tris = mesh.triangles;
            vids = tids.Select(tid => tris[tid * 3]);
        }

        public override void Execute() {
            var tgts = new List<f2>();
            var mtxs = new float4x4[bones.Length];
            for (var i = 0; i < bones.Length; i++) tgts.Add((V2)bones[i].position);
            for (int i = 0; i < mtxs.Length;  i++) mtxs[i] = bones[i].localToWorldMatrix * mesh.bindposes[i];
            foreach (var i in vids) {
                var w = mesh.boneWeights[i];
                var m = mtxs[w.boneIndex0] * w.weight0 +
                        mtxs[w.boneIndex1] * w.weight1 +
                        mtxs[w.boneIndex2] * w.weight2 +
                        mtxs[w.boneIndex3] * w.weight3;
                tgts.Add((V2)((Matrix4x4)m).MultiplyPoint3x4(vrts[i]));
            }
            convex = new Convex(tgts);
            convex.ExpandLoop();
        }
    }

    public class AnimatedQuickhull2DAprox: AnimatedQuickhull2D {
        protected float[] distPerBone;
        protected float distanceThold;

        public AnimatedQuickhull2DAprox(SkinnedMeshRenderer skin, float weightThold = 0.3f, float distanceThold = 0.3f) {
            this.bones = skin.bones;
            this.distPerBone = new float[bones.Length];
            this.distanceThold = distanceThold;
            var mesh = skin.sharedMesh;
            var vtcs = mesh.vertices;
            var bspv = mesh.GetBonesPerVertex();
            var wgts = mesh.GetAllBoneWeights();
            var bwid = 0;
            var bindposes = mesh.bindposes;

            for (var vi = 0; vi < vtcs.Length; vi++) {
                var bw = wgts[bwid];
                if (bw.weight > weightThold) {
                    var i = bw.boneIndex;
                    var d1 = distPerBone[i];
                    var d2 = length(vtcs[vi] - (V3)(bindposes[i].inverse * new Vector4(0, 0, 0, 1)));
                    if (d2 > d1) distPerBone[i] = d2;
                    bwid += bspv[vi];
                }
            }
        }

        public override void Execute() {
            var bps = new List<f2>();
            for (var i = 0; i < bones.Length; i++) {
                var b = bones[i];
                var d = distPerBone[i];
                if (d > distanceThold) {
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
