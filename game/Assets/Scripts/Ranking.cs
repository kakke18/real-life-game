using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class Ranking : Client {

	bool useServer = StartUp.useServer;

	NetworkStream ns;
	Text titleText; 
	Text[] buttonText;
	Button selectButton, updateButton;
	GameObject item, content;
	public GameObject prefab;
	bool nowFlag = true;
	bool updateFlag = false;
	string ranking = "";

	// Use this for initialization
	void Start () {
		/* サーバを使う場合 */
		if (useServer) {
			this.ns = StartUp.ns;
		}

		//部品
		titleText = GameObject.Find("TitleText").GetComponent<Text>();
		selectButton = GameObject.Find("SelectButton").GetComponent<Button>();
		buttonText = selectButton.gameObject.GetComponentsInChildren<Text>();
		updateButton = GameObject.Find("UpdateButton").GetComponent<Button>();
		content = GameObject.Find("Content");

		//ラベル変更
		ChangeLabel();

		//ランキングを表示
		GetRanking();
		DispRanking();
	}

	/* ランキングを取得 */
	private void GetRanking () {
		/* サーバを使う場合 */
		if (useServer) {
			string send;

			if (nowFlag) {
				send = "50";
			}
			else {
				send = "51";
			}
			base.sendMessage(ns, send); //サーバにランキングを要求

			//受信するまで無限ループ
			int i = 0;
			while (i++ < 3000) {
				base.receiveMessage(ns);
			}
			ranking = base.recMsg;
			ranking = ranking.Remove(0, 1); //5を取り除く
		}
		/* サーバを使わない場合 */
		else {
			if (updateFlag) {
				ranking = "aaa,10000,bbb,9000,ccc,8000,ddd,7000,eee,6000,fff,5000,ggg,4000,hhh,3000,iii,2000,jjj,1000";
			}
			else if (nowFlag) {
				ranking = "a,10000,b,9000,c,8000,d,8000,e,6000,f,6000,g,6000,h,3000,i,2000,j,1000";
			}
			else {
				ranking = "aa,10000,bb,9000,cc,8000,dd,7000,ee,6000,ff,5000,gg,4000,hh,3000,ii,2000,jj,1000";
			}
		}
	}

	/* ランキングを初期化 */
    private void InitRanking () { 
        foreach (Transform n in content.transform) { 
            GameObject.Destroy(n.gameObject);
        }
    }

	/* ランキングを表示 */
	private void DispRanking () {
		string[] rankAry = ranking.Split(',');
		int rankNum = rankAry.Length / 2;
		int nowGold = 0;
		int pastGold = -1; //1つ上の順位の人のゴールド

		for (int i = 0; i < rankNum; i++) {
			//プレファブを取得
            item = (GameObject)Instantiate(prefab);
			//親を指定
            item.transform.SetParent(content.transform);
			item.transform.localScale = Vector3.one;
            //Textを取得
            Text[] itemText = item.GetComponentsInChildren<Text>();
            //Textをセット
			nowGold = int.Parse(rankAry[i * 2 + 1]);
			//ゴールド数が同じなら
			if (nowGold == pastGold) {
				itemText[0].text = "";
			}
			else {
            	itemText[0].text = (i + 1).ToString() + "位";
			}
            itemText[1].text = rankAry[i * 2];
			itemText[2].text = rankAry[i * 2 + 1] + "G";
			pastGold = nowGold;
		}
	}

	/* タイトル，ボタン，更新ボタンを変更 */
	private void ChangeLabel () {
		if (nowFlag) {
			titleText.text = "今週のランキング";
			buttonText[0].text = "先週のランキングへ";
		}
		else {
			titleText.text = "先週のランキング";
			buttonText[0].text = "今週のランキングへ";
		}

		updateButton.gameObject.SetActive(nowFlag);
	}

	/* クリック時の操作 */
	public void OnClick (string type) {
		if (type.Equals("change")) {
			nowFlag = !nowFlag;
		}
		else if (type.Equals("update")) {
			updateFlag = true;
		}

		ChangeLabel();
		GetRanking();
		InitRanking();
		DispRanking();
	}
}
