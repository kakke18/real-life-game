using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number1 : MonoBehaviour {
    public CardMenu CardMenu;
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Text>().text = CardMenu.GetCard(1) + "枚";
    }
}
