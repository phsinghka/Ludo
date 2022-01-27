using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class MainDataManager : MonoBehaviour
{
    public static MainDataManager Instance;
    public Sprite[] avatars;
    public float[] tableBootValues;
    public TeenPatti.RoomType teenPattiRoomType; 
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    public string userName
    {
        get { return PlayerPrefs.GetString("userName", ""); }
        set { PlayerPrefs.SetString("userName", value); }
    }
    public string login_user_name
    {
        get { return PlayerPrefs.GetString("login_user_name", ""); }
        set { PlayerPrefs.SetString("login_user_name", value); }
    }
    public string id
    {
        get { return PlayerPrefs.GetString("id", ""); }
        set { PlayerPrefs.SetString("id", value); }
    }
    public string _id
    {
        get { return PlayerPrefs.GetString("_id", ""); }
        set { PlayerPrefs.SetString("_id", value); }
    }
    public string password
    {
        get { return PlayerPrefs.GetString("password", ""); }
        set { PlayerPrefs.SetString("password", value); }
    }
    public string referralId
    {
        get { return PlayerPrefs.GetString("referralId", ""); }
        set { PlayerPrefs.SetString("referralId", value); }
    }
    public string uniqueId
    {
        get { return PlayerPrefs.GetString("uniqueId", ""); }
        set { PlayerPrefs.SetString("uniqueId", value); }
    }
    public string mobile
    {
        get { return PlayerPrefs.GetString("mobile", ""); }
        set { PlayerPrefs.SetString("mobile", value); }
    }
    public string token
    {
        get { return PlayerPrefs.GetString("token", ""); }
        set { PlayerPrefs.SetString("token", value); }
    }
    public float playPoint
    {
        get { return PlayerPrefs.GetFloat("playPoints", 0); }
        set { PlayerPrefs.SetFloat("playPoints", value); }
    }
    public string profile_url
    {
        get { return PlayerPrefs.GetString("profile_url", ""); }
        set { PlayerPrefs.SetString("profile_url", value); }
    }
    public float safePoint
    {
        get { return PlayerPrefs.GetFloat("safePoints", 0); }
        set { PlayerPrefs.SetFloat("safePoints", value); }
    }
    public int avatarId
    {
        get { return PlayerPrefs.GetInt("avatarId", 0); }
        set { PlayerPrefs.SetInt("avatarId", value); }
    }
    public string lastGameID
    {
        get { return PlayerPrefs.GetString("lastGameID", ""); }
        set { PlayerPrefs.SetString("lastGameID", value); }
    }
    public int game_winning
    {
        get { return PlayerPrefs.GetInt("game_winning", 0); }
        set { PlayerPrefs.SetInt("game_winning", value); }
    }
    public int table_id
    {
        get { return PlayerPrefs.GetInt("table_id", 0); }
        set { PlayerPrefs.SetInt("table_id", value); }
    }
}


[Serializable]
public class RegisterData
{
    /*public string userName;
    public string id;
    public string password;
    public string referralId;
    public string uniqueId;
    public string mobile;
    public string token;
    public float playPoint;
    public float safePoint;
    public int avatarId;*/

    public string _id;
    public string id;
    public string display_user_name;
    public string unique_id;
    public float chips;
    public int game_winning;
    public int table_id;
    public bool rejoin;
    public string profile_url;


    public string password;
    public string referralId;
    public string mobile;
    public string token;
    public float safePoint;
    public int avatarId;


}
