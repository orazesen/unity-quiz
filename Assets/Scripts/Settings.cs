using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Settings : MonoBehaviour
    {
        private PrefManager prefManager;
        public Image soundImg;
        public Sprite soundMuteSprite;
        public Sprite soundSprite;
        public Image musicImg;
        public Sprite musicMuteSprite;
        public Sprite musicSprite;
        public Dropdown timerDropdown;
        public List<string> timers;

        private bool sound;
        private bool music;
        private int timer;

        public AudioSource musicSource;
        public AudioSource soundSource;

        private bool isMusicOn = false;


        // Start is called before the first frame update
        public void Start()
        {
            prefManager = GetComponent<PrefManager>();
            sound = prefManager.Sound;
            music = prefManager.Music;
            timer = prefManager.Timer;

            InitTimer();
            soundImg.sprite = (sound) ? soundSprite : soundMuteSprite;
            musicImg.sprite = (music) ? musicSprite : musicMuteSprite;

            soundSource.mute = !sound;

            if (music)
            {
                musicSource.Play();
            }
            else
            {
                musicSource.Stop();
            }
        }

        void InitTimer()
        {
            timerDropdown.ClearOptions();
            timerDropdown.AddOptions(timers);
            timerDropdown.value = timer;
        }

        public void Sound()
        {
            sound = !sound;
            Debug.Log(sound);
            soundImg.sprite = (sound) ? soundSprite : soundMuteSprite;
            prefManager.Sound = sound;
            soundSource.mute = !sound;
        }

        public void Music()
        {
            music = !music;
            Debug.Log(music);
            musicImg.sprite = (music) ? musicSprite : musicMuteSprite;
            prefManager.Music = music;
            IsMusicOn = music;
        }

        public void Timer(int time)
        {
            timer = time;
            SetTimer();
        }

        public void SetTimer()
        {
            prefManager.Timer = timerDropdown.value;
        }

        private bool IsMusicOn
        {
            get
            {
                return isMusicOn;
            }
            set
            {
                isMusicOn = value;
                if (isMusicOn)
                {
                    musicSource.Play();
                }
                else
                {
                    musicSource.Stop();
                }
            }
        }
    }
}
