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

Aplikacja jest dystrybuowana jako **samodzielny plik wykonywalny (.exe)**, co oznacza, że nie wymaga instalacji, konfiguracji środowiska uruchomieniowego ani Microsoft Store.

### Jak zacząć?
1. Przejdź do zakładki **[Releases]** (Wydania) na tej stronie.
2. Pobierz najnowszą wersję pliku `TechStoreApp.exe`.
3. Uruchom pobrany plik na komputerze z systemem Windows 10/11.

*Uwaga: Przy pierwszym uruchomieniu aplikacja automatycznie utworzy lokalną bazę danych i wypełni ją przykładowym asortymentem (baza SQLite / LocalDB wbudowana w środowisko). Może to zająć kilka sekund.*

### Domyślne konto administratora
Aplikacja od razu generuje konto, które pozwala na przetestowanie wszystkich funkcji:
*   **Email:** `admin`
*   **Hasło:** `admin`

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