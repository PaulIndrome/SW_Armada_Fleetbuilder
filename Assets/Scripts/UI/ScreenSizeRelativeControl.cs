using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeRelativeControl : MonoBehaviour
{
    public float resizeDelayDuration = 0.5f;
    public List<ScreenSizeRelative> screenSizeRelatives;
    

    private Coroutine resizeRoutine;
    private WaitForSeconds resizeDelay;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        resizeDelay = new WaitForSeconds(resizeDelayDuration);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        resizeRoutine = StartCoroutine(ResizeAllRoutine());
    }

    public void AddScreenSizeRelative(ScreenSizeRelative ssr){
        screenSizeRelatives.Add(ssr);
    }

    IEnumerator ResizeAllRoutine(){
        int i = 0;
        while(enabled){
            for(i = 0; i < screenSizeRelatives.Count; i++){
                screenSizeRelatives[i].Resize();
            }
            yield return resizeDelay;
        }
    }

}
