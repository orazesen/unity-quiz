using Assets.Scripts;
using Assets.Scripts.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    //Game objects
    public Text normalChallengeScoreText;
    public Text randomChallengeScoreText;
    public Text againstTheClockScore;
    private PrefManager prefManager;

    private int normalHighScore = 0;
    private int randomHighScore = 0;
    private int againstTheClockHighScore = 0;

    void Start()
    {
        prefManager = GetComponent<PrefManager>();
        LoadData();
    }

    public void LoadData()
    {
        if (prefManager.AmINewUser == 0)
        {
            prefManager.AmINewUser = 1;
            prefManager.Sound = true;
            prefManager.Music = true;
            prefManager.Timer = 0;
            prefManager.LeavedGameQuestionCount = -1;
        }

        againstTheClockHighScore = prefManager.AgainstTheClock;
        normalHighScore = prefManager.NormalChallengeScore;
        randomHighScore = prefManager.RandomChallengeScore;
    }

    // Sets scores to profile
    public void SetUp()
    {
        normalChallengeScoreText.text = randomHighScore + "%";
        randomChallengeScoreText.text = normalHighScore + "%";
        againstTheClockScore.text = againstTheClockHighScore + "%";
    }
}
