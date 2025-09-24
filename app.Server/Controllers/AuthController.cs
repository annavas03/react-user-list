using app.Server.Data.Entities.Identity;
using app.Server.Identity;
using app.Server.Models.Account;
using app.Server.Service;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using app.Server.Interfaces;

namespace app.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<UserEntity> userManager,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration) : ControllerBase
    {
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Token))
            {
                return BadRequest("Token is required.");
            }

            // Validate the Google token here (e.g., using Google API)
            // For demonstration purposes, we'll assume the token is valid and retrieve user info.
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.Token);
            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo");
            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized("Invalid Google token.");
            }
            var userJson = await response.Content.ReadAsStringAsync();

            var googleUser = JsonSerializer.Deserialize<GoogleAccountModel>(userJson);

            var existingUser = await userManager.FindByEmailAsync(googleUser!.Email);

            //уже уже зареєструвався, але хоче зайти через Google
            if (existingUser != null)
            {
                //Шукаємо чи він входив через Google
                var userLoginGoogle = await userManager.FindByLoginAsync("Google", googleUser.GoogleId);

                // Якщо не знайшли, то додаємо логін Google до існуючого користувача
                if (userLoginGoogle == null)
                {
                    await userManager.AddLoginAsync(existingUser, new UserLoginInfo("Google", googleUser.GoogleId, "Google"));
                }

                existingUser.LoginType = "Google"; 
                await userManager.UpdateAsync(existingUser);

                var token = await jwtTokenService.GenerateTokenAsync(existingUser);
                return Ok(
                    new
                    {
                        token
                    });
            }
            else
            {
                // Якщо користувач не знайдений, створюємо нового
                var newUser = new UserEntity
                {
                    UserName = googleUser.Email,
                    Email = googleUser.Email,
                    FirstName = googleUser.FirstName,
                    LastName = googleUser.LastName,
                    LoginType = "Google"
                };

                var result = await userManager.CreateAsync(newUser);

                result = await userManager.AddLoginAsync(newUser, new UserLoginInfo("Google", googleUser.GoogleId, "Google"));

                await userManager.AddToRoleAsync(newUser, "User");
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.Select(e => e.Description));
                }
                var token = await jwtTokenService.GenerateTokenAsync(newUser);
                return Ok(
                    new
                    {
                        token
                    });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            var result = await userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return Unauthorized("Invalid email or password.");
            }
            //
            var token = await jwtTokenService.GenerateTokenAsync(user);

            return Ok(
                new
                {
                    token
                });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email and password are required.");
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return BadRequest("User with this email already exists.");
            }
            user = new UserEntity
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow
            };

            if (model.ImageFile != null)
            {
                var imagesDir = configuration["ImagesDir"] ?? "images";
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), imagesDir, imageName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                user.Image = imageName;
            }
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
            // Optionally, assign a default role to the user
            await userManager.AddToRoleAsync(user, Constants.Roles.User);
            var token = await jwtTokenService.GenerateTokenAsync(user);
            return Ok(new
            {
                token
            });
        }
    }
}