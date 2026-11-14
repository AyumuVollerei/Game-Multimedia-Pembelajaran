using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemAsetDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ElemenPanelManager manager;
    private Sprite spriteAset;
    private bool penuhSatuPoster;

    private GameObject objekSementara;
    private RectTransform rectSementara;
    private Canvas canvasUtama;

    public void Setup(ElemenPanelManager mgr, Sprite sprite, bool penuhPoster)
    {
        manager = mgr;
        spriteAset = sprite;
        penuhSatuPoster = penuhPoster;
        canvasUtama = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasUtama == null || spriteAset == null) return;

        objekSementara = new GameObject("PreviewDrag_" + spriteAset.name);
        objekSementara.transform.SetParent(canvasUtama.transform, false);
        objekSementara.transform.SetAsLastSibling();

        Image img = objekSementara.AddComponent<Image>();
        img.sprite = spriteAset;
        img.preserveAspect = true;
        img.raycastTarget = false;

        rectSementara = objekSementara.GetComponent<RectTransform>();
        rectSementara.sizeDelta = new Vector2(140, 140);

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
            PasangAsetKePoster(eventData);
        }
    }

    private void PasangAsetKePoster(PointerEventData eventData)
    {
        // Jika aset memenuhi satu poster (Latar Belakang / Overlay)
        if (penuhSatuPoster)
        {
            bool isOverlay = spriteAset.name.ToLower().Contains("overlay") || spriteAset.name.ToLower().Contains("tekstur");
            string namaObjek = isOverlay ? "Poster_Overlay" : "Poster_LatarBelakang";

            // Hapus latar/overlay lama jika pemain menggantinya dengan yang baru
            Transform lama = manager.areaPoster.Find(namaObjek);
            if (lama != null)
                Destroy(lama.gameObject);

            GameObject objPenuh = new GameObject(namaObjek);
            objPenuh.transform.SetParent(manager.areaPoster, false);

            // Latar belakang ditaruh paling bawah, overlay di atas latar belakang
            if (isOverlay)
            {
                Transform latarAda = manager.areaPoster.Find("Poster_LatarBelakang");
                objPenuh.transform.SetSiblingIndex(latarAda != null ? 1 : 0);
            }
            else
            {
                objPenuh.transform.SetAsFirstSibling();
            }

            RectTransform rectPenuh = objPenuh.AddComponent<RectTransform>();
            rectPenuh.anchorMin = Vector2.zero;
            rectPenuh.anchorMax = Vector2.one;
            rectPenuh.offsetMin = Vector2.zero;
            rectPenuh.offsetMax = Vector2.zero;

            Image imgPenuh = objPenuh.AddComponent<Image>();
            imgPenuh.sprite = spriteAset;
            imgPenuh.preserveAspect = false;
            imgPenuh.raycastTarget = false;
            return;
        }

        // Jika aset biasa (Kopi, Bazar, Maskot, Stempel, dll.)
        GameObject objElemen = new GameObject("Elemen_" + spriteAset.name);
        objElemen.transform.SetParent(manager.areaPoster, false);
        objElemen.transform.SetAsLastSibling();

        Image imgElemen = objElemen.AddComponent<Image>();
        imgElemen.sprite = spriteAset;
        imgElemen.preserveAspect = true;

        RectTransform rectElemen = objElemen.GetComponent<RectTransform>();
        float lebarAsli = spriteAset.rect.width;
        float tinggiAsli = spriteAset.rect.height;
        float skalaAwal = 160f / Mathf.Max(lebarAsli, tinggiAsli);
        rectElemen.sizeDelta = new Vector2(lebarAsli * skalaAwal, tinggiAsli * skalaAwal);

        Vector2 posisiLokalPoster;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            manager.areaPoster,
            eventData.position,
            eventData.pressEventCamera,
            out posisiLokalPoster
        );
        rectElemen.anchoredPosition = posisiLokalPoster;

        ElemenPosterDraggable dragPoster = objElemen.AddComponent<ElemenPosterDraggable>();
        dragPoster.areaBatas = manager.areaPoster;
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