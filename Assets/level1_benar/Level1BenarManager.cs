using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level1BenarManager : MonoBehaviour
{
    [Header("Pengaturan Skor & Nyawa Header")]
    public TMP_Text teksSkorHeader;
    public TMP_Text teksNyawaHeader;
    public int skorLevel1 = 30;

    [Header("Sorotan Jawaban Benar (Palet)")]
    public Image[] gambarTombolPalet;
    public int indeksPaletBenar = 0;
    public float skalaDipilih = 1.06f;

    [Header("Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxCorrect;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "Level2";

    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource != null && sfxCorrect != null)
        {
            audioSource.PlayOneShot(sfxCorrect);
        }

        PlayerPrefs.SetInt("TotalSkor", 0);
        int skorSekarang = PlayerPrefs.GetInt("TotalSkor", 0) + skorLevel1;
        PlayerPrefs.SetInt("TotalSkor", skorSekarang);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
        {
            teksSkorHeader.text = "Score " + skorSekarang;
        }

        int sisaNyawa = PlayerPrefs.GetInt("NyawaPemain", 3);
        if (teksNyawaHeader != null)
        {
            teksNyawaHeader.text = "Nyawa " + sisaNyawa;
        }

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

        string tujuan = (string.IsNullOrEmpty(namaSceneBerikutnya) || namaSceneBerikutnya == "MainMenu")
            ? "Level2"
            : namaSceneBerikutnya;

        SceneManager.LoadScene(tujuan);
    }
}