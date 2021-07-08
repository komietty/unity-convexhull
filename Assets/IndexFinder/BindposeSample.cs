using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class BindposeSample : MonoBehaviour {
    protected SkinnedMeshRenderer skin;
    protected Transform[] bones;
    protected Mesh mesh;
    [SerializeField] protected bool showTpose;
    [SerializeField] protected int tid;
    internal Vector3 vrt;
    internal Vector3 nrm;

    void Start() {
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        this.bones = skin.bones;
        this.mesh = skin.sharedMesh;
        var vtcs = new List<Vector3>();
        var tris = new List<int>();
        mesh.GetVertices(vtcs);
        mesh.GetTriangles(tris, 0);

        // get bindpose position
        if (showTpose) {
            var bindposes = mesh.bindposes;
            for (var i = 0; i < bones.Length; i++) {
                var p = bindposes[i].inverse * new Vector4(0, 0, 0, 1);
                var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.localScale *= 0.03f;
                g.transform.position = p;
            }
        }
    }

    void Update() {
        var vtcs = new List<Vector3>();
        var nrms = new List<Vector3>();
        var tris = new List<int>();

        mesh.GetNormals(nrms);
        mesh.GetVertices(vtcs);
        mesh.GetTriangles(tris, 0);

        int vid = tris[tid * 3];
        var _vrt = vtcs[vid];
        var _nrm = nrms[vid];
        var _mtx = GetMatrixFromCompact(vid);
        //var _mtx = GetMatrixFromUniform(vid);

        vrt = _mtx.MultiplyPoint3x4(_vrt);
        nrm = _mtx.MultiplyVector(_nrm);
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(vrt, 0.05f * Vector3.one);
        Gizmos.DrawRay(vrt, nrm);
    }

    Matrix4x4 GetMatrixFromCompact(int vid) {
        var bspv = mesh.GetBonesPerVertex();
        var wgts = mesh.GetAllBoneWeights();
        var bwid = 0;
        var bindposes = mesh.bindposes;
        var mtx = new float4x4();
        for (var vi = 0; vi < mesh.vertexCount; vi++) {
            for (var i = 0; i < bspv[vi]; i++) {
                if (vi == vid) {
                    var bw = wgts[bwid];
                    var bi = bw.boneIndex;
                    var mx = (float4x4)(bones[bi].localToWorldMatrix * bindposes[i]);
                    mtx += mx * bw.weight;
                }
                bwid++;
            }
        }
        return mtx;
    }

    Matrix4x4 GetMatrixFromUniform(int vid) { 
        var boneMatrices = new float4x4[skin.bones.Length];
        for (int i = 0; i < boneMatrices.Length; i++) 
            boneMatrices[i] = skin.bones[i].localToWorldMatrix * mesh.bindposes[i];

        var weight = mesh.boneWeights[vid];
        var bm0 = boneMatrices[weight.boneIndex0];
        var bm1 = boneMatrices[weight.boneIndex1];
        var bm2 = boneMatrices[weight.boneIndex2];
        var bm3 = boneMatrices[weight.boneIndex3];
        var mtx = bm0 * weight.weight0 +
                  bm1 * weight.weight1 +
                  bm2 * weight.weight2 +
                  bm3 * weight.weight3;
        return mtx;
    }
}
