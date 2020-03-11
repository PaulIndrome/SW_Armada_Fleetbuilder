using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class ScreenSizeRelative : MonoBehaviour
{
    [ReadOnly, SerializeField] private Vector2 screenRelativeSize;


    [Range(0f, 1f)]
    [SerializeField] private float relativeWidth;
    [SerializeField] private bool squareWidth = false;
    
    
    [Range(0f, 1f)]
    [SerializeField] private float relativeHeight;
    [SerializeField] private bool squareHeight = false;


    [SerializeField] private float maxSizeX = 0, maxSizeY = 0, minSizeX = 0, minSizeY = 0;


    private float width, height;
    private ScreenOrientation lastOrientation;
    private RectTransform rectTransform;
    private WaitForSeconds halfSecondWait;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        halfSecondWait = new WaitForSeconds(0.5f);
        GetComponentInParent<ScreenSizeRelativeControl>().AddScreenSizeRelative(this);
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // StartCoroutine(ResizeRoutine());
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        lastOrientation = Screen.orientation;
        Resize();
        // this.enabled = false;
    }

    // /// <summary>
    // /// LateUpdate is called every frame, if the Behaviour is enabled.
    // /// It is called after all Update functions have been called.
    // /// </summary>
    // void LateUpdate()
    // {
    //     Profiler.BeginSample("ScreenSizeRelative LateUpdate()");
    //     if(Screen.orientation != lastOrientation){
    //         lastOrientation = Screen.orientation;
    //         Resize();
    //     }
    //     Profiler.EndSample();
    // }

    public void Resize(){
        Profiler.BeginSample("Resize()", this);
        width = Mathf.Clamp(Screen.width * relativeWidth, minSizeX, maxSizeX > 0 ? maxSizeX : Screen.width);
        height = Mathf.Clamp(Screen.height * relativeHeight, minSizeY, maxSizeY > 0 ? maxSizeY : Screen.height);
        rectTransform.sizeDelta = screenRelativeSize = new Vector2(squareHeight ? height : width, squareWidth ? width : height);
        Profiler.EndSample();
    }

    // IEnumerator ResizeRoutine(){
    //     while(enabled){
    //         // if(Screen.orientation != lastOrientation)
    //             Resize();
    //         yield return halfSecondWait;
    //     }
    // }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        // StopAllCoroutines();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScreenSizeRelative)), CanEditMultipleObjects]
public class ScreenSizeRelativeEditor : Editor {


    public override void OnInspectorGUI(){
        // DrawDefaultInspector();
        serializedObject.UpdateIfRequiredOrScript();
        ScreenSizeRelative target = serializedObject.targetObject as ScreenSizeRelative;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("screenRelativeSize"));

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(serializedObject.FindProperty("squareHeight").boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("squareWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relativeWidth"));
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(serializedObject.FindProperty("squareWidth").boolValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("squareHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relativeHeight"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Limits", EditorStyles.boldLabel);


        EditorGUI.indentLevel += 1;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minSizeX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minSizeY"));
        EditorGUI.indentLevel -= 1;

        EditorGUI.indentLevel += 1;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSizeX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSizeY"));
        EditorGUI.indentLevel -= 1;

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
