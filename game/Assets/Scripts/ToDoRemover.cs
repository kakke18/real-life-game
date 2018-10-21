using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;

public class ToDoRemover : MonoBehaviour {

    public static bool changeFlag = false;
    public static bool finishFlag = false;
    public static string removeToDo = "";
    public static Color category;

    public void RemoveToDo () {
        GameObject parent = transform.parent.gameObject;
        Text[] remove = parent.GetComponentsInChildren<Text>();
        removeToDo = remove[0].text;
        changeFlag = true;
        ToDoController.OpenDialog();
    }

    public void FinishToDo () {
        GameObject parent = transform.parent.gameObject;
        Text[] finish = parent.GetComponentsInChildren<Text>();
        removeToDo = finish[0].text;
        category = finish[0].color;
        changeFlag = true;
        finishFlag =true;
        ToDoController.OpenDialog();
    }
}