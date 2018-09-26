using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private List<AudioClip> _clips;

        public int menuMusic;
        public int battleMusic;
        public int buttonClick;
        public int shot;
        public int wallHit;
        public int shipHit;

        [SerializeField]
        private AudioSource _effectSource;

        [SerializeField]
        private AudioSource _musicSource;

        public static AudioManager Instance { get; private set; }

        // Use this for initialization
        void Awake()
        {
            Instance = Instance ?? (this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayClip(int clip, bool loop)
        {
            PlaySomething(_effectSource, clip, loop);
        }

        public void PlayMusic(int clip, bool loop)
        {
            PlaySomething(_musicSource, clip, loop);
        }
        
        private void PlaySomething(AudioSource source, int clip, bool loop)
        {
            if (clip < 0 || clip >= this._clips.Count)
            {
                return;
            }

            if (!loop)
            {
                source.PlayOneShot(_clips[clip], 1);
            }
            else
            {
                source.loop = true;
                source.clip = _clips[clip];
                source.Play();
            }
        }
    }
}