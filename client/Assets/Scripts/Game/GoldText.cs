using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldText : MonoBehaviour {
    public Player Player;
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Text>().text = Player.GetGold().ToString();
	}
}
