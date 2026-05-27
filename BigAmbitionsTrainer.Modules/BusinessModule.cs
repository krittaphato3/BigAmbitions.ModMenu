using System;
using System.Collections.Generic;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.UI.Components;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace BigAmbitionsTrainer.Modules;

public static class BusinessModule
{
	public class PlayerBusinessInfo
	{
		public string DisplayName;

		public string BusinessName;

		public int HouseNumber;

		public string StreetName;

		internal BuildingRegistration _registration;
	}

	public static bool AllCoursesUnlocked { get; private set; }

	public static bool AllContactsUnlocked { get; private set; }

	public static bool DisableWholesaleImportLimits { get; private set; }

	public static bool AllProductsFromImporters { get; private set; }

	public static float CustomerPromotionMultiplier { get; set; }

	public static float EmployeeSalaryMultiplier { get; set; }

	public static float WholesaleUrgentFeeMultiplier { get; set; }

	public static float ImporterUrgentFeeMultiplier { get; set; }

	public static float BankInterestRate { get; set; }

	public static float RivalsDifficultyMultiplier { get; set; }

	public static string StatusMessage { get; private set; } = "";

	public static bool StatusIsSuccess { get; private set; }

	public static List<PlayerBusinessInfo> PlayerBusinesses { get; private set; } = new List<PlayerBusinessInfo>();

	public static string SearchFilter { get; set; } = "";

	public static void Initialize()
	{
		MelonLogger.Msg("[BusinessModule] Initialized.");
	}

	public static void OnUpdate()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				AllCoursesUnlocked = val.allCoursesUnlocked;
				AllContactsUnlocked = val.allContactsUnlocked;
				DisableWholesaleImportLimits = val.disableWholesaleAndImportLimits;
				AllProductsFromImporters = val.allProductsAvailableFromImporters;
				CustomerPromotionMultiplier = val.baseCustomerPromotionMultiplier;
				EmployeeSalaryMultiplier = val.employeeHourlySalaryMultiplier;
				WholesaleUrgentFeeMultiplier = val.wholesaleUrgentFeeMultiplier;
				ImporterUrgentFeeMultiplier = val.importerUrgentFeeMultiplier;
				BankInterestRate = val.bankInterestRate;
				RivalsDifficultyMultiplier = val.rivalsDifficultyMultiplier;
				if (TrainerConfig.AllCoursesUnlocked && !val.allCoursesUnlocked)
				{
					val.allCoursesUnlocked = true;
				}
				if (TrainerConfig.AllContactsUnlocked && !val.allContactsUnlocked)
				{
					val.allContactsUnlocked = true;
				}
				if (TrainerConfig.DisableWholesaleImportLimits && !val.disableWholesaleAndImportLimits)
				{
					val.disableWholesaleAndImportLimits = true;
				}
				if (TrainerConfig.AllProductsFromImporters && !val.allProductsAvailableFromImporters)
				{
					val.allProductsAvailableFromImporters = true;
				}
			}
		}
		catch
		{
		}
	}

	public static void ToggleAllCourses(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.allCoursesUnlocked = value;
			}
			TrainerConfig.AllCoursesUnlocked = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ToggleAllContacts(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.allContactsUnlocked = value;
			}
			TrainerConfig.AllContactsUnlocked = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ToggleWholesaleImportLimits(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.disableWholesaleAndImportLimits = value;
			}
			TrainerConfig.DisableWholesaleImportLimits = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ToggleAllProductsFromImporters(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.allProductsAvailableFromImporters = value;
			}
			TrainerConfig.AllProductsFromImporters = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ApplyCustomerPromotionMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.baseCustomerPromotionMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ApplyEmployeeSalaryMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.employeeHourlySalaryMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ApplyWholesaleUrgentFeeMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.wholesaleUrgentFeeMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ApplyImporterUrgentFeeMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.importerUrgentFeeMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ApplyBankInterestRate(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.bankInterestRate = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void ApplyRivalsDifficultyMultiplier(float value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.rivalsDifficultyMultiplier = value;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] " + ex.Message);
		}
	}

	public static void MaxAllSatisfaction()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				SetStatus("No save loaded.", success: false);
				return;
			}
			List<BuildingRegistration> buildingRegistrations = current.BuildingRegistrations;
			if (buildingRegistrations == null)
			{
				SetStatus("No building data.", success: false);
				return;
			}
			int num = 0;
			for (int i = 0; i < buildingRegistrations.Count; i++)
			{
				try
				{
					BuildingRegistration val = buildingRegistrations[i];
					if (val != null && val.BuildingOwnedByPlayer && val.satisfaction != null)
					{
						val.satisfaction.customerService = 100;
						val.satisfaction.pricing = 100;
						val.satisfaction.cleanliness = 100;
						val.satisfaction.facility = 100;
						val.satisfaction.overall = 100;
						num++;
					}
				}
				catch
				{
				}
			}
			SetStatus($"Maxed satisfaction for {num} buildings.", success: true);
			ToastNotification.Show($"Maxed satisfaction for {num} buildings.");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error: " + ex.Message, success: false);
		}
	}

	public static void RefreshPlayerBusinesses()
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			PlayerBusinesses.Clear();
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				return;
			}
			List<BuildingRegistration> buildingRegistrations = current.BuildingRegistrations;
			if (buildingRegistrations == null)
			{
				return;
			}
			string value = (SearchFilter ?? "").Trim().ToLowerInvariant();
			for (int i = 0; i < buildingRegistrations.Count; i++)
			{
				try
				{
					BuildingRegistration val = buildingRegistrations[i];
					if (val == null || !val.BuildingOwnedByPlayer)
					{
						continue;
					}
					string text = "";
					try
					{
						text = val.GetDisplayName();
					}
					catch
					{
						text = "Unknown";
					}
					string text2 = "";
					try
					{
						text2 = val.BusinessName ?? "";
					}
					catch
					{
					}
					if (string.IsNullOrEmpty(value) || text.ToLowerInvariant().Contains(value) || text2.ToLowerInvariant().Contains(value))
					{
						PlayerBusinessInfo playerBusinessInfo = new PlayerBusinessInfo();
						playerBusinessInfo.DisplayName = text;
						playerBusinessInfo.BusinessName = text2;
						playerBusinessInfo._registration = val;
						try
						{
							playerBusinessInfo.HouseNumber = val.StreetNumber;
							playerBusinessInfo.StreetName = ((object)val.StreetName/*cast due to constrained. prefix*/).ToString();
						}
						catch
						{
							playerBusinessInfo.HouseNumber = 0;
							playerBusinessInfo.StreetName = "?";
						}
						PlayerBusinesses.Add(playerBusinessInfo);
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Business] Error refreshing businesses: " + ex.Message);
		}
	}

	public static void TeleportToBusiness(int index)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (index < 0 || index >= PlayerBusinesses.Count)
			{
				SetStatus("Invalid business index.", success: false);
				return;
			}
			PlayerBusinessInfo playerBusinessInfo = PlayerBusinesses[index];
			GameManager.Command_TeleportPlayerToAddress(playerBusinessInfo._registration.StreetNumber, playerBusinessInfo._registration.StreetName);
			SetStatus("Teleported to " + playerBusinessInfo.DisplayName + ".", success: true);
			ToastNotification.Show("Teleported to " + playerBusinessInfo.DisplayName);
		}
		catch (Exception ex)
		{
			SetStatus("Teleport error: " + ex.Message, success: false);
			ToastNotification.Show("Teleport error: " + ex.Message, success: false);
		}
	}

	private static void SetStatus(string message, bool success)
	{
		StatusMessage = message;
		StatusIsSuccess = success;
	}
}
