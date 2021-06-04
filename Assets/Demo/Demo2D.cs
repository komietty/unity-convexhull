using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace kmty.geom.d2.animatedquickhull {
    using UR = UnityEngine.Random;
    using f2 = float2;
    using f3 = float3;
    using SG = Segment;

    public class Demo2D : MonoBehaviour {
        protected AnimatedQuickhull2D aqh;
        protected SkinnedMeshRenderer skin;
        protected Convex cvx;
        protected f2[] points;
        protected GUIStyle style;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            aqh = new AnimatedQuickhull2D(skin);
            style = new GUIStyle();
            style.fontSize = 20;
        }

        void Update() {
            aqh.Execute();
        }

        void OnGUI() {
            GUI.Label(new Rect(10, 10, 300, 30),  $"xmin:{aqh.convex.aabb.Min.x}", style);
            GUI.Label(new Rect(10, 90, 300, 30),  $"ymin:{aqh.convex.aabb.Min.y}", style);
            GUI.Label(new Rect(10, 50, 300, 30),  $"xmax:{aqh.convex.aabb.Max.x}", style);
            GUI.Label(new Rect(10, 130, 300, 30), $"ymax:{aqh.convex.aabb.Max.y}", style);
        }

        void OnRenderObject() {
            GL.PushMatrix();
            GL.Color(Color.white);
            aqh.convex.Draw();
            GL.PopMatrix();
        }
    }
}
