using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NetCore.Data.ViewModels;

namespace NetCore.Web.Controllers
{
    public class DataController : Controller
    {
        private IDataProtector _protector;

        public DataController(IDataProtectionProvider provider) 
        {
            _protector = provider.CreateProtector("NetCore.Data.v1");
        }
        [HttpGet]
        public IActionResult AES()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AES(AESInfo aes)
        {
            string message = string.Empty;
            ModelState.Remove("EncUserInfo");
            ModelState.Remove("DecUserInfo");
            if (ModelState.IsValid)
            {
                string userInfo = aes.UserId + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo);//암호화 정보
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo);//복호화 정보  

                ViewData["Message"] = "암복호화가 성공적으로 이루어졌습니다.";

                return View(aes);
            }
            else 
            {
                message = "암복호화를 위한 정보를 올바르게 입력하세요";
                
            }
            ModelState.AddModelError(string.Empty,message);
            return View();
        }
    }
}
