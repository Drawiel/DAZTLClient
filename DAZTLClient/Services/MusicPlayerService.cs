using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace DAZTLClient.Services
{
    public class MusicPlayerService
    {
        private static MusicPlayerService _instance;
        private MediaPlayer _mediaPlayer;
        private DispatcherTimer _timer;

        private List<string> _originalPlaylist = new List<string>();
        private List<string> _shuffledPlaylist = new List<string>();
        private int _currentIndex = -1;
        private Random _random = new Random();

        public bool IsRepeating { get; set; } = false;
        public event Action<bool> ShuffleStateChanged;

        private bool _isShuffling;

        public bool IsShuffling
        {
            get => _isShuffling;
            set
            {
                if (_isShuffling != value)
                {
                    _isShuffling = value;

                    if (value)
                    {
                        // Al activar shuffle, guardamos la lista original y creamos una mezclada
                        if (_originalPlaylist.Count == 0 && _shuffledPlaylist.Count > 0)
                        {
                            _originalPlaylist = new List<string>(_shuffledPlaylist);
                        }

                        _shuffledPlaylist = new List<string>(_originalPlaylist);
                        ShuffleList(_shuffledPlaylist);

                        // Mantenemos la canción actual en la misma posición si es posible
                        if (_currentIndex >= 0 && _currentIndex < _originalPlaylist.Count)
                        {
                            var currentSong = _originalPlaylist[_currentIndex];
                            _currentIndex = _shuffledPlaylist.IndexOf(currentSong);
                        }
                    }
                    else
                    {
                        // Al desactivar shuffle, volvemos a la lista original
                        if (_currentIndex >= 0 && _currentIndex < _shuffledPlaylist.Count)
                        {
                            var currentSong = _shuffledPlaylist[_currentIndex];
                            _currentIndex = _originalPlaylist.IndexOf(currentSong);
                        }
                    }

                    ShuffleStateChanged?.Invoke(value);
                }
            }
        }

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

        // Método para mezclar una lista
        private void ShuffleList(List<string> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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
            _originalPlaylist = new List<string>(playlist);
            _shuffledPlaylist = new List<string>(playlist);
            _currentIndex = -1;

            if (IsShuffling)
            {
                ShuffleList(_shuffledPlaylist);
            }
        }

        public void PlayAt(int index)
        {
            if (_originalPlaylist.Count == 0) return;

            var currentPlaylist = IsShuffling ? _shuffledPlaylist : _originalPlaylist;

            if (index < 0 || index >= currentPlaylist.Count) return;

            _currentIndex = index;
            PlayCurrent();
        }

        private void PlayCurrent()
        {
            var currentPlaylist = IsShuffling ? _shuffledPlaylist : _originalPlaylist;

            if (_currentIndex < 0 || _currentIndex >= currentPlaylist.Count) return;

            var url = currentPlaylist[_currentIndex];
            _mediaPlayer.Open(new Uri(url));
            _mediaPlayer.Play();
            _timer.Start();
            PlaybackStateChanged?.Invoke(true);
        }

        public void Play(string url)
        {
            _originalPlaylist = new List<string> { url };
            _shuffledPlaylist = new List<string> { url };
            _currentIndex = 0;
            PlayCurrent();
        }

        public void PlayNext()
        {
            var currentPlaylist = IsShuffling ? _shuffledPlaylist : _originalPlaylist;

            if (currentPlaylist.Count == 0) return;

            if (IsShuffling)
            {
                // En modo shuffle, simplemente avanzamos al siguiente índice
                _currentIndex++;
                if (_currentIndex >= currentPlaylist.Count)
                    _currentIndex = 0;
            }
            else
            {
                _currentIndex++;
                if (_currentIndex >= currentPlaylist.Count)
                    _currentIndex = 0;
            }

            PlayCurrent();
        }

        public void PlayPrevious()
        {
            var currentPlaylist = IsShuffling ? _shuffledPlaylist : _originalPlaylist;

            if (currentPlaylist.Count == 0) return;

            if (IsShuffling)
            {
                _currentIndex--;
                if (_currentIndex < 0)
                    _currentIndex = currentPlaylist.Count - 1;
            }
            else
            {
                _currentIndex--;
                if (_currentIndex < 0)
                    _currentIndex = currentPlaylist.Count - 1;
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