using basketSurvey.Contracts.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace basketSurvey.Services
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
        {
            var user = await _userManager.Users.Where(x => x.Id == userId).ProjectToType<UserProfileResponse>().SingleAsync();

            return Result.Success(user);
        }

        public async Task<Result> UpdateInfoAsync(string userId,UserUpdateRequest request)
        {
            //var user = await _userManager.Users.Where(x => x.Id == userId).SingleAsync();

            //user = request.Adapt(user);

            //await _userManager.UpdateAsync(user);

            //return Result.Success();

            await _userManager.Users.Where(x => x.Id == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(u => u.FirstName, request.FirstName)
                .SetProperty(u => u.LastName, request.LastName));

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            

            var result = await _userManager.ChangePasswordAsync(user,request.CurrentPassword,request.NewPassword);

            if (result.Succeeded)

                return Result.Success();

           
                var error = result.Errors.First();

                return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));

        }
    }
}
