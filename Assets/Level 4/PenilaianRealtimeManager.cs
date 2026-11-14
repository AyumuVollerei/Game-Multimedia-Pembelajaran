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
        Image imgPoster = areaPoster.GetComponent<Image>();
        bool adaLatarBelakang = imgPoster != null && imgPoster.sprite != null;

        Transform trOverlay = areaPoster.parent != null ? areaPoster.parent.Find("slot overlay") : null;
        bool adaOverlay = trOverlay != null && trOverlay.GetComponent<Image>() != null && trOverlay.GetComponent<Image>().sprite != null;

        Transform wadahSlot = areaPoster.parent != null ? areaPoster.parent : areaPoster;
        PosterSlotTarget[] semuaSlot = wadahSlot.GetComponentsInChildren<PosterSlotTarget>();

        List<PosterSlotTarget> slotTerisi = new List<PosterSlotTarget>();
        List<PosterSlotTarget> slotCocok = new List<PosterSlotTarget>();
        List<PosterSlotTarget> slotTeksTerisi = new List<PosterSlotTarget>();
        PosterSlotTarget slotPertamaSalah = null;

        foreach (PosterSlotTarget slot in semuaSlot)
        {
            if (!slot.SudahTerisi) continue;

            slotTerisi.Add(slot);
            if (slot.AdalahTeksTerisi)
                slotTeksTerisi.Add(slot);

            if (slot.ApakahElemenCocok())
            {
                slotCocok.Add(slot);
            }
            else if (slotPertamaSalah == null)
            {
                slotPertamaSalah = slot;
            }
        }

        if (slotTerisi.Count == 0)
        {
            targetKeseimbangan = 0f;
            targetKontras = adaLatarBelakang ? 20f : 0f;
            targetKedekatan = 0f;
            SkorTotalSekarang = Mathf.RoundToInt((targetKeseimbangan + targetKontras + targetKedekatan) / 3f);

            if (teksTips != null)
            {
                teksTips.text = adaLatarBelakang
                    ? "Tips: isi slot judul, subjudul, dan ilustrasi sesuai tempatnya"
                    : "Tips: mulai dengan memasang latar belakang poster";
            }
            return;
        }

        // 1. KESEIMBANGAN (Distribusi atas, tengah, bawah + bobot elemen yang tepat slotnya)
        float bobotAtas = 0f, bobotTengah = 0f, bobotBawah = 0f;
        foreach (PosterSlotTarget slot in slotTerisi)
        {
            float nilaiSlot = slot.ApakahElemenCocok() ? 1f : 0.4f;
            float y = slot.GetComponent<RectTransform>().anchoredPosition.y;

            if (y > 80f) bobotAtas += nilaiSlot;
            else if (y >= -220f) bobotTengah += nilaiSlot;
            else bobotBawah += nilaiSlot;
        }
        float distribusiVertikal = (Mathf.Min(bobotAtas, 2f) / 2f * 35f)
                                + (Mathf.Min(bobotTengah, 2f) / 2f * 40f)
                                + (Mathf.Min(bobotBawah, 1f) * 25f);
        targetKeseimbangan = Mathf.Clamp(distribusiVertikal, 10f, 100f);

        // 2. KONTRAS (Latar belakang + kecocokan ketebalan font Judul vs Subjudul)
        float skorKontras = 0f;
        if (adaLatarBelakang) skorKontras += 35f;
        if (adaOverlay) skorKontras += 10f;

        int teksCocok = 0;
        foreach (PosterSlotTarget t in slotTeksTerisi)
        {
            if (t.ApakahElemenCocok()) teksCocok++;
        }

        if (teksCocok >= 2) skorKontras += 55f;
        else if (teksCocok == 1) skorKontras += 35f;
        else if (slotTeksTerisi.Count >= 1) skorKontras += 15f;

        targetKontras = Mathf.Clamp(skorKontras, 10f, 100f);

        // 3. KEDEKATAN (Kelengkapan slot & kesesuaian pasangan elemen)
        float skorKedekatan = 15f + (slotCocok.Count * 14f);
        if (slotTerisi.Count > slotCocok.Count)
        {
            // Penalti jika ada elemen yang salah masuk slot
            int jumlahSalah = slotTerisi.Count - slotCocok.Count;
            skorKedekatan -= jumlahSalah * 8f;
        }
        targetKedekatan = Mathf.Clamp(skorKedekatan, 10f, 100f);

        SkorTotalSekarang = Mathf.RoundToInt((targetKeseimbangan + targetKontras + targetKedekatan) / 3f);

        // 4. TIPS OTOMATIS BERDASARKAN KECOCOKAN SLOT
        if (teksTips != null)
        {
            if (!adaLatarBelakang)
                teksTips.text = "Tips: tambahkan latar belakang agar poster lebih hidup";
            else if (slotPertamaSalah != null)
                teksTips.text = "Tips: " + slotPertamaSalah.pesanJikaKurangCocok;
            else if (slotTeksTerisi.Count < 2)
                teksTips.text = "Tips: lengkapi font Judul (tebal) dan Subjudul (tipis)";
            else if (slotCocok.Count < 6)
                teksTips.text = "Tips: lengkapi semua 6 slot dengan elemen yang sesuai";
            else
                teksTips.text = "Tips: semua elemen terpasang tepat pada slotnya!";
        }
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