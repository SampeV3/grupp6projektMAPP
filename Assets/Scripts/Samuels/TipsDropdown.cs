using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsDropdown : MonoBehaviour
{

    [SerializeField] GameObject Walk;
    [SerializeField] GameObject Shoot;

    
    public void HandleChoosenTips(int tipValue)
    {
        if (tipValue == 0)
        {
            Walk.SetActive(true);
        }
        if (tipValue == 1)
        {

        }
        if (tipValue == 2)
        {

        }
        if (tipValue == 3)
        {

        }
        if (tipValue == 4)
        {

        }
        if (tipValue == 5)
        {

        }
    }
}
