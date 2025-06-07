using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Daztl;
using DAZTLClient.Models; // Usamos las clases generadas por gRPC

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


                // Guardar el token
                App.Current.Properties["auth_token"] = response.AccessToken;

                return "Inicio de sesión exitoso.";
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
                    Password = request.Password
                };
                var response = await _client.RegisterUserAsync(grpcRequest);


                return "Usuario registrado correctamente.";
            }
            catch (Grpc.Core.RpcException ex)
            {
                return $"Error en el registro: {ex.Status.Detail}";
            }
        }
    }
}
