using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    static TMP_Text messageBox;
    private void Awake()
    {
        messageBox = GetComponent<TMP_Text>();
        if (messageBox != null )
        {
            Debug.Log("Couldn't get TMP text field");
        }
    }
    public static void PutTextInMessageBox(string text)
    {
        messageBox.text = text;
    }
}
