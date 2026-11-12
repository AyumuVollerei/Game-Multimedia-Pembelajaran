using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestBriefingManager : MonoBehaviour
{
    // Fungsi untuk tombol "Kembali"
    public void TombolKembali()
    {
        // Memuat ulang Scene MainMenu
        SceneManager.LoadScene("MainMenu");
    }

    // Fungsi untuk tombol "Terima Quest"
    public void TombolTerimaQuest()
    {
        Debug.Log("Quest Diterima! Menyiapkan Level 1...");

        // Memuat Scene Level1
        SceneManager.LoadScene("Level1");
    }
}