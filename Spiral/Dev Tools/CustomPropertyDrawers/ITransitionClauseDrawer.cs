using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ITransitionClause), true)]
public class ITransitionClauseDrawer : ICustomPropertyDrawer
{
    static Dictionary<string, Type> TypeMap;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawGUI(ref TypeMap, typeof(ITransitionClause), "Transition Clause", position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight;
    }

}
