using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("------------------- Sound And Music ---------------------")]
    public Image OnSound;
    public Image OffSound , OnMusic , OffMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundButtionClick()
    {
        if(PlayerPrefs.GetInt("Sound") != 1)
        {
            //Sound off(1 == Sound Off)
            OnSound.gameObject.SetActive(false);
            OffSound.gameObject.SetActive(true);

            PlayerPrefs.SetInt("Sound",1);
        }
        else
        {
            //Sound On(0 == Sound On)
            OnSound.gameObject.SetActive(true);
            OffSound.gameObject.SetActive(false);

            PlayerPrefs.SetInt("Sound",0);
        }
    }

    public void MusicButtionClick()
    {
        if(PlayerPrefs.GetInt("Music") != 1)
        {
            //Music off(1 == Music Off)
            OnMusic.gameObject.SetActive(false);
            OffMusic.gameObject.SetActive(true);

            PlayerPrefs.SetInt("Music",1);
        }
        else
        {
            //Music On(0 == Music On)
            OnMusic.gameObject.SetActive(true);
            OffMusic.gameObject.SetActive(false);

            PlayerPrefs.SetInt("Music",0);
        }
    }
}
