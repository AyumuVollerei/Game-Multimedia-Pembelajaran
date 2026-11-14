using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PosterActionManager : MonoBehaviour
{
    public static PosterActionManager Instance;

    [Header("Referensi Kanvas Poster")]
    public RectTransform areaPoster;

    [Header("Tombol Aksi (Opsional)")]
    public Button tombolUndo;
    public Button tombolRedo;

    [Header("SFX Klik (Opsional)")]
    public AudioSource audioSource;
    public AudioClip sfxKlik;

    [System.Serializable]
    private class DataElemen
    {
        public string namaObjek;
        public bool adalahTeks;
        public bool penuhSatuPoster;

        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 offsetMin;
        public Vector2 offsetMax;

        public Sprite sprite;
        public Color warnaGambar;
        public bool preserveAspect;

        public string isiTeks;
        public TMP_FontAsset fontAsset;
        public float ukuranFont;
        public Color warnaTeks;
    }

    private List<List<DataElemen>> riwayatUndo = new List<List<DataElemen>>();
    private List<List<DataElemen>> riwayatRedo = new List<List<DataElemen>>();

    private bool sedangMemulihkan = false;
    private int jumlahAnakSebelumnya = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SimpanKeRiwayat();
        PerbaruiStatusTombol();
    }

    void LateUpdate()
    {
        if (areaPoster == null || sedangMemulihkan) return;

        if (areaPoster.childCount != jumlahAnakSebelumnya)
        {
            jumlahAnakSebelumnya = areaPoster.childCount;
            SimpanKeRiwayat();
        }
    }

    public void PilihElemen(GameObject obj)
    {
        if (areaPoster == null) return;

        foreach (Transform anak in areaPoster)
        {
            ElemenPosterDraggable itemGambar = anak.GetComponent<ElemenPosterDraggable>();
            if (itemGambar != null)
                itemGambar.SetSorotan(anak.gameObject == obj);

            TeksResizableController itemTeks = anak.GetComponent<TeksResizableController>();
            if (itemTeks != null && anak.gameObject != obj)
                itemTeks.SetStatusTerpilih(false);
        }
    }

    public void CatatPerubahan()
    {
        if (sedangMemulihkan) return;
        SimpanKeRiwayat();
    }

    private void SimpanKeRiwayat()
    {
        if (areaPoster == null) return;

        List<DataElemen> snapshot = AmbilSnapshotPoster();
        riwayatUndo.Add(snapshot);
        riwayatRedo.Clear();

        if (riwayatUndo.Count > 30)
            riwayatUndo.RemoveAt(0);

        jumlahAnakSebelumnya = areaPoster.childCount;
        PerbaruiStatusTombol();
    }

    public void TombolReset()
    {
        MainkanSFX();
        if (areaPoster == null || areaPoster.childCount == 0) return;

        sedangMemulihkan = true;
        for (int i = areaPoster.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(areaPoster.GetChild(i).gameObject);
        }
        sedangMemulihkan = false;

        SimpanKeRiwayat();
    }

    public void TombolUndo()
    {
        MainkanSFX();
        if (riwayatUndo.Count <= 1) return;

        List<DataElemen> kondisiSekarang = riwayatUndo[riwayatUndo.Count - 1];
        riwayatUndo.RemoveAt(riwayatUndo.Count - 1);
        riwayatRedo.Add(kondisiSekarang);

        List<DataElemen> kondisiSebelumnya = riwayatUndo[riwayatUndo.Count - 1];
        PulihkanSnapshot(kondisiSebelumnya);
        PerbaruiStatusTombol();
    }

    public void TombolRedo()
    {
        MainkanSFX();
        if (riwayatRedo.Count == 0) return;

        List<DataElemen> kondisiBerikutnya = riwayatRedo[riwayatRedo.Count - 1];
        riwayatRedo.RemoveAt(riwayatRedo.Count - 1);
        riwayatUndo.Add(kondisiBerikutnya);

        PulihkanSnapshot(kondisiBerikutnya);
        PerbaruiStatusTombol();
    }

    private List<DataElemen> AmbilSnapshotPoster()
    {
        List<DataElemen> daftar = new List<DataElemen>();

        foreach (Transform anak in areaPoster)
        {
            RectTransform rt = anak.GetComponent<RectTransform>();
            TMP_InputField input = anak.GetComponent<TMP_InputField>();

            DataElemen data = new DataElemen
            {
                namaObjek = anak.name,
                anchorMin = rt.anchorMin,
                anchorMax = rt.anchorMax,
                pivot = rt.pivot,
                anchoredPosition = rt.anchoredPosition,
                sizeDelta = rt.sizeDelta,
                offsetMin = rt.offsetMin,
                offsetMax = rt.offsetMax
            };

            if (input != null)
            {
                data.adalahTeks = true;
                data.isiTeks = input.text;
                data.fontAsset = input.fontAsset;
                data.ukuranFont = input.pointSize;
                data.warnaTeks = input.textComponent != null ? input.textComponent.color : Color.black;
            }
            else
            {
                data.adalahTeks = false;
                Image img = anak.GetComponent<Image>();
                data.penuhSatuPoster = (anak.name == "Poster_LatarBelakang" || anak.name == "Poster_Overlay");
                if (img != null)
                {
                    data.sprite = img.sprite;
                    data.warnaGambar = img.color;
                    data.preserveAspect = img.preserveAspect;
                }
            }

            daftar.Add(data);
        }

        return daftar;
    }

    private void PulihkanSnapshot(List<DataElemen> snapshot)
    {
        sedangMemulihkan = true;

        for (int i = areaPoster.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(areaPoster.GetChild(i).gameObject);
        }

        foreach (DataElemen data in snapshot)
        {
            if (data.adalahTeks)
                BangunUlangTeks(data);
            else
                BangunUlangGambar(data);
        }

        jumlahAnakSebelumnya = areaPoster.childCount;
        sedangMemulihkan = false;
    }

    private void BangunUlangGambar(DataElemen data)
    {
        GameObject obj = new GameObject(data.namaObjek);
        obj.transform.SetParent(areaPoster, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = data.anchorMin;
        rt.anchorMax = data.anchorMax;
        rt.pivot = data.pivot;
        rt.anchoredPosition = data.anchoredPosition;
        rt.sizeDelta = data.sizeDelta;
        if (data.penuhSatuPoster)
        {
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        Image img = obj.AddComponent<Image>();
        img.sprite = data.sprite;
        img.color = data.warnaGambar;
        img.preserveAspect = data.preserveAspect;
        img.raycastTarget = !data.penuhSatuPoster;

        if (!data.penuhSatuPoster)
        {
            ElemenPosterDraggable drag = obj.AddComponent<ElemenPosterDraggable>();
            drag.areaBatas = areaPoster;
        }
    }

    private void BangunUlangTeks(DataElemen data)
    {
        GameObject objTeks = new GameObject(data.namaObjek);
        objTeks.transform.SetParent(areaPoster, false);

        RectTransform rectUtama = objTeks.AddComponent<RectTransform>();
        rectUtama.sizeDelta = data.sizeDelta;
        rectUtama.anchoredPosition = data.anchoredPosition;

        Image bgKotak = objTeks.AddComponent<Image>();
        bgKotak.color = new Color(1f, 1f, 1f, 0.01f);

        GameObject objIsiTeks = new GameObject("Text");
        objIsiTeks.transform.SetParent(objTeks.transform, false);
        RectTransform rectTeks = objIsiTeks.AddComponent<RectTransform>();
        rectTeks.anchorMin = Vector2.zero;
        rectTeks.anchorMax = Vector2.one;
        rectTeks.offsetMin = new Vector2(3f, 2f);
        rectTeks.offsetMax = new Vector2(-3f, -2f);

        TextMeshProUGUI komponenTeks = objIsiTeks.AddComponent<TextMeshProUGUI>();
        komponenTeks.font = data.fontAsset;
        komponenTeks.fontSize = data.ukuranFont;
        komponenTeks.color = data.warnaTeks;
        komponenTeks.alignment = TextAlignmentOptions.Center;
        komponenTeks.textWrappingMode = TextWrappingModes.Normal;

        GameObject objPlaceholder = new GameObject("Placeholder");
        objPlaceholder.transform.SetParent(objTeks.transform, false);
        RectTransform rectPh = objPlaceholder.AddComponent<RectTransform>();
        rectPh.anchorMin = Vector2.zero;
        rectPh.anchorMax = Vector2.one;
        rectPh.offsetMin = new Vector2(3f, 2f);
        rectPh.offsetMax = new Vector2(-3f, -2f);

        TextMeshProUGUI komponenPh = objPlaceholder.AddComponent<TextMeshProUGUI>();
        komponenPh.font = data.fontAsset;
        komponenPh.fontSize = data.ukuranFont;
        komponenPh.color = new Color(0.2f, 0.2f, 0.2f, 0.45f);
        komponenPh.alignment = TextAlignmentOptions.Center;
        komponenPh.text = "Ketik di sini...";

        TMP_InputField inputField = objTeks.AddComponent<TMP_InputField>();
        inputField.textViewport = rectUtama;
        inputField.textComponent = komponenTeks;
        inputField.placeholder = komponenPh;
        inputField.fontAsset = data.fontAsset;
        inputField.pointSize = data.ukuranFont;
        inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
        inputField.text = data.isiTeks;

        TeksResizableController kontrolTeks = objTeks.AddComponent<TeksResizableController>();
        kontrolTeks.Inisialisasi(areaPoster, inputField, komponenTeks, komponenPh, bgKotak);
        kontrolTeks.SetStatusTerpilih(false);
    }

    private void PerbaruiStatusTombol()
    {
        if (tombolUndo != null)
            tombolUndo.interactable = riwayatUndo.Count > 1;

        if (tombolRedo != null)
            tombolRedo.interactable = riwayatRedo.Count > 0;
    }

    private void MainkanSFX()
    {
        if (audioSource != null && sfxKlik != null)
            audioSource.PlayOneShot(sfxKlik);
    }
}