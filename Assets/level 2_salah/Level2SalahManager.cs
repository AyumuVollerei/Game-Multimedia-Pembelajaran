using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level2SalahManager : MonoBehaviour
{
    [Header("1. Pengaturan Skor, Nyawa & Progress Bar")]
    public TMP_Text teksSkorHeader;
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksLencanaSkor;
    public Slider progressBar;
    public float nilaiProgressSelesaiLevel2 = 2f; // 2 dari Max Value 4
    public int poinSalahLevel2 = 0;

    [Header("2. Sorotan Kartu yang Dipilih Pemain")]
    public Image[] gambarKartuHuruf; // 0: Display Rounded, 1: Serif Klasik, 2: Monospace
    public float skalaDipilih = 1.06f;
    [Range(0f, 1f)]
    public float transparansiTidakDipilih = 0.55f;

    [Header("3. Teks Pratinjau yang Berubah Font-nya (Kartu Bawah)")]
    public TMP_Text teksJudul48;
    public TMP_Text teksSubjudul24;
    public TMP_Text teksIsi14;
    public TMP_FontAsset fontDisplayRounded; // Index 0
    public TMP_FontAsset fontSerifKlasik;    // Index 1
    public TMP_FontAsset fontMonospace;      // Index 2

    [Header("4. Teks Penjelasan Salah (Otomatis)")]
    public TMP_Text teksPenjelasanSalah;
    [TextArea(2, 4)]
    public string penjelasanSalahSerif = "Huruf Serif Klasik lebih memberikan kesan formal dan elegan, bukan suasana santai dan ramah. Coba pilih jenis huruf yang ujungnya membulat agar terasa lebih hangat!";
    [TextArea(2, 4)]
    public string penjelasanSalahMonospace = "Huruf Monospace memiliki jarak karakter yang kaku dan terkesan teknis seperti kode komputer. Coba cari jenis huruf yang lebih ramah dan santai untuk acara festival!";

    [Header("5. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxWrong;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("6. Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "Level3";
    public string namaSceneHabisNyawa = "MainMenu";
    public TMP_Text teksTombolLanjut;

    private int sisaNyawa = 3;
    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && sfxWrong != null)
        {
            audioSource.PlayOneShot(sfxWrong);
        }

        sisaNyawa = PlayerPrefs.GetInt("NyawaPemain", 3);
        if (teksNyawaHeader != null)
        {
            teksNyawaHeader.text = "Nyawa " + sisaNyawa;
        }

        if (sisaNyawa <= 0 && teksTombolLanjut != null)
        {
            teksTombolLanjut.text = "Menu Utama";
        }

        int skorSebelumnya = PlayerPrefs.GetInt("TotalSkor", 0);
        int skorBaru = skorSebelumnya + poinSalahLevel2;
        PlayerPrefs.SetInt("TotalSkor", skorBaru);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorBaru;

        if (teksLencanaSkor != null)
            teksLencanaSkor.text = "+" + poinSalahLevel2;

        if (progressBar != null)
            progressBar.value = nilaiProgressSelesaiLevel2;

        int pilihanPemain = PlayerPrefs.GetInt("PilihanHuruf", 1);
        SorotKartuDipilih(pilihanPemain);
        TerapkanFontPratinjau(pilihanPemain);
        TampilkanPenjelasan(pilihanPemain);
    }

    private void SorotKartuDipilih(int indeksDipilih)
    {
        if (gambarKartuHuruf == null || gambarKartuHuruf.Length == 0) return;

        for (int i = 0; i < gambarKartuHuruf.Length; i++)
        {
            if (gambarKartuHuruf[i] == null) continue;

            CanvasGroup cg = gambarKartuHuruf[i].GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = gambarKartuHuruf[i].gameObject.AddComponent<CanvasGroup>();
            }

            if (i == indeksDipilih)
            {
                gambarKartuHuruf[i].transform.localScale = Vector3.one * skalaDipilih;
                gambarKartuHuruf[i].color = Color.white;
                cg.alpha = 1f;
            }
            else
            {
                gambarKartuHuruf[i].transform.localScale = Vector3.one;
                gambarKartuHuruf[i].color = new Color(1f, 1f, 1f, transparansiTidakDipilih);
                cg.alpha = transparansiTidakDipilih;
            }
        }
    }

    private void TerapkanFontPratinjau(int indeksDipilih)
    {
        TMP_FontAsset fontDipilih = null;
        if (indeksDipilih == 0) fontDipilih = fontDisplayRounded;
        else if (indeksDipilih == 1) fontDipilih = fontSerifKlasik;
        else if (indeksDipilih == 2) fontDipilih = fontMonospace;

        if (fontDipilih != null)
        {
            if (teksJudul48 != null) teksJudul48.font = fontDipilih;
            if (teksSubjudul24 != null) teksSubjudul24.font = fontDipilih;
            if (teksIsi14 != null) teksIsi14.font = fontDipilih;
        }
    }

    private void TampilkanPenjelasan(int indeksDipilih)
    {
        if (teksPenjelasanSalah == null) return;

        if (indeksDipilih == 1)
            teksPenjelasanSalah.text = penjelasanSalahSerif;
        else if (indeksDipilih == 2)
            teksPenjelasanSalah.text = penjelasanSalahMonospace;
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
            ? "Level3"
            : namaSceneBerikutnya;

        string sceneTujuan = (sisaNyawa > 0) ? targetLanjut : namaSceneHabisNyawa;
        SceneManager.LoadScene(sceneTujuan);
    }
}