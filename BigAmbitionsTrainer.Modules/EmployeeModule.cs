using System;
using BigAmbitionsTrainer.UI.Components;
using Il2Cpp;
using Il2CppBigAmbitions.Characters.Skills;
using Il2CppEntities;
using Il2CppHelpers;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace BigAmbitionsTrainer.Modules;

public static class EmployeeModule
{
	public static int EmployeeCount { get; private set; }

	public static float SalaryMultiplier { get; private set; } = 1f;

	public static int SelectedEmployeeIndex { get; set; }

	public static string SelectedEmployeeName { get; private set; } = "";

	public static float SelectedEmployeeSatisfaction { get; private set; }

	public static float SelectedEmployeeWage { get; private set; }

	public static string StatusMessage { get; private set; } = "";

	public static bool StatusIsSuccess { get; private set; }

	public static int CandidateSkillLevel { get; set; } = 100;

	public static void Initialize()
	{
		CandidateSkillLevel = 100;
		MelonLogger.Msg("[EmployeeModule] Initialized.");
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
			List<EmployeeInstance> employeeInstances = current.EmployeeInstances;
			EmployeeCount = employeeInstances?.Count ?? 0;
			GameVariables gameVariables = current.gameVariables;
			if (gameVariables != null)
			{
				SalaryMultiplier = gameVariables.employeeHourlySalaryMultiplier;
			}
			if (employeeInstances != null && employeeInstances.Count > 0)
			{
				if (SelectedEmployeeIndex >= employeeInstances.Count)
				{
					SelectedEmployeeIndex = employeeInstances.Count - 1;
				}
				if (SelectedEmployeeIndex < 0)
				{
					SelectedEmployeeIndex = 0;
				}
				try
				{
					EmployeeInstance val = employeeInstances[SelectedEmployeeIndex];
					if (val != null)
					{
						try
						{
							CharacterData characterData = val.characterData;
							SelectedEmployeeName = ((characterData != null) ? characterData.name : null) ?? "Unknown";
						}
						catch
						{
							SelectedEmployeeName = "Unknown";
						}
						try
						{
							SelectedEmployeeSatisfaction = val.satisfaction;
						}
						catch
						{
						}
						try
						{
							SelectedEmployeeWage = val.hourlyWage;
							return;
						}
						catch
						{
							return;
						}
					}
					return;
				}
				catch
				{
					return;
				}
			}
			SelectedEmployeeName = "";
			SelectedEmployeeSatisfaction = 0f;
			SelectedEmployeeWage = 0f;
		}
		catch
		{
		}
	}

	public static void MaxSatisfaction()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			List<EmployeeInstance> val = ((current != null) ? current.EmployeeInstances : null);
			if (val == null || val.Count == 0)
			{
				SetStatus("No employees.", success: false);
				return;
			}
			EmployeeInstance val2 = val[SelectedEmployeeIndex];
			if (val2 != null)
			{
				val2.satisfaction = 100f;
				SetStatus("Set " + SelectedEmployeeName + " satisfaction to 100.", success: true);
			}
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void ApplySalaryMultiplier(float value)
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
			MelonLogger.Warning("[Employee] Error setting salary multiplier: " + ex.Message);
		}
	}

	public static void MaxAllSatisfaction()
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			List<EmployeeInstance> val = ((current != null) ? current.EmployeeInstances : null);
			if (val == null || val.Count == 0)
			{
				SetStatus("No employees.", success: false);
				return;
			}
			int num = 0;
			for (int i = 0; i < val.Count; i++)
			{
				try
				{
					EmployeeInstance val2 = val[i];
					if (val2 != null)
					{
						val2.satisfaction = 100f;
						num++;
					}
				}
				catch
				{
				}
			}
			SetStatus($"Maxed satisfaction for {num} employees.", success: true);
			ToastNotification.Show($"Maxed satisfaction for {num} employees.");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void SetAllWages(float wage)
	{
		try
		{
			GameInstance current = SaveGameManager.Current;
			List<EmployeeInstance> val = ((current != null) ? current.EmployeeInstances : null);
			if (val == null || val.Count == 0)
			{
				SetStatus("No employees.", success: false);
				return;
			}
			int num = 0;
			for (int i = 0; i < val.Count; i++)
			{
				try
				{
					EmployeeInstance val2 = val[i];
					if (val2 != null)
					{
						val2.hourlyWage = wage;
						num++;
					}
				}
				catch
				{
				}
			}
			SetStatus($"Set wage to ${wage:F2} for {num} employees.", success: true);
			ToastNotification.Show($"Set wage to ${wage:F2} for {num} employees.");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void GenerateCandidate(int skillIndex, int skillLevel)
	{
		try
		{
			RecruitmentHelper.Command_GenerateCandidate((SkillName)skillIndex, skillLevel);
			SetStatus($"Generated candidate: skill={skillIndex}, level={skillLevel}.", success: true);
			ToastNotification.Show($"Generated candidate (skill {skillIndex}, level {skillLevel}).");
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error generating candidate: " + ex.Message, success: false);
		}
	}

	private static void SetStatus(string message, bool success)
	{
		StatusMessage = message;
		StatusIsSuccess = success;
	}
}
