using Assets.Scripts;
using Assets.Scripts.Constants;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// This class is responsible to handle category selection
public class CategorySelector : MonoBehaviour
{
    //Custom Scripts
    private PrefManager prefManager;
    private SQLiteController controller;
    private GamePlay quiz;
    private Navigation navigation;
    private ConfirmationController message;
    private Settings settings;

    //Unity Objects
    public GameObject playBtn;
    public Transform categoryBtns;
    public Text chosenCategoryText;    
    public AudioSource btnClickSound;

    //Enums
    public QuestionCategory category = QuestionCategory.None;

    //Variables
    public Color reddishColor;
    public float startSpeed;
    public float deceleration;
    public bool stop = true;

    private float time = 0f;    
    private float timeOut;    
    private int currentBtnIndex = -1;
    private int lastBtnIndex = -1;
    public GameObject categoryBtnPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GetCustomScripts();
    }

    //Instantiating all category buttons
    public void SetCategoryBtns(List<Category> categories)
    {
        foreach (var item in categories)
        {
            GameObject catBtn = Instantiate(categoryBtnPrefab, categoryBtns);
            catBtn.GetComponent<CategoryHolder>().type = item.categoryId;
            catBtn.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(Directories.CategorySprites + item.sprite);
        }        
    }

    //Getting instance of custom scripts
    void GetCustomScripts()
    {
        controller = GetComponent<SQLiteController>();
        message = GetComponent<ConfirmationController>();
        quiz = GetComponent<GamePlay>();
        prefManager = GetComponent<PrefManager>();
        navigation = GetComponent<Navigation>();
        settings = GetComponent<Settings>();
    }

    // Resume button calls this function to continue incomplete game
    public void ResumGame()
    {
        category = QuestionCategory.None;
        quiz.State = GamePlay.GameState.NOTSTARTED;
        quiz.Timer = prefManager.LeavedGameTime;
        quiz.TrueAnswers = prefManager.LeavedGameTrueAnswer;
        quiz.FalseAnswers = prefManager.LeavedGameFalseAnswer;
        quiz.Mode = (GamePlay.GameMode)prefManager.LeavedGameMode;
        quiz.QuestionCount = prefManager.LeavedGameQuestionCount;
        category = (QuestionCategory)prefManager.LeavedGameCategory;
        StartGame();
        SetCategoryText(category);
    }

    // Normal Challenge button calls this function to play normal game (that player can choose his/her own gameplay)
    public void NormalChallenge()
    {
        category = QuestionCategory.None;
        quiz.Timer = 0f;
        quiz.Mode = GamePlay.GameMode.NORMAL;
        quiz.State = GamePlay.GameState.NOTSTARTED;
        navigation.ShowGameCategories();
        chosenCategoryText.text = "";

        for (int i = 0; i < categoryBtns.childCount; i++)
        {
            Button currentBtn = categoryBtns.GetChild(i).GetComponent<Button>();
            currentBtn.onClick.RemoveAllListeners();
            int k = i;
            currentBtn.onClick.AddListener(() => SetButtonColors(k));
            currentBtn.enabled = true;
        }
    }

    // Random Challenge button calls this function
    public void RandomChallenge()
    {
        chosenCategoryText.text = "";
        category = QuestionCategory.None;
        quiz.State = GamePlay.GameState.NOTSTARTED;
        quiz.Timer = 0f;
        quiz.Mode = GamePlay.GameMode.RANDOM;
        navigation.ShowGameCategories();

        for (int i = 0; i < categoryBtns.childCount; i++)
        {
            CategoryHolder holder = categoryBtns.GetChild(i).GetComponent<CategoryHolder>();
            Button currentBtn = categoryBtns.GetChild(i).GetComponent<Button>();
            currentBtn.enabled = false;
        }
        timeOut = UnityEngine.Random.Range(2f, 6f);
        stop = false;
    }

    // Against the clock challenge buttons calls this function
    public void AgainstTheClock()
    {
        chosenCategoryText.text = "";
        category = QuestionCategory.None;
        quiz.State = GamePlay.GameState.NOTSTARTED;
        if (settings.timers == null)
        {
            Debug.Log("Timers are null");
        }
        quiz.Timer = Int32.Parse(settings.timers[prefManager.Timer].Replace(" s", ""));
        quiz.Mode = GamePlay.GameMode.CLOCK;
        navigation.ShowGameCategories();
        for (int i = 0; i < categoryBtns.childCount; i++)
        {
            Button currentBtn = categoryBtns.GetChild(i).GetComponent<Button>();
            currentBtn.onClick.RemoveAllListeners();
            int k = i;
            currentBtn.onClick.AddListener(() => SetButtonColors(k));
            currentBtn.enabled = true;
        }
    }
    public void MixedChallenge()
    {
        category = QuestionCategory.None;
        quiz.Timer = 0f;
        quiz.Mode = GamePlay.GameMode.NORMAL;
        quiz.State = GamePlay.GameState.NOTSTARTED;

        StartGame();
    }

    // Sets current child button color and resets previously selected button color
    void SetButtonColors(int currentChildIndex)
    {
        btnClickSound.Play();
        CategoryHolder holder = categoryBtns.GetChild(currentChildIndex).GetComponent<CategoryHolder>();

        if (lastBtnIndex != - 1)
        {
            Image prevButton = categoryBtns.GetChild(lastBtnIndex).GetComponent<Image>();
            prevButton.color = reddishColor;
            prevButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }

        lastBtnIndex = currentChildIndex;

        SetCategoryText(holder.type);

        category = holder.type;
        categoryBtns.GetChild(currentChildIndex).GetComponent<Image>().color = Color.white;
        categoryBtns.GetChild(currentChildIndex).GetChild(0).GetComponent<Image>().color = reddishColor;
        playBtn.SetActive(true);
    }


    // This function sets chosen category text at the top middle
    void SetCategoryText(QuestionCategory categ)
    {
        var category = controller.categories.Where(c => c.categoryId == categ).FirstOrDefault();
        chosenCategoryText.text = category == null ? "" : chosenCategoryText.text = category.name;
    }

    // This function Resets category selection window
    public void ResetButtons()
    {
        if (lastBtnIndex != -1)
        {
            Image prevButton = categoryBtns.GetChild(lastBtnIndex).GetComponent<Image>();
            prevButton.color = reddishColor;
            prevButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            lastBtnIndex = -1;
            stop = true;
            currentBtnIndex = -1;
            time = 0f;
            startSpeed = .2f;
            chosenCategoryText.text = "";
        }
    }

    // Update is called once per frame
    // Random category selection by time
    void Update()
    {
        if (!stop)
        {
            time += Time.deltaTime;
            if (time >= startSpeed)
            {
                timeOut -= time;
                if (timeOut <= 2)
                {
                    startSpeed += deceleration;
                }
                if (timeOut <= 0)
                {
                    stop = true;
                    SetButton();
                    return;
                }
                time = 0f;
                SetButton();
            }
        }
    }

    // Sets category buttons color in random challenge
    void SetButton()
    {
        //current child index in category buttons
        currentBtnIndex++;

        int prevBtnIndex = categoryBtns.childCount - 1;

        if (currentBtnIndex >= categoryBtns.childCount)
        {
            currentBtnIndex = 0;
        }
        if (currentBtnIndex > 0)
        {
            prevBtnIndex = currentBtnIndex - 1;
        }

        Image currentBtn = categoryBtns.GetChild(currentBtnIndex).GetComponent<Image>();
        CategoryHolder _category = currentBtn.GetComponent<CategoryHolder>();
        Image oldBtn = categoryBtns.GetChild(prevBtnIndex).GetComponent<Image>();

        currentBtn.color = Color.white;
        currentBtn.transform.GetChild(0).GetComponent<Image>().color = reddishColor;

        oldBtn.color = reddishColor;
        oldBtn.transform.GetChild(0).GetComponent<Image>().color = Color.white;

        lastBtnIndex = currentBtnIndex;

        category = _category.type;
        SetCategoryText(_category.type);

        if (stop)
        {
            playBtn.SetActive(true);
        }
    }

    // Starts game by chosen category
    public void StartGame()
    {
        if (!stop)
        {
            return;
        }

        quiz.StartGame(category);

        //if (category == QuestionCategory.None)
        //{
        //    message.ShowSingleButton("Choose your category.");
        //}
        //else
        //{
        //    quiz.StartGame(category);
        //}
    }
}
