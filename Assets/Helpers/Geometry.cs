using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Geometry
{
 
    public static Vector3? Intersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        if (doIntersect(p1, p2, p3, p4))
        {
            return Vector3.zero;
        }
        else return null;
    }

    public static Vector3? IntersectsWithItself(List<Vector3> path)
    {
        var arr = path.ToArray();

        
        for (int i = 1; i < arr.Length; i++)
        {
            for (int j = 1; j < arr.Length; j++) {
                if (i == j) continue;
                if (Mathf.Abs(j - i) <= 1) continue;
                var intersection = Intersection(arr[i - 1], arr[i], arr[j - 1], arr[j]);

                if (intersection.HasValue) return intersection;
            }
        }
        return null;
    }

    public static Vector3? Intersects(List<Vector3> path1, List<Vector3> path2)
    {
        var arr1 = path1.ToArray();
        var arr2 = path2.ToArray();


        for (int i = 1; i < arr1.Length; i++)
        {
            for (int j = 1; j < arr2.Length; j++)
            {
                if (i == j) continue;
                if (Mathf.Abs(j - i) <= 1) continue;
                var intersection = Intersection(arr1[i - 1], arr1[i], arr2[j - 1], arr2[j]);

                if (intersection.HasValue) return intersection;
            }
        }
        return null;
    }


    internal static bool IntersectWithAny(List<Vector3> path, List<List<Vector3>> paths)
    {
        foreach (var other_path in paths)
        {
            return Intersects(path, other_path).HasValue;
        }

        return false;
    }

    static bool ccw(Vector3 A, Vector3 B, Vector3 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }
    // Return true if line segments AB and CD intersect
    static bool intersect(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        return ccw(A, C, D) != ccw(B, C, D) && ccw(A, B, C) != ccw(A, B, D);
    }



    static bool onSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;

        return false;
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are colinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    static int orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0; // colinear

        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    // The main function that returns true if line segment 'p1q1'
    // and 'p2q2' intersect.
    static bool doIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        // Find the four orientations needed for general and
        // special cases
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1
        if (o1 == 0 && onSegment(p1, p2, q1)) return true;

        // p1, q1 and q2 are colinear and q2 lies on segment p1q1
        if (o2 == 0 && onSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2
        if (o3 == 0 && onSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2
        if (o4 == 0 && onSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases
    }

    internal static Vector2 PointInsidePolygon(List<Vector2> polygon, Collider2D col)
    {
        var i = 0;
        for (i = 0; i < 500; i++)
        {
            var bounds = col.bounds;
        
            var p  = new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
            );
            if (isInside(polygon.ToArray(), polygon.Count(), p))
            {
                return p;
            }
        }
        Debug.Log(i);
        return polygon[UnityEngine.Random.Range(0, polygon.Count())];
    }

    internal static Vector2 PointNearEdgeOfPolygon(List<Vector2> polygon)
    {
        var n = polygon.Count();
        var center_x = polygon.Select(i => i.x).Sum() / n;
        var center_y = polygon.Select(i => i.y).Sum() / n;

        var center = new Vector2(center_x, center_y);
        var point = polygon[UnityEngine.Random.Range(0, n)];

        //Debug.Log($"p: {point}, center: {center}");
        return center + (point - center) * 0.9f;
    }

    static bool isInside(Vector2[] polygon, int n, Vector2 p)
    {
        // There must be at least 3 vertices in polygon[]
        if (n < 3)
        {
            return false;
        }

        // Create a point for line segment from p to infinite
        Vector2 extreme = new Vector2(Mathf.Infinity, p.y);

        // Count intersections of the above line
        // with sides of polygon
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;

            // Check if the line segment from 'p' to
            // 'extreme' intersects with the line
            // segment from 'polygon[i]' to 'polygon[next]'
            if (doIntersect(polygon[i],
                            polygon[next], p, extreme))
            {
                // If the point 'p' is colinear with line
                // segment 'i-next', then check if it lies
                // on segment. If it lies, return true, otherwise false
                if (orientation(polygon[i], p, polygon[next]) == 0)
                {
                    return onSegment(polygon[i], p,
                                    polygon[next]);
                }
                count++;
            }
            i = next;
        } while (i != 0);

        // Return true if count is odd, false otherwise
        return (count % 2 == 1); // Same as (count%2 == 1)
    }
}
