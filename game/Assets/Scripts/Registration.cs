using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class Registration : Client {

	bool useServer = StartUp.useServer;

	int errorTest; //Debug用
	
	NetworkStream ns;
	InputField userName, pass, confirmPass;
	Text errorMsg;
	Image errorImg;
	SceneController sc = new SceneController();
	public static string name = "";

	bool blankFlag = false;
	bool matchFlag = false;

	// Use this for initialization
	void Start () {
		/* サーバを使う場合 */
		if (useServer) {
			this.ns = StartUp.ns;
		}

		userName = GameObject.Find("UserNameInputField").GetComponent<InputField>();
		pass = GameObject.Find("PassWordInputField").GetComponent<InputField>();
		confirmPass = GameObject.Find("PassWordInputField2").GetComponent<InputField>();
		errorImg = GameObject.Find("ErrorImage").GetComponent<Image>();
		errorMsg = GameObject.Find("ErrorMessage").GetComponent<Text>();

		errorImg.gameObject.SetActive(false);
		userName.ActivateInputField();
	}
	
	// Update is called once per frame
	void Update () {
		/* サーバを使う場合 */
		if (useServer) {
			if(blankFlag) {
				blankFlag = false;
				errorImg.gameObject.SetActive(true);
				errorMsg.text = "未記入の箇所があります";
				userName.ActivateInputField();
			}
			if(matchFlag) {
				matchFlag = false;
				errorImg.gameObject.SetActive(true);
				errorMsg.text = "確認用パスワードが一致しません";
				confirmPass.ActivateInputField();
			}

			base.receiveMessage(ns); //メッセージを受信
			if (base.recMsg != "") {
				if (base.recMsg.Contains("01")) {
					//string mass = "10:1,0,カード1:2,0,ルーレット権:3,0,多めのゴールド:4,5000,5000G:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0";
					//string card = "100,100,100,100";
					string mass = "10:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0";
					string card = "0,0,0,0";
					//string gold = "10000";

					base.sendMessage(ns, "2" + mass);
					//base.sendMessage(ns, "3" + gold);
					base.sendMessage(ns, "6" + card);
					sc.SystemDescription();
				}
				/* Registration error */
				else if (base.recMsg.Contains("00")) {
					errorImg.gameObject.SetActive(true);
					errorMsg.text = "このユーザ名は既に存在します";	
					userName.ActivateInputField();
				}
			}
		}
	}

	public void OnClick() {
		string pass1, pass2, sendMsg;

		name = userName.text;
		pass1 = pass.text;
		pass2 = confirmPass.text;

		/* サーバを使う場合 */
		if (useServer) {
			if (name != "" && pass1 != "" && pass2 != "") {
				if (!pass1.Equals(pass2)) {
					matchFlag = true;
				}
				else {
					sendMsg = "0" + name + "\n" + pass1;
					base.sendMessage(ns, sendMsg);
				}
			}
			else {
				blankFlag = true;
			}
		}
		/* サーバを使わない場合 */
		else {
			if (int.TryParse(name, out errorTest)) {
				errorImg.gameObject.SetActive(true);
				if(errorTest == 0) {
					errorMsg.text = "未記入の箇所があります";
					userName.ActivateInputField();
				}
				else if (errorTest == 1) {
					errorMsg.text = "確認用パスワードが一致しません";
					userName.ActivateInputField();	
				}
				else if (errorTest == 2) {
					errorMsg.text = "このユーザ名は既に存在します";
					userName.ActivateInputField();
				}
				else {
					sc.SystemDescription();
				}
			}
			else {
				Debug.Log("Please input number on name");
			}
		}
	}
}
