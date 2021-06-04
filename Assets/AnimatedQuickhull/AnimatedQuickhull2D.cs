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
    using SG = Segment;

    public class AnimatedQuickhull2D {
        public Transform[] bones { get; protected set; }
        public Convex convex { get; protected set; }

        public AnimatedQuickhull2D(SkinnedMeshRenderer skin) {
            this.bones = skin.bones;
            Execute();
        }

        public void Execute() {
            convex = new Convex(bones.Select(b => (f2)(V2)b.position));
            convex.ExpandLoop();
        }
    }

}
