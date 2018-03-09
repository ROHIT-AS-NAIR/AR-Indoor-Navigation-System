using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateDisplayController : MonoBehaviour
{
    /* change app bar, app text and toast */

	private GameObject actionBar;
    private ToastMessageScript toastMessageScript;
    private SoundManager soundManager;
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
        soundManager = GameObject.Find("ARCamera").GetComponent<SoundManager>();
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

    public void PlaySound(SoundManager.SoundType soundtype)
    {
        soundManager.Play(soundtype);
    }

    /* get roataion and defind sound type */
    public SoundManager.SoundType GetSoundDirection(float zRotation)
    {
        if(zRotation > 180){ zRotation = zRotation - 360;}
        float abszRotation = Mathf.Abs(zRotation % 360f);
        if(abszRotation < 30)
        {
            return SoundManager.SoundType.GoBack;
        }
        else if(abszRotation > 150)
        {
            return SoundManager.SoundType.GoAhead;
        }
        else
        {
            if(zRotation < 0)
            {
                return SoundManager.SoundType.TurnRight;
            }
            else if(zRotation > 0)
            {
                return SoundManager.SoundType.TurnLeft;
            }
        }

        return SoundManager.SoundType.GoAhead;
    }
}
