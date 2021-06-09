using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kmty.geom.d3.animatedquickhull {
    public class Demo3D : MonoBehaviour {
        [SerializeField] protected Material mat;
        protected AnimatedQuickhull3D aqh;
        protected SkinnedMeshRenderer skin;
        protected Convex cvx;
        protected GUIStyle style;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            aqh = new AnimatedQuickhull3D(skin);
            style = new GUIStyle();
            style.fontSize = 20;
        }

        void Update() {
            aqh.Execute();
            aqh.CreateMesh();
            Graphics.DrawMesh(aqh.mesh, Vector3.zero, Quaternion.identity, mat, 0);
        }

        void OnRenderObject() {
            GL.PushMatrix();
            GL.Color(Color.white);
            aqh.convex.Draw();
            GL.PopMatrix();
        }
    }
}
