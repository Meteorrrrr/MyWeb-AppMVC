namespace MVC_1.Middleware{
    public class FirstMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
          var path=context.Request.Path;
          if(path=="/xxxx")
          {
            await context.Response.WriteAsync("Cam truy cap");
          }


           await  next(context);
        }
    }

    public static class MiddlewareExtend
    {
        public static void UseFirstMiddleware(this WebApplication app)
        {
            app.UseMiddleware<FirstMiddleware>();
            
        }
    }



}