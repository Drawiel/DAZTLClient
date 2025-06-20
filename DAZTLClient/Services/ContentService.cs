using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Daztl;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace DAZTLClient.Services
{
    public class ContentService
    {
        private readonly MusicService.MusicServiceClient _client;

        public ContentService()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:50051", new GrpcChannelOptions
            {
                MaxReceiveMessageSize = 50 * 1024 * 1024, 
                MaxSendMessageSize = 50 * 1024 * 1024,   
                Credentials = ChannelCredentials.Insecure
            });
            _client = new MusicService.MusicServiceClient(channel);
        }

        public async Task<PlaylistListResponse> ListPlaylistsAsync()
        {
            try
            {
                var request = new PlaylistListRequest
                {
                    Token = SessionManager.Instance.AccessToken
                };

                var reply = await _client.ListPlaylistsAsync(request);
                return reply;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error al obtener playlists: {ex.Status.Detail}");
            }
        }

        public async Task<AlbumDetailResponse> GetAlbumDetailAsync(int albumId)
        {
            try
            {
                var request = new AlbumDetailRequest { AlbumId = albumId };
                var headers = new Metadata
        {
            { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
        };
                return await _client.GetAlbumDetailAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error al obtener detalles del álbum: {ex.Status.Detail}");
            }
        }
        public async Task<GlobalSearchResponse> GlobalSearchAsync(string query)
        {
            try
            {
                var request = new SearchRequest { Query = query };

                var headers = new Metadata
                {
                    { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
                };

                return await _client.GlobalSearchAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error en búsqueda global: {ex.Status.Detail}");
            }
        }

        public async Task<SongListResponse> ListSongsAsync()
        {
            var request = new Daztl.Empty();
            return await _client.ListSongsAsync(request);
        }

        public async Task<AlbumListResponse> ListAlbumsAsync()
        {
            var request = new Daztl.Empty();
            return await _client.ListAlbumsAsync(request);
        }

        public async Task<ArtistListResponse> ListArtistsAsync()
        {
            var request = new Daztl.Empty();
            return await _client.ListArtistsAsync(request);
        }
        public async Task<GenericResponse> CreatePlaylistAsync(string name, string imagePath)
        {
            string base64Image = null;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                base64Image = Convert.ToBase64String(imageBytes);
            }

            var request = new CreatePlaylistRequest
            {
                Name = name,
                CoverUrl = base64Image ?? "",
                Token = SessionManager.Instance.AccessToken
            };

            var response = await _client.CreatePlaylistAsync(request);

            return new GenericResponse
            {
                Status = response.Status,
                Message = response.Message
            };
        }

        public async Task<SongListResponse> SearchSongsAsync(string query)
        {
            try
            {
                var request = new SearchRequest { Query = query };

                var headers = new Metadata
        {
            { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
        };

                return await _client.SearchSongsAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error al buscar canciones: {ex.Status.Detail}");
            }
        }

        public async Task<GenericResponse> AddSongToPlaylistAsync(string playlistId, string songId, string token)
        {
            try
            {
                var request = new AddSongToPlaylistRequest
                {
                    PlaylistId = int.Parse(playlistId),
                    SongId = int.Parse(songId),
                    Token = token
                };

                return await _client.AddSongToPlaylistAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error al agregar canción a la playlist: {ex.Status.Detail}");
            }
        }

        public async Task<PlaylistResponse> GetPlaylistAsync(string playlistId)
        {
            try
            {
                var request = new PlaylistIdRequest { Id = int.Parse(playlistId) };

                var headers = new Metadata
        {
            { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
        };

                return await _client.GetPlaylistAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error al obtener playlist: {ex.Status.Detail}");
            }
        }

        public async Task<bool> IsArtistLikedAsync(int artistId)
        {
            try
            {
                var request = new ArtistIdRequest
                {
                    ArtistId = artistId,
                    Token = SessionManager.Instance.AccessToken
                };

                var response = await _client.IsArtistLikedAsync(request);
                return response.IsLiked;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error checking like status: {ex.Status.Detail}");
            }
        }

        public async Task ToggleArtistLikeAsync(int artistId)
        {
            try
            {
                var request = new ArtistIdRequest
                {
                    ArtistId = artistId,
                    Token = SessionManager.Instance.AccessToken
                };

                await _client.LikeArtistAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error toggling artist like: {ex.Status.Detail}");
            }
        }

        public async Task<AdminReportResponse> GetAdminReportAsync(string token, string reportType)
        {
            try
            {
                var grpcRequest = new Daztl.AdminReportRequest
                {
                    Token = token,
                    ReportType = reportType
                };

                var response = await _client.GetAdminReportAsync(grpcRequest);

                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = new AdminReportResponse();
                errorResponse.Status = "error";
                errorResponse.Message = $"Error: {ex.Message}";
                return errorResponse;
            }
        }

        public async Task<ArtistReportResponse> GetArtistReportAsync(string token, string reportType)
        {
            try
            {
                var grpcRequest = new ArtistReportRequest
                {
                    Token = token,
                    ReportType = reportType
                };

                var response = await _client.GetArtistReportAsync(grpcRequest);
                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = new ArtistReportResponse();
                errorResponse.Status = "error";
                errorResponse.Message = $"Error: {ex.Message}";
                return errorResponse;
            }
        }

        public async Task<GenericResponse> UploadSongAsync(string token, string title, string audioFilePath, string coverImagePath)
        {
            GenericResponse response = new(){ Message = "Success", Status = "200"};
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("El título de la canción es requerido");

                if (!File.Exists(audioFilePath))
                    throw new FileNotFoundException("Archivo de audio no encontrado");

                byte[] audioBytes = File.ReadAllBytes(audioFilePath);
                if (audioBytes.Length == 0)
                    throw new Exception("El archivo de audio está vacío");

                if (audioBytes.Length > 50 * 1024 * 1024)
                    throw new Exception("El archivo de audio es demasiado grande (máximo 50MB)");

                ByteString coverImageBytes = ByteString.Empty;
                if (!string.IsNullOrEmpty(coverImagePath) && File.Exists(coverImagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(coverImagePath);
                    if (imageBytes.Length > 5 * 1024 * 1024) 
                        throw new Exception("La imagen es demasiado grande (máximo 5MB)");
                    coverImageBytes = ByteString.CopyFrom(imageBytes);
                }

                var request = new UploadSongRequest
                {
                    Token = token,
                    Title = title,
                    AudioFile = ByteString.CopyFrom(audioBytes),
                    CoverImage = coverImageBytes,
                    ReleaseDate = DateTime.Now.ToString("yyyy-MM-dd")
                };

                var headers = new Metadata
                {
                    { "authorization", $"Bearer {token}" }
                };

                Console.WriteLine($"Subiendo canción: {title} (Audio: {audioBytes.Length} bytes, Imagen: {coverImageBytes.Length} bytes)");

                response = await _client.UploadSongAsync(request, headers, deadline: DateTime.UtcNow.AddMinutes(5));
                Thread.Sleep(1000);
                if (response.Status == "success")
                {
                    var requestNotification = new Notification
                    {
                        Message = $"Nueva cancion '{title}'. Únete al chat para ver que piensan de ella.",
                    };

                    var responseNotification = await _client.CreateNotificationAsync(requestNotification, headers, deadline: DateTime.UtcNow.AddMinutes(5));

                }

                Console.WriteLine($"Respuesta del servidor: {response.Status} - {response.Message}");
                return response;
            }
            catch (RpcException ex)
            {

                var headers = new Metadata
                {
                    { "authorization", $"Bearer {token}" }
                };
                var requestNotification = new Notification
                {
                    Message = $"Nueva cancion '{title}'. Únete al chat para ver que piensan de ella.",
                };

                var responseNotification = await _client.CreateNotificationAsync(requestNotification, headers, deadline: DateTime.UtcNow.AddMinutes(5));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"ERROR: Archivo no encontrado - {ex.Message}");
                throw new Exception("Archivo de audio no encontrado. Verifica la ruta.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ERROR: Argumento inválido - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR inesperado: {ex}");
                throw new Exception($"Error inesperado: {ex.Message}");
            }
            return response;
        }


        public async Task<AlbumDetailResponse> GetsAlbumDetailAsync(int albumId)
        {
            try
            {
                var request = new AlbumDetailRequest { AlbumId = albumId };
                var headers = new Metadata
        {
            { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
        };
                return await _client.GetAlbumDetailAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error al obtener detalles del álbum: {ex.Status.Detail}");
            }
        }

        public async Task<GenericResponse> UploadAlbumAsync(
            string token,
            string title,
            string coverPath,
            List<string> songPaths)
        {
            var request = new UploadAlbumRequest
            {
                Token = token,
                Title = title,
            };

            if (File.Exists(coverPath))
            {
                request.CoverImage = ByteString.CopyFrom(File.ReadAllBytes(coverPath));
            }

            foreach (var songPath in songPaths)
            {
                if (File.Exists(songPath))
                {
                    var song = new AlbumSong
                    {
                        AudioFile = ByteString.CopyFrom(File.ReadAllBytes(songPath)),
                        Filename = Path.GetFileName(songPath)
                    };
                    request.Songs.Add(song);
                }
            }

            return await _client.UploadAlbumAsync(request);
        }
        public async Task<Daztl.Empty> SendChatMessageAsync(int songId, string message)
        {
            try
            {
                var request = new MessageRequest
                {
                    Token = SessionManager.Instance.AccessToken,
                    Message = message
                };

                var headers = new Metadata
                {
                    { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
                };

                return await _client.SendMessageChatAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error sending chat message: {ex.Status.Detail}");
            }
        }

        public async Task<Daztl.NotificationListResponse> ListNotificationsAsync()
        {
            try
            {
                var emptyRequest = new Daztl.Empty();
                var reply = await _client.ListNotificationsAsync(emptyRequest);
                return reply;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"ERROR gRPC: {ex.Status.Detail}");
                throw new Exception($"Error al obtener las notificaciones: {ex.Status.Detail}");
            }
        }
        public async Task<Daztl.GenericResponse> DeleteNotificationAsync(int id)
        {
            try
            {
                var request = new MarkAsSeenRequest
                {
                    Token = SessionManager.Instance.AccessToken,
                    NotificationId = id
                };

                var headers = new Metadata
                {
                    { "authorization", $"Bearer {SessionManager.Instance.AccessToken}" }
                };

                return await _client.MarkNotificationAsSeenAsync(request, headers);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Error sending chat message: {ex.Status.Detail}");
            }
        }
    }
}
