namespace OfficeEntry.WebApp.Area.Identity;

////public class EditUserRequirement : IAuthorizationRequirement
////{
////    public EditUserRequirement(params Guid[] sids) => SIDs = sids;

////    public Guid[] SIDs { get; }
////}

////public class EditUserRequirementHandler : AuthorizationHandler<EditUserRequirement>
////{
////    private readonly IDomainUserService _domainUserService;
////    private readonly ICurrentUserService _currentUserService;

////    public EditUserRequirementHandler(IDomainUserService domainUserService, ICurrentUserService currentUserService)
////    {
////        _domainUserService = domainUserService;
////        _currentUserService = currentUserService;
////    }

////    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditUserRequirement requirement)
////    {
////        //var claims = context.User.Claims.ToArray();

////        foreach (var group in requirement.SIDs)
////        {
////            var isUserInGroup = _domainUserService.IsUserInGroup(AdAccount.For(_currentUserService.UserId), group);

////            if (isUserInGroup)
////            {
////                context.Succeed(requirement);
////                break;
////            }
////        }

////        //if (context.User.HasClaim(
////        //    c => c.Type == "Role" && c.Issuer == "OPC" && c.Value is "SystemAdministrator" or ""))
////        //{
////        //    context.Succeed(requirement);
////        //}

////        return Task.CompletedTask;
////    }
////}
