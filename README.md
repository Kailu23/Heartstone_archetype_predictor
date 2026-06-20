# Hearthstone Archetype Predictor

Desktop aplikacija za analizu Hearthstone deck stringova — dekodira deck, prikazuje slike karata i predviđa arhetip decka.

## Tehnologije

- **C# / .NET 10** — jezik i platforma
- **Avalonia UI** — cross-platform desktop UI framework
- **HearthDb** — biblioteka za dekodiranje Hearthstone deck stringova
- **MVVM** — arhitekturalni obrazac (Model-View-ViewModel)

---

## Pokretanje aplikacije

```bash
dotnet run
```

---

## Automatsko testiranje

### Korišteni alati

| Alat | Svrha |
|---|---|
| **xUnit** | Test framework |
| **coverlet** | Code coverage |
| **dorny/test-reporter** | GitHub Actions reporting |
| **CodeCoverageSummary** | Coverage badge u CI |

### Struktura testova

```
Tests/
├── Helpers/
│   └── WaitHelper.cs              # Ekvivalent Selenium Wait naredbi
├── PageObjects/
│   ├── MainWindowPage.cs          # POM za glavni prozor
│   └── SerializerPage.cs          # POM za HearthstoneSerializer
├── HearthstoneSerializerTests.cs  # Unit testovi (15 testova)
├── MainWindowViewModelTests.cs    # Unit testovi (13 testova)
└── IntegrationTests.cs            # Integracijski testovi (3 testa)
```

### Pokretanje testova

```bash
# Sve testove
dotnet test Tests/

# S prikazom coverage-a
dotnet test Tests/ /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generiraj HTML coverage report (zahtijeva ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:TestResults/coverage.xml -targetdir:TestResults/html -reporttypes:Html
```

### Napredne tehnike

#### Page Object Model (POM)

Svaki "ekran" aplikacije ima svoju Page klasu u `Tests/PageObjects/`. Testovi ne komuniciraju direktno s ViewModelom već kroz Page Object — što ih čini čitljivijima i lakšima za održavanje:

```csharp
// Bez POM-a (loše):
var vm = new MainWindowViewModel();
bool result = await vm.NewDeckString("...");

// S POM-om (dobro):
var page = new MainWindowPage();
bool result = await page.EnterDeckString("...");
```

#### Wait naredbe

`WaitHelper` preslikava Selenium Wait koncepte na async C# testove:

```csharp
// ImplicitWait — čeka fiksno vrijeme
await WaitHelper.ImplicitWait(500);

// ExplicitWait — čeka dok uvjet nije ispunjen
await WaitHelper.WaitUntil(() => page.ImageCount > 0, TimeSpan.FromSeconds(10));

// FluentWait — polling s ignoriranjem iznimki
var value = await WaitHelper.FluentWait(
    supplier: () => GetSomeValue(),
    until: v => v == "expected",
    timeout: TimeSpan.FromSeconds(5));
```

#### CI integracija

GitHub Actions automatski:
1. Builda projekt
2. Pokreće sve testove
3. Generira TRX izvještaj
4. Prikazuje rezultate direktno na Pull Requestu
5. Objavljuje coverage report kao artifact

Vidi `.github/workflows/ci.yml`.
