using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class controls all navigation activities
public class Navigation : MonoBehaviour
{
    //Unity objects
    public GameObject MainPanel;
    public GameObject ScoresPanel;
    public GameObject SettingsPanel;
    public GameObject GamePanel;
    public GameObject gameModeBtns;
    public GameObject gameCategoryBtns;
    public GameObject gameWindow;
    public GameObject resumeBtn;
    public Sprite backSprite;
    public Sprite leaveSprite;
    public Button backBtn;
    public List<Action> actions = new List<Action>();

    //Custom Scripts
    private PrefManager prefManager;
    private ConfirmationController message;
    private GamePlay quizPlay;
    private CategorySelector selector;

    void Start()
    {
        prefManager = GetComponent<PrefManager>();
        message = GetComponent<ConfirmationController>();
        quizPlay = GetComponent<GamePlay>();
        selector = GetComponent<CategorySelector>();
    }


    public void ShowMain()
    {
        MainPanel.SetActive(true);
        ScoresPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        GamePanel.SetActive(false);
    }

    public void StartGame()
    {
        MainPanel.SetActive(false);
        GamePanel.SetActive(true);
        gameModeBtns.SetActive(true);
        gameCategoryBtns.SetActive(false);
        gameWindow.SetActive(false);
        selector.playBtn.SetActive(false);

        if (prefManager.LeavedGameQuestionCount == -1)
        {
            resumeBtn.SetActive(false);
        }
        else
        {
            resumeBtn.SetActive(true);
        }
        Actions = StartGame;
    }

    public void ShowGameCategories()
    {
        gameModeBtns.SetActive(false);
        gameCategoryBtns.SetActive(true);
        gameWindow.SetActive(false);
        
        Actions = ShowGameCategories;
    }

    public void ShowGameWindow()
    {
        gameModeBtns.SetActive(false);
        gameCategoryBtns.SetActive(false);
        gameWindow.SetActive(true);
        Actions = ShowGameWindow;
    }

    public void ShowProfile()
    {
        ScoresPanel.SetActive(true);
        Actions = ShowProfile;
    }

    public void ShowSettings()
    {
        SettingsPanel.SetActive(true);
        Actions = ShowSettings;
    }
    
    public void GoBack()
    {
        if (actions.Count > 0)
        {
            if (actions[actions.Count - 1] == ShowGameWindow && quizPlay.State == GamePlay.GameState.ONPLAY)
            {
                BackToCategories();
                quizPlay.State = GamePlay.GameState.ONPAUSE;
            }
            else if (actions[actions.Count - 1] == ShowGameCategories)
            {
                selector.ResetButtons();
                ClosePanel();
            }
            else
            {
                ClosePanel();
            }
        }
        else
        {
            message.ShowMessage("Do you want to exit?", Application.Quit, "Yes", null, "No");
        }
    }
    

    public void BackToCategories()
    {
        quizPlay.gameEndingPanel.SetActive(false);
        actions.RemoveAt(actions.Count - 1);
        quizPlay.State = GamePlay.GameState.NOTSTARTED;
        GoBack();
    }
    

    public void ClosePanel()
    {
        actions.RemoveAt(actions.Count - 1);
        if (actions.Count > 0)
        {
            actions[actions.Count - 1].Invoke();
            actions.RemoveAt(actions.Count - 1);
        }
        if (actions.Count == 0)
        {
            backBtn.GetComponent<Image>().sprite = leaveSprite;
        }
        else
        {
            backBtn.GetComponent<Image>().sprite = backSprite;
        }
    }


    private Action Actions
    {
        get
        {
            return actions[actions.Count - 1];
        }
        set
        {
            if (actions.Count == 0)
            {
                actions.Add(ShowMain);
            }
            actions.Add(value);
            backBtn.GetComponent<Image>().sprite = backSprite;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GoBack();
        }
    }
}
