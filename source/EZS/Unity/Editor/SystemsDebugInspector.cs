using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    [CustomEditor(typeof(SystemsDebugMono))]
    public class SystemsDebugInspector : Editor
    {
        private const int SYSTEM_MONITOR_DATA_LENGTH = 80;
        private static bool showSystemsMonitor = true;
        private int lastRenderedFrameCount;
        private GUIContent pauseButtonContent;

        private Queue<float> systemMonitorData;
        private Graph systemsMonitor;
        private SystemView[] systemViews;
        private SystemsDebugMono systemsDebugMono;
        private SystemsDebug SystemsDebug;
        private bool selected;


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
            systemsDebugMono.gameObject.name = $"ECS Systems Debug {systemsDebugMono.Systems.executeTime : 0.000} ms";
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
            EditorGUILayout.LabelField("Execution duration", $"{systems.executeTime : 0.000} ms");
        
            EditorGUILayout.EndVertical();

            if (pauseButtonContent == null)
                pauseButtonContent = EditorGUIUtility.IconContent("PauseButton On");
        
            systems.Systems.Alive = GUILayout.Toggle(systems.Systems.Alive, pauseButtonContent, (GUIStyle) "CommandLeft");

            EditorGUILayout.EndHorizontal();
            if (!EditorApplication.isPaused)
                AddDuration((float) systems.executeTime);
            systemsMonitor.Draw(systemMonitorData.ToArray(), 120f);
        
        
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField("Systems:");
            EditorGUILayout.LabelField("time ms:  | max time ms:");
            EditorGUILayout.EndHorizontal();


            var iJobSystemInteraceTag = typeof(IJobSystemTag);
            GUIStyle burstSystemsStyle = new GUIStyle(EditorStyles.textField);
            burstSystemsStyle.normal.textColor = new Color(1f, 0.46f, 0f);
        
            for (var i = 0; i < systemViews.Length; i++)
            {
                systemViews[i].name = systems.Systems.updateSystemsList[i].GetType().Name;
                systemViews[i].time = systems.executeTimes[i];
                // if (systemViews[i].time > systemViews[i].maxTime)
                //     systemViews[i].maxTime = systemViews[i].time;
                //
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                if (iJobSystemInteraceTag.IsInstanceOfType(systems.Systems.updateSystemsList[i]))
                {
                    EditorGUILayout.LabelField(systemViews[i].name, burstSystemsStyle);
                    EditorGUILayout.LabelField($"{systemViews[i].time : 0.000} ms", burstSystemsStyle);
                }
                else
                {
                    EditorGUILayout.LabelField(systemViews[i].name);
                    EditorGUILayout.LabelField($"{systemViews[i].time : 0.000} ms");
                }
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

        private struct SystemView
        {
            public double time;
            public double maxTime;
            public string name;
        }
    }
}