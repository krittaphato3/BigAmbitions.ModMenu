using System;
using BigAmbitionsTrainer.Config;
using Il2Cpp;
using Il2CppHelpers;
using MelonLoader;
using UnityEngine;

namespace BigAmbitionsTrainer.Modules;

public static class VehicleModule
{
	public static float CurrentFuel { get; private set; }

	public static float CurrentCondition { get; private set; }

	public static float CurrentDirtiness { get; private set; }

	public static float UnpaidParkingAmount { get; private set; }

	public static bool HasSelectedVehicle { get; private set; }

	public static bool IsVehicleDamageDisabled { get; private set; }

	public static bool IsVehicleFuelDisabled { get; private set; }

	public static string StatusMessage { get; private set; } = "";

	public static bool StatusIsSuccess { get; private set; }

	public static void Initialize()
	{
		MelonLogger.Msg("[VehicleModule] Initialized.");
	}

	public static void OnUpdate()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				IsVehicleDamageDisabled = val.disableVehicleDamage;
				IsVehicleFuelDisabled = val.disableVehicleFuel;
				if (TrainerConfig.DisableVehicleDamage && !val.disableVehicleDamage)
				{
					val.disableVehicleDamage = true;
				}
				if (TrainerConfig.DisableVehicleFuel && !val.disableVehicleFuel)
				{
					val.disableVehicleFuel = true;
				}
			}
			GameManager instance = InstanceBehavior<GameManager>.Instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			VehicleController selectedVehicle = instance.selectedVehicle;
			if ((Object)(object)selectedVehicle != (Object)null)
			{
				HasSelectedVehicle = true;
				CurrentFuel = selectedVehicle.GetCurrentFuel();
				CurrentCondition = selectedVehicle.GetCurrentCondition();
				VehicleInstance vehicleInstance = selectedVehicle.vehicleInstance;
				if (vehicleInstance != null)
				{
					CurrentDirtiness = vehicleInstance.dirtiness;
					UnpaidParkingAmount = vehicleInstance.unpaidParkingAmount;
				}
			}
			else
			{
				HasSelectedVehicle = false;
				CurrentFuel = 0f;
				CurrentCondition = 0f;
				CurrentDirtiness = 0f;
				UnpaidParkingAmount = 0f;
			}
		}
		catch
		{
		}
	}

	public static void ToggleVehicleDamage(bool disabled)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.disableVehicleDamage = disabled;
			}
			TrainerConfig.DisableVehicleDamage = disabled;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Vehicle] Error toggling damage: " + ex.Message);
		}
	}

	public static void ToggleVehicleFuel(bool disabled)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.disableVehicleFuel = disabled;
			}
			TrainerConfig.DisableVehicleFuel = disabled;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Vehicle] Error toggling fuel: " + ex.Message);
		}
	}

	public static void RepairVehicle()
	{
		try
		{
			VehicleController selectedVehicle = GetSelectedVehicle();
			if ((Object)(object)selectedVehicle == (Object)null)
			{
				SetStatus("No vehicle selected.", success: false);
				return;
			}
			selectedVehicle.Repair();
			SetStatus("Vehicle repaired.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void RefuelVehicle()
	{
		try
		{
			VehicleController selectedVehicle = GetSelectedVehicle();
			if ((Object)(object)selectedVehicle == (Object)null)
			{
				SetStatus("No vehicle selected.", success: false);
				return;
			}
			selectedVehicle.SetFuel(100f);
			SetStatus("Vehicle refueled to 100%.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void CleanVehicle()
	{
		try
		{
			VehicleController selectedVehicle = GetSelectedVehicle();
			if ((Object)(object)selectedVehicle == (Object)null)
			{
				SetStatus("No vehicle selected.", success: false);
				return;
			}
			selectedVehicle.SetDirtiness(0f);
			SetStatus("Vehicle cleaned.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void ClearParkingTickets()
	{
		try
		{
			VehicleController selectedVehicle = GetSelectedVehicle();
			if ((Object)(object)selectedVehicle == (Object)null)
			{
				SetStatus("No vehicle selected.", success: false);
				return;
			}
			VehicleInstance vehicleInstance = selectedVehicle.vehicleInstance;
			if (vehicleInstance != null)
			{
				vehicleInstance.unpaidParkingAmount = 0f;
				SetStatus("Parking tickets cleared.", success: true);
			}
			else
			{
				SetStatus("No vehicle instance found.", success: false);
			}
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void TowToGasStation()
	{
		try
		{
			VehicleHelper.Command_TowVehicle((TowDestination)0);
			SetStatus("Towing to gas station...", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void TowToAutoRepair()
	{
		try
		{
			VehicleHelper.Command_TowVehicle((TowDestination)1);
			SetStatus("Towing to auto repair...", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	private static VehicleController GetSelectedVehicle()
	{
		GameManager instance = InstanceBehavior<GameManager>.Instance;
		if (instance == null)
		{
			return null;
		}
		return instance.selectedVehicle;
	}

	private static void SetStatus(string message, bool success)
	{
		StatusMessage = message;
		StatusIsSuccess = success;
	}
}
