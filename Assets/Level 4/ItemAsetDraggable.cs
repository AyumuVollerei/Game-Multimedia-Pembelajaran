using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemAsetDraggable : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ElemenPanelManager manager;
    private Sprite gambarAset;
    private bool penuhSatuPoster;
    private string kategoriAset = "";

    private GameObject objekSementara;
    private RectTransform rectSementara;
    private Canvas canvasUtama;

    public void Setup(ElemenPanelManager mgr, Sprite sprite, bool fullPoster, string kategori = "")
    {
        manager = mgr;
        gambarAset = sprite;
        penuhSatuPoster = fullPoster;
        kategoriAset = kategori;
        canvasUtama = GetComponentInParent<Canvas>();
    }

    private Camera AmbilKameraCanvas()
    {
        if (canvasUtama == null || canvasUtama.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;
        return canvasUtama.worldCamera != null ? canvasUtama.worldCamera : Camera.main;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging || manager == null || manager.areaPoster == null || gambarAset == null) return;

        if (penuhSatuPoster)
        {
            PasangLatarAtauOverlay();
            return;
        }

        Transform wadahSlot = manager.areaPoster.parent != null ? manager.areaPoster.parent : manager.areaPoster;
        PosterSlotTarget[] semuaSlot = wadahSlot.GetComponentsInChildren<PosterSlotTarget>(true);

        // Prioritaskan slot kosong yang kata kuncinya cocok terlebih dahulu
        foreach (PosterSlotTarget slot in semuaSlot)
        {
            if (slot.terimaGambar && !slot.SudahTerisi && CekKecocokanAwal(slot))
            {
                slot.PasangGambar(gambarAset, true, kategoriAset);
                return;
            }
        }

        // Jika tidak ada yang spesifik cocok, isi slot gambar kosong berikutnya
        foreach (PosterSlotTarget slot in semuaSlot)
        {
            if (slot.terimaGambar && !slot.SudahTerisi)
            {
                slot.PasangGambar(gambarAset, true, kategoriAset);
                return;
            }
        }
    }

    private bool CekKecocokanAwal(PosterSlotTarget slot)
    {
        if (string.IsNullOrWhiteSpace(slot.kataKunciGambarCocok)) return true;
        string gabungan = (kategoriAset + " " + gambarAset.name).ToLower();
        foreach (string k in slot.kataKunciGambarCocok.ToLower().Split(','))
        {
            if (!string.IsNullOrWhiteSpace(k) && gabungan.Contains(k.Trim()))
                return true;
        }
        return false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasUtama == null || gambarAset == null) return;

        objekSementara = new GameObject("PreviewAset");
        objekSementara.transform.SetParent(canvasUtama.transform, false);
        objekSementara.transform.SetAsLastSibling();

        Image img = objekSementara.AddComponent<Image>();
        img.sprite = gambarAset;
        img.preserveAspect = true;
        img.color = new Color(1f, 1f, 1f, 0.85f);
        img.raycastTarget = false;

        rectSementara = objekSementara.GetComponent<RectTransform>();
        rectSementara.sizeDelta = new Vector2(120, 120);

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

        Camera cam = AmbilKameraCanvas();

        if (penuhSatuPoster)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(manager.areaPoster, eventData.position, cam))
                PasangLatarAtauOverlay();
            return;
        }

        Transform wadahSlot = manager.areaPoster.parent != null ? manager.areaPoster.parent : manager.areaPoster;
        PosterSlotTarget[] semuaSlot = wadahSlot.GetComponentsInChildren<PosterSlotTarget>(true);

        foreach (PosterSlotTarget slot in semuaSlot)
        {
            if (!slot.terimaGambar) continue;

            RectTransform rtSlot = slot.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rtSlot, eventData.position, cam))
            {
                slot.PasangGambar(gambarAset, true, kategoriAset);
                break;
            }
        }
    }

    private void PasangLatarAtauOverlay()
    {
        bool adalahOverlay = (kategoriAset + " " + gambarAset.name).ToLower().Contains("overlay");

        if (adalahOverlay)
        {
            Transform trOverlay = manager.areaPoster.parent != null
                ? manager.areaPoster.parent.Find("slot overlay")
                : null;

            if (trOverlay != null)
            {
                Image imgOverlay = trOverlay.GetComponent<Image>();
                if (imgOverlay != null)
                {
                    imgOverlay.sprite = gambarAset;
                    imgOverlay.preserveAspect = false;
                    imgOverlay.color = new Color(1f, 1f, 1f, 0.7f);
                    imgOverlay.raycastTarget = false;
                }
            }
        }
        else
        {
            Image imgPoster = manager.areaPoster.GetComponent<Image>();
            if (imgPoster != null)
            {
                imgPoster.sprite = gambarAset;
                imgPoster.preserveAspect = false;
                imgPoster.color = Color.white;
            }
        }

        if (PosterActionManager.Instance != null)
            PosterActionManager.Instance.CatatPerubahan();
    }

    private void UpdatePosisiDrag(PointerEventData eventData)
    {
        Vector2 posisiLokal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasUtama.transform as RectTransform,
            eventData.position,
            AmbilKameraCanvas(),
            out posisiLokal
        );
        rectSementara.anchoredPosition = posisiLokal;
    }
}