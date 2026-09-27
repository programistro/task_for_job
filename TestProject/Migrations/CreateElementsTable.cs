using FluentMigrator;

namespace TestProject.Migrations;

[Migration(20260927000000, "Create elements table")]
public class CreateElementsTable : Migration
{
    public override void Up()
    {
        Create.Table("elements")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("content").AsString().NotNullable()
            .WithColumn("attribute").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("elements");
    }
}
