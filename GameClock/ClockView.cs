using TMPro;
using UnityEngine;

namespace GameClock
{
    public class ClockView : MonoBehaviour
    {
        private static bool _loggedFont;

        private TextMeshProUGUI _text;
        private float _sourceFontSize;
        private float _timer;
        private bool _force;
        private bool _wasReady;
        private string _shown = "";
        private bool _hasMapPoint;
        private Vector2 _mapPoint;
        private float _mapWidth = 220f;

        internal static void RequestRefresh()
        {
            ClockView view = FindView();
            if (view != null)
            {
                view._force = true;
            }
        }

        internal static void Attach(Hud hud)
        {
            if (hud == null || hud.m_rootObject == null)
            {
                return;
            }

            TMP_Text source = hud.m_healthText;
            if (source == null || source.font == null)
            {
                if (!_loggedFont)
                {
                    _loggedFont = true;
                    Plugin.LogWarning("HUD health text is missing. Game Clock label skipped.");
                }

                return;
            }

            if (hud.m_rootObject.transform.Find("GameClock") != null)
            {
                return;
            }

            GameObject go = new GameObject("GameClock");
            go.SetActive(false);
            go.transform.SetParent(hud.m_rootObject.transform, false);
            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.font = source.font;
            text.fontSharedMaterial = source.fontSharedMaterial;
            text.fontSize = source.fontSize;
            text.fontStyle = source.fontStyle;
            text.color = source.color;
            text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.alignment = TextAlignmentOptions.Top;
            text.lineSpacing = 0f;
            text.text = "";
            text.rectTransform.sizeDelta = new Vector2(220f, 48f);
            ClockView view = go.AddComponent<ClockView>();
            view._text = text;
            view._sourceFontSize = source.fontSize;
            go.SetActive(true);
        }

        private void Update()
        {
            bool ready = IsReady();
            _text.enabled = ready;
            if (!ready)
            {
                _wasReady = false;
                return;
            }

            ApplyPlacement();
            _timer += Time.unscaledDeltaTime;
            if (!_wasReady || _force || _timer >= 1f)
            {
                _wasReady = true;
                _force = false;
                _timer = 0f;
                string next = BuildText();
                if (next != _shown)
                {
                    _shown = next;
                    _text.text = next;
                }

                FitToMap();
            }

            SnapUnderMap();
        }

        private static bool IsReady()
        {
            if (EnvMan.instance == null || Player.m_localPlayer == null)
            {
                return false;
            }

            Hud hud = Hud.instance;
            if (hud == null || hud.m_rootObject == null || !hud.m_rootObject.activeInHierarchy)
            {
                return false;
            }

            if (!ClockText.IsTimeReady(EnvMan.instance.GetDayFraction()))
            {
                return false;
            }

            return ClockText.ShouldShow(MapIsOpen(), SmallMapActive(), Plugin.Corner.Value);
        }

        private static bool MapIsOpen()
        {
            return Minimap.instance != null && Minimap.IsOpen();
        }

        private static bool SmallMapActive()
        {
            Minimap map = Minimap.instance;
            return map != null && map.m_smallRoot != null && map.m_smallRoot.activeInHierarchy;
        }

        private string BuildText()
        {
            EnvMan env = EnvMan.instance;
            string weather = "";
            EnvSetup current = env.GetCurrentEnvironment();
            if (current != null && !string.IsNullOrEmpty(current.m_name))
            {
                weather = Localization.instance != null
                    ? Localization.instance.Localize(current.m_name)
                    : current.m_name;
            }

            Vector3 wind = env.GetWindDir();
            ClockSnapshot snapshot = new ClockSnapshot
            {
                DayFraction = env.GetDayFraction(),
                Day = env.GetDay(),
                Weather = weather,
                WindX = wind.x,
                WindZ = wind.z,
                WindIntensity = env.GetWindIntensity(),
                IsCold = EnvMan.IsCold(),
                IsWet = EnvMan.IsWet(),
                IsFreezing = EnvMan.IsFreezing()
            };
            return ClockText.Format(snapshot, Plugin.CurrentOptions());
        }

