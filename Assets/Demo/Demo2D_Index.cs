using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace kmty.geom.d2.animatedquickhull {
    using f2 = float2;

    public class Demo2D_Index : MonoBehaviour {
        [SerializeField] protected Material dbg;
        [SerializeField, Range(0, 1)] protected float speed = 1;
        [SerializeField] int[] triangleIndices;
        protected AnimatedQuickhull2D aqh;
        protected SkinnedMeshRenderer skin;
        protected Animator anim;
        protected Convex cvx;
        protected f2[] points;
        protected GUIStyle style;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            anim = GetComponent<Animator>();
            aqh = new AnimatedQuickhull2DIndex(skin, triangleIndices);
            style = new GUIStyle();
            style.fontSize = 20;
        }

        void Update() {
            aqh.Execute();
            anim.speed = speed;
            transform.rotation = Quaternion.AngleAxis(0.3f, Vector3.up) * transform.rotation; 
        }

        void OnGUI() {
            GUI.Label(new Rect(10, 10, 300, 30),  $"xmin:{aqh.convex.aabb.min.x}", style);
            GUI.Label(new Rect(10, 90, 300, 30),  $"ymin:{aqh.convex.aabb.min.y}", style);
            GUI.Label(new Rect(10, 50, 300, 30),  $"xmax:{aqh.convex.aabb.max.x}", style);
            GUI.Label(new Rect(10, 130, 300, 30), $"ymax:{aqh.convex.aabb.max.y}", style);
        }

        void OnRenderObject() {
            dbg.SetPass(0);
            GL.PushMatrix();
            aqh.convex.Draw();
            GL.PopMatrix();
        }
    }
}
