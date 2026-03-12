public class LegislationStateService
{
    public DateTime LeftDate { get; set; } = new DateTime(2003, 1, 1);
    public DateTime RightDate { get; set; } = DateTime.Now;

    public event Action OnChange;

    public void UpdateTimeline(DateTime left, DateTime right)
    {
        LeftDate = left;
        RightDate = right;
        OnChange?.Invoke();
    }
}
