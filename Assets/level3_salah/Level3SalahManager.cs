using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level3SalahManager : MonoBehaviour
{
    [Header("1. Pengaturan Skor, Nyawa & Progress Bar")]
    public TMP_Text teksSkorHeader;
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksLencanaSkor;
    public Slider progressBar;
    public float nilaiProgressSelesaiLevel3 = 3f; // 3 dari Max Value 4
    public int poinSalahLevel3 = 0;

    [Header("2. Sorotan Kartu yang Dipilih Pemain")]
    public Image[] gambarKartuIlustrasi; // 0: Cangkir kopi, 1: Biji kopi, 2: Siluet kota malam, 3: Bentuk abstrak
    public float skalaDipilih = 1.06f;
    [Range(0f, 1f)]
    public float transparansiTidakDipilih = 0.55f;

    [Header("3. Teks Penjelasan Salah (Otomatis)")]
    public TMP_Text teksPenjelasanSalah;
    [TextArea(2, 4)]
    public string penjelasanSalahBijiKopi = "Ilustrasi biji kopi terlalu fokus pada bahan mentah dan kurang menggambarkan suasana acara festival. Coba pilih gambar yang lebih langsung menunjukkan pengalaman menikmati kopi!";
    [TextArea(2, 4)]
    public string penjelasanSalahSiluetKota = "Siluet kota malam terasa terlalu umum dan tidak spesifik menunjukkan tema kopi. Coba cari ilustrasi utama yang langsung berkaitan dengan tema festival kopi!";
    [TextArea(2, 4)]
    public string penjelasanSalahBentukAbstrak = "Bentuk abstrak hanya sekadar mengisi ruang kosong tanpa menjelaskan isi acara kepada pembaca. Coba pilih ilustrasi yang langsung mewakili tema festival kopi!";

    [Header("4. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxWrong;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("5. Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "Level4";
    public string namaSceneHabisNyawa = "MainMenu";
    public TMP_Text teksTombolLanjut;

    private int sisaNyawa = 3;
    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // 1. Putar suara jawaban salah saat scene terbuka
        if (audioSource != null && sfxWrong != null)
        {
            audioSource.PlayOneShot(sfxWrong);
        }

        // 2. Ambil sisa nyawa terkini (sudah dikurangi 1 di Level3)
        sisaNyawa = PlayerPrefs.GetInt("NyawaPemain", 3);
        if (teksNyawaHeader != null)
        {
            teksNyawaHeader.text = "Nyawa " + sisaNyawa;
        }

        if (sisaNyawa <= 0 && teksTombolLanjut != null)
        {
            teksTombolLanjut.text = "Menu Utama";
        }

        // 3. Skor tetap (+0) melanjutkan skor dari level sebelumnya
        int skorSebelumnya = PlayerPrefs.GetInt("TotalSkor", 0);
        int skorBaru = skorSebelumnya + poinSalahLevel3;
        PlayerPrefs.SetInt("TotalSkor", skorBaru);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorBaru;

        if (teksLencanaSkor != null)
            teksLencanaSkor.text = "+" + poinSalahLevel3;

        // 4. Perbarui progress bar menjadi 3/4
        if (progressBar != null)
            progressBar.value = nilaiProgressSelesaiLevel3;

        // 5. Sorot pilihan salah pemain & tampilkan penjelasan yang sesuai
        int pilihanPemain = PlayerPrefs.GetInt("PilihanGambar", 1);
        SorotKartuDipilih(pilihanPemain);
        TampilkanPenjelasan(pilihanPemain);
    }

    private void SorotKartuDipilih(int indeksDipilih)
    {
        if (gambarKartuIlustrasi == null || gambarKartuIlustrasi.Length == 0) return;

        for (int i = 0; i < gambarKartuIlustrasi.Length; i++)
        {
            if (gambarKartuIlustrasi[i] == null) continue;

            CanvasGroup cg = gambarKartuIlustrasi[i].GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = gambarKartuIlustrasi[i].gameObject.AddComponent<CanvasGroup>();
            }

            if (i == indeksDipilih)
            {
                gambarKartuIlustrasi[i].transform.localScale = Vector3.one * skalaDipilih;
                gambarKartuIlustrasi[i].color = Color.white;
                cg.alpha = 1f;
            }
            else
            {
                gambarKartuIlustrasi[i].transform.localScale = Vector3.one;
                gambarKartuIlustrasi[i].color = new Color(1f, 1f, 1f, transparansiTidakDipilih);
                cg.alpha = transparansiTidakDipilih;
            }
        }
    }

    private void TampilkanPenjelasan(int indeksDipilih)
    {
        if (teksPenjelasanSalah == null) return;

        if (indeksDipilih == 1)
            teksPenjelasanSalah.text = penjelasanSalahBijiKopi;
        else if (indeksDipilih == 2)
            teksPenjelasanSalah.text = penjelasanSalahSiluetKota;
        else if (indeksDipilih == 3)
            teksPenjelasanSalah.text = penjelasanSalahBentukAbstrak;
    }

    public void TombolLanjut()
    {
        if (sedangPindah) return;
        StartCoroutine(PindahDenganSuara());
    }

    private IEnumerator PindahDenganSuara()
    {
        sedangPindah = true;

        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
            yield return new WaitForSecondsRealtime(jedaLanjut);
        }

        string targetLanjut = (string.IsNullOrEmpty(namaSceneBerikutnya) || namaSceneBerikutnya == "MainMenu")
            ? "Level4"
            : namaSceneBerikutnya;

        string sceneTujuan = (sisaNyawa > 0) ? targetLanjut : namaSceneHabisNyawa;
        SceneManager.LoadScene(sceneTujuan);
    }
}