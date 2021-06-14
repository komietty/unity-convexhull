using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d3.animatedquickhull {
    using f3 = Unity.Mathematics.float3;
    using V3 = Vector3;

    public class Demo3D : MonoBehaviour {
        [SerializeField] protected Material mat;
        [SerializeField, Range(0, 1)] protected float speed;
        protected AnimatedQuickhull3D aqh;
        protected Animator anim;
        protected SkinnedMeshRenderer skin;
        protected Convex cvx;
        protected GUIStyle style;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            anim = GetComponent<Animator>();
            aqh = new AnimatedQuickhull3D(skin);
            style = new GUIStyle();
            style.fontSize = 20;

        }

        void Update() {
            anim.speed = speed;
            aqh.Execute();
            aqh.CreateMesh();
            Graphics.DrawMesh(aqh.mesh, V3.zero, Quaternion.identity, mat, 0);
        }

        void OnRenderObject() {
            GL.PushMatrix();
            GL.Color(Color.white);
            aqh.convex.Draw();
            GL.PopMatrix();
        }

        void OnDrawGizmos() {
            if (!Application.isPlaying) return;
            var nodes = aqh.convex.nodes;
            for (int i = 0; i < nodes.Count; i++) {
                var n = nodes[i];
                Gizmos.DrawLine(
                    (f3)n.t.GetGravityCenter(),
                    (f3)n.t.GetGravityCenter() + (f3)n.normal * 0.3f
                );
            }
        }
    }
}
