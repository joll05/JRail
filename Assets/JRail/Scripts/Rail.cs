using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Rail : MonoBehaviour
{
    [HideInInspector]
    public RailPoint[] points = new RailPoint[0];

    [HideInInspector]
    public RailPoint startPoint = null;

    public RailPoint AddPoint()
    {
        RailPoint newPoint = ScriptableObject.CreateInstance<RailPoint>();

        // If this is the first point, make it the start point
        if(points.Length == 0) startPoint = newPoint;

        // Add the new point to the end of the array
        Array.Resize(ref points, points.Length + 1);
        points[points.Length - 1] = newPoint;

        return newPoint;
    }

    public void RemovePoint(RailPoint point)
    {
        points = points.Where(val => val != point).ToArray();
    }
}
