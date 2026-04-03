IBL.Blazor - versione con ruoli e dashboard

Cosa include:
- Home page con spiegazione del progetto
- Login / registrazione cliente
- Redirect automatico alla dashboard in base al ruolo
- Dashboard Admin
  - creazione nuovi dipendenti / clienti / admin
  - creazione container tramite API esistenti
- Dashboard Dipendente
  - selezione o creazione cliente
  - creazione booking tramite API esistenti
  - associazione container al booking
  - lookup rapido container
- Dashboard Cliente
  - visualizzazione booking associati al proprio account
  - visualizzazione dei container referenziati

Account demo:
- admin@ibl.local / Admin123!
- employee@ibl.local / Employee123!
- customer@ibl.local / Customer123!

Come avviare:
1. Avviare il backend ReeferSentinel.Monolith
   dotnet restore
   dotnet build
   dotnet run

2. Controllare la porta delle API e Swagger.
   Aggiornare IBL.Blazor/appsettings.json se necessario.

3. Avviare il frontend Blazor
   cd IBL.Blazor
   dotnet restore
   dotnet build
   dotnet run

Nota tecnica:
- Il sistema di login/ruoli in questa versione è implementato nel frontend con un servizio scoped e account demo/in-memory.
- Le azioni operative principali (creazione booking, associazione container, creazione container, lettura booking/container) chiamano le API già presenti del monolite.
- La dashboard cliente mostra i booking associati al cliente tramite un registro applicativo lato frontend, utile per la demo del flusso a ruoli.
