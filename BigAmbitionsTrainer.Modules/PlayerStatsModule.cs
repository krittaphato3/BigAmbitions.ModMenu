using System;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.UI.Components;
using Il2Cpp;
using Il2CppBigAmbitions.Characters;
using Il2CppUI.Smartphone.Apps.Persona;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigAmbitionsTrainer.Modules;

public static class PlayerStatsModule
{
	private static ThirdPersonCharacter _cachedPlayer;

	private static int _playerSearchCooldown;

	private static int _hungerRefillCounter;

	private static int _readCooldown;

	private const int ReadInterval = 30;

	public static float CurrentEnergy { get; private set; }

	public static float CurrentHappiness { get; private set; }

	public static float CurrentHunger { get; private set; }

	public static bool IsEnergyDisabled { get; private set; }

	public static bool IsHappinessDisabled { get; private set; }

	public static bool IsAgingDisabled { get; private set; }

	public static bool IsHungerDisabled { get; private set; }

	public static int PlayerSpeedIndex { get; private set; }

	public static void Initialize()
	{
		_readCooldown = 10;
		MelonLogger.Msg("[PlayerStatsModule] Initialized.");
	}

	public static void OnUpdate()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			if (current == null)
			{
				return;
			}
			GameVariables gameVariables = current.gameVariables;

			if (gameVariables != null)
			{
				if (TrainerConfig.DisableEnergy && !gameVariables.disableEnergy)
				{
					gameVariables.disableEnergy = true;
				}
				if (TrainerConfig.DisableHappiness && !gameVariables.disableHappiness)
				{
					gameVariables.disableHappiness = true;
				}
				if (TrainerConfig.DisableAging && !gameVariables.disableAging)
				{
					gameVariables.disableAging = true;
				}
			}
			if (TrainerConfig.DisableHunger)
			{
				_hungerRefillCounter++;
				if (_hungerRefillCounter >= 60)
				{
					_hungerRefillCounter = 0;
					try
					{
						GameManager.Command_ChangeHunger(100);
					}
					catch
					{
					}
				}
			}

			_readCooldown--;
			if (_readCooldown > 0)
			{
				return;
			}
			_readCooldown = ReadInterval;

			CurrentEnergy = current.Energy;
			CurrentHappiness = current.Happiness;
			CurrentHunger = current.Hunger;
			IsHungerDisabled = TrainerConfig.DisableHunger;
			if (gameVariables != null)
			{
				IsEnergyDisabled = gameVariables.disableEnergy;
				IsHappinessDisabled = gameVariables.disableHappiness;
				IsAgingDisabled = gameVariables.disableAging;
			}
			try
			{
				if ((Object)(object)_cachedPlayer == (Object)null && _playerSearchCooldown <= 0)
				{
					_cachedPlayer = Object.FindObjectOfType<ThirdPersonCharacter>();
					_playerSearchCooldown = 60;
				}
				if (_playerSearchCooldown > 0)
				{
					_playerSearchCooldown--;
				}
				if ((Object)(object)_cachedPlayer != (Object)null)
				{
					var walkingSpeed = _cachedPlayer.walkingSpeed;
					if ((int)walkingSpeed == 1)
					{
						PlayerSpeedIndex = 0;
					}
					else if ((int)walkingSpeed == 2)
					{
						PlayerSpeedIndex = 1;
					}
					else if ((int)walkingSpeed == 3)
					{
						PlayerSpeedIndex = 2;
					}
					else if ((int)walkingSpeed == 4)
					{
						PlayerSpeedIndex = 3;
					}
				}
			}
			catch
			{
			}
		}
		catch
		{
		}
	}

	public static void SetEnergy(float value)
	{
		if (SaveGameManager.Current == null) return;
		try
		{
			GameManager.Command_SetEnergy(value);
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error setting energy: " + ex.Message);
		}
	}

	public static void ChangeHappiness(int amount)
	{
		if (SaveGameManager.Current == null) return;
		try
		{
			GameManager.Command_ChangeHappiness(amount);
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error changing happiness: " + ex.Message);
		}
	}

	public static void ChangeHunger(int amount)
	{
		if (SaveGameManager.Current == null) return;
		try
		{
			GameManager.Command_ChangeHunger(amount);
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error changing hunger: " + ex.Message);
		}
	}

	public static void FillAllNeeds()
	{
		if (SaveGameManager.Current == null) return;

		try { GameManager.Command_SetEnergy(100f); }
		catch (Exception ex) { MelonLogger.Warning("[PlayerStats] Error filling energy: " + ex.Message); }

		try { GameManager.Command_ChangeHappiness(100); }
		catch (Exception ex) { MelonLogger.Warning("[PlayerStats] Error filling happiness: " + ex.Message); }

		try { GameManager.Command_ChangeHunger(100); }
		catch (Exception ex) { MelonLogger.Warning("[PlayerStats] Error filling hunger: " + ex.Message); }
	}

	public static void ToggleDisableEnergy(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.disableEnergy = value;
			}
			TrainerConfig.DisableEnergy = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error toggling energy: " + ex.Message);
		}
	}

	public static void ToggleDisableHappiness(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.disableHappiness = value;
			}
			TrainerConfig.DisableHappiness = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error toggling happiness: " + ex.Message);
		}
	}

	public static void ToggleDisableAging(bool value)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			GameVariables val = ((current != null) ? current.gameVariables : null);
			if (val != null)
			{
				val.disableAging = value;
			}
			TrainerConfig.DisableAging = value;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error toggling aging: " + ex.Message);
		}
	}

	public static void SetPlayerSpeed(int speedIndex)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((Object)(object)_cachedPlayer == (Object)null)
			{
				_cachedPlayer = Object.FindObjectOfType<ThirdPersonCharacter>();
			}
			ThirdPersonCharacter cachedPlayer = _cachedPlayer;
			if ((Object)(object)cachedPlayer != (Object)null)
			{
				cachedPlayer.walkingSpeed = (ThirdPersonCharacter.WalkingSpeed)(speedIndex switch
				{
					1 => 2, 
					2 => 3, 
					3 => 4, 
					_ => 1, 
				});
				PlayerSpeedIndex = speedIndex;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Speed error: " + ex.Message);
		}
	}

	public static void ChangeAge(float amount)
	{
		try
		{
			GameManager.Command_ChangeAge(amount);
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error changing age: " + ex.Message);
		}
	}

	public static void ToggleDisableHunger(bool value)
	{
		try
		{
			TrainerConfig.DisableHunger = value;
			IsHungerDisabled = value;
			_hungerRefillCounter = 0;
			if (value)
			{
				try
				{
					GameManager.Command_ChangeHunger(100);
				}
				catch
				{
				}
			}
			ToastNotification.Show(value ? "Hunger decay disabled" : "Hunger decay enabled");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error toggling hunger: " + ex.Message);
		}
	}

	public static void SetAge(float delta)
	{
		try
		{
			GameManager.Command_ChangeAge(delta);
			string value = ((delta >= 0f) ? "+" : "");
			ToastNotification.Show($"Age changed by {value}{delta:F1} years.");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error setting age: " + ex.Message);
			ToastNotification.Show("Error changing age: " + ex.Message, success: false);
		}
	}

	public static void CompletePersonalGoals()
	{
		try
		{
			PersonalGoals.Command_CompleteAll();
			ToastNotification.Show("All personal goals completed!");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PlayerStats] Error completing personal goals: " + ex.Message);
			ToastNotification.Show("Error: " + ex.Message, success: false);
		}
	}
}
