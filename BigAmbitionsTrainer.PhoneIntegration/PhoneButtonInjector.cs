using System;
using System.Runtime.CompilerServices;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.UI.Components;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using Il2CppUI.Smartphone;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BigAmbitionsTrainer.PhoneIntegration;

public static class PhoneButtonInjector
{
	private static GameObject _fullMenuButton;

	private static bool _fullMenuInjected;

	private static int _searchCooldown;

	private static int _retryCount;

	private static bool _explorationDone;

	private static int _startDelay;

	private const int StartDelayFrames = 180;

	private const int SearchInterval = 90;

	private const int MaxRetries = 50;

	public static bool IsInjected
	{
		get
		{
			if (_fullMenuInjected)
			{
				return (Object)(object)_fullMenuButton != (Object)null;
			}
			return false;
		}
	}

	private static bool _lastPhoneIntegration = true;

	public static void Initialize()
	{
		_fullMenuButton = null;
		_fullMenuInjected = false;
		_searchCooldown = 0;
		_retryCount = 0;
		_explorationDone = false;
		_startDelay = StartDelayFrames;
		_lastPhoneIntegration = TrainerConfig.PhoneIntegration;
		MelonLogger.Msg("[PhoneIntegration] Initialized.");
	}

	public static void OnUpdate()
	{
		bool phoneIntegration = TrainerConfig.PhoneIntegration;
		if (phoneIntegration != _lastPhoneIntegration)
		{
			_lastPhoneIntegration = phoneIntegration;
			if (phoneIntegration)
			{
				MelonLogger.Msg("[PhoneIntegration] Phone integration enabled, starting injection.");
				ResetState();
			}
			else
			{
				MelonLogger.Msg("[PhoneIntegration] Phone integration disabled, removing.");
				Remove();
			}
		}
		if (!phoneIntegration)
		{
			return;
		}
		if (_startDelay > 0)
		{
			_startDelay--;
			return;
		}
		if (_fullMenuInjected)
		{
			try
			{
				if (!((Object)(object)_fullMenuButton == (Object)null))
				{
					return;
				}
				MelonLogger.Msg("[PhoneIntegration] Injected button destroyed, will re-inject.");
				ResetState();
			}
			catch
			{
				ResetState();
			}
		}
		if (_retryCount < 50)
		{
			_searchCooldown--;
			if (_searchCooldown <= 0)
			{
				_searchCooldown = 90;
				_retryCount++;
				TryInject();
			}
		}
	}

	private static void ResetState()
	{
		_fullMenuInjected = false;
		_fullMenuButton = null;
		_retryCount = 0;
		_explorationDone = false;
		TrainerPanel.Destroy();
	}

	private static void TryInject()
	{
		try
		{
			if (!_explorationDone)
			{
				ExploreHierarchy();
				_explorationDone = true;
			}
			if (!_fullMenuInjected)
			{
				TryInjectFullMenu();
			}
			if (_retryCount >= 50 && !_fullMenuInjected)
			{
				MelonLogger.Warning("[PhoneIntegration] Max retries reached - phone injection failed. The phone UI may not be available yet.");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PhoneIntegration] TryInject error: " + ex.Message);
		}
	}

