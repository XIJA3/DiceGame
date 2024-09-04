//using DataModels.IRepository;
//using ApplicationTemplate.Infrastructure.Repository;
//using ApplicationTemplate.Server.Common.Interfaces;
//using ApplicationTemplate.Server.Common.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;

//namespace ApplicationTemplate.Infrastructure.Identity;

//public class IdentityService(
//    UserManager<ApplicationUser> userManager,
//    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
//    IAuthorizationService authorizationService,
//    IPlayerRepository playerRepository) : IIdentityService
//{
//    private readonly UserManager<ApplicationUser> _userManager = userManager;
//    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
//    private readonly IAuthorizationService _authorizationService = authorizationService;
//    private readonly IPlayerRepository _playerRepository = playerRepository;

//    public async Task<string?> GetUserNameAsync(long userId)
//    {
//        var user = await _playerRepository.GetPlayerById(userId);

//        return user?.UserName;
//    }

//    public async Task<long> CreateUserAsync(string userName)
//    {
//        var createdUser = await _playerRepository.CreateAsync(userName);

//        return createdUser.Id;
//    }

//    //public async Task<bool> IsInRoleAsync(long userId, string role)
//    //{
//    //    var user = await _userManager.FindByIdAsync(userId);

//    //    return user != null && await _userManager.IsInRoleAsync(user, role);
//    //}

//    //public async Task<bool> AuthorizeAsync(long userId, string policyName)
//    //{
//    //    var user = await _userManager.FindByIdAsync(userId);

//    //    if (user == null)
//    //    {
//    //        return false;
//    //    }

//    //    var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

//    //    var result = await _authorizationService.AuthorizeAsync(principal, policyName);

//    //    return result.Succeeded;
//    //}

//    public async Task<Result> DeleteUserAsync(long userId)
//    {
//        var user = await _playerRepository.GetPlayerById(userId);

//        var user = await _userManager.FindByIdAsync(userId);

//        return user != null ? await DeleteUserAsync(user) : Result.Success();
//    }
//}
