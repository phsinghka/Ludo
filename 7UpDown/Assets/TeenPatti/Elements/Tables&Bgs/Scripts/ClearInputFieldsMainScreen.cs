using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearInputFieldsMainScreen : MonoBehaviour
{
    public InputField[] allInputFields;
    public Text errorText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartOtpSection(){
        FirebaseOTP.Instance.StartOtp();
    }
    public void ClearInputFields(){ 
        errorText.text = "";
        foreach (InputField item in allInputFields)
        {
            item.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
