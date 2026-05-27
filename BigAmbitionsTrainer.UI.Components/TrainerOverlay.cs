using System;
using BigAmbitionsTrainer.Config;
using BigAmbitionsTrainer.Modules;
using MelonLoader;
using UnityEngine;

namespace BigAmbitionsTrainer.UI.Components;

public static class TrainerOverlay
{
    private static bool _visible;
    private static bool _closing;
    private static float _animStartTime;
    private const float FadeDuration = 0.15f;
    private static int _activeTab;
    private static Vector2 _scrollPos;
    private static bool _stylesInited;
    private static Texture2D _winBgTex;
    private static Texture2D _tabBgTex;
    private static Texture2D _tabActiveBgTex;
    private static Texture2D _btnBgTex;
    private static Texture2D _scrollThumbTex;
    private static GUIStyle _windowBgStyle;
    private static GUIStyle _tabStyle;
    private static GUIStyle _tabActiveStyle;
    private static GUIStyle _btnStyle;
    private static GUIStyle _labelStyle;
    private static GUIStyle _scrollThumbStyle;

    private static readonly string[] TabNames = { "Money", "Player", "Vehicles", "Business", "Gameplay", "Staff", "Rivals", "Settings" };
    private const int TabCount = 8;
    private const float WinW = 880f;
    private const float WinH = 640f;
    private const float TabH = 36f;
    private const float ContentTop = 50f;
    private const float ContentH = 560f;

    public static bool Visible => _visible;
    public static void Toggle()
    {
        if (_closing)
        {
            _closing = false;
            _visible = true;
            _animStartTime = Time.unscaledTime;
        }
        else if (_visible)
        {
            _closing = true;
            _animStartTime = Time.unscaledTime;
        }
        else
        {
            _visible = true;
            _animStartTime = Time.unscaledTime;
            _activeTab = 0;
            _scrollPos = Vector2.zero;
        }
    }

    public static void OnGUI()
    {
        if (!_visible && !_closing) return;
        try
        {
            float elapsed = Time.unscaledTime - _animStartTime;
            float alpha = Mathf.Clamp01(elapsed / FadeDuration);
            if (_closing)
            {
                alpha = 1f - Mathf.Clamp01(elapsed / FadeDuration);
                if (alpha <= 0f)
                {
                    DestroyStyles();
                    _visible = false;
                    _closing = false;
                    return;
                }
            }

            EnsureStyles();
            float cx = ((float)Screen.width - WinW) * 0.5f;
            float cy = ((float)Screen.height - WinH) * 0.5f;

            Color prevColor = GUI.color;

            GUI.color = new Color(1f, 1f, 1f, alpha);
            GUI.Box(new Rect(cx, cy, WinW, WinH), GUIContent.none, _windowBgStyle);

            for (int i = 0; i < TabCount; i++)
            {
                float tw = (WinW - 16f) / (float)TabCount - 2f;
                float tx = cx + 8f + (float)i * (tw + 2f);
                if (ClickableButton(new Rect(tx, cy + 8f, tw, TabH), TabNames[i], i == _activeTab ? _tabActiveStyle : _tabStyle))
                {
                    _activeTab = i;
                    _scrollPos = Vector2.zero;
                }
            }

            float vw = WinW - 16f;
            GUI.Box(new Rect(cx + 8f, cy + ContentTop - 4f, vw, 2f), GUIContent.none);
            Rect viewRect = new Rect(cx + 8f, cy + ContentTop, vw, ContentH);

            if (Event.current.type == EventType.ScrollWheel && viewRect.Contains(Event.current.mousePosition))
            {
                _scrollPos.y -= Event.current.delta.y * 30f;
                Event.current.Use();
            }

            float contentBottom = 0f;
            float maxScroll = 0f;
            GUI.BeginGroup(viewRect);
            try
            {
                float sy = 4f - _scrollPos.y;
                switch (_activeTab)
                {
                    case 0: DrawMoneyTab(ref sy); break;
                    case 1: DrawPlayerTab(ref sy); break;
                    case 2: DrawVehicleTab(ref sy); break;
                    case 3: DrawBusinessTab(ref sy); break;
                    case 4: DrawGameplayTab(ref sy); break;
                    case 5: DrawStaffTab(ref sy); break;
                    case 6: DrawRivalsTab(ref sy); break;
                    case 7: DrawSettingsTab(ref sy); break;
                }

                contentBottom = sy + 10f;
                maxScroll = Mathf.Max(0f, contentBottom - ContentH);
                _scrollPos.y = Mathf.Clamp(_scrollPos.y, 0f, maxScroll);
            }
            finally
            {
                GUI.EndGroup();
            }

            if (maxScroll > 0f)
            {
                float sbW = 8f;
                float sbX = cx + 8f + vw - sbW - 3f;
                float thumbH = Mathf.Max(24f, ContentH * (ContentH / contentBottom));
                float thumbY = cy + ContentTop + (_scrollPos.y / maxScroll) * (ContentH - thumbH);
                GUI.Box(new Rect(sbX, thumbY, sbW, thumbH), GUIContent.none, _scrollThumbStyle);
            }

            GUI.color = prevColor;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[TrainerOverlay] Error: " + ex.Message);
        }
    }

