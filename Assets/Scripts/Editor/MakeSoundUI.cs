using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MakeSound))]
public class MakeSoundUI : Editor
{
    private void OnSceneGUI()
    {
        MakeSound sound = (MakeSound)target;

        Handles.color = Color.blue;
        Handles.DrawWireArc(sound.transform.position, Vector3.up, Vector3.forward, 360, sound.radius);
    }
}
