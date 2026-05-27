using System;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.Modules;
using BigAmbitionsTrainer.PhoneIntegration;
using BigAmbitionsTrainer.UI.Components;
using MelonLoader;

namespace BigAmbitionsTrainer.Core;

public class TrainerMain : MelonMod
{
	public override void OnInitializeMelon()
	{
		MelonLogger.Msg("===========================================");
		MelonLogger.Msg("  ItzRealOzone Trainer v4.0 loaded!");
		MelonLogger.Msg("  Open your phone to access the ItzRealOzone Trainer app");
		MelonLogger.Msg("===========================================");
		TrainerConfig.Initialize();
		MoneyModule.Initialize();
		PlayerStatsModule.Initialize();
		VehicleModule.Initialize();
		BusinessModule.Initialize();
		GameplayModule.Initialize();
		EmployeeModule.Initialize();
		WorldModule.Initialize();
		RivalsModule.Initialize();
		PhoneButtonInjector.Initialize();
		MelonLogger.Msg("[Trainer] All modules initialized successfully.");
	}

	public override void OnUpdate()
	{
		try
		{
			PlayerStatsModule.OnUpdate();
			VehicleModule.OnUpdate();
			BusinessModule.OnUpdate();
			GameplayModule.OnUpdate();
			EmployeeModule.OnUpdate();
			WorldModule.OnUpdate();
			MoneyModule.OnUpdate();
			RivalsModule.OnUpdate();
			PhoneButtonInjector.OnUpdate();
		}
		catch (Exception value)
		{
			MelonLogger.Error($"[Trainer] OnUpdate error: {value}");
		}
	}

	public override void OnGUI()
	{
		ToastNotification.DrawToasts();
	}
}
