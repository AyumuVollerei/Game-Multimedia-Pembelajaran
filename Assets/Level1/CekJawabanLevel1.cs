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
    
    public Sprite spriteAlasanNormal;   // Kapsul krem (Vector (7)_0)
    public Sprite spriteAlasanDipilih;  // Kapsul biru tua (Vector (6)_0)

    public Color warnaTeksAlasanNormal = new Color32(20, 30, 43, 255);
    public Color warnaTeksAlasanDipilih = Color.white;

    [Header("3. Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    private int paletTerpilih = -1;
    private int alasanTerpilih = -1;

    private int kunciPalet = 0;
    private int kunciAlasan = 0;

    void Start()
    {
        ResetTampilanPalet();
        ResetTampilanAlasan();
    }

    // Fungsi pembantu untuk membunyikan SFX klik
    private void MainkanSuaraKlik()
    {
        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
        }
    }

    public void PilihPalet(int indeks)
    {
        MainkanSuaraKlik(); // Bunyi saat kartu palet diklik
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
        MainkanSuaraKlik(); // Bunyi saat tombol alasan diklik
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
        MainkanSuaraKlik(); // Bunyi saat tombol Cek Jawaban diklik

        if (paletTerpilih == -1 || alasanTerpilih == -1)
        {
            Debug.Log("Pilih palet dan alasannya terlebih dahulu!");
            return;
        }

        PlayerPrefs.SetInt("PilihanPalet", paletTerpilih);
        PlayerPrefs.SetInt("PilihanAlasan", alasanTerpilih);
        PlayerPrefs.Save();

        if (paletTerpilih == kunciPalet && alasanTerpilih == kunciAlasan)
        {
            SceneManager.LoadScene("level 1_benar");
        }
        else
        {
            SceneManager.LoadScene("level 1_salah");
        }
    }
}