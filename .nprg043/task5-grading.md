# Prvni dojem

`*` README obsahuje velmi minimalisticky uvod, instrukce pro prelozeni
knihovny a spusteni prikladu, priklady definice options a prehled
zakladnich konceptu.

`~` Popis projektu v README.md je az prilis strucny -- 1 veta. Ocekaval bych
odstavec, ktery poskytne high-level prehled o tom, co knihovna dela a proc
pouzit zrovna ji.

`~` Sekce **Key concepts** jakozto prehled zakladnich konceptu pro praci
s knihovnou by mela byt na zacatku, ne na konci.

`~` Chybi odkaz na referencni dokumentaci pripadne navod na jeji vygenerovani.
Rovnez chybi zminka o testech pripadne instrukce pro jejich spusteni.

`-` Chybi uceleny priklad, ktery by ukazal kompletni pouziti knihovny v
jednoduche situaci, vcetne pristupu k naparsovanym hodnotam pojmenovanych a
pozicnich parametru.

`*` Knihovna jde prelozit (s warningy), z testu prochazi pouze jedna sada,
neexistuje CI setup.


# Dokumentace

`!!` Referencni dokumentace (generovana ze zdrojovek) neexistuje.

`!` Dokumentace trid/atributu je minimalisticka. Vetsina ma maximalne brief
popis, ktery nic detailnejsiho nerika.

`!` Dokumentace metod/properties je minimalisticka.

  * Metody/properties maji bud jednoradkovy nebo zadny popis.
  * Popis parametru neresi pripustne hodnoty.
  * Informace o vyhazovanych vyjimkach je omezena na seznam.

`*` S ohledem na deklarativni styl API predstavuji atributy
domenove-specificky jazyk, kterym se popisuje struktura prikazove radky. Je
tedy dulezite vsechny prvky jazyka detailne zdokumentovat a vysvetlit, jak se
bude parser pri jejich pouziti chovat. Rovnez je potreba se zabyvat tim, za
jakych okolnosti muze byt konfigurace neplatna a co se bude dit.


# Rozhrani knihovny

`*` Deklarativni API. Parametry z prikazove radky se definuji jako anotovane
instancni promenne tridy reprezentujici konfiguraci programu. Anotace
(atributy) umoznuji identifikovat hodnoty predavane pomoci pojmenovanych nebo
pozicnich parametru, typ promennych urcuje typ hodnot, predavanych v retezcove
podobe na prikazove radce.

`-` Jazyk atributu ma nizkou granularitu.

  * Atributu je pomerne malo a kazdy zhlukuje velky pocet ruznych
    properties, ktere jsou v naproste vetsine nepovinne.
    
    Misto atributu `Option` s nepovinnymi properties by bylo vhodne
    nadefinovat vice jednoduchych atributu, jejichz pouziti je rovnez
    nepovinne, ale umoznuje rozdelit ruzne koncepty, podobne jako to dela
    parametr `[Boundaries]`.

  * V rade pripadu nuti uzivatele predavat jeden literal v poli. Misto toho by
    bylo vhodne, aby bylo mozne nektere atributy specifikovat vicekrat.

`-` API ma vetsi "povrch" nez by bylo potreba.

  * Metoda `Parse(string, ...)` neni pro bezne pouziti potreba (nehlede na
    to, ze jeji spravna implementace je netrivialni, protoze by mela delat
    radu veci, ktere normalne dela shell).

  * API poskytuje vetsi mnozstvi vyjimek.
    
    Stacily by dva hlavni typy vyjimek pro konfiguraci a chyby na vstupu.
    Dulezite je, aby se k uzivateli dostala vhodna chybova hlaska. Vetsina
    vyjimek by tedy mohla byt pro knihovnu interni. To by design vasi knihovny
    nijak zasadne nezmenilo, ale zmensilo by to povrch API.

  * Neni jasne, k cemu se pouziva trida `CommandExtensions`, resp. proc
    jeji metody nejsou ve tride `CommandParser` a proc jsou potreba metody
    jako `IsPresent()` nebo `GetHelpText()` pro specificke options.

`-` Rozhrani je sice kompletne bezestavove a parsovani poskytuje staticka
metoda, ale rozhrani hlavniho parseru se v podstate sklada ze dvou trid,
`CommandParser` a `CommandExtensions`.

