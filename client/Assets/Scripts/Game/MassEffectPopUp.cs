using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassEffectPopUp : MonoBehaviour {

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetFalse()
    {
        gameObject.SetActive(false);
    }

    public void SetTrue()
    {
        gameObject.SetActive(true);
    }
}
