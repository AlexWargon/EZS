using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SystemsDebugMono))]
public class SystemsDebugEditor : Editor
{
    private const int SYSTEM_MONITOR_DATA_LENGTH = 80;
    private static bool showSystemsMonitor = true;
    private int lastRenderedFrameCount;
    private GUIContent pauseButtonContent;

    private Queue<float> systemMonitorData;
    private Graph systemsMonitor;
    private SystemView[] systemViews;
    public override void OnInspectorGUI()
    {
        var systems = (SystemsDebugMono) target;
        base.OnInspectorGUI();
        DrawSystemsMonitor(systems.Systems);
        EditorUtility.SetDirty(target);
    }

    private void DrawSystemsMonitor(SystemsDebug systems)
    {
        if (systemsMonitor == null)
        {
            systemsMonitor = new Graph(SYSTEM_MONITOR_DATA_LENGTH);
            systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
        }

        if (systemViews == null)
        {
            systemViews = new SystemView[systems.Systems.updateSystemsList.Count];
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Execution duration", $"{systems.executeTime : 0.00} ms");
        
        EditorGUILayout.EndVertical();

        if (pauseButtonContent == null)
            pauseButtonContent = EditorGUIUtility.IconContent("PauseButton On");
        
        systems.Systems.Alive = GUILayout.Toggle(systems.Systems.Alive, pauseButtonContent, (GUIStyle) "CommandLeft");

        EditorGUILayout.EndHorizontal();
        if (!EditorApplication.isPaused)
            AddDuration((float) systems.executeTime);
        systemsMonitor.Draw(systemMonitorData.ToArray(), 120f);
        for (var i = 0; i < systemViews.Length; i++)
        {
            systemViews[i].name = systems.Systems.updateSystemsList[i].GetType().Name;
            systemViews[i].time = systems.executeTimes[i];
            if (systemViews[i].time > systemViews[i].maxTime)
                systemViews[i].maxTime = systemViews[i].time;
        }
        //SortByTime(systemViews);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField("Systems:");
        EditorGUILayout.LabelField("time ms:    | max time ms:");
        EditorGUILayout.EndHorizontal();
        for (var i = 0; i < systemViews.Length; i++)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField(systemViews[i].name);
            EditorGUILayout.LabelField($"{systemViews[i].time : 0.00} ms|{systemViews[i].maxTime : 0.00} ms");
            EditorGUILayout.EndHorizontal();
        }
    }

    private SystemView[] SortByTime(SystemView[] array)
    {
        Array.Sort(array, (x,y) => y.time.CompareTo(x.time));
        return array;
    }
    private SystemView[] SortByName(SystemView[] array)
    {
        Array.Sort(array, (x,y) => String.Compare(y.name, x.name, StringComparison.Ordinal));
        return array;
    }
    private void AddDuration(float duration)
    {
        if (Time.renderedFrameCount == lastRenderedFrameCount)
            return;
        lastRenderedFrameCount = Time.renderedFrameCount;
        if (systemMonitorData.Count >= SYSTEM_MONITOR_DATA_LENGTH)
        {
            double num = systemMonitorData.Dequeue();
        }

        systemMonitorData.Enqueue(duration);
    }
    public struct SystemView
    {
        public double time;
        public double maxTime;
        public string name;
    }
}