using System;
using UnityEngine;

namespace EconomyProject.Scripts
{
    public class EndTimerScript : MonoBehaviour
    {
        private float _currentTime;

        private float _startTime;

        public int seconds;

        public int minutes;

        public int hours;

        public string CurrentTime
        {
            get
            {
                var t = TimeSpan.FromSeconds(_startTime - _currentTime);

                string currentTimeLeft = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}:{t.Milliseconds:D3}";
                return currentTimeLeft;
            }
        }


        private void Start()
        {
            int totalTime = seconds + (60 * minutes) + (60 * 60 * hours);
            _currentTime = totalTime;
            _startTime = totalTime;
        }

        private void FixedUpdate()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                Application.Quit();
            }
        }
    }
}
