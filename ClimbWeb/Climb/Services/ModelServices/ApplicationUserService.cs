using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Extensions;
using Climb.Requests.Account;
using Climb.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Climb.Services.ModelServices
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICdnService cdnService;
        private readonly ISignInManager signInManager;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly ITokenHelper tokenHelper;
        private readonly IUrlUtility urlUtility;
        private readonly IUserManager userManager;

        public ApplicationUserService(ApplicationDbContext dbContext, ICdnService cdnService, ISignInManager signInManager, IEmailSender emailSender, IConfiguration configuration, ITokenHelper tokenHelper, IUrlUtility urlUtility, IUserManager userManager)
        {
            this.dbContext = dbContext;
            this.cdnService = cdnService;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.tokenHelper = tokenHelper;
            this.urlUtility = urlUtility;
            this.userManager = userManager;
        }

        public async Task<ApplicationUser> Register(RegisterRequest request, IUrlHelper urlHelper, string requestScheme)
        {
            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                Name = request.Name,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            if(result.Succeeded)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = urlUtility.EmailConfirmationLink(urlHelper, user.Id, code, requestScheme);
                await emailSender.SendEmailConfirmationAsync(request.Email, callbackUrl);

                await signInManager.SignInAsync(user, false);
                return user;
            }

            throw new BadRequestException();
        }

        public async Task<string> LogIn(LoginRequest request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            var result = await signInManager.PasswordSignInAsync(user?.UserName, request.Password, request.RememberMe, false);
            if(result.Succeeded)
            {
                var token = tokenHelper.CreateUserToken(configuration.GetSecurityKey(), DateTime.Now.AddMinutes(30), request.Email);
                return token;
            }

            throw new BadRequestException();
        }

        public async Task<string> UploadProfilePic(string userID, IFormFile image)
        {
            if(image == null)
            {
                throw new BadRequestException(nameof(image), "No image uploaded.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userID);

            if(user == null)
            {
                throw new NotFoundException(typeof(ApplicationUser), userID);
            }

            dbContext.Update(user);
            
            var imageKey = await cdnService.ReplaceImageAsync(user.ProfilePicKey, image, ClimbImageRules.ProfilePic);
            var imageUrl = cdnService.GetImageUrl(imageKey, ClimbImageRules.ProfilePic);
            user.ProfilePicKey = imageKey;
            await dbContext.SaveChangesAsync();
            return imageUrl;
        }

        public async Task UpdateSettings(string userID, string username, string name, IFormFile profilePic)
        {
            var user = await dbContext.Users
                .Include(u => u.LeagueUsers)
                .FirstOrDefaultAsync(u => u.Id == userID);
            if(user == null)
            {
                throw new NotFoundException(typeof(ApplicationUser), userID);
            }

            dbContext.Update(user);
            dbContext.UpdateRange(user.LeagueUsers);

            user.UserName = username;
            user.Name = name;
            foreach(var leagueUser in user.LeagueUsers)
            {
                leagueUser.DisplayName = username;
            }

            if(profilePic != null)
            {
                await UploadProfilePic(user.Id, profilePic);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}