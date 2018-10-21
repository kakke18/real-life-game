using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MassText : MonoBehaviour {

    public Masses Masses;
    public int massNum = 0;

    // Update is called once per frame
    void Update () {
        string massText = Masses.GetMass(massNum);

        //赤色
        if (massText.Contains("カード")) {
            this.GetComponent<Text>().text = massText;
            this.GetComponent<Text>().color = Color.white;
            this.GetComponent<Text>().fontSize = 22;
        }
        //青色
        else if (massText.Contains("ルーレット")) {
            this.GetComponent<Text>().text = massText;
            this.GetComponent<Text>().color = Color.white;
        }
        //緑色
        else if (massText.Contains("多め")) {
            this.GetComponent<Text>().text = "多めの\nゴールド";
            this.GetComponent<Text>().fontSize = 20;
        }
        //黄色
        else {
            this.GetComponent<Text>().text = massText;
            this.GetComponent<Text>().fontSize = 22;
        }
    }
}
