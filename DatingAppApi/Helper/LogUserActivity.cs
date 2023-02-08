//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using DatingAppApi.Repositories.Interface;
//using Microsoft.Extensions.DependencyInjection;
//using System.Security.Claims;

//namespace DatingAppApi.Helper
//{
//    public class LogUserActivity : IAsyncActionFilter
//    {
//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            var resultContext = await next();
//            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
//            var repo = resultContext.HttpContext.RequestServices.GetService<IDataRepository>();
//            var user = await repo.GetUser(userId);
//            user.LastActive = DateTime.Now;
//            await repo.SaveAll();
//        }
//    }
//}
