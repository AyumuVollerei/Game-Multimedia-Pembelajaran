using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CekJawabanLevel2 : MonoBehaviour
{
    [Header("1. Header UI (Nyawa, Skor & Progress Bar)")]
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksSkorHeader;
    public Slider progressBar;
    public float nilaiProgressLevel2 = 1f;
    public int nyawaDefault = 3;

    [Header("2. Tombol Pilihan Jenis Huruf (3 Kartu Atas)")]
    public Image[] gambarTombolHuruf; // 0: Display Rounded, 1: Serif Klasik, 2: Monospace
    public float skalaHurufDipilih = 1.06f;
    public float skalaHurufNormal = 1.0f;
    [Range(0f, 1f)]
    public float transparansiTidakDipilih = 0.55f;

    [Header("3. Teks Pratinjau yang Berubah Font-nya (Kartu Bawah)")]
    public TMP_Text teksJudul48;
    public TMP_Text teksSubjudul24;
    public TMP_Text teksIsi14;

    [Header("4. Aset Font TMP (Urutan Sesuai Kartu Atas)")]
    public TMP_FontAsset fontDisplayRounded; // Index 0 (Jawaban Benar)
    public TMP_FontAsset fontSerifKlasik;    // Index 1 (Salah)
    public TMP_FontAsset fontMonospace;      // Index 2 (Salah)

    [Header("5. Kunci Jawaban & Navigasi Scene")]
    public int kunciHuruf = 0; // 0 = Display Rounded
    public string namaSceneBenar = "level 2_benar";
    public string namaSceneSalah = "level 2_salah";

    [Header("6. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;
    public float jedaPindahScene = 0.15f;

    private int hurufTerpilih = -1;
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
            progressBar.value = nilaiProgressLevel2;

        ResetTampilanHuruf();
    }

    private void MainkanSuaraKlik()
    {
        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
        }
    }

    public void PilihHuruf(int indeks)
    {
        MainkanSuaraKlik();
        hurufTerpilih = indeks;

        for (int i = 0; i < gambarTombolHuruf.Length; i++)
        {
            if (gambarTombolHuruf[i] == null) continue;

            CanvasGroup cg = gambarTombolHuruf[i].GetComponent<CanvasGroup>();
            if (cg == null)
                cg = gambarTombolHuruf[i].gameObject.AddComponent<CanvasGroup>();

            if (i == indeks)
            {
                gambarTombolHuruf[i].transform.localScale = Vector3.one * skalaHurufDipilih;
                gambarTombolHuruf[i].color = Color.white;
                cg.alpha = 1f;
            }
            else
            {
                gambarTombolHuruf[i].transform.localScale = Vector3.one * skalaHurufNormal;
                gambarTombolHuruf[i].color = new Color(1f, 1f, 1f, transparansiTidakDipilih);
                cg.alpha = transparansiTidakDipilih;
            }
        }

        TMP_FontAsset fontDipilih = null;
        if (indeks == 0) fontDipilih = fontDisplayRounded;
        else if (indeks == 1) fontDipilih = fontSerifKlasik;
        else if (indeks == 2) fontDipilih = fontMonospace;

        if (fontDipilih != null)
        {
            if (teksJudul48 != null) teksJudul48.font = fontDipilih;
            if (teksSubjudul24 != null) teksSubjudul24.font = fontDipilih;
            if (teksIsi14 != null) teksIsi14.font = fontDipilih;
        }
    }

    private void ResetTampilanHuruf()
    {
        for (int i = 0; i < gambarTombolHuruf.Length; i++)
        {
            if (gambarTombolHuruf[i] == null) continue;

            CanvasGroup cg = gambarTombolHuruf[i].GetComponent<CanvasGroup>();
            if (cg == null)
                cg = gambarTombolHuruf[i].gameObject.AddComponent<CanvasGroup>();

            gambarTombolHuruf[i].transform.localScale = Vector3.one * skalaHurufNormal;
            gambarTombolHuruf[i].color = Color.white;
            cg.alpha = 1f;
        }
    }

    public void TombolCekJawabanLevel2()
    {
        if (sedangPindah) return;
        MainkanSuaraKlik();

        if (hurufTerpilih == -1)
        {
            Debug.Log("Pilih jenis huruf terlebih dahulu!");
            return;
        }

        StartCoroutine(ProsesCekJawaban());
    }

    private IEnumerator ProsesCekJawaban()
    {
        sedangPindah = true;

        if (sfxKlik != null)
            yield return new WaitForSecondsRealtime(jedaPindahScene);

        PlayerPrefs.SetInt("PilihanHuruf", hurufTerpilih);

        if (hurufTerpilih == kunciHuruf)
        {
            // BENAR: Nyawa tetap -> pindah ke level 2_benar
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();

            SceneManager.LoadScene(namaSceneBenar);
        }
        else
        {
            // SALAH: Nyawa berkurang 1 -> pindah ke level 2_salah
            nyawaSekarang = Mathf.Max(0, nyawaSekarang - 1);
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();

            if (teksNyawaHeader != null)
                teksNyawaHeader.text = "Nyawa " + nyawaSekarang;

            SceneManager.LoadScene(namaSceneSalah);
        }
    }
}