using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCore.Data.DataModels;

namespace NetCore.Services.Data
{
    //2. Fluent API
    
    /// <summary>
    /// * Entity Framework Core의 Code-First
    /// Code-First의 장점
    /// 1. Table과 Column을 Application에서 관리
    /// 2. Migrations를 통한 이력관리
    /// Code-First의 단점
    /// 1. 사소한 작업을 Migrations하는 것이 번거로움
    /// 2. 운영서버에 바로 적용이 어려움
    /// 
    /// [과정]
    /// * 패키지 관리자 콘솔
    /// PM> add-migration AddingUserTables -project NetCore.Migrations
    ///     -> NetCore.Migrations 프로젝트에 Migrations 폴더 및 내부에 Migration 파일이 생성됨
    /// PM> update-database -project NetCore.Migrations
    ///     -> Microsoft SQL Server Management Studio에 DB가 생성됨
    /// * SQL Management에 로그인 후, 각 dbo.User, dbo.UserRole, dbo.UserRolesByUser 우클릭 후 Design 클릭하여
    ///   JoinedUtcDate 컬럼 속성의 Default Value or Binding에 SysUtcDateTime을 직접 넣어줬음
    ///     -> 각 정보가 생성될 때마다 그때의 시간이 기록되도록 하기 위해서 넣음
    /// </summary>

    //상속
    //CodeFirstDbContext : 자식클래스
    //DbContext : 부모클래스
    public class CodeFirstDbContext : DbContext
    {
        //생성자 상속
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {
        }

        //DB 테이블 리스트 지정
        public DbSet<User> Users { get; set; }

        //메서드 상속, 부모클래스에서 OnModelCreating 메서드가 virtual
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //4가지 작업
            //DB 테이블이름 변경
            modelBuilder.Entity<User>().ToTable(name: "User");

            //복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });

            //컬럼 기본값 지정
            modelBuilder.Entity<User>(e =>
            {
                e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
            });

            //인덱스 지정
            //modelBuilder.Entity<User>().HasIndex(c => new { c.UserEmail });
            modelBuilder.Entity<User>().HasIndex(c => new { c.UserEmail }).IsUnique(unique:true);
        }
    }
}
