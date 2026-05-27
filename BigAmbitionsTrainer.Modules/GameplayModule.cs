using System;
using System.Collections.Generic;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.UI.Components;
using Il2Cpp;
using Il2CppEntities;
using Il2CppHelpers;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigAmbitionsTrainer.Modules;

public static class GameplayModule
{
	public static float GameSpeed { get; private set; } = 1f;

	public static bool TrafficEnabled { get; private set; } = true;

	public static bool TutorialEnabled { get; private set; } = true;

	public static bool Invincibility { get; private set; }

	public static bool FpsTestMode { get; private set; }

	public static int CurrentDay { get; private set; }

	public static int CurrentHour { get; private set; }

	public static float CurrentMinute { get; private set; }

	public static string StatusMessage { get; private set; } = "";

	public static bool StatusIsSuccess { get; private set; }

	public static void Initialize()
	{
		MelonLogger.Msg("[GameplayModule] Initialized.");
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
					TutorialEnabled = gameVariables.tutorialEnabled;
					if (TrainerConfig.DisableTutorial && gameVariables.tutorialEnabled)
					{
						gameVariables.tutorialEnabled = false;
					}
				}
			}
			GameSpeed = GameManager.MinutesMultiplier;
			GameManager instance = InstanceBehavior<GameManager>.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				TrafficEnabled = instance.spawnTraffic;
				Invincibility = instance.setInvincibilityOnStart;
				if (TrainerConfig.DisableTraffic && instance.spawnTraffic)
				{
					instance.spawnTraffic = false;
				}
				if (TrainerConfig.Invincibility && !instance.setInvincibilityOnStart)
				{
					instance.setInvincibilityOnStart = true;
				}
			}
		}
		catch
		{
		}
	}

	public static void SetGameSpeed(float speed)
	{
		try
		{
			GameManager.MinutesMultiplier = speed;
			TrainerConfig.GameSpeed = speed;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Gameplay] Error setting game speed: " + ex.Message);
		}
	}

	public static void ToggleTraffic(bool enabled)
	{
		try
		{
			GameManager instance = InstanceBehavior<GameManager>.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				instance.spawnTraffic = enabled;
			}
			TrainerConfig.DisableTraffic = !enabled;
			if (!enabled)
			{
				GameManager.Command_ToggleTraffic();
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Gameplay] Error toggling traffic: " + ex.Message);
		}
	}

	public static void ToggleTutorial(bool enabled)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.tutorialEnabled = enabled;
			}
			TrainerConfig.DisableTutorial = !enabled;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Gameplay] Error toggling tutorial: " + ex.Message);
		}
	}

	public static void ToggleInvincibility(bool enabled)
	{
		try
		{
			GameManager instance = InstanceBehavior<GameManager>.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				instance.setInvincibilityOnStart = enabled;
			}
			TrainerConfig.Invincibility = enabled;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Gameplay] Error toggling invincibility: " + ex.Message);
		}
	}

	public static void SaveGame()
	{
		try
		{
			GameManager instance = InstanceBehavior<GameManager>.Instance;
			if ((Object)(object)instance == (Object)null)
			{
				SetStatus("GameManager not found.", success: false);
				return;
			}
			bool flag = instance.SaveGame("TrainerSave", false);
			SetStatus(flag ? "Game saved successfully." : "Save failed.", flag);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void TeleportToQuestTarget()
	{
		try
		{
			GameManager.Command_TeleportPlayerToQuestTarget();
			SetStatus("Teleported to quest target.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void TeleportToDestination()
	{
		try
		{
			GameManager.Command_TeleportPlayerToDestination();
			SetStatus("Teleported to destination.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void ToggleFpsTestMode()
	{
		try
		{
			GameManager.Command_ToggleFpsTestMode();
			FpsTestMode = !FpsTestMode;
			SetStatus("FPS Test Mode toggled.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void DeliverAllImportsPaid()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				SetStatus("No save loaded.", success: false);
				return;
			}
			int day = current.Day;
			var deliveryContracts = current.DeliveryContracts;
			int num = 0;
			if (deliveryContracts != null)
			{
				var enumerator = deliveryContracts.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DeliveryContract current2 = enumerator.Current;
					try
					{
						if (current2.enabled && current2.nextDeliveryDay > day)
						{
							current2.nextDeliveryDay = day;
							num++;
						}
					}
					catch
					{
					}
				}
			}
			ImportPartnership.DoAllDeliveries();
			SetStatus($"Delivered {num} contracts (paid).", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void DeliverAllImportsFree()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				SetStatus("No save loaded.", success: false);
				return;
			}
			int day = current.Day;
			var deliveryContracts = current.DeliveryContracts;
			int num = 0;
			var list = new System.Collections.Generic.List<float>();
			if (deliveryContracts != null)
			{
				var enumerator = deliveryContracts.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DeliveryContract current2 = enumerator.Current;
					try
					{
						list.Add(current2.deliveryFee);
						if (current2.enabled)
						{
							current2.deliveryFee = 0f;
							if (current2.nextDeliveryDay > day)
							{
								current2.nextDeliveryDay = day;
								num++;
							}
						}
					}
					catch
					{
						list.Add(0f);
					}
				}
			}
			ImportPartnership.DoAllDeliveries();
			if (deliveryContracts != null)
			{
				for (int i = 0; i < deliveryContracts.Count && i < list.Count; i++)
				{
					try
					{
						deliveryContracts[i].deliveryFee = list[i];
					}
					catch
					{
					}
				}
			}
			SetStatus($"Delivered {num} contracts (free).", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void SkipToNextDay()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				SetStatus("No save loaded.", success: false);
				return;
			}
			int day = current.Day;
			current.Day = day + 1;
			current.Hour = 6;
			current.Minute = 0f;
			SetStatus($"Skipped to Day {day + 1}, 06:00.", success: true);
			ToastNotification.Show($"Skipped to Day {day + 1}");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error skipping day: " + ex.Message, success: false);
		}
	}

	public static void SetTimeOfDay(int hour, int minute)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				SetStatus("No save loaded.", success: false);
				return;
			}
			current.Hour = hour;
			current.Minute = minute;
			SetStatus($"Time set to {hour:D2}:{minute:D2}.", success: true);
			ToastNotification.Show($"Time set to {hour:D2}:{minute:D2}");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void UnlockAllContacts()
	{
		try
		{
			ContactsHelper.UnlockAllContacts();
			SetStatus("All contacts unlocked.", success: true);
			ToastNotification.Show("All contacts unlocked!");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error: " + ex.Message, success: false);
		}
	}

	public static void CompleteQuest()
	{
		try
		{
			TutorialHelper.Command_CompleteQuest();
			SetStatus("Quest completed.", success: true);
			ToastNotification.Show("Quest completed!");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error: " + ex.Message, success: false);
		}
	}

	public static void CompleteObjective()
	{
		try
		{
			TutorialHelper.Command_CompleteObjective();
			SetStatus("Objective completed.", success: true);
			ToastNotification.Show("Objective completed!");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error: " + ex.Message, success: false);
		}
	}

	private static void SetStatus(string message, bool success)
	{
		StatusMessage = message;
		StatusIsSuccess = success;
	}
}
