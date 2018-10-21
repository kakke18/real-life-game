using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class Login : Client {

	bool useServer = StartUp.useServer;

	int errorTest; //Debug用
	
	NetworkStream ns;

	InputField userName, passWord;
	Text errorMsg;
	Image erroImg;
	SceneController sc = new SceneController();

	bool blankFlag = false;
	public static string name = ""; 

	// Use this for initialization
	void Start () {
		/* サーバを使わ場合 */
		if (useServer) {
			this.ns = StartUp.ns;
		}

		userName = GameObject.Find("UserNameInputField").GetComponent<InputField>();
		passWord = GameObject.Find("PassWordInputField").GetComponent<InputField>();
		erroImg = GameObject.Find("ErrorImage").GetComponent<Image>();
		errorMsg = GameObject.Find("ErrorMessage").GetComponent<Text>();

		erroImg.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		/* サーバを使う場合 */
		if (useServer) {
			if(blankFlag) {
				blankFlag = false;
				erroImg.gameObject.SetActive(true);
				errorMsg.text = "空白の箇所があります";
				userName.ActivateInputField();
			}

			base.receiveMessage(ns); //メッセージを受信
			if (base.recMsg != "") {			
				if (base.recMsg.Contains("11")) {
					sc.MyPage();
				}
				/* Login error */
				else if (base.recMsg.Contains("101")) {
					erroImg.gameObject.SetActive(true);
					errorMsg.text = "このユーザ名は存在しません";
					userName.ActivateInputField();
				}
				else if (base.recMsg.Contains("102")) {
					erroImg.gameObject.SetActive(true);
					errorMsg.text = "ユーザ名とパスワードが一致しません";
					userName.ActivateInputField();
				}
			}
		}
	}

	public void OnClick () {
		string pass, sendMsg;

		name = userName.text;
		pass = passWord.text;

		/* サーバを使う場合 */
		if (useServer) {
			if (name != "" && pass != "") {
				sendMsg = "1" + name + "\n" + pass;
				base.sendMessage(ns, sendMsg);
			}
			else {
				blankFlag = true;
			}
		}
		/* サーバを使わない場合 */
		else {
			if (int.TryParse(name, out errorTest)) {
				if(errorTest == 0) {
					erroImg.gameObject.SetActive(true);
					errorMsg.text = "未記入の箇所があります";
					userName.ActivateInputField();
				}
				else if (errorTest == 1) {
					erroImg.gameObject.SetActive(true);
					errorMsg.text = "ユーザ名とパスワードが一致しません";
					userName.ActivateInputField();
				}
				else if (errorTest == 2) {
					erroImg.gameObject.SetActive(true);
					errorMsg.text = "このユーザ名は存在しません";
					userName.ActivateInputField();
				}
				else {
					sc.MyPage();
				}
			}
			else {
				Debug.Log("Please input number on name");
			}
		}
	}
}
