using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FOV))]
public class FOV_Editor : Editor
{
    private void OnSceneGUI()
    {
        FOV fow = (FOV)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, 'y', false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, 'z', true);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);
    }
}
