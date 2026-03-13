using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using LegislationTimeMachine.Models;

namespace LegislationTimeMachine.Services
{
    public class LegislationStateService
    {
        private readonly HttpClient _http;
        private int _leftYear = 2003;

        public List<LegislativeNode> MasterNodes { get; private set; } = new();
        public List<LegislationFileInfo> AvailableActs { get; private set; } = new();
        public string CurrentActTitle { get; private set; } = "Select Legislation";
        public bool IsLoading { get; private set; } = false;

        // Date state for the Time Machine
        public DateTime LeftDate { get; set; } = new DateTime(2003, 1, 1);
        public DateTime RightDate { get; set; } = DateTime.Now;

        public int LeftYear
        {
            get => _leftYear;
            set
            {
                if (_leftYear != value)
                {
                    _leftYear = value;
                    LeftDate = new DateTime(value, 1, 1);
                    NotifyStateChanged();
                }
            }
        }

        public event Action? OnChange;

        public LegislationStateService(HttpClient http)
        {
            _http = http;
        }

        public async Task LoadAvailableActsAsync()
        {
            try
            {
                // Fetches the manifest list from wwwroot/data/manifest.json
                var fileNames = await _http.GetFromJsonAsync<List<string>>("data/manifest.json");
                AvailableActs.Clear();

                if (fileNames != null)
                {
                    foreach (var name in fileNames)
                    {
                        AvailableActs.Add(new LegislationFileInfo 
                        { 
                            FileName = name, 
                            ShortTitle = name.Replace(".xml", "").Replace("Act", " Act") 
                        });
                    }
                }
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading manifest: {ex.Message}");
            }
        }

        public async Task LoadNewActAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            IsLoading = true;
            NotifyStateChanged();

            try
            {
                var xmlString = await _http.GetStringAsync($"data/{fileName}");
                var doc = XDocument.Parse(xmlString);

                // Extract ShortTitle using the 2002 schema pattern
                CurrentActTitle = doc.Descendants("ShortTitle").FirstOrDefault()?.Value 
                                  ?? fileName.Replace(".xml", "");

                var parser = new LegislationParser();
                MasterNodes = parser.ParseToTree(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error switching acts: {ex.Message}");
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
