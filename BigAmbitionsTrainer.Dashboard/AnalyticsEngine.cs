using System;
using System.Collections.Generic;
using System.Linq;

namespace BigAmbitionsTrainer.Dashboard;

public static class AnalyticsEngine
{
	public static AnalyticsResult Compute(List<Transaction> transactions)
	{
		AnalyticsResult analyticsResult = new AnalyticsResult();
		if (transactions == null || transactions.Count == 0)
		{
			return analyticsResult;
		}
		analyticsResult.TransactionCount = transactions.Count;
		analyticsResult.MinDay = transactions.Min((Transaction t) => t.Day);
		analyticsResult.MaxDay = transactions.Max((Transaction t) => t.Day);
		analyticsResult.DaySpan = analyticsResult.MaxDay - analyticsResult.MinDay + 1;
		List<Transaction> list = transactions.Where((Transaction t) => t.IsIncome).ToList();
		List<Transaction> list2 = transactions.Where((Transaction t) => t.IsExpense).ToList();
		analyticsResult.TotalIncome = list.Sum((Transaction t) => t.Amount);
		analyticsResult.TotalExpenses = list2.Sum((Transaction t) => t.Amount);
		analyticsResult.IncomeByType = GroupByType(list, analyticsResult.TotalIncome);
		analyticsResult.ExpensesByType = GroupByType(list2, analyticsResult.TotalExpenses);
		analyticsResult.RevenueByCompany = GroupByCompany(list);
		analyticsResult.ExpensesByCompany = GroupByCompany(list2);
		analyticsResult.DailyNetIncome = ComputeDailyTotals(transactions, analyticsResult.MinDay, analyticsResult.MaxDay);
		analyticsResult.IncomeVsExpensesBars = ComputeIncomeVsExpenses(list, list2);
		return analyticsResult;
	}

	private static List<TypeBreakdown> GroupByType(List<Transaction> transactions, float total)
	{
		float absTotal = Math.Abs(total);
		if (absTotal <= 0f)
		{
			absTotal = 1f;
		}
		return (from t in transactions
			group t by t.Type ?? "Unknown" into g
			select new TypeBreakdown
			{
				Type = g.Key,
				Amount = g.Sum((Transaction t) => t.Amount),
				Percentage = Math.Abs(g.Sum((Transaction t) => t.Amount)) / absTotal * 100f
			} into b
			orderby Math.Abs(b.Amount) descending
			select b).ToList();
	}

	private static List<CompanyBreakdown> GroupByCompany(List<Transaction> transactions)
	{
		return (from t in transactions
			group t by t.Company ?? "Other" into g
			select new CompanyBreakdown
			{
				Company = g.Key,
				Amount = g.Sum((Transaction t) => t.Amount)
			} into b
			orderby Math.Abs(b.Amount) descending
			select b).ToList();
	}

	private static List<DailyTotal> ComputeDailyTotals(List<Transaction> transactions, int minDay, int maxDay)
	{
		Dictionary<int, DailyTotal> dictionary = new Dictionary<int, DailyTotal>();
		for (int i = minDay; i <= maxDay; i++)
		{
			dictionary[i] = new DailyTotal
			{
				Day = i
			};
		}
		foreach (Transaction transaction in transactions)
		{
			if (!dictionary.ContainsKey(transaction.Day))
			{
				dictionary[transaction.Day] = new DailyTotal
				{
					Day = transaction.Day
				};
			}
			if (transaction.IsIncome)
			{
				dictionary[transaction.Day].Income += transaction.Amount;
			}
			else
			{
				dictionary[transaction.Day].Expenses += transaction.Amount;
			}
		}
		List<DailyTotal> list = dictionary.Values.OrderBy((DailyTotal d) => d.Day).ToList();
		if (list.Count > 60)
		{
			return AggregateIntoBuckets(list, 7);
		}
		return list;
	}

	private static List<DailyTotal> AggregateIntoBuckets(List<DailyTotal> daily, int bucketSize)
	{
		List<DailyTotal> list = new List<DailyTotal>();
		for (int i = 0; i < daily.Count; i += bucketSize)
		{
			int num = Math.Min(i + bucketSize, daily.Count);
			List<DailyTotal> range = daily.GetRange(i, num - i);
			list.Add(new DailyTotal
			{
				Day = range[0].Day,
				Income = range.Sum((DailyTotal d) => d.Income),
				Expenses = range.Sum((DailyTotal d) => d.Expenses)
			});
		}
		return list;
	}

	private static List<IncomeExpenseBar> ComputeIncomeVsExpenses(List<Transaction> income, List<Transaction> expenses)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (Transaction item in income)
		{
			hashSet.Add(item.Type ?? "Unknown");
		}
		foreach (Transaction expense in expenses)
		{
			hashSet.Add(expense.Type ?? "Unknown");
		}
		Dictionary<string, float> dictionary = (from t in income
			group t by t.Type ?? "Unknown").ToDictionary((IGrouping<string, Transaction> g) => g.Key, (IGrouping<string, Transaction> g) => g.Sum((Transaction t) => t.Amount));
		Dictionary<string, float> dictionary2 = (from t in expenses
			group t by t.Type ?? "Unknown").ToDictionary((IGrouping<string, Transaction> g) => g.Key, (IGrouping<string, Transaction> g) => Math.Abs(g.Sum((Transaction t) => t.Amount)));
		List<IncomeExpenseBar> list = new List<IncomeExpenseBar>();
		foreach (string item2 in hashSet)
		{
			float num = (dictionary.ContainsKey(item2) ? dictionary[item2] : 0f);
			float num2 = (dictionary2.ContainsKey(item2) ? dictionary2[item2] : 0f);
			if (num > 0f || num2 > 0f)
			{
				list.Add(new IncomeExpenseBar
				{
					Label = item2,
					Income = num,
					Expenses = num2
				});
			}
		}
		return list.OrderByDescending((IncomeExpenseBar b) => b.Income + b.Expenses).ToList();
	}

	public static List<Transaction> FilterByDayRange(List<Transaction> all, int fromDay, int toDay)
	{
		return all.Where((Transaction t) => t.Day >= fromDay && t.Day <= toDay).ToList();
	}

	public static List<Transaction> FilterByCompanies(List<Transaction> all, HashSet<string> companies)
	{
		if (companies == null || companies.Count == 0)
		{
			return all;
		}
		return all.Where((Transaction t) => companies.Contains(t.Company ?? "Other")).ToList();
	}

	public static List<Transaction> FilterByTypes(List<Transaction> all, HashSet<string> types)
	{
		if (types == null || types.Count == 0)
		{
			return all;
		}
		return all.Where((Transaction t) => types.Contains(t.Type ?? "Unknown")).ToList();
	}

	public static List<string> GetAllCompanies(List<Transaction> all)
	{
		return (from c in all.Select((Transaction t) => t.Company ?? "Other").Distinct()
			orderby c
			select c).ToList();
	}

	public static List<string> GetAllTypes(List<Transaction> all)
	{
		return (from t in all.Select((Transaction t) => t.Type ?? "Unknown").Distinct()
			orderby t
			select t).ToList();
	}
}
