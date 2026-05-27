using System.Collections.Generic;

namespace BigAmbitionsTrainer.Dashboard;

public class AnalyticsResult
{
	public float TotalIncome { get; set; }

	public float TotalExpenses { get; set; }

	public float NetIncome => TotalIncome + TotalExpenses;

	public int TransactionCount { get; set; }

	public int DaySpan { get; set; }

	public int MinDay { get; set; }

	public int MaxDay { get; set; }

	public List<TypeBreakdown> IncomeByType { get; set; } = new List<TypeBreakdown>();

	public List<TypeBreakdown> ExpensesByType { get; set; } = new List<TypeBreakdown>();

	public List<CompanyBreakdown> RevenueByCompany { get; set; } = new List<CompanyBreakdown>();

	public List<CompanyBreakdown> ExpensesByCompany { get; set; } = new List<CompanyBreakdown>();

	public List<DailyTotal> DailyNetIncome { get; set; } = new List<DailyTotal>();

	public List<IncomeExpenseBar> IncomeVsExpensesBars { get; set; } = new List<IncomeExpenseBar>();
}
