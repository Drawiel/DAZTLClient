using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Daztl;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

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

    }
}
