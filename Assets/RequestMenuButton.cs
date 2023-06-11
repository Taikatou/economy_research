using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class RequestMenuButton : MonoBehaviour
{
    void Start()
    {
        if (TrainingConfig.AdventurerNoRequestMenu)
        {
            gameObject.SetActive(false);
        }
    }
}
