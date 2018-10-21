using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/* サーバとの送受信に必要 */
using System.Net;
using System.Net.Sockets;
/* -------------------- */

//サーバとの送受信のためにClientクラスを継承
public class Player : Client {

    bool useServer = StartUp.useServer;
    NetworkStream ns; //サーバとの送受信に必要
    public GameObject obj;
    public Masses Masses;
    public RouletteController RouletteController;
    public StartBonus StartBonus;
    public StartBonusText StartBonusText;
    public CardMenu CardMenu;
    public Use1 Use1;
    public Use2 Use2;
    public PopUpText PopUpText;
    public MassEffectPopUp MassEffectPopUp;
    public AnimationController AnimationController;
    int gold = 0;
    int nextPlace = 1;
    int stamina;
    int PartTimeJobCount = 0;
    float positionX, positionZ;
    bool DoubleMoveFlag = false;
    bool DoubleGoldFlag = false;


    public void Start () {
        /* サーバを使う */
        if (useServer) {
            this.ns= StartUp.ns;

            //ゴールド取得
            base.sendMessage(ns, "3"); //サーバにゴールド数を要求
            do {
                base.receiveMessage(ns); //サーバから受信
            } while (!base.recMsg.StartsWith("3"));
            gold = int.Parse(base.recMsg.Remove(0, 1)); //先頭を取り除いた値を代入  
    
            //スタミナ取得
            //ゴールド取得
            base.sendMessage(ns, "4"); //サーバにスタミナ数を要求
            do {
                base.receiveMessage(ns); //サーバから受信
            } while (!base.recMsg.StartsWith("4"));
            stamina = int.Parse(base.recMsg.Remove(0, 1)); //先頭を取り除いた値を代入  
        }
        else {
            gold = 10000;
            stamina = 10;
        }
    }

    public void SetPlayer(int position)
    {
        Vector3 posi = new Vector3(Masses.MassGameObjects[position].transform.localPosition.x, 0.5f,
            Masses.MassGameObjects[position].transform.localPosition.z);
        this.gameObject.transform.position = posi;

        //プレイヤの向き
        int y;
        if (position < 5) {
            y = 270;
        }
        else if (position < 10) {
            y = 180;
        }
        else if (position < 15) {
            y = 90;
        }
        else {
            y = 0;
        }
        this.gameObject.transform.rotation = Quaternion.Euler(0, y, 0);
        nextPlace = position+1;
    }

    public void Move(int moveCount)
    {
        //プレイヤがいたところを-10
        Masses.ChangeMass(nextPlace - 1, "0");
        AnimationController.SetTrue();

        var sequence = DOTween.Sequence();
        if (DoubleMoveFlag)
        {
            moveCount *= 2;
            DoubleMoveFlag = false;

            if (CardMenu.GetCard(1) > 0)
            {
                Use1.SetTrue();
            }
        }

        for (int i = 0; i < moveCount; i++)
        {
            int j = 0;
            float k = 0;
            if (nextPlace == 20)
            {
                nextPlace = 0;
            }
            positionX = Masses.MassGameObjects[nextPlace].transform.localPosition.x;
            positionZ = Masses.MassGameObjects[nextPlace].transform.localPosition.z;

            float time = 0.2f;

            sequence.Append(
                this.gameObject.transform.DOMoveX(positionX, time)
            );
            sequence.Append(
                this.transform.DOMoveZ(positionZ, time)
            );  

            if(nextPlace == 0)
            {
                sequence.Append(
                    this.gameObject.transform.DORotate(new Vector3(0, 270,0), 1f)
                    );
            }else if(nextPlace == 5)
            {
                sequence.Append(
                    this.gameObject.transform.DORotate(new Vector3(0, 180, 0), 1f)
                    );
            }
            else if(nextPlace == 10)
            {
                sequence.Append(
                    this.gameObject.transform.DORotate(new Vector3(0, 90, 0), 1f)
                    );
            }else if(nextPlace == 15)
            {
                sequence.Append(
                    this.gameObject.transform.DORotate(new Vector3(0, 0, 0), 1f)
                    );
            }

            if(nextPlace == 0)
            {
                if (DoubleGoldFlag)
                {
                    sequence.AppendCallback(() => gold += 20000);
                    StartBonusText.Double();
                    DoubleGoldFlag = false;

                    if(CardMenu.GetCard(2) > 0)
                    {
                        Use2.SetTrue();
                    }
                }
                else
                {
                    sequence.AppendCallback(() => gold += 10000);
                    StartBonusText.Normal();
                }
                sequence.AppendCallback(() => StartBonus.Bonus());
            }

            nextPlace += 1;

            if (i == moveCount - 1)
            {
                sequence.AppendCallback(() => RouletteController.RotMoveFinish());
                sequence.AppendCallback(() => MassEffect(nextPlace - 1));
                sequence.AppendCallback(() => AnimationController.SetFalse());
            }
        }
    }

