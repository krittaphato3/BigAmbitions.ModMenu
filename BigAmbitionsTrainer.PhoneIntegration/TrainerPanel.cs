using System;
using System.Runtime.CompilerServices;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.Modules;
using BigAmbitionsTrainer.UI.Components;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BigAmbitionsTrainer.PhoneIntegration;

public static class TrainerPanel
{
	private static GameObject _panelRoot;

	private static bool _built;

	private static GameObject[] _tabContents;

	private static GameObject[] _tabButtons;

	private static int _activeTab;

	private const int TabCount = 7;

	private static Sprite _spriteRoundCard;

	private static Sprite _spriteWhitebox;

	private static Sprite _spriteSplitter;

	private static Sprite _spriteRoundedBox;

	private static Sprite _spriteBarBg;

	private static Sprite _spriteScrollbar;

	private static TMP_FontAsset _gameFont;

	private static bool _assetsCached;

	private static RectTransform _indicatorRect;

	private static readonly System.Collections.Generic.List<(string name, GameObject card, GameObject header, bool collapsed, float originalHeight)> _cardStates = new System.Collections.Generic.List<(string, GameObject, GameObject, bool, float)>();

	private static string _searchText = "";

	private static readonly System.Collections.Generic.List<(GameObject go, string label, int tabIndex)> _searchableItems = new System.Collections.Generic.List<(GameObject, string, int)>();

	private static int _currentTabBeingBuilt = -1;

	private const float PanelWidth = 3840f;

	private const float TabBarHeight = 100f;

	private const float SplitterHeight = 8f;

	private const float ScrollbarWidth = 30f;

	private const float ContentPadX = 100f;

	private const float ContentPadTop = 40f;

	private const float ContentPadBottom = 40f;

	private const float CardPadX = 50f;

	private const float CardPadTop = 40f;

	private const float CardPadBottom = 40f;

	private const float CardSpacing = 40f;

	private const float CardInnerSpacing = 20f;

	private const float CardWidth = 3610f;

	private const float RowHeight = 80f;

	private const float RowSpacing = 16f;

	private const float SliderTotalHeight = 120f;

	private const float ToggleHeight = 80f;

	private const float HeaderHeight = 60f;

	private const float SectionHeight = 50f;

	private const float HelpTextHeight = 44f;

	private const float DividerHeight = 8f;

	private const float InputFieldHeight = 64f;

	private static readonly Color CardWhite = Color.white;

	private static readonly Color CardTextDark = new Color(0.15f, 0.17f, 0.22f, 1f);

	private static readonly Color CardTextMuted = new Color(0.45f, 0.48f, 0.55f, 1f);

	private static readonly Color TabTextActive = Color.white;

	private static readonly Color TabTextInactive = new Color(0.65f, 0.67f, 0.72f, 1f);

	private static readonly Color SplitterColor = new Color(1f, 1f, 1f, 0.149f);

	private static readonly Color SectionHeaderLight = new Color(0.9f, 0.91f, 0.93f, 1f);

	private static readonly Color ListBg = new Color(0.15f, 0.172f, 0.255f, 0.427f);

	private static readonly Color BtnBlue = new Color(0.271f, 0.477f, 0.66f, 1f);

	private static readonly Color BtnSuccess = new Color(0.22f, 0.72f, 0.35f, 1f);

	private static readonly Color BtnDanger = new Color(0.82f, 0.22f, 0.22f, 1f);

	private static readonly Color BtnWarning = new Color(0.9f, 0.58f, 0.15f, 1f);

	private static readonly Color BtnNeutral = new Color(0.3f, 0.32f, 0.38f, 1f);

	private static readonly Color ToggleOn = new Color(0.22f, 0.72f, 0.35f, 1f);

	private static readonly Color ToggleOff = new Color(0.3f, 0.32f, 0.38f, 1f);

	private static readonly Color SliderTrack = new Color(0.15f, 0.172f, 0.255f, 0.427f);

	private static readonly Color SliderFill = new Color(0.271f, 0.477f, 0.66f, 1f);

	private static readonly Color SliderHandle = Color.white;

	private static readonly Color InputBg = new Color(0.15f, 0.172f, 0.255f, 0.427f);

	private static readonly Color InputBorder = new Color(0.3f, 0.32f, 0.38f, 1f);

	private static readonly Color InputPlaceholder = new Color(0.5f, 0.53f, 0.6f, 1f);

	private static readonly Color InputText = new Color(0.9f, 0.91f, 0.93f, 1f);

	public static GameObject PanelRoot => _panelRoot;

	public static bool IsBuilt
	{
		get
		{
			if (_built)
			{
				return (Object)(object)_panelRoot != (Object)null;
			}
			return false;
		}
	}

