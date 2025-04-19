using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AweDev.Utilities
{
    public class PauseGame : MonoBehaviour
    {
        [SerializeField] private bool _pauseOnStart;
        [SerializeField] private bool _isPaused;

        private void Init()
        {
            if (_pauseOnStart)
            {
                _isPaused = true;
                Time.timeScale = 0;
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
            }
        }
    }
}