    public void MassEffect(int place)
    {
        string[] stArrayData = Masses.board[place].Split(',');

        if(stArrayData[0] == "1")
        {
            PopUpText.ChangeText(int.Parse(stArrayData[0]), int.Parse(stArrayData[1]));
            CardMenu.SetCard(int.Parse(stArrayData[1]));
        }
        else if(stArrayData[0] == "2")
        {
            PopUpText.ChangeText(int.Parse(stArrayData[0]), int.Parse(stArrayData[1]));
            stamina += 1;
            //スタミナをサーバに送信
            if (useServer) {
                string send = "4";
                send += stamina.ToString();
                base.sendMessage(ns, send);
            }
        }
        else if(stArrayData[0] == "3")
        {
            if (DoubleGoldFlag)
            {
                PopUpText.ChangeText(int.Parse(stArrayData[0]), 2 * (5000 + 1000 * PartTimeJobCount));
                gold += 2 * (5000 + 1000*PartTimeJobCount);
                DoubleGoldFlag = false;

                if (CardMenu.GetCard(2) > 0) {
                    Use2.SetTrue();
                }
            }
            else
            {
                PopUpText.ChangeText(int.Parse(stArrayData[0]), 5000 + 1000 * PartTimeJobCount);
                gold += (5000 + 1000 * PartTimeJobCount);
            }

            //ゴールドをサーバに送信
            if (useServer) {
                string send = "3";
                send += gold.ToString();
                base.sendMessage(ns, send);
            }

            PartTimeJobCount++;
        }
        else if(stArrayData[0] == "4")
        {
            if (DoubleGoldFlag)
            {
                PopUpText.ChangeText(int.Parse(stArrayData[0]), 2 * (int.Parse(stArrayData[1])));
                gold += 2 * (int.Parse(stArrayData[1]));
                DoubleGoldFlag = false;

                if (CardMenu.GetCard(2) > 0)
                {
                    Use2.SetTrue();
                }
            }
            else
            {
                PopUpText.ChangeText(int.Parse(stArrayData[0]), int.Parse(stArrayData[1]));
                gold += int.Parse(stArrayData[1]);
            }

            //ゴールドの変更をサーバに送信
            if (useServer) {
                base.sendMessage(ns, "3" + gold.ToString());
            }
        }

        if (stArrayData[0] != "0")
        {
            MassEffectPopUp.SetTrue();
            Masses.ChangeColor(0, place);
        }
        Masses.ChangeMass(place, "10");

        //盤面情報の変更をサーバに送信
        if (useServer) {
            string sendMsg = "";
            for (int i = 0; i < 20; i++) {
                sendMsg += Masses.board[i];
                if (i != 19) {
                    sendMsg += ":";
                }
            }
            base.sendMessage(ns, "2" + sendMsg);
        }
    }

    public int GetGold()
    {
        return gold;
    }

    public void DoubleMove()
    {
        DoubleMoveFlag = true;
    }

    public void DoubleGold()
    {
        DoubleGoldFlag = true;
    }

    public int GetStamina()
    {
        return stamina;
    }

    public void ReduceStamina()
    {
        stamina -= 1;

        //スタミナをサーバに送信
        if (useServer) {
            string send = "4";
            send += stamina.ToString();
            base.sendMessage(ns, send);
        }
    }

    public bool GetDoubleGoldFlag()
    {
        return DoubleGoldFlag;
    }

    public bool GetDoubleMoveFlag()
    {
        return DoubleMoveFlag;
    }
}
