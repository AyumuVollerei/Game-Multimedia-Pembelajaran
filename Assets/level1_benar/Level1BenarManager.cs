using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level1BenarManager : MonoBehaviour
{
    [Header("Pengaturan Skor")]
    public TMP_Text teksSkorHeader;
    public int skorLevel1 = 30;

    [Header("Sorotan Jawaban Benar (Palet)")]
    public Image[] gambarTombolPalet; // 0: Analogus, 1: Komplementer, 2: Monokromatik
    public int indeksPaletBenar = 0;  // 0 = Analogus
    public float skalaDipilih = 1.06f;

    [Header("Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxCorrect;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "MainMenu"; // Ubah ke nama scene Level 2 jika sudah ada

    private bool sedangPindah = false;

    void Start()
    {
        // 1. Putar SFX jawaban benar
        if (audioSource != null && sfxCorrect != null)
        {
            audioSource.PlayOneShot(sfxCorrect);
        }

        // 2. Set skor menjadi 30 dan tampilkan di Header
        PlayerPrefs.SetInt("TotalSkor", 0);
        int skorSekarang = PlayerPrefs.GetInt("TotalSkor", 0) + skorLevel1;
        PlayerPrefs.SetInt("TotalSkor", skorSekarang);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
        {
            teksSkorHeader.text = "Score " + skorSekarang;
        }

        // 3. Tonjolkan kartu palet yang benar
        TampilkanPaletBenar();
    }

    private void TampilkanPaletBenar()
    {
        for (int i = 0; i < gambarTombolPalet.Length; i++)
        {
            if (gambarTombolPalet[i] == null) continue;

            if (i == indeksPaletBenar)
            {
                gambarTombolPalet[i].transform.localScale = Vector3.one * skalaDipilih;
                gambarTombolPalet[i].color = Color.white;
            }
            else
            {
                gambarTombolPalet[i].transform.localScale = Vector3.one;
                gambarTombolPalet[i].color = new Color(1f, 1f, 1f, 0.55f);
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
        SceneManager.LoadScene(namaSceneBerikutnya);
    }
}