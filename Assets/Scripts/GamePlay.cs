using Assets.Scripts;
using Assets.Scripts.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls flow of game
public class GamePlay : MonoBehaviour
{
    // Unity Objects
    public GameObject categoryPanel;
    public GameObject questionPanel;
    public Text timeText;
    public Text scoreText;
    public Text questionText;
    public Text categoryText;
    public Button[] answers;
    public Text trueCount;
    public Text falseCount;
    public GameObject gameEndingPanel;
    public Text gameEndingText;
    public GameObject[] gameEndingStars;
    public Image gameEndingSlider;
    public Text gameEndingSliderText;
    public Animator starsAnimator;
    public Color defaultColor;
    public AudioSource btnClickSound;
    public AudioSource backgroundMusic;
    public AudioSource wonSound;
    public AudioSource lostSound;

    //Custom Scripts
    private SQLiteController controller;
    private List<Question> questions;
    private Question question;
    private PrefManager prefManager;
    private Navigation navigation;
    private Profile profile;
    private CategorySelector selector;
    private Settings settings;

    // C# variables
    public float time = 0f;
    private int index;
    private bool answered = false;
    private GameState state = GameState.NONE;
    private GameMode mode = GameMode.NONE;
    private int questionCount = 0;
    private int trueAnswers;
    private int falseAnswers;
    private static int addingTime = 3;
    private static int subtractingTime = 6;
    private static float delayBtwQuestions = .9f;
    public static float BACKGROUND_MUSIC_NORMALIZE_DELAY = 2f;

    public void StartGame(QuestionCategory category)
    {
        Debug.Log(category);
        if (category == QuestionCategory.None)
        {
            Debug.Log("getting random questions");
            questions = controller.GetRandomQuestions();
            
        }
        else
        {
            questions = controller.GetChallengeQuestions(category);
        }

        if (questions.Count == 0)
        {
            Debug.Log("This question type is empty");
        }
        else
        {
            navigation.ShowGameWindow();
            ResetUI();
            NextQuestion();
        }
    }

