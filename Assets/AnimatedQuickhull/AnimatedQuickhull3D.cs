using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace kmty.geom.d3.animatedquickhull {
    using f2 = float2;
    using f3 = float3;
    using V2 = Vector2;
    using V3 = Vector3;
    using SG = Segment;

    public class AnimatedQuickhull3D {
        public Transform[] bones { get; protected set; }
        public Convex convex { get; protected set; }
        public Mesh mesh { get; protected set; }

        public AnimatedQuickhull3D(SkinnedMeshRenderer skin) {
            this.bones = skin.bones;
            this.mesh = new Mesh();
            Execute();
        }

        public void Execute() {
            convex = new Convex(bones.Select(b => (f3)b.position + new f3(0, 0, 0)));
            //convex.Expand();
            convex.ExpandLoop();
        }

        public void CreateMesh() { 
            var nodes = convex.nodes;
            var vs = new V3[nodes.Count * 3];
            var ns = new V3[nodes.Count * 3];
            var ts = new int[nodes.Count * 3];
            for(var i =0; i < nodes.Count; i++) {
                var j = i * 3;
                var n = nodes[i];
                var v = cross(n.t.b - n.t.a, n.t.c - n.t.a);
                var f = dot(v, n.normal) < 0;
                vs[j + 0] = (f3)n.t.a;
                vs[j + 1] = f ? (f3)n.t.c : (f3)n.t.b;
                vs[j + 2] = f ? (f3)n.t.b : (f3)n.t.c;
                ns[j + 0] = (f3)n.normal;
                ns[j + 1] = (f3)n.normal;
                ns[j + 2] = (f3)n.normal;
                ts[j + 0] = j;
                ts[j + 1] = j + 1;
                ts[j + 2] = j + 2;
            }
            this.mesh.SetVertices(vs);
            this.mesh.SetNormals(ns);
            this.mesh.SetTriangles(ts, 0);
        }
    }
}
