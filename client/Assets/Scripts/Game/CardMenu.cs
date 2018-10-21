using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

public class CardMenu : Client {

    bool useServer = StartUp.useServer;
    NetworkStream ns;
    public Player Player;
    public Masses Masses;
    public Use0 Use0;
    public Use1 Use1;
    public Use2 Use2;
    public Use3 Use3;
    public Card3Error Card3Error;
    public RouletteController RouletteController;
    int[] card = { 0, 0, 0, 0 };

    public void Start()
    {
        gameObject.SetActive(false);

        /* サーバを使う場合 */
        if (useServer) {
            this.ns = StartUp.ns;
            base.sendMessage(ns, "6");
            do {
                base.receiveMessage(ns);
            } while (base.recMsg == "");
            SetCard(base.recMsg.Remove(0, 1));
        }
        else {
            SetCard("30,3,3,1");
        }
    }

    public void In()
    {
        if (RouletteController.GetRotMoveFlag())
        {
            gameObject.SetActive(true);
        }
    }

    public void Out()
    {
        gameObject.SetActive(false);
    }

    public int GetCard(int offset)
    {
        return card[offset];
    }

    public void SetCard(int offset)
    {
        if(card[offset] == 0)
        {
            if(offset == 0)
            {
                Use0.SetTrue();
            }
            else if(offset == 1 && !Player.GetDoubleMoveFlag())
            {
                Use1.SetTrue();
            }
            else if(offset == 2 && !Player.GetDoubleGoldFlag())
            {
                Use2.SetTrue();
            }
            else if(offset == 3)
            {
                Use3.SetTrue();
            }
        }
        card[offset] += 1;

	    /* サーバを使う場合 */
        if (useServer) {
            string send = "";
            foreach (int ele in card) {
                    string str = ele.ToString();
            send += "," + str;
            }
            send = send.Remove(0, 1);
            send = "6" + send;
            base.sendMessage(ns, send);
        }
    }

    public void Card0(int moveCount)
    {
        gameObject.SetActive(false);
        RouletteController.SetRotMoveFlag(false);
        Player.Move(moveCount);
        card[0] -= 1;

        if(card[0] == 0)
        {
            Use0.SetFalse();
        }
    }

    public void Card1()
    {
        gameObject.SetActive(false);
        Player.DoubleMove();
        card[1] -= 1;

        Use1.SetFalse();
    }

    public void Card2()
    {
        gameObject.SetActive(false);
        Player.DoubleGold();
        card[2] -= 1;

        Use2.SetFalse();
    }

    public void Card3()
    {
        gameObject.SetActive(false);

        int[] whiteMass1 = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };
        int k = 0;
        for (int i = 1; i < 20; i++)
        {
            if(Masses.GetBoardColor(i) == 0)
            {
                whiteMass1[k] = i;
                k++;
            }
        }

        int[] whiteMass2 = new int[k];
        for(int i=0; i<k; i++)
        {
            whiteMass2[i] = whiteMass1[i];
        }

        int[] whiteMass3 = whiteMass2.OrderBy(i => Guid.NewGuid()).ToArray();
        if (whiteMass2.Length == 1)
        {
            Masses.ChangeMass(whiteMass3[0], "4,5000,5000G");
            Masses.ChangeColor(4, whiteMass3[0]);
        }
        else if(whiteMass2.Length == 0)
        {
            Card3Error.SetTrue();
            card[3]++;
        }
        else
        {
            Masses.ChangeMass(whiteMass3[0], "4,5000,5000G");
            Masses.ChangeColor(4, whiteMass3[0]);
            Masses.ChangeMass(whiteMass3[1], "4,5000,5000G");
            Masses.ChangeColor(4, whiteMass3[1]);
        }

        card[3] -= 1;

        if (card[3] == 0)
        {
            Use3.SetFalse();
        }
    }

    public void SetCard(string strCard)
    {
        string[] card = strCard.Split(',');

        for (int i = 0; i<card.Length; i++)
        {
            this.card[i] = int.Parse(card[i]);
        }
    }
}
