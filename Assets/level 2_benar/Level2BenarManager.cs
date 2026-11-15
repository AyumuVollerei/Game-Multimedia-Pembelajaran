using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level2BenarManager : MonoBehaviour
{
    [Header("1. Pengaturan Skor, Nyawa & Progress Bar")]
    public TMP_Text teksSkorHeader;
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksLencanaSkor;
    public Slider progressBar;
    public float nilaiProgressSelesaiLevel2 = 2f; // 2 dari Max Value 4 (50%)
    public int poinBenarLevel2 = 10;

    [Header("2. Sorotan Kartu Pilihan Huruf")]
    public Image[] gambarKartuHuruf; // 0: Display Rounded, 1: Serif Klasik, 2: Monospace
    public float skalaDipilih = 1.06f;
    [Range(0f, 1f)]
    public float transparansiTidakDipilih = 0.55f;

    [Header("3. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxCorrect;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("4. Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "Level3";

    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && sfxCorrect != null)
        {
            audioSource.PlayOneShot(sfxCorrect);
        }

        int sisaNyawa = PlayerPrefs.GetInt("NyawaPemain", 3);
        if (teksNyawaHeader != null)
        {
            teksNyawaHeader.text = "Nyawa " + sisaNyawa;
        }

        // Tambah +10 ke TotalSkor
        int skorSebelumnya = PlayerPrefs.GetInt("TotalSkor", 0);
        int skorBaru = skorSebelumnya + poinBenarLevel2;
        PlayerPrefs.SetInt("TotalSkor", skorBaru);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorBaru;

        if (teksLencanaSkor != null)
            teksLencanaSkor.text = "+" + poinBenarLevel2;

        if (progressBar != null)
            progressBar.value = nilaiProgressSelesaiLevel2;

        int pilihanPemain = PlayerPrefs.GetInt("PilihanHuruf", 0);
        SorotKartuDipilih(pilihanPemain);
    }

    private void SorotKartuDipilih(int indeksDipilih)
    {
        if (gambarKartuHuruf == null || gambarKartuHuruf.Length == 0) return;

        for (int i = 0; i < gambarKartuHuruf.Length; i++)
        {
            if (gambarKartuHuruf[i] == null) continue;

            if (i == indeksDipilih)
            {
                gambarKartuHuruf[i].transform.localScale = Vector3.one * skalaDipilih;
                gambarKartuHuruf[i].color = Color.white;
            }
            else
            {
                gambarKartuHuruf[i].transform.localScale = Vector3.one;
                gambarKartuHuruf[i].color = new Color(1f, 1f, 1f, transparansiTidakDipilih);
            }
        }
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
            yield return new WaitForSeconds(jedaLanjut);
        }

        string targetScene = (string.IsNullOrEmpty(namaSceneBerikutnya) || namaSceneBerikutnya == "MainMenu")
            ? "Level3"
            : namaSceneBerikutnya;

        SceneManager.LoadScene(targetScene);
    }
}