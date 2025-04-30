using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;

namespace NetCore.Services.Data
{
    public class DBFirstDbContext : DbContext
    {
        //생성자 상속
        public DBFirstDbContext(DbContextOptions<DBFirstDbContext> options) : base(options)
        {
        }
        //DB table 리스트 지정 // 데이터베이스의 특정 테이블을 코드에서 다룰 수 있도록 설정하는 작업
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserRolesByUser> UserRolesByUsers { get; set; }
        // virtual
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //DB 테이블 이름 변경 및 매핑
            modelBuilder.Entity<User>().ToTable(name: "User"); //User 테이블을 지정
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");

            // 복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });
        }
    }

}

