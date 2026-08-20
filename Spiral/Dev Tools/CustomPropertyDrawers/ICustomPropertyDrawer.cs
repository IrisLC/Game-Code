using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class ICustomPropertyDrawer : PropertyDrawer
{
    public static void DrawGUI(ref Dictionary<string, Type> TypeMap, Type PropertyType, string propertyTypeName, Rect position, SerializedProperty property, GUIContent label)
    {
        if (TypeMap == null)
        {
            BuildTypeMap(ref TypeMap, PropertyType);
        }

        GUIContent propertyName = new(property.name + ": ");
        float width = EditorStyles.label.CalcSize(propertyName).x;

        Rect nameRect = new(position.x, position.y, width, EditorGUIUtility.singleLineHeight);
        Rect typeRect = new(position.x + width, position.y, position.width - width, EditorGUIUtility.singleLineHeight);
        Rect contentRect = new(position.x + width, position.y + EditorGUIUtility.singleLineHeight, position.width - width, position.height - EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(nameRect, propertyName);

        EditorGUI.BeginProperty(position, label, property);
        string typeName = property.managedReferenceFullTypename;
        string displayName = GetShortTypeName(typeName);

        if (EditorGUI.DropdownButton(typeRect, new GUIContent(displayName ?? $"Select {propertyTypeName} Type"), FocusType.Keyboard))
        {
            GenericMenu menu = new();
            if (TypeMap == null || TypeMap.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent($"No {propertyTypeName}s Available"));
                menu.ShowAsContext();
                return;
            }

            foreach (KeyValuePair<string, Type> MappedType in TypeMap)
            {
                string name = MappedType.Key;
                Type type = MappedType.Value;
                menu.AddItem(new GUIContent(name), type.FullName == typeName, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        if (property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(contentRect, property, GUIContent.none, true);
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight;
    }

    protected static void BuildTypeMap(ref Dictionary<string, Type> TypeMap, Type type)
    {
        Type BaseType = type;

        TypeMap = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm =>
            {
                try { return asm.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(t => !t.IsAbstract && BaseType.IsAssignableFrom(t))
            .ToDictionary(t => ObjectNames.NicifyVariableName(t.Name));
    }

    protected static string GetShortTypeName(string fullTypeName)
    {
        if (string.IsNullOrEmpty(fullTypeName))
        {
            return null;
        }

        string[] parts = fullTypeName.Split(' ');

        return parts.Length > 1 ? parts[1].Split('.').Last() : fullTypeName;
    }
}
