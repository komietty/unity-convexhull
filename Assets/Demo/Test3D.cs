using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using kmty.geom.d3;
using kmty.geom.d3.animatedquickhull;
using f3 = Unity.Mathematics.float3;

public class Test3D : MonoBehaviour {
    [SerializeField, Range(0, 100)] protected int num;
    [SerializeField, Range(0, 2)] protected float dst;
    [SerializeField] protected bool showNormal;
    [SerializeField] protected bool showOthers;
    [SerializeField, Range(0, 10)] protected int currId;
    Convex convex;

    void Start() {
        for (int i = 0; i < num; i++) {
            var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.localScale *= 0.1f;
            g.transform.position = UnityEngine.Random.onUnitSphere * dst;
            g.transform.SetParent(this.transform);
        }
    }

    void Update() {
        var l = new List<f3>();
        foreach (Transform c in transform) {
            l.Add(c.position);
        }
        convex = new Convex(l);
        convex.ExpandLoop();
    }

    void OnRenderObject() {
        GL.PushMatrix();
        GL.Color(Color.white);
        convex.Draw();
        GL.PopMatrix();
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        var nodes = convex.nodes;
        if (showOthers) {
            for (int i = 0; i < nodes.Count; i++) {
                var n = nodes[i];
                Gizmos.DrawLine((f3)n.a, (f3)n.b);
                Gizmos.DrawLine((f3)n.b, (f3)n.c);
                Gizmos.DrawLine((f3)n.c, (f3)n.a);
                if (showNormal) {
                    Gizmos.DrawLine(
                        (f3)n.t.GetGravityCenter(),
                        (f3)n.t.GetGravityCenter() + (f3)n.normal * 0.5f
                    );
                }
            }
        }

        for (int i = 0; i < nodes.Count; i++) {
            var n = nodes[i];
            if (i == currId && n.neighbors.Count > 3) {
                Gizmos.color = Color.cyan;
                n.neighbors.ForEach(nei => {
                    Gizmos.DrawLine((f3)nei.a, (f3)nei.b);
                    Gizmos.DrawLine((f3)nei.b, (f3)nei.c);
                    Gizmos.DrawLine((f3)nei.c, (f3)nei.a);
                });
                Gizmos.color = Color.red;
                Gizmos.DrawLine((f3)n.a, (f3)n.b);
                Gizmos.DrawLine((f3)n.b, (f3)n.c);
                Gizmos.DrawLine((f3)n.c, (f3)n.a);
            }
        }
    }

}
