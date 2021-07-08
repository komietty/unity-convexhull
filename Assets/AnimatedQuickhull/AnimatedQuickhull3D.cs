using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3.animatedquickhull {
    using f3 = float3;
    using V3 = Vector3;

    public abstract class AnimatedQuickhull3D { 
        public Transform[] bones { get; protected set; }
        public Convex convex { get; protected set; }
        public abstract void Execute(int itr);

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

    public class AnimatedQuickhull3DIndex: AnimatedQuickhull3D {
        protected IEnumerable<int> vids;
        protected Mesh mesh;
        protected V3[] vrts;
        protected int[] tris;

        public AnimatedQuickhull3DIndex(SkinnedMeshRenderer skin, IEnumerable<int> tids) {
            this.bones = skin.bones;
            this.mesh = skin.sharedMesh;
            this.vrts = mesh.vertices; 
            this.tris = mesh.triangles;
            vids = tids.Select(tid => tris[tid * 3]);
        }

        public override void Execute(int itr) {
            var tgts = new List<f3>();
            var mtxs = new float4x4[bones.Length];
            for (var i = 0; i < bones.Length; i++) tgts.Add(bones[i].position);
            for (int i = 0; i < mtxs.Length;  i++) mtxs[i] = bones[i].localToWorldMatrix * mesh.bindposes[i];
            foreach (var i in vids) {
                var w = mesh.boneWeights[i];
                var m = mtxs[w.boneIndex0] * w.weight0 +
                        mtxs[w.boneIndex1] * w.weight1 +
                        mtxs[w.boneIndex2] * w.weight2 +
                        mtxs[w.boneIndex3] * w.weight3;
                tgts.Add(((Matrix4x4)m).MultiplyPoint3x4(vrts[i]));
            }
            convex = new Convex(tgts);
            convex.ExpandLoop(itr);
        }
    }

    public class AnimatedQuickhull3DAprox: AnimatedQuickhull3D { 
        public float[] distPerBone { get; }
        public float   distanceThold;

        public AnimatedQuickhull3DAprox(SkinnedMeshRenderer skin, float weightThold, float distanceThold) {
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

        public override void Execute(int itr) {
            var bps = new List<f3>();
            for (var i = 0; i < bones.Length; i++) {
                var b = bones[i];
                var d = distPerBone[i];
                if (d > distanceThold) {
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
    }
}
