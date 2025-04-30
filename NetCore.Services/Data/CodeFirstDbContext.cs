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
    //codefirstdbcontext는 dbcontext를 상속받음
    /*  Code-First의 장점
     * 1. Table과 Column을 Application에서 관리
     * 2. Migrations를 통한 이력관리
     *  
     *  Code-First의 단점
     *  1. 사소한 작업을 Migrations하는 것이 번거로움
     *  2. 운영서버에 바로 적용이 어려움
     * 
     * [과정] //패키지 관리자 콘솔
     * add-migration [MigrationName] - project [ProjectName]
     *               ex)AddingUserTables       ex) NetCore.Migrations            
     * : Project안에 Migrations폴더가 생기고 MigrationName의 파일이 생성됨
     * 
     * update-database -project [ProjectName]
     *                          ex)NetCore.Migrations
     * : SMMS에 Migraitions한 DB가 생성됨.
     *  Tip) SMMS에 Disign에서 기본값 설정해줘야 하는 값도 있음. ex) SysUtcDateTime()
     *       이유 : 시간같은 경우 코드에서 작성해주면 빌드하는 시간이 기본값으로 적용되기 때문에 의미가 없다.
     */
    public class CodeFirstDbContext :DbContext
    {
        //생성자 상속
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options):base(options) 
        {

        }
        //DB table 리스트 지정 // 데이터베이스의 특정 테이블을 코드에서 다룰 수 있도록 설정하는 작업
        public DbSet<User> Users { get; set; } //User 테이블을 DbSet으로 지정
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRolesByUser> UserRolesByUser { get; set; }

        //메서드 상속 , 부모클래스에서 OnModelCreating 메서드가 virtual로 선언되어있기 때문에 자식클래스에서 재정의 가능
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //4가지 작업
            // DB 테이블 이름 변경
            modelBuilder.Entity<User>().ToTable(name : "User"); //User 테이블을 지정
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");


            // 복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c=> new { c.UserId, c.RoleId });

            //컬럼 기본값 지정
            modelBuilder.Entity<User>(e => { e.Property(c => c.IsMemberShipWithdrawn).HasDefaultValue(value: false); });

            //컬럼 인덱스 지정
            modelBuilder.Entity<User>().HasIndex(c => new { c.UserEmail }).IsUnique(unique:true);

        }
    }
}
