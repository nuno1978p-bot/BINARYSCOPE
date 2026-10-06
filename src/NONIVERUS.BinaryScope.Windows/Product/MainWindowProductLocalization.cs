using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace NONIVERUS.BinaryScope.Windows.Product;

/// <summary>
/// Runtime localization bridge for the v0.1.6 analyzer chrome.
/// This deliberately localizes product/analyzer UI labels only.
/// Deep analysis output and exported reports remain within the documented
/// v0.1.6 localization boundary and are not claimed as fully localized.
/// </summary>
public static class MainWindowProductLocalization
{
    private sealed record Entry(
        string En,
        string Pt,
        string Es,
        string Fr,
        string De,
        string It,
        string Pl,
        string Nl,
        string Cs,
        string Ro)
    {
        public string For(string languageCode) => languageCode switch
        {
            "pt" => Pt,
            "es" => Es,
            "fr" => Fr,
            "de" => De,
            "it" => It,
            "pl" => Pl,
            "nl" => Nl,
            "cs" => Cs,
            "ro" => Ro,
            _ => En
        };

        public IEnumerable<string> All()
        {
            yield return En;
            yield return Pt;
            yield return Es;
            yield return Fr;
            yield return De;
            yield return It;
            yield return Pl;
            yield return Nl;
            yield return Cs;
            yield return Ro;
        }
    }

