using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level3BenarManager : MonoBehaviour
{
    [Header("1. Pengaturan Skor, Nyawa & Progress Bar")]
    public TMP_Text teksSkorHeader;
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksLencanaSkor;
    public Slider progressBar;
    public float nilaiProgressSelesaiLevel3 = 3f; // 3 dari Max Value 4
    public int poinBenarLevel3 = 20;

    [Header("2. Sorotan Kartu Jawaban Benar")]
    public Image[] gambarKartuIlustrasi; // 0: Cangkir kopi, 1: Biji kopi, 2: Siluet kota malam, 3: Bentuk abstrak
    public int indeksJawabanBenar = 0;   // 0 = Cangkir kopi
    public float skalaDipilih = 1.06f;
    [Range(0f, 1f)]
    public float transparansiTidakDipilih = 0.55f;

    [Header("3. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxCorrect;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("4. Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "Level4";

    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // 1. Putar suara jawaban benar saat scene terbuka
        if (audioSource != null && sfxCorrect != null)
        {
            audioSource.PlayOneShot(sfxCorrect);
        }

        // 2. Tampilkan sisa nyawa terkini
        int sisaNyawa = PlayerPrefs.GetInt("NyawaPemain", 3);
        if (teksNyawaHeader != null)
        {
            teksNyawaHeader.text = "Nyawa " + sisaNyawa;
        }

        // 3. Tambahkan skor (+20) ke TotalSkor dari level sebelumnya
        int skorSebelumnya = PlayerPrefs.GetInt("TotalSkor", 0);
        int skorBaru = skorSebelumnya + poinBenarLevel3;
        PlayerPrefs.SetInt("TotalSkor", skorBaru);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorBaru;

        if (teksLencanaSkor != null)
            teksLencanaSkor.text = "+" + poinBenarLevel3;

        // 4. Perbarui progress bar menjadi 3/4
        if (progressBar != null)
            progressBar.value = nilaiProgressSelesaiLevel3;

        // 5. Sorot kartu jawaban benar (Cangkir kopi) dan redupkan 3 kartu lainnya
        SorotKartuBenar();
    }

    private void SorotKartuBenar()
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

            if (i == indeksJawabanBenar)
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

        string targetLanjut = string.IsNullOrEmpty(namaSceneBerikutnya) ? "Level4" : namaSceneBerikutnya;
        SceneManager.LoadScene(targetLanjut);
    }
}