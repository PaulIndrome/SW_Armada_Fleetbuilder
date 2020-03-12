using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleGraphicSwap : MonoBehaviour
{
    public Image isOnGraphic, isOffGraphic;
    private Toggle toggle;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        toggle.onValueChanged.AddListener(SwapGraphic);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        SwapGraphic(toggle.isOn);
    }

    public void SwapGraphic(bool onOff){
        isOnGraphic.gameObject.SetActive(onOff);
        isOffGraphic.gameObject.SetActive(!onOff);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(SwapGraphic);    
    }

}
