using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Il2Cpp;
using MelonLoader;

namespace BigAmbitionsTrainer.Dashboard;

public static class TransactionParser
{
	private static readonly Regex RevenuePattern = new Regex("^(.+?)\\s+Revenue$", RegexOptions.Compiled);

	private static readonly Regex WagePattern = new Regex("^\\((.+?)\\)\\s+Daily\\s+Wage$", RegexOptions.Compiled);

	private static readonly Regex RentPattern = new Regex("^(.+?)\\s+Rent$", RegexOptions.Compiled);

	private static readonly Regex StockPattern = new Regex("^(.+?)\\s+(?:Stock|Inventory|Supplies)$", RegexOptions.Compiled);

	private static readonly Regex GenericCompanyPattern = new Regex("^\\((.+?)\\)\\s+", RegexOptions.Compiled);

	public static string FindDefaultPath()
	{
		string[] directories;
		try
		{
			string saveGameFolderPath = SaveGamePathHelper.SaveGameFolderPath;
			if (!string.IsNullOrEmpty(saveGameFolderPath))
			{
				string text = Path.Combine(saveGameFolderPath, "Transactions.csv");
				if (File.Exists(text))
				{
					MelonLogger.Msg("[Dashboard] Found CSV via SaveGamePathHelper: " + text);
					return text;
				}
				if (Directory.Exists(saveGameFolderPath))
				{
					directories = Directory.GetDirectories(saveGameFolderPath);
					for (int i = 0; i < directories.Length; i++)
					{
						string text2 = Path.Combine(directories[i], "Transactions.csv");
						if (File.Exists(text2))
						{
							return text2;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Dashboard] SaveGamePathHelper not available: " + ex.Message);
		}
		directories = new string[2]
		{
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "Hovgaard Games", "Big Ambitions"),
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "Hovgaard Games", "Big Ambitions")
		};
		foreach (string text3 in directories)
		{
			if (!Directory.Exists(text3))
			{
				continue;
			}
			string text4 = Path.Combine(text3, "Transactions.csv");
			if (File.Exists(text4))
			{
				return text4;
			}
			try
			{
				string[] directories2 = Directory.GetDirectories(text3);
				foreach (string path in directories2)
				{
					string text5 = Path.Combine(path, "Transactions.csv");
					if (File.Exists(text5))
					{
						return text5;
					}
					string path2 = Path.Combine(path, "saves");
					if (!Directory.Exists(path2))
					{
						continue;
					}
					string[] directories3 = Directory.GetDirectories(path2);
					for (int k = 0; k < directories3.Length; k++)
					{
						string text6 = Path.Combine(directories3[k], "Transactions.csv");
						if (File.Exists(text6))
						{
							return text6;
						}
					}
				}
			}
			catch (Exception ex2)
			{
				MelonLogger.Warning("[Dashboard] Error searching for save files: " + ex2.Message);
			}
		}
		return "";
	}

	public static List<Transaction> Parse(string filePath)
	{
		List<Transaction> list = new List<Transaction>();
		if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
		{
			MelonLogger.Warning("[Dashboard] File not found: " + filePath);
			return list;
		}
		try
		{
			string[] array = File.ReadAllLines(filePath);
			bool flag = true;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i].Trim();
				if (string.IsNullOrWhiteSpace(text))
				{
					continue;
				}
				if (flag)
				{
					flag = false;
					string text2 = text.ToLowerInvariant();
					if (text2.Contains("description") || text2.Contains("day") || text2.Contains("type"))
					{
						continue;
					}
				}
				Transaction transaction = ParseLine(text);
				if (transaction != null)
				{
					list.Add(transaction);
				}
			}
			MelonLogger.Msg($"[Dashboard] Parsed {list.Count} transactions from {filePath}");
		}
		catch (Exception value)
		{
			MelonLogger.Error($"[Dashboard] Failed to parse CSV: {value}");
		}
		return list;
	}

	private static Transaction ParseLine(string line)
	{
		try
		{
			List<string> list = SplitCsvLine(line);
			if (list.Count < 4)
			{
				return null;
			}
			string description = list[0].Trim().Trim('"');
			string s = list[1].Trim().Trim('"');
			string type = list[2].Trim().Trim('"');
			string s2 = list[3].Trim().Trim('"');
			string id = ((list.Count > 4) ? list[4].Trim().Trim('"') : "");
			if (!int.TryParse(s, out var result))
			{
				return null;
			}
			if (!float.TryParse(s2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result2))
			{
				return null;
			}
			return new Transaction
			{
				Description = description,
				Day = result,
				Type = type,
				Amount = result2,
				Id = id,
				Company = ExtractCompany(description)
			};
		}
		catch
		{
			return null;
		}
	}

	private static List<string> SplitCsvLine(string line)
	{
		List<string> list = new List<string>();
		bool flag = false;
		int num = 0;
		for (int i = 0; i < line.Length; i++)
		{
			switch (line[i])
			{
			case '"':
				flag = !flag;
				break;
			case ',':
				if (!flag)
				{
					list.Add(line.Substring(num, i - num));
					num = i + 1;
				}
				break;
			}
		}
		if (num <= line.Length)
		{
			list.Add(line.Substring(num));
		}
		return list;
	}

	private static string ExtractCompany(string description)
	{
		if (string.IsNullOrEmpty(description))
		{
			return null;
		}
		Match match = RevenuePattern.Match(description);
		if (match.Success)
		{
			return match.Groups[1].Value.Trim();
		}
		match = WagePattern.Match(description);
		if (match.Success)
		{
			return match.Groups[1].Value.Trim();
		}
		match = RentPattern.Match(description);
		if (match.Success)
		{
			return match.Groups[1].Value.Trim();
		}
		match = StockPattern.Match(description);
		if (match.Success)
		{
			return match.Groups[1].Value.Trim();
		}
		match = GenericCompanyPattern.Match(description);
		if (match.Success)
		{
			return match.Groups[1].Value.Trim();
		}
		return null;
	}
}
