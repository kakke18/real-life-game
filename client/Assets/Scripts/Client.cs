/*
	サーバとの送受信用クラス
 */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class Client : MonoBehaviour {

	MemoryStream ms;
	Encoding enc = Encoding.UTF8;
	bool connectFlag = false;
	bool receiveFlag = false;
	protected string recMsg = "";

	public virtual void sendMessage (NetworkStream ns, string msg) {
		byte[] sendBytes = enc.GetBytes(msg + '\n');
		ns.Write(sendBytes, 0, sendBytes.Length);
		Debug.Log("send:" + msg);	
	}

	public virtual void receiveMessage (NetworkStream ns) {
		byte[] recBytes = new byte[256];
		int recSize = 0;

		//Receive message
		while (ns.DataAvailable) {
			ms = new MemoryStream();
			recSize = ns.Read(recBytes, 0, recBytes.Length);

			if (recSize == 0) {
				Debug.Log("サーバが切断しました");
				break;
			}

			ms.Write(recBytes, 0, recSize);

			if (recBytes[recSize - 1] == '\n') {
				receiveFlag = true;
				break;
			}
		}
		if (receiveFlag) {
			//Covert the received data to a character string
			recMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
			ms.Close();
			//Delete '\n'
			recMsg = recMsg.TrimEnd('\n');
			Debug.Log("receive:" + recMsg);
			receiveFlag = false;
		}
	}
}
