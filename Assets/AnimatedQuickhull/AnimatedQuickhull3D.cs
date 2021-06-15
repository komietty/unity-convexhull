using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3.animatedquickhull {
    using f3 = float3;
    using V3 = Vector3;

    public class AnimatedQuickhull3D {
        public Transform[] bones { get; protected set; }
        public Convex convex { get; protected set; }
        public float[] distPerBone { get; }

        public AnimatedQuickhull3D(SkinnedMeshRenderer skin, float weightThreshold) {
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
                if (bw.weight > weightThreshold) {
                    var i = bw.boneIndex;
                    var d1 = distPerBone[i];
                    var d2 = length(vtcs[vi] - (V3)(bindposes[i].inverse * new Vector4(0, 0, 0, 1)));
                    if (d2 > d1) distPerBone[i] = d2;
                    bwid += bspv[vi];
                }
            }
        }

        public void Execute(float distanceThreshold, int itr) {
            var bps = new List<f3>();
            for (var i = 0; i < bones.Length; i++) {
                var b = bones[i];
                var d = distPerBone[i];
                if (d > distanceThreshold) {
                    bps.Add(b.position + new V3(-d, -d, -d));
                    bps.Add(b.position + new V3(+d, +d, +d));
                    bps.Add(b.position + new V3(+d, -d, -d));
                    bps.Add(b.position + new V3(-d, +d, -d));
                    bps.Add(b.position + new V3(-d, -d, +d));
                    bps.Add(b.position + new V3(-d, +d, +d));
                    bps.Add(b.position + new V3(+d, -d, +d));
                    bps.Add(b.position + new V3(+d, +d, -d));
                } else { 
                    bps.Add(b.position);
                }
            }
            convex = new Convex(bps);
            convex.ExpandLoop(itr);
        }

        public Mesh CreateMesh() {
            var mesh = new Mesh();
            var nodes = convex.nodes;
            var vs = new V3[nodes.Count * 3];
            var ts = new int[nodes.Count * 3];
            for(var i =0; i < nodes.Count; i++) {
                var j = i * 3;
                var n = nodes[i];
                var v = cross(n.t.b - n.t.a, n.t.c - n.t.a);
                var f = dot(v, n.normal) < 0;
                vs[j + 0] = (f3)n.t.a;
                vs[j + 1] = f ? (f3)n.t.c : (f3)n.t.b;
                vs[j + 2] = f ? (f3)n.t.b : (f3)n.t.c;
                ts[j + 0] = j;
                ts[j + 1] = j + 1;
                ts[j + 2] = j + 2;
            }
            mesh.SetVertices(vs);
            mesh.SetTriangles(ts, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
