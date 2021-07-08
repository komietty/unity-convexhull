using UnityEngine;

namespace kmty.geom.d3.animatedquickhull {
    using f3 = Unity.Mathematics.float3;
    using V3 = Vector3;

    public class Demo3D_Aprox: MonoBehaviour {
        [SerializeField] protected Material mat;
        [SerializeField] protected Material dbg;
        [SerializeField, Range(1, 20)] protected int itr = 15;
        [SerializeField, Range(0, 1)] protected float dstThreshold = 0.3f;
        [SerializeField, Range(0, 1)] protected float wgtThreshold = 0.3f;
        [SerializeField, Range(0, 1)] protected float speed = 1;
        protected AnimatedQuickhull3D aqh;
        protected Animator anim;
        protected SkinnedMeshRenderer skin;
        protected Convex cvx;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            anim = GetComponent<Animator>();
            aqh = new AnimatedQuickhull3DAprox(skin, wgtThreshold, dstThreshold);
        }

        void Update() {
            transform.rotation = Quaternion.AngleAxis(0.3f, V3.up) * transform.rotation; 
            anim.speed = speed;
            aqh.Execute(itr);
            Graphics.DrawMesh(aqh.CreateMesh(), V3.left * 1.5f, Quaternion.identity, mat, 0);
        }

        void OnRenderObject() {
            dbg.SetPass(0);
            GL.PushMatrix();
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
                    (f3)n.t.GetGravityCenter() + (f3)n.normal * 0.2f
                );
            }
        }
    }
}
