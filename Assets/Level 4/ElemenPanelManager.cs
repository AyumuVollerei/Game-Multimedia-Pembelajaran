using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class KategoriElemen
{
    public string namaKategori;         // Contoh: Latar Belakang, Teks, Kopi, dll.
    public bool penuhSatuPoster;        // Centang HANYA untuk Latar Belakang & Overlay
    public bool isKategoriTeks;         // Centang HANYA untuk kategori Teks

    [Header("Isi Jika Gambar / Aset Biasa")]
    public Sprite[] daftarAset;

    [Header("Isi Jika Kategori Teks (Font)")]
    public TMP_FontAsset[] daftarFont;
}

public class ElemenPanelManager : MonoBehaviour
{
    [Header("Pengaturan Panel & Wadah")]
    public RectTransform bgElemen;
    public RectTransform areaPoster;
    public Transform wadahAset;
    public GameObject itemAsetPrefab;
    public GameObject itemFontPrefab;

    [Header("Ukuran Lebar bg elemen")]
    public float lebarNormal = 325.5f;
    public float lebarMelebar = 695f;
    public float kecepatanAnimasi = 12f;

    [Header("Daftar Kategori & Aset")]
    public List<KategoriElemen> daftarKategori = new List<KategoriElemen>();

    [Header("SFX Klik (Opsional)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    private float targetLebar;
    private int kategoriAktif = -1;

    void Start()
    {
        targetLebar = lebarNormal;
        if (bgElemen != null)
            bgElemen.sizeDelta = new Vector2(lebarNormal, bgElemen.sizeDelta.y);

        if (wadahAset != null)
            wadahAset.gameObject.SetActive(false);
    }

    void Update()
    {
        if (bgElemen != null)
        {
            float lebarSekarang = Mathf.Lerp(bgElemen.sizeDelta.x, targetLebar, Time.deltaTime * kecepatanAnimasi);
            bgElemen.sizeDelta = new Vector2(lebarSekarang, bgElemen.sizeDelta.y);
        }
    }

    public void PilihKategori(int indeks)
    {
        if (audioSource != null && sfxKlik != null)
            audioSource.PlayOneShot(sfxKlik);

        if (kategoriAktif == indeks)
        {
            TutupPanelAset();
            return;
        }

        kategoriAktif = indeks;
        targetLebar = lebarMelebar;

        if (wadahAset != null)
        {
            wadahAset.gameObject.SetActive(true);
            IsiDaftarAset(indeks);
        }
    }

    public void TutupPanelAset()
    {
        kategoriAktif = -1;
        targetLebar = lebarNormal;
        if (wadahAset != null)
            wadahAset.gameObject.SetActive(false);
    }

    private void IsiDaftarAset(int indeks)
    {
        foreach (Transform anak in wadahAset)
        {
            Destroy(anak.gameObject);
        }

        if (indeks < 0 || indeks >= daftarKategori.Count) return;

        KategoriElemen kategori = daftarKategori[indeks];

        if (kategori.isKategoriTeks)
        {
            foreach (TMP_FontAsset font in kategori.daftarFont)
            {
                if (font == null) continue;

                GameObject itemBaru = Instantiate(itemFontPrefab, wadahAset);
                TMP_Text teksPreview = itemBaru.GetComponent<TMP_Text>();
                if (teksPreview != null)
                {
                    teksPreview.font = font;
                    teksPreview.color = new Color32(20, 30, 43, 255);
                    teksPreview.alignment = TextAlignmentOptions.Center;
                    teksPreview.enableAutoSizing = false;
                    teksPreview.fontSize = 20;
                    teksPreview.textWrappingMode = TextWrappingModes.Normal;
                    teksPreview.overflowMode = TextOverflowModes.Overflow;

                    string namaBersih = font.name.Replace(" SDF", "").Replace("-", " ");
                    teksPreview.text = "<size=180%>Aa</size>\n" + namaBersih;
                }

                ItemFontDraggable dragFont = itemBaru.GetComponent<ItemFontDraggable>();
                if (dragFont == null)
                    dragFont = itemBaru.AddComponent<ItemFontDraggable>();

                dragFont.Setup(this, font);
            }
            return;
        }

        foreach (Sprite gambar in kategori.daftarAset)
        {
            if (gambar == null) continue;

            GameObject itemBaru = Instantiate(itemAsetPrefab, wadahAset);
            Image img = itemBaru.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = gambar;
                img.preserveAspect = true;
            }

            ItemAsetDraggable dragScript = itemBaru.GetComponent<ItemAsetDraggable>();
            if (dragScript == null)
                dragScript = itemBaru.AddComponent<ItemAsetDraggable>();

            dragScript.Setup(this, gambar, kategori.penuhSatuPoster);
        }
    }
}