using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class StageCameraPointRender : MonoBehaviour
{
    public StageCameraPoint startingPoint;
    private List<StageCameraPoint> stageCameraPoints;

    private const float orthoSize = 5.625f;
    private const float aspectRatio = 16f / 9f;

    private void OnEnable()
    {
        if (startingPoint == null) return;

        stageCameraPoints = new List<StageCameraPoint>();
        stageCameraPoints.Add(startingPoint);

        StageCameraPoint current = startingPoint;
        while (true)
        {
            current = current.next;
            if (current == null) break;
            else stageCameraPoints.Add(current);
        }
    }

    private void OnDrawGizmos()
    {
        if (stageCameraPoints == null || stageCameraPoints.Count == 0) return;

        Gizmos.color = Color.green;

        GUIStyle textstyle = new GUIStyle();
        textstyle.normal.textColor = Color.white;
        textstyle.alignment = TextAnchor.MiddleCenter;
        stageCameraPoints[0].time = 0;
        // Iterate through each StageCameraPoint
        for (int i = 0; i < stageCameraPoints.Count; i++)
        {
            stageCameraPoints[i].CalculateNextDistance();
            var point = stageCameraPoints[i];
            if (point == null) continue;

            // Draw the camera rect
            Vector2 cameraSize = new Vector2(orthoSize * 2 * aspectRatio, orthoSize * 2);
            Vector3 rectPosition = point.transform.position;

            Vector3[] corners = GetRectangleCorners(rectPosition, cameraSize);
            

            // Use GUI to draw the label
            //Handles.BeginGUI();
            
            Handles.Label(point.transform.position, GetFormattedTime(point), textstyle);
            //GUI.Label(new Rect(point.transform.position.x - 50, point.transform.position.y - 10, 100, 20), GetFormattedTime(point)); // Position adjusted to center text
            //Handles.EndGUI();

            // Draw lines to the next point's rectangle (avoiding internal lines)
            if (point.next != null)
            {
                Vector3 nextPosition = point.next.transform.position;
                Vector3[] nextCorners = GetRectangleCorners(nextPosition, cameraSize);

                // Draw only the outer connections
                DrawPathBetweenRectangles(corners, nextCorners, point.transform.position, point.next.transform.position);
            }
            DrawRectangleEdges(corners, point);
        }

    }

    private string GetFormattedTime(StageCameraPoint point)
    {
        float seconds = point.time;
        int minutes = (int)seconds / 60; // Get the number of minutes
        int remainingSeconds = (int)seconds % 60; // Get the remaining seconds

        // Format the result as "minutes:seconds"
        return string.Format("{0}:{1:D2}", minutes, remainingSeconds);
    }

    private Vector3[] GetRectangleCorners(Vector3 position, Vector2 size)
    {
        Vector3[] corners = new Vector3[4];
        corners[0] = position + new Vector3(-size.x / 2, size.y / 2, 0); // Top-left
        corners[1] = position + new Vector3(size.x / 2, size.y / 2, 0);  // Top-right
        corners[2] = position + new Vector3(size.x / 2, -size.y / 2, 0); // Bottom-right
        corners[3] = position + new Vector3(-size.x / 2, -size.y / 2, 0); // Bottom-left
        return corners;
    }

    private void DrawRectangleEdges(Vector3[] corners, StageCameraPoint point)
    {
        if (point.pointType == StagePointType.Anchor) Gizmos.color = Color.green;
        else if (point.pointType == StagePointType.Elite) Gizmos.color = Color.blue;
        else Gizmos.color = Color.red;
        // Only draw the outer edges of the rectangle
        Gizmos.DrawLine(corners[0], corners[1]); // Top-left to Top-right
        Gizmos.DrawLine(corners[1], corners[2]); // Top-right to Bottom-right
        Gizmos.DrawLine(corners[2], corners[3]); // Bottom-right to Bottom-left
        Gizmos.DrawLine(corners[3], corners[0]); // Bottom-left to Top-left
    }

    private void DrawPathBetweenRectangles(Vector3[] rectA, Vector3[] rectB, Vector2 from, Vector2 to)
    {
        Gizmos.color = Color.green;
        // Connect outer edges (Top, Right, Bottom, Left) between rectangles
        // Avoid drawing lines across the internal diagonal area.

        // Connect top edges
        if (rectA[0].x <= rectB[0].x && rectA[0].y <= rectB[0].y) Gizmos.DrawLine(rectA[0], rectB[0]); // Top-left to Top-left
        if (rectA[0].x >= rectB[0].x && rectA[0].y >= rectB[0].y) Gizmos.DrawLine(rectA[0], rectB[0]); // Top-left to Top-left

        if (rectA[1].x <= rectB[1].x && rectA[1].y >= rectB[1].y) Gizmos.DrawLine(rectA[1], rectB[1]); // Top-right to Top-right
        if (rectA[1].x >= rectB[1].x && rectA[1].y <= rectB[1].y) Gizmos.DrawLine(rectA[1], rectB[1]); // Top-right to Top-right

        // Connect bottom edges
        if (rectA[2].x <= rectB[2].x && rectA[2].y <= rectB[2].y) Gizmos.DrawLine(rectA[2], rectB[2]); // Bottom-right to Bottom-right
        if (rectA[2].x >= rectB[2].x && rectA[2].y >= rectB[2].y) Gizmos.DrawLine(rectA[2], rectB[2]); // Bottom-right to Bottom-right

        if (rectA[3].x <= rectB[3].x && rectA[3].y >= rectB[3].y) Gizmos.DrawLine(rectA[3], rectB[3]); // Bottom-left to Bottom-left
        if (rectA[3].x >= rectB[3].x && rectA[3].y <= rectB[3].y) Gizmos.DrawLine(rectA[3], rectB[3]); // Bottom-left to Bottom-left
    }
}