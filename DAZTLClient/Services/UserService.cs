using Daztl;
using DAZTLClient.Models;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows; // Usamos las clases generadas por gRPC

namespace DAZTLClient.Services
{
    public class UserService
    {
        private readonly MusicService.MusicServiceClient _client;

        public UserService()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:50051");
            _client = new MusicService.MusicServiceClient(channel);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                var grpcRequest = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var response = await _client.LoginUserAsync(grpcRequest);

                SessionManager.Instance.StartSession(response.AccessToken, response.RefreshToken, response.Role);

                if (response.Role == "artist")
                {
                    return "Inicio de sesion de artista";
                }
                else if (response.Role == "listener")
                {
                    return "Inicio de sesion de oyente";
                }
                else if(response.Role == "admin")
                {
                    return "Inicio de sesion de admin";
                }
                else
                {
                    return "No se reconocio el rol";
                }
            }
            catch (Grpc.Core.RpcException ex)
            {
                return $"Error en el login: {ex.Status.Detail}";
            }
        }


        public async Task<string> RegisterAsync(Models.RegisterRequest request)
        {
            try
            {
                var grpcRequest = new Daztl.RegisterRequest
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };
                var response = await _client.RegisterUserAsync(grpcRequest);


                return "Usuario registrado correctamente.";
            }
            catch (Grpc.Core.RpcException ex)
            {
                return $"Error en el registro: {ex.Status.Detail}";
            }
        }

        public async Task<string> RegisterArtistAsync(Models.RegisterArtistRequest request)
        {
            try
            {
                var grpcRequest = new Daztl.RegisterArtistRequest
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Bio = request.Bio
                };

                var response = await _client.RegisterArtistAsync(grpcRequest);

                return "Artista registrado correctamente.";
            }
            catch (Grpc.Core.RpcException ex)
            {
                return $"Error en el registro de artista: {ex.Status.Detail}";
            }
        }

        public async Task<Daztl.ArtistProfileResponse> GetArtistProfileAsync(string token)
        {
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token}");

                var grpcRequest = new Daztl.Empty();

                var response = await _client.GetArtistProfileAsync(grpcRequest, headers);

                return new Daztl.ArtistProfileResponse
                {
                    Username = response.Username,
                    Email = response.Email,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    ProfileImageUrl = response.ProfileImageUrl,
                    Bio = response.Bio,
                    ArtistProfileId = (int)response.ArtistProfileId
                };
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw new Exception($"Error al obtener perfil de artista: {ex.Status.Detail}");
            }
        }

        public async Task<Daztl.UserProfileResponse> GetListenerProfileAsync(string token)
        {
            try
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token}");

                var grpcRequest = new Daztl.Empty();

                var response = await _client.GetProfileAsync(grpcRequest, headers);

                return new Daztl.UserProfileResponse
                {
                    Username = response.Username,
                    Email = response.Email,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    ProfileImageUrl = response.ProfileImageUrl,
                };
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw new Exception($"Error al obtener perfil de oyente: {ex.Status.Detail}");
            }
        }

        public async Task<string> UpdateArtistProfileAsync(string token, string username, string password, string bio)
        {
            try
            {
                var grpcRequest = new Daztl.UpdateArtistProfileRequest
                {
                    Token = token,
                };

                if (!string.IsNullOrWhiteSpace(username))
                    grpcRequest.Username = username;
                if (!string.IsNullOrWhiteSpace(password))
                    grpcRequest.Password = password;
                if (!string.IsNullOrWhiteSpace(bio))
                    grpcRequest.Bio = bio;

                var response = await _client.UpdateArtistProfileAsync(grpcRequest);

                if (response.Status == "success")
                {
                    return "Perfil de artista actualizado correctamente.";
                }
                else
                {
                    return $"Error: {response.Message}";
                }
            }
            catch (RpcException ex) 
            { 
                return $"Error al actualizar perfil de artista: {ex.Status.Detail}";
            }
        }

        public async Task<string> UploadProfileImageAsync(string token, string imagePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string filename = Path.GetFileName(imagePath);

                var grpcRequest = new Daztl.UploadProfileImageRequest
                {
                    Token = token,
                    ImageData = Google.Protobuf.ByteString.CopyFrom(imageBytes),
                    Filename = filename
                };

                var response = await _client.UploadProfileImageAsync(grpcRequest);
                return response.Status == "success" ?
                    response.Message :
                    $"Error: {response.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public async Task<string> UpdateProfileAsync(string token, string email, string firstName, string lastName, string username, string password)
        {
            try
            {
                var grpcRequest = new Daztl.UpdateProfileRequest
                {
                    Token = token
                };

                if (!string.IsNullOrWhiteSpace(email))
                    grpcRequest.Email = email;
                if (!string.IsNullOrWhiteSpace(firstName))
                    grpcRequest.FirstName = firstName;
                if (!string.IsNullOrWhiteSpace(lastName))
                    grpcRequest.LastName = lastName;
                if (!string.IsNullOrWhiteSpace(username))
                    grpcRequest.Username = username;
                if (!string.IsNullOrWhiteSpace(password)) 
                    grpcRequest.Password = password;

                var response = await _client.UpdateProfileAsync(grpcRequest);

                if (response.Status == "success")
                {
                    return "Perfil actualizado correctamente.";
                }
                else
                {
                    return $"Error: {response.Message}";
                }
            }
            catch (Grpc.Core.RpcException ex)
            {
                return $"Error al actualizar perfil: {ex.Status.Detail}";
            }
        }

    }


}

    
