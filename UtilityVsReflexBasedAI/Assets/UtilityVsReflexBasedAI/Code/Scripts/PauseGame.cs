using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AweDev.Utilities
{
    public class PauseGame : MonoBehaviour
    {
        [SerializeField] private bool _pauseOnStart;
        [SerializeField] private bool _isPaused;

        public static event Action<float> OnGameSpeedChanged;

        private void Init()
        {
            if (_pauseOnStart)
            {
                _isPaused = true;
                Time.timeScale = 0;
                OnGameSpeedChanged?.Invoke(Time.timeScale);
            }
        }

        private void Start()
        {
            Init();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isPaused = !_isPaused;

                Time.timeScale = _isPaused ? 0 : 1;
                OnGameSpeedChanged?.Invoke(Time.timeScale);
            }
        }
    }
}
