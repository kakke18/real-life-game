using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour {

    public void SetFalse()
    {
        gameObject.SetActive(false);
    }

    public void SetTrue()
    {
        gameObject.SetActive(true);
    }
}
