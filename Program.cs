using System.Net;
using App.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MVC_1.Middleware;
using MVC_1.Models;
using MVC_1.Security.Handler;
using MVC_1.Security.Requirement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<FirstMiddleware>();

builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();//đăng ký Handler xử lý các Requirements


//  builder.Services.AddAuthentication()
//          .AddCookie(options =>
//         {
//             // options.ExpireTimeSpan = TimeSpan.FromMinutes(0);
//             // options.SlidingExpiration = true;
//             options.LoginPath ="/login/";
//             options.AccessDeniedPath = "/khongduoctruycap.html";
// });

builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/login";
    options.LogoutPath = $"/logout";
    options.AccessDeniedPath = $"/khongduoctruycap.html";
});

builder.Services.AddOptions();                                        // Kích hoạt Options
var mailsettings = builder.Configuration.GetSection("MailSettings");
//var x=mailsettings["Mail"]; Đọc giá trị của key Mail trong Section MailSetting
//Console.WriteLine(x);
// đọc config
builder.Services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject
builder.Services.AddTransient<IEmailSender, SendMailService>();


// builder.Services.AddAuthentication()
//                     .AddGoogle(options => {
//                         var gconfig = builder.Configuration.GetSection("Authentication:Google");
//                         options.ClientId = gconfig["ClientId"];
//                         options.ClientSecret = gconfig["ClientSecret"];
//                         // https://localhost:5001/signin-google
//                         options.CallbackPath =  "/dang-nhap-tu-google";
//                     })
//                     .AddFacebook(options => {
//                         var fconfig = builder.Configuration.GetSection("Authentication:Facebook");
//                         options.AppId  = fconfig["AppId"];
//                         options.AppSecret = fconfig["AppSecret"];
//                         options.CallbackPath =  "/dang-nhap-tu-facebook";
//                     })
//                     // .AddTwitter()
//                     // .AddMicrosoftAccount()
//                     ;


builder.Services.AddDbContext<AppDbContext>(
option => option.UseSqlServer(builder.Configuration.GetConnectionString("AppMvcDatabase"))
);
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddTransient<CartService>();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<AdminSidebarService>();

builder.Services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg =>
{                    // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "tranthach";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = false; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanView", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireClaim("user", "chophep");

    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("duocxoa", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireAssertion(context => context.User.HasClaim(c =>
        (c.Type == "user" || c.Type == "TemporaryBadgeId")
         ));
    });
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InAge", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.Requirements.Add(new AgeRequirement(1970, 2002));
        builder.Requirements.Add(new AuthorRequirement());
    });

});

var app = builder.Build();


//app.UseFirstMiddleware();//Middleware tự xây dựng


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePages(
    applicationBuilderError => applicationBuilderError
    .Run(async context =>
    {
        var response = context.Response;
        var code = response.StatusCode;
        var content = @$"
        <head>
        <meta charset='utf-8' />
        </head>
        <p style='color:red;font-size:30px'>Có lỗi xảy ra:{code}-{(HttpStatusCode)code}</p>";
        await context.Response.WriteAsync(content);
    })
);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "defaultarea",
    pattern: "{controller}/{action=Index}/{id?}",
    areaName:"DataBase"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