    void ResetUI()
    {
        questionText.text = "";
        
        categoryText.text = "";
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i].enabled = true;
            answers[i].GetComponentInChildren<Text>().text = "";
            answers[i].GetComponent<Image>().color = defaultColor;
        }
        TrueAnswers = 0;
        FalseAnswers = 0;
        State = GameState.ONPLAY;
        QuestionCount = 0;
    }
    
    public void ContinueGame()
    {
        State = GameState.ONPLAY;
    }

    // Start is called before the first frame update
    void Start()
    {
        settings = GetComponent<Settings>();
        controller = GetComponent<SQLiteController>();
        profile = GetComponent<Profile>();
        prefManager = GetComponent<PrefManager>();
        selector = GetComponent<CategorySelector>();
        navigation = GetComponent<Navigation>();
        SetButtons();
        ResetUI();
    }

    void SetButtons()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i].onClick.RemoveAllListeners();
            int j = i;
            answers[i].onClick.AddListener(() => Answer(j));
        }
    }

    void NextQuestion()
    {
        if (State == GameState.ENDED)
        {
            return;
        }
        if (questions.Count <= 0)
        {
            State = GameState.ENDED;
            return;
        }
        if (Mode != GameMode.CLOCK && QuestionCount == 10)
        {
            State = GameState.ENDED;
            return;
        }

        int random = UnityEngine.Random.Range(0, questions.Count);
        Index = random;
        answered = false;
        QuestionCount++;
    }

    public int QuestionCount
    {
        get
        {
            return questionCount;
        }
        set
        {
            questionCount = value;
            if (Mode != GameMode.CLOCK && questionCount > 10)
            {
                State = GameState.ENDED;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetTime();
    }
    
    public float Timer
    {
        get
        {
            return time;
        }
        set
        {
            time = value;
            int i = (int)time;
            string s = "";
            if (i < 10)
            {
                s = "00:0" + i;
            }
            else if (i < 60)
            {
                s = "00:" + i;
            }
            else if(i >= 60)
            {
                s = "0" + (int)(i / 60) + ":";
                if (i % 60 < 10)
                {
                    s += "0" + i % 60;
                }
                else
                {
                    s += i % 60 + "";
                }
            }
            timeText.text = s + "";
        }
    }

    void SetQuestion()
    {
        questionText.text = question.Q;
        
        foreach (Category cat in controller.categories)
        {
            if (cat.categoryId == question.Type)
            {
                categoryText.text = cat.name;
            }
        }

        answers[0].GetComponentInChildren<Text>().text = question.A;
        answers[1].GetComponentInChildren<Text>().text = question.B;
        answers[2].GetComponentInChildren<Text>().text = question.C;
        answers[3].GetComponentInChildren<Text>().text = question.D;
        State = GameState.ONPLAY;
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i].enabled = true;
        }
    }

    void Answer(int i)
    {
        btnClickSound.Play();
        if (answered)
        {
            return;
        }
        for (int j = 0; j < answers.Length; j++)
        {
            answers[j].enabled = false;
        }
        answered = true;
        if (i == question.TrueAnswer - 1)
        {
            if (Mode == GameMode.CLOCK)
            {
                Timer += addingTime;
            }            
            StartCoroutine(IAnswerTrue(question.TrueAnswer - 1));
        }
        else
        {
            if (Mode == GameMode.CLOCK)
            {
                Timer -= subtractingTime;
            }
            StartCoroutine(IAnswerFalse(question.TrueAnswer - 1, i));
        }
    }

    IEnumerator IAnswerTrue(int i)
    {
        TrueAnswers++;
        answers[i].GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(delayBtwQuestions);
        yield return new WaitUntil(() => State == GameState.ONPLAY);
        questions.Remove(question);
        NextQuestion();
    }

    IEnumerator IAnswerFalse(int trueAnswer, int givenAnswer)
    {
        FalseAnswers++;
        answers[trueAnswer].GetComponent<Image>().color = Color.green;
        answers[givenAnswer].GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(delayBtwQuestions);
        yield return new WaitUntil(() => State == GameState.ONPLAY);
        questions.Remove(question);
        NextQuestion();
    }
    

    int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
            question = questions[index];
            StartCoroutine(IResetAll());
        }
    }

    IEnumerator IResetAll()
    {
        yield return new WaitForSeconds(.3f);
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i].GetComponent<Image>().color = defaultColor;
        }
        SetQuestion();
    }

    public GameState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            switch (state)
            {
                case GameState.NOTSTARTED:
                    break;
                case GameState.ONPLAY:
                    break;
                case GameState.ONPAUSE:
                    SaveCurrentGame();
                    break;
                case GameState.ENDED:
                    answered = true;
                    ShowGameEnding();
                    break;
                case GameState.NONE:
                    break;
                default:
                    break;
            }
        }
    }

    public void PauseTheGame()
    {
        State = GameState.ONPAUSE;
    }

    void SaveCurrentGame()
    {
        prefManager.LeavedGameQuestionCount = QuestionCount;
        prefManager.LeavedGameFalseAnswer = FalseAnswers;
        prefManager.LeavedGameTrueAnswer = TrueAnswers;
        prefManager.LeavedGameTime = (int)Timer;
        prefManager.LeavedGameMode = (int)Mode;
        prefManager.LeavedGameCategory = (int)(selector.category);
    }
    
    void SaveGame()
    {
        if (Mode == GameMode.NORMAL)
        {
            if (prefManager.NormalChallengeScore < TrueAnswers * 10)
            {
                prefManager.NormalChallengeScore = TrueAnswers * 10;
            }            
        }
        else if (Mode == GameMode.RANDOM)
        {
            if (prefManager.RandomChallengeScore < TrueAnswers * 10)
            {
                prefManager.RandomChallengeScore = TrueAnswers * 10;
            }            
        }
        else if (Mode == GameMode.CLOCK)
        {
            float score = (float)TrueAnswers / Int32.Parse(settings.timers[prefManager.Timer].Replace(" s", ""));
            score *= 100;

            if (score > 100)
            {
                score = 100;
            }

            if (score > prefManager.AgainstTheClock)
            {
                prefManager.AgainstTheClock = TrueAnswers * 10;
            }
        }
        profile.LoadData();
        prefManager.LeavedGameQuestionCount = -1;
    }

    void ShowGameEnding()
    {
        if (TrueAnswers >= 5)
        {
            gameEndingText.text = "You Won!";
            wonSound.Play();
        }
        else
        {
            gameEndingText.text = "You Lost!";
            lostSound.Play();
        }

        backgroundMusic.volume = 0.5f;
        StartCoroutine(INormalizeBackgroundMusic());
        gameEndingSlider.fillAmount = 0f;

        gameEndingPanel.SetActive(true);

        for (int i = 0; i < gameEndingStars.Length; i++)
        {
            gameEndingStars[i].SetActive(false);
        }
        StartCoroutine(ISetSlider());
    }

    IEnumerator INormalizeBackgroundMusic()
    {
        yield return new WaitForSeconds(BACKGROUND_MUSIC_NORMALIZE_DELAY);
        backgroundMusic.volume = 1f;
    }

    IEnumerator ISetSlider()
    {
        yield return new WaitForSeconds(.4f);
        float value = gameEndingSlider.fillAmount;
        gameEndingSliderText.text = 0 + "%";

        float score = 0;
        if (Mode == GameMode.CLOCK)
        {
            score = (float)TrueAnswers / Int32.Parse(settings.timers[prefManager.Timer].Replace(" s", ""));

            score *= 100;

            if (score > 100)
            {
                score = 100;
            }
        }
        else
        {
            score = TrueAnswers * 10;
        }

        while (value < score)
        {
            value++;
            gameEndingSlider.fillAmount = value / 100;
            gameEndingSliderText.text = (int)value + "%";
            yield return null;
        }

        starsAnimator.SetTrigger("stars");

        yield return null;
        if (value >= 50 && value <= 70)
        {
            gameEndingStars[0].SetActive(true);
        }
        else if (value > 70 && value <= 90)
        {
            gameEndingStars[0].SetActive(true);
            gameEndingStars[1].SetActive(true);
        }
        else if (value > 90)
        {
            gameEndingStars[0].SetActive(true);
            gameEndingStars[1].SetActive(true);
            gameEndingStars[2].SetActive(true);
        }

        SaveGame();
    }

    public void CloseGameEnding()
    {
        gameEndingPanel.GetComponent<Animator>().SetTrigger("fade_in");
        StartCoroutine(IClose());
    }

    IEnumerator IClose()
    {
        yield return new WaitForSeconds(.5f);
        navigation.BackToCategories();
    }

    void SetTime()
    {
        if (State == GameState.ONPLAY)
        {
            switch (Mode)
            {
                case GameMode.NORMAL:
                    Timer += Time.deltaTime;
                    break;
                case GameMode.RANDOM:
                    Timer += Time.deltaTime;
                    break;
                case GameMode.CLOCK:
                    Timer -= Time.deltaTime;
                    if (Timer <= 0)
                    {
                        State = GameState.ENDED;
                        Timer = 0f;
                    }
                    break;
            }
        }
    }

    public GameMode Mode
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
        }
    }

    public int TrueAnswers
    {
        get
        {
            return trueAnswers;
        }
        set
        {
            trueAnswers = value;
            trueCount.text = "Dogry: " + trueAnswers;
            scoreText.text = "Utuk: " + trueAnswers * 10 + "%";
        }
    }

    public int FalseAnswers
    {
        get
        {
            return falseAnswers;
        }
        set
        {
            falseAnswers = value;
            falseCount.text = "Yalnys: " + falseAnswers;
        }
    }

    public enum GameState
    {
        NOTSTARTED,
        ONPLAY,
        ONPAUSE,
        ENDED,
        NONE
    }
    
    public enum GameMode
    {
        NORMAL,
        RANDOM,
        CLOCK,
        NONE
    }
}
