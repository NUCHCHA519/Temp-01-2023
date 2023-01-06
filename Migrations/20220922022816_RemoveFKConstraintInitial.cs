using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterCoreMVC.Migrations
{
    public partial class RemoveFKConstraintInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE AppUser NOCHECK CONSTRAINT ALL
                ALTER TABLE tblookup NOCHECK CONSTRAINT ALL
                ALTER TABLE tbbasic_crud NOCHECK CONSTRAINT ALL
                ALTER TABLE tbstudent NOCHECK CONSTRAINT ALL                
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
