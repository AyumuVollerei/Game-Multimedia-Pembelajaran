using UnityEngine;
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

        Transform wadahSlot = manager.areaPoster.parent != null ? manager.areaPoster.parent : manager.areaPoster;
        PosterSlotTarget[] semuaSlot = wadahSlot.GetComponentsInChildren<PosterSlotTarget>();

        foreach (PosterSlotTarget slot in semuaSlot)
        {
            if (slot.terimaTeks && !slot.SudahTerisi)
            {
                slot.PasangTeks(fontDipilih);
                return;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasUtama == null || fontDipilih == null) return;

        objekSementara = new GameObject("PreviewFont_" + fontDipilih.name);
        objekSementara.transform.SetParent(canvasUtama.transform, false);
        objekSementara.transform.SetAsLastSibling();

        TextMeshProUGUI txt = objekSementara.AddComponent<TextMeshProUGUI>();
        txt.font = fontDipilih;
        txt.text = "Aa - " + fontDipilih.name.Replace(" SDF", "");
        txt.fontSize = 24;
        txt.color = new Color32(20, 30, 43, 255);
        txt.alignment = TextAlignmentOptions.Center;
        txt.raycastTarget = false;

        rectSementara = objekSementara.GetComponent<RectTransform>();
        rectSementara.sizeDelta = new Vector2(260, 50);

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

        Destroy(objekSementara);
        objekSementara = null;

        if (manager == null || manager.areaPoster == null) return;

        Transform wadahSlot = manager.areaPoster.parent != null ? manager.areaPoster.parent : manager.areaPoster;
        PosterSlotTarget[] semuaSlot = wadahSlot.GetComponentsInChildren<PosterSlotTarget>();

        foreach (PosterSlotTarget slot in semuaSlot)
        {
            if (!slot.terimaTeks) continue;

            RectTransform rtSlot = slot.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rtSlot, eventData.position, eventData.pressEventCamera))
            {
                slot.PasangTeks(fontDipilih);
                break;
            }
        }
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