        private void ApplyPlacement()
        {
            string corner = ClockText.NormalizeCorner(Plugin.Corner.Value);
            float offsetX = Plugin.OffsetX.Value;
            float offsetY = Plugin.OffsetY.Value;
            RectTransform rect = _text.rectTransform;
            if (TryGetMap(out Vector2 mapPoint, out float measuredWidth))
            {
                _mapPoint = mapPoint;
                _hasMapPoint = true;
                _mapWidth = measuredWidth;
            }

            float width = Mathf.Max(40f, _mapWidth - 8f);
            ApplyWidth(rect, width);
            if (corner == "TopLeft")
            {
                SetCorner(rect, 0f, TextAlignmentOptions.TopLeft);
                rect.anchoredPosition = new Vector2(24f + offsetX, -24f + offsetY);
                return;
            }

            if (corner == "TopCenter")
            {
                SetCorner(rect, 0.5f, TextAlignmentOptions.Top);
                rect.anchoredPosition = new Vector2(offsetX, -24f + offsetY);
                return;
            }

            SetCorner(rect, 1f, TextAlignmentOptions.Top);
            if (_hasMapPoint)
            {
                rect.anchoredPosition = _mapPoint + new Vector2(offsetX - 4f, offsetY);
                return;
            }

            rect.anchoredPosition = new Vector2(-24f + offsetX, -24f + offsetY);
        }

        private void ApplyWidth(RectTransform rect, float width)
        {
            Vector2 size = rect.sizeDelta;
            if (Mathf.Abs(size.x - width) > 0.5f)
            {
                size.x = width;
                rect.sizeDelta = size;
                _force = true;
            }
        }

        private void FitToMap()
        {
            float width = Mathf.Max(40f, _text.rectTransform.sizeDelta.x);
            float cap = _sourceFontSize > 0f ? _sourceFontSize : 18f;
            _text.fontSize = Mathf.Clamp(width / 12f, 12f, cap);
            _text.textWrappingMode = TextWrappingModes.Normal;
            _text.overflowMode = TextOverflowModes.Ellipsis;
            for (int pass = 0; pass < 2; pass++)
            {
                _text.ForceMeshUpdate();
                float preferred = _text.preferredWidth;
                if (preferred <= width || preferred <= 1f)
                {
                    break;
                }

                _text.fontSize = Mathf.Max(11f, _text.fontSize * (width / preferred));
            }

            _text.ForceMeshUpdate();
            Vector2 size = _text.rectTransform.sizeDelta;
            size.y = Mathf.Max(18f, _text.preferredHeight);
            _text.rectTransform.sizeDelta = size;
        }

        private void SnapUnderMap()
        {
            if (!_hasMapPoint || ClockText.NormalizeCorner(Plugin.Corner.Value) != "TopRight")
            {
                return;
            }

            Vector2 pos = _text.rectTransform.anchoredPosition;
            pos.y = ClockText.LabelTop(_mapPoint.y, Plugin.OffsetY.Value);
            _text.rectTransform.anchoredPosition = pos;
        }

        private static void SetCorner(RectTransform rect, float anchorX, TextAlignmentOptions alignment)
        {
            Vector2 anchor = new Vector2(anchorX, 1f);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.GetComponent<TextMeshProUGUI>().alignment = alignment;
        }

        private static bool TryGetMap(out Vector2 bottomRight, out float width)
        {
            bottomRight = Vector2.zero;
            width = 0f;
            Minimap map = Minimap.instance;
            if (map == null || map.m_smallRoot == null || Hud.instance == null || Hud.instance.m_rootObject == null)
            {
                return false;
            }

            RectTransform mapRect = null;
            if (map.m_mapImageSmall != null)
            {
                mapRect = map.m_mapImageSmall.rectTransform;
            }

            if (mapRect == null || mapRect.rect.width < 1f)
            {
                mapRect = map.m_smallRoot.GetComponent<RectTransform>();
            }
            RectTransform hudRect = Hud.instance.m_rootObject.GetComponent<RectTransform>();
            if (mapRect == null || hudRect == null)
            {
                return false;
            }

            if (mapRect.rect.width < 1f || mapRect.rect.height < 1f)
            {
                return false;
            }

            Vector3[] corners = new Vector3[4];
            mapRect.GetWorldCorners(corners);
            Vector3 left = hudRect.InverseTransformPoint(corners[0]);
            Vector3 right = hudRect.InverseTransformPoint(corners[3]);
            Rect hud = hudRect.rect;
            bottomRight = new Vector2(right.x - hud.xMax, right.y - hud.yMax);
            width = Mathf.Abs(right.x - left.x);
            return width >= 1f;
        }

        private static ClockView FindView()
        {
            if (Hud.instance == null || Hud.instance.m_rootObject == null)
            {
                return null;
            }

            Transform child = Hud.instance.m_rootObject.transform.Find("GameClock");
            if (child == null)
            {
                return null;
            }

            return child.GetComponent<ClockView>();
        }
    }
}
