-- Skill Popularity Report dataset. Bind @StartDate and @EndDate in Report Builder.
SELECT c.Name AS Category, s.Title AS Skill, COUNT(DISTINCT s.Id) AS Listings,
	   COUNT(DISTINCT b.Id) AS Bookings,
	   SUM(CASE WHEN b.Status = 5 THEN 1 ELSE 0 END) AS CompletedSessions
FROM SkillCategories c
JOIN SkillListings s ON s.CategoryId = c.Id
LEFT JOIN Bookings b ON b.SkillListingId = s.Id AND b.CreatedAt >= @StartDate AND b.CreatedAt < DATEADD(day, 1, @EndDate)
GROUP BY c.Name, s.Title
ORDER BY Bookings DESC, s.Title;
