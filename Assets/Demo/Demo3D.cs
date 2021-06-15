using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d3.animatedquickhull {
    using f3 = Unity.Mathematics.float3;
    using V3 = Vector3;

    public class Demo3D : MonoBehaviour {
        [SerializeField] protected Material mat;
        [SerializeField, Range(1, 20)] protected int itr = 15;
        [SerializeField, Range(0, 1)] protected float dstThreshold = 0.3f;
        [SerializeField, Range(0, 1)] protected float wgtThreshold = 0.3f;
        [SerializeField, Range(0, 1)] protected float speed = 1;
        protected AnimatedQuickhull3D aqh;
        protected Animator anim;
        protected SkinnedMeshRenderer skin;
        protected Convex cvx;
        protected GUIStyle style;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            anim = GetComponent<Animator>();
            aqh = new AnimatedQuickhull3D(skin, wgtThreshold);
            style = new GUIStyle();
            style.fontSize = 20;

        }

        void Update() {
            transform.rotation = Quaternion.AngleAxis(0.3f, V3.up) * transform.rotation; 
            anim.speed = speed;
            aqh.Execute(dstThreshold, itr);
            Graphics.DrawMesh(aqh.CreateMesh(), V3.left * 1.5f, Quaternion.identity, mat, 0);
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
