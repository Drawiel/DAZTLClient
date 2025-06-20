﻿using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Daztl;
using DAZTLClient.Models;
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

    }
}
