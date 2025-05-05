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
        [SerializeField] private float _initialSpeed = 1f; // Initial speed when the game starts

        private float _previousSpeed;

        public static event Action<float> OnGameSpeedChanged;

        private void Init()
        {
            _previousSpeed = _initialSpeed;

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

                if (_isPaused)
                {
                    _previousSpeed = Time.timeScale;
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = _previousSpeed;
                }

                OnGameSpeedChanged?.Invoke(Time.timeScale);
            }
        }
    }
}
