# Log Refactor — Ekstraksi Business Logic ke Service Layer

Dokumen ini mencatat perubahan refactoring yang dilakukan untuk menambahkan **Service Layer** serta meningkatkan pemisahan tanggung jawab (Separation of Concerns) dan kualitas kode (Clean Code).

**Tanggal:** 24 Juli 2026

## Ringkasan

Perubahan yang dilakukan:

* Menambahkan **Service Layer** untuk mengelola business logic terkait supplier.
* Menambahkan `OperationResult<T>` sebagai standar hasil operasi dari Service.
* Memperbarui `SupplierController` agar menggunakan `ISupplierService` dan menangani hasil operasi melalui `OperationResult`.

---

## File yang Ditambahkan

### `TechnicalTest/Services/OperationResult.cs`

Menyediakan class generic sederhana yang digunakan sebagai pembungkus hasil operasi, terdiri dari:

* `Success`
* `Message`
* `Data`

serta beberapa helper method untuk mempermudah pembuatan hasil operasi.

### `TechnicalTest/Services/ISupplierService.cs`

Berisi interface yang mendefinisikan seluruh operasi bisnis terkait Supplier yang digunakan oleh Controller.

### `TechnicalTest/Services/SupplierService.cs`

Implementasi Service yang menggunakan `ISupplierRepository`, bertugas untuk:

* Melakukan validasi business rule seperti pengecekan kode supplier yang duplikat.
* Mengembalikan hasil operasi dalam bentuk `OperationResult` untuk proses Create, Update, dan Delete.

---

## File yang Dimodifikasi

### `TechnicalTest/Controllers/SupplierController.cs`

Perubahan yang dilakukan:

* Mengganti penggunaan langsung `ISupplierRepository` menjadi `ISupplierService`.
* Controller kini memanggil Service dan memproses hasil `OperationResult` untuk:

  * Menambahkan `ModelState` jika terjadi validasi.
  * Mengembalikan HTTP Status Code yang sesuai apabila terjadi kegagalan operasi.

---

## Alasan dan Tujuan Desain

Refactoring ini dilakukan dengan tujuan:

* Menjadikan **Repository** hanya bertanggung jawab terhadap akses data (Stored Procedure dan proses mapping).
* Memindahkan business rule, seperti validasi kode supplier yang duplikat, ke dalam **Service Layer**.
* Menjaga Controller tetap sederhana sehingga hanya berfokus pada proses request dan response HTTP.
* Menggunakan `OperationResult<T>` sebagai pola sederhana untuk mengembalikan hasil operasi tanpa perlu menggunakan Exception pada kondisi validasi yang memang diperkirakan terjadi.

---

## Peningkatan Clean Code

Beberapa prinsip Clean Code yang diterapkan setelah refactoring:

### Single Responsibility Principle (SRP)

* Controller bertanggung jawab terhadap proses HTTP Request dan Response.
* Service bertanggung jawab terhadap Business Logic.
* Repository bertanggung jawab terhadap akses data.

### Mengurangi Duplikasi Kode

Proses pengecekan kode supplier yang duplikat tidak lagi dilakukan di beberapa action Controller, tetapi dipusatkan di dalam Service.

### Penanganan Hasil Operasi Lebih Jelas

Service mengembalikan informasi keberhasilan maupun kegagalan secara eksplisit melalui `OperationResult`, sehingga Controller tidak perlu mengetahui detail implementasi business logic.

### Menghapus Comment yang tidak perlu
setiap command yang tidak perlu dihapus dari baris

