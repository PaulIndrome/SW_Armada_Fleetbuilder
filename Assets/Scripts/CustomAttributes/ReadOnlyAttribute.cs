using UnityEngine;
public class ReadOnlyAttribute : PropertyAttribute { 

    public bool unlockable = false;

    public ReadOnlyAttribute(bool unlockable = false){
        this.unlockable = unlockable;
    }
}