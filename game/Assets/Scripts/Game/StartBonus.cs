using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBonus : MonoBehaviour {
    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void Bonus()
    {
        gameObject.SetActive(true);
    }

    public void FinishBonus()
    {
        gameObject.SetActive(false);
    }
}