	private static void TryInjectFullMenu()
	{
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			FullMenu val = FindFullMenu();
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			Transform val2 = null;
			FullMenuAppButton val3 = null;
			try
			{
				Il2CppArrayBase<FullMenuAppButton> val4 = Resources.FindObjectsOfTypeAll<FullMenuAppButton>();
				if (val4 != null && val4.Length > 0)
				{
					foreach (FullMenuAppButton item in val4)
					{
						if (!((Object)(object)item == (Object)null) && !((Object)item).name.Contains("Template") && !((Object)item).name.Contains("template"))
						{
							val2 = ((Component)item).transform.parent;
							val3 = item;
							MelonLogger.Msg($"[PhoneIntegration] Found FullMenuAppButton '{((Object)item).name}' in '{((val2 != null) ? ((Object)val2).name : null)}'");
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Msg("[PhoneIntegration] Error finding FullMenuAppButtons: " + ex.Message);
			}
			if ((Object)(object)val2 == (Object)null || (Object)(object)val3 == (Object)null)
			{
				MelonLogger.Msg("[PhoneIntegration] Could not find FullMenu top bar.");
				return;
			}
			Transform val5 = val2.Find("TrainerFullMenuButton");
			if ((Object)(object)val5 != (Object)null)
			{
				_fullMenuButton = ((Component)val5).gameObject;
				_fullMenuInjected = true;
				EnsureTrainerPanel(val);
				return;
			}
			GameObject val6 = Object.Instantiate<GameObject>(((Component)val3).gameObject, val2);
			if (!((Object)(object)val6 == (Object)null))
			{
				((Object)val6).name = "TrainerFullMenuButton";
				val6.SetActive(true);
				val6.transform.SetAsLastSibling();
				SetButtonTitle(val6);
				SetIconColor(val6, new Color(0.95f, 0.6f, 0.15f, 1f));
				HookButtonClick(val6);
				_fullMenuButton = val6;
				_fullMenuInjected = true;
				EnsureTrainerPanel(val);
				MelonLogger.Msg("[PhoneIntegration] FullMenu button injected successfully.");
				ToastNotification.Show("ItzRealOzone Trainer added to phone!");
			}
		}
		catch (Exception ex2)
		{
			MelonLogger.Warning("[PhoneIntegration] TryInjectFullMenu error: " + ex2.Message);
		}
	}

	private static void EnsureTrainerPanel(FullMenu fullMenu)
	{
		if (TrainerPanel.IsBuilt)
		{
			return;
		}
		try
		{
			Transform val = null;
			try
			{
				val = fullMenu.appsContainer;
			}
			catch
			{
			}
			if ((Object)(object)val == (Object)null)
			{
				val = FindChildRecursive(((Component)fullMenu).transform, "AppsContainer");
			}
			if ((Object)(object)val == (Object)null)
			{
				GameObject val2 = GameObject.Find("Canvases/FullMenu/Canvas/AppsContainer");
				if ((Object)(object)val2 != (Object)null)
				{
					val = val2.transform;
				}
			}
			if ((Object)(object)val == (Object)null)
			{
				MelonLogger.Warning("[PhoneIntegration] Could not find AppsContainer.");
			}
			else
			{
				TrainerPanel.Build(val);
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PhoneIntegration] EnsureTrainerPanel error: " + ex.Message);
		}
	}

	private static void HookButtonClick(GameObject buttonGO)
	{
		try
		{
			Button val = buttonGO.GetComponent<Button>();
			if ((Object)(object)val == (Object)null)
			{
				val = buttonGO.GetComponentInChildren<Button>(true);
			}
			if ((Object)(object)val != (Object)null)
			{
				((UnityEventBase)val.onClick).RemoveAllListeners();
				((UnityEvent)val.onClick).AddListener((UnityAction)OnTrainerClicked);
				MelonLogger.Msg("[PhoneIntegration] Button click hooked.");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PhoneIntegration] HookButtonClick: " + ex.Message);
		}
	}

	private static void OnTrainerClicked()
	{
		try
		{
			MelonLogger.Msg("[PhoneIntegration] Trainer button clicked.");
			ShowTrainerPanel();
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PhoneIntegration] OnTrainerClicked: " + ex.Message);
		}
	}

	private static void ShowTrainerPanel()
	{
		try
		{
			FullMenu val = FindFullMenu();
			if ((Object)(object)val == (Object)null)
			{
				MelonLogger.Warning("[PhoneIntegration] FullMenu not found.");
				return;
			}
			EnsureTrainerPanel(val);
			if (!TrainerPanel.IsBuilt)
			{
				MelonLogger.Warning("[PhoneIntegration] TrainerPanel not built.");
				return;
			}
			try
			{
				var appButtons = val._appButtons;
				if (appButtons != null)
				{
					var enumerator = appButtons.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<AppName, FullMenuAppButton> current = enumerator.Current;
						try
						{
							FullMenuAppButton value = current.Value;
							if (value != null)
							{
								value.HideSelectedIcon();
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				if ((Object)(object)_fullMenuButton != (Object)null)
				{
					FullMenuAppButton component = _fullMenuButton.GetComponent<FullMenuAppButton>();
					if ((Object)(object)component != (Object)null)
					{
						component.ShowSelectedIcon();
					}
				}
			}
			catch
			{
			}
			TrainerPanel.Show();
			MelonLogger.Msg("[PhoneIntegration] Trainer panel shown.");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PhoneIntegration] ShowTrainerPanel error: " + ex.Message);
		}
	}

	public static void OpenTrainer()
	{
		ShowTrainerPanel();
	}

	public static void Remove()
	{
		if ((Object)(object)_fullMenuButton != (Object)null)
		{
			try
			{
				Object.Destroy((Object)(object)_fullMenuButton);
			}
			catch
			{
			}
			_fullMenuButton = null;
		}
		TrainerPanel.Destroy();
		_fullMenuInjected = false;
		_retryCount = 0;
		_explorationDone = false;
		MelonLogger.Msg("[PhoneIntegration] Removed.");
	}

	private static void SetButtonTitle(GameObject buttonGO)
	{
		try
		{
			AppButton component = buttonGO.GetComponent<AppButton>();
			if ((Object)(object)component != (Object)null && (Object)(object)component.appTitle != (Object)null)
			{
				component.appTitle.SetValue("Trainer", true);
				return;
			}
		}
		catch
		{
		}
		try
		{
			Il2CppArrayBase<TMP_Text> componentsInChildren = buttonGO.GetComponentsInChildren<TMP_Text>(true);
			if (componentsInChildren == null)
			{
				return;
			}
			foreach (TMP_Text item in componentsInChildren)
			{
				try
				{
					item.text = "Trainer";
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
	}

	private static void SetIconColor(GameObject buttonGO, Color color)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			AppButton component = buttonGO.GetComponent<AppButton>();
			if ((Object)(object)component != (Object)null && (Object)(object)component.icon != (Object)null)
			{
				((Graphic)component.icon).color = color;
				return;
			}
		}
		catch
		{
		}
		try
		{
			Il2CppArrayBase<Image> componentsInChildren = buttonGO.GetComponentsInChildren<Image>(true);
			if (componentsInChildren == null)
			{
				return;
			}
			foreach (Image item in componentsInChildren)
			{
				if ((Object)(object)item == (Object)null)
				{
					continue;
				}
				RectTransform component2 = ((Component)item).GetComponent<RectTransform>();
				if ((Object)(object)component2 != (Object)null)
				{
					Rect rect = component2.rect;
					if (rect.width > 40f)
					{
						((Graphic)item).color = color;
						break;
					}
				}
			}
		}
		catch
		{
		}
	}

	private static FullMenu FindFullMenu()
	{
		try
		{
			FullMenu val = Object.FindObjectOfType<FullMenu>();
			if ((Object)(object)val != (Object)null)
			{
				return val;
			}
			Il2CppArrayBase<FullMenu> val2 = Resources.FindObjectsOfTypeAll<FullMenu>();
			if (val2 != null && val2.Length > 0)
			{
				return val2[0];
			}
		}
		catch
		{
		}
		return null;
	}

	private static Transform FindChildRecursive(Transform parent, string name)
	{
		if ((Object)(object)parent == (Object)null)
		{
			return null;
		}
		Transform val = parent.Find(name);
		if ((Object)(object)val != (Object)null)
		{
			return val;
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (!((Object)(object)child == (Object)null))
			{
				if (((Object)child).name == name)
				{
					return child;
				}
				Transform val2 = FindChildRecursive(child, name);
				if ((Object)(object)val2 != (Object)null)
				{
					return val2;
				}
			}
		}
		return null;
	}

	private static void ExploreHierarchy()
	{
		MelonLogger.Msg("[PhoneIntegration] ========== EXPLORATION ==========");
		try
		{
			Il2CppArrayBase<FullMenuAppButton> val = Resources.FindObjectsOfTypeAll<FullMenuAppButton>();
			MelonLogger.Msg($"[PhoneIntegration] Found {val?.Length ?? 0} FullMenuAppButton instances");
			if (val != null)
			{
				foreach (FullMenuAppButton item in val)
				{
					if ((Object)(object)item != (Object)null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[PhoneIntegration]   '");
						defaultInterpolatedStringHandler.AppendFormatted(((Object)item).name);
						defaultInterpolatedStringHandler.AppendLiteral("' parent='");
						Transform parent = ((Component)item).transform.parent;
						defaultInterpolatedStringHandler.AppendFormatted((parent != null) ? ((Object)parent).name : null);
						defaultInterpolatedStringHandler.AppendLiteral("'");
						MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Msg("[PhoneIntegration] Exploration error: " + ex.Message);
		}
		MelonLogger.Msg("[PhoneIntegration] ========== EXPLORATION DONE ==========");
	}
}
