using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemFontDraggable : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ElemenPanelManager manager;
    private TMP_FontAsset fontDipilih;

    private GameObject objekSementara;
    private RectTransform rectSementara;
    private Canvas canvasUtama;

    public void Setup(ElemenPanelManager mgr, TMP_FontAsset font)
    {
        manager = mgr;
        fontDipilih = font;
        canvasUtama = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging || manager == null || manager.areaPoster == null || fontDipilih == null) return;
        BuatTeksEditableDiPoster(Vector2.zero);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasUtama == null || fontDipilih == null) return;

        objekSementara = new GameObject("PreviewFont_" + fontDipilih.name);
        objekSementara.transform.SetParent(canvasUtama.transform, false);
        objekSementara.transform.SetAsLastSibling();

        // Menggunakan TextMeshProUGUI (bukan TMP_Text) agar tidak error
        TextMeshProUGUI txt = objekSementara.AddComponent<TextMeshProUGUI>();
        txt.font = fontDipilih;
        txt.text = "Ketik Teks...";
        txt.fontSize = 36;
        txt.color = new Color32(20, 30, 43, 255);
        txt.alignment = TextAlignmentOptions.Center;
        txt.raycastTarget = false;

        rectSementara = objekSementara.GetComponent<RectTransform>();
        rectSementara.sizeDelta = new Vector2(300, 70);

        UpdatePosisiDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (objekSementara != null)
            UpdatePosisiDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (objekSementara == null) return;

        bool diDalamPoster = RectTransformUtility.RectangleContainsScreenPoint(
            manager.areaPoster,
            eventData.position,
            eventData.pressEventCamera
        );

        Destroy(objekSementara);
        objekSementara = null;

        if (diDalamPoster)
        {
            Vector2 posisiLokalPoster;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                manager.areaPoster,
                eventData.position,
                eventData.pressEventCamera,
                out posisiLokalPoster
            );
            BuatTeksEditableDiPoster(posisiLokalPoster);
        }
    }

    private void BuatTeksEditableDiPoster(Vector2 posisiAwal)
    {
        GameObject objTeks = new GameObject("ElemenTeks_" + fontDipilih.name);
        objTeks.transform.SetParent(manager.areaPoster, false);
        objTeks.transform.SetAsLastSibling();

        RectTransform rectUtama = objTeks.AddComponent<RectTransform>();
        rectUtama.sizeDelta = new Vector2(280, 75);
        rectUtama.anchoredPosition = posisiAwal;

        Image bgKotak = objTeks.AddComponent<Image>();
        bgKotak.color = new Color(1f, 1f, 1f, 0.01f);

        // Komponen Text
        GameObject objIsiTeks = new GameObject("Text");
        objIsiTeks.transform.SetParent(objTeks.transform, false);
        RectTransform rectTeks = objIsiTeks.AddComponent<RectTransform>();
        rectTeks.anchorMin = Vector2.zero;
        rectTeks.anchorMax = Vector2.one;
        rectTeks.offsetMin = new Vector2(12, 8);
        rectTeks.offsetMax = new Vector2(-12, -8);

        TextMeshProUGUI komponenTeks = objIsiTeks.AddComponent<TextMeshProUGUI>();
        komponenTeks.font = fontDipilih;
        komponenTeks.fontSize = 36;
        komponenTeks.color = new Color32(20, 30, 43, 255);
        komponenTeks.alignment = TextAlignmentOptions.Center;
        komponenTeks.textWrappingMode = TextWrappingModes.Normal; // Otomatis turun baris

        // Komponen Placeholder
        GameObject objPlaceholder = new GameObject("Placeholder");
        objPlaceholder.transform.SetParent(objTeks.transform, false);
        RectTransform rectPh = objPlaceholder.AddComponent<RectTransform>();
        rectPh.anchorMin = Vector2.zero;
        rectPh.anchorMax = Vector2.one;
        rectPh.offsetMin = new Vector2(12, 8);
        rectPh.offsetMax = new Vector2(-12, -8);

        TextMeshProUGUI komponenPh = objPlaceholder.AddComponent<TextMeshProUGUI>();
        komponenPh.font = fontDipilih;
        komponenPh.fontSize = 36;
        komponenPh.color = new Color(0.2f, 0.2f, 0.2f, 0.45f);
        komponenPh.alignment = TextAlignmentOptions.Center;
        komponenPh.text = "Ketik di sini...";

        // InputField
        TMP_InputField inputField = objTeks.AddComponent<TMP_InputField>();
        inputField.textViewport = rectUtama;
        inputField.textComponent = komponenTeks;
        inputField.placeholder = komponenPh;
        inputField.fontAsset = fontDipilih;
        inputField.pointSize = 36;
        inputField.lineType = TMP_InputField.LineType.MultiLineNewline;

        TeksResizableController kontrolTeks = objTeks.AddComponent<TeksResizableController>();
        kontrolTeks.Inisialisasi(manager.areaPoster, inputField, komponenTeks, komponenPh, bgKotak);

        inputField.Select();
        inputField.ActivateInputField();
    }

    private void UpdatePosisiDrag(PointerEventData eventData)
    {
        Vector2 posisiLokal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasUtama.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out posisiLokal
        );
        rectSementara.anchoredPosition = posisiLokal;
    }
}