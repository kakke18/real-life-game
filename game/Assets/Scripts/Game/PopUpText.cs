using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour {

	public void ChangeText(int kind, int other)
    {
        if(kind == 1)
        {
            if(other == 0)
            {
                this.GetComponent<Text>().text = "1～5の中から好きなマス進むカード\nを手に入れた！";
            }
            else if(other == 1)
            {
                this.GetComponent<Text>().text = "次進むマスを2倍にするカード\nを手に入れた！";
            }
            else if(other == 2)
            {
                this.GetComponent<Text>().text = "次手に入れるゴールドを2倍にするカード\nを手に入れた！";
            }
            else if(other == 3)
            {
                this.GetComponent<Text>().text = "ランダムな白マス2つまで黄マスにするカード\nを手に入れた！";
            }
        }
        else if(kind == 2)
        {
            this.GetComponent<Text>().text = "スポーツマス！\nルーレット権を手に入れた！";
        }
        else if(kind == 3)
        {
            this.GetComponent<Text>().text = "バイトボーナス！\n"+ other.ToString() +"G手に入れた！";
        }
        else if(kind == 4)
        {
            this.GetComponent<Text>().text = other.ToString() + "G手に入れた！";
        }
    }
}
