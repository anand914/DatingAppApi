﻿//using DatingAppApi.Repositories.Interface;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;

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