using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TeksResizableController : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private static List<TeksResizableController> semuaTeks = new List<TeksResizableController>();

    private RectTransform areaBatas;
    private RectTransform rectTransform;
    private Canvas canvasUtama;

    private TMP_InputField inputField;
    private TextMeshProUGUI teksUtama;
    private TextMeshProUGUI teksPlaceholder;
    private Image bgKotak;
    private GameObject tombolHandle;

    private float ukuranFontSekarang = 6f;
    private float minUkuranFont = 3f;
    private float maxUkuranFont = 65f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasUtama = GetComponentInParent<Canvas>();
        semuaTeks.Add(this);
    }

    void OnDestroy()
    {
        semuaTeks.Remove(this);
    }

    public void Inisialisasi(RectTransform batas, TMP_InputField field, TextMeshProUGUI txt, TextMeshProUGUI ph, Image bg)
    {
        areaBatas = batas;
        inputField = field;
        teksUtama = txt;
        teksPlaceholder = ph;
        bgKotak = bg;

        ukuranFontSekarang = field.pointSize;

        inputField.onValueChanged.AddListener((_) =>
        {
            SesuaikanUkuranKotak();
            BatasiDalamPoster();
        });

        inputField.onEndEdit.AddListener((_) =>
        {
            if (PosterActionManager.Instance != null)
                PosterActionManager.Instance.CatatPerubahan();
        });

        BuatHandlePojok();
        PilihTeksIni();
        SesuaikanUkuranKotak();
    }

    private void BuatHandlePojok()
    {
        tombolHandle = new GameObject("HandleResize");
        tombolHandle.transform.SetParent(transform, false);

        RectTransform rectHandle = tombolHandle.AddComponent<RectTransform>();
        rectHandle.anchorMin = new Vector2(1f, 0f);
        rectHandle.anchorMax = new Vector2(1f, 0f);
        rectHandle.pivot = new Vector2(0.5f, 0.5f);
        rectHandle.anchoredPosition = Vector2.zero;
        rectHandle.sizeDelta = new Vector2(6f, 6f);

        Image imgHandle = tombolHandle.AddComponent<Image>();
        imgHandle.color = new Color32(30, 115, 230, 255);

        HandleTarikUkuran dragHandle = tombolHandle.AddComponent<HandleTarikUkuran>();
        dragHandle.controller = this;
    }

    public void PilihTeksIni()
    {
        transform.SetAsLastSibling();
        if (PosterActionManager.Instance != null)
            PosterActionManager.Instance.PilihElemen(gameObject);

        foreach (var item in semuaTeks)
        {
            if (item != null)
                item.SetStatusTerpilih(item == this);
        }
    }

    public void SetStatusTerpilih(bool aktif)
    {
        if (tombolHandle != null)
            tombolHandle.SetActive(aktif);

        if (bgKotak != null)
            bgKotak.color = aktif ? new Color(0.15f, 0.5f, 0.95f, 0.15f) : new Color(1f, 1f, 1f, 0.01f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PilihTeksIni();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasUtama == null) return;

        float skalaPoster = areaBatas != null ? areaBatas.lossyScale.x : canvasUtama.scaleFactor;
        rectTransform.anchoredPosition += eventData.delta / skalaPoster;
        BatasiDalamPoster();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PosterActionManager.Instance != null)
            PosterActionManager.Instance.CatatPerubahan();
    }

    public void UbahUkuranDariDrag(Vector2 deltaDrag)
    {
        if (canvasUtama == null) return;

        float perubahan = (deltaDrag.x - deltaDrag.y) / canvasUtama.scaleFactor * 0.09f;
        ukuranFontSekarang = Mathf.Clamp(ukuranFontSekarang + perubahan, minUkuranFont, maxUkuranFont);

        inputField.pointSize = ukuranFontSekarang;
        teksUtama.fontSize = ukuranFontSekarang;
        teksPlaceholder.fontSize = ukuranFontSekarang;

        SesuaikanUkuranKotak();
        BatasiDalamPoster();
    }

    public void SesuaikanUkuranKotak()
    {
        if (teksUtama == null || rectTransform == null) return;

        float maxLebarPoster = areaBatas != null ? areaBatas.rect.width - 6f : 94f;
        float maxTinggiPoster = areaBatas != null ? areaBatas.rect.height - 6f : 94f;

        bool kosong = string.IsNullOrEmpty(inputField.text);
        string isi = kosong ? teksPlaceholder.text : inputField.text;

        Vector2 ukuranIdeal = teksUtama.GetPreferredValues(isi, maxLebarPoster - 6f, 0f);

        float lebarBaru = Mathf.Clamp(ukuranIdeal.x + 12f, 32f, maxLebarPoster);
        float tinggiBaru = Mathf.Clamp(ukuranIdeal.y + 6f, 14f, maxTinggiPoster);

        rectTransform.sizeDelta = new Vector2(lebarBaru, tinggiBaru);
    }

    private void BatasiDalamPoster()
    {
        if (areaBatas == null) return;

        Vector2 pos = rectTransform.anchoredPosition;
        float batasX = Mathf.Max(0f, (areaBatas.rect.width - rectTransform.rect.width) * 0.5f);
        float batasY = Mathf.Max(0f, (areaBatas.rect.height - rectTransform.rect.height) * 0.5f);

        pos.x = Mathf.Clamp(pos.x, -batasX, batasX);
        pos.y = Mathf.Clamp(pos.y, -batasY, batasY);

        rectTransform.anchoredPosition = pos;
    }
}

public class HandleTarikUkuran : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public TeksResizableController controller;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (controller != null)
            controller.PilihTeksIni();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (controller != null)
            controller.UbahUkuranDariDrag(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PosterActionManager.Instance != null)
            PosterActionManager.Instance.CatatPerubahan();
    }
}