using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AweDev.Utilities
{
	public class AdjustGameSpeed : MonoBehaviour
	{
        [SerializeField] private Vector2 _gameSpeedRange = new(0f, 20f);
        [SerializeField] private float _gameSpeedIncrement = 1f;
        public float gameSpeed;

        private void Start()
        {
            PauseGame.OnGameSpeedChanged += (speed) => gameSpeed = speed;

            gameSpeed = Time.timeScale;
        }

        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            //Debug.Log($"Mouse Scroll: {scroll}"); // Debugging line to check scroll value
            if (Mathf.Abs(scroll) > 0.01f) // prevents tiny accidental scroll changes
            {
                AdjustSpeed(scroll);
            }
        }

        private void AdjustSpeed(float mouseScrollValue)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            float increment;
            
            if (mouseScrollValue > 0)
            {
                increment = gameSpeed < 1f ? _gameSpeedIncrement / 10 : _gameSpeedIncrement; // Adjust increment based on current game speed
            }
            else
            {
                increment = gameSpeed <= 1f ? _gameSpeedIncrement / 10 : _gameSpeedIncrement; // Adjust increment based on current game speed
            }

            if (mouseScrollValue > 0)
            {
                gameSpeed += increment;
            }
            else if (mouseScrollValue < 0 && gameSpeed - increment >= 0)
            {
                gameSpeed -= increment;
            }

            Mathf.Clamp(gameSpeed, _gameSpeedRange.x, _gameSpeedRange.y);

            Time.timeScale = gameSpeed;

            //Debug.Log($"Game Speed: {gameSpeed}");
#endif
        }
    }
}
