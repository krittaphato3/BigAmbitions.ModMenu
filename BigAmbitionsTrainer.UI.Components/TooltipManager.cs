using UnityEngine;

namespace BigAmbitionsTrainer.UI.Components;

public static class TooltipManager
{
	private static string _activeTooltip;
	private static GUIStyle _tooltipStyle;
	private static Texture2D _tooltipBg;

	public static void Show(string text)
	{
		_activeTooltip = text;
	}

	public static void Hide()
	{
		_activeTooltip = null;
	}

	public static void OnGUI()
	{
		if (_activeTooltip == null)
		{
			return;
		}
		if (_tooltipStyle == null)
		{
			_tooltipBg = new Texture2D(1, 1);
			_tooltipBg.SetPixel(0, 0, new Color(0.1f, 0.12f, 0.16f, 0.95f));
			_tooltipBg.Apply();
			_tooltipStyle = new GUIStyle();
			_tooltipStyle.normal.background = _tooltipBg;
			_tooltipStyle.normal.textColor = Color.white;
			_tooltipStyle.fontSize = 13;
			_tooltipStyle.padding = new RectOffset(8, 8, 4, 4);
			_tooltipStyle.alignment = (TextAnchor)3;
			_tooltipStyle.wordWrap = true;
			_tooltipStyle.clipping = (TextClipping)0;
		}
		GUIContent content = new GUIContent(_activeTooltip);
		Vector2 vector = _tooltipStyle.CalcSize(content);
		Vector2 mousePosition = Event.current.mousePosition;
		float num = mousePosition.x + 15f;
		float num2 = mousePosition.y - vector.y - 5f;
		if (num + vector.x > (float)Screen.width)
		{
			num = (float)Screen.width - vector.x - 5f;
		}
		if (num2 < 0f)
		{
			num2 = mousePosition.y + 15f;
		}
		GUI.Box(new Rect(num, num2, vector.x + 6f, vector.y + 4f), _activeTooltip, _tooltipStyle);
	}
}
