using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseOTP : MonoBehaviour
{
    public static FirebaseOTP Instance;
    public InputField inputFieldsMobNumber;
    public InputField[] inputFields;
    public Button SubmitButton;
    public InputField OtpEntered;
    public Text GetOtpToVerify;
    public Button GetOtpToVerifyBtn;
    public Text errorText;
    FirebaseAuth firebaseAuth;
    private uint phoneAuthTimeoutMs = 60 * 1000;
    public string verificationId;

    PhoneAuthProvider provider;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        verificationId = null;
        GetOtpToVerifyBtn.interactable = true;
        GetOtpToVerify.text = "GET OTP";
        firebaseAuth = FirebaseAuth.DefaultInstance;
        SubmitButton.interactable = false;
        inputFieldsMobNumber.readOnly = false;
        OtpEntered.readOnly = false;
        GetOtpToVerifyBtn.onClick.RemoveAllListeners();
        GetOtpToVerifyBtn.onClick.AddListener(() => GetOtp());
        foreach (InputField item in inputFields)
        {
            item.readOnly = true;
        }
    }

    public void StartOtp(){
        verificationId = null;
        GetOtpToVerifyBtn.interactable = true;
        GetOtpToVerify.text = "GET OTP";
        firebaseAuth = FirebaseAuth.DefaultInstance;
        SubmitButton.interactable = false;
        inputFieldsMobNumber.readOnly = false;
        OtpEntered.readOnly = false;

        GetOtpToVerifyBtn.onClick.RemoveAllListeners();
        GetOtpToVerifyBtn.onClick.AddListener(() => GetOtp());
        foreach (InputField item in inputFields)
        {
            item.readOnly = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetOtp()
    {
        if(inputFieldsMobNumber.text.Length<10){
            errorText.text = "Please Enter Valid Number";
        }
        else{
            provider = PhoneAuthProvider.GetInstance(firebaseAuth);
            provider.VerifyPhoneNumber("+91" + inputFieldsMobNumber.text, phoneAuthTimeoutMs, null,
            verificationCompleted: (credential) =>
            {
                // Auto-sms-retrieval or instant validation has succeeded (Android only).
                // There is no need to input the verification code.
                // `credential` can be used instead of calling GetCredential().
            },
            verificationFailed: (error) =>
            {
                errorText.text = "Please Try Again Later";
                Debug.Log(error);
                GetOtpToVerify.text = "GET OTP";
            GetOtpToVerifyBtn.onClick.RemoveAllListeners();

                GetOtpToVerifyBtn.onClick.AddListener(() => GetOtp());


                // The verification code was not sent.
                // `error` contains a human readable explanation of the problem.
            },
            codeSent: (id, token) =>
            {
                verificationId = id;
                errorText.text = "Code Send Successfull";
                Debug.Log("Code send " + id);
                GetOtpToVerify.text = "VERIFY";
            GetOtpToVerifyBtn.onClick.RemoveAllListeners();

                GetOtpToVerifyBtn.onClick.AddListener(() => VerifyOTP());
                // Verification code was successfully sent via SMS.
                // `id` contains the verification id that will need to passed in with
                // the code from the user when calling GetCredential().
                // `token` can be used if the user requ ests the code be sent again, to
                // tie the two requests together.
            },
            codeAutoRetrievalTimeOut: (id) =>
            {
                // Called when the auto-sms-retrieval has timed out, based on the given
                // timeout parameter.
                // `id` contains the verification id of the request that timed out.
            });
        }
    }

    public void VerifyOTP()
    {
    #if UNITY_EDITOR
    inputFieldsMobNumber.readOnly = true;
    foreach (InputField item in inputFields)
    {
        item.readOnly = false;
    }
    SubmitButton.interactable = true;
    GetOtpToVerify.text = "VERIFIED";
    OtpEntered.readOnly = true;
    GetOtpToVerifyBtn.interactable = false;
    #endif
    #if UNITY_ANDROID
    Credential credential = provider.GetCredential(verificationId, OtpEntered.text);
    firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            errorText.text = "Entered Otp is Faulted";
            GetOtpToVerify.text = "GET OTP";
    GetOtpToVerifyBtn.onClick.RemoveAllListeners();

            GetOtpToVerifyBtn.onClick.AddListener(() => GetOtp());
            return;
        }
        else if(task.IsCompleted){
            inputFieldsMobNumber.readOnly = true;
            foreach (InputField item in inputFields)
            {
                item.readOnly = false;
            }
            SubmitButton.interactable = true;
            GetOtpToVerify.text = "VERIFIED";
            OtpEntered.readOnly = true;
            GetOtpToVerifyBtn.interactable = false;

        }
        FirebaseUser newUser = task.Result;
        Debug.Log(("User signed in successfully")
        + ("Phone number: " + newUser.PhoneNumber) + ("Phone provider ID: " + newUser.ProviderId) + task.Result);


    });
    #endif
    }
}
