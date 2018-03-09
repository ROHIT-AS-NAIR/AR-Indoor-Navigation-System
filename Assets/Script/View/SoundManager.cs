using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public string[] audioFileToLoadName = {
		"01 init_app", // ส่องกล้องไปยังจุดต่างๆตามอาคาร เพื่อระบุตำแหน่งของคุณ
		"02 init_fornav", // ส่องกล้องไปยังจุดต่างๆตามอาคาร เพื่อระบุตำแหน่ง และเริ่มต้นการนำทาง
        "03 located", // ระบุตำแหน่งแล้ว
		"04 start_nav", // เริ่มต้นการนำทางแล้ว
		"08 cancle_nav", // ยกเลิกการนำทางแล้ว
		"09 reached", // ถึงปลายทางแล้ว
		"10 end_nav", // สิ้นสุดการนำทาง
		"11 turn_right", // เลี้ยวขวา
		"12 turn_left", // เลี้ยวซ้าย
		"13 go_back", // ไปด้านหลัง
		"14 go_ahead" // ตรงไปด้านหน้า
		};
	
	public enum SoundType
	{
		InitApp,
		InitFornav,
		Located,
		StartNav,
		CancleNav,
		Reached,
		EndNav,
		TurnRight,
		TurnLeft,
		GoBack,
		GoAhead
	}
	
	private AudioClip[] audioClipList;
	AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
		if(audioSource == null)
		{
			InitSoundManager();
		}
    }

	void Awake()
	{
		if(audioSource == null)
		{
			InitSoundManager();
		}
	}

    // Update is called once per frame
    void Update()
    {

    }

	/* init and load sound to list */
	private void InitSoundManager()
	{
		audioSource = this.gameObject.GetComponent<AudioSource>();
		audioClipList = new AudioClip[audioFileToLoadName.Length];
		for (int i = 0; i < audioFileToLoadName.Length; i++)
		{
			audioClipList[i] = (AudioClip) Resources.Load("Sounds/" + audioFileToLoadName[i]);
		}
		
	}

	public void Play(SoundType soundtype)
	{
		if(audioSource == null)
		{
			InitSoundManager();
		}
		AudioClip clip = audioClipList[(int)soundtype];
		audioSource.PlayOneShot(clip);
	}
}
