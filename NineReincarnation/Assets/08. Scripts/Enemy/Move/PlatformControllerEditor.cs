using UnityEngine;
using UnityEditor;
using static Enemy.Move.EnemyMove;

namespace Enemy.Move
{
    [CustomEditor(typeof(EnemyMove))]
    public class PlatformControllerEditor : Editor
    {
        EnemyMove platformController;

        SelectionInfo selectionInfo;

        private bool needsRepaint = false;
        private GUIStyle style = new GUIStyle();

        private void OnEnable()
        {
            platformController = target as EnemyMove;
            selectionInfo = new SelectionInfo();
            style.normal.textColor = Color.black;
        }
        private void OnSceneGUI()
        {
            if (platformController.Editing)
            {
                HandleEvents();
                HandleUI();
            }
        }

        private void HandleUI()
        {
            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(10, 10, 200, 70));
                {
                    if (Event.current.modifiers == EventModifiers.Control)
                        GUILayout.Label("Removing points", style);
                    else
                        GUILayout.Label("Adding points", style);
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }

        private void HandleEvents()
        {
            Event guiEvent = Event.current;
            switch (guiEvent.type)
            {
                case EventType.Repaint:
                    Draw();
                    break;
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    break;
                default:
                    HandleInput(guiEvent);

                    if (needsRepaint)
                    {
                        HandleUtility.Repaint();
                        needsRepaint = false;
                    }
                    break;
            }
        }

        private void Draw()
        {
            switch (platformController.PathType)
            {
                case WaypointPathType.Circle:
                    DrawCicle();
                    break;
                case WaypointPathType.Ellipse:
                    DrawElipse();
                    break;
                case WaypointPathType.LineClosed:
                    DrawLineClosed();
                    break;
                case WaypointPathType.LineOpen:
                    DrawLineOpen();
                    break;
            }

            needsRepaint = false;
        }

