using System;
using Il2Cpp;
using MelonLoader;

namespace BigAmbitionsTrainer.Modules;

public static class MoneyModule
{
	public static float CurrentMoney { get; private set; }

	public static float CurrentNetWorth { get; private set; }

	public static int TaxPercentage { get; set; }

	public static float MarketPriceMultiplier { get; set; }

	public static float ExportMultiplier { get; set; }

	public static string StatusMessage { get; private set; } = "";

	public static bool StatusIsSuccess { get; private set; }

	public static void Initialize()
	{
		MelonLogger.Msg("[MoneyModule] Initialized.");
	}

	public static void OnUpdate()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current != null)
			{
				CurrentMoney = current.Money;
				CurrentNetWorth = current.NetWorth;
				GameVariables gameVariables = current.gameVariables;
				if (gameVariables != null)
				{
					TaxPercentage = gameVariables.taxPercentage;
					MarketPriceMultiplier = gameVariables.marketPriceMultiplier;
					ExportMultiplier = gameVariables.exportMultiplier;
				}
			}
		}
		catch
		{
		}
	}

	public static void AddMoney(float amount)
	{
		try
		{
			GameManager.Command_ChangeMoney(amount);
			SetStatus($"Added ${amount:N0}", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void SetMoney(float amount)
	{
		try
		{
			GameManager.Command_SetMoney(amount);
			SetStatus($"Money set to ${amount:N0}", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void ApplyTaxPercentage(int value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.taxPercentage = value;
				TaxPercentage = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[MoneyModule] Error setting tax: " + ex.Message);
		}
	}

	public static void ApplyMarketPriceMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.marketPriceMultiplier = value;
				MarketPriceMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[MoneyModule] Error setting market price multiplier: " + ex.Message);
		}
	}

	public static void ApplyExportMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.exportMultiplier = value;
				ExportMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[MoneyModule] Error setting export multiplier: " + ex.Message);
		}
	}

	private static void SetStatus(string message, bool success)
	{
		StatusMessage = message;
		StatusIsSuccess = success;
	}
}
