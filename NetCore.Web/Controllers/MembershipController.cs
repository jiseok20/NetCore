using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Web.Models;
using NuGet.Protocol.Plugins;

namespace NetCore.Web.Controllers
{
    public class MembershipController : Controller
    {
        //private IUser _user = new UserService(); // Interface에서 service를 사용하기 위해 Service Class 인스턴스를 받아온다.
        //의존성 주입 - 생성자 주입 방법 (닷넷코어에서 기본적으로 제공하는 의존성 주입 방식)
        //생성자의 파라미터를 통해 인터페이스를 지정하여 서비스클래스 인스턴스를 받아옴
        private IUser _user;
        public MembershipController(IUser user) { _user = user; }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // 위조방지토큰을 통해 View로부터 받은 Post Data가 유효한지 검증
        // Data => Services => Web
        // Data => Services
        // Data => Web
        public IActionResult Login(LoginInfo login)
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
            return View(login);
        }
    }
}


