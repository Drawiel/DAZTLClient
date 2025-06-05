using DAZTLClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAZTLClient.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        public UserService() { 
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8000");
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/register/", content);

            if(response.IsSuccessStatusCode) {
                return "Usuario registrado correctamente";
            }

            string error = await response.Content.ReadAsStringAsync();
            return $"Error en el registro {error}";
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/login/", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                App.Current.Properties["auth_token"] = loginResponse.Token;

                return "Inicio de sesion exitoso.";
            }

            var error = await response.Content.ReadAsStringAsync();
            return $"Error en el login: {error}";
        }
    }
}
