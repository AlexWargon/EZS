﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    [CustomEditor(typeof(SystemsDebugMono))]
    public class SystemsDebugInspector : Editor
    {
        private const int SYSTEM_MONITOR_DATA_LENGTH = 50;
        private static bool showSystemsMonitor = true;
        private int lastRenderedFrameCount;
        private GUIContent pauseButtonContent;
        private bool selected;

        private Queue<float> systemMonitorData;
        private SystemsDebug SystemsDebug;
        private SystemsDebugMono systemsDebugMono;
        private Graph systemsMonitor;
        private SystemView[] systemViews;


        private void Awake()
        {
            DebugUpdate.Redraw += UpdateNameTime;
        }

        private void OnEnable()
        {
            systemsDebugMono = (SystemsDebugMono) target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            DrawSystemsMonitor(systemsDebugMono.Systems);
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void UpdateNameTime()
        {
            systemsDebugMono.gameObject.name = $"ECS Systems Debug {systemsDebugMono.Systems.executeTime: 0.000} ms";
        }

        private void DrawSystemsMonitor(SystemsDebug systemsDebug)
        {
            if (systemsMonitor == null)
            {
                systemsMonitor = new Graph(SYSTEM_MONITOR_DATA_LENGTH);
                systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
            }

            if (systemViews == null) systemViews = new SystemView[systemsDebug.Systems.updateSystemsList.Count];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Execution duration", $"{systemsDebug.executeTime: 0.000} ms");

            EditorGUILayout.EndVertical();

            if (pauseButtonContent == null)
                pauseButtonContent = EditorGUIUtility.IconContent("PauseButton On");

            systemsDebug.Systems.Alive = GUILayout.Toggle(systemsDebug.Systems.Alive, pauseButtonContent, "CommandLeft");

            EditorGUILayout.EndHorizontal();
            if (!EditorApplication.isPaused)
                AddDuration((float) systemsDebug.executeTime);
            systemsMonitor.Draw(systemMonitorData.ToArray(), 120f);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField($"Systems: [{systemsDebug.Systems.updateSystemsList.Count}]");
            EditorGUILayout.LabelField("time ms:  | max time ms:");
            EditorGUILayout.EndHorizontal();

            //var iJobSystemInteraceTag = typeof(IJobSystemTag);
            var burstSystemsStyle = new GUIStyle(EditorStyles.textField);
            burstSystemsStyle.normal.textColor = new Color(1f, 0.46f, 0f);

            for (var i = 0; i < systemViews.Length; i++)
            {
                var system = systemViews[i];
                var systemType = systemsDebug.Systems.updateSystemsList[i].GetType();
                system.name = systemType.Name;
                system.time = systemsDebug.executeTimes[i];
                
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                if (systemType.IsDefined(typeof(SystemColorAttribute), false)) {
                    DrawSystemWithColor(systemType, system);
                }
                else
                {
                    EditorGUILayout.LabelField(system.name);
                    EditorGUILayout.LabelField($"{(system.time > 0.002 ? system.time : 0.000): 0.000} ms");
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawSystemWithColor(Type systemType, SystemView system) {
            var atribute = (SystemColorAttribute) Attribute.GetCustomAttribute(systemType, typeof(SystemColorAttribute));
            var style = new GUIStyle(EditorStyles.textField);
            style.normal.textColor = atribute.color;
            EditorGUILayout.LabelField(system.name, style);
            EditorGUILayout.LabelField($"{(system.time > 0.002 ? system.time : 0.000): 0.000} ms", style);
        }

        private SystemView[] SortByTime(SystemView[] array)
        {
            Array.Sort(array, (x, y) => y.time.CompareTo(x.time));
            return array;
        }

        private SystemView[] SortByName(SystemView[] array)
        {
            Array.Sort(array, (x, y) => string.Compare(y.name, x.name, StringComparison.Ordinal));
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

        private struct SystemView
        {
            public double time;
            public double maxTime;
            public string name;
        }
    }
}