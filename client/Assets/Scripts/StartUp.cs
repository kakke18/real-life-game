using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class StartUp : MonoBehaviour {

	//public static bool useServer = false; 
	public static bool useServer = true; 

	TcpClient tcp;
	public static NetworkStream ns;

	InputField ip, port;
	Button registration, login, connect;
	bool connectFlag = false;

	string ipAddress;
	int portNum;

	// Use this for initialization
	void Start () {
		ip = GameObject.Find("IPInputField").GetComponent<InputField>();
		port = GameObject.Find("PortInputField").GetComponent<InputField>();
		connect = GameObject.Find("ConnectButton").GetComponent<Button>();
		registration = GameObject.Find("RegistButton").GetComponent<Button>();
		login = GameObject.Find("LoginButton").GetComponent<Button>();
		
		registration.gameObject.SetActive(false);
		login.gameObject.SetActive(false);
	}

	void Update () {
		if (connectFlag) {
			ip.gameObject.SetActive(false);
			port.gameObject.SetActive(false);
			connect.gameObject.SetActive(false);

			registration.gameObject.SetActive(true);
			login.gameObject.SetActive(true);
		}
	}

	public void OnClick () {
		/* サーバを使う場合 */
		if (useServer) {
			if (ip.text != "" && port.text != "") {
				ipAddress = ip.text;
				portNum = int.Parse(port.text);

				try {
					tcp = new TcpClient(ipAddress, portNum);
					connectFlag = true;
					ns = tcp.GetStream();
				}
				catch {
					Debug.Log("connect error");
					return;
				}
			}
		}
		/* サーバを使わない場合 */
		else {
			connectFlag = true;
		}
	}
}