    private static readonly Entry[] Entries =
    [
        new("Android App Bundle · DEX evidence · Google Play local preflight",
            "Android App Bundle · evidência DEX · pré-verificação local Google Play",
            "Android App Bundle · evidencia DEX · preflight local de Google Play",
            "Android App Bundle · preuves DEX · précontrôle local Google Play",
            "Android App Bundle · DEX-Nachweise · lokaler Google-Play-Preflight",
            "Android App Bundle · evidenze DEX · preflight locale Google Play",
            "Android App Bundle · dane DEX · lokalny preflight Google Play",
            "Android App Bundle · DEX-bewijs · lokale Google Play-preflight",
            "Android App Bundle · důkazy DEX · místní preflight Google Play",
            "Android App Bundle · dovezi DEX · preflight local Google Play"),

        new("Compare with .aab",
            "Comparar com .aab", "Comparar con .aab", "Comparer avec .aab",
            "Mit .aab vergleichen", "Confronta con .aab", "Porównaj z .aab",
            "Vergelijken met .aab", "Porovnat s .aab", "Compară cu .aab"),

        new("Open .aab",
            "Abrir .aab", "Abrir .aab", "Ouvrir .aab",
            ".aab öffnen", "Apri .aab", "Otwórz .aab",
            ".aab openen", "Otevřít .aab", "Deschide .aab"),

        new("BUNDLE",
            "BUNDLE", "BUNDLE", "BUNDLE", "BUNDLE", "BUNDLE",
            "BUNDLE", "BUNDLE", "BUNDLE", "BUNDLE"),

        new("Drop an Android App Bundle here",
            "Arraste um Android App Bundle para aqui",
            "Arrastra aquí un Android App Bundle",
            "Déposez ici un Android App Bundle",
            "Android App Bundle hier ablegen",
            "Trascina qui un Android App Bundle",
            "Upuść tutaj Android App Bundle",
            "Sleep hier een Android App Bundle naartoe",
            "Sem přetáhněte Android App Bundle",
            "Trage aici un Android App Bundle"),

        new("No bundle loaded",
            "Nenhum bundle carregado", "Ningún bundle cargado", "Aucun bundle chargé",
            "Kein Bundle geladen", "Nessun bundle caricato", "Nie wczytano bundle",
            "Geen bundle geladen", "Není načten žádný bundle", "Niciun bundle încărcat"),

        new("DEX FILES",
            "FICHEIROS DEX", "ARCHIVOS DEX", "FICHIERS DEX",
            "DEX-DATEIEN", "FILE DEX", "PLIKI DEX",
            "DEX-BESTANDEN", "SOUBORY DEX", "FIȘIERE DEX"),

        new("GOOGLE DEX GATE",
            "LIMITE DEX GOOGLE", "LÍMITE DEX DE GOOGLE", "SEUIL DEX GOOGLE",
            "GOOGLE-DEX-GRENZE", "SOGLIA DEX GOOGLE", "PRÓG DEX GOOGLE",
            "GOOGLE DEX-GRENS", "LIMIT DEX GOOGLE", "PRAG DEX GOOGLE"),

        new("UNVERIFIED",
            "NÃO VERIFICADO", "NO VERIFICADO", "NON VÉRIFIÉ",
            "NICHT VERIFIZIERT", "NON VERIFICATO", "NIEZWERYFIKOWANE",
            "NIET GEVERIFIEERD", "NEOVĚŘENO", "NEVERIFICAT"),

        new("Load an .aab",
            "Carregue um .aab", "Carga un .aab", "Chargez un .aab",
            ".aab laden", "Carica un .aab", "Wczytaj .aab",
            "Laad een .aab", "Načtěte .aab", "Încarcă un .aab"),

        new("Deep DEX metrics",
            "Métricas DEX detalhadas", "Métricas DEX detalladas", "Métriques DEX détaillées",
            "Detaillierte DEX-Metriken", "Metriche DEX dettagliate", "Szczegółowe metryki DEX",
            "Gedetailleerde DEX-metrieken", "Podrobné metriky DEX", "Metrici DEX detaliate"),

        new("read-only",
            "só de leitura", "solo lectura", "lecture seule",
            "schreibgeschützt", "sola lettura", "tylko odczyt",
            "alleen-lezen", "jen pro čtení", "doar citire"),

        new("DEX inventory",
            "Inventário DEX", "Inventario DEX", "Inventaire DEX",
            "DEX-Inventar", "Inventario DEX", "Inwentarz DEX",
            "DEX-inventaris", "Inventář DEX", "Inventar DEX"),

        new("No DEX loaded",
            "Nenhum DEX carregado", "Ningún DEX cargado", "Aucun DEX chargé",
            "Kein DEX geladen", "Nessun DEX caricato", "Nie wczytano DEX",
            "Geen DEX geladen", "Není načten žádný DEX", "Niciun DEX încărcat"),

        new("Google DEX policy preflight",
            "Pré-verificação da política DEX Google",
            "Preflight de la política DEX de Google",
            "Précontrôle de la politique DEX Google",
            "Preflight der Google-DEX-Richtlinie",
            "Preflight della policy DEX Google",
            "Preflight zasad DEX Google",
            "Preflight van Google DEX-beleid",
            "Preflight zásad DEX Google",
            "Preflight pentru politica DEX Google"),

        new("Load an .aab to run the local preflight.",
            "Carregue um .aab para executar a pré-verificação local.",
            "Carga un .aab para ejecutar el preflight local.",
            "Chargez un .aab pour exécuter le précontrôle local.",
            "Laden Sie eine .aab-Datei für den lokalen Preflight.",
            "Carica un .aab per eseguire il preflight locale.",
            "Wczytaj .aab, aby uruchomić lokalny preflight.",
            "Laad een .aab om de lokale preflight uit te voeren.",
            "Načtěte .aab pro místní preflight.",
            "Încarcă un .aab pentru a rula preflight-ul local."),

        new("Threshold margin not measured.",
            "Margem para o limite não medida.",
            "Margen respecto al límite no medido.",
            "Marge par rapport au seuil non mesurée.",
            "Abstand zum Grenzwert nicht gemessen.",
            "Margine rispetto alla soglia non misurato.",
            "Margines do progu nie został zmierzony.",
            "Marge tot de grens niet gemeten.",
            "Rezerva do limitu nebyla změřena.",
            "Marja față de prag nu a fost măsurată."),

        new("Configured boundaries",
            "Limites configurados", "Límites configurados", "Seuils configurés",
            "Konfigurierte Grenzen", "Soglie configurate", "Skonfigurowane progi",
            "Geconfigureerde grenzen", "Nastavené limity", "Praguri configurate"),

        new("PLAY CONSOLE-ONLY METRICS",
            "MÉTRICAS EXCLUSIVAS DA PLAY CONSOLE",
            "MÉTRICAS EXCLUSIVAS DE PLAY CONSOLE",
            "MÉTRIQUES PLAY CONSOLE UNIQUEMENT",
            "NUR-PLAY-CONSOLE-METRIKEN",
            "METRICHE SOLO PLAY CONSOLE",
            "METRYKI TYLKO Z PLAY CONSOLE",
            "ALLEEN PLAY CONSOLE-METRIEKEN",
            "METRIKY POUZE Z PLAY CONSOLE",
            "METRICI DOAR DIN PLAY CONSOLE"),

        new("UNPROVEN LOCALLY",
            "NÃO COMPROVADO LOCALMENTE", "NO DEMOSTRADO LOCALMENTE", "NON PROUVÉ LOCALEMENT",
            "LOKAL NICHT NACHGEWIESEN", "NON DIMOSTRATO LOCALMENTE", "NIEPOTWIERDZONE LOKALNIE",
            "LOKAAL NIET BEWEZEN", "LOKÁLNĚ NEPROKÁZÁNO", "NEDEMONSTRAT LOCAL"),

        new("Binary contributors",
            "Contribuidores binários", "Contribuidores binarios", "Contributeurs binaires",
            "Binäre Beiträge", "Contributori binari", "Składniki binarne",
            "Binaire bijdragen", "Binární přispěvatelé", "Contribuitori binari"),

        new("packaged AAB footprint",
            "footprint do AAB empacotado", "huella del AAB empaquetado", "empreinte de l’AAB empaqueté",
            "Footprint des gepackten AAB", "impronta dell’AAB pacchettizzato", "ślad spakowanego AAB",
            "footprint van verpakte AAB", "stopa zabaleného AAB", "amprenta AAB împachetat"),

        new("Load an .aab to inspect packaged native/AOT contributors.",
            "Carregue um .aab para inspecionar os contribuidores native/AOT empacotados.",
            "Carga un .aab para inspeccionar los contribuidores native/AOT empaquetados.",
            "Chargez un .aab pour inspecter les contributeurs native/AOT empaquetés.",
            "Laden Sie eine .aab-Datei, um gepackte Native/AOT-Beiträge zu prüfen.",
            "Carica un .aab per ispezionare i contributori native/AOT pacchettizzati.",
            "Wczytaj .aab, aby sprawdzić spakowane składniki native/AOT.",
            "Laad een .aab om verpakte native/AOT-bijdragen te inspecteren.",
            "Načtěte .aab pro kontrolu zabalených native/AOT přispěvatelů.",
            "Încarcă un .aab pentru a inspecta contribuitorii native/AOT împachetați."),

        new("Largest logical contributors across packaged ABIs",
            "Maiores contribuidores lógicos entre ABIs empacotadas",
            "Mayores contribuidores lógicos entre ABI empaquetadas",
            "Principaux contributeurs logiques parmi les ABI empaquetées",
            "Größte logische Beiträge über gepackte ABIs",
            "Principali contributori logici nelle ABI pacchettizzate",
            "Największe logiczne składniki w spakowanych ABI",
            "Grootste logische bijdragen over verpakte ABI’s",
            "Největší logické příspěvky napříč zabalenými ABI",
            "Cei mai mari contribuitori logici din ABI-urile împachetate"),

        new("Category totals will appear here.",
            "Os totais por categoria aparecerão aqui.",
            "Los totales por categoría aparecerán aquí.",
            "Les totaux par catégorie apparaîtront ici.",
            "Kategoriesummen werden hier angezeigt.",
            "I totali per categoria appariranno qui.",
            "Sumy kategorii pojawią się tutaj.",
            "Categorietotalen verschijnen hier.",
            "Součty kategorií se zobrazí zde.",
            "Totalurile pe categorii vor apărea aici."),

        new("DEPENDENCY GRAPH",
            "GRAFO DE DEPENDÊNCIAS", "GRAFO DE DEPENDENCIAS", "GRAPHE DE DÉPENDANCES",
            "ABHÄNGIGKEITSGRAPH", "GRAFO DELLE DIPENDENZE", "GRAF ZALEŻNOŚCI",
            "AFHANKELIJKHEIDSGRAAF", "GRAF ZÁVISLOSTÍ", "GRAF DE DEPENDENȚE"),

        new("UNPROVEN FROM AAB ALONE",
            "NÃO COMPROVADO APENAS PELO AAB",
            "NO DEMOSTRADO SOLO CON EL AAB",
            "NON PROUVÉ PAR L’AAB SEUL",
            "AUS DEM AAB ALLEIN NICHT NACHGEWIESEN",
            "NON DIMOSTRATO DAL SOLO AAB",
            "NIEPOTWIERDZONE NA PODSTAWIE SAMEGO AAB",
            "NIET BEWEZEN OP BASIS VAN ALLEEN DE AAB",
            "NEPROKÁZÁNO POUZE Z AAB",
            "NEDEMONSTRAT DOAR DIN AAB"),

        new("Evidence",
            "Evidência", "Evidencia", "Preuves", "Nachweise",
            "Evidenze", "Dowody", "Bewijs", "Důkazy", "Dovezi"),

        new("No bundle loaded.",
            "Nenhum bundle carregado.", "Ningún bundle cargado.", "Aucun bundle chargé.",
            "Kein Bundle geladen.", "Nessun bundle caricato.", "Nie wczytano bundle.",
            "Geen bundle geladen.", "Není načten žádný bundle.", "Niciun bundle încărcat."),

        new("Export JSON",
            "Exportar JSON", "Exportar JSON", "Exporter JSON", "JSON exportieren",
            "Esporta JSON", "Eksportuj JSON", "JSON exporteren", "Exportovat JSON", "Exportă JSON"),

        new("Export TXT summary",
            "Exportar resumo TXT", "Exportar resumen TXT", "Exporter le résumé TXT",
            "TXT-Zusammenfassung exportieren", "Esporta riepilogo TXT", "Eksportuj podsumowanie TXT",
            "TXT-samenvatting exporteren", "Exportovat souhrn TXT", "Exportă rezumat TXT"),

        new("Module",
            "Módulo", "Módulo", "Module", "Modul", "Modulo",
            "Moduł", "Module", "Modul", "Modul"),

        new("Raw bytes",
            "Bytes raw", "Bytes raw", "Octets bruts", "Rohbytes", "Byte raw",
            "Bajty raw", "Ruwe bytes", "Raw bajty", "Octeți raw"),

        new("Compressed",
            "Comprimido", "Comprimido", "Compressé", "Komprimiert", "Compresso",
            "Skompresowane", "Gecomprimeerd", "Komprimováno", "Comprimat"),

        new("Classes",
            "Classes", "Clases", "Classes", "Klassen", "Classi",
            "Klasy", "Klassen", "Třídy", "Clase"),

        new("Defined",
            "Definidos", "Definidos", "Définies", "Definiert", "Definiti",
            "Zdefiniowane", "Gedefinieerd", "Definováno", "Definite"),

        new("With code",
            "Com código", "Con código", "Avec code", "Mit Code", "Con codice",
            "Z kodem", "Met code", "S kódem", "Cu cod"),

        new("Contributor",
            "Contribuidor", "Contribuidor", "Contributeur", "Beitrag",
            "Contributore", "Składnik", "Bijdrage", "Přispěvatel", "Contribuitor"),

        new("Category",
            "Categoria", "Categoría", "Catégorie", "Kategorie",
            "Categoria", "Kategoria", "Categorie", "Kategorie", "Categorie"),

        new("Raw",
            "Raw", "Raw", "Brut", "Roh", "Raw",
            "Raw", "Ruw", "Raw", "Raw")
    ];

