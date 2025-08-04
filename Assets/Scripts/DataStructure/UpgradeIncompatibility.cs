#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class UpgradeCompatibility {

    public ShipSize compatibleShipsizes = (ShipSize)~0;
    
    public bool hasCompatibleShiptype = false;
    public ShipType[] compatibleShipTypes = new ShipType[0];

    public bool hasCombinedIncompatibility = false;
    public ShipSize incompatibleShipsizes = 0;
    public UpgradeType[] incompatibleUpgradeTypes = new UpgradeType[0];

}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(UpgradeCompatibility))]
public class UpgradeIncompatibilityDrawer : PropertyDrawer {
    public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label){
        EditorGUI.BeginProperty(position, label, property);
        
        EditorGUILayout.PropertyField(property.FindPropertyRelative("compatibleShipsizes"), true);

        SerializedProperty hasCompatibleShiptypeProp = property.FindPropertyRelative("hasCompatibleShiptype");
        EditorGUILayout.PropertyField(hasCompatibleShiptypeProp, true);
        if(hasCompatibleShiptypeProp.boolValue){
            EditorGUILayout.PropertyField(property.FindPropertyRelative("compatibleShipTypes"), true);
        }

        SerializedProperty hasCombinedIncompatibilityProp = property.FindPropertyRelative("hasCombinedIncompatibility");
        EditorGUILayout.PropertyField(hasCombinedIncompatibilityProp, true);
        if(hasCombinedIncompatibilityProp.boolValue){
            EditorGUILayout.PropertyField(property.FindPropertyRelative("incompatibleShipsizes"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("incompatibleUpgradeTypes"), true);
        }
    }
}

#endif