    private static Texture2D MakeTex(int w, int h, Color color)
    {
        Texture2D tex = new Texture2D(w, h);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }

    private static readonly GUIContent _sharedContent = new GUIContent();

    private static bool IsClickInRect(Rect rect)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
        {
            Event.current.Use();
            return true;
        }
        return false;
    }

    private static bool ClickableButton(Rect rect, string text, GUIStyle style)
    {
        Color prev = GUI.color;
        bool hovering = Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition);
        if (hovering)
        {
            float b = 1.35f;
            GUI.color = new Color(b, b, b, prev.a);
        }
        GUI.Box(rect, GUIContent.none, style);
        GUI.Label(rect, text, style);
        if (hovering) GUI.color = prev;
        return IsClickInRect(rect);
    }

    private static void DestroyStyles()
    {
        _stylesInited = false;
        if (_winBgTex != null) { UnityEngine.Object.DestroyImmediate(_winBgTex); _winBgTex = null; }
        if (_tabBgTex != null) { UnityEngine.Object.DestroyImmediate(_tabBgTex); _tabBgTex = null; }
        if (_tabActiveBgTex != null) { UnityEngine.Object.DestroyImmediate(_tabActiveBgTex); _tabActiveBgTex = null; }
        if (_btnBgTex != null) { UnityEngine.Object.DestroyImmediate(_btnBgTex); _btnBgTex = null; }
        if (_scrollThumbTex != null) { UnityEngine.Object.DestroyImmediate(_scrollThumbTex); _scrollThumbTex = null; }
        _windowBgStyle = null;
        _tabStyle = null;
        _tabActiveStyle = null;
        _btnStyle = null;
        _labelStyle = null;
        _scrollThumbStyle = null;
    }

    public static void Cleanup()
    {
        DestroyStyles();
    }

    private static void EnsureStyles()
    {
        if (_stylesInited) return;

        _windowBgStyle = new GUIStyle();
        _winBgTex = MakeTex(1, 1, new Color(0.1f, 0.12f, 0.18f, 1f));
        GUIStyleState winState = new GUIStyleState();
        winState.background = _winBgTex;
        _windowBgStyle.normal = winState;

        _tabStyle = new GUIStyle();
        _tabBgTex = MakeTex(1, 1, new Color(0.3f, 0.32f, 0.4f, 1f));
        GUIStyleState tabState = new GUIStyleState();
        tabState.background = _tabBgTex;
        tabState.textColor = new Color(0.85f, 0.87f, 0.9f, 1f);
        _tabStyle.normal = tabState;
        _tabStyle.fontSize = 14;
        _tabStyle.alignment = TextAnchor.MiddleCenter;
        _tabStyle.padding = new RectOffset(6, 6, 4, 4);

        _tabActiveStyle = new GUIStyle();
        _tabActiveBgTex = MakeTex(1, 1, new Color(0.35f, 0.6f, 0.85f, 1f));
        GUIStyleState tabActiveState = new GUIStyleState();
        tabActiveState.background = _tabActiveBgTex;
        tabActiveState.textColor = Color.white;
        _tabActiveStyle.normal = tabActiveState;
        _tabActiveStyle.fontSize = 14;
        _tabActiveStyle.alignment = TextAnchor.MiddleCenter;
        _tabActiveStyle.padding = new RectOffset(6, 6, 4, 4);

        _btnStyle = new GUIStyle();
        _btnBgTex = MakeTex(1, 1, new Color(0.4f, 0.42f, 0.5f, 1f));
        GUIStyleState btnState = new GUIStyleState();
        btnState.background = _btnBgTex;
        btnState.textColor = new Color(0.95f, 0.96f, 0.98f, 1f);
        _btnStyle.normal = btnState;
        _btnStyle.fontSize = 14;
        _btnStyle.alignment = TextAnchor.MiddleCenter;
        _btnStyle.padding = new RectOffset(8, 8, 4, 4);

        _labelStyle = new GUIStyle();
        GUIStyleState labelState = new GUIStyleState();
        labelState.textColor = new Color(0.6f, 0.75f, 0.95f, 1f);
        _labelStyle.normal = labelState;
        _labelStyle.fontSize = 15;
        _labelStyle.alignment = TextAnchor.UpperLeft;

        _scrollThumbStyle = new GUIStyle();
        _scrollThumbTex = MakeTex(1, 1, new Color(0.5f, 0.5f, 0.55f, 0.5f));
        GUIStyleState thumbState = new GUIStyleState();
        thumbState.background = _scrollThumbTex;
        _scrollThumbStyle.normal = thumbState;

        _stylesInited = true;
    }

    private static bool ToggleBtn(string label, bool current, float x, float y, float w)
    {
        Color orig = GUI.color;
        GUI.color = current ? new Color(0.3f, 0.65f, 0.35f, 1f) : new Color(0.55f, 0.3f, 0.3f, 1f);
        bool clicked = ClickableButton(new Rect(x, y, w, 30f), (current ? "ON: " : "OFF: ") + label, _btnStyle);
        GUI.color = orig;
        if (clicked) return !current;
        return current;
    }

    private static void SectionLabel(string text, ref float sy, float vw)
    {
        GUI.Label(new Rect(4f, sy, vw, 26f), text, _labelStyle);
        sy += 32f;
    }

    private static void DrawMoneyTab(ref float sy)
    {
        float vw = WinW - 36f;
        SectionLabel("Quick Add Money", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 12f) / 4f, 32f), "$1K", _btnStyle)) { MoneyModule.AddMoney(1000f); ToastNotification.Show("Added $1,000"); }
        if (ClickableButton(new Rect(8f + (vw - 12f) / 4f, sy, (vw - 12f) / 4f, 32f), "$5K", _btnStyle)) { MoneyModule.AddMoney(5000f); ToastNotification.Show("Added $5,000"); }
        if (ClickableButton(new Rect(12f + (vw - 12f) / 2f, sy, (vw - 12f) / 4f, 32f), "$10K", _btnStyle)) { MoneyModule.AddMoney(10000f); ToastNotification.Show("Added $10,000"); }
        if (ClickableButton(new Rect(16f + (vw - 12f) * 3f / 4f, sy, (vw - 12f) / 4f, 32f), "$50K", _btnStyle)) { MoneyModule.AddMoney(50000f); ToastNotification.Show("Added $50,000"); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, (vw - 8f) / 3f, 32f), "$100K", _btnStyle)) { MoneyModule.AddMoney(100000f); ToastNotification.Show("Added $100,000"); }
        if (ClickableButton(new Rect(8f + (vw - 8f) / 3f, sy, (vw - 8f) / 3f, 32f), "$500K", _btnStyle)) { MoneyModule.AddMoney(500000f); ToastNotification.Show("Added $500,000"); }
        if (ClickableButton(new Rect(12f + (vw - 8f) * 2f / 3f, sy, (vw - 8f) / 3f, 32f), "$1M", _btnStyle)) { MoneyModule.AddMoney(1000000f); ToastNotification.Show("Added $1,000,000"); }
        sy += 38f;
        SectionLabel("Economy", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Tax: " + MoneyModule.TaxPercentage + "%", _btnStyle)) { MoneyModule.ApplyTaxPercentage((int)(MoneyModule.TaxPercentage + 5f > 100f ? 0f : MoneyModule.TaxPercentage + 5f)); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Price Mult: " + MoneyModule.MarketPriceMultiplier.ToString("F1") + "x", _btnStyle)) { MoneyModule.ApplyMarketPriceMultiplier(MoneyModule.MarketPriceMultiplier >= 5f ? 0.1f : MoneyModule.MarketPriceMultiplier + 0.5f); }
        sy += 38f;
    }

    private static void DrawPlayerTab(ref float sy)
    {
        float vw = WinW - 36f;
        SectionLabel("Needs & Stats", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, vw, 32f), "Fill All Needs", _btnStyle)) { PlayerStatsModule.FillAllNeeds(); ToastNotification.Show("All needs filled!"); }
        sy += 38f;
        SectionLabel("Energy", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 8f) / 4f, 32f), "25", _btnStyle)) { PlayerStatsModule.SetEnergy(25f); ToastNotification.Show("Energy: 25"); }
        if (ClickableButton(new Rect(8f + (vw - 8f) / 4f, sy, (vw - 8f) / 4f, 32f), "50", _btnStyle)) { PlayerStatsModule.SetEnergy(50f); ToastNotification.Show("Energy: 50"); }
        if (ClickableButton(new Rect(12f + (vw - 8f) / 2f, sy, (vw - 8f) / 4f, 32f), "75", _btnStyle)) { PlayerStatsModule.SetEnergy(75f); ToastNotification.Show("Energy: 75"); }
        if (ClickableButton(new Rect(16f + (vw - 8f) * 3f / 4f, sy, (vw - 8f) / 4f, 32f), "100", _btnStyle)) { PlayerStatsModule.SetEnergy(100f); ToastNotification.Show("Energy: 100"); }
        sy += 38f;
        SectionLabel("Happiness", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "-25 Happy", _btnStyle)) { PlayerStatsModule.ChangeHappiness(-25); ToastNotification.Show("Happiness -25"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "+25 Happy", _btnStyle)) { PlayerStatsModule.ChangeHappiness(25); ToastNotification.Show("Happiness +25"); }
        sy += 38f;
        SectionLabel("Hunger", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "-25 Hunger", _btnStyle)) { PlayerStatsModule.ChangeHunger(-25); ToastNotification.Show("Hunger -25"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "+25 Hunger", _btnStyle)) { PlayerStatsModule.ChangeHunger(25); ToastNotification.Show("Hunger +25"); }
        sy += 38f;
        SectionLabel("Speed", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 8f) / 4f, 32f), "Walk", _btnStyle)) { PlayerStatsModule.SetPlayerSpeed(0); ToastNotification.Show("Speed: Walk"); }
        if (ClickableButton(new Rect(8f + (vw - 8f) / 4f, sy, (vw - 8f) / 4f, 32f), "Jog", _btnStyle)) { PlayerStatsModule.SetPlayerSpeed(1); ToastNotification.Show("Speed: Jog"); }
        if (ClickableButton(new Rect(12f + (vw - 8f) / 2f, sy, (vw - 8f) / 4f, 32f), "Run", _btnStyle)) { PlayerStatsModule.SetPlayerSpeed(2); ToastNotification.Show("Speed: Run"); }
        if (ClickableButton(new Rect(16f + (vw - 8f) * 3f / 4f, sy, (vw - 8f) / 4f, 32f), "Scooter", _btnStyle)) { PlayerStatsModule.SetPlayerSpeed(3); ToastNotification.Show("Speed: Scooter"); }
        sy += 38f;
        SectionLabel("Toggles", ref sy, vw);
        TrainerConfig.DisableEnergy = ToggleBtn("Energy Decay", TrainerConfig.DisableEnergy, 4f, sy, (vw - 4f) / 2f);
        TrainerConfig.DisableHappiness = ToggleBtn("Happy Decay", TrainerConfig.DisableHappiness, 8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f);
        sy += 36f;
        TrainerConfig.DisableHunger = ToggleBtn("Hunger Decay", TrainerConfig.DisableHunger, 4f, sy, (vw - 4f) / 2f);
        TrainerConfig.DisableAging = ToggleBtn("Aging", TrainerConfig.DisableAging, 8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f);
        sy += 36f;
        SectionLabel("Age", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "-5 Years", _btnStyle)) { PlayerStatsModule.ChangeAge(-5f); ToastNotification.Show("Age -5"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "+5 Years", _btnStyle)) { PlayerStatsModule.ChangeAge(5f); ToastNotification.Show("Age +5"); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, vw, 32f), "Complete All Personal Goals", _btnStyle)) { PlayerStatsModule.CompletePersonalGoals(); ToastNotification.Show("Goals completed!"); }
        sy += 38f;
    }

    private static void DrawVehicleTab(ref float sy)
    {
        float vw = WinW - 36f;
        SectionLabel("Toggles", ref sy, vw);
        TrainerConfig.DisableVehicleDamage = ToggleBtn("Vehicle Damage", TrainerConfig.DisableVehicleDamage, 4f, sy, (vw - 4f) / 2f);
        TrainerConfig.DisableVehicleFuel = ToggleBtn("Vehicle Fuel", TrainerConfig.DisableVehicleFuel, 8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f);
        sy += 36f;
        SectionLabel("Actions", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Repair", _btnStyle)) { VehicleModule.RepairVehicle(); ToastNotification.Show("Vehicle repaired!"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Refuel", _btnStyle)) { VehicleModule.RefuelVehicle(); ToastNotification.Show("Vehicle refueled!"); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Clean", _btnStyle)) { VehicleModule.CleanVehicle(); ToastNotification.Show("Vehicle cleaned!"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Clear Tickets", _btnStyle)) { VehicleModule.ClearParkingTickets(); ToastNotification.Show("Tickets cleared!"); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Tow to Gas", _btnStyle)) { VehicleModule.TowToGasStation(); ToastNotification.Show("Towing to gas..."); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Tow to Repair", _btnStyle)) { VehicleModule.TowToAutoRepair(); ToastNotification.Show("Towing to repair..."); }
        sy += 38f;
    }

    private static void DrawBusinessTab(ref float sy)
    {
        float vw = WinW - 36f;
        if (ClickableButton(new Rect(4f, sy, vw, 32f), "Max All Customer Satisfaction", _btnStyle)) { BusinessModule.MaxAllSatisfaction(); ToastNotification.Show("All satisfaction maxed!"); }
        sy += 38f;
        SectionLabel("Unlocks", ref sy, vw);
        TrainerConfig.AllCoursesUnlocked = ToggleBtn("All Courses", TrainerConfig.AllCoursesUnlocked, 4f, sy, (vw - 4f) / 2f);
        TrainerConfig.AllContactsUnlocked = ToggleBtn("All Contacts", TrainerConfig.AllContactsUnlocked, 8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f);
        sy += 36f;
        TrainerConfig.DisableWholesaleImportLimits = ToggleBtn("No Import Limits", TrainerConfig.DisableWholesaleImportLimits, 4f, sy, (vw - 4f) / 2f);
        TrainerConfig.AllProductsFromImporters = ToggleBtn("All Import Products", TrainerConfig.AllProductsFromImporters, 8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f);
        sy += 36f;
        SectionLabel("Multipliers", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Promo: " + BusinessModule.CustomerPromotionMultiplier.ToString("F1") + "x", _btnStyle)) { BusinessModule.ApplyCustomerPromotionMultiplier(BusinessModule.CustomerPromotionMultiplier >= 10f ? 0.1f : BusinessModule.CustomerPromotionMultiplier + 0.5f); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Salary: " + BusinessModule.EmployeeSalaryMultiplier.ToString("F1") + "x", _btnStyle)) { BusinessModule.ApplyEmployeeSalaryMultiplier(BusinessModule.EmployeeSalaryMultiplier >= 5f ? 0f : BusinessModule.EmployeeSalaryMultiplier + 0.25f); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Interest: " + BusinessModule.BankInterestRate + "%", _btnStyle)) { BusinessModule.ApplyBankInterestRate(BusinessModule.BankInterestRate >= 50f ? 0f : BusinessModule.BankInterestRate + 5f); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Rivals: " + BusinessModule.RivalsDifficultyMultiplier.ToString("F1") + "x", _btnStyle)) { BusinessModule.ApplyRivalsDifficultyMultiplier(BusinessModule.RivalsDifficultyMultiplier >= 5f ? 0f : BusinessModule.RivalsDifficultyMultiplier + 0.5f); }
        sy += 38f;
    }

    private static void DrawGameplayTab(ref float sy)
    {
        float vw = WinW - 36f;
        SectionLabel("Game Speed", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 12f) / 5f, 32f), "Pause", _btnStyle)) { GameplayModule.SetGameSpeed(0f); ToastNotification.Show("Paused"); }
        if (ClickableButton(new Rect(8f + (vw - 12f) / 5f, sy, (vw - 12f) / 5f, 32f), "1x", _btnStyle)) { GameplayModule.SetGameSpeed(1f); ToastNotification.Show("Speed: 1x"); }
        if (ClickableButton(new Rect(12f + (vw - 12f) * 2f / 5f, sy, (vw - 12f) / 5f, 32f), "2x", _btnStyle)) { GameplayModule.SetGameSpeed(2f); ToastNotification.Show("Speed: 2x"); }
        if (ClickableButton(new Rect(16f + (vw - 12f) * 3f / 5f, sy, (vw - 12f) / 5f, 32f), "5x", _btnStyle)) { GameplayModule.SetGameSpeed(5f); ToastNotification.Show("Speed: 5x"); }
        if (ClickableButton(new Rect(20f + (vw - 12f) * 4f / 5f, sy, (vw - 12f) / 5f, 32f), "10x", _btnStyle)) { GameplayModule.SetGameSpeed(10f); ToastNotification.Show("Speed: 10x"); }
        sy += 38f;
        SectionLabel("Time", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, vw, 32f), "Skip to Next Day", _btnStyle)) { GameplayModule.SkipToNextDay(); ToastNotification.Show("Skipped to next day"); }
        sy += 38f;
        float tw = (vw - 12f) / 4f;
        if (ClickableButton(new Rect(4f, sy, tw, 32f), "6 AM", _btnStyle)) { GameplayModule.SetTimeOfDay(6, 0); }
        if (ClickableButton(new Rect(8f + tw, sy, tw, 32f), "12 PM", _btnStyle)) { GameplayModule.SetTimeOfDay(12, 0); }
        if (ClickableButton(new Rect(12f + tw * 2f, sy, tw, 32f), "6 PM", _btnStyle)) { GameplayModule.SetTimeOfDay(18, 0); }
        if (ClickableButton(new Rect(16f + tw * 3f, sy, tw, 32f), "10 PM", _btnStyle)) { GameplayModule.SetTimeOfDay(22, 0); }
        sy += 38f;
        SectionLabel("Toggles", ref sy, vw);
        TrainerConfig.DisableTraffic = ToggleBtn("Traffic", TrainerConfig.DisableTraffic, 4f, sy, (vw - 4f) / 2f);
        TrainerConfig.Invincibility = ToggleBtn("Invincibility", TrainerConfig.Invincibility, 8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f);
        sy += 36f;
        TrainerConfig.DisableTutorial = ToggleBtn("Tutorial", TrainerConfig.DisableTutorial, 4f, sy, (vw - 4f) / 2f);
        sy += 36f;
        SectionLabel("Quests & Imports", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Complete Quest", _btnStyle)) { GameplayModule.CompleteQuest(); ToastNotification.Show("Quest completed!"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Complete Objective", _btnStyle)) { GameplayModule.CompleteObjective(); ToastNotification.Show("Objective completed!"); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Deliver Paid", _btnStyle)) { GameplayModule.DeliverAllImportsPaid(); ToastNotification.Show("Imports delivered (paid)"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Deliver Free", _btnStyle)) { GameplayModule.DeliverAllImportsFree(); ToastNotification.Show("Imports delivered (free)"); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, vw, 32f), "Save Game", _btnStyle)) { GameplayModule.SaveGame(); ToastNotification.Show("Game saved!"); }
        sy += 38f;
    }

    private static void DrawStaffTab(ref float sy)
    {
        float vw = WinW - 36f;
        if (ClickableButton(new Rect(4f, sy, vw, 32f), "Max ALL Employee Satisfaction", _btnStyle)) { EmployeeModule.MaxAllSatisfaction(); ToastNotification.Show("All satisfaction maxed!"); }
        sy += 38f;
        SectionLabel("Salary", ref sy, vw);
        if (ClickableButton(new Rect(4f, sy, (vw - 8f) / 4f, 32f), "Free", _btnStyle)) { EmployeeModule.ApplySalaryMultiplier(0f); ToastNotification.Show("Salary: Free"); }
        if (ClickableButton(new Rect(8f + (vw - 8f) / 4f, sy, (vw - 8f) / 4f, 32f), "0.5x", _btnStyle)) { EmployeeModule.ApplySalaryMultiplier(0.5f); }
        if (ClickableButton(new Rect(12f + (vw - 8f) / 2f, sy, (vw - 8f) / 4f, 32f), "1x", _btnStyle)) { EmployeeModule.ApplySalaryMultiplier(1f); }
        if (ClickableButton(new Rect(16f + (vw - 8f) * 3f / 4f, sy, (vw - 8f) / 4f, 32f), "2x", _btnStyle)) { EmployeeModule.ApplySalaryMultiplier(2f); }
        sy += 38f;
        SectionLabel("Candidates (Lv" + EmployeeModule.CandidateSkillLevel + ")", ref sy, vw);
        float cw = (vw - 12f) / 4f;
        if (ClickableButton(new Rect(4f, sy, cw, 32f), "CustService", _btnStyle)) { EmployeeModule.GenerateCandidate(0, EmployeeModule.CandidateSkillLevel); }
        if (ClickableButton(new Rect(8f + cw, sy, cw, 32f), "Cleaning", _btnStyle)) { EmployeeModule.GenerateCandidate(1, EmployeeModule.CandidateSkillLevel); }
        if (ClickableButton(new Rect(12f + cw * 2f, sy, cw, 32f), "Lawyer", _btnStyle)) { EmployeeModule.GenerateCandidate(2, EmployeeModule.CandidateSkillLevel); }
        if (ClickableButton(new Rect(16f + cw * 3f, sy, cw, 32f), "Purchasing", _btnStyle)) { EmployeeModule.GenerateCandidate(3, EmployeeModule.CandidateSkillLevel); }
        sy += 38f;
        if (ClickableButton(new Rect(4f, sy, cw, 32f), "Logistics", _btnStyle)) { EmployeeModule.GenerateCandidate(4, EmployeeModule.CandidateSkillLevel); }
        if (ClickableButton(new Rect(8f + cw, sy, cw, 32f), "Delivery", _btnStyle)) { EmployeeModule.GenerateCandidate(5, EmployeeModule.CandidateSkillLevel); }
        if (ClickableButton(new Rect(12f + cw * 2f, sy, cw, 32f), "Programmer", _btnStyle)) { EmployeeModule.GenerateCandidate(6, EmployeeModule.CandidateSkillLevel); }
        if (ClickableButton(new Rect(16f + cw * 3f, sy, cw, 32f), "HR Manager", _btnStyle)) { EmployeeModule.GenerateCandidate(7, EmployeeModule.CandidateSkillLevel); }
        sy += 38f;
    }

    private static void DrawRivalsTab(ref float sy)
    {
        float vw = WinW - 36f;
        if (ClickableButton(new Rect(4f, sy, (vw - 4f) / 2f, 32f), "Refresh Data", _btnStyle)) { RivalsModule.RefreshRivals(); ToastNotification.Show("Rivals refreshed!"); }
        if (ClickableButton(new Rect(8f + (vw - 4f) / 2f, sy, (vw - 4f) / 2f, 32f), "Defeat ALL", _btnStyle)) { RivalsModule.DefeatAllRivals(); ToastNotification.Show("All rivals defeated!"); }
        sy += 38f;
        SectionLabel("Difficulty", ref sy, vw);
        float dw = (vw - 12f) / 4f;
        if (ClickableButton(new Rect(4f, sy, dw, 32f), "Easy", _btnStyle)) { BusinessModule.ApplyRivalsDifficultyMultiplier(0.5f); ToastNotification.Show("Rivals: Easy"); }
        if (ClickableButton(new Rect(8f + dw, sy, dw, 32f), "Normal", _btnStyle)) { BusinessModule.ApplyRivalsDifficultyMultiplier(1f); ToastNotification.Show("Rivals: Normal"); }
        if (ClickableButton(new Rect(12f + dw * 2f, sy, dw, 32f), "Hard", _btnStyle)) { BusinessModule.ApplyRivalsDifficultyMultiplier(2f); ToastNotification.Show("Rivals: Hard"); }
        if (ClickableButton(new Rect(16f + dw * 3f, sy, dw, 32f), "Brutal", _btnStyle)) { BusinessModule.ApplyRivalsDifficultyMultiplier(5f); ToastNotification.Show("Rivals: Brutal"); }
        sy += 38f;
    }

    private static void DrawSettingsTab(ref float sy)
    {
        float vw = WinW - 36f;
        SectionLabel("Settings", ref sy, vw);
        float halfW = (vw - 8f) / 2f;
        if (ClickableButton(new Rect(4f, sy, halfW, 36f), "Save All Settings", _btnStyle))
        {
            TrainerConfig.Save();
            ToastNotification.Show("Settings saved!");
        }
        if (ClickableButton(new Rect(8f + halfW, sy, halfW, 36f), "Load All Settings", _btnStyle))
        {
            TrainerConfig.Load();
            ToastNotification.Show("Settings loaded!");
        }
        sy += 44f;
        TrainerConfig.PhoneIntegration = ToggleBtn("Show Trainer in Phone", TrainerConfig.PhoneIntegration, 4f, sy, vw);
        sy += 38f;
        SectionLabel("Info", ref sy, vw);
        GUI.Label(new Rect(4f, sy, vw, 26f), "ItzRealOzone Trainer v4.0", _labelStyle);
        sy += 24f;
        GUI.Label(new Rect(4f, sy, vw, 26f), "Press F8 to toggle this overlay", _labelStyle);
        sy += 24f;
        if (ClickableButton(new Rect(4f, sy, vw, 36f), "Close Overlay", _btnStyle)) { _closing = true; _animStartTime = Time.unscaledTime; }
        sy += 44f;
    }
}
