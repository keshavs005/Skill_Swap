-- SkillCredit Economy dataset. Bind @StartDate and @EndDate in Report Builder.
SELECT u.DisplayName, w.AvailableCredits, w.ReservedCredits,
	   SUM(CASE WHEN t.TransactionType IN (0, 3) AND t.ToWalletId = w.Id THEN t.Amount ELSE 0 END) AS CreditsEarned,
	   SUM(CASE WHEN t.TransactionType = 3 AND t.FromWalletId = w.Id THEN t.Amount ELSE 0 END) AS CreditsSpent
FROM UserProfiles u JOIN Wallets w ON w.UserProfileId = u.Id
LEFT JOIN CreditTransactions t ON (t.FromWalletId = w.Id OR t.ToWalletId = w.Id) AND t.CreatedAt >= @StartDate AND t.CreatedAt < DATEADD(day, 1, @EndDate)
GROUP BY u.DisplayName, w.AvailableCredits, w.ReservedCredits;
