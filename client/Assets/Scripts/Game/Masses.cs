using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

//サーバとの送受信のためにClientクラスを継承
public class Masses : Client {

    bool useServer = StartUp.useServer; ///サーバを使う場合はtrue StartUp.csで宣言
    NetworkStream ns; //サーバとの送受信に必要
    public GameObject[] MassGameObjects;
    public Player Player;
    public string[] board;
    public string board2 = "";

    public void Start()
    {
        string[] stArrayData;

        //* サーバを使う場合 */
        if (useServer) {
            /* スタート時にサーバから受信したボード情報で表示 */
            this.ns= StartUp.ns;
            base.sendMessage(ns, "2"); //サーバにボード情報を要求
            do {
                base.receiveMessage(ns); //サーバから受信
            } while (base.recMsg == "");
            SetBoard(base.recMsg.Remove(0, 1)); //boardにセット  
        }
        /* サーバを使わない場合 */
        else {
            //string board = "0:0:0:0:0:0:0:0:0:1,0,カード1:0:0:0:0:0:0:0:0:0:0";
            string board = "10:1,0,カード1:2,0,ルーレット権:3,0,多めのゴールド:4,5000,5000G:1,0,カード1:2,0,ルーレット権:3,0,多めのゴールド:4,5000,5000G:1,0,カード1:2,0,ルーレット権:3,0,多めのゴールド:4,5000,5000G:1,0,カード1:2,0,ルーレット権:3,0,多めのゴールド:4,5000,5000G:1,0,カード1:2,0,ルーレット権:3,0,多めのゴールド";
            SetBoard(board);
        }

        MassGameObjects[0].GetComponent<Renderer>().material.color = Color.black;

        for (int i = 0; i< board.Length; i++)
        {
            stArrayData = board[i].Split(',');

            //プレイヤだったら
            if (int.Parse(stArrayData[0]) >= 10){
                Player.SetPlayer(i);
            }

            if (i != 0)
            {
                ChangeColor(int.Parse(stArrayData[0]), i);
            }
        }
        
    }

    public void ChangeColor(int color, int offset)
    {
        color = color % 10; //プレイヤがいる場所

        if (color == 1)
        {
            MassGameObjects[offset].GetComponent<Renderer>().material.color = Color.red;
        }
        else if (color == 2)
        {
            MassGameObjects[offset].GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (color == 3)
        {
            MassGameObjects[offset].GetComponent<Renderer>().material.color = Color.green;
        }
        else if (color == 4)
        {
            MassGameObjects[offset].GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if(color == 0)
        {
            MassGameObjects[offset].GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            Debug.Log("error");
        }
    }

    public void SetBoard(string strBoard)
    {
        string[] board = strBoard.Split(':');
        
        for(int i=0; i<this.board.Length; i++)
        {
            Debug.Log(board[i]);
            this.board[i] = board[i];
        }
    }

    public void ChangeMass(int nextPlace, string str)
    {
        this.board[nextPlace] = str;
    }

    public string GetMass(int place)
    {
        string[] strArrayData = board[place].Split(',');
        /*-----------白マスの場合，文字を表示しない------------*/
        if (int.Parse(strArrayData[0]) == 0 || int.Parse(strArrayData[0]) == 10) {
            return "";
        }
        /*----------------------------------------*/
        else {
            return strArrayData[2];
        }
    }

    public int GetBoardColor(int place)
    {
        string[] strArrayData = board[place].Split(',');

        return int.Parse(strArrayData[0]);
    }

    // 文字の出現回数をカウント
    public int CountChar(string s, string c) {
        return s.Length - s.Replace(c, "").Length;
    }
}
