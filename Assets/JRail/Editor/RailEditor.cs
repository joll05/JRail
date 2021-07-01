using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Rail))]
public class RailEditor : Editor
{
    Rail rail;
    Transform railTransform;

    RailPoint selectedPoint = null;
    
    /// <summary>
    /// Sets variables related to the target script.
    /// </summary>
    void GetRail()
    {
        rail = target as Rail;
        railTransform = rail.transform;
    }

    void OnSceneGUI()
    {
        GetRail();
       
        Vector3[] points = new Vector3[rail.points.Length];
        for (int i = 0; i < rail.points.Length; i++)
        {
            Vector2 point = ShowPointHandle(rail.points[i]);
            points[i] = point;
        }

        Handles.color = Color.white;
        Handles.DrawAAPolyLine(points);

        DrawGUI();
    }

    const float buttonSize = 0.05f;
    const float buttonPickSize = buttonSize * 1.5f;

    /// <summary>
    /// Show a position handle on the point and return the points position in world space.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    Vector2 ShowPointHandle(RailPoint point)
    {
        //Get the point position in world space by using TransformPoint
        Vector2 position = railTransform.TransformPoint(point.position);

        float handleSize = HandleUtility.GetHandleSize(position);

        //Set color based on if the point is the starting point
        Handles.color = point == rail.startPoint ? Color.green : Color.white;
        //Draw a button and check if it was pressed
        if(Handles.Button(position, Quaternion.identity, buttonSize * handleSize, buttonPickSize * handleSize, Handles.DotHandleCap))
        {
            selectedPoint = point;
        }

        if(selectedPoint == point)
        {
            //Start checking for changes
            EditorGUI.BeginChangeCheck();
            //Draw a position handle and if it has been moved set the position variable to the new position of the handle.
            position = Handles.DoPositionHandle(position, Quaternion.identity);
            //Finish checking for changes, and if there were changes set the new position and record the changes to the point object
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(point, "Move Point");
                EditorUtility.SetDirty(point);
                //Use InverseTransformPoint to set the point in local space
                point.position = railTransform.InverseTransformPoint(position);
            }
        }

        return position;
    }

    /// <summary>
    /// Draw GUI elements
    /// </summary>
    void DrawGUI()
    {
        Handles.BeginGUI();

        //Set button options to set dimensions
        GUILayoutOption[] buttonOptions = new GUILayoutOption[]
        {
            GUILayout.Width(30),
            GUILayout.Height(30)
        };

        //Set button style to set the font size
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
        };

        //Display add and remove buttons and add/remove a point when they are pressed

        if(GUILayout.Button(new GUIContent("+", "Add point"), buttonStyle, buttonOptions))
        {
            //Record changes to the rail object
            Undo.RecordObject(rail, "Add Point");
            EditorUtility.SetDirty(rail);

            RailPoint point = rail.AddPoint();
            selectedPoint = point;
        }

        if(GUILayout.Button(new GUIContent("-", "Remove selected point"), buttonStyle, buttonOptions))
        {
            if (selectedPoint != null)
            {
                //Record changes to the rail object
                Undo.RecordObject(rail, "Delete Point");
                EditorUtility.SetDirty(rail);

                rail.RemovePoint(selectedPoint);
            }
            else Debug.LogError("You must select a point to delete it!");
        }

        Handles.EndGUI();
    }
}
