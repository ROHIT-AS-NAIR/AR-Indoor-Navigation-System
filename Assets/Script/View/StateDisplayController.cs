using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateDisplayController : MonoBehaviour
{
    /* change app bar, app text and toast */

	private GameObject actionBar;
    private ToastMessageScript toastMessageScript;
	private Image actionBarImg;
	private Text appNameText;

    // Use this for initialization
    void Start()
    {
		InitStateDisplay();
    }

	void Awake()
	{
		InitStateDisplay();
	}

    // Update is called once per frame
    void Update()
    {

    }

    private void InitStateDisplay()
    {
		actionBar = gameObject.GetComponent<CanvasButtonScript>().actionBar;
        toastMessageScript = gameObject.GetComponent<ToastMessageScript>();
        actionBarImg = actionBar.GetComponent<Image>();
        appNameText = actionBar.gameObject.transform.Find("AppName").gameObject.GetComponent<Text>();
    }

    public void ChangeActionBarColor(MainController.AppState color)
    {
        switch (color)
        {
            case MainController.AppState.Idle:
                actionBarImg.color = new Color32(60, 126, 255, 255);
                break;
            case MainController.AppState.Navigate:
                actionBarImg.color = new Color32(126, 60, 255, 255);
                break;
            default: actionBarImg.color = new Color32(60, 126, 255, 255); break;
        }
    }

	public void ShowToastMessage(string toastmsg)
	{
		toastMessageScript.showToastOnUiThread(toastmsg, true);
	}

	public void ShowToastMessage(string toastmsg, bool isshort)
	{
		toastMessageScript.showToastOnUiThread(toastmsg, isshort);
	}

    public void ChangeActionText(string actext)
    {
        appNameText.text = actext;
    }
}
