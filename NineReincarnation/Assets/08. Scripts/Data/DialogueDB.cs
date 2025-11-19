using System;
using System.Collections.Generic;
using ExcelData;
using UnityEngine;

public class DialogueDB
{
    private Dictionary<Type, object> _databases = new();

    public DialogueDB()
    {
        DialogueDataSO res = Resources.Load<DialogueDataSO>("DialogueData");

        RegisterDatabase<DialogueClass>(res.Dialogue, item => item.ID);
        RegisterDatabase<CameraClass>(res.Camera, item => item.ID);
        RegisterDatabase<AnimationClass>(res.Animation, item => item.ID);
        RegisterDatabase<ScriptClass>(res.Script, item => item.ID);
        RegisterDatabase<BubbleClass>(res.Bubble, item => item.ID);
    }
    private void RegisterDatabase<T>(IEnumerable<T> sourceList, Func<T, int> keySelector)
    {
        if (sourceList == null) return;

        Dictionary<int, T> newDictionary = new Dictionary<int, T>();

        foreach (T item in sourceList)
        {
            int key = keySelector(item);

            newDictionary.Add(key, item);
        }

        _databases.Add(typeof(T), newDictionary);
    }

    public T GetData<T>(int id) where T : class
    {
        Type type = typeof(T);

        if (_databases.TryGetValue(type, out object db))
        {
            Dictionary<int, T> typedDict = (Dictionary<int, T>)db;

            if (typedDict.TryGetValue(id, out T value))
            {
                return value;
            }
        }

        return null;
    }
}
