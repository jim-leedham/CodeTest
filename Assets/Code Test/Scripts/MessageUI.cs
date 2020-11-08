using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Text messageTitle;
    [SerializeField] private Text messageDesc;

    public void ShowUI(string title, string message, Color messageColor, float time = 1.5f)
    {
        messageTitle.text = title;
        messageDesc.text = message;
		messageDesc.color = messageColor;
		gameObject.SetActive(true);
        if(time > 0.0f)
        {
            StartCoroutine(HideUI(time));
        }
    }

    public IEnumerator HideUI(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
