using System;

namespace DioLive.Triangle.CoreClient
{
    public class ManualTimer
    {
        public event EventHandler Tick;

        public ManualTimer(TimeSpan updateInterval)
        {
            this.UpdateInterval = updateInterval;
        }

        public TimeSpan UpdateInterval { get; set; }

        public TimeSpan TimeElapsed { get; set; }

        public void AddTimeElapsed(TimeSpan timeElapsed)
        {
            this.TimeElapsed += timeElapsed;

            while (this.TimeElapsed >= this.UpdateInterval)
            {
                this.TimeElapsed -= this.UpdateInterval;
                this.OnTick();
            }
        }

        public static ManualTimer operator +(ManualTimer gameTimer, TimeSpan timeElapsed)
        {
            gameTimer.AddTimeElapsed(timeElapsed);
            return gameTimer;
        }

        protected virtual void OnTick()
        {
            this.Tick?.Invoke(this, EventArgs.Empty);
        }
    }
}