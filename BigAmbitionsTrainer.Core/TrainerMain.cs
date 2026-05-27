using System;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.Modules;
using BigAmbitionsTrainer.PhoneIntegration;
using BigAmbitionsTrainer.UI.Components;
using MelonLoader;

namespace BigAmbitionsTrainer.Core;

public class TrainerMain : MelonMod
{
	private static int _updateCooldown;

	private const int UpdateInterval = 30;

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
		_updateCooldown = 5;
		MelonLogger.Msg("[Trainer] All modules initialized successfully.");
	}

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vKey);

	private const int VK_F8 = 0x77;

	private static bool _f8PrevDown;

	private static bool IsF8Pressed()
	{
		bool flag = (GetAsyncKeyState(VK_F8) & 0x8000) != 0;
		bool result = flag && !_f8PrevDown;
		_f8PrevDown = flag;
		return result;
	}

	public override void OnUpdate()
	{
		try
		{
			if (IsF8Pressed())
			{
				TrainerOverlay.Toggle();
			}
			PhoneButtonInjector.OnUpdate();
			_updateCooldown--;
			if (_updateCooldown > 0)
			{
				return;
			}
			_updateCooldown = UpdateInterval;
			PlayerStatsModule.OnUpdate();
			VehicleModule.OnUpdate();
			BusinessModule.OnUpdate();
			GameplayModule.OnUpdate();
			EmployeeModule.OnUpdate();
			WorldModule.OnUpdate();
			MoneyModule.OnUpdate();
			RivalsModule.OnUpdate();
		}
		catch (Exception value)
		{
			MelonLogger.Error($"[Trainer] OnUpdate error: {value}");
		}
	}

	public override void OnGUI()
	{
		TrainerOverlay.OnGUI();
		ToastNotification.DrawToasts();
		ConfirmationDialog.OnGUI();
		TooltipManager.OnGUI();
	}

	public override void OnApplicationQuit()
	{
		TrainerOverlay.Cleanup();
	}
}
