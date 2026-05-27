using System;
using System.Collections.Generic;
using BigAmbitionsTrainer.UI.Components;
using Il2CppBigAmbitions.Rivals;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace BigAmbitionsTrainer.Modules;

public static class RivalsModule
{
	public class RivalInfo
	{
		public string Name;

		public string Id;

		public bool Defeated;

		public string Neighbourhood;

		public int Buildings;

		public int Businesses;

		public float WeeklyIncome;

		internal RivalData _data;
	}

	public static string StatusMessage { get; private set; } = "";

	public static bool StatusIsSuccess { get; private set; }

	public static System.Collections.Generic.List<RivalInfo> Rivals { get; private set; } = new System.Collections.Generic.List<RivalInfo>();

	public static int SelectedRivalIndex { get; set; }

	public static void Initialize()
	{
		MelonLogger.Msg("[RivalsModule] Initialized.");
	}

	public static void OnUpdate()
	{
	}

	public static void RefreshRivals()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Rivals.Clear();
			var rivalDataCache = RivalsHelper.RivalDataCache;
			if (rivalDataCache != null)
			{
				var enumerator = rivalDataCache.GetEnumerator();
				while (enumerator.MoveNext())
				{
					RivalData value = enumerator.Current.Value;
					try
					{
						RivalInfo rivalInfo = new RivalInfo();
						rivalInfo._data = value;
						try
						{
							rivalInfo.Name = value.rivalName;
						}
						catch
						{
							rivalInfo.Name = "Unknown";
						}
						try
						{
							rivalInfo.Id = value.id;
						}
						catch
						{
							rivalInfo.Id = "";
						}
						try
						{
							rivalInfo.Defeated = RivalsHelper.IsRivalDefeated(value.id);
						}
						catch
						{
						}
						try
						{
							rivalInfo.Neighbourhood = ((object)value.MostActiveNeighborhood/*cast due to constrained. prefix*/).ToString();
						}
						catch
						{
							rivalInfo.Neighbourhood = "?";
						}
						try
						{
							rivalInfo.Buildings = ((value.ownedBuildings != null) ? value.ownedBuildings.Count : 0);
						}
						catch
						{
						}
						try
						{
							rivalInfo.Businesses = ((value.ownedBusinesses != null) ? value.ownedBusinesses.Count : 0);
						}
						catch
						{
						}
						try
						{
							rivalInfo.WeeklyIncome = value.WeeklyIncome;
						}
						catch
						{
						}
						Rivals.Add(rivalInfo);
					}
					catch
					{
					}
				}
			}
			if (SelectedRivalIndex >= Rivals.Count)
			{
				SelectedRivalIndex = Math.Max(0, Rivals.Count - 1);
			}
			SetStatus($"Found {Rivals.Count} rivals.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void DefeatSelectedRival()
	{
		try
		{
			if (SelectedRivalIndex < 0 || SelectedRivalIndex >= Rivals.Count)
			{
				SetStatus("No rival selected.", success: false);
				return;
			}
			RivalInfo rivalInfo = Rivals[SelectedRivalIndex];
			try
			{
				RivalsHelper.OnRivalDefeat(rivalInfo._data);
				SetStatus("Defeated " + rivalInfo.Name + ".", success: true);
			}
			catch (Exception ex)
			{
				SetStatus("Defeat failed: " + ex.Message, success: false);
			}
		}
		catch (Exception ex2)
		{
			SetStatus("Error: " + ex2.Message, success: false);
		}
	}

	public static void ShutdownRivalBusinesses()
	{
		try
		{
			if (SelectedRivalIndex < 0 || SelectedRivalIndex >= Rivals.Count)
			{
				SetStatus("No rival selected.", success: false);
				return;
			}
			RivalInfo rivalInfo = Rivals[SelectedRivalIndex];
			RivalsHelper.ShutdownAllRivalBusinesses(rivalInfo._data);
			SetStatus("Shut down " + rivalInfo.Name + "'s businesses.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void SellRivalRealEstate()
	{
		try
		{
			if (SelectedRivalIndex < 0 || SelectedRivalIndex >= Rivals.Count)
			{
				SetStatus("No rival selected.", success: false);
				return;
			}
			RivalInfo rivalInfo = Rivals[SelectedRivalIndex];
			RivalsHelper.SellAllRealEstate(rivalInfo.Id);
			SetStatus("Sold " + rivalInfo.Name + "'s real estate.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void StopRivalAttacks()
	{
		try
		{
			if (SelectedRivalIndex < 0 || SelectedRivalIndex >= Rivals.Count)
			{
				SetStatus("No rival selected.", success: false);
				return;
			}
			RivalInfo rivalInfo = Rivals[SelectedRivalIndex];
			RivalsHelper.StopSpecialRivalAttacks(rivalInfo._data);
			SetStatus("Stopped " + rivalInfo.Name + "'s attacks.", success: true);
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
		}
	}

	public static void DefeatAllRivals()
	{
		try
		{
			if (Rivals.Count == 0)
			{
				SetStatus("No rivals loaded. Press Refresh first.", success: false);
				return;
			}
			int num = 0;
			for (int i = 0; i < Rivals.Count; i++)
			{
				try
				{
					RivalInfo rivalInfo = Rivals[i];
					if (!rivalInfo.Defeated && rivalInfo._data != null)
					{
						RivalsHelper.OnRivalDefeat(rivalInfo._data);
						num++;
					}
				}
				catch
				{
				}
			}
			SetStatus($"Defeated {num} rivals.", success: true);
			ToastNotification.Show($"Defeated {num} rivals!");
			RefreshRivals();
		}
		catch (Exception ex)
		{
			SetStatus("Error: " + ex.Message, success: false);
			ToastNotification.Show("Error defeating rivals: " + ex.Message, success: false);
		}
	}

	private static void SetStatus(string msg, bool success)
	{
		StatusMessage = msg;
		StatusIsSuccess = success;
	}
}
