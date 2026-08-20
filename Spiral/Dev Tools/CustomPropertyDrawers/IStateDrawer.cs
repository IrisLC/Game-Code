using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IState), true)]
public class IStateDrawer : ICustomPropertyDrawer
{
    static Dictionary<string, Type> TypeMap;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawGUI(ref TypeMap, typeof(IState), "State", position, property, label);
    }

}
