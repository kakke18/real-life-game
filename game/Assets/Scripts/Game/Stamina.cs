using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour {
    public Player Player;
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Text>().text = Player.GetStamina().ToString();
    }
}
