using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer {

    private bool unLocked = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute) attribute;
        
        Rect togglePosition = position;
        togglePosition.x -= 12f;
        togglePosition.width = 12f;
        if(readOnlyAttribute.unlockable){
            unLocked = EditorGUI.ToggleLeft(togglePosition, GUIContent.none, unLocked);
        }

        EditorGUI.BeginDisabledGroup(!unLocked);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }
}