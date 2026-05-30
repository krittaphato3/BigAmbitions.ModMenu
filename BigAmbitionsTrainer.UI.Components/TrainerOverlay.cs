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
    private const float FadeDuration = 0.2f;
    private static int _activeTab;
    private static int _prevActiveTab;
    private static float _tabSwitchTime;
    private static float _tabAnimProgress = 1f;
    private const float TabAnimDuration = 0.12f;
    private static Vector2 _scrollPos;
    private static bool _stylesInited;

    private static Texture2D _winBgTex;
    private static Texture2D _winBorderTex;
    private static Texture2D _tabBgTex;
    private static Texture2D _tabActiveBgTex;
    private static Texture2D _tabHoverBgTex;
    private static Texture2D _tabIndicatorTex;
    private static Texture2D _btnBgTex;
    private static Texture2D _sectionBgTex;
    private static Texture2D _accentLineTex;
    private static Texture2D _scrollTrackTex;
    private static Texture2D _scrollThumbTex;
    private static Texture2D _scrollThumbHoverTex;
    private static Texture2D _sliderTrackTex;
    private static Texture2D _sliderFillTex;
    private static Texture2D _sliderHandleTex;
    private static Texture2D _inputBgTex;
    private static Texture2D _inputBorderTex;

    private static GUIStyle _windowBgStyle;
    private static GUIStyle _windowBorderStyle;
    private static GUIStyle _tabStyle;
    private static GUIStyle _tabActiveStyle;
    private static GUIStyle _tabHoverStyle;
    private static GUIStyle _btnStyle;
    private static GUIStyle _sectionLabelStyle;
    private static GUIStyle _scrollThumbStyle;
    private static GUIStyle _scrollTrackStyle;
    private static GUIStyle _titleStyle;
    private static GUIStyle _sliderValueStyle;
    private static GUIStyle _inputTextStyle;

    private static readonly string[] TabNames = { "Money", "Player", "Vehicles", "Business", "Gameplay", "Staff", "Rivals", "Settings" };
    private const int TabCount = 8;
    private const float WinW = 940f;
    private const float WinH = 700f;
    private const float TitleBarH = 32f;
    private const float TabH = 38f;
    private const float ContentTop = 80f;
    private const float ContentH = 600f;

    private static readonly Color AccentBlue = new Color(0.271f, 0.477f, 0.66f, 1f);
    private static readonly Color AccentGreen = new Color(0.22f, 0.72f, 0.35f, 1f);
    private static readonly Color AccentRed = new Color(0.82f, 0.22f, 0.22f, 1f);
    private static readonly Color AccentOrange = new Color(0.9f, 0.58f, 0.15f, 1f);
    private static readonly Color ToggleOnColor = new Color(0.22f, 0.72f, 0.35f, 1f);
    private static readonly Color ToggleOffColor = new Color(0.3f, 0.32f, 0.38f, 1f);
    private static readonly Color SectionBgColor = new Color(0.08f, 0.1f, 0.15f, 0.4f);
    private static readonly Color SliderTrackColor = new Color(0.12f, 0.14f, 0.19f, 0.8f);
    private static readonly Color SliderFillColor = new Color(0.271f, 0.477f, 0.66f, 1f);
    private static readonly Color SliderHandleColor = Color.white;
    private static readonly Color InputBgColor = new Color(0.12f, 0.14f, 0.19f, 0.6f);
    private static readonly Color InputBorderColor = new Color(0.25f, 0.27f, 0.32f, 1f);
    private static readonly Color TextLight = new Color(0.9f, 0.91f, 0.93f, 1f);
    private static readonly Color TextMuted = new Color(0.55f, 0.57f, 0.62f, 1f);

    public static bool Visible => _visible;

    private static int _draggingSliderId = -1;
    private static int _focusedInputId = -1;
    private static string[] _inputTexts = new string[20];
    private static float _cursorBlinkTime;
    private static bool _cursorVisible;

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
            _prevActiveTab = 0;
            _tabAnimProgress = 1f;
            _scrollPos = Vector2.zero;
            _draggingSliderId = -1;
            _focusedInputId = -1;
        }
    }

    public static void OnGUI()
    {
        if (!_visible && !_closing) return;

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape && _focusedInputId >= 0)
        {
            _focusedInputId = -1;
            Event.current.Use();
            return;
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape && _visible && !_closing && _focusedInputId < 0)
        {
            _closing = true;
            _animStartTime = Time.unscaledTime;
            Event.current.Use();
            return;
        }

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
                    _focusedInputId = -1;
                    return;
                }
            }

            _cursorBlinkTime += Time.unscaledDeltaTime;
            if (_cursorBlinkTime > 0.5f)
            {
                _cursorVisible = !_cursorVisible;
                _cursorBlinkTime = 0f;
            }

            EnsureStyles();

            if (_tabAnimProgress < 1f)
            {
                _tabAnimProgress = Mathf.Clamp01((Time.unscaledTime - _tabSwitchTime) / TabAnimDuration);
            }

            float cx = ((float)Screen.width - WinW) * 0.5f;
            float cy = ((float)Screen.height - WinH) * 0.5f;

            Color prevColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);

            DrawWindowBackground(cx, cy);
            DrawTitleBar(cx, cy);
            DrawTabBar(cx, cy);
            DrawTabIndicator(cx, cy);

            float vw = WinW - 24f;
            Rect viewRect = new Rect(cx + 12f, cy + ContentTop, vw, ContentH);

            if (Event.current.type == EventType.ScrollWheel && _focusedInputId < 0 && viewRect.Contains(Event.current.mousePosition))
            {
                _scrollPos.y -= Event.current.delta.y * 35f;
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
                    case 0: DrawMoneyTab(ref sy, vw); break;
                    case 1: DrawPlayerTab(ref sy, vw); break;
                    case 2: DrawVehicleTab(ref sy, vw); break;
                    case 3: DrawBusinessTab(ref sy, vw); break;
                    case 4: DrawGameplayTab(ref sy, vw); break;
                    case 5: DrawStaffTab(ref sy, vw); break;
                    case 6: DrawRivalsTab(ref sy, vw); break;
                    case 7: DrawSettingsTab(ref sy, vw); break;
                }
                contentBottom = sy + 10f;
                maxScroll = Mathf.Max(0f, contentBottom - ContentH);
                _scrollPos.y = Mathf.Clamp(_scrollPos.y, 0f, maxScroll);
            }
            finally
            {
                GUI.EndGroup();
            }

            DrawScrollbar(cx, cy, vw, contentBottom, maxScroll);

            if (_draggingSliderId >= 0 && Event.current.type == EventType.MouseUp)
            {
                _draggingSliderId = -1;
            }

            GUI.color = prevColor;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[TrainerOverlay] Error: " + ex.Message);
        }
    }

    private static void DrawWindowBackground(float cx, float cy)
    {
        GUI.Box(new Rect(cx, cy, WinW, WinH), GUIContent.none, _windowBgStyle);
        GUI.Box(new Rect(cx, cy, WinW, WinH), GUIContent.none, _windowBorderStyle);
    }

    private static void DrawTitleBar(float cx, float cy)
    {
        GUI.Label(new Rect(cx + 16f, cy + 4f, 300f, TitleBarH), "ItzRealOzone Trainer v1.0.1", _titleStyle);
    }

    private static void DrawTabBar(float cx, float cy)
    {
        for (int i = 0; i < TabCount; i++)
        {
            float tw = (WinW - 16f) / (float)TabCount;
            float tx = cx + 8f + (float)i * tw;
            Rect tabRect = new Rect(tx, cy + TitleBarH + 4f, tw, TabH);

            bool hovering = Event.current.type == EventType.Repaint && tabRect.Contains(Event.current.mousePosition);

            if (i == _activeTab)
            {
                GUI.Box(tabRect, GUIContent.none, _tabActiveStyle);
                GUI.Label(tabRect, TabNames[i], _tabActiveStyle);
            }
            else if (hovering)
            {
                GUI.Box(tabRect, GUIContent.none, _tabHoverStyle);
                GUI.Label(tabRect, TabNames[i], _tabHoverStyle);
            }
            else
            {
                GUI.Box(tabRect, GUIContent.none, _tabStyle);
                GUI.Label(tabRect, TabNames[i], _tabStyle);
            }

            if (IsClickInRect(tabRect))
            {
                SwitchTab(i);
                _focusedInputId = -1;
            }
        }
    }

    private static void DrawTabIndicator(float cx, float cy)
    {
        float tw = (WinW - 16f) / (float)TabCount;
        float fromX = cx + 8f + (float)_prevActiveTab * tw;
        float toX = cx + 8f + (float)_activeTab * tw;
        float currentX = Mathf.Lerp(fromX, toX, _tabAnimProgress);
        Color prev = GUI.color;
        GUI.color = Color.white;
        GUI.Box(new Rect(currentX, cy + TitleBarH + 4f + TabH - 3f, tw, 3f), GUIContent.none, _tabIndicatorGUIStyle());
        GUI.color = prev;
    }

    private static GUIStyle _tabIndicatorGUIStyle()
    {
        var s = new GUIStyle();
        s.normal.background = _tabIndicatorTex;
        return s;
    }

    private static void SwitchTab(int newTab)
    {
        if (newTab == _activeTab) return;
        _prevActiveTab = _activeTab;
        _activeTab = newTab;
        _tabSwitchTime = Time.unscaledTime;
        _tabAnimProgress = 0f;
        _scrollPos = Vector2.zero;
    }

    private static void DrawScrollbar(float cx, float cy, float vw, float contentBottom, float maxScroll)
    {
        if (maxScroll > 0f)
        {
            float sbX = cx + 8f + vw - 14f;
            float sbY = cy + ContentTop;
            float sbH = ContentH;
            float sbW = 12f;

            GUI.Box(new Rect(sbX, sbY, sbW, sbH), GUIContent.none, _scrollTrackStyle);

            float thumbH = Mathf.Max(30f, ContentH * (ContentH / contentBottom));
            float thumbY = sbY + (_scrollPos.y / maxScroll) * (sbH - thumbH);

            Rect thumbRect = new Rect(sbX, thumbY, sbW, thumbH);
            bool hovering = Event.current.type == EventType.Repaint && thumbRect.Contains(Event.current.mousePosition);
            Color prev = GUI.color;
            GUI.color = hovering ? new Color(0.6f, 0.62f, 0.7f, 0.7f) : new Color(0.5f, 0.52f, 0.58f, 0.5f);
            GUI.Box(thumbRect, GUIContent.none, _scrollThumbStyle);
            GUI.color = prev;
        }
    }

    private static Texture2D MakeTex(int w, int h, Color color)
    {
        Texture2D tex = new Texture2D(w, h);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }

    private static Texture2D MakeBorderTex(int w, int h, Color color)
    {
        Texture2D tex = new Texture2D(w, h);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, (x == 0 || x == w - 1 || y == 0 || y == h - 1) ? color : Color.clear);
        tex.Apply();
        return tex;
    }

    private static bool IsClickInRect(Rect rect)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
        {
            Event.current.Use();
            return true;
        }
        return false;
    }

    private static bool ClickableColorBtn(Rect rect, string text, Color bgColor, Color hoverColor)
    {
        Color prev = GUI.color;
        bool hovering = Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition);
        GUI.color = hovering ? hoverColor : bgColor;
        GUI.Box(rect, GUIContent.none, _btnStyle);
        GUI.Label(rect, text, _btnStyle);
        GUI.color = prev;
        return IsClickInRect(rect);
    }

    private static bool ToggleBtn(string label, bool current, float x, float y, float w)
    {
        Color prev = GUI.color;
        Rect rect = new Rect(x, y, w, 34f);
        bool hovering = Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition);
        float bright = hovering ? 1.2f : 1f;
        GUI.color = current
            ? new Color(ToggleOnColor.r * bright, ToggleOnColor.g * bright, ToggleOnColor.b * bright, 1f)
            : new Color(ToggleOffColor.r * bright, ToggleOffColor.g * bright, ToggleOffColor.b * bright, 1f);
        GUI.Box(rect, GUIContent.none, _btnStyle);
        GUI.Label(rect, (current ? "ON  " : "OFF  ") + label, _btnStyle);
        GUI.color = prev;
        if (IsClickInRect(rect)) return !current;
        return current;
    }

    private static float CustomSlider(Rect rect, string label, float value, float min, float max, bool wholeNumbers, int id)
    {
        float trackH = 12f;
        float handleSize = 16f;
        float trackY = rect.y + (rect.height - trackH) * 0.5f;
        float trackX = rect.x + 120f;
        float trackW = rect.width - 120f - 60f;

        if (trackW < 20f) trackW = 20f;

        GUI.Label(new Rect(rect.x, rect.y, 116f, rect.height), label, _sectionLabelStyle);

        string valStr = wholeNumbers ? $"{(int)value}" : $"{value:F1}";
        GUI.Label(new Rect(rect.x + rect.width - 56f, rect.y, 52f, rect.height), valStr, _sliderValueStyle);

        Rect trackRect = new Rect(trackX, trackY, trackW, trackH);
        GUI.Box(trackRect, GUIContent.none, _sliderTrackStyle());

        float fillW = ((value - min) / (max - min)) * trackW;
        Rect fillRect = new Rect(trackX, trackY, fillW, trackH);
        GUI.Box(fillRect, GUIContent.none, _sliderFillStyle());

        float handleX = trackX + fillW - handleSize * 0.5f;
        if (handleX < trackX) handleX = trackX;
        if (handleX > trackX + trackW - handleSize) handleX = trackX + trackW - handleSize;
        Rect handleRect = new Rect(handleX, rect.y + (rect.height - handleSize) * 0.5f, handleSize, handleSize);
        Color prev = GUI.color;
        bool hovering = Event.current.type == EventType.Repaint && handleRect.Contains(Event.current.mousePosition);
        GUI.color = hovering || _draggingSliderId == id ? new Color(1f, 1f, 1f, 1f) : new Color(0.85f, 0.85f, 0.85f, 0.9f);
        GUI.Box(handleRect, GUIContent.none, _sliderHandleStyle());
        GUI.color = prev;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (handleRect.Contains(Event.current.mousePosition) || trackRect.Contains(Event.current.mousePosition))
            {
                _draggingSliderId = id;
                float relX = Mathf.Clamp01((Event.current.mousePosition.x - trackX) / trackW);
                float newVal = min + relX * (max - min);
                if (wholeNumbers) newVal = Mathf.Round(newVal);
                value = Mathf.Clamp(newVal, min, max);
                Event.current.Use();
            }
        }

        if (_draggingSliderId == id && Event.current.type == EventType.MouseDrag)
        {
            float relX = Mathf.Clamp01((Event.current.mousePosition.x - trackX) / trackW);
            float newVal = min + relX * (max - min);
            if (wholeNumbers) newVal = Mathf.Round(newVal);
            value = Mathf.Clamp(newVal, min, max);
            Event.current.Use();
        }

        return value;
    }

    private static GUIStyle _sliderTrackStyle()
    {
        var s = new GUIStyle(); s.normal.background = _sliderTrackTex; return s;
    }
    private static GUIStyle _sliderFillStyle()
    {
        var s = new GUIStyle(); s.normal.background = _sliderFillTex; return s;
    }
    private static GUIStyle _sliderHandleStyle()
    {
        var s = new GUIStyle(); s.normal.background = _sliderHandleTex; return s;
    }

    private static GUIStyle _dynStyle(Texture2D tex)
    {
        var s = new GUIStyle(); s.normal.background = tex; return s;
    }

    private static string InputField(Rect rect, string placeholder, string text, int id)
    {
        bool isFocused = _focusedInputId == id;
        GUI.Box(rect, GUIContent.none, _dynStyle(isFocused ? _inputBorderTex : _inputBgTex));

        if (isFocused && Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Backspace && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
            {
                _focusedInputId = -1;
                Event.current.Use();
            }
        }

        if (isFocused && Event.current.type == EventType.KeyDown && Event.current.character != 0 && !char.IsControl(Event.current.character))
        {
            text += Event.current.character;
            Event.current.Use();
        }

        string displayText = text;
        if (isFocused && _cursorVisible) displayText += "|";

        if (string.IsNullOrEmpty(text) && !isFocused)
        {
            Color prev = GUI.color;
            GUI.color = TextMuted;
            GUI.Label(new Rect(rect.x + 8f, rect.y, rect.width - 16f, rect.height), placeholder, _inputTextStyle);
            GUI.color = prev;
        }
        else
        {
            GUI.Label(new Rect(rect.x + 8f, rect.y, rect.width - 16f, rect.height), displayText, _inputTextStyle);
        }

        if (IsClickInRect(rect))
        {
            _focusedInputId = id;
            _cursorBlinkTime = 0f;
            _cursorVisible = true;
        }

        if (isFocused && Event.current.type == EventType.MouseDown && !rect.Contains(Event.current.mousePosition) && _draggingSliderId < 0)
        {
            _focusedInputId = -1;
        }

        return text;
    }

    private static void SectionLabel(string text, ref float sy, float vw)
    {
        float x = 6f;
        float w = vw - 12f;
        GUI.Box(new Rect(x, sy, w, 28f), GUIContent.none, _dynStyle(_sectionBgTex));
        GUI.Box(new Rect(x, sy, 3f, 28f), GUIContent.none, _dynStyle(_accentLineTex));
        GUI.Label(new Rect(x + 12f, sy, w - 12f, 28f), text, _sectionLabelStyle);
        sy += 36f;
    }

    private static void DestroyStyles()
    {
        _stylesInited = false;
        if (_winBgTex != null) { UnityEngine.Object.DestroyImmediate(_winBgTex); _winBgTex = null; }
        if (_winBorderTex != null) { UnityEngine.Object.DestroyImmediate(_winBorderTex); _winBorderTex = null; }
        if (_tabBgTex != null) { UnityEngine.Object.DestroyImmediate(_tabBgTex); _tabBgTex = null; }
        if (_tabActiveBgTex != null) { UnityEngine.Object.DestroyImmediate(_tabActiveBgTex); _tabActiveBgTex = null; }
        if (_tabHoverBgTex != null) { UnityEngine.Object.DestroyImmediate(_tabHoverBgTex); _tabHoverBgTex = null; }
        if (_tabIndicatorTex != null) { UnityEngine.Object.DestroyImmediate(_tabIndicatorTex); _tabIndicatorTex = null; }
        if (_btnBgTex != null) { UnityEngine.Object.DestroyImmediate(_btnBgTex); _btnBgTex = null; }
        if (_sectionBgTex != null) { UnityEngine.Object.DestroyImmediate(_sectionBgTex); _sectionBgTex = null; }
        if (_accentLineTex != null) { UnityEngine.Object.DestroyImmediate(_accentLineTex); _accentLineTex = null; }
        if (_scrollTrackTex != null) { UnityEngine.Object.DestroyImmediate(_scrollTrackTex); _scrollTrackTex = null; }
        if (_scrollThumbTex != null) { UnityEngine.Object.DestroyImmediate(_scrollThumbTex); _scrollThumbTex = null; }
        if (_scrollThumbHoverTex != null) { UnityEngine.Object.DestroyImmediate(_scrollThumbHoverTex); _scrollThumbHoverTex = null; }
        if (_sliderTrackTex != null) { UnityEngine.Object.DestroyImmediate(_sliderTrackTex); _sliderTrackTex = null; }
        if (_sliderFillTex != null) { UnityEngine.Object.DestroyImmediate(_sliderFillTex); _sliderFillTex = null; }
        if (_sliderHandleTex != null) { UnityEngine.Object.DestroyImmediate(_sliderHandleTex); _sliderHandleTex = null; }
        if (_inputBgTex != null) { UnityEngine.Object.DestroyImmediate(_inputBgTex); _inputBgTex = null; }
        if (_inputBorderTex != null) { UnityEngine.Object.DestroyImmediate(_inputBorderTex); _inputBorderTex = null; }
        _windowBgStyle = null;
        _windowBorderStyle = null;
        _tabStyle = null;
        _tabActiveStyle = null;
        _tabHoverStyle = null;
        _btnStyle = null;
        _sectionLabelStyle = null;
        _scrollThumbStyle = null;
        _scrollTrackStyle = null;
        _titleStyle = null;
        _sliderValueStyle = null;
        _inputTextStyle = null;
    }

    public static void Cleanup()
    {
        DestroyStyles();
    }

    private static void EnsureStyles()
    {
        if (_stylesInited) return;

        Color windowBg = new Color(0.08f, 0.1f, 0.15f, 1f);
        Color windowBg2 = new Color(0.12f, 0.14f, 0.2f, 1f);
        Color borderColor = new Color(0.2f, 0.22f, 0.28f, 1f);
        Color tabColor = new Color(0.18f, 0.2f, 0.26f, 1f);
        Color tabActiveColor = new Color(0.271f, 0.477f, 0.66f, 1f);
        Color tabHoverColor = new Color(0.22f, 0.24f, 0.3f, 1f);
        Color indicatorColor = new Color(0.271f, 0.477f, 0.66f, 1f);
        Color btnNormal = new Color(0.28f, 0.3f, 0.36f, 1f);

        _winBgTex = MakeTex(1, 1, windowBg);
        _windowBgStyle = new GUIStyle();
        _windowBgStyle.normal.background = _winBgTex;

        _winBorderTex = MakeBorderTex(1, 1, borderColor);
        _windowBorderStyle = new GUIStyle();
        _windowBorderStyle.normal.background = _winBorderTex;

        _tabBgTex = MakeTex(1, 1, tabColor);
        _tabStyle = new GUIStyle();
        _tabStyle.normal.background = _tabBgTex;
        _tabStyle.normal.textColor = new Color(0.75f, 0.77f, 0.82f, 1f);
        _tabStyle.fontSize = 14;
        _tabStyle.alignment = TextAnchor.MiddleCenter;
        _tabStyle.padding = new RectOffset(4, 4, 4, 4);

        _tabActiveBgTex = MakeTex(1, 1, tabActiveColor);
        _tabActiveStyle = new GUIStyle();
        _tabActiveStyle.normal.background = _tabActiveBgTex;
        _tabActiveStyle.normal.textColor = Color.white;
        _tabActiveStyle.fontSize = 14;
        _tabActiveStyle.alignment = TextAnchor.MiddleCenter;
        _tabActiveStyle.padding = new RectOffset(4, 4, 4, 4);

        _tabHoverBgTex = MakeTex(1, 1, tabHoverColor);
        _tabHoverStyle = new GUIStyle();
        _tabHoverStyle.normal.background = _tabHoverBgTex;
        _tabHoverStyle.normal.textColor = new Color(0.9f, 0.92f, 0.95f, 1f);
        _tabHoverStyle.fontSize = 14;
        _tabHoverStyle.alignment = TextAnchor.MiddleCenter;
        _tabHoverStyle.padding = new RectOffset(4, 4, 4, 4);

        _tabIndicatorTex = MakeTex(1, 1, indicatorColor);

        _btnBgTex = MakeTex(1, 1, btnNormal);
        _btnStyle = new GUIStyle();
        _btnStyle.normal.background = _btnBgTex;
        _btnStyle.normal.textColor = new Color(0.95f, 0.96f, 0.98f, 1f);
        _btnStyle.fontSize = 13;
        _btnStyle.alignment = TextAnchor.MiddleCenter;
        _btnStyle.padding = new RectOffset(6, 6, 3, 3);

        _sectionBgTex = MakeTex(1, 1, SectionBgColor);
        _sectionLabelStyle = new GUIStyle();
        _sectionLabelStyle.normal.textColor = new Color(0.65f, 0.8f, 0.95f, 1f);
        _sectionLabelStyle.fontSize = 14;
        _sectionLabelStyle.alignment = TextAnchor.MiddleLeft;
        _sectionLabelStyle.fontStyle = FontStyle.Bold;
        _sectionLabelStyle.padding = new RectOffset(6, 4, 2, 2);

        _titleStyle = new GUIStyle();
        _titleStyle.normal.textColor = new Color(0.6f, 0.75f, 0.95f, 1f);
        _titleStyle.fontSize = 16;
        _titleStyle.alignment = TextAnchor.MiddleLeft;
        _titleStyle.fontStyle = FontStyle.Bold;

        _sliderValueStyle = new GUIStyle();
        _sliderValueStyle.normal.textColor = AccentBlue;
        _sliderValueStyle.fontSize = 14;
        _sliderValueStyle.alignment = TextAnchor.MiddleRight;
        _sliderValueStyle.fontStyle = FontStyle.Bold;

        _inputTextStyle = new GUIStyle();
        _inputTextStyle.normal.textColor = TextLight;
        _inputTextStyle.fontSize = 13;
        _inputTextStyle.alignment = TextAnchor.MiddleLeft;
        _inputTextStyle.padding = new RectOffset(8, 8, 2, 2);

        _scrollTrackTex = MakeTex(1, 1, new Color(0.1f, 0.12f, 0.17f, 0.5f));
        _scrollTrackStyle = new GUIStyle();
        _scrollTrackStyle.normal.background = _scrollTrackTex;

        _scrollThumbTex = MakeTex(1, 1, new Color(0.5f, 0.52f, 0.58f, 0.5f));
        _scrollThumbStyle = new GUIStyle();
        _scrollThumbStyle.normal.background = _scrollThumbTex;

        _sliderTrackTex = MakeTex(1, 1, SliderTrackColor);
        _sliderFillTex = MakeTex(1, 1, SliderFillColor);
        _sliderHandleTex = MakeTex(1, 1, SliderHandleColor);

        _inputBgTex = MakeTex(1, 1, InputBgColor);
        _inputBorderTex = MakeTex(1, 1, InputBorderColor);
        _accentLineTex = MakeTex(1, 1, AccentBlue);

        _stylesInited = true;
    }

    private static void DrawMoneyTab(ref float sy, float vw)
    {
        SectionLabel("QUICK ADD MONEY", ref sy, vw);
        float bw = (vw - 28f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "$1K", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { MoneyModule.AddMoney(1000f); ToastNotification.Show("Added $1,000"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "$5K", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { MoneyModule.AddMoney(5000f); ToastNotification.Show("Added $5,000"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "$10K", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { MoneyModule.AddMoney(10000f); ToastNotification.Show("Added $10,000"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "$50K", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { MoneyModule.AddMoney(50000f); ToastNotification.Show("Added $50,000"); }
        sy += 42f;

        bw = (vw - 20f) / 3f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "$100K", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { MoneyModule.AddMoney(100000f); ToastNotification.Show("Added $100,000"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "$500K", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { MoneyModule.AddMoney(500000f); ToastNotification.Show("Added $500,000"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "$1M", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { MoneyModule.AddMoney(1000000f); ToastNotification.Show("Added $1,000,000"); }
        sy += 42f;

        SectionLabel("CUSTOM MONEY", ref sy, vw);
        float iw = vw - 12f - 120f;
        _inputTexts[0] = InputField(new Rect(6f, sy, iw, 34f), "Enter amount...", _inputTexts[0] ?? "", 0);
        if (ClickableColorBtn(new Rect(10f + iw, sy, 56f, 34f), "Add", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f)))
        {
            if (float.TryParse(_inputTexts[0], out var amt)) { MoneyModule.AddMoney(amt); ToastNotification.Show($"Added ${amt:N0}"); _inputTexts[0] = ""; }
            else ToastNotification.Show("Invalid amount");
        }
        if (ClickableColorBtn(new Rect(70f + iw, sy, 56f, 34f), "Set", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f)))
        {
            if (float.TryParse(_inputTexts[0], out var amt)) { MoneyModule.SetMoney(amt); ToastNotification.Show($"Money set to ${amt:N0}"); _inputTexts[0] = ""; }
            else ToastNotification.Show("Invalid amount");
        }
        sy += 42f;

        SectionLabel("ECONOMY", ref sy, vw);
        MoneyModule.ApplyTaxPercentage((int)CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Tax %", MoneyModule.TaxPercentage, 0f, 100f, true, 10));
        sy += 36f;
        MoneyModule.MarketPriceMultiplier = CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Price Mult", MoneyModule.MarketPriceMultiplier, 0.1f, 5f, false, 11);
        MoneyModule.ApplyMarketPriceMultiplier(MoneyModule.MarketPriceMultiplier);
        sy += 36f;
        MoneyModule.ExportMultiplier = CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Export Mult", MoneyModule.ExportMultiplier, 0.1f, 10f, false, 12);
        MoneyModule.ApplyExportMultiplier(MoneyModule.ExportMultiplier);
        sy += 42f;
        sy += 8f;
    }

    private static void DrawPlayerTab(ref float sy, float vw)
    {
        SectionLabel("NEEDS & STATS", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Fill All Needs", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.FillAllNeeds(); ToastNotification.Show("All needs filled!"); }
        sy += 42f;

        SectionLabel("ENERGY", ref sy, vw);
        float energy = PlayerStatsModule.CurrentEnergy;
        float newEnergy = CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Level", energy, 0f, 100f, true, 20);
        if (Math.Abs(newEnergy - energy) > 0.01f) PlayerStatsModule.SetEnergy(newEnergy);
        sy += 36f;
        float bw = (vw - 28f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "25", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { PlayerStatsModule.SetEnergy(25f); ToastNotification.Show("Energy: 25"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "50", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { PlayerStatsModule.SetEnergy(50f); ToastNotification.Show("Energy: 50"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "75", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { PlayerStatsModule.SetEnergy(75f); ToastNotification.Show("Energy: 75"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "100", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.SetEnergy(100f); ToastNotification.Show("Energy: 100"); }
        sy += 42f;

        SectionLabel("HAPPINESS", ref sy, vw);
        bw = (vw - 32f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "-25", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { PlayerStatsModule.ChangeHappiness(-25); ToastNotification.Show("Happiness -25"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "-10", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { PlayerStatsModule.ChangeHappiness(-10); ToastNotification.Show("Happiness -10"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "+10", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.ChangeHappiness(10); ToastNotification.Show("Happiness +10"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "+25", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.ChangeHappiness(25); ToastNotification.Show("Happiness +25"); }
        sy += 42f;

        SectionLabel("HUNGER", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "-25", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { PlayerStatsModule.ChangeHunger(-25); ToastNotification.Show("Hunger -25"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "-10", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { PlayerStatsModule.ChangeHunger(-10); ToastNotification.Show("Hunger -10"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "+10", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.ChangeHunger(10); ToastNotification.Show("Hunger +10"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "+25", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.ChangeHunger(25); ToastNotification.Show("Hunger +25"); }
        sy += 42f;

        SectionLabel("MOVEMENT SPEED", ref sy, vw);
        bw = (vw - 28f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Walk", new Color(0.3f, 0.32f, 0.38f, 1f), new Color(0.4f, 0.42f, 0.48f, 1f))) { PlayerStatsModule.SetPlayerSpeed(0); ToastNotification.Show("Speed: Walk"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Jog", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { PlayerStatsModule.SetPlayerSpeed(1); ToastNotification.Show("Speed: Jog"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "Run", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { PlayerStatsModule.SetPlayerSpeed(2); ToastNotification.Show("Speed: Run"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "Scooter", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.SetPlayerSpeed(3); ToastNotification.Show("Speed: Scooter"); }
        sy += 42f;

        SectionLabel("TOGGLES", ref sy, vw);
        bw = (vw - 12f) / 2f;
        TrainerConfig.DisableEnergy = ToggleBtn("Energy Decay", TrainerConfig.DisableEnergy, 6f, sy, bw);
        TrainerConfig.DisableHappiness = ToggleBtn("Happy Decay", TrainerConfig.DisableHappiness, 10f + bw, sy, bw);
        sy += 42f;
        TrainerConfig.DisableHunger = ToggleBtn("Hunger Decay", TrainerConfig.DisableHunger, 6f, sy, bw);
        TrainerConfig.DisableAging = ToggleBtn("Aging", TrainerConfig.DisableAging, 10f + bw, sy, bw);
        sy += 42f;

        SectionLabel("AGE", ref sy, vw);
        bw = (vw - 32f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "-5 Years", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { PlayerStatsModule.ChangeAge(-5f); ToastNotification.Show("Age -5"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "-1 Year", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { PlayerStatsModule.ChangeAge(-1f); ToastNotification.Show("Age -1"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "+1 Year", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { PlayerStatsModule.ChangeAge(1f); ToastNotification.Show("Age +1"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "+5 Years", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { PlayerStatsModule.ChangeAge(5f); ToastNotification.Show("Age +5"); }
        sy += 42f;

        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Complete All Personal Goals", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { PlayerStatsModule.CompletePersonalGoals(); ToastNotification.Show("Goals completed!"); }
        sy += 42f;
        sy += 8f;
    }

    private static void DrawVehicleTab(ref float sy, float vw)
    {
        SectionLabel("TOGGLES", ref sy, vw);
        float bw = (vw - 12f) / 2f;
        TrainerConfig.DisableVehicleDamage = ToggleBtn("Vehicle Damage", TrainerConfig.DisableVehicleDamage, 6f, sy, bw);
        TrainerConfig.DisableVehicleFuel = ToggleBtn("Vehicle Fuel", TrainerConfig.DisableVehicleFuel, 10f + bw, sy, bw);
        sy += 42f;

        SectionLabel("ACTIONS", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Repair", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { VehicleModule.RepairVehicle(); ToastNotification.Show("Vehicle repaired!"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Refuel", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { VehicleModule.RefuelVehicle(); ToastNotification.Show("Vehicle refueled!"); }
        sy += 42f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Clean", new Color(0.3f, 0.32f, 0.38f, 1f), new Color(0.4f, 0.42f, 0.48f, 1f))) { VehicleModule.CleanVehicle(); ToastNotification.Show("Vehicle cleaned!"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Clear Tickets", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { VehicleModule.ClearParkingTickets(); ToastNotification.Show("Tickets cleared!"); }
        sy += 42f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Tow to Gas", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { VehicleModule.TowToGasStation(); ToastNotification.Show("Towing to gas..."); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Tow to Repair", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { VehicleModule.TowToAutoRepair(); ToastNotification.Show("Towing to repair..."); }
        sy += 42f;
        sy += 8f;
    }

    private static void DrawBusinessTab(ref float sy, float vw)
    {
        SectionLabel("CUSTOMER SATISFACTION", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Max All Customer Satisfaction", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { BusinessModule.MaxAllSatisfaction(); ToastNotification.Show("All satisfaction maxed!"); }
        sy += 42f;

        SectionLabel("UNLOCKS & TOGGLES", ref sy, vw);
        float bw = (vw - 12f) / 2f;
        TrainerConfig.AllCoursesUnlocked = ToggleBtn("All Courses", TrainerConfig.AllCoursesUnlocked, 6f, sy, bw);
        TrainerConfig.AllContactsUnlocked = ToggleBtn("All Contacts", TrainerConfig.AllContactsUnlocked, 10f + bw, sy, bw);
        sy += 42f;
        TrainerConfig.DisableWholesaleImportLimits = ToggleBtn("No Import Limits", TrainerConfig.DisableWholesaleImportLimits, 6f, sy, bw);
        TrainerConfig.AllProductsFromImporters = ToggleBtn("All Import Products", TrainerConfig.AllProductsFromImporters, 10f + bw, sy, bw);
        sy += 42f;

        SectionLabel("BUSINESS MULTIPLIERS", ref sy, vw);
        BusinessModule.ApplyCustomerPromotionMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Promotion", BusinessModule.CustomerPromotionMultiplier, 0.1f, 10f, false, 30));
        sy += 36f;
        BusinessModule.ApplyEmployeeSalaryMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Salary", BusinessModule.EmployeeSalaryMultiplier, 0f, 5f, false, 31));
        sy += 36f;
        BusinessModule.ApplyBankInterestRate(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Interest %", BusinessModule.BankInterestRate, 0f, 50f, true, 32));
        sy += 36f;
        BusinessModule.ApplyRivalsDifficultyMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Rivals", BusinessModule.RivalsDifficultyMultiplier, 0f, 5f, false, 33));
        sy += 36f;
        BusinessModule.ApplyWholesaleUrgentFeeMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Wholesale Fee", BusinessModule.WholesaleUrgentFeeMultiplier, 0f, 5f, false, 34));
        sy += 36f;
        BusinessModule.ApplyImporterUrgentFeeMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Importer Fee", BusinessModule.ImporterUrgentFeeMultiplier, 0f, 5f, false, 35));
        sy += 42f;
        sy += 8f;
    }

    private static void DrawGameplayTab(ref float sy, float vw)
    {
        SectionLabel("GAME SPEED", ref sy, vw);
        GameplayModule.SetGameSpeed(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Speed", GameplayModule.GameSpeed, 0f, 10f, false, 40));
        sy += 36f;
        float bw = (vw - 32f) / 5f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Pause", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { GameplayModule.SetGameSpeed(0f); ToastNotification.Show("Paused"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "1x", new Color(0.3f, 0.32f, 0.38f, 1f), new Color(0.4f, 0.42f, 0.48f, 1f))) { GameplayModule.SetGameSpeed(1f); ToastNotification.Show("Speed: 1x"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "2x", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.SetGameSpeed(2f); ToastNotification.Show("Speed: 2x"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "5x", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { GameplayModule.SetGameSpeed(5f); ToastNotification.Show("Speed: 5x"); }
        if (ClickableColorBtn(new Rect(22f + bw * 4f, sy, bw, 34f), "10x", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { GameplayModule.SetGameSpeed(10f); ToastNotification.Show("Speed: 10x"); }
        sy += 42f;

        SectionLabel("TIME CONTROLS", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Skip to Next Day", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.SkipToNextDay(); ToastNotification.Show("Skipped to next day"); }
        sy += 42f;

        bw = (vw - 28f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "6 AM", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { GameplayModule.SetTimeOfDay(6, 0); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "12 PM", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.SetTimeOfDay(12, 0); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "6 PM", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { GameplayModule.SetTimeOfDay(18, 0); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "10 PM", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.SetTimeOfDay(22, 0); }
        sy += 42f;

        SectionLabel("SET CUSTOM TIME", ref sy, vw);
        float iw1 = (vw - 28f) / 3f;
        _inputTexts[1] = InputField(new Rect(6f, sy, iw1, 34f), "Hour (0-23)", _inputTexts[1] ?? "", 1);
        _inputTexts[2] = InputField(new Rect(10f + iw1, sy, iw1, 34f), "Min (0-59)", _inputTexts[2] ?? "", 2);
        if (ClickableColorBtn(new Rect(14f + iw1 * 2f, sy, iw1, 34f), "Set Time", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f)))
        {
            if (int.TryParse(_inputTexts[1], out var h) && int.TryParse(_inputTexts[2], out var m) && h >= 0 && h <= 23 && m >= 0 && m <= 59)
            { GameplayModule.SetTimeOfDay(h, m); ToastNotification.Show($"Time set to {h:D2}:{m:D2}"); _inputTexts[1] = ""; _inputTexts[2] = ""; }
            else ToastNotification.Show("Invalid time (hour 0-23, min 0-59)");
        }
        sy += 42f;

        SectionLabel("TOGGLES", ref sy, vw);
        bw = (vw - 12f) / 2f;
        TrainerConfig.DisableTraffic = ToggleBtn("Traffic", TrainerConfig.DisableTraffic, 6f, sy, bw);
        TrainerConfig.Invincibility = ToggleBtn("Invincibility", TrainerConfig.Invincibility, 10f + bw, sy, bw);
        sy += 42f;
        TrainerConfig.DisableTutorial = ToggleBtn("Tutorial", TrainerConfig.DisableTutorial, 6f, sy, bw);
        sy += 42f;

        SectionLabel("QUESTS & CONTACTS", ref sy, vw);
        bw = (vw - 12f) / 2f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Complete Quest", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { GameplayModule.CompleteQuest(); ToastNotification.Show("Quest completed!"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Complete Objective", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.CompleteObjective(); ToastNotification.Show("Objective completed!"); }
        sy += 42f;
        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Unlock All Contacts", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.UnlockAllContacts(); ToastNotification.Show("Contacts unlocked!"); }
        sy += 42f;

        SectionLabel("TELEPORTATION", ref sy, vw);
        bw = (vw - 12f) / 2f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "To Quest Target", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.TeleportToQuestTarget(); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "To Destination", new Color(0.3f, 0.32f, 0.38f, 1f), new Color(0.4f, 0.42f, 0.48f, 1f))) { GameplayModule.TeleportToDestination(); }
        sy += 42f;

        SectionLabel("IMPORT DELIVERIES", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Deliver All (Paid)", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { GameplayModule.DeliverAllImportsPaid(); ToastNotification.Show("Imports delivered (paid)"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Deliver All (Free)", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { GameplayModule.DeliverAllImportsFree(); ToastNotification.Show("Imports delivered (free)"); }
        sy += 42f;

        SectionLabel("BANK INTEREST", ref sy, vw);
        WorldModule.ApplyBankInterestMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Interest Mult", WorldModule.BankInterestMultiplier, 0f, 5f, false, 41));
        sy += 42f;

        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Save Game (TrainerSave)", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { GameplayModule.SaveGame(); ToastNotification.Show("Game saved!"); }
        sy += 42f;
        sy += 8f;
    }

    private static void DrawStaffTab(ref float sy, float vw)
    {
        SectionLabel("BULK ACTIONS", ref sy, vw);
        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 34f), "Max ALL Employee Satisfaction", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { EmployeeModule.MaxAllSatisfaction(); ToastNotification.Show("All satisfaction maxed!"); }
        sy += 42f;

        SectionLabel("SALARY MULTIPLIER", ref sy, vw);
        EmployeeModule.ApplySalaryMultiplier(CustomSlider(new Rect(6f, sy, vw - 12f, 30f), "Mult", EmployeeModule.SalaryMultiplier, 0f, 5f, false, 50));
        sy += 36f;
        float bw = (vw - 28f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Free", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { EmployeeModule.ApplySalaryMultiplier(0f); ToastNotification.Show("Salary: Free"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "0.5x", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.ApplySalaryMultiplier(0.5f); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "1x", new Color(0.3f, 0.32f, 0.38f, 1f), new Color(0.4f, 0.42f, 0.48f, 1f))) { EmployeeModule.ApplySalaryMultiplier(1f); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "2x", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { EmployeeModule.ApplySalaryMultiplier(2f); }
        sy += 42f;

        SectionLabel("SET WAGES", ref sy, vw);
        float iw = vw - 12f - 130f;
        _inputTexts[3] = InputField(new Rect(6f, sy, iw, 34f), "e.g. 15.00", _inputTexts[3] ?? "", 3);
        if (ClickableColorBtn(new Rect(10f + iw, sy, 120f, 34f), "Set All Wages", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f)))
        {
            if (float.TryParse(_inputTexts[3], out var wage)) { EmployeeModule.SetAllWages(wage); ToastNotification.Show($"All wages set to ${wage:F2}/hr"); _inputTexts[3] = ""; }
            else ToastNotification.Show("Invalid wage amount");
        }
        sy += 42f;

        SectionLabel("RECRUITMENT CANDIDATES", ref sy, vw);
        float iw2 = vw - 12f - 100f;
        _inputTexts[4] = InputField(new Rect(6f, sy, iw2, 34f), $"Skill Level (1-100) [Current: {EmployeeModule.CandidateSkillLevel}]", _inputTexts[4] ?? "", 4);
        if (ClickableColorBtn(new Rect(10f + iw2, sy, 90f, 34f), "Set", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f)))
        {
            if (int.TryParse(_inputTexts[4], out var level) && level >= 1 && level <= 100)
            { EmployeeModule.CandidateSkillLevel = level; ToastNotification.Show($"Skill level set to {level}"); _inputTexts[4] = ""; }
            else ToastNotification.Show("Enter 1-100");
        }
        sy += 42f;

        bw = (vw - 12f) / 2f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "CustService", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(0, EmployeeModule.CandidateSkillLevel); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Cleaning", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(1, EmployeeModule.CandidateSkillLevel); }
        sy += 42f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Lawyer", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(2, EmployeeModule.CandidateSkillLevel); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Purchasing", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(3, EmployeeModule.CandidateSkillLevel); }
        sy += 42f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Logistics", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(4, EmployeeModule.CandidateSkillLevel); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Delivery", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(5, EmployeeModule.CandidateSkillLevel); }
        sy += 42f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Programmer", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(6, EmployeeModule.CandidateSkillLevel); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "HR Manager", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { EmployeeModule.GenerateCandidate(7, EmployeeModule.CandidateSkillLevel); }
        sy += 42f;
        sy += 8f;
    }

    private static void DrawRivalsTab(ref float sy, float vw)
    {
        SectionLabel("RIVAL ACTIONS", ref sy, vw);
        float bw = (vw - 12f) / 2f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Refresh Rivals Data", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { RivalsModule.RefreshRivals(); ToastNotification.Show("Rivals refreshed!"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Defeat ALL Rivals", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { RivalsModule.DefeatAllRivals(); ToastNotification.Show("All rivals defeated!"); }
        sy += 42f;

        SectionLabel("RIVALS DIFFICULTY", ref sy, vw);
        bw = (vw - 28f) / 4f;
        if (ClickableColorBtn(new Rect(6f, sy, bw, 34f), "Easy (0.5x)", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f))) { BusinessModule.ApplyRivalsDifficultyMultiplier(0.5f); ToastNotification.Show("Rivals: Easy"); }
        if (ClickableColorBtn(new Rect(10f + bw, sy, bw, 34f), "Normal (1x)", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f))) { BusinessModule.ApplyRivalsDifficultyMultiplier(1f); ToastNotification.Show("Rivals: Normal"); }
        if (ClickableColorBtn(new Rect(14f + bw * 2f, sy, bw, 34f), "Hard (2x)", AccentOrange, new Color(1f, 0.68f, 0.25f, 1f))) { BusinessModule.ApplyRivalsDifficultyMultiplier(2f); ToastNotification.Show("Rivals: Hard"); }
        if (ClickableColorBtn(new Rect(18f + bw * 3f, sy, bw, 34f), "Brutal (5x)", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { BusinessModule.ApplyRivalsDifficultyMultiplier(5f); ToastNotification.Show("Rivals: Brutal"); }
        sy += 42f;
        sy += 8f;
    }

    private static void DrawSettingsTab(ref float sy, float vw)
    {
        SectionLabel("SETTINGS", ref sy, vw);
        float halfW = (vw - 16f) / 2f;
        if (ClickableColorBtn(new Rect(6f, sy, halfW, 38f), "Save All Settings", AccentGreen, new Color(0.3f, 0.85f, 0.45f, 1f)))
        {
            TrainerConfig.Save();
            ToastNotification.Show("Settings saved!");
        }
        if (ClickableColorBtn(new Rect(10f + halfW, sy, halfW, 38f), "Load All Settings", AccentBlue, new Color(0.37f, 0.58f, 0.75f, 1f)))
        {
            TrainerConfig.Load();
            ToastNotification.Show("Settings loaded!");
        }
        sy += 48f;

        SectionLabel("INTEGRATION", ref sy, vw);
        TrainerConfig.PhoneIntegration = ToggleBtn("Show Trainer in Phone", TrainerConfig.PhoneIntegration, 6f, sy, vw - 12f);
        sy += 44f;

        SectionLabel("ABOUT", ref sy, vw);
        float infoX = 12f;
        GUI.Label(new Rect(infoX, sy, vw - 24f, 24f), "ItzRealOzone Trainer v1.0.1", _titleStyle);
        sy += 28f;
        GUI.Label(new Rect(infoX, sy, vw - 24f, 24f), "Press F8 to toggle  |  Press ESC to close", _sectionLabelStyle);
        sy += 28f;
        Color saveC = GUI.color;
        GUI.color = TextMuted;
        GUI.Label(new Rect(infoX, sy, vw - 24f, 24f), "Made by ItzRealOzone", _sectionLabelStyle);
        GUI.color = saveC;
        sy += 28f;

        if (ClickableColorBtn(new Rect(6f, sy, vw - 12f, 38f), "Close Overlay", AccentRed, new Color(0.95f, 0.32f, 0.32f, 1f))) { _closing = true; _animStartTime = Time.unscaledTime; }
        sy += 48f;
    }
}
