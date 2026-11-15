using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Level1SalahManager : MonoBehaviour
{
    [Header("1. Pengaturan Skor & Nyawa Header")]
    public TMP_Text teksSkorHeader;
    public TMP_Text teksNyawaHeader;
    public TMP_Text teksLencanaSkor;
    public int skorSalahSatuBenar = 15;
    public int skorSalahSemua = 5;

    [Header("2. Sorotan Palet yang Dipilih Pemain")]
    public Image[] gambarTombolPalet;
    public float skalaDipilih = 1.06f;

    [Header("3. Sorotan Alasan yang Dipilih Pemain")]
    public Image[] gambarTombolAlasan;
    public TMP_Text[] teksTombolAlasan;
    public Sprite spriteAlasanNormal;
    public Sprite spriteAlasanDipilih;
    public Color warnaTeksAlasanNormal = new Color32(20, 30, 43, 255);
    public Color warnaTeksAlasanDipilih = Color.white;

    [Header("4. Teks Penjelasan Salah (Otomatis)")]
    public TMP_Text teksPenjelasanSalah;
    [TextArea(2, 4)]
    public string penjelasanSalahKomplementer = "Perpaduan biru tua dan oranye lebih memberikan kesan berani dan dinamis, bukan suasana hangat yang lembut. Coba perhatikan kembali kombinasi warna yang terdiri dari merah, oranye, dan kuning!";
    [TextArea(2, 4)]
    public string penjelasanSalahMonokromatik = "Monokromatik biru hanya menggunakan gradasi satu warna biru yang memberikan kesan dingin dan tenang, bukan suasana hangat. Coba cari palet dengan nuansa merah, oranye, dan kuning!";
    [TextArea(2, 4)]
    public string penjelasanSalahAlasan1 = "Palet Analogus sudah tepat untuk suasana hangat, tetapi alasan 'Biru selalu aman untuk semua poster' tidak sesuai karena setiap proyek memiliki kebutuhan suasana (mood) yang berbeda.";
    [TextArea(2, 4)]
    public string penjelasanSalahAlasan2 = "Palet Analogus sudah tepat untuk suasana hangat, tetapi banyaknya warna bukanlah patokan utama dalam desain—kesesuaian warna dengan mood brief adalah hal yang paling penting.";

    [Header("5. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxWrong;
    public AudioClip sfxKlik;
    public float jedaLanjut = 0.2f;

    [Header("6. Navigasi Tombol Lanjut")]
    public string namaSceneBerikutnya = "Level2";
    public string namaSceneHabisNyawa = "MainMenu";
    public TMP_Text teksTombolLanjut;

    private int kunciPalet = 0;
    private int kunciAlasan = 0;
    private int sisaNyawa = 3;
    private bool sedangPindah = false;

    void Start()
    {
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

        int paletDipilih = PlayerPrefs.GetInt("PilihanPalet", 1);
        int alasanDipilih = PlayerPrefs.GetInt("PilihanAlasan", 1);

        bool adaYangBenar = (paletDipilih == kunciPalet) || (alasanDipilih == kunciAlasan);
        int poinDidapat = adaYangBenar ? skorSalahSatuBenar : skorSalahSemua;

        PlayerPrefs.SetInt("TotalSkor", 0);
        int skorBaru = PlayerPrefs.GetInt("TotalSkor", 0) + poinDidapat;
        PlayerPrefs.SetInt("TotalSkor", skorBaru);
        PlayerPrefs.Save();

        if (teksSkorHeader != null)
            teksSkorHeader.text = "Score " + skorBaru;

        if (teksLencanaSkor != null)
            teksLencanaSkor.text = "+" + poinDidapat;

        TampilkanPilihanPalet(paletDipilih);
        TampilkanPilihanAlasan(alasanDipilih);
        TampilkanPenjelasanSesuaiPilihan(paletDipilih, alasanDipilih);
    }

    private void TampilkanPilihanPalet(int indeksDipilih)
    {
        for (int i = 0; i < gambarTombolPalet.Length; i++)
        {
            if (gambarTombolPalet[i] == null) continue;

            if (i == indeksDipilih)
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

    private void TampilkanPilihanAlasan(int indeksDipilih)
    {
        for (int i = 0; i < gambarTombolAlasan.Length; i++)
        {
            bool terpilih = (i == indeksDipilih);

            if (gambarTombolAlasan[i] != null)
            {
                gambarTombolAlasan[i].color = Color.white;
                if (terpilih && spriteAlasanDipilih != null)
                    gambarTombolAlasan[i].sprite = spriteAlasanDipilih;
                else if (!terpilih && spriteAlasanNormal != null)
                    gambarTombolAlasan[i].sprite = spriteAlasanNormal;
            }

            if (i < teksTombolAlasan.Length && teksTombolAlasan[i] != null)
            {
                teksTombolAlasan[i].color = terpilih ? warnaTeksAlasanDipilih : warnaTeksAlasanNormal;
            }
        }
    }

    private void TampilkanPenjelasanSesuaiPilihan(int paletDipilih, int alasanDipilih)
    {
        if (teksPenjelasanSalah == null) return;

        if (paletDipilih == 1)
            teksPenjelasanSalah.text = penjelasanSalahKomplementer;
        else if (paletDipilih == 2)
            teksPenjelasanSalah.text = penjelasanSalahMonokromatik;
        else
            teksPenjelasanSalah.text = (alasanDipilih == 1) ? penjelasanSalahAlasan1 : penjelasanSalahAlasan2;
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

        string targetLanjut = (string.IsNullOrEmpty(namaSceneBerikutnya) || namaSceneBerikutnya == "MainMenu" || namaSceneBerikutnya == "Level1")
            ? "Level2"
            : namaSceneBerikutnya;

        string sceneTujuan = (sisaNyawa > 0) ? targetLanjut : namaSceneHabisNyawa;
        Debug.Log("Sisa Nyawa: " + sisaNyawa + " -> Pindah ke: " + sceneTujuan);
        SceneManager.LoadScene(sceneTujuan);
    }
}