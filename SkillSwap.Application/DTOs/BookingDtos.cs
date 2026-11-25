using SkillSwap.Domain.Enums;

namespace SkillSwap.Application.DTOs;

public sealed record CreateBookingRequest(int SkillListingId, int LearnerId, DateTime StartTime);
public sealed record BookingDto(int Id, int SkillListingId, int LearnerId, string SkillTitle, string TeacherName, DateTime StartTime, DateTime EndTime, BookingStatus Status, decimal CreditCost);
public sealed record ReviewRequest(int BookingId, int ReviewerId, int Rating, string Comment);
