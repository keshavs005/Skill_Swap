namespace SkillSwap.Application.DTOs;

public sealed record SkillListItem(int Id, string Title, string Description, string TeacherName, string CategoryName);
public sealed record SkillDetails(int Id, string Title, string Description, int TeacherId, string TeacherName, string CategoryName);
public sealed record CreateSkillRequest(int TeacherId, int CategoryId, string Title, string Description);
public sealed record CategoryDto(int Id, string Name, string? Description);
