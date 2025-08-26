using JeeBeginner.Classes;
using JeeBeginner.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AngularBackEnd.MiddleWare
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CheckPermission : Attribute, IAsyncActionFilter
    {
        private readonly string _permission;
        public CheckPermission(string permission)
        {
            this._permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var httpContext = context.HttpContext;

                var authService = httpContext.RequestServices.GetService<ICustomAuthorizationService>();

                var configuration = httpContext.RequestServices.GetService<IConfiguration>();

                var jwtSecret = configuration["JWT:Secret"];

                var user = Ulities.GetUserByHeader(httpContext.Request.Headers, jwtSecret);

                if (user == null)
                {
                    context.Result = new JsonResult(JsonResultCommon.BatBuoc("Đăng nhập"))
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return; // kết thúc luôn
                }

                bool isHasPermission = authService.IsReadOnlyPermit(_permission, user.Username);

                if (!isHasPermission)
                {
                    context.Result = new JsonResult(JsonResultCommon.ThatBai("Bạn không có quyền thực hiện thao tác này"))
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return; // kết thúc luôn
                }

                // Nếu qua được hết check thì cho chạy tiếp action
                await next();
            }
            catch (Exception ex)
            {
                context.Result = new JsonResult(JsonResultCommon.Exception(ex))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
    }
