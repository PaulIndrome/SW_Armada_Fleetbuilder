using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{

    public int customTargetFrameRate = -1;
    [SerializeField] private ApplicationChrome.States navBarState;

    void Awake()
    {
        if(customTargetFrameRate > 30){
            Application.targetFrameRate = customTargetFrameRate;
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        //ApplicationChrome.navigationBarState = navBarState;
    }

}
