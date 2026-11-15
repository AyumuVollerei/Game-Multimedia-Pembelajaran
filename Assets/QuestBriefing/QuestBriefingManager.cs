using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestBriefingManager : MonoBehaviour
{
    [Header("Pengaturan Efek Suara (SFX)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;
    public float jedaPindahScene = 0.15f;

    private bool sedangPindah = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Fungsi untuk tombol "Kembali"
    public void TombolKembali()
    {
        if (sedangPindah) return;
        StartCoroutine(PindahDenganSuara("MainMenu"));
    }

    // Fungsi untuk tombol "Terima Quest"
    public void TombolTerimaQuest()
    {
        if (sedangPindah) return;
        Debug.Log("Quest Diterima! Menyiapkan Level 1...");
        StartCoroutine(PindahDenganSuara("Level1"));
    }

    private IEnumerator PindahDenganSuara(string namaSceneTujuan)
    {
        sedangPindah = true;

        if (audioSource != null && sfxKlik != null)
        {
            audioSource.PlayOneShot(sfxKlik);
            yield return new WaitForSecondsRealtime(jedaPindahScene);
        }

        SceneManager.LoadScene(namaSceneTujuan);
    }
}