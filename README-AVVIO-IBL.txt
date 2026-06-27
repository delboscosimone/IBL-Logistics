IBL - International BlueHarbor Logistics
Versione ricostruita con UI Blazor scura + backend ASP.NET Core + MySQL XAMPP

REQUISITI
1) .NET SDK 8 installato
2) XAMPP avviato con MySQL attivo
3) Database MySQL raggiungibile su localhost:3306 con utente root senza password

AVVIO CONSIGLIATO
1) Apri un terminale nella cartella backend:
   cd ReeferSentinel.Monolith
   dotnet restore
   dotnet run --launch-profile http

   Backend API:
   http://localhost:51067/swagger

2) Apri un secondo terminale nella cartella frontend:
   cd IBL.Blazor
   dotnet restore
   dotnet run --launch-profile http

   Frontend Blazor:
   http://localhost:5268

NOTE IMPORTANTI DATABASE
- In ambiente Development il backend prova a creare automaticamente il database e le tabelle con EnsureCreated().
- Se trova un vecchio database non allineato, la voce Database:AutoRepairOnStartup=true permette di eliminare e ricreare il database locale ibl_logistics.
- Questa impostazione è comoda per demo scolastiche, ma se vuoi conservare dati reali mettila a false in ReeferSentinel.Monolith/appsettings.json.

ACCOUNT DEMO FRONTEND
Admin:
  email: admin@ibl.local
  password: Admin123!

Dipendente:
  email: employee@ibl.local
  password: Employee123!

Cliente:
  email: customer@ibl.local
  password: Customer123!

COSA È STATO RICOSTRUITO
- UI/UX scura in stile screenshots: sidebar, topbar, home page, login, admin, area dipendente.
- Login persistente con ProtectedLocalStorage.
- Dashboard button vicino allo stato online.
- Registrazione cliente con Nome, Azienda, P.IVA/CF, Email, Password.
- Gestione porti: 10 porti internazionali seedati nel backend.
- Booking con data partenza e data arrivo stimata.
- Creazione booking con selezione porto partenza/arrivo.
- Filtro container disponibili per porto e data.
- Possibilità di associare più container a un booking.
- Categorie MSC con setpoint temperatura/umidità e icone.
- Visualizzazione area cliente con booking collegati localmente.

LIMITI NOTI
- L'autenticazione è dimostrativa lato Blazor, non è un sistema JWT/cookie production-ready.
- Il modello resta compatibile con il progetto originario ReeferSentinel, ma è stato adattato al dominio IBL.
- Per una produzione reale servirebbe completare la rifattorizzazione con BookingContainers e ContainerMovements come tabelle autonome.
