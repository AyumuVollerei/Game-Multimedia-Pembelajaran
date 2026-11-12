using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Fungsi untuk tombol Mulai Petualangan
    public void TombolMulai()
    {
        // Langsung memuat Scene bernama "QuestBriefing"
        SceneManager.LoadScene("QuestBriefing");
    }

    // Fungsi untuk tombol Cara Bermain (sementara kita beri pesan Log saja)
    public void TombolCaraBermain()
    {
        Debug.Log("Panel Cara Bermain akan muncul di sini!");
    }
}