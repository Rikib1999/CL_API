## First impression

Po naklonování a prvním otevření README.md to vypadá, že je vše dobře popsané, bouhže po přečtení jsem nebyl o moc moudřejší jak vlastně knihovnu správně používat.

README.md začíná krásným a stručným návodem jak nainstalovat a přidat knihovnu k nějakému projektu. Vše je zde stručně a jasně.
Dále máme použití knihovny. To popisuje jak si vytvořit jaký si vlastní objekt, do kterého budeme přidávat nějaké Options nebo Arguments. To jak přidávat další Options a Arguments je dále ukázáno v příkladech, ale jak se budou parsovat data z příkazové řádky, to nám nikdo neprozradí. Nikde není k zjištění jak s námi vytvořeným objektem dále pracovat a jak se tam dostanou data z příkazové řádky.

Jsou zde zmíněné nějaké klíčové koncepty jako například využití reflexe při definování Options nebo Arguments. Mrzí mě, že zde není moc vysvětleno v jakém momentě se mi vyplatí Arguments používat. Dále je zde konečně zmíněná nějaká generická třída CommandParser a jenjí metoda parse. Ale bohužel nikde žádný příklad jak ji použít, co předat jako generický typ, ...

Nakonec zde není zmíněné žádné možné rozšíření knihovny. Jsou zde zmíněné nějaké interface, které musí třídy splňovat, ale to je vše. Později si ale může člověk všimnout, že knihovna je opravdu mocná a ve většině případech to ani není třeba.

Nakonec zde bohužel není zmíněno jak vygenerovat jakoukoliv dokumentaci, ani zde není řečeno kde nějakou uživatelem vytvořenou dokumentaci najít. Po prozkoumání repozitáře jsem ani žádnou nenalezl :(.

## High-level review of the library API

Způsob jakým se definují Otions, Arguments, atd. je jak intuitivní pro uživatele, tak velice dobrý z hlediska čitelnosti a udržitelnosti kódu. Bohužel zde není podpora řídit program pomocí Eventů, ale po dokončení parseru se musíme ptát, zda byla nějaká Option nalezena. To je veliká škoda.

Knihovna podporuje spoustu typů, které umí sama naparsovat, ale bohužel, žádná podpora přidání vlastního typu bez zásahu do samotné knihovny.

Knihovna umí generovat vlastní HELP TEXT na bázi předem dané dokumentace od uživatele.

Pro přístup k datům si musí uživatel vytvořit vlastní objekt a do něj mu jsou pak data uložena. Tento způsob je Imutable a tak může uživatel jeden objekt použít ve více parserech, což je velké plus. Rovněž, jak jsem zmínil je možné využívat více separátních parserů, které se neovlivňují.

Po bližším prozkoumání funkčnosti jsem narazil na chybu co se týče konzistentnosti při hledání Option, když se parsuje řádek. Pokud máme Option s jmény ```--fo``` a ```--form```, tak záleží na pořadí v jakém jsme jména zadali a při Parsování to může vyvolat chybu. Přepokládá se, že bude následovat znak oddělující argument a jmééno optiony a ten je vynechán i přesto, že je to nějaký další platný znak jiné Option.

Celkově je však kód velice čitelný a jednoduchý napsat + tak intuitivní, že VS inteli sence pro C# zvládá dost dobře předvídat co chce uživatel psát, což práci usnadňuje.

Kód je taky dobře strukturovaný a tak pokud uživatel potřebuje nahlédnout do nějaké funkce, třídy, apo. tak je to velice jednoduché a intuitivní.

## Extending the library

Ve většině případů ani uživatel rozšiřovat nepotřebuje a to byl i případ DateTime parseru. Pokud by opravdu ale někdo chtěl nějaké rozšíření, tak si musí oddědit celý oběkt Parseru a přidat si do seznamu svůj oběkt, který potřebuje parsovat + se musí ujistit, že ho lze naparsovat způsobem, který používá knihovna. Tento způsob není nikde popsaný ani řečený nějakým Interfacem, takže uživatel musí opravdu navštívit kód a porozumět mu.

Co se týče DateTime parseru, tak si myslím, že velkou práci zde dělá c# a jejich objekt ```DateTime``` + metoda ```ToString()```. Avšak i díky parseru je to opravdu jednoduché na implementaci.

## Documentation

Bohužel kromě README.md, žádná dokumentace neexistuje :(. Pro útěchu alespoň kód je velice dobře okomentovaný a tak při používání nějakého pokročilejšího IDE je uživatel schopen zjistit základní chování metod.

## Implementation

Co se týče čitelnosti kódu uvntř knihovny, tak se dost často stává, že jsou zde vnořené 2-3 cykly s nějakými vnořenými podmínkami, což dělá veliké odsazení. Pokud toto spojíme s delšími názvy, které umí c# omezit různými způsoby jako ```var```, ```new()```, ```using```, atd. To potom tvoří řádky tak dlouhé, že se občas nevejdou na monitor, což zhoršuje čitelnost.

Občas se oběví i nějaké komentáře v rodném jazyce, což úplně nepřidává na kvalitě čitelnisti.

Jinak se ale tvůrci snažili dekomponovat program do různých privátních tříd a metod. Opravdu není k nalezení moc duplicitního kódu. Také jsou vhodně použité nějaké statické atributy.

Co se týče testů tak jsou dobře napsané, čitelné, ale bohužel nedokázali odhalit ani tak očividnou chybu, kterou jsem našel při implementaci DateTimeParseru.


## Detailed comments

Pro shrnutí. Velmi se mi líbí jednoduchost pro používání. Bohužel kvůli README.md docela trvá než se člověk sžije s knohovnou, ale poté už je to velice užitečný nástroj na jednoduché parsování. Pokud někdo potřebuje parsovat i nějaké subcommandy, tak to taky lze udělat, ale už je to mnohem složitější.

Co se týče čitelnosti tak pro uživatele, který se nepotřebuje dívat do knihovny je to super. Umožňuje to napsat opravdu hezký kód. Pokud potřebujete zjistit něco více, tak se dostáváme k chybám, které jsou popsány výše.

K testům: !!! V gitlabu chybí soubor .gitlab-ci.yml

Největším problémem je chybějící dokumentace, což velice, ale velice zpomalilo pocopení knihovny.
