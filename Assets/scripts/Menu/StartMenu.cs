using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour {

    public Canvas settingstMenu;
    public Button exit; 
    public Button settings;

    // Use this for initialization
    void Start()
    {

        /*settingstMenu = settingstMenu.GetComponent<Canvas>();
        exit = exit.GetComponent<Button>();
        settings = settings.GetComponent<Button>();
        settingstMenu.enabled = false;*/

    }

    public void SettingsPress()
    {
        settingstMenu.enabled = true;
        exit.enabled = false;
        settings.enabled = false;
    }

    public void noPress() 
    {
        settingstMenu.enabled = false;
        exit.enabled = true;
        settings.enabled = true;

    }

    public void changeSomething() // zmacknuti nejakeho tlacitka
    {
    }

    public void exitGame() // pokus aby to neco delalo jinak to tu nebude
    {
        Application.Quit();
    }

	public void restartGame()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