        private void DrawElipse()
        {
            Vector3 centerPosition = platformController.transform.position - new Vector3(0, platformController.ElipseRadiusY, 0);

            Matrix4x4 m = Matrix4x4.TRS(
                centerPosition,          // 위치
                Quaternion.identity,     // 회전
                new Vector3(platformController.ElipseRadiusX, platformController.ElipseRadiusY, 1f) // X·Y만 스케일
            );

            using (new Handles.DrawingScope(Color.red, m))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1f);
            }
        }

        private void DrawCicle()
        {
            Handles.color = Color.red;
            Vector3 centerPosition = platformController.transform.position - new Vector3(0, platformController.CircleRadius, 0);
            Handles.DrawWireDisc(centerPosition,   // 중심
                      Vector3.back,                // 평면 법선
                      platformController.CircleRadius);
        }

        private void DrawLineOpen()
        {
            for (int i = 0; i < platformController.Waypoints.Count; i++)
            {
                Vector3 nextPoint = platformController.Waypoints[(i + 1) % platformController.Waypoints.Count];

                if (i != platformController.Waypoints.Count - 1)
                {
                    // Draw Edges
                    if (i == selectionInfo.lineIndex)
                    {
                        Handles.color = Color.red;
                        Handles.DrawLine(platformController.Waypoints[i], nextPoint, 1f);
                    }
                    else
                    {
                        Handles.color = Color.black;
                        Handles.DrawDottedLine(platformController.Waypoints[i], nextPoint, 1f);
                    }
                }

                // Draw Points
                if (i == selectionInfo.pointIndex)
                {
                    Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
                }
                else
                {
                    Handles.color = Color.white;
                }

                Handles.DrawSolidDisc(platformController.Waypoints[i], Vector3.back, platformController.HandleRadius);

                Handles.color = Color.black;
                Handles.Label(platformController.Waypoints[i], i.ToString(), style);
            }
        }

        private void DrawLineClosed()
        {
            for (int i = 0; i < platformController.Waypoints.Count; i++)
            {
                Vector3 nextPoint = platformController.Waypoints[(i + 1) % platformController.Waypoints.Count];

                // Draw Edges
                if (i == selectionInfo.lineIndex)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(platformController.Waypoints[i], nextPoint, 1f);
                }
                else
                {
                    Handles.color = Color.black;
                    Handles.DrawDottedLine(platformController.Waypoints[i], nextPoint, 1f);
                }

                // Draw Points
                if (i == selectionInfo.pointIndex)
                {
                    Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
                }
                else
                {
                    Handles.color = Color.white;
                }

                Handles.DrawSolidDisc(platformController.Waypoints[i], Vector3.back, platformController.HandleRadius);

                Handles.color = Color.black;
                Handles.Label(platformController.Waypoints[i], i.ToString(), style);
            }
        }

        void HandleInput(Event guiEvent)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float dstToDrawPlane = (0 - mouseRay.origin.z) / mouseRay.direction.z;
            Vector3 mousePosition = SnapToGrid(mouseRay.GetPoint(dstToDrawPlane));

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Control)
            {
                HandleLeftMouseDownDelete(mousePosition);
                return;
            }

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDown(mousePosition);
            }

            if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseUp(mousePosition);
            }

            if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDrag(mousePosition);
            }

            if (!selectionInfo.pointIsSelected)
            {
                UpdateMouseOverInfo(mousePosition);
            }
        }

        void HandleLeftMouseDownDelete(Vector3 mousePosition)
        {
            Undo.RecordObject(platformController, "Remove waypoint");
            platformController.Waypoints.RemoveAt(selectionInfo.pointIndex);
            selectionInfo.pointIndex = -1;
            needsRepaint = true;
        }

        void HandleLeftMouseDown(Vector3 mousePosition)
        {
            if (!selectionInfo.mouseIsOverPoint)
            {
                int newPointIndex = (selectionInfo.mouseIsOverLine) ? selectionInfo.lineIndex + 1 : platformController.Waypoints.Count;
                Undo.RecordObject(platformController, "Add waypoint");
                platformController.Waypoints.Insert(newPointIndex, mousePosition);
                selectionInfo.pointIndex = newPointIndex;
            }

            selectionInfo.pointIsSelected = true;
            selectionInfo.positionAtStartOfDrag = mousePosition;
            needsRepaint = true;
        }

        void HandleLeftMouseUp(Vector3 mousePosition)
        {
            if (selectionInfo.pointIsSelected)
            {
                platformController.Waypoints[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
                Undo.RecordObject(platformController, "Move point");
                platformController.Waypoints[selectionInfo.pointIndex] = mousePosition;

                selectionInfo.pointIsSelected = false;
                selectionInfo.pointIndex = -1;
                needsRepaint = true;
            }
        }

        void HandleLeftMouseDrag(Vector3 mousePosition)
        {
            if (selectionInfo.pointIsSelected)
            {
                platformController.Waypoints[selectionInfo.pointIndex] = mousePosition;
                needsRepaint = true;
            }
        }

        void UpdateMouseOverInfo(Vector3 currMousePosition)
        {
            int mouseOverPointIndex = -1;
            for (int i = 0; i < platformController.Waypoints.Count; i++)
            {
                if (Vector3.Distance(currMousePosition, platformController.Waypoints[i]) < platformController.HandleRadius)
                {
                    mouseOverPointIndex = i;
                    break;
                }
            }

            if (mouseOverPointIndex != selectionInfo.pointIndex)
            {
                selectionInfo.pointIndex = mouseOverPointIndex;
                selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

                needsRepaint = true;
            }

            if (selectionInfo.mouseIsOverPoint)
            {
                selectionInfo.mouseIsOverLine = false;
                selectionInfo.lineIndex = -1;
            }
            else
            {
                int mouseOverLineIndex = -1;
                float closestLineDistance = platformController.HandleRadius;
                for (int i = 0; i < platformController.Waypoints.Count; i++)
                {
                    Vector3 nextPointInShape = platformController.Waypoints[(i + 1) % platformController.Waypoints.Count];
                    float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(currMousePosition, platformController.Waypoints[i], nextPointInShape);
                    if (dstFromMouseToLine < closestLineDistance)
                    {
                        closestLineDistance = dstFromMouseToLine;
                        mouseOverLineIndex = i;
                    }
                }

                if (selectionInfo.lineIndex != mouseOverLineIndex)
                {
                    selectionInfo.lineIndex = mouseOverLineIndex;
                    selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                    needsRepaint = true;
                }
            }
        }

        Vector3 SnapToGrid(Vector3 position)
        {
            float snappedX = Mathf.Round(position.x / platformController.SnappingSettings.x) * platformController.SnappingSettings.x;
            float snappedY = Mathf.Round(position.y / platformController.SnappingSettings.y) * platformController.SnappingSettings.y;

            return new Vector3(snappedX, snappedY, position.z);
        }

        public void DrawText(string text, Vector3 worldPos, Vector2 screenOffset = default, Color? color = default, int alignment = 0)
        {
            Handles.BeginGUI();

            var restoreColor = GUI.color;
            if (color.HasValue) GUI.color = color.Value;

            var view = UnityEditor.SceneView.currentDrawingSceneView;
            var screenPos = view.camera.WorldToScreenPoint(worldPos);
            screenPos += new Vector3(screenOffset.x, screenOffset.y, 0f);

            if (screenPos.y < 0f || screenPos.y > Screen.height || screenPos.x < 0f || screenPos.x > Screen.width || screenPos.z < 0f)
            {
                GUI.color = restoreColor;

            }
            else
            {
                var size = GUI.skin.label.CalcSize(new GUIContent(text));

                if (alignment == 0)
                {
                    screenPos.x -= (size.x / 2f) + 1f;
                }
                else if (alignment < 0)
                {
                    screenPos.x -= size.x - 2f;
                }
                else
                {
                    screenPos.x -= 4f;
                }

                GUI.Label(new Rect(screenPos.x, -screenPos.y + view.position.height + 4f, size.x, size.y), text);
                GUI.color = restoreColor;

            }

            Handles.EndGUI();
        }

        public class SelectionInfo
        {
            public int pointIndex = -1;
            public bool mouseIsOverPoint = false;
            public bool pointIsSelected = false;
            public Vector3 positionAtStartOfDrag;

            public int lineIndex = -1;
            public bool mouseIsOverLine = false;
        }
    }
}