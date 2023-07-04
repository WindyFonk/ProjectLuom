using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyFOV))]
public class EnemyFOVEditor : Editor
{
    private void OnSceneGUI()
    {
        //Radius
        EnemyFOV fov = (EnemyFOV)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngle1 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle1 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle2 * fov.radius);

        if (fov.seePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.player.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegree)
    {
        angleInDegree += eulerY;
        return new Vector3(Mathf.Sin(angleInDegree*Mathf.Deg2Rad),0,Mathf.Cos(angleInDegree*Mathf.Deg2Rad));
    }
}
