using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MVC_1.Models;
using MVC_1.Security.Requirement;

namespace MVC_1.Security.Handler
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly UserManager<AppUser> _userManager;


        public AppAuthorizationHandler(UserManager<AppUser> userManager)
        {
            _userManager=userManager;
        }
        public  Task HandleAsync(AuthorizationHandlerContext context)
        {


  
            var pendingRequirement=context.PendingRequirements.ToList();
            foreach(var requirement in pendingRequirement)
            {
                if(requirement is AgeRequirement)
                {
                    var inAge=InAge(context.User,(AgeRequirement)requirement);
                    if(inAge.Result)
                    {
                        context.Succeed(requirement);
                    }

                }

                if(requirement is AuthorRequirement)
                {
                    if(IsAuthor(context.User,context.Resource,(AuthorRequirement)requirement))
                    {
                         context.Succeed(requirement);
                        
                    }

                }
            }

            return Task.CompletedTask;
              
        }

        private bool IsAuthor(ClaimsPrincipal user, object? resource, AuthorRequirement requirement)
        {
           
           //Nếu là vai trò là Admin cho chỉnh sửa tất cả các bài viết
           //Nếu không có vai trò Admin thì kiểm tra tác giả của bài viết, 
           // nếu là tác giả bài viết thì mới cho chỉnh sửa
            if(user.IsInRole("Admin")) 
            {return true;}
            var post=resource as Post;
            var appUserTask=_userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser=appUserTask.Result;

            if(appUser?.Id==post?.AuthorId)
            {
                return true;
            }
            return false;
              
        }

        private async Task<bool> InAge(ClaimsPrincipal user, AgeRequirement requirement)
        {
            

            var appUser= await _userManager.GetUserAsync(user);
            if(appUser?.BirthDate==null)
            {
                return false;
            }

            var year=appUser.BirthDate.Value.Year;
            if(year>=requirement._Minyear&&year<=requirement._Maxyear)
            {
                return true;
            }
       
            return false;
        }
    }
}