using System;

namespace DioLive.Triangle.CoreClient
{
    public class GameTimer
    {
        public GameTimer(TimeSpan updateInterval)
        {
            this.UpdateInterval = updateInterval;
        }

        public TimeSpan UpdateInterval { get; set; }

        public TimeSpan TimeElapsed { get; set; }

        public void AddTimeElapsed(TimeSpan timeElapsed)
        {
            this.TimeElapsed += timeElapsed;
        }

        public bool CheckElapsed()
        {
            if (this.TimeElapsed >= this.UpdateInterval)
            {
                this.TimeElapsed -= this.UpdateInterval;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static GameTimer operator +(GameTimer gameTimer, TimeSpan timeElapsed)
        {
            gameTimer.AddTimeElapsed(timeElapsed);
            return gameTimer;
        }
    }
}