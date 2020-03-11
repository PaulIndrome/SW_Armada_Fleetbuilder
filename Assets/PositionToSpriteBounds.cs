using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class PositionToSpriteBounds : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] public Image boundsImage;

    [Header("Settings")]
    [SerializeField] public bool positionAtLeft = false, positionAtRight = false;
    [SerializeField] public bool positionAtTop = false, positionAtBottom = false;
    private RectTransform boundsImageRectTransform;
    private RectTransform rectTransform;

    [Header("Set via script")]
    [SerializeField] private float spriteWidth;
    [SerializeField] private float spriteHeight;
    [SerializeField] private float topGap;
    [SerializeField] private float rightGap;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(boundsImage == null) {
            this.enabled = false;
            return;
        }

        boundsImageRectTransform = boundsImage.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        
    }

    public void RecalculateToSpriteBounds(float customXOffset = 0f, float customYOffset = 0f){
        if(!boundsImageRectTransform){
            Awake();
        }

        float rectWidth, rectHeight;
        rectWidth = boundsImageRectTransform.sizeDelta.x != 0 ? boundsImageRectTransform.sizeDelta.x : boundsImageRectTransform.rect.width;
        rectHeight = boundsImageRectTransform.sizeDelta.y != 0 ? boundsImageRectTransform.sizeDelta.y : boundsImageRectTransform.rect.height;

        spriteWidth = (rectHeight / boundsImage.sprite.texture.height) * boundsImage.sprite.texture.width;
        spriteHeight = (rectWidth / boundsImage.sprite.texture.width) * boundsImage.sprite.texture.height;

        // Debug.Log($"boundsImageRectTransform.sizeDelta.x: {boundsImageRectTransform.sizeDelta.x}");
        // Debug.Log($"boundsImageRectTransform.sizeDelta.y: {boundsImageRectTransform.sizeDelta.y}");
        // Debug.Log($"boundsImage.sprite.texture.width: {boundsImage.sprite.texture.width}");
        // Debug.Log($"boundsImage.sprite.texture.height: {boundsImage.sprite.texture.height}");
        // Debug.Log($"boundsImageRectTransform.rect.width: {boundsImageRectTransform.rect.width}");
        // Debug.Log($"boundsImageRectTransform.rect.height: {boundsImageRectTransform.rect.height}");

        if(spriteHeight > rectHeight){
            topGap = 0f;
        } else {
            topGap = (rectHeight % spriteHeight) * 0.5f;
        }

        if(spriteWidth > rectWidth){
            rightGap = 0f;
        } else {
            rightGap = (rectWidth % spriteWidth) * 0.5f;
        }

        // Vector2 customOffset = new Vector2(customXOffset, customYOffset);
        
        // Vector2 resized = Vector2.zero;
        // resized.x = - (rightGap * 2);
        // resized.y = - (topGap * 2);

        Vector2 sizeDeltaNew = Vector2.zero;
        Vector2 anchoredPosNew = Vector2.zero;

        if(positionAtLeft){
            sizeDeltaNew.x -= rightGap;
        } else {
            anchoredPosNew.x += rightGap * 0.5f;
        }

        if(positionAtRight){
            sizeDeltaNew.x -= rightGap;
        } else {
            anchoredPosNew.x -= rightGap * 0.5f;
        }

        if(positionAtTop){
            sizeDeltaNew.y -= topGap;
        } else {
            anchoredPosNew.y -= topGap * 0.5f;
        }

        if(positionAtBottom){
            sizeDeltaNew.y -= topGap;
        } else {
            anchoredPosNew.y += topGap * 0.5f;
        }

        rectTransform.sizeDelta = sizeDeltaNew;
        rectTransform.anchoredPosition = anchoredPosNew;
    }
    
    [ContextMenu("Reposition test")]
    public void RepositionTest(){
        RecalculateToSpriteBounds();
    }

    
}

#if UNITY_EDITOR
[CustomEditor(typeof(PositionToSpriteBounds)), CanEditMultipleObjects]
public class PositionToSpriteBoundsEditor : Editor {


    public override void OnInspectorGUI(){
        // DrawDefaultInspector();
        serializedObject.UpdateIfRequiredOrScript();
        PositionToSpriteBounds target = serializedObject.targetObject as PositionToSpriteBounds;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
        GUI.enabled = true;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("boundsImage"));

        EditorGUILayout.Space();

        EditorGUI.indentLevel += 1;

        target.positionAtTop = EditorGUILayout.Toggle("Top", target.positionAtTop/*  && !target.positionAtBottom */);
        target.positionAtBottom = EditorGUILayout.Toggle("Bottom", target.positionAtBottom/*  && !target.positionAtTop */);

        EditorGUILayout.Space();

        target.positionAtLeft = EditorGUILayout.Toggle("Left", target.positionAtLeft/*  && !target.positionAtRight */);
        target.positionAtRight = EditorGUILayout.Toggle("Right", target.positionAtRight/*  && !target.positionAtLeft */);
        
        EditorGUI.indentLevel -= 1;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("topGap"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightGap"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
