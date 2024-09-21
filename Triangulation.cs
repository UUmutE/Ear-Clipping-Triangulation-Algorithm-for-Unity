using System.Collections.Generic;
using UnityEngine;

public class Triangulation
{
    public static List<int> Triangulate(List<Vector2> points)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < points.Count; i++)
            indices.Add(i);

        List<int> triangles = new List<int>();

        if (points.Count < 3)
            return triangles;

        // Determine if the polygon is clockwise or counter-clockwise
        bool isClockwise = IsClockwise(points);

        int counter = 0; // To prevent infinite loops
        while (indices.Count > 3 && counter < 1000)
        {
            counter++;
            bool earFound = false;

            for (int i = 0; i < indices.Count; i++)
            {
                int prevIndex = indices[(i - 1 + indices.Count) % indices.Count];
                int currIndex = indices[i];
                int nextIndex = indices[(i + 1) % indices.Count];

                Vector2 prevPoint = points[prevIndex];
                Vector2 currPoint = points[currIndex];
                Vector2 nextPoint = points[nextIndex];

                // Check if the triangle is convex
                if (IsConvex(prevPoint, currPoint, nextPoint, isClockwise))
                {
                    // Check if any other point is inside the triangle
                    bool hasPointInside = false;
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (j == prevIndex || j == currIndex || j == nextIndex)
                            continue;

                        if (PointInTriangle(points[j], prevPoint, currPoint, nextPoint))
                        {
                            hasPointInside = true;
                            break;
                        }
                    }

                    if (!hasPointInside)
                    {
                        // Ear found
                        triangles.Add(prevIndex);
                        triangles.Add(currIndex);
                        triangles.Add(nextIndex);

                        indices.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }
            }

            if (!earFound)
            {
                Debug.LogWarning("No ear found. Possible invalid polygon.");
                break;
            }
        }

        // Add the last triangle
        if (indices.Count == 3)
        {
            triangles.Add(indices[0]);
            triangles.Add(indices[1]);
            triangles.Add(indices[2]);
        }

        return triangles;
    }

    private static bool IsClockwise(List<Vector2> points)
    {
        float sum = 0;
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 a = points[i];
            Vector2 b = points[(i + 1) % points.Count];
            sum += (b.x - a.x) * (b.y + a.y);
        }
        return sum > 0;
    }

    private static bool IsConvex(Vector2 prev, Vector2 curr, Vector2 next, bool isClockwise)
    {
        float cross = CrossProduct(prev, curr, next);
        return isClockwise ? cross < 0 : cross > 0;
    }

    private static float CrossProduct(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    private static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        bool b1 = Sign(pt, v1, v2) < 0.0f;
        bool b2 = Sign(pt, v2, v3) < 0.0f;
        bool b3 = Sign(pt, v3, v1) < 0.0f;
        return (b1 == b2) && (b2 == b3);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
