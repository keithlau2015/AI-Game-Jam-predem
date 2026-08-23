using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;

public static class GenericMethod
{
    public static List<int> ConvertStringToIntList(string source, char separator = '|')
    {
        List<int> result = new List<int>();
        if (string.IsNullOrEmpty(source))
            return result;

        string[] splitID = source.Split(separator);
        foreach (string id in splitID)
        {
            int item = 0;
            if (!int.TryParse(id, out item))
                continue;
            result.Add(item);
        }
        return result;
    }

    public static List<float> ConvertStringToFloatList(string source, char separator = '|')
    {
        List<float> result = new List<float>();
        if (string.IsNullOrEmpty(source))
            return result;

        string[] splitID = source.Split(separator);
        foreach (string id in splitID)
        {
            float item = 0;
            if (!float.TryParse(id, out item))
                continue;
            result.Add(item);
        }
        return result;
    }

    public static List<long> ConvertStringToLongList(string source, char separator = '|')
    {
        List<long> result = new List<long>();
        if (string.IsNullOrEmpty(source))
            return result;

        string[] splitID = source.Split(separator);
        foreach (string id in splitID)
        {
            long item = 0;
            if (!long.TryParse(id, out item))
                continue;
            result.Add(item);
        }
        return result;
    }

    public static List<string> ConvertStringToStringList(string source, char separator = '|')
    {
        List<string> result = new List<string>();
        if (string.IsNullOrEmpty(source))
            return result;

        string[] splitID = source.Split(separator);
        foreach (string id in splitID)
        {
            result.Add(id);
        }
        return result;
    }
}
