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

        private List<SongInfo> _originalPlaylist = new List<SongInfo>();
        private List<SongInfo> _shuffledPlaylist = new List<SongInfo>();
        private int _currentIndex = -1;
        private Random _random = new Random();

        public SongInfo CurrentSong => _currentIndex >= 0 && _currentIndex < CurrentPlaylist.Count
            ? CurrentPlaylist[_currentIndex]
            : null;

        private List<SongInfo> CurrentPlaylist => IsShuffling ? _shuffledPlaylist : _originalPlaylist;

        public bool IsRepeating { get; set; } = false;
        public event Action<bool> ShuffleStateChanged;
        public event Action SongInfoChanged;

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
                        if (_originalPlaylist.Count == 0 && _shuffledPlaylist.Count > 0)
                        {
                            _originalPlaylist = new List<SongInfo>(_shuffledPlaylist);
                        }

                        _shuffledPlaylist = new List<SongInfo>(_originalPlaylist);
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
        public static event Action<bool> PlaybackStateChanged;
        public TimeSpan CurrentDuration => _mediaPlayer.NaturalDuration.HasTimeSpan ? _mediaPlayer.NaturalDuration.TimeSpan : TimeSpan.Zero;

        private MusicPlayerService()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        private void ShuffleList(List<SongInfo> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                var value = list[k];
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

        public void SetPlaylist(List<SongInfo> playlist)
        {
            _originalPlaylist = new List<SongInfo>(playlist);
            _shuffledPlaylist = new List<SongInfo>(playlist);
            _currentIndex = -1;

            if (IsShuffling)
            {
                ShuffleList(_shuffledPlaylist);
            }
        }

        public void PlayAt(int index)
        {
            if (CurrentPlaylist.Count == 0) return;

            if (index < 0 || index >= CurrentPlaylist.Count) return;

            _currentIndex = index;
            PlayCurrent();
        }

        private void PlayCurrent()
        {
            if (_currentIndex < 0 || _currentIndex >= CurrentPlaylist.Count) return;

            var song = CurrentPlaylist[_currentIndex];
            _mediaPlayer.Open(new Uri(song.AudioUrl));
            _mediaPlayer.Play();
            _timer.Start();
            PlaybackStateChanged?.Invoke(true);
            SongInfoChanged?.Invoke();
        }

        public void Play(SongInfo song)
        {
            _originalPlaylist = new List<SongInfo> { song };
            _shuffledPlaylist = new List<SongInfo> { song };
            _currentIndex = 0;

            SongInfoChanged?.Invoke();
            PlayCurrent();
        }

        public void PlayNext()
        {
            if (CurrentPlaylist.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= CurrentPlaylist.Count)
                _currentIndex = 0;

            SongInfoChanged?.Invoke();
            PlayCurrent();
        }

        public void PlayPrevious()
        {
            if (CurrentPlaylist.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = CurrentPlaylist.Count - 1;

            SongInfoChanged?.Invoke();
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
    public class SongInfo
    {
        public string AudioUrl { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AlbumCoverUrl { get; set; }
    }
}