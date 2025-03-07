using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClienteServ.Servidor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build();

            host.Run();
        }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
    }

    public class RSAEncryption
    {
        private readonly RSA _rsa;

        public RSAEncryption()
        {
            _rsa = RSA.Create();
        }

        public void GenerateKeys(out string publicKey, out string privateKey)
        {
            var publicKeyBytes = _rsa.ExportRSAPublicKey();
            var privateKeyBytes = _rsa.ExportRSAPrivateKey();

            publicKey = ExportPublicKeyToPEM(publicKeyBytes);
            privateKey = ExportPrivateKeyToPEM(privateKeyBytes);
        }

        private string ExportPublicKeyToPEM(byte[] publicKeyBytes)
        {
            return "-----BEGIN PUBLIC KEY-----\n" +
                   Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                   "\n-----END PUBLIC KEY-----";
        }

        private string ExportPrivateKeyToPEM(byte[] privateKeyBytes)
        {
            return "-----BEGIN PRIVATE KEY-----\n" +
                   Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                   "\n-----END PRIVATE KEY-----";
        }

        public string Encrypt(string plainText, string publicKey)
        {
            byte[] publicKeyBytes = ConvertFromPEM(publicKey);
            _rsa.ImportRSAPublicKey(publicKeyBytes, out _);
            byte[] encrypted = _rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText, string privateKey)
        {
            try
            {
                byte[] privateKeyBytes = ConvertFromPEM(privateKey);
                _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                byte[] decrypted = _rsa.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descifrar: {ex.Message}");
                return string.Empty;
            }
        }

        private byte[] ConvertFromPEM(string pem)
        {
            if (string.IsNullOrWhiteSpace(pem))
            {
                throw new ArgumentException("La clave PEM no puede estar vacía o ser nula.");
            }

            string cleanPem = pem.Replace("-----BEGIN PUBLIC KEY-----", "")
                                 .Replace("-----END PUBLIC KEY-----", "")
                                 .Replace("-----BEGIN PRIVATE KEY-----", "")
                                 .Replace("-----END PRIVATE KEY-----", "")
                                 .Replace("\n", "")
                                 .Replace("\r", "")
                                 .Trim();

            try
            {
                return Convert.FromBase64String(cleanPem);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Error al convertir la clave PEM desde Base64. Verifica el formato de la clave.", ex);
            }
        }
    }

    public class Startup
    {
        private readonly List<Pokemon> _pokemonList = new List<Pokemon>();
        private static RSAEncryption rsaEncryption = new RSAEncryption();

        public Startup()
        {
            // Inicialización de claves RSA
            rsaEncryption.GenerateKeys(out string publicKey, out string privateKey);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            services.AddControllers();
            services.AddHttpClient();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                // Ruta GET para obtener todos los Pokémon
                endpoints.MapGet("/api/pokemon", async context =>
                {
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(_pokemonList);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonResponse);
                });

                // Ruta GET para obtener un Pokémon por ID
                endpoints.MapGet("/api/pokemon/{id}", async context =>
                {
                    var id = int.Parse(context.Request.RouteValues["id"].ToString());
                    var pokemon = _pokemonList.FirstOrDefault(p => p.Id == id);
                    if (pokemon != null)
                    {
                        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(pokemon);
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(jsonResponse);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync($"Pokémon con ID {id} no encontrado.");
                    }
                });

                // Ruta POST para agregar un nuevo Pokémon
                endpoints.MapPost("/api/pokemon", async context =>
                {
                    var pokemonData = await System.Text.Json.JsonSerializer.DeserializeAsync<Pokemon>(context.Request.Body);

                    if (pokemonData != null)
                    {
                        pokemonData.Id = _pokemonList.Count + 1;  // Asignación de ID automático
                        _pokemonList.Add(pokemonData);

                        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(pokemonData);
                        context.Response.StatusCode = 201;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(jsonResponse);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("❌ Datos del Pokémon no válidos.");
                    }
                });

                // Ruta PUT para actualizar un Pokémon por ID
                endpoints.MapPut("/api/pokemon/{id}", async context =>
                {
                    var id = int.Parse(context.Request.RouteValues["id"].ToString());
                    var pokemonData = await System.Text.Json.JsonSerializer.DeserializeAsync<Pokemon>(context.Request.Body);

                    var existingPokemon = _pokemonList.FirstOrDefault(p => p.Id == id);

                    if (existingPokemon != null && pokemonData != null)
                    {
                        existingPokemon.Name = pokemonData.Name;
                        existingPokemon.Type = pokemonData.Type;
                        existingPokemon.Hp = pokemonData.Hp;
                        existingPokemon.Attack = pokemonData.Attack;

                        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(existingPokemon);
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(jsonResponse);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("❌ Pokémon no encontrado o datos inválidos.");
                    }
                });

                // Ruta DELETE para eliminar un Pokémon por ID
                endpoints.MapDelete("/api/pokemon/{id}", async context =>
                {
                    var id = int.Parse(context.Request.RouteValues["id"].ToString());
                    var pokemonToDelete = _pokemonList.FirstOrDefault(p => p.Id == id);

                    if (pokemonToDelete != null)
                    {
                        _pokemonList.Remove(pokemonToDelete);
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync($"✅ Pokémon con ID {id} eliminado.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("❌ Pokémon no encontrado.");
                    }
                });
            });
        }
    }
}
