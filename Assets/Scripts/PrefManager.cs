using Assets.Scripts;
using Assets.Scripts.Constants;
using UnityEngine;

//Su klas PlayerPref-i kontrol edya
public class PrefManager : MonoBehaviour
{
    private Settings settings;

    // Start is called before the first frame update
    void Start()
    {
        settings = GetComponent<Settings>();
    }

    public int AmINewUser
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.NEW_USER, 0);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.NEW_USER, value);
            PlayerPrefs.Save();
        }
    }


    public bool Sound
    {
        get
        {
            bool isSoundOn = (PlayerPrefs.GetInt(Prefs.SOUND) == 1) ? true : false;
            return isSoundOn;
        }
        set
        {
            int isSoundOn = (value) ? 1 : 0;
            PlayerPrefs.SetInt(Prefs.SOUND, isSoundOn);
            PlayerPrefs.Save();
        }
    }


    public bool Music
    {
        get
        {
            bool isMusicOn = (PlayerPrefs.GetInt(Prefs.MUSIC) == 1) ? true : false;
            return isMusicOn; 
        }
        set
        {
            int isMusicOn = (value) ? 1 : 0;
            PlayerPrefs.SetInt(Prefs.MUSIC, isMusicOn);
            PlayerPrefs.Save();
        }
    }


    public int Timer
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.TIME,0);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.TIME, value);
            PlayerPrefs.Save();
        }
    }


    public int AgainstTheClock
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.TIME_CHALLENGE_SCORE, 0);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.TIME_CHALLENGE_SCORE, value);
            PlayerPrefs.Save();
        }
    }
    public int NormalChallengeScore
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.NORMAL_CHALLENGE_SCORE, 0);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.NORMAL_CHALLENGE_SCORE, value);
            PlayerPrefs.Save();
        }
    }

    public int RandomChallengeScore
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.RANDOM_CHALLENGE_SCORE, 0);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.RANDOM_CHALLENGE_SCORE, value);
            PlayerPrefs.Save();
        }
    }


    public int LeavedGameTime
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.LEAVED_GAME_TIME);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.LEAVED_GAME_TIME, value);
            PlayerPrefs.Save();
        }
    }


    public int LeavedGameTrueAnswer
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.LEAVED_GAME_TRUE_ANSWERS);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.LEAVED_GAME_TRUE_ANSWERS, value);
            PlayerPrefs.Save();
        }
    }


    public int LeavedGameFalseAnswer
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.LEAVED_GAME_FALSE_ANSWERS);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.LEAVED_GAME_FALSE_ANSWERS, value);
            PlayerPrefs.Save();
        }
    }


    public int LeavedGameQuestionCount
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.LEAVED_GAME_QUESTION_COUNT);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.LEAVED_GAME_QUESTION_COUNT, value);
            PlayerPrefs.Save();
        }
    }


    public int LeavedGameMode
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.LEAVED_GAME_MODE);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.LEAVED_GAME_MODE, value);
            PlayerPrefs.Save();
        }
    }

    public int LeavedGameCategory
    {
        get
        {
            return PlayerPrefs.GetInt(Prefs.LEAVED_GAME_CATEGORY);
        }
        set
        {
            PlayerPrefs.SetInt(Prefs.LEAVED_GAME_CATEGORY, value);
            PlayerPrefs.Save();
        }
    }
}
