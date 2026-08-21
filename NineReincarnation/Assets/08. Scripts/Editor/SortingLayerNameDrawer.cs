using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortingLayerNameAttribute))]
public sealed class SortingLayerNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SortingLayer[] sortingLayers = SortingLayer.layers;

        if (sortingLayers.Length == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        string[] names = new string[sortingLayers.Length];
        int selectedIndex = 0;

        for (int i = 0; i < sortingLayers.Length; i++)
        {
            names[i] = sortingLayers[i].name;

            if (string.Equals(property.stringValue, names[i], StringComparison.Ordinal))
            {
                selectedIndex = i;
            }
        }

        EditorGUI.BeginProperty(position, label, property);
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, names);
        property.stringValue = names[selectedIndex];
        EditorGUI.EndProperty();
    }
}
