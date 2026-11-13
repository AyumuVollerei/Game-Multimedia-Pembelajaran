using UnityEngine;
using UnityEngine.SceneManagement;

public class CaraBermainManager : MonoBehaviour
{
    // Fungsi untuk tombol "Mulai Petualangan"
    public void TombolMulaiPetualangan()
    {
        Debug.Log("Mulai Petualangan diklik! Pindah ke Quest Briefing...");
        // Memuat Scene QuestBriefing (sesuaikan jika nama scene tujuan berbeda)
        SceneManager.LoadScene("QuestBriefing");
    }

    // Fungsi untuk tombol "Kembali"
    public void TombolKembali()
    {
        Debug.Log("Kembali diklik! Pindah ke Main Menu...");
        // Memuat ulang Scene MainMenu
        SceneManager.LoadScene("MainMenu");
    }
}