using System;
using System.Collections.Generic;
using UnityEngine;


namespace Wargon.ezs {
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemColorAttribute : Attribute {
        public Color color;
        private static readonly Dictionary<string, Color> colors = new (){
            {DColor.red, Color.red},
            {DColor.green, Color.green},
            {DColor.blue, new Color(0f, 0.65f, 1f)},
            {DColor.yellow, Color.yellow},
            {DColor.cyan, Color.cyan},
            {DColor.violet, new Color(0.65f, 0f, 1f)},
            {DColor.orange, new Color(1f, 0.59f, 0f)},
            {DColor.pink, new Color(1f, 0f, 0.57f)}
        };
        public SystemColorAttribute(string colorName) {
            color = colors[colorName];
        }

        public SystemColorAttribute(float r, float g, float b) {
            color = new Color(r, g, b);
        }
        public SystemColorAttribute(Color c) {
            color = c;
        }
    }

    public static class DColor {
        public const string red = "red";
        public const string green = "green";
        public const string blue = "blue";
        public const string yellow = "yellow";
        public const string cyan = "cyan";
        public const string violet = "violet";
        public const string orange = "orange";
        public const string pink = "pink";
    }
}
