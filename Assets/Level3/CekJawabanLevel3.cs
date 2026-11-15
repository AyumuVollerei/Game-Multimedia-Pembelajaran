using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CekJawabanLevel3 : MonoBehaviour
{
    [Header("1. Header UI (Nyawa, Skor & Progress Bar)")]
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksSkorHeader;
    public Slider progressBar;
    public float nilaiProgressLevel3 = 2f; // 2 dari Max Value 4
    public int nyawaDefault = 3;

    [Header("2. Tombol Pilihan Ilustrasi (4 Kartu)")]
    public Image[] gambarKartuIlustrasi; // 0: Cangkir kopi, 1: Biji kopi, 2: Siluet kota malam, 3: Bentuk abstrak
    public float skalaKartuDipilih = 1.06f;
    public float skalaKartuNormal = 1.0f;
    [Range(0f, 1f)]
    public float transparansiTidakDipilih = 0.55f;

    [Header("3. Kunci Jawaban & Navigasi Scene")]
    public int kunciGambar = 0; // 0 = Cangkir kopi (bisa diubah di Inspector jika perlu)
    public string namaSceneBenar = "level 3_benar";
    public string namaSceneSalah = "level 3_salah";

    [Header("4. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;
    public float jedaPindahScene = 0.15f;

    private int gambarTerpilih = -1;
    private int nyawaSekarang;
    private int skorSekarang;
    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        nyawaSekarang = PlayerPrefs.GetInt("NyawaPemain", nyawaDefault);
        skorSekarang = PlayerPrefs.GetInt("TotalSkor", 0);

        if (teksNyawaHeader != null)
            teksNyawaHeader.text = "Nyawa " + nyawaSekarang;

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorSekarang;

        if (progressBar != null)
            progressBar.value = nilaiProgressLevel3;

        ResetTampilanKartu();
    }

    private void MainkanSuaraKlik()
    {
        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
        }
    }

    // Dipanggil oleh Button OnClick masing-masing kartu (0, 1, 2, atau 3)
    public void PilihGambar(int indeks)
    {
        if (sedangPindah) return;

        MainkanSuaraKlik();
        gambarTerpilih = indeks;

        for (int i = 0; i < gambarKartuIlustrasi.Length; i++)
        {
            if (gambarKartuIlustrasi[i] == null) continue;

            CanvasGroup cg = gambarKartuIlustrasi[i].GetComponent<CanvasGroup>();
            if (cg == null)
                cg = gambarKartuIlustrasi[i].gameObject.AddComponent<CanvasGroup>();

            if (i == indeks)
            {
                gambarKartuIlustrasi[i].transform.localScale = Vector3.one * skalaKartuDipilih;
                gambarKartuIlustrasi[i].color = Color.white;
                cg.alpha = 1f;
            }
            else
            {
                gambarKartuIlustrasi[i].transform.localScale = Vector3.one * skalaKartuNormal;
                gambarKartuIlustrasi[i].color = new Color(1f, 1f, 1f, transparansiTidakDipilih);
                cg.alpha = transparansiTidakDipilih;
            }
        }
    }

    private void ResetTampilanKartu()
    {
        if (gambarKartuIlustrasi == null) return;

        for (int i = 0; i < gambarKartuIlustrasi.Length; i++)
        {
            if (gambarKartuIlustrasi[i] == null) continue;

            CanvasGroup cg = gambarKartuIlustrasi[i].GetComponent<CanvasGroup>();
            if (cg == null)
                cg = gambarKartuIlustrasi[i].gameObject.AddComponent<CanvasGroup>();

            gambarKartuIlustrasi[i].transform.localScale = Vector3.one * skalaKartuNormal;
            gambarKartuIlustrasi[i].color = Color.white;
            cg.alpha = 1f;
        }
    }

    // Dipanggil oleh tombol "Pilih Gambar"
    public void TombolPilihGambar()
    {
        if (sedangPindah) return;
        MainkanSuaraKlik();

        if (gambarTerpilih == -1)
        {
            Debug.Log("Pilih ilustrasi terlebih dahulu!");
            return;
        }

        StartCoroutine(ProsesCekJawaban());
    }

    private IEnumerator ProsesCekJawaban()
    {
        sedangPindah = true;

        if (sfxKlik != null)
            yield return new WaitForSecondsRealtime(jedaPindahScene);

        PlayerPrefs.SetInt("PilihanGambar", gambarTerpilih);

        if (gambarTerpilih == kunciGambar)
        {
            // BENAR: Nyawa tetap -> pindah ke level 3_benar
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();

            SceneManager.LoadScene(namaSceneBenar);
        }
        else
        {
            // SALAH: Nyawa berkurang 1 -> pindah ke level 3_salah
            nyawaSekarang = Mathf.Max(0, nyawaSekarang - 1);
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();

            if (teksNyawaHeader != null)
                teksNyawaHeader.text = "Nyawa " + nyawaSekarang;

            SceneManager.LoadScene(namaSceneSalah);
        }
    }
}