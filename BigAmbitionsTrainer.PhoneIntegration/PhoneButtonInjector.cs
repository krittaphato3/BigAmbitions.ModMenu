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

namespace BigAmbitionsTrainer.PhoneIntegration;

public static class PhoneButtonInjector
{
	private static GameObject _fullMenuButton;

	private static bool _fullMenuInjected;

	private static int _searchCooldown;

	private static int _retryCount;

	private static bool _explorationDone;

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

	public static void Initialize()
	{
		_fullMenuButton = null;
		_fullMenuInjected = false;
		_searchCooldown = 0;
		_retryCount = 0;
		_explorationDone = false;
		MelonLogger.Msg("[PhoneIntegration] Initialized.");
	}

	public static void OnUpdate()
	{
		if (!TrainerConfig.PhoneIntegration)
		{
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
			if (_retryCount % 10 == 0)
			{
				MelonLogger.Msg($"[PhoneIntegration] Attempt {_retryCount}/{50} injected={_fullMenuInjected}");
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
				AdjustButtonSizes(val2);
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
				((UnityEvent)val.onClick).AddListener(UnityAction.op_Implicit((Action)OnTrainerClicked));
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
				Dictionary<AppName, FullMenuAppButton> appButtons = val._appButtons;
				if (appButtons != null)
				{
					Enumerator<AppName, FullMenuAppButton> enumerator = appButtons.GetEnumerator();
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

	private static void AdjustButtonSizes(Transform container)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RectTransform component = ((Component)container).GetComponent<RectTransform>();
			if ((Object)(object)component == (Object)null)
			{
				return;
			}
			MelonLogger.Msg($"[PhoneIntegration] Container '{((Object)container).name}' anchoredPos={component.anchoredPosition} sizeDelta={component.sizeDelta} anchorMin={component.anchorMin} anchorMax={component.anchorMax}");
			HorizontalLayoutGroup component2 = ((Component)container).GetComponent<HorizontalLayoutGroup>();
			if ((Object)(object)component2 != (Object)null)
			{
				MelonLogger.Msg("[PhoneIntegration] Found HorizontalLayoutGroup, disabling it.");
				((Behaviour)component2).enabled = false;
			}
			GridLayoutGroup component3 = ((Component)container).GetComponent<GridLayoutGroup>();
			if ((Object)(object)component3 != (Object)null)
			{
				MelonLogger.Msg("[PhoneIntegration] Found GridLayoutGroup, disabling it.");
				((Behaviour)component3).enabled = false;
			}
			ContentSizeFitter component4 = ((Component)container).GetComponent<ContentSizeFitter>();
			if ((Object)(object)component4 != (Object)null)
			{
				MelonLogger.Msg("[PhoneIntegration] Found ContentSizeFitter, disabling it.");
				((Behaviour)component4).enabled = false;
			}
			Rect rect = component.rect;
			float num = rect.width;
			if (num <= 0f)
			{
				num = 1400f;
			}
			int num2 = 0;
			for (int i = 0; i < container.childCount; i++)
			{
				Transform child = container.GetChild(i);
				if ((Object)(object)child != (Object)null && ((Component)child).gameObject.activeSelf)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				return;
			}
			float num3 = num / (float)num2;
			if (num3 > 220f)
			{
				num3 = 220f;
			}
			int num4 = 0;
			for (int j = 0; j < container.childCount; j++)
			{
				Transform child2 = container.GetChild(j);
				if ((Object)(object)child2 == (Object)null || !((Component)child2).gameObject.activeSelf)
				{
					continue;
				}
				RectTransform component5 = ((Component)child2).GetComponent<RectTransform>();
				if ((Object)(object)component5 != (Object)null)
				{
					component5.anchorMin = new Vector2(0f, 1f);
					component5.anchorMax = new Vector2(0f, 1f);
					component5.pivot = new Vector2(0f, 1f);
					component5.sizeDelta = new Vector2(num3, component5.sizeDelta.y);
					component5.anchoredPosition = new Vector2((float)num4 * num3, 0f);
				}
				try
				{
					Il2CppArrayBase<TMP_Text> componentsInChildren = ((Component)child2).GetComponentsInChildren<TMP_Text>(true);
					if (componentsInChildren != null)
					{
						foreach (TMP_Text item in componentsInChildren)
						{
							if ((Object)(object)item != (Object)null && item.fontSize > 20f)
							{
								item.fontSize = 20f;
							}
						}
					}
				}
				catch
				{
				}
				num4++;
			}
			Vector2 anchoredPosition = component.anchoredPosition;
			float y = anchoredPosition.y - 30f;
			if ((Object)(object)container.Find("__bizmod_shifted") == (Object)null)
			{
				GameObject val = new GameObject("__bizmod_shifted");
				val.transform.SetParent(container, false);
				val.SetActive(false);
				anchoredPosition.y = y;
				component.anchoredPosition = anchoredPosition;
				MelonLogger.Msg($"[PhoneIntegration] Shifted container down to y={anchoredPosition.y}");
			}
			MelonLogger.Msg($"[PhoneIntegration] Adjusted {num2} buttons to width {num3:F0}px (container={num:F0}px)");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[PhoneIntegration] AdjustButtonSizes: " + ex.Message);
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
