using UnityEngine;
using System.Collections;
using Assets.scripts;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public GameObject mainCanvas;
	public GameObject settingsCanvas;
	public GameObject gamesetCanvas;

	public Dropdown classDropdown;
	public Dropdown difficultyDropdown;
	public Dropdown gametypeDropdown;

	// Use this for initialization
	void Start () 
	{
		mainCanvas.SetActive(true);
		settingsCanvas.SetActive(false);
		gamesetCanvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (settingsCanvas.activeSelf)
			{
				Options();
			}

			if (gamesetCanvas.activeSelf)
			{
				StartNewGame();
			}
		}
	}

	public void CreateGame()
	{
		string selectedClass = classDropdown.captionText.text;
		string difficulty = difficultyDropdown.captionText.text;
		string gameType = gametypeDropdown.captionText.text;
		string profile = "Lukas";

		GameSession.className = selectedClass;
		GameSession.difficulty = difficulty;
		GameSession.gameType = gameType;
		GameSession.profileName = profile;

		SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
		SceneManager.LoadScene("gamewindow");
		GUIUtility.ExitGUI();
	}

	public void StartNewGame()
	{
		if (gamesetCanvas.activeSelf)
		{
			gamesetCanvas.SetActive(false);
			mainCanvas.SetActive(true);
		}
		else
		{
			mainCanvas.SetActive(false);
			gamesetCanvas.SetActive(true);
		}
	}

	public void Options()
	{
		if (settingsCanvas.activeSelf)
		{
			settingsCanvas.SetActive(false);
			mainCanvas.SetActive(true);
		}
		else
		{
			mainCanvas.SetActive(false);
			settingsCanvas.SetActive(true);
		}
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
