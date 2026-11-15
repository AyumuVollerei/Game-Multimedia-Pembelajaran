using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CekJawabanLevel1 : MonoBehaviour
{
    [Header("1. Tombol Pilihan Palet")]
    public Image[] gambarTombolPalet;
    public float skalaPaletDipilih = 1.06f;
    public float skalaPaletNormal = 1.0f;

    [Header("2. Tombol Pilihan Alasan (Figma)")]
    public Image[] gambarTombolAlasan;
    public TMP_Text[] teksTombolAlasan;
    
    public Sprite spriteAlasanNormal;
    public Sprite spriteAlasanDipilih;

    public Color warnaTeksAlasanNormal = new Color32(20, 30, 43, 255);
    public Color warnaTeksAlasanDipilih = Color.white;

    [Header("3. Sistem Nyawa & Header UI")]
    public TMP_Text teksNyawa;
    public int nyawaDefault = 3;

    [Header("4. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    private int paletTerpilih = -1;
    private int alasanTerpilih = -1;

    private int kunciPalet = 0;
    private int kunciAlasan = 0;
    private int nyawaSekarang;

    void Start()
    {
        nyawaSekarang = PlayerPrefs.GetInt("NyawaPemain", nyawaDefault);
        
        if (nyawaSekarang <= 0)
        {
            nyawaSekarang = nyawaDefault;
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();
        }

        PerbaruiTeksNyawa();
        ResetTampilanPalet();
        ResetTampilanAlasan();
    }

    private void PerbaruiTeksNyawa()
    {
        if (teksNyawa != null)
            teksNyawa.text = "Nyawa " + nyawaSekarang;
    }

    private void MainkanSuaraKlik()
    {
        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
        }
    }

    public void PilihPalet(int indeks)
    {
        MainkanSuaraKlik();
        paletTerpilih = indeks;

        for (int i = 0; i < gambarTombolPalet.Length; i++)
        {
            if (gambarTombolPalet[i] == null) continue;

            if (i == indeks)
            {
                gambarTombolPalet[i].transform.localScale = Vector3.one * skalaPaletDipilih;
                gambarTombolPalet[i].color = Color.white;
            }
            else
            {
                gambarTombolPalet[i].transform.localScale = Vector3.one * skalaPaletNormal;
                gambarTombolPalet[i].color = new Color(1f, 1f, 1f, 0.55f);
            }
        }
    }

    public void PilihAlasan(int indeks)
    {
        MainkanSuaraKlik();
        alasanTerpilih = indeks;
        ResetTampilanAlasan();

        if (indeks >= 0 && indeks < gambarTombolAlasan.Length)
        {
            if (gambarTombolAlasan[indeks] != null && spriteAlasanDipilih != null)
                gambarTombolAlasan[indeks].sprite = spriteAlasanDipilih;

            if (indeks < teksTombolAlasan.Length && teksTombolAlasan[indeks] != null)
                teksTombolAlasan[indeks].color = warnaTeksAlasanDipilih;
        }
    }

    private void ResetTampilanPalet()
    {
        for (int i = 0; i < gambarTombolPalet.Length; i++)
        {
            if (gambarTombolPalet[i] != null)
            {
                gambarTombolPalet[i].transform.localScale = Vector3.one * skalaPaletNormal;
                gambarTombolPalet[i].color = Color.white;
            }
        }
    }

    private void ResetTampilanAlasan()
    {
        for (int i = 0; i < gambarTombolAlasan.Length; i++)
        {
            if (gambarTombolAlasan[i] != null)
            {
                gambarTombolAlasan[i].color = Color.white;
                if (spriteAlasanNormal != null)
                    gambarTombolAlasan[i].sprite = spriteAlasanNormal;
            }

            if (i < teksTombolAlasan.Length && teksTombolAlasan[i] != null)
                teksTombolAlasan[i].color = warnaTeksAlasanNormal;
        }
    }

    public void TombolCekJawabanLevel1()
    {
        MainkanSuaraKlik();

        if (paletTerpilih == -1 || alasanTerpilih == -1)
        {
            Debug.Log("Pilih palet dan alasannya terlebih dahulu!");
            return;
        }

        PlayerPrefs.SetInt("PilihanPalet", paletTerpilih);
        PlayerPrefs.SetInt("PilihanAlasan", alasanTerpilih);

        if (paletTerpilih == kunciPalet && alasanTerpilih == kunciAlasan)
        {
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();

            SceneManager.LoadScene("level 1_benar");
        }
        else
        {
            nyawaSekarang = Mathf.Max(0, nyawaSekarang - 1);
            PlayerPrefs.SetInt("NyawaPemain", nyawaSekarang);
            PlayerPrefs.Save();

            PerbaruiTeksNyawa();
            SceneManager.LoadScene("level 1_salah");
        }
    }
}