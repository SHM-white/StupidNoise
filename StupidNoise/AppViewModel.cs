using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace StupidNoise
{
    public class AppViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _isOn;
        private string _audioPath = string.Empty;
        private int _minTime = 1;
        private int _maxTime = 10;

        private readonly MediaPlayer _mediaPlayer = new();
        private readonly string _settingsPath;
        private Timer? _playbackTimer;
        private readonly Random _random = new();

        public AppViewModel()
        {
            _settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StupidNoise",
                "settings.ini");

            LoadSettings();
        }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value) return;
                _isOn = value;
                OnPropertyChanged();

                if (_isOn)
                {
                    // Start the random playback cycle
                    ScheduleNextPlayback();
                }
                else
                {
                    // Stop the timer and any current playback
                    _playbackTimer?.Dispose();
                    _playbackTimer = null;
                    Application.Current.Dispatcher.Invoke(() => _mediaPlayer.Stop());
                }
            }
        }

        public string AudioPath
        {
            get => _audioPath;
            set
            {
                if (_audioPath == value) return;
                _audioPath = value;
                OnPropertyChanged();
                SaveSettings();

                // If the switch is on, stop current playback to apply the new path immediately on next cycle
                if (IsOn)
                {
                    Application.Current.Dispatcher.Invoke(() => _mediaPlayer.Stop());
                }
            }
        }

        public int MinTime
        {
            get => _minTime;
            set
            {
                if (_minTime == value) return;
                if (value < 1) value = 1;
                if (value > _maxTime) value = _maxTime;
                _minTime = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }

        public int MaxTime
        {
            get => _maxTime;
            set
            {
                if (_maxTime == value) return;
                if (value < _minTime) value = _minTime;
                _maxTime = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }

        private void ScheduleNextPlayback()
        {
            var minMs = MinTime * 1000;
            var maxMs = MaxTime * 1000;
            if (minMs >= maxMs)
            {
                maxMs = minMs + 1;
            }

            // Calculate random interval between MinTime and MaxTime seconds
            var interval = _random.Next(minMs, maxMs);

            // Set up the timer to fire once after the interval
            _playbackTimer = new Timer(PlaySoundAndReschedule, null, interval, Timeout.Infinite);
        }

        private void PlaySoundAndReschedule(object? state)
        {
            // Play the audio on the UI thread
            Application.Current.Dispatcher.Invoke(PlayAudio);

            // If the switch is still on, schedule the next playback
            if (IsOn)
            {
                ScheduleNextPlayback();
            }
        }

        private void PlayAudio()
        {
            if (File.Exists(AudioPath))
            {
                try
                {
                    _mediaPlayer.Open(new Uri(AudioPath));
                    _mediaPlayer.Play();
                }
                catch
                {
                    // Handle invalid audio file
                }
            }
        }

        private void LoadSettings()
        {
            var settings = new Dictionary<string, string>();
            if (File.Exists(_settingsPath))
            {
                settings = File.ReadAllLines(_settingsPath)
                    .Where(line => !string.IsNullOrWhiteSpace(line) && line.Contains('='))
                    .Select(line => line.Split('=', 2))
                    .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim(), StringComparer.OrdinalIgnoreCase);
            }

            if (settings.TryGetValue("AudioPath", out var path) && File.Exists(path))
            {
                _audioPath = path;
            }
            else
            {
                // Fallback to default 1.wav in the application's directory
                var defaultPath = Path.Combine(AppContext.BaseDirectory, "1.wav");
                if (File.Exists(defaultPath))
                {
                    _audioPath = defaultPath;
                }
            }

            if (settings.TryGetValue("MinTime", out var minTimeStr) && int.TryParse(minTimeStr, out var minTime))
            {
                _minTime = minTime;
            }

            if (settings.TryGetValue("MaxTime", out var maxTimeStr) && int.TryParse(maxTimeStr, out var maxTime))
            {
                _maxTime = maxTime;
            }
            
            OnPropertyChanged(nameof(AudioPath));
            OnPropertyChanged(nameof(MinTime));
            OnPropertyChanged(nameof(MaxTime));
        }

        private void SaveSettings()
        {
            var settings = new Dictionary<string, string>
            {
                ["AudioPath"] = AudioPath,
                ["MinTime"] = MinTime.ToString(CultureInfo.InvariantCulture),
                ["MaxTime"] = MaxTime.ToString(CultureInfo.InvariantCulture)
            };

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
                var lines = settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
                File.WriteAllLines(_settingsPath, lines);
            }
            catch
            {
                // Handle settings save error
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose()
        {
            _playbackTimer?.Dispose();
            Application.Current.Dispatcher.Invoke(() => _mediaPlayer.Close());
        }
    }
}
