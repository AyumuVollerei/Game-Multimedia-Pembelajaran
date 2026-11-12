using UnityEngine;
using UnityEngine.UI;

public class AutoProgressBar : MonoBehaviour
{
    public Slider progressBar;
    
    [Header("Pengaturan Level")]
    public int levelSaatIni = 1; // Anda cukup mengetik "1" untuk Level 1, "2" untuk Level 2 di Inspector
    public float kecepatan = 1.5f;

    private float targetNilai; // Disembunyikan karena akan dihitung otomatis

    void Start()
    {
        // Otomatis mengubah Level 1 menjadi 0, Level 2 menjadi 1, dst.
        targetNilai = levelSaatIni - 1; 

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }
    }

    void Update()
    {
        if (progressBar != null && progressBar.value < targetNilai)
        {
            progressBar.value = Mathf.MoveTowards(progressBar.value, targetNilai, kecepatan * Time.deltaTime);
        }
    }
}