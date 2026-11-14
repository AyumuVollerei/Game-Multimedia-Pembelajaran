using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElemenPosterDraggable : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public RectTransform areaBatas;

    private RectTransform rectTransform;
    private Canvas canvasUtama;
    private Outline garisSeleksi;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasUtama = GetComponentInParent<Canvas>();

        garisSeleksi = gameObject.AddComponent<Outline>();
        garisSeleksi.effectColor = new Color32(30, 115, 230, 255);
        garisSeleksi.effectDistance = new Vector2(1.5f, -1.5f);
        garisSeleksi.enabled = false;
    }

    public void SetSorotan(bool aktif)
    {
        if (garisSeleksi != null)
            garisSeleksi.enabled = aktif;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (PosterActionManager.Instance != null)
            PosterActionManager.Instance.PilihElemen(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasUtama == null) return;

        float skalaPoster = areaBatas != null ? areaBatas.lossyScale.x : canvasUtama.scaleFactor;
        rectTransform.anchoredPosition += eventData.delta / skalaPoster;

        if (areaBatas != null)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            float batasX = Mathf.Max(0f, (areaBatas.rect.width - rectTransform.rect.width) * 0.5f);
            float batasY = Mathf.Max(0f, (areaBatas.rect.height - rectTransform.rect.height) * 0.5f);
            pos.x = Mathf.Clamp(pos.x, -batasX, batasX);
            pos.y = Mathf.Clamp(pos.y, -batasY, batasY);
            rectTransform.anchoredPosition = pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PosterActionManager.Instance != null)
            PosterActionManager.Instance.CatatPerubahan();
    }
}