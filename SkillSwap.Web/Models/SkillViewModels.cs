namespace SkillSwap.Web.Models;

public sealed record SkillCard(int Id, string Title, string Description, string TeacherName, string CategoryName);
public sealed record SkillDetailsViewModel(int Id, string Title, string Description, int TeacherId, string TeacherName, string CategoryName);
