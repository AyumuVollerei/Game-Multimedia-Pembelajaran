using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PosterSlotTarget : MonoBehaviour
{
    [Header("Jenis Elemen yang Diterima Slot Ini")]
    public bool terimaGambar = true;
    public bool terimaTeks = false;

    [Header("Kriteria Kecocokan Slot (Pisahkan dengan koma)")]
    [Tooltip("Contoh: stempel,logo,bazar atau kopi,maskot,nusantara. Kosongkan jika bebas.")]
    public string kataKunciGambarCocok = "";
    [Tooltip("Contoh: bold,black untuk Judul atau regular,medium untuk Subjudul. Kosongkan jika bebas.")]
    public string kataKunciFontCocok = "";
    public string pesanJikaKurangCocok = "elemen di slot ini kurang sesuai";

    [Header("Pengaturan Teks Khusus Slot Ini")]
    public float ukuranFontSlot = 42f;
    [TextArea(1, 3)]
    public string teksBawaanSlot = "Judul Poster";
    public Color warnaTeksDefault = new Color32(20, 30, 43, 255);
    public TextAlignmentOptions perataanTeks = TextAlignmentOptions.MidlineLeft;

    private Image bingkaiSlot;
    private GameObject labelPetunjuk;
    private Image gambarIsi;
    private TMP_InputField inputTeksIsi;
    private string infoElemenTerpasang = "";

    public bool SudahTerisi { get; private set; }
    public bool AdalahTeksTerisi => SudahTerisi && inputTeksIsi != null && inputTeksIsi.gameObject.activeSelf;
    public bool AdalahGambarTerisi => SudahTerisi && gambarIsi != null && gambarIsi.gameObject.activeSelf;
    public float UkuranFontAktif => inputTeksIsi != null ? inputTeksIsi.pointSize : 0f;

    public Sprite SpriteAktif => AdalahGambarTerisi ? gambarIsi.sprite : null;
    public string TeksAktif => AdalahTeksTerisi ? inputTeksIsi.text : "";
    public TMP_FontAsset FontAktif => AdalahTeksTerisi ? inputTeksIsi.fontAsset : null;

    void Awake()
    {
        bingkaiSlot = GetComponent<Image>();

        TMP_Text petunjuk = GetComponentInChildren<TMP_Text>();
        if (petunjuk != null)
            labelPetunjuk = petunjuk.gameObject;

        SiapkanWadahIsi();
        KosongkanSlot();
    }

    private void SiapkanWadahIsi()
    {
        GameObject objImg = new GameObject("IsiGambar");
        objImg.transform.SetParent(transform, false);
        RectTransform rtImg = objImg.AddComponent<RectTransform>();
        rtImg.anchorMin = Vector2.zero;
        rtImg.anchorMax = Vector2.one;
        rtImg.offsetMin = Vector2.zero;
        rtImg.offsetMax = Vector2.zero;

        gambarIsi = objImg.AddComponent<Image>();
        gambarIsi.preserveAspect = true;
        gambarIsi.raycastTarget = false;
        objImg.SetActive(false);

        GameObject objTeks = new GameObject("IsiTeks");
        objTeks.transform.SetParent(transform, false);
        RectTransform rtTeks = objTeks.AddComponent<RectTransform>();
        rtTeks.anchorMin = Vector2.zero;
        rtTeks.anchorMax = Vector2.one;
        rtTeks.offsetMin = new Vector2(10f, 4f);
        rtTeks.offsetMax = new Vector2(-10f, -4f);

        TextMeshProUGUI komponenTeks = objTeks.AddComponent<TextMeshProUGUI>();
        komponenTeks.fontSize = ukuranFontSlot;
        komponenTeks.color = warnaTeksDefault;
        komponenTeks.alignment = perataanTeks;
        komponenTeks.textWrappingMode = TextWrappingModes.Normal;

        inputTeksIsi = objTeks.AddComponent<TMP_InputField>();
        inputTeksIsi.textViewport = rtTeks;
        inputTeksIsi.textComponent = komponenTeks;
        inputTeksIsi.pointSize = ukuranFontSlot;
        inputTeksIsi.lineType = TMP_InputField.LineType.MultiLineNewline;

        inputTeksIsi.onEndEdit.AddListener((_) =>
        {
            if (PosterActionManager.Instance != null)
                PosterActionManager.Instance.CatatPerubahan();
        });

        objTeks.SetActive(false);
    }

    public bool PasangGambar(Sprite spriteBaru, bool catatRiwayat = true, string infoKategori = "")
    {
        if (!terimaGambar || spriteBaru == null) return false;

        SudahTerisi = true;
        infoElemenTerpasang = (infoKategori + " " + spriteBaru.name).ToLower();
        SembunyikanBingkai();

        inputTeksIsi.gameObject.SetActive(false);
        gambarIsi.gameObject.SetActive(true);
        gambarIsi.sprite = spriteBaru;
        gambarIsi.color = Color.white;

        if (catatRiwayat && PosterActionManager.Instance != null)
            PosterActionManager.Instance.CatatPerubahan();

        return true;
    }

    public bool PasangTeks(TMP_FontAsset fontBaru, string teksKustom = null, bool catatRiwayat = true)
    {
        if (!terimaTeks || fontBaru == null) return false;

        bool sebelumnyaSudahTeks = AdalahTeksTerisi;

        SudahTerisi = true;
        infoElemenTerpasang = fontBaru.name.ToLower();
        SembunyikanBingkai();

        gambarIsi.gameObject.SetActive(false);
        inputTeksIsi.gameObject.SetActive(true);

        inputTeksIsi.fontAsset = fontBaru;
        inputTeksIsi.pointSize = ukuranFontSlot;
        if (inputTeksIsi.textComponent != null)
        {
            inputTeksIsi.textComponent.font = fontBaru;
            inputTeksIsi.textComponent.fontSize = ukuranFontSlot;
            inputTeksIsi.textComponent.alignment = perataanTeks;
        }

        if (teksKustom != null)
            inputTeksIsi.text = teksKustom;
        else if (!sebelumnyaSudahTeks || string.IsNullOrWhiteSpace(inputTeksIsi.text))
            inputTeksIsi.text = teksBawaanSlot;

        if (catatRiwayat && PosterActionManager.Instance != null)
            PosterActionManager.Instance.CatatPerubahan();

        return true;
    }

    // Mengecek apakah elemen yang dimasukkan pemain cocok dengan peruntukan slot ini
    public bool ApakahElemenCocok()
    {
        if (!SudahTerisi) return false;

        string daftarKunci = AdalahTeksTerisi ? kataKunciFontCocok : kataKunciGambarCocok;
        if (string.IsNullOrWhiteSpace(daftarKunci)) return true;

        string[] kataKunci = daftarKunci.ToLower().Split(',');
        foreach (string kunci in kataKunci)
        {
            string k = kunci.Trim();
            if (!string.IsNullOrEmpty(k) && infoElemenTerpasang.Contains(k))
                return true;
        }

        return false;
    }

    public void KosongkanSlot()
    {
        SudahTerisi = false;
        infoElemenTerpasang = "";

        if (gambarIsi != null)
        {
            gambarIsi.sprite = null;
            gambarIsi.gameObject.SetActive(false);
        }
        if (inputTeksIsi != null)
        {
            inputTeksIsi.text = "";
            inputTeksIsi.gameObject.SetActive(false);
        }

        if (bingkaiSlot != null)
            bingkaiSlot.color = new Color(1f, 1f, 1f, 0.28f);

        if (labelPetunjuk != null)
            labelPetunjuk.SetActive(true);
    }

    private void SembunyikanBingkai()
    {
        if (bingkaiSlot != null)
            bingkaiSlot.color = new Color(1f, 1f, 1f, 0f);

        if (labelPetunjuk != null)
            labelPetunjuk.SetActive(false);
    }
}