using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartBonusText : MonoBehaviour {

    // Update is called once per frame
    public void Normal()
    {
        this.GetComponent<Text>().text = "周回ボーナス！\n10000Gゲット！！";
    }

    public void Double()
    {
        this.GetComponent<Text>().text = "周回ボーナス！\n20000Gゲット！！";
    }
}
