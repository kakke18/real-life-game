using System.Collections;
using UnityEngine;

public class RouletteController : MonoBehaviour {
    public Player Player;
    public GameObject roulette;
    public StartButton StartButton;
    int num;
    float rotSpeed = 0, rotRotationZ = 0;
    bool rotMoveFlag = true, rotStopFlag = false;   //rotMoveFlag:回せる状態かどうか  rotStopFlag:ブレーキをかけてるか

    void Update () {
        if (rotSpeed >= 0.05)
        {
            rotRotationZ += rotSpeed;                                           //ルーレト回転ここから
            if(rotRotationZ >= 342)
            {
                rotRotationZ -= 360;
            }
            transform.rotation = Quaternion.Euler(0, 0, rotRotationZ);         //ここまで

            if (rotStopFlag)
            {
                rotSpeed *= Random.Range(0.80f, 0.99f);
            }
        }

		if (rotSpeed <= 0.05 && rotSpeed > 0) {
            num = (int)( (rotRotationZ + 18) /36)+1;
            Player.Move(num);
            Debug.Log(num.ToString());
            rotSpeed = 0;
        }
	}

    public void RotStart()
    {
        if (rotMoveFlag && Player.GetStamina() > 0)
        {
            StartButton.SetFalse();
            rotSpeed = 20;
            rotMoveFlag = false;
            Player.ReduceStamina();
        }
    }

    public void RotStop()
    {
        if (!rotMoveFlag)
        {
            rotStopFlag = true;
            StartButton.SetTrue();
        }
    }

    public void RotMoveFinish()
    {
        rotMoveFlag = true;
        rotStopFlag = false;
    }

    public bool GetRotMoveFlag()
    {
        return rotMoveFlag;
    }

    public void SetRotMoveFlag(bool flag)
    {
        rotMoveFlag = flag;
    }
}
