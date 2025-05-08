using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Web.Models;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace NetCore.Web.Controllers
{
    public class MembershipController : Controller
    {
        //private IUser _user = new UserService(); // Interface에서 service를 사용하기 위해 Service Class 인스턴스를 받아온다.
        //의존성 주입 - 생성자 주입 방법 (닷넷코어에서 기본적으로 제공하는 의존성 주입 방식)
        //생성자의 파라미터를 통해 인터페이스를 지정하여 서비스클래스 인스턴스를 받아옴
        private IUser _user;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IUser user)
        {
            _user = user;
            _context = accessor.HttpContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/Membership/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/Membership/LoginAsync")]
        [ValidateAntiForgeryToken] // 위조방지토큰을 통해 View로부터 받은 Post Data가 유효한지 검증
        //[AllowAnonymous]
        // Data => Services => Web
        // Data => Services
        // Data => Web
        // 비동기 함수로 변경함 원래는 그냥 있었는데
        public async Task<IActionResult> LoginAsync(LoginInfo login)
        {
            string message = string.Empty;
            if (ModelState.IsValid) 
            {
                //string userId = "jiseoklee";
                //string password = "123456";
                //뷰모델
                //서비스 개념
                //if (login.UserId.Equals(userId) && login.Password.Equals(password))
                if(_user.MatchTheUserInfo(login))
                {
                    //신원보증과 승인권한
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type:ClaimTypes.Name,
                                  value:userInfo.UserName),
                        new Claim(type:ClaimTypes.Role,
                                  value:userTopRole.RoleId + "|" + userTopRole.UserRole.RoleName + "|" + userTopRole.UserRole.RolePriority.ToString()),
                    }, authenticationType:CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal: new ClaimsPrincipal(identity: identity),
                                               properties: new AuthenticationProperties()
                                               {
                                                   IsPersistent = login.RememberMe,
                                                   ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                               });

                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다";
                    return RedirectToAction("Index", "Membership");
                }
                else { message = "로그인되지 않았습니다."; }
            }
            else 
            { 
                message = "로그인 정보를 올바르게 입력하세요.";
            }
            ModelState.AddModelError(string.Empty, message);
            return View("Login",login);
        }
        [HttpGet("Membership/LogOutAsync")]
        public async Task<IActionResult> LogOutAsync() {
            await _context.SignOutAsync(scheme:CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br /> 웹사이트를 원활히 이용하시려면 로그인하세요. ";
            return RedirectToAction("Index", "Membership");
        }
    }
}


