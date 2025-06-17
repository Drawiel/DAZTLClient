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
            var channel = GrpcChannel.ForAddress("http://localhost:50051");
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
            try
            {
                if (!File.Exists(audioFilePath))
                    throw new FileNotFoundException("Archivo de audio no encontrado");

                byte[] audioBytes = File.ReadAllBytes(audioFilePath);
                if (audioBytes.Length == 0)
                    throw new Exception("El archivo de audio está vacío");

                ByteString coverImageBytes = ByteString.Empty;
                if (!string.IsNullOrEmpty(coverImagePath) && File.Exists(coverImagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(coverImagePath);
                    coverImageBytes = ByteString.CopyFrom(imageBytes);
                }

                var request = new UploadSongRequest
                {
                    Token = token, 
                    Title = title,
                    AudioFile = ByteString.CopyFrom(audioBytes),
                    CoverImage = coverImageBytes
                };

                var headers = new Metadata
        {
            { "authorization", $"Bearer {token}" }
        };

                Console.WriteLine($"Intentando subir canción: {title} ({audioBytes.Length} bytes)");
                var response = await _client.UploadSongAsync(request, headers);
                Console.WriteLine("Respuesta recibida del servidor");

                return response;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"ERROR gRPC: {ex.Status.Detail} | Code: {ex.StatusCode}");
                throw new Exception($"Error del servidor: {ex.Status.Detail ?? "Sin detalles"} (Código: {ex.StatusCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR inesperado: {ex.ToString()}");
                throw;
            }
        }

    }
}
