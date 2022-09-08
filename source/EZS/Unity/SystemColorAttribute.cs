using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class SystemColorAttribute : Attribute {
    public Color color;
    private static Dictionary<string, Color> colors = new Dictionary<string, Color>{
        {"red", Color.red},
        {"green", Color.green},
        {"blue", Color.blue},
        {"yellow", Color.yellow},
        {"cyan", Color.cyan}
    };
    public SystemColorAttribute(string colorName) {
        color = colors[colorName];
    }
}