using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

using Random = UnityEngine.Random;

namespace Wargon
{
    public static class ArrayHelp {
        public static void BasicCounting( ref int[] array) //простой вариант сортировки подсчетом
        {
            int n = array.Length;
            int max = 0;
            for (int i = 0; i < n; i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                }
            }

            int[] freq = new int[max + 1];
            for (int i = 0; i < max + 1; i++)
            {
                freq[i] = 0;
            }
            for (int i = 0; i < n; i++)
            {
                freq[array[i]]++;
            }

            for (int i = 0, j = 0; i <= max; i++)
            {
                while (freq[i] > 0)
                {
                    array[j] = i;
                    j++;
                    freq[i]--;
                }
            }
        }
    }
    public static class Kit
    {
        public static float value => Random.value;

        // RANDOM ARRAY
        public static T[] RandomArray<T>(this T[] arrayOne, T[] arrayTwo)
        {
            var values = new List<T[]> {arrayOne, arrayTwo};
            var randomIndex = Random.Range(0, values.Count);
            return values[randomIndex];
        }

        // RANDOM LIST
        public static List<T> RandomArray<T>(this List<T> arrayOne, List<T> arrayTwo)
        {
            var values = new List<List<T>> {arrayOne, arrayTwo};
            var randomIndex = Random.Range(0, values.Count);
            return values[randomIndex];
        }

        // RANDOM BOOL WITH CHANCE ON TRUE
        public static bool RandomBool(float chanceOnTrue)
        {
            return Random.value < chanceOnTrue ? true : false;
        }

        // RANDOM OBJECT FROM ARRAY OF ANY TYPE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRandomElement<T>(this T[] array)
        {
            var randomIndex = Random.Range(0, array.Length-1);
            return array[randomIndex];
        }

        public static T RandomElement<T>(List<T> array)
        {
            var randomIndex = Random.Range(0, array.Count-1);
            return array[randomIndex];
        }

        // RANDOM ENUM
        public static T RandomEnum<T>()
        {
            var values = Enum.GetValues(typeof(T));
            return (T) values.GetValue(Random.Range(0, values.Length));
        }

        public static int RandomRange(int from, int to)
        {
            return Random.Range(from, to);
        }

        public static float RandomRange(float from, float to)
        {
            return Random.Range(from, to);
        }

        public static bool EnumArrayExist<T>(T[] whatCompare, T withWhat)
        {
            for (var i = 0; i < whatCompare.Length; i++)
                if (whatCompare[i].Equals(withWhat))
                    return true;
            return false;
        }
        public static bool EnumArrayExist<T>(List<T> whatCompare, T withWhat)
        {
            for (var i = 0; i < whatCompare.Count; i++)
                if (whatCompare[i].Equals(withWhat))
                    return true;
            return false;
        }
        public static bool Has<T>(this T[] whatCompare, T withWhat)
        {
            for (int i = 0,iMax = whatCompare.Length; i < iMax; i++)
                if (whatCompare[i].Equals(withWhat))
                    return true;
            return false;
        }
        public static bool Hass(this Enum[] whatCompare, Enum withWhat)
        {
            for (int i = 0,iMax = whatCompare.Length; i < iMax; i++)
                if (whatCompare[i].Equals(withWhat))
                    return true;
            return false;
        }
        public static bool MouseDown()
        {
            return Input.GetMouseButtonDown(0);
        }

        private static Camera mainCamera;
        private static readonly Vector3 Offset = new Vector3(0, 0, 10);
        public static Vector3 MousePosition()
        {
            if(mainCamera == null)
                mainCamera = Camera.main;
            if (mainCamera == null)
                throw new Exception("Can't get mouse position, main camera not found!");
            return mainCamera.ScreenToWorldPoint(Input.mousePosition) + Offset;
        }
        public static Vector3 MousePosition([NotNull]Camera camera)
        {
            return camera.ScreenToWorldPoint(Input.mousePosition) + Offset;
        }
        public static Vector2 Rotate(Vector2 aPoint, float aDegree)
        {
            var rad = aDegree * Mathf.Deg2Rad;
            var s = Mathf.Sin(rad);
            var c = Mathf.Cos(rad);
            return new Vector2(
                aPoint.x * c - aPoint.y * s,
                aPoint.y * c + aPoint.x * s);
        }

        public static Vector3 RandomVector3(float range1, float range2) {
            return new Vector3(Random.Range(range1, range2), Random.Range(range1, range2),
                Random.Range(range1, range2));
        }
    }
    

}

public static class ListExtension
{
    public static bool HasType(this IList list, Type whatHas)
    {
        var i = 0;
        var count = list.Count;
        for (i = 0; i < count; i++)
            if (list[i].GetType() == whatHas)
                return true;
        return false;
    }
}

public static class Log
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(string massage)
    {
        Debug.Log(massage);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Show(Color color, string massage)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=#{(byte) (color.r * 255f):X2}{(byte) (color.g * 255f):X2}{(byte) (color.b * 255f):X2}>{massage}</color>");
#endif
    }
}

