namespace LegislationTimeMachine.Services
{
    public class LegislationStateService
    {
        public DateTime LeftDate { get; set; } = new DateTime(2003, 1, 1);
        public DateTime RightDate { get; set; } = DateTime.Now;

        // Add this property for the UI slider to work
        public int LeftYear 
        { 
            get => LeftDate.Year; 
            set => LeftDate = new DateTime(value, 1, 1); 
        }

        public event Action? OnChange;

        public void UpdateTimeline(DateTime left, DateTime right)
        {
            LeftDate = left;
            RightDate = right;
            OnChange?.Invoke();
        }
    }
}
