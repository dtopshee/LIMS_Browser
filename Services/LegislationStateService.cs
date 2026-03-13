using System.Xml.Linq;
using LegislationTimeMachine.Models;

namespace LegislationTimeMachine.Services
{
    public class LegislationStateService
    {
        private readonly HttpClient _http;
        public List<LegislativeNode> MasterNodes { get; private set; } = new();
        public bool IsLoading { get; private set; } = false;

        public DateTime LeftDate { get; set; } = new DateTime(2003, 1, 1);
        public DateTime RightDate { get; set; } = DateTime.Now;

        private int _leftYear = 2003;
        public int LeftYear 
        { 
            get => _leftYear; 
            set {
                if (_leftYear != value) {
                    _leftYear = value;
                    NotifyStateChanged(); // This "pings" the Timeline.razor page
                }
            }
        }

        public event Action? OnChange;

        public LegislationStateService(HttpClient http)
        {
            _http = http;
        }

        public async Task InitializeAsync()
        {
            if (MasterNodes.Any() || IsLoading) return;

            IsLoading = true;
            NotifyStateChanged();

            try
            {
                // Ensure this matches your filename in wwwroot/data/
                //var xmlString = await _http.GetStringAsync("data/criminal-code-recent.xml");
                var xmlString = await _http.GetStringAsync("data/P-21.xml");
                var doc = XDocument.Parse(xmlString);

                var parser = new LegislationParser();
                MasterNodes = parser.ParseToTree(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error loading XML: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