	public static bool Build(Transform appsContainer)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		if (_built && (Object)(object)_panelRoot != (Object)null)
		{
			return true;
		}
		try
		{
			MelonLogger.Msg("[TrainerPanel] Building panel (manual positioning)...");
			FindGameAssets();
			_panelRoot = new GameObject("Trainer");
			_panelRoot.transform.SetParent(appsContainer, false);
			RectTransform obj = _panelRoot.AddComponent<RectTransform>();
			obj.anchorMin = Vector2.zero;
			obj.anchorMax = Vector2.one;
			obj.offsetMin = Vector2.zero;
			obj.offsetMax = Vector2.zero;
			BuildTabBar(_panelRoot.transform);
			BuildSplitterWithIndicator(_panelRoot.transform);
			BuildSearchBar(_panelRoot.transform);
			GameObject val = CreateObj("ContentArea", _panelRoot.transform);
			RectTransform component = val.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.pivot = new Vector2(0.5f, 0.5f);
			component.offsetMin = new Vector2(0f, 0f);
			component.offsetMax = new Vector2(0f, -188f);
			_tabContents = (GameObject[])(object)new GameObject[7];
			string[] array = new string[7] { "Money", "Player", "Vehicles", "Business", "Gameplay", "Employees", "Rivals" };
			for (int i = 0; i < 7; i++)
			{
				_currentTabBeingBuilt = i;
				GameObject val2 = CreateScrollView(val.transform, array[i]);
				Transform val3 = val2.transform.Find("Viewport/Content");
				if (!((Object)(object)val3 == (Object)null))
				{
					_tabContents[i] = val2;
					float num = 0f;
					switch (i)
					{
					case 0:
						num = BuildMoneyTab(val3);
						break;
					case 1:
						num = BuildPlayerTab(val3);
						break;
					case 2:
						num = BuildVehicleTab(val3);
						break;
					case 3:
						num = BuildBusinessTab(val3);
						break;
					case 4:
						num = BuildGameplayTab(val3);
						break;
					case 5:
						num = BuildEmployeeTab(val3);
						break;
					case 6:
						num = BuildRivalsTab(val3);
						break;
					}
					_currentTabBeingBuilt = -1;
					num += 20f;
					RectTransform component2 = CreateTMP(val3, "Credit", "Made by ItzRealOzone", 22, new Color(1f, 1f, 1f, 0.35f), (FontStyles)2, (TextAlignmentOptions)514).GetComponent<RectTransform>();
					component2.anchorMin = new Vector2(0f, 1f);
					component2.anchorMax = new Vector2(1f, 1f);
					component2.pivot = new Vector2(0.5f, 1f);
					component2.anchoredPosition = new Vector2(0f, 0f - num);
					component2.sizeDelta = new Vector2(0f, 50f);
					num += 70f;
					((Component)val3).GetComponent<RectTransform>().sizeDelta = new Vector2(0f, num);
					val2.SetActive(i == 0);
				}
			}
			_activeTab = 0;
			UpdateIndicatorPosition(0);
			_panelRoot.SetActive(false);
			_built = true;
			MelonLogger.Msg("[TrainerPanel] Panel built v2 - button fix applied.");
			return true;
		}
		catch (Exception value)
		{
			MelonLogger.Error($"[TrainerPanel] Build error: {value}");
			if ((Object)(object)_panelRoot != (Object)null)
			{
				Object.Destroy((Object)(object)_panelRoot);
			}
			_panelRoot = null;
			_built = false;
			return false;
		}
	}

	public static void Show()
	{
		if ((Object)(object)_panelRoot == (Object)null)
		{
			return;
		}
		try
		{
			Transform parent = _panelRoot.transform.parent;
			if ((Object)(object)parent != (Object)null)
			{
				for (int i = 0; i < parent.childCount; i++)
				{
					Transform child = parent.GetChild(i);
					if ((Object)(object)child != (Object)null && (Object)(object)((Component)child).gameObject != (Object)(object)_panelRoot)
					{
						((Component)child).gameObject.SetActive(false);
					}
				}
			}
			_panelRoot.SetActive(true);
			MelonLogger.Msg("[TrainerPanel] Trainer panel shown.");
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[TrainerPanel] Show error: " + ex.Message);
		}
	}

	public static void Hide()
	{
		if ((Object)(object)_panelRoot != (Object)null)
		{
			_panelRoot.SetActive(false);
		}
	}

	private static void DumpSizes(Transform t, int depth, int maxDepth)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)t == (Object)null || depth > maxDepth)
		{
			return;
		}
		if (depth >= 2 && !((Component)t).gameObject.activeSelf)
		{
			string value = new string(' ', depth * 2);
			MelonLogger.Msg($"[TrainerPanel] {value}'{((Object)t).name}' (INACTIVE, skipped)");
			return;
		}
		RectTransform component = ((Component)t).GetComponent<RectTransform>();
		string value2 = new string(' ', depth * 2);
		Image component2 = ((Component)t).GetComponent<Image>();
		string text = "";
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
		if ((Object)(object)component2 != (Object)null)
		{
			string text2 = text;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
			defaultInterpolatedStringHandler.AppendLiteral(" img=");
			defaultInterpolatedStringHandler.AppendFormatted<Color>(((Graphic)component2).color);
			defaultInterpolatedStringHandler.AppendLiteral(" sprite=");
			Sprite sprite = component2.sprite;
			defaultInterpolatedStringHandler.AppendFormatted((sprite != null) ? ((Object)sprite).name : null);
			text = text2 + defaultInterpolatedStringHandler.ToStringAndClear();
		}
		defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 10);
		defaultInterpolatedStringHandler.AppendLiteral("[TrainerPanel] ");
		defaultInterpolatedStringHandler.AppendFormatted(value2);
		defaultInterpolatedStringHandler.AppendLiteral("'");
		defaultInterpolatedStringHandler.AppendFormatted(((Object)t).name);
		defaultInterpolatedStringHandler.AppendLiteral("' pos=");
		defaultInterpolatedStringHandler.AppendFormatted((component != null) ? new Vector2?(component.anchoredPosition) : ((Vector2?)null));
		defaultInterpolatedStringHandler.AppendLiteral(" size=");
		defaultInterpolatedStringHandler.AppendFormatted((component != null) ? new Vector2?(component.sizeDelta) : ((Vector2?)null));
		defaultInterpolatedStringHandler.AppendLiteral(" rect=");
		float? value3;
		Rect rect;
		if (component == null)
		{
			value3 = null;
		}
		else
		{
			rect = component.rect;
					value3 = rect.width;
		}
		defaultInterpolatedStringHandler.AppendFormatted(value3);
		defaultInterpolatedStringHandler.AppendLiteral("x");
		float? value4;
		if (component == null)
		{
			value4 = null;
		}
		else
		{
			rect = component.rect;
					value4 = rect.height;
		}
		defaultInterpolatedStringHandler.AppendFormatted(value4);
		defaultInterpolatedStringHandler.AppendLiteral(" anchor=(");
		defaultInterpolatedStringHandler.AppendFormatted((component != null) ? new Vector2?(component.anchorMin) : ((Vector2?)null));
		defaultInterpolatedStringHandler.AppendLiteral("..");
		defaultInterpolatedStringHandler.AppendFormatted((component != null) ? new Vector2?(component.anchorMax) : ((Vector2?)null));
		defaultInterpolatedStringHandler.AppendLiteral(") active=");
		defaultInterpolatedStringHandler.AppendFormatted(((Component)t).gameObject.activeSelf);
		defaultInterpolatedStringHandler.AppendFormatted(text);
		MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
		for (int i = 0; i < t.childCount && i < 10; i++)
		{
			DumpSizes(t.GetChild(i), depth + 1, maxDepth);
		}
	}

	public static void Destroy()
	{
		if ((Object)(object)_panelRoot != (Object)null)
		{
			try
			{
				if (_tabButtons != null)
				{
					for (int i = 0; i < _tabButtons.Length; i++)
					{
						GameObject btn = _tabButtons[i];
						if ((Object)(object)btn != (Object)null)
						{
							Button b = btn.GetComponent<Button>();
							if ((Object)(object)b != (Object)null)
							{
								((UnityEventBase)b.onClick).RemoveAllListeners();
							}
						}
					}
				}
				Object.Destroy((Object)(object)_panelRoot);
			}
			catch
			{
			}
			_panelRoot = null;
		}
		_built = false;
		_tabContents = null;
		_tabButtons = null;
		_indicatorRect = null;
		_spriteRoundCard = null;
		_spriteWhitebox = null;
		_spriteSplitter = null;
		_spriteRoundedBox = null;
		_spriteBarBg = null;
		_spriteScrollbar = null;
		_gameFont = null;
		_assetsCached = false;
		_cardStates.Clear();
		_searchableItems.Clear();
		_searchText = "";
	}

	private static void FindGameAssets()
	{
		if (_assetsCached)
		{
			return;
		}
		MelonLogger.Msg("[TrainerPanel] Searching for game sprites...");
		Il2CppArrayBase<Image> obj = Resources.FindObjectsOfTypeAll<Image>();
		int num = 0;
		foreach (Image item in obj)
		{
			if ((Object)(object)item == (Object)null || (Object)(object)item.sprite == (Object)null)
			{
				continue;
			}
			switch (((Object)item.sprite).name)
			{
			case "white-round-corner-drop":
				if ((Object)(object)_spriteRoundCard == (Object)null)
				{
					_spriteRoundCard = item.sprite;
					num++;
				}
				break;
			case "whitebox":
				if ((Object)(object)_spriteWhitebox == (Object)null)
				{
					_spriteWhitebox = item.sprite;
					num++;
				}
				break;
			case "Square-With-Padding-8":
				if ((Object)(object)_spriteSplitter == (Object)null)
				{
					_spriteSplitter = item.sprite;
					num++;
				}
				break;
			case "white-rounded-box":
				if ((Object)(object)_spriteRoundedBox == (Object)null)
				{
					_spriteRoundedBox = item.sprite;
					num++;
				}
				break;
			case "bar_background":
				if ((Object)(object)_spriteBarBg == (Object)null)
				{
					_spriteBarBg = item.sprite;
					num++;
				}
				break;
			case "Scrollbar":
				if ((Object)(object)_spriteScrollbar == (Object)null)
				{
					_spriteScrollbar = item.sprite;
					num++;
				}
				break;
			}
		}
		MelonLogger.Msg($"[TrainerPanel] Found {num}/6 sprites: RoundCard={(((Object)(object)_spriteRoundCard != (Object)null) ? "OK" : "MISSING")} Whitebox={(((Object)(object)_spriteWhitebox != (Object)null) ? "OK" : "MISSING")} Splitter={(((Object)(object)_spriteSplitter != (Object)null) ? "OK" : "MISSING")} RoundedBox={(((Object)(object)_spriteRoundedBox != (Object)null) ? "OK" : "MISSING")} BarBg={(((Object)(object)_spriteBarBg != (Object)null) ? "OK" : "MISSING")} Scrollbar={(((Object)(object)_spriteScrollbar != (Object)null) ? "OK" : "MISSING")}");
		MelonLogger.Msg("[TrainerPanel] Searching for game font...");
		foreach (TextMeshProUGUI item2 in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>())
		{
			if ((Object)(object)item2 != (Object)null && (Object)(object)((TMP_Text)item2).font != (Object)null)
			{
				_gameFont = ((TMP_Text)item2).font;
				MelonLogger.Msg("[TrainerPanel] Game font found: '" + ((Object)_gameFont).name + "'");
				break;
			}
		}
		if ((Object)(object)_gameFont == (Object)null)
		{
			MelonLogger.Warning("[TrainerPanel] Game font NOT found! TMP text will use default font.");
		}
		_assetsCached = true;
	}

	private static void BuildTabBar(Transform parent)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateObj("Tabs", parent);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(1f, 1f);
		component.pivot = new Vector2(0.5f, 1f);
		component.anchoredPosition = Vector2.zero;
		component.sizeDelta = new Vector2(0f, 100f);
		string[] array = new string[7] { "Money", "Player", "Vehicles", "Business", "Gameplay", "Staff", "Rivals" };
		_tabButtons = (GameObject[])(object)new GameObject[7];
		float num = 548.5714f;
		for (int i = 0; i < 7; i++)
		{
			int tabIndex = i;
			GameObject val2 = CreateObj("Tab_" + array[i], val.transform);
			RectTransform component2 = val2.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0f);
			component2.anchorMax = new Vector2(0f, 1f);
			component2.pivot = new Vector2(0f, 0.5f);
			component2.anchoredPosition = new Vector2((float)i * num, 0f);
			component2.sizeDelta = new Vector2(num, 0f);
			Image val3 = val2.AddComponent<Image>();
			((Graphic)val3).color = Color.clear;
			Button obj = val2.AddComponent<Button>();
			((Selectable)obj).targetGraphic = (Graphic)(object)val3;
			ColorBlock colors = ((Selectable)obj).colors;
			colors.normalColor = Color.white;
			colors.highlightedColor = Color.white;
			colors.pressedColor = Color.white;
			colors.fadeDuration = 0f;
			((Selectable)obj).colors = colors;
			CreateTMP(val2.transform, "Label", array[i], 50, (i == 0) ? TabTextActive : TabTextInactive, (FontStyles)0, (TextAlignmentOptions)514);
			((UnityEvent)obj.onClick).AddListener((UnityAction)delegate
			{
				SwitchTab(tabIndex);
			});
			_tabButtons[i] = val2;
		}
	}

	private static void BuildSplitterWithIndicator(Transform parent)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateObj("SplitterWithIndicator", parent);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(1f, 1f);
		component.pivot = new Vector2(0.5f, 1f);
		component.anchoredPosition = new Vector2(0f, -100f);
		component.sizeDelta = new Vector2(0f, 8f);
		Image val2 = val.AddComponent<Image>();
		if ((Object)(object)_spriteSplitter != (Object)null)
		{
			val2.sprite = _spriteSplitter;
			val2.type = (Image.Type)1;
		}
		((Graphic)val2).color = SplitterColor;
		GameObject obj = CreateObj("Indicator", val.transform);
		RectTransform component2 = obj.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 0f);
		component2.anchorMax = new Vector2(0f, 1f);
		component2.pivot = new Vector2(0f, 0.5f);
		component2.sizeDelta = new Vector2(0f, 0f);
		Image val3 = obj.AddComponent<Image>();
		if ((Object)(object)_spriteSplitter != (Object)null)
		{
			val3.sprite = _spriteSplitter;
			val3.type = (Image.Type)1;
		}
		((Graphic)val3).color = Color.white;
		_indicatorRect = component2;
	}

	private static void BuildSearchBar(Transform parent)
	{
		GameObject val = CreateObj("SearchBar", parent);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(100f, -108f);
		component.sizeDelta = new Vector2(3610f, 80f);
		Image val2 = val.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val2.sprite = _spriteRoundedBox;
			val2.type = (Image.Type)1;
		}
		((Graphic)val2).color = CardWhite;
		CreateTMP(val.transform, "Label", "Search:", 32, CardTextDark, (FontStyles)0, (TextAlignmentOptions)513).GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, 0f);
		GameObject val3 = CreateObj("Input", val.transform);
		RectTransform component2 = val3.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 0.5f);
		component2.anchorMax = new Vector2(0f, 0.5f);
		component2.pivot = new Vector2(0f, 0.5f);
		component2.anchoredPosition = new Vector2(250f, 0f);
		component2.sizeDelta = new Vector2(3310f, 56f);
		Image val4 = val3.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val4.sprite = _spriteRoundedBox;
			val4.type = (Image.Type)1;
		}
		((Graphic)val4).color = InputBg;
		Il2CppTMPro.TMP_InputField val5 = val3.AddComponent<Il2CppTMPro.TMP_InputField>();
		GameObject val6 = new GameObject("Text");
		val6.transform.SetParent(val3.transform, false);
		RectTransform obj = val6.AddComponent<RectTransform>();
		obj.anchorMin = Vector2.zero;
		obj.anchorMax = Vector2.one;
		obj.offsetMin = new Vector2(16f, 4f);
		obj.offsetMax = new Vector2(-16f, -4f);
		Il2CppTMPro.TextMeshProUGUI val7 = val6.AddComponent<Il2CppTMPro.TextMeshProUGUI>();
		((TMP_Text)val7).fontSize = 28;
		((Graphic)val7).color = InputText;
		((TMP_Text)val7).fontStyle = (FontStyles)0;
		((TMP_Text)val7).alignment = (TextAlignmentOptions)513;
		if ((Object)(object)_gameFont != (Object)null)
		{
			((TMP_Text)val7).font = _gameFont;
		}
		GameObject val8 = new GameObject("Placeholder");
		val8.transform.SetParent(val3.transform, false);
		RectTransform obj2 = val8.AddComponent<RectTransform>();
		obj2.anchorMin = Vector2.zero;
		obj2.anchorMax = Vector2.one;
		obj2.offsetMin = new Vector2(16f, 4f);
		obj2.offsetMax = new Vector2(-16f, -4f);
		Il2CppTMPro.TextMeshProUGUI val9 = val8.AddComponent<Il2CppTMPro.TextMeshProUGUI>();
		((TMP_Text)val9).text = "Type to filter buttons...";
		((TMP_Text)val9).fontSize = 28;
		((Graphic)val9).color = InputPlaceholder;
		((TMP_Text)val9).fontStyle = (FontStyles)2;
		((TMP_Text)val9).alignment = (TextAlignmentOptions)513;
		if ((Object)(object)_gameFont != (Object)null)
		{
			((TMP_Text)val9).font = _gameFont;
		}
		val5.textComponent = val7;
		val5.placeholder = (Graphic)(object)val9;
		val5.text = "";
		val5.onValueChanged.AddListener((UnityAction<string>)delegate(string text)
		{
			_searchText = text ?? "";
			ApplySearchFilter();
		});
		MelonLogger.Msg("[TrainerPanel] Search bar built.");
	}

	private static void UpdateIndicatorPosition(int tabIndex)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_indicatorRect == (Object)null))
		{
			float num = 1f / 7f;
			float num2 = (float)tabIndex * num;
			float num3 = (float)(tabIndex + 1) * num;
			_indicatorRect.anchorMin = new Vector2(num2, 0f);
			_indicatorRect.anchorMax = new Vector2(num3, 1f);
			_indicatorRect.offsetMin = Vector2.zero;
			_indicatorRect.offsetMax = Vector2.zero;
		}
	}

	private static void ToggleCard(int index)
	{
		if (index < 0 || index >= _cardStates.Count)
		{
			return;
		}
		var value = _cardStates[index];
		bool collapsed = !value.collapsed;
		float oldHeight = collapsed ? value.originalHeight : 140f;
		float newHeight = collapsed ? 140f : value.originalHeight;
		float deltaY = oldHeight - newHeight;
		for (int i = 0; i < value.card.transform.childCount; i++)
		{
			Transform child = value.card.transform.GetChild(i);
			if ((Object)(object)child != (Object)null && (Object)(object)((Component)child).gameObject != (Object)(object)value.header)
			{
				((Component)child).gameObject.SetActive(!collapsed);
			}
		}
		RectTransform cardRT = value.card.GetComponent<RectTransform>();
		Vector2 size = cardRT.sizeDelta;
		size.y = newHeight;
		cardRT.sizeDelta = size;
		float cardY = cardRT.anchoredPosition.y;
		Transform contentParent = value.card.transform.parent;
		if ((Object)(object)contentParent != (Object)null)
		{
			for (int j = 0; j < contentParent.childCount; j++)
			{
				Transform sibling = contentParent.GetChild(j);
				if ((Object)(object)sibling != (Object)null && (Object)(object)((Component)sibling).gameObject != (Object)(object)value.card)
				{
					RectTransform siblingRT = ((Component)sibling).GetComponent<RectTransform>();
					if ((Object)(object)siblingRT != (Object)null && siblingRT.anchoredPosition.y < cardY)
					{
						Vector2 pos = siblingRT.anchoredPosition;
						pos.y += deltaY;
						siblingRT.anchoredPosition = pos;
					}
				}
			}
			RectTransform contentRT = ((Component)contentParent).GetComponent<RectTransform>();
			if ((Object)(object)contentRT != (Object)null)
			{
				Vector2 contentSize = contentRT.sizeDelta;
				contentSize.y -= deltaY;
				contentRT.sizeDelta = contentSize;
			}
		}
		Transform headerTransform = value.header.transform;
		Transform val = headerTransform.Find("CollapseArrow");
		if ((Object)(object)val != (Object)null)
		{
			Il2CppTMPro.TextMeshProUGUI componentInChildren = val.GetComponentInChildren<Il2CppTMPro.TextMeshProUGUI>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				((TMP_Text)componentInChildren).text = (collapsed ? "\u25b6" : "\u25bc");
			}
		}
		_cardStates[index] = (value.name, value.card, value.header, collapsed, value.originalHeight);
	}

	private static void ApplySearchFilter()
	{
		bool flag = !string.IsNullOrEmpty(_searchText);
		foreach (var item in _searchableItems)
		{
			if ((Object)(object)item.go != (Object)null && item.tabIndex == _activeTab)
			{
				bool active = !flag || item.label.IndexOf(_searchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
				if (item.go.activeSelf != active)
				{
					item.go.SetActive(active);
				}
			}
		}
	}

	private static void SwitchTab(int index)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (_tabContents == null || _tabButtons == null)
		{
			return;
		}
		_activeTab = index;
		for (int i = 0; i < 7; i++)
		{
			if ((Object)(object)_tabContents[i] != (Object)null)
			{
				_tabContents[i].SetActive(i == index);
			}
			if ((Object)(object)_tabButtons[i] != (Object)null)
			{
				TextMeshProUGUI componentInChildren = _tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();
				if ((Object)(object)componentInChildren != (Object)null)
				{
					((Graphic)componentInChildren).color = ((i == index) ? TabTextActive : TabTextInactive);
				}
			}
		}
		UpdateIndicatorPosition(index);
		ApplySearchFilter();
	}

	private static GameObject CreateScrollView(Transform parent, string name)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateObj("Scroll_" + name, parent);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		ScrollRect val2 = val.AddComponent<ScrollRect>();
		val2.horizontal = false;
		val2.vertical = true;
		val2.movementType = (ScrollRect.MovementType)1;
		val2.scrollSensitivity = 40f;
		((Graphic)val.AddComponent<Image>()).color = Color.clear;
		GameObject val3 = CreateObj("Viewport", val.transform);
		RectTransform component2 = val3.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.offsetMin = Vector2.zero;
		component2.offsetMax = Vector2.zero;
		((Graphic)val3.AddComponent<Image>()).color = Color.white;
		val3.AddComponent<Mask>().showMaskGraphic = false;
		RectTransform component3 = CreateObj("Content", val3.transform).GetComponent<RectTransform>();
		component3.anchorMin = new Vector2(0f, 1f);
		component3.anchorMax = new Vector2(1f, 1f);
		component3.pivot = new Vector2(0.5f, 1f);
		component3.anchoredPosition = Vector2.zero;
		component3.sizeDelta = new Vector2(0f, 0f);
		val2.viewport = component2;
		val2.content = component3;
		try
		{
			GameObject val4 = CreateObj("Scrollbar", val.transform);
			RectTransform component4 = val4.GetComponent<RectTransform>();
			component4.anchorMin = new Vector2(1f, 0f);
			component4.anchorMax = new Vector2(1f, 1f);
			component4.pivot = new Vector2(1f, 0.5f);
			component4.sizeDelta = new Vector2(16f, 0f);
			Image val5 = val4.AddComponent<Image>();
			if ((Object)(object)_spriteScrollbar != (Object)null)
			{
				val5.sprite = _spriteScrollbar;
				val5.type = (Image.Type)1;
			}
			((Graphic)val5).color = new Color(1f, 1f, 1f, 0.05f);
			GameObject val6 = CreateObj("Sliding Area", val4.transform);
			RectTransform component5 = val6.GetComponent<RectTransform>();
			component5.anchorMin = Vector2.zero;
			component5.anchorMax = Vector2.one;
			component5.offsetMin = Vector2.zero;
			component5.offsetMax = Vector2.zero;
			GameObject obj = CreateObj("Handle", val6.transform);
			RectTransform component6 = obj.GetComponent<RectTransform>();
			component6.anchorMin = Vector2.zero;
			component6.anchorMax = Vector2.one;
			component6.offsetMin = Vector2.zero;
			component6.offsetMax = Vector2.zero;
			Image val7 = obj.AddComponent<Image>();
			if ((Object)(object)_spriteScrollbar != (Object)null)
			{
				val7.sprite = _spriteScrollbar;
				val7.type = (Image.Type)1;
			}
			((Graphic)val7).color = new Color(1f, 1f, 1f, 0.3f);
			Scrollbar val8 = val4.AddComponent<Scrollbar>();
			val8.handleRect = component6;
			val8.direction = (Scrollbar.Direction)2;
			((Selectable)val8).targetGraphic = (Graphic)(object)val7;
			val2.verticalScrollbar = val8;
			val2.verticalScrollbarVisibility = (ScrollRect.ScrollbarVisibility)2;
			val2.verticalScrollbarSpacing = -2f;
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[TrainerPanel] Scrollbar creation failed: " + ex.Message);
		}
		return val;
	}

	private static float BuildMoneyTab(Transform content)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 0f;
		num2 += 80f;
		num2 += PlaceButtonRow(null, 0f, 0f, 4);
		num2 += 20f;
		num2 += PlaceButtonRow(null, 0f, 0f, 3);
		float num3 = 40f + num2 + 40f;
		Transform card = CreateCard(content, "Card_QuickAddMoney", 100f, num, 3610f, num3);
		float cy = 40f;
		PlaceCardHeader(card, "Quick Add Money", ref cy);
		Transform row = PlaceRow(card, ref cy, 4);
		ActionBtn(row, "$1K", BtnSuccess, delegate
		{
			MoneyModule.AddMoney(1000f);
			Toast("Added $1,000");
		}, 0, 4);
		ActionBtn(row, "$5K", BtnSuccess, delegate
		{
			MoneyModule.AddMoney(5000f);
			Toast("Added $5,000");
		}, 1, 4);
		ActionBtn(row, "$10K", BtnSuccess, delegate
		{
			MoneyModule.AddMoney(10000f);
			Toast("Added $10,000");
		}, 2, 4);
		ActionBtn(row, "$50K", BtnSuccess, delegate
		{
			MoneyModule.AddMoney(50000f);
			Toast("Added $50,000");
		}, 3, 4);
		Transform row2 = PlaceRow(card, ref cy, 3);
		ActionBtn(row2, "$100K", BtnBlue, delegate
		{
			MoneyModule.AddMoney(100000f);
			Toast("Added $100,000");
		}, 0, 3);
		ActionBtn(row2, "$500K", BtnBlue, delegate
		{
			MoneyModule.AddMoney(500000f);
			Toast("Added $500,000");
		}, 1, 3);
		ActionBtn(row2, "$1M", BtnWarning, delegate
		{
			MoneyModule.AddMoney(1000000f);
			Toast("Added $1,000,000");
		}, 2, 3);
		num += num3 + 40f;
		float num4 = 340f;
		Transform card2 = CreateCard(content, "Card_CustomMoney", 100f, num, 3610f, num4);
		cy = 40f;
		PlaceCardHeader(card2, "Custom Money", ref cy);
		PlaceInputWithButton(card2, ref cy, "Amount to Add:", "e.g. 25000", "Add", BtnBlue, (InputField.ContentType)3, delegate(string inputText)
		{
			if (float.TryParse(inputText, out var result))
			{
				MoneyModule.AddMoney(result);
				Toast($"Added ${result:N0}");
			}
			else
			{
				Toast("Invalid amount");
			}
		});
		PlaceInputWithButton(card2, ref cy, "Set Money To:", "e.g. 100000", "Set", BtnWarning, (InputField.ContentType)3, delegate(string inputText)
		{
			if (float.TryParse(inputText, out var result))
			{
				MoneyModule.SetMoney(result);
				Toast($"Money set to ${result:N0}");
			}
			else
			{
				Toast("Invalid amount");
			}
		});
		num += num4 + 40f;
		float num5 = 560f;
		Transform card3 = CreateCard(content, "Card_EconomySettings", 100f, num, 3610f, num5);
		cy = 40f;
		PlaceCardHeader(card3, "Economy Settings", ref cy);
		PlaceSlider(card3, ref cy, "Tax Percentage", 0f, 100f, MoneyModule.TaxPercentage, wholeNumbers: true, onChanged: (val) => MoneyModule.ApplyTaxPercentage((int)val), onReleased: (val) => Toast($"Tax set to {(int)val}%"));
		PlaceSlider(card3, ref cy, "Market Price Multiplier", 0.1f, 5f, MoneyModule.MarketPriceMultiplier, wholeNumbers: false, onChanged: (val) => MoneyModule.ApplyMarketPriceMultiplier(val), onReleased: (val) => Toast($"Market price: {val:F1}x"));
		PlaceSlider(card3, ref cy, "Export Multiplier", 0.1f, 10f, MoneyModule.ExportMultiplier, wholeNumbers: false, onChanged: (val) => MoneyModule.ApplyExportMultiplier(val), onReleased: (val) => Toast($"Export: {val:F1}x"));
		return num + (num5 + 40f);
	}

	private static float BuildPlayerTab(Transform content)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 240f;
		Transform card = CreateCard(content, "Card_Needs", 100f, num, 3610f, num2);
		float cy = 40f;
		PlaceCardHeader(card, "Needs", ref cy);
		ActionBtn(PlaceRow(card, ref cy, 1), "Fill All Needs", BtnSuccess, delegate
		{
			PlayerStatsModule.FillAllNeeds();
			Toast("All needs filled!");
		}, 0, 1);
		num += num2 + 40f;
		float num3 = 450f;
		Transform card2 = CreateCard(content, "Card_Energy", 100f, num, 3610f, num3);
		cy = 40f;
		PlaceCardHeader(card2, "Energy & Stats", ref cy);
		PlaceSectionLabel(card2, "Energy", ref cy);
		PlaceSlider(card2, ref cy, "Energy Level", 0f, 100f, 100f, wholeNumbers: true, onChanged: (val) => PlayerStatsModule.SetEnergy(val), onReleased: (val) => Toast($"Energy: {(int)val}"));
		Transform row = PlaceRow(card2, ref cy, 4);
		ActionBtn(row, "25", BtnNeutral, delegate
		{
			PlayerStatsModule.SetEnergy(25f);
			Toast("Energy: 25");
		}, 0, 4);
		ActionBtn(row, "50", BtnNeutral, delegate
		{
			PlayerStatsModule.SetEnergy(50f);
			Toast("Energy: 50");
		}, 1, 4);
		ActionBtn(row, "75", BtnBlue, delegate
		{
			PlayerStatsModule.SetEnergy(75f);
			Toast("Energy: 75");
		}, 2, 4);
		ActionBtn(row, "100", BtnSuccess, delegate
		{
			PlayerStatsModule.SetEnergy(100f);
			Toast("Energy: 100");
		}, 3, 4);
		num += num3 + 40f;
		float num4 = 240f;
		Transform card3 = CreateCard(content, "Card_Happiness", 100f, num, 3610f, num4);
		cy = 40f;
		PlaceCardHeader(card3, "Happiness", ref cy);
		Transform row2 = PlaceRow(card3, ref cy, 4);
		ActionBtn(row2, "-25", BtnDanger, delegate
		{
			PlayerStatsModule.ChangeHappiness(-25);
			Toast("Happiness -25");
		}, 0, 4);
		ActionBtn(row2, "-10", BtnDanger, delegate
		{
			PlayerStatsModule.ChangeHappiness(-10);
			Toast("Happiness -10");
		}, 1, 4);
		ActionBtn(row2, "+10", BtnSuccess, delegate
		{
			PlayerStatsModule.ChangeHappiness(10);
			Toast("Happiness +10");
		}, 2, 4);
		ActionBtn(row2, "+25", BtnSuccess, delegate
		{
			PlayerStatsModule.ChangeHappiness(25);
			Toast("Happiness +25");
		}, 3, 4);
		num += num4 + 40f;
		float num5 = 240f;
		Transform card4 = CreateCard(content, "Card_Hunger", 100f, num, 3610f, num5);
		cy = 40f;
		PlaceCardHeader(card4, "Hunger", ref cy);
		Transform row3 = PlaceRow(card4, ref cy, 4);
		ActionBtn(row3, "-25", BtnDanger, delegate
		{
			PlayerStatsModule.ChangeHunger(-25);
			Toast("Hunger -25");
		}, 0, 4);
		ActionBtn(row3, "-10", BtnDanger, delegate
		{
			PlayerStatsModule.ChangeHunger(-10);
			Toast("Hunger -10");
		}, 1, 4);
		ActionBtn(row3, "+10", BtnSuccess, delegate
		{
			PlayerStatsModule.ChangeHunger(10);
			Toast("Hunger +10");
		}, 2, 4);
		ActionBtn(row3, "+25", BtnSuccess, delegate
		{
			PlayerStatsModule.ChangeHunger(25);
			Toast("Hunger +25");
		}, 3, 4);
		num += num5 + 40f;
		float num6 = 240f;
		Transform card5 = CreateCard(content, "Card_Speed", 100f, num, 3610f, num6);
		cy = 40f;
		PlaceCardHeader(card5, "Movement Speed", ref cy);
		Transform row4 = PlaceRow(card5, ref cy, 4);
		ActionBtn(row4, "Walk", BtnNeutral, delegate
		{
			PlayerStatsModule.SetPlayerSpeed(0);
			Toast("Speed: Walk");
		}, 0, 4);
		ActionBtn(row4, "Jog", BtnNeutral, delegate
		{
			PlayerStatsModule.SetPlayerSpeed(1);
			Toast("Speed: Jog");
		}, 1, 4);
		ActionBtn(row4, "Run", BtnBlue, delegate
		{
			PlayerStatsModule.SetPlayerSpeed(2);
			Toast("Speed: Run");
		}, 2, 4);
		ActionBtn(row4, "Scooter", BtnWarning, delegate
		{
			PlayerStatsModule.SetPlayerSpeed(3);
			Toast("Speed: Scooter");
		}, 3, 4);
		num += num6 + 40f;
		float num7 = 540f;
		Transform card6 = CreateCard(content, "Card_Toggles", 100f, num, 3610f, num7);
		cy = 40f;
		PlaceCardHeader(card6, "Decay Toggles", ref cy);
		PlaceToggle(card6, ref cy, "Disable Energy Decay", TrainerConfig.DisableEnergy, delegate(bool v)
		{
			PlayerStatsModule.ToggleDisableEnergy(v);
		});
		PlaceToggle(card6, ref cy, "Disable Happiness Decay", TrainerConfig.DisableHappiness, delegate(bool v)
		{
			PlayerStatsModule.ToggleDisableHappiness(v);
		});
		PlaceToggle(card6, ref cy, "Disable Hunger Decay", TrainerConfig.DisableHunger, delegate(bool v)
		{
			PlayerStatsModule.ToggleDisableHunger(v);
		});
		PlaceToggle(card6, ref cy, "Disable Aging", TrainerConfig.DisableAging, delegate(bool v)
		{
			PlayerStatsModule.ToggleDisableAging(v);
		});
		num += num7 + 40f;
		float num8 = 240f;
		Transform card7 = CreateCard(content, "Card_Age", 100f, num, 3610f, num8);
		cy = 40f;
		PlaceCardHeader(card7, "Age", ref cy);
		Transform row5 = PlaceRow(card7, ref cy, 4);
		ActionBtn(row5, "-5 Years", BtnDanger, delegate
		{
			PlayerStatsModule.ChangeAge(-5f);
			Toast("Age -5");
		}, 0, 4);
		ActionBtn(row5, "-1 Year", BtnDanger, delegate
		{
			PlayerStatsModule.ChangeAge(-1f);
			Toast("Age -1");
		}, 1, 4);
		ActionBtn(row5, "+1 Year", BtnNeutral, delegate
		{
			PlayerStatsModule.ChangeAge(1f);
			Toast("Age +1");
		}, 2, 4);
		ActionBtn(row5, "+5 Years", BtnNeutral, delegate
		{
			PlayerStatsModule.ChangeAge(5f);
			Toast("Age +5");
		}, 3, 4);
		num += num8 + 40f;
		float num9 = 240f;
		Transform card8 = CreateCard(content, "Card_Goals", 100f, num, 3610f, num9);
		cy = 40f;
		PlaceCardHeader(card8, "Goals", ref cy);
		ActionBtn(PlaceRow(card8, ref cy, 1), "Complete All Personal Goals", BtnSuccess, delegate
		{
			PlayerStatsModule.CompletePersonalGoals();
			Toast("Goals completed!");
		}, 0, 1);
		return num + (num9 + 40f);
	}

	private static float BuildVehicleTab(Transform content)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 340f;
		Transform card = CreateCard(content, "Card_VehicleToggles", 100f, num, 3610f, num2);
		float cy = 40f;
		PlaceCardHeader(card, "Vehicle Toggles", ref cy);
		PlaceToggle(card, ref cy, "Disable Vehicle Damage", TrainerConfig.DisableVehicleDamage, delegate(bool v)
		{
			VehicleModule.ToggleVehicleDamage(v);
		});
		PlaceToggle(card, ref cy, "Disable Vehicle Fuel", TrainerConfig.DisableVehicleFuel, delegate(bool v)
		{
			VehicleModule.ToggleVehicleFuel(v);
		});
		num += num2 + 40f;
		float num3 = 340f;
		Transform card2 = CreateCard(content, "Card_VehicleActions", 100f, num, 3610f, num3);
		cy = 40f;
		PlaceCardHeader(card2, "Vehicle Actions", ref cy);
		Transform row = PlaceRow(card2, ref cy, 2);
		ActionBtn(row, "Repair Vehicle", BtnSuccess, delegate
		{
			VehicleModule.RepairVehicle();
			Toast("Vehicle repaired!");
		}, 0, 2);
		ActionBtn(row, "Refuel Vehicle", BtnBlue, delegate
		{
			VehicleModule.RefuelVehicle();
			Toast("Vehicle refueled!");
		}, 1, 2);
		Transform row2 = PlaceRow(card2, ref cy, 2);
		ActionBtn(row2, "Clean Vehicle", BtnNeutral, delegate
		{
			VehicleModule.CleanVehicle();
			Toast("Vehicle cleaned!");
		}, 0, 2);
		ActionBtn(row2, "Clear Parking Tickets", BtnWarning, delegate
		{
			VehicleModule.ClearParkingTickets();
			Toast("Tickets cleared!");
		}, 1, 2);
		num += num3 + 40f;
		float num4 = 240f;
		Transform card3 = CreateCard(content, "Card_Towing", 100f, num, 3610f, num4);
		cy = 40f;
		PlaceCardHeader(card3, "Towing Services", ref cy);
		Transform row3 = PlaceRow(card3, ref cy, 2);
		ActionBtn(row3, "Tow to Gas Station", BtnBlue, delegate
		{
			VehicleModule.TowToGasStation();
			Toast("Towing to gas station...");
		}, 0, 2);
		ActionBtn(row3, "Tow to Auto Repair", BtnWarning, delegate
		{
			VehicleModule.TowToAutoRepair();
			Toast("Towing to auto repair...");
		}, 1, 2);
		return num + (num4 + 40f);
	}

	private static float BuildBusinessTab(Transform content)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 240f;
		Transform card = CreateCard(content, "Card_Satisfaction", 100f, num, 3610f, num2);
		float cy = 40f;
		PlaceCardHeader(card, "Customer Satisfaction", ref cy);
		ActionBtn(PlaceRow(card, ref cy, 1), "Max All Customer Satisfaction", BtnSuccess, delegate
		{
			BusinessModule.MaxAllSatisfaction();
			Toast("All satisfaction maxed!");
		}, 0, 1);
		num += num2 + 40f;
		float num3 = 540f;
		Transform card2 = CreateCard(content, "Card_Unlocks", 100f, num, 3610f, num3);
		cy = 40f;
		PlaceCardHeader(card2, "Unlocks & Toggles", ref cy);
		PlaceToggle(card2, ref cy, "Unlock All Courses", TrainerConfig.AllCoursesUnlocked, delegate(bool v)
		{
			BusinessModule.ToggleAllCourses(v);
		});
		PlaceToggle(card2, ref cy, "Unlock All Contacts", TrainerConfig.AllContactsUnlocked, delegate(bool v)
		{
			BusinessModule.ToggleAllContacts(v);
		});
		PlaceToggle(card2, ref cy, "Disable Wholesale/Import Limits", TrainerConfig.DisableWholesaleImportLimits, delegate(bool v)
		{
			BusinessModule.ToggleWholesaleImportLimits(v);
		});
		PlaceToggle(card2, ref cy, "All Products From Importers", TrainerConfig.AllProductsFromImporters, delegate(bool v)
		{
			BusinessModule.ToggleAllProductsFromImporters(v);
		});
		num += num3 + 40f;
		float num4 = 100f;
		for (int num5 = 0; num5 < 6; num5++)
		{
			num4 += 140f;
		}
		num4 += 40f;
		Transform card3 = CreateCard(content, "Card_Multipliers", 100f, num, 3610f, num4);
		cy = 40f;
		PlaceCardHeader(card3, "Business Multipliers", ref cy);
		PlaceSlider(card3, ref cy, "Customer Promotion Mult", 0.1f, 10f, 1f, wholeNumbers: false, onChanged: (val) => BusinessModule.ApplyCustomerPromotionMultiplier(val), onReleased: (val) => Toast($"Promotion: {val:F1}x"));
		PlaceSlider(card3, ref cy, "Employee Salary Mult", 0f, 5f, 1f, wholeNumbers: false, onChanged: (val) => BusinessModule.ApplyEmployeeSalaryMultiplier(val), onReleased: (val) => Toast($"Salary: {val:F1}x"));
		PlaceSlider(card3, ref cy, "Wholesale Urgent Fee Mult", 0f, 5f, 1f, wholeNumbers: false, onChanged: (val) => BusinessModule.ApplyWholesaleUrgentFeeMultiplier(val), onReleased: (val) => Toast($"Wholesale fee: {val:F1}x"));
		PlaceSlider(card3, ref cy, "Importer Urgent Fee Mult", 0f, 5f, 1f, wholeNumbers: false, onChanged: (val) => BusinessModule.ApplyImporterUrgentFeeMultiplier(val), onReleased: (val) => Toast($"Importer fee: {val:F1}x"));
		PlaceSlider(card3, ref cy, "Bank Interest Rate", 0f, 50f, 5f, wholeNumbers: true, onChanged: (val) => BusinessModule.ApplyBankInterestRate(val), onReleased: (val) => Toast($"Interest: {(int)val}%"));
		PlaceSlider(card3, ref cy, "Rivals Difficulty Mult", 0f, 5f, 1f, wholeNumbers: false, onChanged: (val) => BusinessModule.ApplyRivalsDifficultyMultiplier(val), onReleased: (val) => Toast($"Rivals: {val:F1}x"));
		return num + (num4 + 40f);
	}

	private static float BuildGameplayTab(Transform content)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 380f;
		Transform card = CreateCard(content, "Card_GameSpeed", 100f, num, 3610f, num2);
		float cy = 40f;
		PlaceCardHeader(card, "Game Speed", ref cy);
		PlaceSlider(card, ref cy, "Speed Multiplier", 0f, 10f, GameplayModule.GameSpeed, wholeNumbers: false, onChanged: (val) => GameplayModule.SetGameSpeed(val), onReleased: (val) => Toast($"Speed: {val:F1}x"));
		Transform row = PlaceRow(card, ref cy, 5);
		ActionBtn(row, "Pause", BtnDanger, delegate
		{
			GameplayModule.SetGameSpeed(0f);
			Toast("Game paused");
		}, 0, 5);
		ActionBtn(row, "1x", BtnNeutral, delegate
		{
			GameplayModule.SetGameSpeed(1f);
			Toast("Speed: 1x");
		}, 1, 5);
		ActionBtn(row, "2x", BtnBlue, delegate
		{
			GameplayModule.SetGameSpeed(2f);
			Toast("Speed: 2x");
		}, 2, 5);
		ActionBtn(row, "5x", BtnWarning, delegate
		{
			GameplayModule.SetGameSpeed(5f);
			Toast("Speed: 5x");
		}, 3, 5);
		ActionBtn(row, "10x", BtnDanger, delegate
		{
			GameplayModule.SetGameSpeed(10f);
			Toast("Speed: 10x");
		}, 4, 5);
		num += num2 + 40f;
		float num3 = 580f;
		Transform card2 = CreateCard(content, "Card_TimeControls", 100f, num, 3610f, num3);
		cy = 40f;
		PlaceCardHeader(card2, "Time Controls", ref cy);
		ActionBtn(PlaceRow(card2, ref cy, 1), "Skip to Next Day", BtnWarning, delegate
		{
			GameplayModule.SkipToNextDay();
			Toast("Skipped to next day");
		}, 0, 1);
		PlaceSectionLabel(card2, "Preset Times", ref cy);
		Transform row2 = PlaceRow(card2, ref cy, 4);
		ActionBtn(row2, "6 AM", BtnNeutral, delegate
		{
			GameplayModule.SetTimeOfDay(6, 0);
		}, 0, 4);
		ActionBtn(row2, "12 PM", BtnNeutral, delegate
		{
			GameplayModule.SetTimeOfDay(12, 0);
		}, 1, 4);
		ActionBtn(row2, "6 PM", BtnNeutral, delegate
		{
			GameplayModule.SetTimeOfDay(18, 0);
		}, 2, 4);
		ActionBtn(row2, "10 PM", BtnNeutral, delegate
		{
			GameplayModule.SetTimeOfDay(22, 0);
		}, 3, 4);
		PlaceSectionLabel(card2, "Set Custom Time", ref cy);
		PlaceDualInputWithButton(card2, ref cy, "Hour:", "0-23", "Min:", "0-59", "Set Time", BtnBlue, delegate(string hourText, string minText)
		{
			if (int.TryParse(hourText, out var result) && int.TryParse(minText, out var result2) && result >= 0 && result <= 23 && result2 >= 0 && result2 <= 59)
			{
				GameplayModule.SetTimeOfDay(result, result2);
				Toast($"Time set to {result:D2}:{result2:D2}");
			}
			else
			{
				Toast("Invalid time (hour 0-23, min 0-59)");
			}
		});
		num += num3 + 40f;
		float num4 = 440f;
		Transform card3 = CreateCard(content, "Card_GameplayToggles", 100f, num, 3610f, num4);
		cy = 40f;
		PlaceCardHeader(card3, "Gameplay Toggles", ref cy);
		PlaceToggle(card3, ref cy, "Disable Traffic", TrainerConfig.DisableTraffic, delegate(bool v)
		{
			GameplayModule.ToggleTraffic(!v);
		});
		PlaceToggle(card3, ref cy, "Disable Tutorial", TrainerConfig.DisableTutorial, delegate(bool v)
		{
			GameplayModule.ToggleTutorial(!v);
		});
		PlaceToggle(card3, ref cy, "Invincibility", TrainerConfig.Invincibility, delegate(bool v)
		{
			GameplayModule.ToggleInvincibility(v);
		});
		num += num4 + 40f;
		float num5 = 340f;
		Transform card4 = CreateCard(content, "Card_Quests", 100f, num, 3610f, num5);
		cy = 40f;
		PlaceCardHeader(card4, "Quests & Contacts", ref cy);
		Transform row3 = PlaceRow(card4, ref cy, 2);
		ActionBtn(row3, "Complete Quest", BtnSuccess, delegate
		{
			GameplayModule.CompleteQuest();
			Toast("Quest completed!");
		}, 0, 2);
		ActionBtn(row3, "Complete Objective", BtnBlue, delegate
		{
			GameplayModule.CompleteObjective();
			Toast("Objective completed!");
		}, 1, 2);
		ActionBtn(PlaceRow(card4, ref cy, 1), "Unlock All Contacts", BtnBlue, delegate
		{
			GameplayModule.UnlockAllContacts();
			Toast("Contacts unlocked!");
		}, 0, 1);
		num += num5 + 40f;
		float num6 = 240f;
		Transform card5 = CreateCard(content, "Card_Teleport", 100f, num, 3610f, num6);
		cy = 40f;
		PlaceCardHeader(card5, "Teleportation", ref cy);
		Transform row4 = PlaceRow(card5, ref cy, 2);
		ActionBtn(row4, "Teleport to Quest Target", BtnBlue, delegate
		{
			GameplayModule.TeleportToQuestTarget();
		}, 0, 2);
		ActionBtn(row4, "Teleport to Destination", BtnNeutral, delegate
		{
			GameplayModule.TeleportToDestination();
		}, 1, 2);
		num += num6 + 40f;
		float num7 = 304f;
		Transform card6 = CreateCard(content, "Card_Imports", 100f, num, 3610f, num7);
		cy = 40f;
		PlaceCardHeader(card6, "Import Deliveries", ref cy);
		PlaceHelpText(card6, "Force all pending import deliveries to arrive now.", ref cy);
		Transform row5 = PlaceRow(card6, ref cy, 2);
		ActionBtn(row5, "Deliver All (Paid)", BtnBlue, delegate
		{
			GameplayModule.DeliverAllImportsPaid();
			Toast("Imports delivered (paid)");
		}, 0, 2);
		ActionBtn(row5, "Deliver All (Free)", BtnSuccess, delegate
		{
			GameplayModule.DeliverAllImportsFree();
			Toast("Imports delivered (free)");
		}, 1, 2);
		num += num7 + 40f;
		float num8 = 280f;
		Transform card7 = CreateCard(content, "Card_BankInterest", 100f, num, 3610f, num8);
		cy = 40f;
		PlaceCardHeader(card7, "Bank Interest Multiplier", ref cy);
		PlaceSlider(card7, ref cy, "Interest Multiplier", 0f, 5f, WorldModule.BankInterestMultiplier, wholeNumbers: false, onChanged: (val) => WorldModule.ApplyBankInterestMultiplier(val), onReleased: (val) => Toast($"Interest mult: {val:F1}x"));
		num += num8 + 40f;
		float num9 = 240f;
		Transform card8 = CreateCard(content, "Card_SaveGame", 100f, num, 3610f, num9);
		cy = 40f;
		PlaceCardHeader(card8, "Save Game", ref cy);
		ActionBtn(PlaceRow(card8, ref cy, 1), "Save Game (TrainerSave)", BtnSuccess, delegate
		{
			GameplayModule.SaveGame();
			Toast("Game saved!");
		}, 0, 1);
		return num + (num9 + 40f);
	}

	private static float BuildEmployeeTab(Transform content)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 240f;
		Transform card = CreateCard(content, "Card_BulkActions", 100f, num, 3610f, num2);
		float cy = 40f;
		PlaceCardHeader(card, "Bulk Actions", ref cy);
		ActionBtn(PlaceRow(card, ref cy, 1), "Max ALL Employee Satisfaction", BtnSuccess, delegate
		{
			EmployeeModule.MaxAllSatisfaction();
			Toast("All satisfaction maxed!");
		}, 0, 1);
		num += num2 + 40f;
		float num3 = 380f;
		Transform card2 = CreateCard(content, "Card_Salary", 100f, num, 3610f, num3);
		cy = 40f;
		PlaceCardHeader(card2, "Salary Multiplier", ref cy);
		PlaceSlider(card2, ref cy, "Salary Multiplier", 0f, 5f, EmployeeModule.SalaryMultiplier, wholeNumbers: false, onChanged: (val) => EmployeeModule.ApplySalaryMultiplier(val), onReleased: (val) => Toast($"Salary: {val:F1}x"));
		Transform row = PlaceRow(card2, ref cy, 4);
		ActionBtn(row, "Free", BtnSuccess, delegate
		{
			EmployeeModule.ApplySalaryMultiplier(0f);
			Toast("Salary: Free");
		}, 0, 4);
		ActionBtn(row, "0.5x", BtnNeutral, delegate
		{
			EmployeeModule.ApplySalaryMultiplier(0.5f);
			Toast("Salary: 0.5x");
		}, 1, 4);
		ActionBtn(row, "1x", BtnNeutral, delegate
		{
			EmployeeModule.ApplySalaryMultiplier(1f);
			Toast("Salary: 1x");
		}, 2, 4);
		ActionBtn(row, "2x", BtnWarning, delegate
		{
			EmployeeModule.ApplySalaryMultiplier(2f);
			Toast("Salary: 2x");
		}, 3, 4);
		num += num3 + 40f;
		float num4 = 240f;
		Transform card3 = CreateCard(content, "Card_SetWages", 100f, num, 3610f, num4);
		cy = 40f;
		PlaceCardHeader(card3, "Set Wages", ref cy);
		PlaceInputWithButton(card3, ref cy, "Wage for All:", "e.g. 15.00", "Set All Wages", BtnBlue, (InputField.ContentType)3, delegate(string inputText)
		{
			if (float.TryParse(inputText, out var result))
			{
				EmployeeModule.SetAllWages(result);
				Toast($"All wages set to ${result:F2}/hr");
			}
			else
			{
				Toast("Invalid wage amount");
			}
		});
		num += num4 + 40f;
		float num5 = 704f;
		Transform card4 = CreateCard(content, "Card_Candidates", 100f, num, 3610f, num5);
		cy = 40f;
		PlaceCardHeader(card4, "Generate Recruitment Candidates", ref cy);
		PlaceInputWithButton(card4, ref cy, "Skill Level (1-100):", "e.g. 100", "Set", BtnBlue, (InputField.ContentType)3, delegate(string inputText)
		{
			if (int.TryParse(inputText, out var result) && result >= 1 && result <= 100)
			{
				EmployeeModule.CandidateSkillLevel = result;
			}
			else
			{
				Toast("Enter 1-100");
			}
		});
		Transform row2 = PlaceRow(card4, ref cy, 2);
		ActionBtn(row2, "CustService", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(0, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 0, 2);
		ActionBtn(row2, "Cleaning", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(1, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 1, 2);
		Transform row3 = PlaceRow(card4, ref cy, 2);
		ActionBtn(row3, "Lawyer", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(2, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 0, 2);
		ActionBtn(row3, "Purchasing", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(3, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 1, 2);
		Transform row4 = PlaceRow(card4, ref cy, 2);
		ActionBtn(row4, "Logistics", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(4, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 0, 2);
		ActionBtn(row4, "Delivery", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(5, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 1, 2);
		Transform row5 = PlaceRow(card4, ref cy, 2);
		ActionBtn(row5, "Programmer", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(6, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 0, 2);
		ActionBtn(row5, "HR Manager", BtnBlue, delegate
		{
			EmployeeModule.GenerateCandidate(7, EmployeeModule.CandidateSkillLevel);
			Toast("Candidate generated!");
		}, 1, 2);
		return num + (num5 + 40f);
	}

	private static float BuildRivalsTab(Transform content)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		float num2 = 340f;
		Transform card = CreateCard(content, "Card_RivalActions", 100f, num, 3610f, num2);
		float cy = 40f;
		PlaceCardHeader(card, "Rival Actions", ref cy);
		ActionBtn(PlaceRow(card, ref cy, 1), "Refresh Rivals Data", BtnBlue, delegate
		{
			RivalsModule.RefreshRivals();
			Toast("Rivals refreshed!");
		}, 0, 1);
		ActionBtn(PlaceRow(card, ref cy, 1), "Defeat ALL Rivals", BtnDanger, delegate
		{
			RivalsModule.DefeatAllRivals();
			Toast("All rivals defeated!");
		}, 0, 1);
		num += num2 + 40f;
		float num3 = 240f;
		Transform card2 = CreateCard(content, "Card_RivalsDifficulty", 100f, num, 3610f, num3);
		cy = 40f;
		PlaceCardHeader(card2, "Rivals Difficulty", ref cy);
		Transform row = PlaceRow(card2, ref cy, 4);
		ActionBtn(row, "Easy (0.5x)", BtnSuccess, delegate
		{
			BusinessModule.ApplyRivalsDifficultyMultiplier(0.5f);
			Toast("Rivals: Easy");
		}, 0, 4);
		ActionBtn(row, "Normal (1x)", BtnNeutral, delegate
		{
			BusinessModule.ApplyRivalsDifficultyMultiplier(1f);
			Toast("Rivals: Normal");
		}, 1, 4);
		ActionBtn(row, "Hard (2x)", BtnWarning, delegate
		{
			BusinessModule.ApplyRivalsDifficultyMultiplier(2f);
			Toast("Rivals: Hard");
		}, 2, 4);
		ActionBtn(row, "Brutal (5x)", BtnDanger, delegate
		{
			BusinessModule.ApplyRivalsDifficultyMultiplier(5f);
			Toast("Rivals: Brutal");
		}, 3, 4);
		return num + (num3 + 40f);
	}

	private static GameObject CreateObj(string name, Transform parent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		GameObject val = new GameObject(name);
		val.transform.SetParent(parent, false);
		val.AddComponent<RectTransform>();
		return val;
	}

	private static Transform CreateCard(Transform parent, string name, float x, float y, float width, float height)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = CreateObj(name, parent);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(x, 0f - y);
		component.sizeDelta = new Vector2(width, height);
		Image val = obj.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val.sprite = _spriteRoundedBox;
			val.type = (Image.Type)1;
		}
		((Graphic)val).color = CardWhite;
		return obj.transform;
	}

	private static void PlaceCardHeader(Transform card, string title, ref float cy)
	{
		float num = 3510f;
		GameObject cardGO = ((Component)card).gameObject;
		GameObject obj = CreateObj("Header", card);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 60f);
		CreateTMP(obj.transform, "Text", title, 50, CardTextDark, (FontStyles)1, (TextAlignmentOptions)513);
		int cardIndex = _cardStates.Count;
		float origHeight = ((Component)card).GetComponent<RectTransform>().sizeDelta.y;
		_cardStates.Add((title, cardGO, obj, false, origHeight));
		GameObject arrowGO = CreateObj("CollapseArrow", obj.transform);
		RectTransform arrowRT = arrowGO.GetComponent<RectTransform>();
		arrowRT.anchorMin = new Vector2(1f, 0.5f);
		arrowRT.anchorMax = new Vector2(1f, 0.5f);
		arrowRT.pivot = new Vector2(1f, 0.5f);
		arrowRT.anchoredPosition = new Vector2(-20f, 0f);
		arrowRT.sizeDelta = new Vector2(44f, 44f);
		CreateTMP(arrowGO.transform, "Arrow", "\u25bc", 24, CardTextMuted, (FontStyles)0, (TextAlignmentOptions)514);
		Image arrowImg = arrowGO.AddComponent<Image>();
		((Graphic)arrowImg).color = Color.clear;
		((Graphic)arrowImg).raycastTarget = true;
		Button arrowBtn = arrowGO.AddComponent<Button>();
		((Selectable)arrowBtn).targetGraphic = (Graphic)(object)arrowImg;
		ColorBlock arrowColors = ((Selectable)arrowBtn).colors;
		arrowColors.normalColor = Color.white;
		arrowColors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
		arrowColors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
		arrowColors.fadeDuration = 0.08f;
		((Selectable)arrowBtn).colors = arrowColors;
		((UnityEvent)arrowBtn.onClick).AddListener((UnityAction)delegate
		{
			ToggleCard(cardIndex);
		});
		cy += 80f;
	}

	private static Transform PlaceRow(Transform card, ref float cy, int buttonCount)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		GameObject obj = CreateObj("Row", card);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 80f);
		cy += 100f;
		return obj.transform;
	}

	private static float PlaceButtonRow(Transform card, float cy, float innerWidth, int count)
	{
		return 80f;
	}

	private static void ActionBtn(Transform row, string label, Color bgColor, Action onClick, int index, int count)
	{
		float x = ((Component)row).GetComponent<RectTransform>().sizeDelta.x;
		float num = 16f * (float)(count - 1);
		float num2 = (x - num) / (float)count;
		float num3 = (float)index * (num2 + 16f);
		GameObject obj = CreateObj("Btn_" + label, row);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(num3, 0f);
		component.sizeDelta = new Vector2(num2, 80f);
		Image val = obj.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val.sprite = _spriteRoundedBox;
			val.type = (Image.Type)1;
		}
		((Graphic)val).color = bgColor;
		((Graphic)val).raycastTarget = true;
		Button btn = obj.AddComponent<Button>();
		ColorBlock colors = ((Selectable)btn).colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
		colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
		colors.fadeDuration = 0.08f;
		((Selectable)btn).colors = colors;
		((Selectable)btn).targetGraphic = (Graphic)(object)val;
		((UnityEvent)btn.onClick).AddListener((UnityAction)delegate
		{
			try
			{
				onClick?.Invoke();
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[TrainerPanel] Button '" + label + "' error: " + ex.Message);
			}
		});
		CreateTMP(obj.transform, "Label", label, 32, Color.white, (FontStyles)1, (TextAlignmentOptions)514);
		if (_currentTabBeingBuilt >= 0)
		{
			_searchableItems.Add((obj, label, _currentTabBeingBuilt));
		}
		EventTrigger trigger = obj.AddComponent<EventTrigger>();
		EventTrigger.Entry enterEntry = new EventTrigger.Entry();
		enterEntry.eventID = EventTriggerType.PointerEnter;
		enterEntry.callback.AddListener((UnityAction<BaseEventData>)delegate { TooltipManager.Show(label); });
		trigger.triggers.Add(enterEntry);
		EventTrigger.Entry exitEntry = new EventTrigger.Entry();
		exitEntry.eventID = EventTriggerType.PointerExit;
		exitEntry.callback.AddListener((UnityAction<BaseEventData>)delegate { TooltipManager.Hide(); });
		trigger.triggers.Add(exitEntry);
	}

	private static void PlaceToggle(Transform card, ref float cy, string label, bool initialValue, Action<bool> onChanged)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		GameObject val = CreateObj("Toggle_" + label, card);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 80f);
		Image val2 = val.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val2.sprite = _spriteRoundedBox;
			val2.type = (Image.Type)1;
		}
		((Graphic)val2).color = ListBg;
		float num2 = 24f;
		float num3 = 160f;
		float num4 = num - num2 * 2f - num3 - 16f;
		GameObject obj = CreateObj("Label", val.transform);
		RectTransform component2 = obj.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 0f);
		component2.anchorMax = new Vector2(0f, 1f);
		component2.pivot = new Vector2(0f, 0.5f);
		component2.anchoredPosition = new Vector2(num2, 0f);
		component2.sizeDelta = new Vector2(num4, 0f);
		CreateTMP(obj.transform, "Text", label, 32, SectionHeaderLight, (FontStyles)0, (TextAlignmentOptions)513);
		GameObject val3 = CreateObj("ToggleBtn", val.transform);
		RectTransform component3 = val3.GetComponent<RectTransform>();
		component3.anchorMin = new Vector2(1f, 0.5f);
		component3.anchorMax = new Vector2(1f, 0.5f);
		component3.pivot = new Vector2(1f, 0.5f);
		component3.anchoredPosition = new Vector2(0f - num2, 0f);
		component3.sizeDelta = new Vector2(num3, 56f);
		Image toggleImg = val3.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			toggleImg.sprite = _spriteRoundedBox;
			toggleImg.type = (Image.Type)1;
		}
		((Graphic)toggleImg).color = (initialValue ? ToggleOn : ToggleOff);
		Button obj2 = val3.AddComponent<Button>();
		((Selectable)obj2).targetGraphic = (Graphic)(object)toggleImg;
		GameObject statusGO = CreateTMP(val3.transform, "Status", initialValue ? "ON" : "OFF", 28, Color.white, (FontStyles)1, (TextAlignmentOptions)514);
		bool current = initialValue;
