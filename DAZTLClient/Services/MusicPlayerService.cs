using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;

namespace DAZTLClient.Services
{
    public class MusicPlayerService
    {
        private static MusicPlayerService _instance;
        private MediaPlayer _mediaPlayer;
        private DispatcherTimer _timer;

        private List<string> _playlist = new List<string>();
        private int _currentIndex = -1;

        public bool IsRepeating { get; set; } = false;
        public bool IsShuffling { get; set; } = false;

        public event Action<double> PlaybackPositionChanged;
        public event Action<bool> PlaybackStateChanged;
        public TimeSpan CurrentDuration => _mediaPlayer.NaturalDuration.HasTimeSpan ? _mediaPlayer.NaturalDuration.TimeSpan : TimeSpan.Zero;

        private MusicPlayerService()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        public bool IsPlaying()
        {
            return _mediaPlayer != null &&
                   _mediaPlayer.Source != null &&
                   _mediaPlayer.Position > TimeSpan.Zero &&
                   _mediaPlayer.Position < _mediaPlayer.NaturalDuration.TimeSpan;
        }

        public static MusicPlayerService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MusicPlayerService();
                return _instance;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan && _mediaPlayer.Position != null)
            {
                var progress = _mediaPlayer.Position.TotalMilliseconds / _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds * 100;
                PlaybackPositionChanged?.Invoke(progress);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (IsRepeating)
            {
                PlayCurrent();
            }
            else
            {
                PlayNext();
            }
        }

        public void SetPlaylist(List<string> playlist)
        {
            _playlist = playlist;
            _currentIndex = -1;
        }

        public void PlayAt(int index)
        {
            if (_playlist.Count == 0) return;
            if (index < 0 || index >= _playlist.Count) return;

            _currentIndex = index;
            PlayCurrent();
        }

        private void PlayCurrent()
        {
            var url = _playlist[_currentIndex];
            _mediaPlayer.Open(new Uri(url));
            _mediaPlayer.Play();
            _timer.Start();
            PlaybackStateChanged?.Invoke(true);
        }

        public void Play(string url)
        {
            _playlist = new List<string> { url };
            _currentIndex = 0;
            PlayCurrent();
        }

        public void PlayNext()
        {
            if (_playlist.Count == 0) return;

            if (IsShuffling)
            {
                var rnd = new Random();
                _currentIndex = rnd.Next(_playlist.Count);
            }
            else
            {
                _currentIndex++;
                if (_currentIndex >= _playlist.Count)
                    _currentIndex = 0;
            }
            PlayCurrent();
        }

        public void PlayPrevious()
        {
            if (_playlist.Count == 0) return;

            if (IsShuffling)
            {
                var rnd = new Random();
                _currentIndex = rnd.Next(_playlist.Count);
            }
            else
            {
                _currentIndex--;
                if (_currentIndex < 0)
                    _currentIndex = _playlist.Count - 1;
            }
            PlayCurrent();
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            _timer.Stop();
            PlaybackStateChanged?.Invoke(false);
        }

        public void Resume()
        {
            _mediaPlayer.Play();
            _timer.Start();
            PlaybackStateChanged?.Invoke(true);
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
            _timer.Stop();
            PlaybackStateChanged?.Invoke(false);
        }
        public void Seek(TimeSpan position)
        {
            _mediaPlayer.Position = position;
        }
    }
}
