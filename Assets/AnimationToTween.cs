using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationToTween : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] AnimationClip testAnimationClip;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
   
    [ContextMenu("Test Tween From Animation")]
    public void TestTweenFromAnimation(){
        if(!animator){
            Awake();
        }
        GenerateTweenFromAnimation(testAnimationClip);
    }

    void GenerateTweenFromAnimation(AnimationClip animationClip){
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);
        Debug.Log("Amount of curve bindings " + curveBindings.Length);
        
        int framesInClip = (int) (animationClip.frameRate * animationClip.length);
        Debug.Log("Frames in clip: " + framesInClip);

        for(int cb = 0; cb < curveBindings.Length; cb++){
            Debug.Log($"{curveBindings[cb].propertyName} on {curveBindings[cb].path}, type {curveBindings[cb].type}");
            AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[cb]);
            
            foreach(Keyframe keyframe in curve.keys){
                Debug.Log($"{keyframe.time} ({(int)(keyframe.time * animationClip.frameRate)}): {keyframe.value}");
            }

            // for(int i = 0; i < framesInClip; i++){
                
            // }
        }


        EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
        Debug.Log("Amount of curve bindings " + editorCurveBindings.Length);
        foreach(EditorCurveBinding curveBinding in editorCurveBindings){
            Debug.Log($"{curveBinding.propertyName} on {curveBinding.path}");
        }

    }
}
