namespace MqttDashboard.Serevices
{
    public class CounterService
    {
        public event Action<int>? OnCounterChanged;
        private int _latestValue = 0;

        public void RaiseCounter(int value)
        {
            _latestValue = value;
            OnCounterChanged?.Invoke(value);
        }

        public void Subscribe(Action<int> handler)
        {
            OnCounterChanged += handler;
            handler(_latestValue); // Push current value immediately
        }

        public void Unsubscribe(Action<int> handler)
        {
            OnCounterChanged -= handler;
        }
    }


}
