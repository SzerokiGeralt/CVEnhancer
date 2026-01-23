using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVEnhancer.DTO;
using System.Text.RegularExpressions;

namespace CVEnhancer.Services
{
    public class ExtractionService
    {
        // Minimalna lista stop-words PL/EN (możesz rozbudować)
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "i","oraz","to","w","we","z","na","do","się","jest",
            "od","za","o","u","po",
            "praca","pracy","pracy","znajomość","doświadczenie","000","wymagania","praca","lub","b2b","min","pln","zakres","obowiązków","umiejętność","oferujemy","mile","widziane","opieka","pracę","rozwój","medyczna",
            "uop","ci","współpraca","podstawy","tworzenie","rozwoju","prywatna","doświadczenia","utrzymanie","firma","dni","dofinansowanie","aplikacji","cd","opis","umowa","dobra","przez","szukamy","brutto",
            "wynagrodzenie","możliwość","wsparcie","nasze","zdalnie","stanowiska","co","twój","udział","budżet","rekrutacji","rozwiązań","rozmowa","podsumowanie","narzędzi","który","netto","vat",
            "która","minimum","hybrydowo","osoby","godziny","przy","nas","działania","benefity","hr","oferta","lata","wymagane","sp","techniczna","podstawowa","stanowisko","projekcie","chęć","zespole",
            "wdrażanie","problemów","masz","usług","technologii","poziomie","20","30","biuro","szkoleniowy","tryb","boot","szkolenia","firmie","nauki","nowych","języka","pl","które","treści",
            "chce","in","pakiet","obejmuje","pracowników","oferty","elastyczne","biurze","10","multisport","integracyjne","benefitów","firmy","zgłoszeń","narzędzia","zakresu","kodu","zespołem","bez","biura",
            "karta","tworzymy","dodatkowe","dzień","praktyczna","zł","jakości","konfiguracja","wygenerowane","pełnej","ogłoszenia","rekrutacyjnego","jesteśmy","obsługa","office","obsługi","hybrydowym","realizacji","jeśli","czas",
            "26","contact","center","umiejętności","środowisku","umowę","dostęp","platformy","program","szkoleń","platforma","analiza","zadanie","takich","sklepów","decyzja","100","wydajności",
            "zdalnej","obszarze","wiedza","12","zajęć","sportowych","ubezpieczenie","zarządzania","poszukujemy","zmian","11","osobowych","procesu","zgodnie","klientów","ich","naszych","będziesz",
            "będzie","zadania","team","ciebie","lubi","realny","mentoring","współpracy","zatrudnienie","tym","rok","studiów","doświadczeniem","projekty","ma","lokalizacja","integracja","dbanie",
            "onboarding","16","dokładność","wpływ","support","branży","warszawie","wykorzystaniem","technicznych","bardzo","core","raportów","życie","obsługę","wdrożeń","poprzez","systemami","dnia","czego","rola",
            "znasz","chcesz","dodatkowy","zlecenie","sportowa","jakość","wiedzy","urządzeń","systemach","ms","emerytalny","świadczenia","planowaniu","współpracę","start","portfolio","środowiska","warszawa","komunikatywność",
            "samodzielność","200","pomagać","przygotowanie","sprzętu","kursów","wrocław","22","pracujemy","stabilne","stabilność","angielskiego","mikroserwisów","zakresie","modelu","usługi","jednostkowe",
            "centrum","polsce","najmniej","rpa","uipath","realizację","biegła","cv","przetwarzanie","więcej","wiedzę","rozwiązania","jako","twoje","częścią","zawsze","klienta","znajomości","jazdy","kat",
            "posiadasz","łączy","grupy","oparciu","rocznie","wypoczynku","możliwością","polski","błędów","b2","elastyczny","kraków","krótkie","spotkanie","tylko","potrafi","opcjonalnie","miejsce","programowaniu","podstawowe",
            "sprzęt","fintech","płatności","raportowanie","płatne","kursy","certyfikacje","optymalizacja","produktów","posiadać","wdrażaniu","narzędziami","zagadnień","budowanie","zależności","stacjonarnych","naciskiem",
            "rozwiązywanie","organizacja","głównie","ów","konfiguracji","zespołami","incydentów","kontroli","wersji","tygodniu","17","ale","też","znajomością","projekt","zapewnienie","tworzeniu","zatrudnienia",
            "roli","podstaw","bogaty","spotkania","kawa","herbata","produkty","polityk","warta","firmą","serwisowych","pakietu","dodatkowo","prosimy","klauzuli","zgodę","zawartych","potrzeb","niż","nowoczesnych",
            "naszą","dokumentów","umożliwia","zasobów","kto","już","razem","ekspertów","ogłoszenie","stawiamy","rozwiązywania","biznesowych","praktyce","sposób","pisanie","logiki","biznesem","cel","pracować","uczyć",
            "pasji","zainteresowanie","naukę","świąteczne","wolne","model","integracje","nasz","nową","nowe","zespoły","biznesowe","odpowiedzialność","oczekujemy","zadań","odpowiedzialności","stanowisku","nadzór","zewnętrzne","zgłaszanie",
            "tauron","wynagrodzenia","premia","wielu","pracowniczy","którym","również","finansowej","praktycznym","dynamicznym","procesie","tworzenia","funkcjonalności","uczenia","warunki","grafik","stawka","kandydat","link","max",
            "sportowe","itp","innych","dostępności","krótka","polska","czasem","wolnego","wejść","aktualizacje","cierpliwość","może","prawo","źródeł","decyzje","60","gdańsk","24","dobrze","monitoring",
            "wymagana","takimi","czy","oferuje",
        };

        public ExtractedSkillsDTO ExtractKeywords(string rawText, int maxKeywords = 25)
        {
            var tokens = Tokenize(rawText);

            // proste zliczanie częstości
            var freq = tokens
                .GroupBy(t => t)
                .Select(g => new { Term = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(maxKeywords)
                .Select(x => x.Term)
                .ToList();

            return new ExtractedSkillsDTO { Keywords = freq };
        }

        public List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

            text = text.ToLowerInvariant();

            // zamień wszystko co nie jest literą/cyfrą na spacje
            text = Regex.Replace(text, @"[^\p{L}\p{Nd}]+", " ");

            var tokens = text
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => t.Length >= 2)
                .Where(t => !StopWords.Contains(t))
                .ToList();

            return tokens;
        }
    }
}
