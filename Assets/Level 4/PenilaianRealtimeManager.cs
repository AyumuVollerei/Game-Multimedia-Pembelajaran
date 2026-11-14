using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PenilaianRealtimeManager : MonoBehaviour
{
    public static PenilaianRealtimeManager Instance;

    [Header("Referensi Kanvas Poster")]
    public RectTransform areaPoster;

    [Header("Komponen UI Penilaian")]
    public Slider sliderKeseimbangan;
    public TMP_Text teksPersenKeseimbangan;

    public Slider sliderKontras;
    public TMP_Text teksPersenKontras;

    public Slider sliderKedekatan;
    public TMP_Text teksPersenKedekatan;

    [Header("Tips & Skor Header")]
    public TMP_Text teksTips;
    public TMP_Text teksScoreHeader;

    [Header("SFX Klik (Opsional)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    [Header("Pengaturan Animasi Bar")]
    public float kecepatanSlider = 8f;

    private float targetKeseimbangan;
    private float targetKontras;
    private float targetKedekatan;

    public int SkorTotalSekarang { get; private set; }
    public int NilaiKeseimbangan => Mathf.RoundToInt(targetKeseimbangan);
    public int NilaiKontras => Mathf.RoundToInt(targetKontras);
    public int NilaiKedekatan => Mathf.RoundToInt(targetKedekatan);

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SiapkanSlider(sliderKeseimbangan);
        SiapkanSlider(sliderKontras);
        SiapkanSlider(sliderKedekatan);

        if (teksScoreHeader != null)
            teksScoreHeader.text = "Score 0";
    }

    private void SiapkanSlider(Slider sld)
    {
        if (sld == null) return;
        sld.minValue = 0f;
        sld.maxValue = 100f;
        sld.value = 0f;
        sld.interactable = false;
    }

    void Update()
    {
        if (areaPoster == null) return;

        HitungPenilaianPoster();
        PerbaruiTampilanUI();
    }

    // Dipanggil saat tombol "Lihat Hasil" diklik
    public void LihatHasilPoster()
    {
        if (audioSource != null && sfxKlik != null)
            audioSource.PlayOneShot(sfxKlik);

        HitungPenilaianPoster();

        if (teksScoreHeader != null)
            teksScoreHeader.text = "Score " + SkorTotalSekarang;
    }

    private void HitungPenilaianPoster()
    {
        bool adaLatarBelakang = false;
        bool adaOverlay = false;
        List<RectTransform> daftarElemen = new List<RectTransform>();
        List<RectTransform> daftarTeks = new List<RectTransform>();
        List<float> daftarUkuranTeks = new List<float>();

        foreach (Transform anak in areaPoster)
        {
            if (!anak.gameObject.activeInHierarchy) continue;

            if (anak.name == "Poster_LatarBelakang")
            {
                adaLatarBelakang = true;
                continue;
            }
            if (anak.name == "Poster_Overlay")
            {
                adaOverlay = true;
                continue;
            }

            RectTransform rt = anak.GetComponent<RectTransform>();
            if (rt == null) continue;

            daftarElemen.Add(rt);

            TMP_InputField input = anak.GetComponent<TMP_InputField>();
            if (input != null)
            {
                daftarTeks.Add(rt);
                daftarUkuranTeks.Add(input.pointSize);
            }
        }

        if (daftarElemen.Count == 0)
        {
            targetKeseimbangan = 0f;
            targetKontras = adaLatarBelakang ? 20f : 0f;
            targetKedekatan = 0f;
            SkorTotalSekarang = Mathf.RoundToInt((targetKeseimbangan + targetKontras + targetKedekatan) / 3f);

            if (teksTips != null)
            {
                teksTips.text = adaLatarBelakang
                    ? "Tips: tambahkan judul teks dan elemen ilustrasi ke poster"
                    : "Tips: mulai dengan memilih latar belakang poster";
            }
            return;
        }

        // 1. KESEIMBANGAN
        Vector2 totalMomen = Vector2.zero;
        float totalBobot = 0f;

        foreach (RectTransform rt in daftarElemen)
        {
            float luas = Mathf.Max(10f, rt.rect.width * rt.rect.height);
            totalMomen += rt.anchoredPosition * luas;
            totalBobot += luas;
        }

        Vector2 pusatMassa = totalMomen / totalBobot;
        float batasX = Mathf.Max(1f, areaPoster.rect.width * 0.5f);
        float batasY = Mathf.Max(1f, areaPoster.rect.height * 0.5f);

        float penyimpanganX = Mathf.Clamp01(Mathf.Abs(pusatMassa.x) / batasX);
        float penyimpanganY = Mathf.Clamp01(Mathf.Abs(pusatMassa.y) / batasY);

        float skorSeimbangDasar = (1f - (penyimpanganX * 0.7f + penyimpanganY * 0.3f)) * 100f;
        float bonusKelengkapan = Mathf.Min(15f, (daftarElemen.Count - 1) * 5f);
        targetKeseimbangan = Mathf.Clamp(skorSeimbangDasar * 0.85f + bonusKelengkapan, 15f, 100f);

        // 2. KONTRAS
        float skorKontras = 0f;
        if (adaLatarBelakang) skorKontras += 35f;
        if (adaOverlay) skorKontras += 10f;
        if (daftarTeks.Count > 0) skorKontras += 25f;

        if (daftarUkuranTeks.Count >= 2)
        {
            float minFont = Mathf.Min(daftarUkuranTeks.ToArray());
            float maxFont = Mathf.Max(daftarUkuranTeks.ToArray());
            float selisihUkuran = maxFont - minFont;
            skorKontras += Mathf.Clamp(selisihUkuran * 4f, 10f, 30f);
        }
        else if (daftarElemen.Count >= 2)
        {
            skorKontras += 15f;
        }
        targetKontras = Mathf.Clamp(skorKontras, 10f, 100f);

        // 3. KEDEKATAN
        float skorKedekatan = 0f;
        if (daftarTeks.Count >= 2)
        {
            float jarakTerdekat = float.MaxValue;
            for (int i = 0; i < daftarTeks.Count; i++)
            {
                for (int j = i + 1; j < daftarTeks.Count; j++)
                {
                    float d = Vector2.Distance(daftarTeks[i].anchoredPosition, daftarTeks[j].anchoredPosition);
                    if (d < jarakTerdekat) jarakTerdekat = d;
                }
            }

            if (jarakTerdekat < 8f)
                skorKedekatan = 55f;
            else if (jarakTerdekat <= 30f)
                skorKedekatan = 100f - Mathf.Abs(jarakTerdekat - 18f) * 1.2f;
            else
                skorKedekatan = Mathf.Clamp(100f - (jarakTerdekat - 30f) * 1.8f, 20f, 85f);
        }
        else if (daftarElemen.Count >= 2)
        {
            float jarakAntarElemen = Vector2.Distance(daftarElemen[0].anchoredPosition, daftarElemen[1].anchoredPosition);
            skorKedekatan = Mathf.Clamp(85f - Mathf.Abs(jarakAntarElemen - 25f), 30f, 85f);
        }
        else
        {
            skorKedekatan = 25f;
        }
        targetKedekatan = Mathf.Clamp(skorKedekatan, 0f, 100f);

        SkorTotalSekarang = Mathf.RoundToInt((targetKeseimbangan + targetKontras + targetKedekatan) / 3f);

        PerbaruiTipsOtomatis(adaLatarBelakang, daftarTeks.Count);
    }

    private void PerbaruiTipsOtomatis(bool adaLatar, int jumlahTeks)
    {
        if (teksTips == null) return;

        if (!adaLatar)
        {
            teksTips.text = "Tips: tambahkan latar belakang agar poster lebih menarik";
            return;
        }

        if (jumlahTeks == 0)
        {
            teksTips.text = "Tips: tambahkan judul dan subjudul pada poster";
            return;
        }

        if (jumlahTeks == 1)
        {
            teksTips.text = "Tips: tambahkan subjudul dan dekatkan dengan judul";
            return;
        }

        if (targetKeseimbangan >= 80f && targetKontras >= 80f && targetKedekatan >= 80f)
        {
            teksTips.text = "Tips: komposisi poster sudah seimbang dan siap dinilai!";
            return;
        }

        if (targetKedekatan <= targetKeseimbangan && targetKedekatan <= targetKontras)
            teksTips.text = "Tips: dekatkan subjudul dengan judul";
        else if (targetKeseimbangan <= targetKontras)
            teksTips.text = "Tips: atur posisi elemen agar seimbang kiri dan kanan";
        else
            teksTips.text = "Tips: perbesar ukuran judul agar kontras dengan subjudul";
    }

    private void PerbaruiTampilanUI()
    {
        float dt = Time.deltaTime * kecepatanSlider;

        if (sliderKeseimbangan != null)
            sliderKeseimbangan.value = Mathf.Lerp(sliderKeseimbangan.value, targetKeseimbangan, dt);
        if (teksPersenKeseimbangan != null)
            teksPersenKeseimbangan.text = Mathf.RoundToInt(sliderKeseimbangan != null ? sliderKeseimbangan.value : targetKeseimbangan) + "%";

        if (sliderKontras != null)
            sliderKontras.value = Mathf.Lerp(sliderKontras.value, targetKontras, dt);
        if (teksPersenKontras != null)
            teksPersenKontras.text = Mathf.RoundToInt(sliderKontras != null ? sliderKontras.value : targetKontras) + "%";

        if (sliderKedekatan != null)
            sliderKedekatan.value = Mathf.Lerp(sliderKedekatan.value, targetKedekatan, dt);
        if (teksPersenKedekatan != null)
            teksPersenKedekatan.text = Mathf.RoundToInt(sliderKedekatan != null ? sliderKedekatan.value : targetKedekatan) + "%";
    }
}