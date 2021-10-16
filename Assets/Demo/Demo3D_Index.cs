using UnityEngine;

namespace kmty.geom.d3.animatedquickhull {
    using f3 = Unity.Mathematics.float3;
    using V3 = Vector3;

    public class Demo3D_Index: MonoBehaviour {
        [SerializeField] protected Material mat;
        [SerializeField] protected Material dbg;
        [SerializeField] protected bool createCollider;
        [SerializeField] protected bool showDebugMesh;
        [SerializeField, Range(1, 30)] protected int itr = 15;
        [SerializeField, Range(0, 1)] protected float speed = 1;
        [SerializeField] int[] triangleIndices;
        protected AnimatedQuickhull3D aqh;
        protected Animator anim;
        protected SkinnedMeshRenderer skin;
        protected Convex cvx;
        protected MeshCollider cld;

        void Start() {
            skin = GetComponentInChildren<SkinnedMeshRenderer>();
            anim = GetComponent<Animator>();
            aqh = new AnimatedQuickhull3DIndex(skin, triangleIndices);
            if (createCollider) { cld  = gameObject.AddComponent<MeshCollider>(); }
        }

        void Update() {
            //transform.rotation = Quaternion.AngleAxis(0.3f, V3.up) * transform.rotation; 
            anim.speed = speed;
            aqh.Execute(itr);
            var m = aqh.CreateMesh();
            cld.sharedMesh = m;
            if (showDebugMesh) Graphics.DrawMesh(m, V3.zero, Quaternion.identity, mat, 0);
        }

        void OnRenderObject() {
            dbg.SetPass(0);
            GL.PushMatrix();
            GL.Color(Color.green);
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
