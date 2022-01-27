using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneManager : MonoBehaviour
{
    public RectTransform StarGB;
    public void Start(){
        StarGB.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    public void StartTeenPatti()
    {
        PhotonConnectionLogin.Instance.TeenPattiJoinRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // Update the Text on the screen depending on current position of the touch each frame
            StarGB.position = touch.position;
            StarGB.gameObject.SetActive(true);
            Invoke("StopAnimation",0.5f);


        }
        if(Input.GetButtonDown("Fire1")){
            Vector3 mousePos = Input.mousePosition;
            StarGB.position = mousePos;
            StarGB.gameObject.SetActive(true);
            Invoke("StopAnimation",0.5f);


        }
        
    }
    void StopAnimation(){
        StarGB.gameObject.SetActive(false);
    }
}
