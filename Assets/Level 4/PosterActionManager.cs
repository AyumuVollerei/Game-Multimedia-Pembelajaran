using UnityEngine;
using UnityEngine.UI;

public class PosterActionManager : MonoBehaviour
{
    public static PosterActionManager Instance;

    [Header("Referensi Kanvas Poster")]
    public RectTransform areaPoster;

    [Header("SFX Klik (Opsional)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    void Awake()
    {
        Instance = this;
    }

    // Dibiarkan kosong agar script lain yang memanggil CatatPerubahan() tidak error
    public void CatatPerubahan() { }

    public void TombolReset()
    {
        MainkanSFX();
        if (areaPoster == null) return;

        // 1. Kosongkan gambar Latar Belakang di objek poster
        Image imgPoster = areaPoster.GetComponent<Image>();
        if (imgPoster != null)
        {
            imgPoster.sprite = null;
            imgPoster.color = new Color32(253, 246, 227, 255); // Warna krem awal poster
        }

        // 2. Kosongkan gambar di slot overlay
        Transform trOverlay = areaPoster.parent != null ? areaPoster.parent.Find("slot overlay") : null;
        if (trOverlay != null && trOverlay.GetComponent<Image>() != null)
        {
            Image imgOverlay = trOverlay.GetComponent<Image>();
            imgOverlay.sprite = null;
            imgOverlay.color = new Color(1f, 1f, 1f, 0f);
        }

        // 3. Kosongkan slot 1 sampai slot 6
        Transform wadahSlot = areaPoster.parent != null ? areaPoster.parent : areaPoster;
        foreach (PosterSlotTarget slot in wadahSlot.GetComponentsInChildren<PosterSlotTarget>(true))
        {
            slot.KosongkanSlot();
        }
    }

    private void MainkanSFX()
    {
        if (audioSource != null && sfxKlik != null)
            audioSource.PlayOneShot(sfxKlik);
    }
}