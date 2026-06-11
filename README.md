# TechStoreApp 💻🛒

TechStoreApp to nowoczesna aplikacja okienkowa (Desktop) typu e-commerce, stworzona z myślą o systemie Windows. Aplikacja pozwala użytkownikom na przeglądanie katalogu produktów elektronicznych, zarządzanie koszykiem oraz składanie zamówień. Została wyposażona w rozbudowany Panel Administratora do zarządzania asortymentem, użytkownikami i zamówieniami.

Aplikacja została zaprojektowana w architekturze **WinUI 3** (Windows App SDK) i działa jako samodzielny plik wykonywalny (`.exe` - tryb *unpackaged*), co oznacza, że nie wymaga instalacji przez Microsoft Store.

---

## 🌟 Główne funkcje

### 👤 Dla Użytkownika (Klienta)
*   **Katalog Produktów:** Przeglądanie sprzętu z możliwością wyszukiwania po nazwie oraz zaawansowanego filtrowania (kategoria, przedział cenowy).
*   **Koszyk Zakupowy:** Dodawanie/usuwanie produktów, podgląd sumy oraz dynamiczne naliczanie kosztów dostawy.
*   **Składanie Zamówień:** Formularz wyboru kuriera (zintegrowany z bazą dostawców), metody płatności oraz adresu dostawy.
*   **Historia Zamówień:** Szczegółowy podgląd złożonych zamówień, wliczając w to ceny jednostkowe kupionych produktów w momencie transakcji.
*   **Konto Użytkownika:** Możliwość edycji danych osobowych, zmiany hasła oraz **personalizacji wyglądu aplikacji (Motyw Jasny/Ciemny)**.

### 🛡️ Dla Administratora
*   **Zarządzanie Produktami:** Dodawanie, edycja i usuwanie produktów. Sortowanie i wyszukiwanie asortymentu w panelu.
*   **Walidacja SKU:** Zabezpieczenie przed dodaniem produktów o powielającym się kodzie magazynowym.
*   **Zarządzanie Użytkownikami:** Podgląd bazy klientów z wbudowaną wyszukiwarką. Możliwość awansowania na administratora, usuwania kont oraz resetowania haseł (domyślnie na `1234`).
*   **Zarządzanie Zamówieniami:** Pełen dostęp do historii zamówień wszystkich klientów z opcją ich usuwania (anulowania).

---

## 🛠️ Technologie

*   **Interfejs Użytkownika:** WinUI 3 (Windows App SDK)
*   **Język:** C# (.NET 10.0)
*   **Baza Danych:** Microsoft SQL Server (LocalDB)
*   **ORM:** Entity Framework Core
*   **Wzorzec Projektowy:** MVVM (Model-View-ViewModel)

---

## 🚀 Instalacja i Uruchomienie

### Wymagania wstępne
1.  Zainstalowany **.NET SDK** (zgodny z wersją używaną w projekcie).
2.  Zainstalowany **SQL Server Express LocalDB** (wymagany do lokalnego działania bazy danych).
3.  Zainstalowany **Windows App SDK** (komponenty środowiska uruchomieniowego są pakowane razem z aplikacją w trybie SelfContained).

### Kompilacja do pliku .exe
Aplikacja jest skonfigurowana do wydania jako samodzielny plik (Single File) bez pakowania do instalatora MSIX. Aby wygenerować gotowy do uruchomienia plik `.exe` dla architektury 64-bitowej, otwórz terminal (np. PowerShell) w głównym folderze i wpisz:

```powershell
dotnet publish TechStoreApp\TechStoreApp.csproj -p:PublishProfile=win-x64 -c Release
```

Gotowy plik `TechStoreApp.exe` znajdziesz w ścieżce:
`TechStoreApp\bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\`

### Pierwsze uruchomienie
Podczas pierwszego uruchomienia aplikacji (metoda `EnsureDatabaseCreatedAndSeeded`), system automatycznie wygeneruje bazę danych i wypełni ją danymi testowymi:
*   Kategorie (Laptopy, Smartfony itp.)
*   Przewoźnicy (DHL, InPost, DPD itp.)
*   Przykładowe produkty (po 10 dla każdej kategorii)
*   **Domyślne konto administratora**

---

## 📖 Instrukcja Obsługi

### Logowanie jako Administrator
Aby uzyskać dostęp do ukrytej zakładki **"Panel Administratora"** na pasku nawigacyjnym, użyj wygenerowanego automatycznie konta:

*   **Email:** `admin`
*   **Hasło:** `admin`

### Nawigacja po aplikacji
*   **Sklep:** Główny widok katalogu. Użyj lupy, aby znaleźć sprzęt, lub skorzystaj z filtrów kategorii i ceny na górnym pasku.
*   **Koszyk:** Po wybraniu produktów, przejdź tutaj, aby uzupełnić dane adresowe, wybrać kuriera (co zaktualizuje łączną cenę) i opłacić zamówienie.
*   **Moje Zamówienia:** Zakładka dostępna po zalogowaniu. Rozwiń zamówienie, aby zobaczyć jego detale.
*   **Zaloguj się / Profil (na samym dole):** Miejsce do logowania. Po zalogowaniu zmienia się w profil, z którego możesz przejść do swoich danych, zmiany hasła lub kliknąć ikonę **Zębatki (Ustawienia)**, aby zmienić motyw aplikacji lub wylogować się z systemu.
*   **Panel Administratora:** Widoczny tylko dla kont ze statusem Admin. Podzielony na zakładki:
    *   *Produkty:* Lista asortymentu po lewej stronie. Kliknij produkt, aby jego szczegóły pojawiły się w formularzu edycji po prawej.
    *   *Użytkownicy:* Lista kont z wyszukiwarką. Z tego poziomu zresetujesz hasło użytkownikowi, usuniesz go lub przejrzysz i usuniesz jego zamówienia w panelu po prawej stronie.

---
*Projekt stworzony w ramach demonstracji możliwości WinUI 3 oraz EF Core w aplikacjach okienkowych.*