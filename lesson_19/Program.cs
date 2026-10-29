using College_DB.Data;
using College_DB.Models;

var builder = WebApplication.CreateBuilder(args);

string conn_str =
        "Data Source=COMP11A1\\SQLEXPRESS;" +
        "Initial Catalog=College_DB;" +
        "Integrated Security=True;" +
        "TrustServerCertificate=True;";

builder.Services.AddSingleton(new StudentRepository(conn_str));
builder.Services.AddSingleton(new GroupRepository(conn_str));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapGet("/students/all", (StudentRepository repo) =>
    Results.Ok(repo.GetAllStudents()));

app.MapGet("/groups/all", (GroupRepository repo) =>
    Results.Ok(repo.GetAllGroups()));

app.MapPost("/students/add", (Student student, StudentRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(student.FirstName) ||
        string.IsNullOrWhiteSpace(student.LastName) ||
        student.Age < 1 || student.Age > 120 || student.GroupId < 1)
        return Results.BadRequest("Проверьте данные студента.");

    int id = repo.CreateStudent(student);
    return Results.Ok(id);
});

//app.MapPut("/students/{id:int}", (int id, Student student, StudentRepository repo) =>
//    repo.UpdateStudent(id, student) ? Results.Ok(id) : Results.NotFound());

app.MapPut("/students/{id:int}", (int id, Student student) =>
{
    Console.WriteLine(id);
    student.StudentId = id;
    Console.WriteLine(student.StudentId);
    repo.UpdateStudent(id, student);
    return Results.NoContent();
}

app.MapDelete("/students/{id:int}", (int id, StudentRepository repo) =>
    repo.DeleteStudent(id) ? Results.Ok() : Results.NotFound());

app.MapPost("/groups/add", (Group group, GroupRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(group.GroupName))
        return Results.BadRequest("Введите название группы.");

    int id = repo.CreateGroup(group.GroupName.Trim());
    return Results.Ok(id);
});

app.Run();