Ty vsak poskytuji metody, ktere vyzaduji instanci `ICommandDefinition` jako
parametr, coz indikuje, ze by bylo vhodne definici prikazove radky spojit s
instanci `CommandParser`, treba pomoci staticke factory metody, ktera by
dostala instanci `ICommandDefinition`, provedla validaci definice a vratila
instanci `CommandParser`, ktera by mohla poskytovat instancni (nikoliv
staticke) metody `Parse()` a `GetHelp()` bez nutnosti pokazde predavat
instanci `ICommandDefinition`.

`-` Neni jasne, zda je API nejakym zpusobem navrzeno pro rozsirovani.

`~` Nedotazene nazvy.

  * `Delimeter` misto `Delimiter`, `Boundaries` misto `Bounds` nebo `Range`,
    `Dependencies` misto `DependsOn` nebo `Requires`, `Exclusivities` misto
    `Excludes` nebo `ConflictsWith`, `order` misto `position` (v pripade
    pozicnich argumentu).

  * `CommandExtensions`


# Zduvodneni navrhu

`!` Postradam nejaky design dokument, ktery by popsal hlavni koncepty a hlavni
use cases &mdash; veci, co se typicky rikaly pri prezentaci. Navic by se zde
hodilo rozebrat duvody pro radu designovych rozhodnuti v API.


# Kvalita implementace

`!!` Naprosta vetsina metod, ktere se podileji na parsovani, je prilis
slozitych (bezne 5-6 urovni vnoreni) a malo oddeluje vrstvy abstrakce
(implementaci logickych kroku od jejich pouziti na vyssi urovni).

  * Napr. `ParseArguments()`, `ParseOptions()`, `PopulateCommandInstance()`,
    `FindArgsDelimeter()`, `FindParameters()`, `FindArgumentParameters()`, ...

  * V mnoha metodach je inline iterace pres kolekce, ktera by mela byt
    prevedena na dotaz nad kolekci (extrahovany do jine metody) a pak pouziti
    vysledku v ramci "business logiky".
    
    Napr. `CheckDependencies()` nebo `CheckExclusivities()`.

`-` Pri sestaveni knihovny prekladac generuje mozstvi (21) varovani.

`~` V kodu se opakovane vyskytuji literaly ("=", "-", "--"), ktere maji
specificky vyznam a mely by byt idealne reprezentovany symbolem.


# Testy

`*` Knihovna integruje testy od kolegu, nema vlastni testy.

`-` Chybi konfigurace CI, ktera by spoustela testy pri kazdem commitu.

`-` Vetve s testy nejsou zamergovane, kod testu byl do repozitare nakopirovan
bez konfigurace CI.

`!!` Knihovna prochazi pouze jednou sadou testu.


# Zdrojove kody

`!` Samostatne prikazy (v podrizenych vetvich ridicich prikazu) nejsou vzdy
uzavreny do bloku, podrizeny prikaz je casto na stejne radce jako ridici
prikaz.

  * Formatovani kodu by melo zduraznovat strukturu (nadrizeny/podrizeny kod)
    a ve stejnych situacich by melo vypadat pokud mozno stejne.

  * Je vhodne myslet i na to, ze pridani (pripadne odebrani) jednoho prikazu
    do bloku si vynucuje upravu formatovani.

Kod slozeny z radku jsou ty nasledujici je velmi spatne citelny.
```
if (!typeof(T).GetInterfaces().Contains(typeof(ICommandDefinition))) throw new MissingInterfaceException("Class " + typeof(T).Name + " does not implement ICommandDefinition interface.");
```
```
if (option != null && !(generalType == GeneralType.Array || generalType == GeneralType.List) && option.MaxParameterCount > 1) throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Option supports multiple arguments but it is not a collection.");
```

`-` Ve zdrojovkach je rada velmi dlouhych radku (jak v kodu, tak v
komentarich).


# Souhrnne hodnoceni

Vitam snahu o poskytnuti deklarativniho API. Prilis hrube cleneni atributu,
ktere jsem zminoval vyse nebo navrh zbyvajici (imperativni) casti API by
nebyly zasadni problem.

Co vsak vnimam jako zasadni problem je celkove provedeni, ktere tuto praci
radi mezi ty nejslabsi. Warningy pri prekladu, chybejici CI a sada testu,
ktera vubec neprochazi, nezamergovane reviews a testy, neexistujici referencni
dokumentace a velmi sporadicka dokumentace API v kodu, ze ktereho by se
referencni dokumentace generovala. Implementace se jevi jako zbytecne slozita
a kvuli nesrozumitelnosti kodu je velmi tezke se v ni zorientovat.