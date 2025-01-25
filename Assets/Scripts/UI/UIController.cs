using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class UIController : MonoBehaviour
{
    public GameObject InfoScreen;

    public TextMeshProUGUI InfoText;


    public GameObject GameUI; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideInfoScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetGameUI(bool gameUI)
    {
        GameUI.SetActive(gameUI);
    }

    public void HideInfoScreen()
    {
        InfoScreen.SetActive(false);
    }


    public void ShowMessage(string message)
    {
        InfoScreen.SetActive(true);
        InfoText.text = message;
    }
}
