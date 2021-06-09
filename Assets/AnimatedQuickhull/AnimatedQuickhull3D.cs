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
            convex = new Convex(bones.Select(b => (f3)b.position));
            convex.ExpandLoop();
        }

        public void CreateMesh() { 
            var ns = convex.nodes;
            var vs = new Vector3[ns.Count * 3];
            var ts = new int[ns.Count * 3];
            for(var i =0; i < ns.Count; i++) {
                var j = i * 3;
                vs[j + 0] = (f3)ns[i].t.a;
                vs[j + 1] = (f3)ns[i].t.b;
                vs[j + 2] = (f3)ns[i].t.c;
                ts[j + 0] = j;
                ts[j + 1] = j + 1;
                ts[j + 2] = j + 2;
            }
            this.mesh.SetVertices(vs);
            this.mesh.SetTriangles(ts, 0);
            this.mesh.RecalculateNormals();
        }
    }
}
