using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level4CekHasilManager : MonoBehaviour
{
    [Header("1. Header UI")]
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksSkorHeader;

    [Header("2. Tampilan Hasil Poster (Sebelah Kiri)")]
    public RawImage previewPoster;

    [Header("3. Teks Status 5 Prinsip Tata Letak")]
    public TMP_Text teksStatusKontras;
    public TMP_Text teksStatusPengulangan;
    public TMP_Text teksStatusKesejajaran;
    public TMP_Text teksStatusKedekatan;
    public TMP_Text teksStatusKeseimbangan;

    public Color warnaLolos = new Color(0.15f, 0.55f, 0.45f);          // Hijau toska
    public Color warnaPerluDiperbaiki = new Color(0.85f, 0.35f, 0.2f); // Oranye kemerahan

    [Header("4. Pengaturan Skor Level 4")]
    public int poinLolosSemua = 40;

    [Header("5. Efek Suara & Navigasi")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;
    public float jedaPindahScene = 0.15f;
    public string namaSceneLevel4 = "Level4";
    public string namaSceneKirimKlien = "Hasil Penilaian";

    private bool sedangPindah = false;
    private bool semuaPrinsipLolos = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // 1. Tampilkan skor & nyawa terkini
        if (teksNyawaHeader != null)
            teksNyawaHeader.text = "Nyawa " + PlayerPrefs.GetInt("NyawaPemain", 3);

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + PlayerPrefs.GetInt("TotalSkor", 0);

        // 2. Tempelkan gambar poster dari Level 4 ke RawImage sebelah kiri
        if (previewPoster != null && DataPosterLevel4.gambarPoster != null)
        {
            previewPoster.texture = DataPosterLevel4.gambarPoster;
            previewPoster.color = Color.white;
        }

        // 3. Perbarui tulisan Lolos / Perlu Diperbaiki sesuai hasil pengecekan
        AturTeksStatus(teksStatusKontras, DataPosterLevel4.kontrasLolos);
        AturTeksStatus(teksStatusPengulangan, DataPosterLevel4.pengulanganLolos);
        AturTeksStatus(teksStatusKesejajaran, DataPosterLevel4.kesejajaranLolos);
        AturTeksStatus(teksStatusKedekatan, DataPosterLevel4.kedekatanLolos);
        AturTeksStatus(teksStatusKeseimbangan, DataPosterLevel4.keseimbanganLolos);

        // 4. Cek apakah kelima prinsip lolos semua
        semuaPrinsipLolos = DataPosterLevel4.kontrasLolos
                         && DataPosterLevel4.pengulanganLolos
                         && DataPosterLevel4.kesejajaranLolos
                         && DataPosterLevel4.kedekatanLolos
                         && DataPosterLevel4.keseimbanganLolos;
    }

    private void AturTeksStatus(TMP_Text teksTarget, bool isLolos)
    {
        if (teksTarget == null) return;

        teksTarget.text = isLolos ? "Lolos" : "Perlu Diperbaiki";
        teksTarget.color = isLolos ? warnaLolos : warnaPerluDiperbaiki;
    }

    // Hubungkan ke tombol "Kembali" (tidak menambah skor agar tidak bisa diulang-ulang)
    public void TombolKembali()
    {
        if (sedangPindah) return;
        StartCoroutine(PindahDenganSuara(namaSceneLevel4));
    }

    // Hubungkan ke tombol "Kirim ke client"
    public void TombolKirimKeKlien()
    {
        if (sedangPindah) return;

        int poinDidapat = semuaPrinsipLolos ? poinLolosSemua : 0;
        int skorSebelumnya = PlayerPrefs.GetInt("TotalSkor", 0);
        int skorAkhir = skorSebelumnya + poinDidapat;

        PlayerPrefs.SetInt("TotalSkor", skorAkhir);
        PlayerPrefs.SetInt("PoinLevel4", poinDidapat);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorAkhir;

        StartCoroutine(PindahDenganSuara(namaSceneKirimKlien));
    }

    private IEnumerator PindahDenganSuara(string namaSceneTujuan)
    {
        sedangPindah = true;

        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
            yield return new WaitForSecondsRealtime(jedaPindahScene);
        }

        SceneManager.LoadScene(namaSceneTujuan);
    }
}