using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoginRegisterManager : MonoBehaviour
{
    //Input Fields
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    private UserInfoList userInfoList;
    private string userSavedFile = "Resources/userData.json"; //path to save our json file
    public TextMeshProUGUI registerloginText; //our registerLoginText refrence
    public GameObject loginRegisterPanel; //our LoginRegister Panel Refrence
    public Toggle showPasswordToggle;
    [SerializeField] private int minimumUsernameCharacters;
    [SerializeField] private int maximumUsernameCharacters;
    [SerializeField] private int minimumPasswordCharacters;
    [SerializeField] private int maximumPasswordCharacters;
    
    private void Start()
    {
        userInfoList = new UserInfoList();
        LoadUserData(); //load the user information
        //disable and enable the correct compoennts
        registerloginText.gameObject.SetActive(false);
        loginRegisterPanel.SetActive(true);
        showPasswordToggle.isOn = false;
    }


    public void Register()
    {
        //we get the values from the input fields
        string registerUsername = usernameInputField.text;
        string registerPassword = passwordInputField.text;
        //if the username exists
        if (doesUserExistRegister(registerUsername))
        {
            Debug.Log("Name already exists");
            ShowText();
            Invoke("HideText", 3f);
            registerloginText.text = "Name already exists ";
            loginRegisterPanel.SetActive(true);
        }
        //we check if the length of the username is within minimum and maximum characters length we set
        else if (registerUsername.Length < minimumUsernameCharacters || registerUsername.Length > maximumUsernameCharacters)
        {
            Debug.Log("invalid username length");
            ShowText();
            Invoke("HideText", 3f);
            registerloginText.text = "Invalid username length,it must be between " + minimumUsernameCharacters + " and " + maximumUsernameCharacters + " characters)";
            loginRegisterPanel.SetActive(true);
        }
        //we check if the length of the password is within minimum and maximum characters length we set
        else if (registerPassword.Length < minimumPasswordCharacters || registerPassword.Length > maximumPasswordCharacters)
        {
            Debug.Log("invalid password length");
            ShowText();
            Invoke("HideText", 3f);
            registerloginText.text = "Invalid password length,it must be between " + minimumPasswordCharacters + " and " + maximumPasswordCharacters + " characters)";
            loginRegisterPanel.SetActive(true);
        }
        else
        //we create a new user and register him
        {
            UserInfo newUser = new UserInfo()
            {
                username = registerUsername,
                password = registerPassword,
            };
            Debug.Log("Registered New User");
            ShowText();
            registerloginText.text = "Registered New User,Also Welcome " + registerUsername;
            loginRegisterPanel.SetActive(false);
            userInfoList.usersInfoList.Add(newUser);
            SaveUserData();
            AssetDatabase.Refresh();
        }
    }

    public void Login()
    {
        //we get the values from the input fields
        string loginUsername = usernameInputField.text;
        string loginPassword = passwordInputField.text;
        //we check if the username and password are the same
        if (doesUserExistLogin(loginUsername,loginPassword))
        {
            Debug.Log("You are now logged in");
            ShowText();
            registerloginText.text = "You are now logged in " +loginUsername;
            loginRegisterPanel.SetActive(false);
        }
        //if they don't match we print it in the console and the text 
        if (!doesUserExistLogin(loginUsername, loginPassword))
        {
            Debug.Log("Wrong Username or Password");
            ShowText();
            Invoke("HideText", 3f);
            registerloginText.text = "Wrong Username or Password";
        }
    }
    //function to check if the username is already taken and exists in our json file
    private bool doesUserExistRegister(string username)
    {
        return userInfoList.usersInfoList.Exists(user => user.username == username);
    }
    //function to check if the username and password match in our json file
    private bool doesUserExistLogin(string username,string password)
    {
        return userInfoList.usersInfoList.Exists(user => user.username == username && user.password == password);
    }
    //function to save our user data to a json file
    private void SaveUserData()
    {
        string filePath = Path.Combine(Application.dataPath, userSavedFile);
        string json = JsonUtility.ToJson(userInfoList);
        File.WriteAllText(filePath, json);
    }
    //function to load our user data from a a json file
    private void LoadUserData()
    {
        string filePath = Path.Combine(Application.dataPath, userSavedFile);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            userInfoList = JsonUtility.FromJson<UserInfoList>(json);
        }
    }
    //function that enables the registerlogin gameobject
    private void ShowText()
    {
        registerloginText.gameObject.SetActive(true);
    }
    //function that disables the registerlogin gameobject
    private void HideText()
    {
        registerloginText.gameObject.SetActive(false);
    }
    //here we check if the toggle is on or off and change the password input field content type accordindly
    private void Update()
    {
        if (showPasswordToggle.isOn)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        passwordInputField.ForceLabelUpdate();
    }

    //our user information
    [System.Serializable]
    public class UserInfo
    {
        public string username;
        public string password;
    }
    //our user info list
    [System.Serializable]
    public class UserInfoList
    {
        public List<UserInfo> usersInfoList = new List<UserInfo>();
    }
}