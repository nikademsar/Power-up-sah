# Šah+ – računalniška šah aplikacija

## Opis aplikacije

**Šah+** je računalniška šahovska aplikacija, razvita v okolju **Unity** z uporabo programskega jezika **C#**. Aplikacija temelji na klasičnih pravilih šaha, hkrati pa omogoča sodobnejšo uporabniško izkušnjo z integriranim uporabniškim vmesnikom, podporo za igranje proti računalniku in beleženjem rezultatov.

![StartPanel](https://github.com/nikademsar/Power-up-sah/blob/main/Images/StartPanal.gif?raw=true)
![PowerUps](https://github.com/nikademsar/Power-up-sah/blob/main/Images/PowerUps.gif?raw=true)
![EndPana](https://github.com/nikademsar/Power-up-sah/blob/main/Images/EndPanal.png?raw=true)

---

## Glavne funkcionalnosti

### Igranje šaha

* Prikaz standardne **šahovnice 8 × 8**
* Vse osnovne šahovske figure (kmet, trdnjava, konj, lovec, dama, kralj)
* Izmenično igranje belih in črnih figur
* Prikaz igralca, ki je trenutno na potezi
* Zaznava konca igre ob ujetju kralja

### Načini igre

* **Igralec proti igralcu (Two Player)**
  Igranje dveh igralcev na isti napravi
* **Igralec proti računalniku (Vs Bot)**
  Računalniški nasprotnik uporablja lasten šahovski pogon

### Računalniški nasprotnik (Bot)

* Lasten šahovski engine (`PowerUpChess.Engine`)
* Iskanje najboljše poteze z algoritmom preiskovanja drevesa potez
* Nastavljiva globina iskanja (1–5)
* Nastavitev strani bota (igra kot beli ali črni)

### Power-Upi (Posebne sposobnosti)

* Poleg klasičnih pravil šaha aplikacija Šah+ omogoča uporabo posebnih power-upov, ki igri dodajo več raznolikosti in strateške globine.

* Naključna transformacija
Izbrano figuro (ali kmeta) naključno spremeni v drugo šahovsko figuro.

* Zamenjava (Switch)
Omogoča zamenjavo položajev dveh poljubno izbranih figur na šahovnici.

* Dvojna poteza (Double Turn)
Igralcu omogoči, da izvede dve zaporedni potezi.

* Omejitev gibanja na kmeta
Nasprotnik mora vse svoje figure premikati po pravilih gibanja kmeta.

* Kmet kot dama
Izbranemu kmetu začasno omogoči gibanje po pravilih dame.

### Nastavitve igre

* Vklop/izklop igranja proti botu
* Nastavitev težavnosti (globina iskanja)
* Nastavitev barve figur, ki jih igra bot
* Izbira različnih barvnih tem igre
* Beleženje zmag belih in črnih
* Nastavitve se shranjujejo z uporabo `PlayerPrefs`


### Uporabniški vmesnik

* Začetni meni (Start panel)
* Končni zaslon z izpisom zmagovalca (End panel)
* Gumb za ponovni začetek igre
* Pregleden in enostaven UI, prilagojen namiznim windows napravam

---

## Tehnična arhitektura

* **Unity** – igralni pogon
* **C#** – programski jezik
* Ločitev logike igre, UI-ja in šahovskega pogona
* Šahovski bot deluje neodvisno od Unity predstavitve (ločena logika)

---

## ZIP aplikacije:
![Aplikacija](https://github.com/nikademsar/Power-up-sah/blob/main/Build.zip)

---

## Viri in tehnologije

* Unity Documentation: [https://docs.unity.com](https://docs.unity.com)
* C# Language Reference: [https://learn.microsoft.com/dotnet/csharp](https://learn.microsoft.com/dotnet/csharp)
* Minimax / game tree search (teoretična osnova šahovskega bota)
