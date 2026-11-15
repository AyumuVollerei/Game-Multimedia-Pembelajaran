using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PosterActionManager : MonoBehaviour
{
    public static PosterActionManager Instance;

    [Header("1. Header UI Level 4")]
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksSkorHeader;

    [Header("2. Referensi Kanvas Poster")]
    public RectTransform areaPoster;

    [Header("3. Navigasi Cek Hasil")]
    public string namaSceneCekHasil = "Level 4_cek hasil";
    public float jedaPindahScene = 0.15f;

    [Header("4. SFX Klik (Opsional)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    private bool sedangPindah = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Langsung tampilkan sisa nyawa & total skor terakhir dari level sebelumnya
        int nyawaTerkini = PlayerPrefs.GetInt("NyawaPemain", 3);
        int skorTerkini = PlayerPrefs.GetInt("TotalSkor", 0);

        if (teksNyawaHeader != null)
            teksNyawaHeader.text = "Nyawa " + nyawaTerkini;

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorTerkini;
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

    // Hubungkan fungsi ini ke Button -> On Click () pada tombol "Cek Hasil"
    public void TombolCekHasil()
    {
        if (sedangPindah) return;
        StartCoroutine(TangkapPosterLaluPindah());
    }

    private IEnumerator TangkapPosterLaluPindah()
    {
        sedangPindah = true;
        MainkanSFX();

        if (sfxKlik != null)
            yield return new WaitForSecondsRealtime(jedaPindahScene);

        // Tunggu render frame selesai agar semua elemen & teks poster terpotret utuh
        yield return new WaitForEndOfFrame();

        if (areaPoster != null)
        {
            Vector3[] sudut = new Vector3[4];
            areaPoster.GetWorldCorners(sudut);

            Canvas rootCanvas = areaPoster.GetComponentInParent<Canvas>();
            Camera cam = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? rootCanvas.worldCamera
                : null;

            Vector2 titikBawahKiri = RectTransformUtility.WorldToScreenPoint(cam, sudut[0]);
            Vector2 titikAtasKanan = RectTransformUtility.WorldToScreenPoint(cam, sudut[2]);

            int x = Mathf.Clamp(Mathf.RoundToInt(titikBawahKiri.x), 0, Screen.width - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt(titikBawahKiri.y), 0, Screen.height - 1);
            int lebar = Mathf.Clamp(Mathf.RoundToInt(titikAtasKanan.x - titikBawahKiri.x), 1, Screen.width - x);
            int tinggi = Mathf.Clamp(Mathf.RoundToInt(titikAtasKanan.y - titikBawahKiri.y), 1, Screen.height - y);

            Texture2D hasilFoto = new Texture2D(lebar, tinggi, TextureFormat.RGB24, false);
            hasilFoto.ReadPixels(new Rect(x, y, lebar, tinggi), 0, 0);
            hasilFoto.Apply();

            DataPosterLevel4.gambarPoster = hasilFoto;
        }

        SceneManager.LoadScene(namaSceneCekHasil);
    }

    private void MainkanSFX()
    {
        if (audioSource != null && sfxKlik != null)
            audioSource.PlayOneShot(sfxKlik);
    }
}