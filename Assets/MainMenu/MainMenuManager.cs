using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;
    public float jedaPindahScene = 0.2f; // Jeda 0.2 detik agar suara klik tidak terpotong

    private bool sedangPindah = false;

    // Fungsi untuk tombol Mulai Petualangan
    public void TombolMulai()
    {
        if (sedangPindah) return;

        // Reset skor ke 0 setiap kali memulai permainan baru
        PlayerPrefs.SetInt("TotalSkor", 0);
        PlayerPrefs.Save();

        StartCoroutine(PindahSceneDenganSuara("QuestBriefing"));
    }

    // Fungsi untuk tombol Cara Bermain
    public void TombolCaraBermain()
    {
        if (sedangPindah) return;

        StartCoroutine(PindahSceneDenganSuara("CaraBermain"));
    }

    private IEnumerator PindahSceneDenganSuara(string namaScene)
    {
        sedangPindah = true;

        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
            yield return new WaitForSeconds(jedaPindahScene);
        }

        SceneManager.LoadScene(namaScene);
    }
}