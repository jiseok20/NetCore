using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        #region private methods
        private IEnumerable<User> GetUserInfos()
        {
            return new List<User>()
            {
                new User()
                {
                    UserId = "jadejs",
                    UserName = "강영택",
                    UserEmail = "ytkang@ls-electric.com",
                    Password = "123456"
                }
            };
        }

        private bool checkTheUserInfo(string userId, string password)//사용자 정보 체크
        {
            return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();//Any 메서드 : 리스트 데이터 유무체크
        }
        #endregion

        bool IUser.MatchTheUserInfo(LoginInfo login)//Service class : Interface를 상속받은 후에 명시적으로 Interface 구현
        {
            return checkTheUserInfo(login.UserId, login.Password);
        }
    }
}
