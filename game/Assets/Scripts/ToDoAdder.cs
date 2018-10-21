using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;

public class ToDoAdder : MonoBehaviour {

    InputField inputField;
    ToggleGroup toggleGroup;
    public static bool blankFlag = false;
    public static bool toggleFlag = false;
    public static bool changeFlag = false;
    public static string addToDo = ""; 
    

    // Use this for initialization
    void Start () {
        /* 部品 */
        inputField = GameObject.Find("ToDoInputField").GetComponent<InputField>();
        toggleGroup = GameObject.Find("StudyToggle").GetComponent<ToggleGroup>();        
        /* ---- */
	}

	public void AddToDo () {
        toggleFlag = toggleGroup.AnyTogglesOn(); //トグルが選ばれているか
        string str = inputField.text;

        //空欄がなければ
        if (str != "" && toggleFlag) {
            //選ばれたトグルを代入
		    string selectedLabel = toggleGroup.ActiveToggles()
		        .First().GetComponentsInChildren<Text>()
                .First(t => t.name == "Label").text;

            //フラグ
            blankFlag = false;

            //Todo入力画面を初期化
            inputField.text = "";
            toggleGroup.SetAllTogglesOff();

            //追加
            addToDo += "," + str + ",";

            if (selectedLabel.Equals("勉強")) {
                addToDo += "0";
            }
            else if (selectedLabel.Equals("スポーツ")) {
                addToDo += "1";
            }
            else if (selectedLabel.Equals("バイト")) {
                addToDo += "2";
            }
            else if (selectedLabel.Equals("その他")) {
                addToDo += "3";
            }

            //フラグ
            changeFlag = true;
        }
        else {
            blankFlag = true;
        }
    }
}