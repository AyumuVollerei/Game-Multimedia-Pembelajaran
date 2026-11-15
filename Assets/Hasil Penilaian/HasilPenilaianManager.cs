using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HasilPenilaianManager : MonoBehaviour
{
    [Header("1. Poster Karya Pemain (Sebelah Kiri)")]
    public RawImage previewPoster;

    [Header("2. Judul & Rincian 4 Kriteria Penilaian")]
    public TMP_Text teksJudulHasil;              // "Poster Disetujui Klien"
    public TMP_Text teksBintangGambarUtama;      // Baris 1: Gambar Utama
    public TMP_Text teksBintangJenisHuruf;       // Baris 2: Jenis Huruf
    public TMP_Text teksBintangTataLetak;        // Baris 3: Tata Letak
    public TMP_Text teksBintangKesesuaianBrief;  // Baris 4: Kesesuaian Brief

    [Header("3. Badge, Ikon Badge & Skor Akhir")]
    public Image gambarIkonBadge;                // Objek "gambar badge" (lingkaran di sebelah tulisan Badge)
    public Sprite spriteBadge0_20;               // Ikon badge nilai 0 - 20 (Desainer Magang)
    public Sprite spriteBadge21_60;              // Ikon badge nilai 21 - 60 (Desainer Junior)
    public Sprite spriteBadge61_90;              // Ikon badge nilai 61 - 90 (Desainer Kreatif)
    public Sprite spriteBadge91_100;             // Ikon badge nilai 91 - 100 (Art Director)
    public TMP_Text teksBadge;                   // "Badge: Art Director"
    public TMP_Text teksSkorAkhir;               // "Skor akhir 100 dari 100"

    [Header("4. Efek Suara & Navigasi")]
    public AudioSource audioSource;
    public AudioClip sfxSelesai;
    public AudioClip sfxKlik;
    public float jedaPindahScene = 0.15f;
    public string namaSceneMainMenu = "MainMenu";

    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && sfxSelesai != null)
            audioSource.PlayOneShot(sfxSelesai);

        // 1. Tampilkan hasil foto poster dari Level 4 ke RawImage sebelah kiri
        if (previewPoster != null && DataPosterLevel4.gambarPoster != null)
        {
            previewPoster.texture = DataPosterLevel4.gambarPoster;
            previewPoster.color = Color.white;
        }

        // 2. Hitung jumlah prinsip tata letak yang lolos di Level 4 (0 - 5)
        int jumlahPrinsipLolos = 0;
        if (DataPosterLevel4.kontrasLolos) jumlahPrinsipLolos++;
        if (DataPosterLevel4.pengulanganLolos) jumlahPrinsipLolos++;
        if (DataPosterLevel4.kesejajaranLolos) jumlahPrinsipLolos++;
        if (DataPosterLevel4.kedekatanLolos) jumlahPrinsipLolos++;
        if (DataPosterLevel4.keseimbanganLolos) jumlahPrinsipLolos++;

        int totalSkor = Mathf.Clamp(PlayerPrefs.GetInt("TotalSkor", 0), 0, 100);
        int pilihanHuruf = PlayerPrefs.GetInt("PilihanHuruf", 0);
        int pilihanGambar = PlayerPrefs.GetInt("PilihanGambar", 0);

        // 3. Tentukan bintang masing-masing baris (skala 1 - 5)
        int bintangGambar = (pilihanGambar == 0) ? 5 : 3;
        int bintangHuruf = (pilihanHuruf == 0) ? 5 : 3;
        int bintangTataLetak = Mathf.Max(1, jumlahPrinsipLolos);
        int bintangBrief = (totalSkor >= 91) ? 5 : ((totalSkor >= 61) ? 4 : 3);

        if (teksBintangGambarUtama != null) teksBintangGambarUtama.text = bintangGambar + " / 5 Bintang";
        if (teksBintangJenisHuruf != null) teksBintangJenisHuruf.text = bintangHuruf + " / 5 Bintang";
        if (teksBintangTataLetak != null) teksBintangTataLetak.text = bintangTataLetak + " / 5 Bintang";
        if (teksBintangKesesuaianBrief != null) teksBintangKesesuaianBrief.text = bintangBrief + " / 5 Bintang";

        // 4. Tentukan Judul, Nama Badge, & Gambar Ikon Badge sesuai 4 Rentang Nilai
        string namaBadge;
        Sprite ikonTerpilih;

        if (totalSkor >= 91) // 91 - 100
        {
            if (teksJudulHasil != null) teksJudulHasil.text = "Poster Disetujui Klien";
            namaBadge = "Art Director";
            ikonTerpilih = spriteBadge91_100;
        }
        else if (totalSkor >= 61) // 61 - 90
        {
            if (teksJudulHasil != null) teksJudulHasil.text = "Poster Disetujui Klien";
            namaBadge = "Desainer Kreatif";
            ikonTerpilih = spriteBadge61_90;
        }
        else if (totalSkor >= 21) // 21 - 60
        {
            if (teksJudulHasil != null) teksJudulHasil.text = "Poster Disetujui dengan Catatan";
            namaBadge = "Desainer Junior";
            ikonTerpilih = spriteBadge21_60;
        }
        else // 0 - 20
        {
            if (teksJudulHasil != null) teksJudulHasil.text = "Poster Perlu Revisi";
            namaBadge = "Desainer Magang";
            ikonTerpilih = spriteBadge0_20;
        }

        if (gambarIkonBadge != null && ikonTerpilih != null)
        {
            gambarIkonBadge.sprite = ikonTerpilih;
            gambarIkonBadge.color = Color.white;
        }

        if (teksBadge != null) teksBadge.text = "Badge: " + namaBadge;
        if (teksSkorAkhir != null) teksSkorAkhir.text = "Skor akhir " + totalSkor + " dari 100";
    }

    // Hubungkan ke tombol "Kembali ke Menu Utama" (bg menu utama)
    public void TombolKembaliKeMenuUtama()
    {
        if (sedangPindah) return;

        PlayerPrefs.SetInt("TotalSkor", 0);
        PlayerPrefs.SetInt("NyawaPemain", 3);
        PlayerPrefs.SetInt("PoinLevel4", 0);
        PlayerPrefs.Save();

        DataPosterLevel4.gambarPoster = null;
        StartCoroutine(PindahDenganSuara(namaSceneMainMenu));
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