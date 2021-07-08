using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RaycastGetIndices))]
public class RaycastGetIndicesEditor : Editor {
    List<Vector3> vrts = new List<Vector3>();
    List<Vector3> nrms = new List<Vector3>();
    List<int> tris = new List<int>();

    void OnSceneGUI() {
        var t = (RaycastGetIndices)target;

        var f = t.filt;
        if (f != null) {
            var m = f.sharedMesh;
            m.GetVertices(vrts);
            m.GetNormals(nrms);
            m.GetTriangles(tris, 0);
        }

        Handles.color = Color.red;
        foreach (var tid in t.tids) {
            var v1 = vrts[tris[tid * 3 + 0]];
            var v2 = vrts[tris[tid * 3 + 1]];
            var v3 = vrts[tris[tid * 3 + 2]];
            var n1 = nrms[tris[tid * 3 + 0]];
            var n2 = nrms[tris[tid * 3 + 1]];
            var n3 = nrms[tris[tid * 3 + 2]];
            Handles.DrawLines(new Vector3[] {
                v1 + n1 * 0.0f,
                v2 + n2 * 0.0f,
                v2 + n2 * 0.0f,
                v3 + n3 * 0.0f,
                v3 + n3 * 0.0f,
                v1 + n1 * 0.0f
            });
        }

        var r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (!Physics.Raycast(r, out RaycastHit hit)) return;

        var c = (MeshCollider)hit.collider;
        if (c == null || c.sharedMesh == null) return;

        t.tid = hit.triangleIndex;



        var e = Event.current;
        var i = GUIUtility.GetControlID(FocusType.Passive);
        switch (e.GetTypeForControl(i)) {
            case EventType.MouseDown:
                GUIUtility.hotControl = i;
                if (e.button == 0) { t.OnClickMesh(); }
                e.Use();
                break;
        }

    }
}
