-- Session Performance dataset. Bind @StartDate and @EndDate in Report Builder.
SELECT u.DisplayName AS Teacher,
 SUM(CASE WHEN b.Status = 5 THEN 1 ELSE 0 END) AS CompletedSessions,
 SUM(CASE WHEN b.Status = 3 THEN 1 ELSE 0 END) AS CancelledSessions,
 SUM(CASE WHEN b.Status = 0 THEN 1 ELSE 0 END) AS PendingSessions,
 CAST(SUM(CASE WHEN b.Status = 5 THEN 1 ELSE 0 END) AS decimal(10,2)) / NULLIF(COUNT(b.Id), 0) AS CompletionRate,
 AVG(CAST(r.Rating AS decimal(10,2))) AS AverageRating
FROM UserProfiles u JOIN SkillListings s ON s.TeacherId = u.Id
JOIN Bookings b ON b.SkillListingId = s.Id AND b.CreatedAt >= @StartDate AND b.CreatedAt < DATEADD(day, 1, @EndDate)
LEFT JOIN Reviews r ON r.BookingId = b.Id
GROUP BY u.DisplayName;
