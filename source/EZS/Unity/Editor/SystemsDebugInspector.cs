using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wargon.ezs.Unity
{
    [CustomEditor(typeof(SystemsDebugMono))]
    public class SystemsDebugInspector : Editor
    {
        private const int SYSTEM_MONITOR_DATA_LENGTH = 140;
        private static bool showSystemsMonitor = true;
        private int lastRenderedFrameCount;
        private GUIContent pauseButtonContent;
        private bool selected;

        private Queue<float> systemMonitorData;
        private SystemsDebug SystemsDebug;
        private SystemsDebugMono systemsDebugMono;
        private Graph systemsMonitor;
        private SystemView[] systemViews;

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
            if(systemsDebugMono!=null)
                systemsDebugMono.gameObject.name = $"ECS Systems Debug {systemsDebugMono.Systems.executeTime: 0.000} ms";
        }

        private void DrawSystemsMonitor(SystemsDebug systemsDebug)
        {
            if (systemsMonitor == null)
            {
                systemsMonitor = new Graph(SYSTEM_MONITOR_DATA_LENGTH);
                systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
            }

            if (systemViews == null) {
                systemViews = new SystemView[systemsDebug.Systems.updateSystemsList.Count];
                for (var i = 0; i < systemViews.Length; i++) {
                    systemViews[i].tenTimes = new Queue<double>();
                }
            }

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


            var burstSystemsStyle = new GUIStyle(EditorStyles.textField);
            burstSystemsStyle.normal.textColor = new Color(1f, 0.46f, 0f);
            
            for (var i = 0; i < systemViews.Length; i++)
            {
                ref var system = ref systemViews[i];
                var systemType = systemsDebug.Systems.updateSystemsList[i].GetType();
                system.name = systemType.Name;
                system.timems = systemsDebug.executeTimes[i];
                if(systemType == typeof(RemoveComponentSystem)) continue;
                //system.SetNewTime(system.timems);
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                
                if (systemType.IsDefined(typeof(SystemColorAttribute), false)) {
                    systemsDebug.active[i] = EditorGUILayout.Toggle(systemsDebug.active[i]);
                    DrawSystemWithColor(systemType, ref system);
                }
                else if(systemType != typeof(RemoveComponentSystem))
                {
                    systemsDebug.active[i] = EditorGUILayout.Toggle(systemsDebug.active[i]);
                    EditorGUILayout.LabelField(system.name);
                    EditorGUILayout.LabelField($"{(system.timems > 0.001 ? system.timems : 0.000): 0.000} ms");
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawSystemWithColor(Type systemType, ref SystemView system) {
            var atribute = (SystemColorAttribute) Attribute.GetCustomAttribute(systemType, typeof(SystemColorAttribute));
            var style = new GUIStyle(EditorStyles.textField) {normal = {textColor = atribute.color}};
            EditorGUILayout.LabelField(system.name, style);
            EditorGUILayout.LabelField($"{(system.timems/* > 0.002 ? system.time : 0.000*/): 0.000} ms", style);
        }

        private SystemView[] SortByTime(SystemView[] array)
        {
            Array.Sort(array, (x, y) => y.timems.CompareTo(x.timems));
            return array;
        }

        private SystemView[] SortByName(SystemView[] array)
        {
            Array.Sort(array, (x, y) => string.Compare(y.name, x.name, StringComparison.Ordinal));
            return array;
        }
        private SystemView[] SortByAvarage(SystemView[] array)
        {
            Array.Sort(array, (x, y) => y.timems.CompareTo(x.avarage));
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
            public double timems;
            public double maxTime;
            public Queue<double> tenTimes;
            public double mid;
            public string name;
            public bool active;
            private int lastTimeIndex;
            private int timesCount;
            public double avarage;
            public void SetNewTime(double value) {
                tenTimes.Enqueue(value);
                if (tenTimes.Count == 20) {
                    tenTimes.Dequeue();
                }
            }

            public double GetAvarage() {
                avarage = 0;
                foreach (var tenTime in tenTimes) {
                    avarage += tenTime;
                }

                avarage /= tenTimes.Count;
                return avarage;
            }
        }
    }
}