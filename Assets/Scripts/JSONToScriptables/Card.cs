using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{

    public string ID = "";
    public string imageURL = "";
    public string size = "";
    public string faction = "";
    public string name = "";
    public int cost = -1;
    public string[] contradictory = new string[0];
    public bool isCommander = false;
    public bool isUnique = false;
    public bool isSquadron = false;
    public string cardType = "";
    public string[] upgradeSlots = new string[0];

}