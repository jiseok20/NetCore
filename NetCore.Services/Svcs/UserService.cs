using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes; //DBFisrtDB
//using NetCore.Data.DataModels; //CodeFirstDB 할때는 Datamodels 안에 cs 파일 사용했음
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;

namespace NetCore.Services.Svcs
{
    public  class UserService : IUser
    {
        //의존성 주입
        private DBFirstDbContext _context; 
        public UserService(DBFirstDbContext context)
        {
            _context = context;
        }
        #region private method  

        private IEnumerable<User> GetUserInfos() 
        {
            return _context.Users.ToList();
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "jiseoklee",
            //        UserName="이지석",
            //        UserEmail="jiseok.lee@ls-electric.com",
            //        Password="123456"
            //    }
            //};
        }
        private User GetUserInfo(string userId, string password) {

            User user;
            #region 주석
            /*
             상황에 따라 사용하는 방법은 다름
             가급적이면 Lambda 식을 활용한 방법을 추천 (간단한 데이터)
             FromSql문을 쓸때는 Stored Procedure 방식을 추천함 (복잡한 데이터)
            */
            //Lambda
            //user = _context.Users.Where(u => userId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            //Fromsql
            //Table
            //user = _context.Users.FromSql($"SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.[User]")
            //                     .Where(u => userId.Equals(userId) && u.Password.Equals(password))
            //                     .FirstOrDefault();
            //버전업
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.[User]")
            //                     .Where(u => userId.Equals(userId) && u.Password.Equals(password))
            //                     .FirstOrDefault();
            //VIEW
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName,UserEmail,Password,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.uvwUser") //User-defined View =>uvwUser //여기부터는 뒤에 다 Sql구문 추가 해서 만듦
            //                     .Where(u => userId.Equals(userId) && u.Password.Equals(password))
            //                     .FirstOrDefault();
            //FUNCTION
            //user = _context.Users.FromSql($"SELECT UserId, UserName,UserEmail,Password,AccessFailedCount,IsMembershipWithdrawn,JoinedUtcDate FROM dbo.ufnUser({userId},{password})")//User-defined Table Function => ufnUser
            //                   .FirstOrDefault();
            #endregion
            //STORED PROCEDURE
            user = _context.Users.FromSqlInterpolated($"EXEC dbo.uspCheckLoginByUserId {userId}, {password}")//User-defined Stored Procedure => uspCheckLoginByUserId
                                 .ToList()
                                 .FirstOrDefault();

            if (user == null)
            {
                //접속실패횟수에 대한 증가
                //저장 프로시저에만 테이블을 수정했기 때문에 저장 프로시저로만 동작함
                int rowAffected;
                
                //SQL문 직접 작성
                //rowAffected = _context.Database.ExecuteSqlInterpolated($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId={userId}");

                //STORED PROCEDURE Sql Management Studio에서 FailedLoginByUserId를 CREATE PROCEDURE 작업 해줘서 이렇게 쓸 수 있었음
                rowAffected = _context.Database.ExecuteSqlInterpolated($"EXEC dbo.FailedLoginByUserId {userId}");
            }
            return user;
        
        }

        private bool checkTheUserInfo(string userId, string password) //사용자 정보 체크
        {
            //return GetUserInfos().Where(u=>u.UserId.Equals(userId) && u.Password.Equals(password)).Any();//Any method : 리스트 데이터 유무 체크
            return GetUserInfo(userId,password) !=null ? true : false;
        }
        #endregion
        bool IUser.MatchTheUserInfo(LoginInfo login) //interface에 묶여있는 method기 때문에 public,private,protected 적용 X , Interface를 상속받은 후에 명시적으로 interface 구현
        {
            return checkTheUserInfo(login.UserId, login.Password);
        }
    }
}
