using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastGetIndices : MonoBehaviour {
    public MeshFilter filt;
    public int tid = - 1;
    public List<int> tids;
    public void OnClickMesh() { tids.Add(tid); }
}
