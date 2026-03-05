using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    Button newGameButton;
    Button continueButton;
    Button exitGameButton;
    void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        continueButton = transform.GetChild(2).GetComponent<Button>();
        exitGameButton = transform.GetChild(3).GetComponent<Button>();

        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(Continue);
        exitGameButton.onClick.AddListener(ExitGame);
    }
    void NewGame()
    {
        Debug.Log("New Game");
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();
    }
    void Continue()
    {
        Debug.Log("Continue");
        // 转换场景，读取进度
        SceneController.Instance.TransitionToPreviousLevel();
    }
    void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }
}