    private static readonly IReadOnlyDictionary<string, Entry> Reverse = BuildReverse();

    public static void Apply(DependencyObject root, string? languageCode)
    {
        ArgumentNullException.ThrowIfNull(root);
        var language = Normalize(languageCode);
        ApplyNode(root, language);
    }

    private static void ApplyNode(DependencyObject node, string language)
    {
        if (node is TextBlock textBlock && !string.IsNullOrWhiteSpace(textBlock.Text))
            textBlock.Text = Translate(textBlock.Text, language);

        if (node is ContentControl contentControl && contentControl.Content is string content && !string.IsNullOrWhiteSpace(content))
            contentControl.Content = Translate(content, language);

        if (node is DataGrid dataGrid)
        {
            foreach (var column in dataGrid.Columns)
            {
                if (column.Header is string header && !string.IsNullOrWhiteSpace(header))
                    column.Header = Translate(header, language);
            }
        }

        foreach (var child in LogicalTreeHelper.GetChildren(node))
        {
            if (child is DependencyObject dependencyObject)
                ApplyNode(dependencyObject, language);
        }
    }

    private static string Translate(string current, string language)
    {
        if (!Reverse.TryGetValue(current, out var entry))
            return current;

        return entry.For(language);
    }

    private static IReadOnlyDictionary<string, Entry> BuildReverse()
    {
        var result = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in Entries)
        {
            foreach (var value in entry.All())
            {
                if (!string.IsNullOrWhiteSpace(value))
                    result.TryAdd(value, entry);
            }
        }

        return result;
    }

    private static string Normalize(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return "en";

        var value = languageCode.Trim().ToLowerInvariant();
        var dash = value.IndexOf('-');
        return dash > 0 ? value[..dash] : value;
    }
}
