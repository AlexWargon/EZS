using System.Linq;
using UnityEditor;
using UnityEngine;

public class Graph
{
    private readonly Vector3[] _cachedLinePointVerticies;
    private readonly GUIStyle _centeredStyle;
    private readonly GUIStyle _labelTextStyle;
    private readonly Vector3[] _linePoints;
    private float anchorRadius = 1f;
    private string axisFormat = "{0:0.0}";
    private float axisRounding = 1f;
    private int gridLines = 1;
    private string labelFormat = "{0:0.0}";
    private Color lineColor = new Color(0f, 1f, 0.5f);
    private int rightLinePadding = -15;
    private readonly float xBorder = 48f;
    private readonly float yBorder = 20f;

    public Graph(int dataLength)
    {
        _labelTextStyle = new GUIStyle(GUI.skin.label);
        _labelTextStyle.alignment = TextAnchor.UpperRight;
        _centeredStyle = new GUIStyle();
        _centeredStyle.alignment = TextAnchor.UpperCenter;
        _centeredStyle.normal.textColor = Color.white;
        _linePoints = new Vector3[dataLength];
        _cachedLinePointVerticies = new Vector3[4]
        {
            new Vector3(-1f, 1f, 0.0f) * anchorRadius,
            new Vector3(1f, 1f, 0.0f) * anchorRadius,
            new Vector3(1f, -1f, 0.0f) * anchorRadius,
            new Vector3(-1f, -1f, 0.0f) * anchorRadius
        };
    }

    public void Draw(float[] data, float height)
    {
        var rect = GUILayoutUtility.GetRect(EditorGUILayout.GetControlRect().width, height);
        var top = rect.y + yBorder;
        var floor = rect.y + rect.height - yBorder;
        var availableHeight = floor - top;
        var max = data.Length != 0 ? data.Max() : 0.0f;
        if (max % (double) axisRounding != 0.0)
            max = (float) (max + (double) axisRounding - max % (double) axisRounding);
        DrawGridLines(top, rect.width, availableHeight, max);
        DrawAvg(data, top, floor, rect.width, availableHeight, max);
        DrawLine(data, floor, rect.width, availableHeight, max);
    }

    private void DrawGridLines(float top, float width, float availableHeight, float max)
    {
        var color = Handles.color;
        Handles.color = Color.grey;
        var num1 = gridLines + 1;
        var num2 = availableHeight / num1;
        for (var index = 0; index <= num1; ++index)
        {
            var y = top + num2 * index;
            Handles.DrawLine(new Vector2(xBorder, y), new Vector2(width - rightLinePadding, y));
            GUI.Label(new Rect(0.0f, y - 8f, xBorder - 2f, 50f),
                string.Format(axisFormat, (float) (max * (1.0 - index / (double) num1))), _labelTextStyle);
        }

        Handles.color = color;
    }

    private void DrawAvg(float[] data, float top, float floor, float width, float availableHeight, float max)
    {
        var color = Handles.color;
        Handles.color = Color.yellow;
        var num = data.Average();
        var y = floor - availableHeight * (num / max);
        Handles.DrawLine(new Vector2(xBorder, y), new Vector2(width - rightLinePadding, y));
        Handles.color = color;
    }

    private void DrawLine(float[] data, float floor, float width, float availableHeight, float max)
    {
        var num1 = (width - xBorder - rightLinePadding) / data.Length;
        var color = Handles.color;
        var position = new Rect();
        var flag = false;
        var num2 = 0.0f;
        Handles.color = lineColor;
        Handles.matrix = Matrix4x4.identity;
        HandleUtility.handleMaterial.SetPass(0);
        for (var index = 0; index < data.Length; ++index)
        {
            var num3 = data[index];
            var y = floor - availableHeight * (num3 / max);
            var vector2_1 = new Vector2(xBorder + num1 * index, y);
            _linePoints[index] = new Vector3(vector2_1.x, vector2_1.y, 0.0f);
            var num4 = 1f;
            if (!flag)
            {
                var num5 = anchorRadius * 3f;
                var num6 = anchorRadius * 6f;
                var vector2_2 = vector2_1 - Vector2.up * 0.5f;
                position = new Rect(vector2_2.x - num5, vector2_2.y - num5, num6, num6);
                if (position.Contains(Event.current.mousePosition))
                {
                    flag = true;
                    num2 = num3;
                    num4 = 3f;
                }
            }

            Handles.matrix = Matrix4x4.TRS(_linePoints[index], Quaternion.identity, Vector3.one * num4);
            Handles.DrawAAConvexPolygon(_cachedLinePointVerticies);
        }

        Handles.matrix = Matrix4x4.identity;
        Handles.DrawAAPolyLine(3f, data.Length, _linePoints);
        if (flag)
        {
            position.y -= 16f;
            position.width += 50f;
            position.x -= 25f;
            GUI.Label(position, string.Format(labelFormat, num2), _centeredStyle);
        }

        Handles.color = color;
    }
}