((UnityEvent)obj2.onClick).AddListener((UnityAction)delegate
		{
			try
			{
				current = !current;
				onChanged?.Invoke(current);
				((Graphic)toggleImg).color = (current ? ToggleOn : ToggleOff);
				TextMeshProUGUI componentInChildren = statusGO.GetComponentInChildren<TextMeshProUGUI>();
				if ((Object)(object)componentInChildren != (Object)null)
				{
					((TMP_Text)componentInChildren).text = (current ? "ON" : "OFF");
					((Graphic)componentInChildren).color = Color.white;
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[TrainerPanel] Toggle error: " + ex.Message);
			}
		});
		cy += 100f;
	}

	private static void PlaceSlider(Transform card, ref float cy, string label, float min, float max, float initial, bool wholeNumbers, Action<float> onChanged, Action<float> onReleased = null)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		GameObject val = CreateObj("Slider_" + label, card);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 120f);
		float num2 = 44f;
		float num3 = 140f;
		GameObject obj = CreateObj("Label", val.transform);
		RectTransform component2 = obj.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 1f);
		component2.anchorMax = new Vector2(0f, 1f);
		component2.pivot = new Vector2(0f, 1f);
		component2.anchoredPosition = new Vector2(4f, 0f);
		component2.sizeDelta = new Vector2(num - num3 - 8f, num2);
		CreateTMP(obj.transform, "Text", label, 32, CardTextDark, (FontStyles)0, (TextAlignmentOptions)513);
		string text = (wholeNumbers ? $"{(int)initial}" : $"{initial:F1}");
		GameObject obj2 = CreateObj("Value", val.transform);
		RectTransform component3 = obj2.GetComponent<RectTransform>();
		component3.anchorMin = new Vector2(1f, 1f);
		component3.anchorMax = new Vector2(1f, 1f);
		component3.pivot = new Vector2(1f, 1f);
		component3.anchoredPosition = new Vector2(-4f, 0f);
		component3.sizeDelta = new Vector2(num3, num2);
		GameObject val2 = CreateTMP(obj2.transform, "Text", text, 28, BtnBlue, (FontStyles)1, (TextAlignmentOptions)516);
		TextMeshProUGUI valueTmp = val2.GetComponent<TextMeshProUGUI>();
		float num4 = num2 + 8f;
		float num5 = 120f - num4;
		float num6 = 30f;
		float num7 = (num5 - num6) / 2f;
		float num8 = 48f;
		GameObject val3 = CreateObj("SliderGO", val.transform);
		RectTransform component4 = val3.GetComponent<RectTransform>();
		component4.anchorMin = new Vector2(0f, 1f);
		component4.anchorMax = new Vector2(0f, 1f);
		component4.pivot = new Vector2(0f, 1f);
		component4.anchoredPosition = new Vector2(0f, 0f - num4);
		component4.sizeDelta = new Vector2(num, num5);
		GameObject obj3 = CreateObj("Background", val3.transform);
		RectTransform component5 = obj3.GetComponent<RectTransform>();
		component5.anchorMin = new Vector2(0f, 1f);
		component5.anchorMax = new Vector2(0f, 1f);
		component5.pivot = new Vector2(0f, 1f);
		component5.anchoredPosition = new Vector2(0f, 0f - num7);
		component5.sizeDelta = new Vector2(num, num6);
		Image val4 = obj3.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val4.sprite = _spriteRoundedBox;
			val4.type = (Image.Type)1;
		}
		((Graphic)val4).color = SliderTrack;
		GameObject val5 = CreateObj("Fill Area", val3.transform);
		RectTransform component6 = val5.GetComponent<RectTransform>();
		component6.anchorMin = new Vector2(0f, 1f);
		component6.anchorMax = new Vector2(0f, 1f);
		component6.pivot = new Vector2(0f, 1f);
		component6.anchoredPosition = new Vector2(5f, 0f - num7);
		component6.sizeDelta = new Vector2(num - 10f, num6);
		GameObject obj4 = CreateObj("Fill", val5.transform);
		RectTransform component7 = obj4.GetComponent<RectTransform>();
		component7.anchorMin = new Vector2(0f, 0f);
		component7.anchorMax = new Vector2(1f, 1f);
		component7.pivot = new Vector2(0f, 0.5f);
		component7.anchoredPosition = Vector2.zero;
		component7.sizeDelta = Vector2.zero;
		Image val6 = obj4.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val6.sprite = _spriteRoundedBox;
			val6.type = (Image.Type)1;
		}
		((Graphic)val6).color = SliderFill;
		GameObject val7 = CreateObj("Handle Slide Area", val3.transform);
		RectTransform component8 = val7.GetComponent<RectTransform>();
		component8.anchorMin = new Vector2(0f, 1f);
		component8.anchorMax = new Vector2(0f, 1f);
		component8.pivot = new Vector2(0f, 1f);
		component8.anchoredPosition = new Vector2(10f, 0f);
		component8.sizeDelta = new Vector2(num - 20f, num5);
		GameObject obj5 = CreateObj("Handle", val7.transform);
		RectTransform component9 = obj5.GetComponent<RectTransform>();
		component9.anchorMin = new Vector2(0f, 0.5f);
		component9.anchorMax = new Vector2(0f, 0.5f);
		component9.pivot = new Vector2(0.5f, 0.5f);
		component9.anchoredPosition = Vector2.zero;
		component9.sizeDelta = new Vector2(num8, num8);
		Image val8 = obj5.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val8.sprite = _spriteRoundedBox;
			val8.type = (Image.Type)1;
		}
		((Graphic)val8).color = SliderHandle;
		Slider obj6 = val3.AddComponent<Slider>();
		obj6.minValue = min;
		obj6.maxValue = max;
		obj6.wholeNumbers = wholeNumbers;
		obj6.value = Mathf.Clamp(initial, min, max);
		((Selectable)obj6).targetGraphic = (Graphic)(object)val8;
		obj6.fillRect = component7;
		obj6.handleRect = component9;
		((UnityEvent<float>)(object)obj6.onValueChanged).AddListener((UnityAction<float>)delegate(float num9)
		{
			try
			{
				if ((Object)(object)valueTmp != (Object)null)
				{
					((TMP_Text)valueTmp).text = (wholeNumbers ? $"{(int)num9}" : $"{num9:F1}");
				}
				onChanged?.Invoke(num9);
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[TrainerPanel] Slider '" + label + "' error: " + ex.Message);
			}
		});
		if (onReleased != null)
		{
			EventTrigger sliderTrigger = val3.AddComponent<EventTrigger>();
			EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
			pointerUpEntry.eventID = EventTriggerType.PointerUp;
			pointerUpEntry.callback.AddListener((UnityAction<BaseEventData>)delegate
			{
				try
				{
					onReleased(obj6.value);
				}
				catch (Exception ex2)
				{
					MelonLogger.Warning("[TrainerPanel] Slider release '" + label + "' error: " + ex2.Message);
				}
			});
			sliderTrigger.triggers.Add(pointerUpEntry);
		}
		cy += 140f;
	}

	private static void PlaceInputWithButton(Transform card, ref float cy, string label, string placeholder, string buttonLabel, Color buttonColor, InputField.ContentType contentType, Action<string> onSubmit)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		float num2 = 280f;
		float num3 = 220f;
		float num4 = 16f;
		float num5 = num - num2 - num3 - num4 * 2f;
		GameObject val = CreateObj("InputRow_" + label, card);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 80f);
		GameObject obj = CreateObj("Label", val.transform);
		RectTransform component2 = obj.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 0f);
		component2.anchorMax = new Vector2(0f, 1f);
		component2.pivot = new Vector2(0f, 0.5f);
		component2.anchoredPosition = new Vector2(0f, 0f);
		component2.sizeDelta = new Vector2(num2, 0f);
		CreateTMP(obj.transform, "Text", label, 32, CardTextDark, (FontStyles)0, (TextAlignmentOptions)513);
		float num6 = num2 + num4;
		GameObject val2 = CreateObj("InputField", val.transform);
		RectTransform component3 = val2.GetComponent<RectTransform>();
		component3.anchorMin = new Vector2(0f, 0.5f);
		component3.anchorMax = new Vector2(0f, 0.5f);
		component3.pivot = new Vector2(0f, 0.5f);
		component3.anchoredPosition = new Vector2(num6, 0f);
		component3.sizeDelta = new Vector2(num5, 64f);
		Image val3 = val2.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val3.sprite = _spriteRoundedBox;
			val3.type = (Image.Type)1;
		}
		((Graphic)val3).color = InputBg;
		GameObject val4 = new GameObject("Text");
		val4.transform.SetParent(val2.transform, false);
		RectTransform obj2 = val4.AddComponent<RectTransform>();
		obj2.anchorMin = Vector2.zero;
		obj2.anchorMax = Vector2.one;
		obj2.offsetMin = new Vector2(16f, 4f);
		obj2.offsetMax = new Vector2(-16f, -4f);
		Text val5 = val4.AddComponent<Text>();
		val5.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		val5.fontSize = 28;
		((Graphic)val5).color = InputText;
		val5.alignment = (TextAnchor)3;
		val5.supportRichText = false;
		GameObject val6 = new GameObject("Placeholder");
		val6.transform.SetParent(val2.transform, false);
		RectTransform obj3 = val6.AddComponent<RectTransform>();
		obj3.anchorMin = Vector2.zero;
		obj3.anchorMax = Vector2.one;
		obj3.offsetMin = new Vector2(16f, 4f);
		obj3.offsetMax = new Vector2(-16f, -4f);
		Text val7 = val6.AddComponent<Text>();
		val7.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		val7.fontSize = 28;
		((Graphic)val7).color = InputPlaceholder;
		val7.fontStyle = (FontStyle)2;
		val7.alignment = (TextAnchor)3;
		val7.text = placeholder;
		val7.supportRichText = false;
		InputField inputField = val2.AddComponent<InputField>();
		inputField.textComponent = val5;
		inputField.placeholder = (Graphic)(object)val7;
		inputField.contentType = contentType;
		((Selectable)inputField).targetGraphic = (Graphic)(object)val3;
		float num7 = num6 + num5 + num4;
		GameObject val8 = CreateObj("Btn_" + buttonLabel, val.transform);
		RectTransform component4 = val8.GetComponent<RectTransform>();
		component4.anchorMin = new Vector2(0f, 0.5f);
		component4.anchorMax = new Vector2(0f, 0.5f);
		component4.pivot = new Vector2(0f, 0.5f);
		component4.anchoredPosition = new Vector2(num7, 0f);
		component4.sizeDelta = new Vector2(num3, 64f);
		Image val9 = val8.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val9.sprite = _spriteRoundedBox;
			val9.type = (Image.Type)1;
		}
		((Graphic)val9).color = buttonColor;
		Button obj4 = val8.AddComponent<Button>();
		((Selectable)obj4).targetGraphic = (Graphic)(object)val9;
		ColorBlock colors = ((Selectable)obj4).colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
		colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
		colors.fadeDuration = 0.08f;
		((Selectable)obj4).colors = colors;
		CreateTMP(val8.transform, "Label", buttonLabel, 28, Color.white, (FontStyles)1, (TextAlignmentOptions)514);
		((UnityEvent)obj4.onClick).AddListener((UnityAction)delegate
		{
			try
			{
				string obj5 = inputField.text ?? "";
				onSubmit?.Invoke(obj5);
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[TrainerPanel] Input '" + label + "' error: " + ex.Message);
			}
		});
		cy += 100f;
	}

	private static void PlaceDualInputWithButton(Transform card, ref float cy, string label1, string placeholder1, string label2, string placeholder2, string buttonLabel, Color buttonColor, Action<string, string> onSubmit)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		float num2 = 110f;
		float num3 = 100f;
		float num4 = 220f;
		float num5 = 12f;
		float num6 = (num - num2 - num3 - num4 - num5 * 4f) / 2f;
		GameObject val = CreateObj("DualInputRow_" + label1 + "_" + label2, card);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 80f);
		float num7 = 0f;
		GameObject obj = CreateObj("Label1", val.transform);
		RectTransform component2 = obj.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0f, 0f);
		component2.anchorMax = new Vector2(0f, 1f);
		component2.pivot = new Vector2(0f, 0.5f);
		component2.anchoredPosition = new Vector2(num7, 0f);
		component2.sizeDelta = new Vector2(num2, 0f);
		CreateTMP(obj.transform, "Text", label1, 28, CardTextDark, (FontStyles)0, (TextAlignmentOptions)513);
		num7 += num2 + num5;
		GameObject val2 = CreateInputFieldOnly(val.transform, "Input1", placeholder1, num7, num6);
		InputField inputField1 = val2.GetComponent<InputField>();
		num7 += num6 + num5;
		GameObject obj2 = CreateObj("Label2", val.transform);
		RectTransform component3 = obj2.GetComponent<RectTransform>();
		component3.anchorMin = new Vector2(0f, 0f);
		component3.anchorMax = new Vector2(0f, 1f);
		component3.pivot = new Vector2(0f, 0.5f);
		component3.anchoredPosition = new Vector2(num7, 0f);
		component3.sizeDelta = new Vector2(num3, 0f);
		CreateTMP(obj2.transform, "Text", label2, 28, CardTextDark, (FontStyles)0, (TextAlignmentOptions)513);
		num7 += num3 + num5;
		GameObject val3 = CreateInputFieldOnly(val.transform, "Input2", placeholder2, num7, num6);
		InputField inputField2 = val3.GetComponent<InputField>();
		num7 += num6 + num5;
		GameObject val4 = CreateObj("Btn_" + buttonLabel, val.transform);
		RectTransform component4 = val4.GetComponent<RectTransform>();
		component4.anchorMin = new Vector2(0f, 0.5f);
		component4.anchorMax = new Vector2(0f, 0.5f);
		component4.pivot = new Vector2(0f, 0.5f);
		component4.anchoredPosition = new Vector2(num7, 0f);
		component4.sizeDelta = new Vector2(num4, 64f);
		Image val5 = val4.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val5.sprite = _spriteRoundedBox;
			val5.type = (Image.Type)1;
		}
		((Graphic)val5).color = buttonColor;
		Button obj3 = val4.AddComponent<Button>();
		((Selectable)obj3).targetGraphic = (Graphic)(object)val5;
		ColorBlock colors = ((Selectable)obj3).colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
		colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
		colors.fadeDuration = 0.08f;
		((Selectable)obj3).colors = colors;
		CreateTMP(val4.transform, "Label", buttonLabel, 28, Color.white, (FontStyles)1, (TextAlignmentOptions)514);
		((UnityEvent)obj3.onClick).AddListener((UnityAction)delegate
		{
			try
			{
				string arg = inputField1.text ?? "";
				string arg2 = inputField2.text ?? "";
				onSubmit?.Invoke(arg, arg2);
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[TrainerPanel] DualInput error: " + ex.Message);
			}
		});
		cy += 100f;
	}

	private static GameObject CreateInputFieldOnly(Transform parent, string name, string placeholder, float xPos, float width)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CreateObj(name, parent);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0.5f);
		component.anchorMax = new Vector2(0f, 0.5f);
		component.pivot = new Vector2(0f, 0.5f);
		component.anchoredPosition = new Vector2(xPos, 0f);
		component.sizeDelta = new Vector2(width, 64f);
		Image val2 = val.AddComponent<Image>();
		if ((Object)(object)_spriteRoundedBox != (Object)null)
		{
			val2.sprite = _spriteRoundedBox;
			val2.type = (Image.Type)1;
		}
		((Graphic)val2).color = InputBg;
		GameObject val3 = new GameObject("Text");
		val3.transform.SetParent(val.transform, false);
		RectTransform obj = val3.AddComponent<RectTransform>();
		obj.anchorMin = Vector2.zero;
		obj.anchorMax = Vector2.one;
		obj.offsetMin = new Vector2(16f, 4f);
		obj.offsetMax = new Vector2(-16f, -4f);
		Text val4 = val3.AddComponent<Text>();
		val4.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		val4.fontSize = 28;
		((Graphic)val4).color = InputText;
		val4.alignment = (TextAnchor)3;
		val4.supportRichText = false;
		GameObject val5 = new GameObject("Placeholder");
		val5.transform.SetParent(val.transform, false);
		RectTransform obj2 = val5.AddComponent<RectTransform>();
		obj2.anchorMin = Vector2.zero;
		obj2.anchorMax = Vector2.one;
		obj2.offsetMin = new Vector2(16f, 4f);
		obj2.offsetMax = new Vector2(-16f, -4f);
		Text val6 = val5.AddComponent<Text>();
		val6.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		val6.fontSize = 28;
		((Graphic)val6).color = InputPlaceholder;
		val6.fontStyle = (FontStyle)2;
		val6.alignment = (TextAnchor)3;
		val6.text = placeholder;
		val6.supportRichText = false;
		InputField obj3 = val.AddComponent<InputField>();
		obj3.textComponent = val4;
		obj3.placeholder = (Graphic)(object)val6;
		obj3.contentType = (InputField.ContentType)2;
		((Selectable)obj3).targetGraphic = (Graphic)(object)val2;
		return val;
	}

	private static void PlaceHelpText(Transform card, string text, ref float cy)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		GameObject obj = CreateObj("HelpText", card);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 44f);
		CreateTMP(obj.transform, "Text", text, 28, CardTextMuted, (FontStyles)2, (TextAlignmentOptions)513);
		cy += 64f;
	}

	private static void PlaceSectionLabel(Transform card, string text, ref float cy)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		float num = 3510f;
		GameObject obj = CreateObj("Section", card);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(50f, 0f - cy);
		component.sizeDelta = new Vector2(num, 50f);
		CreateTMP(obj.transform, "Text", text, 44, CardTextDark, (FontStyles)1, (TextAlignmentOptions)513);
		cy += 70f;
	}

	private static GameObject CreateTMP(Transform parent, string name, string text, int fontSize, Color color, FontStyles style, TextAlignmentOptions alignment)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = CreateObj(name, parent);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		TextMeshProUGUI val = obj.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val).text = text;
		((TMP_Text)val).fontSize = fontSize;
		((Graphic)val).color = color;
		((TMP_Text)val).fontStyle = style;
		((TMP_Text)val).alignment = alignment;
		((TMP_Text)val).enableWordWrapping = true;
		((TMP_Text)val).overflowMode = (TextOverflowModes)1;
		((Graphic)val).raycastTarget = false;
		if ((Object)(object)_gameFont != (Object)null)
		{
			((TMP_Text)val).font = _gameFont;
		}
		return obj;
	}

	private static void Toast(string msg)
	{
		ToastNotification.Show(msg);
	}
}
