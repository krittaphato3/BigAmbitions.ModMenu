using System;
using Il2Cpp;
using MelonLoader;

namespace BigAmbitionsTrainer.Modules;

public static class WorldModule
{
	public static int CurrentDay { get; private set; }

	public static int CurrentHour { get; private set; }

	public static float CurrentMinute { get; private set; }

	public static float BankInterestMultiplier { get; set; } = 1f;

	public static void Initialize()
	{
		MelonLogger.Msg("[WorldModule] Initialized.");
	}

	public static void OnUpdate()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current != null)
			{
				CurrentDay = current.Day;
				CurrentHour = current.Hour;
				CurrentMinute = current.Minute;
				GameVariables gameVariables = current.gameVariables;
				if (gameVariables != null)
				{
					BankInterestMultiplier = gameVariables.bankInterestMultiplier;
				}
			}
		}
		catch
		{
		}
	}

	public static void ApplyBankInterestMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.bankInterestMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[World] Error setting bank interest multiplier: " + ex.Message);
		}
	}
}
