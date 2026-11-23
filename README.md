# 🎨 Studio Quest

**Studio Quest** adalah game edukasi interaktif 2D berbasis Android yang mengajak pemain berperan sebagai seorang desainer grafis dalam menyelesaikan proyek desain poster dari klien. Pemain ditantang untuk mengambil keputusan visual yang tepat—mulai dari pemilihan palet warna, tipografi, ilustrasi utama, hingga penyusunan tata letak sesuai dengan kebutuhan *brief* klien.

---

## ✨ Fitur Utama

* **Simulasi Brief Klien Nyata:** Pemain membaca kebutuhan spesifik klien (target audiens, *mood*, dan ukuran media) sebelum mulai mendesain.
* **4 Tahap Tantangan Desain:**
  1. **Level 1 – Palet Warna:** Memilih kombinasi warna yang sesuai dengan suasana (*mood*) poster.
  2. **Level 2 – Jenis Huruf (Tipografi):** Menentukan gaya font yang tepat dan mudah dibaca.
  3. **Level 3 – Gambar Utama (Ilustrasi):** Memilih elemen visual utama yang paling relevan dengan tema acara.
  4. **Level 4 – Tata Letak & Revisi Klien:** Menyusun komposisi elemen desain dan menyesuaikan hasil akhir berdasarkan masukan klien.
* **Umpan Balik Edukatif Otomatis:** Setiap jawaban benar maupun salah dilengkapi penjelasan alasan desain secara spesifik untuk membantu pemain memahami prinsip dasar desain komunikasi visual.
* **Sistem Nyawa, Skor & Penilaian Akhir:** Dilengkapi indikator *progress bar*, perhitungan skor kumulatif, serta evaluasi akhir di halaman **Hasil Penilaian**.

---

## 🛠️ Teknologi yang Digunakan

* **Game Engine:** Unity 6 (`6000.6.8f1`)
* **Bahasa Pemrograman:** C#
* **UI & Tipografi:** Unity UI (uGUI) & TextMeshPro
* **Platform Target:** Android (ARM64, IL2CPP)

---

## 📂 Struktur Proyek Utama

```text
Assets/
├── Scenes/          # Seluruh scene permainan (SplashScreen, MainMenu, QuestBriefing, Level1–4, Hasil Penilaian)
├── Scripts/         # Logika permainan, manajemen skor/nyawa, dan navigasi antar-scene
├── Sprites/         # Aset karakter maskot, kartu ilustrasi, dan elemen antarmuka (UI)
└── Audio/           # Efek suara (SFX) interaksi tombol dan umpan balik jawaban
