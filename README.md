# ClevrJobs

AI-driven jobbannons sida (enbart jobbannonser med `C#` som nyckelord för nu). Hämtar data från Platsbanken, bearbetar den med hjälp av Google Gemini för att få fram en sammanfattning, nyckelord och annat bra att veta. All data visas sedan i en simpel webbapp.

## Komponenter

**Scraping** - Worker Service som med hjälp av Playwright navigerar till rätt sida, öppnar annonser och sparar ner rätt data.

**AI Bearbetning** - Använder Google Gemini för att analysera och extrahera:

- _Måste-ha_ och _meriterande_ kunskaper.
- Uppskattning av svårighetsgrad (NewGrad, Junior, Mid, Senior, Lead)
- Nyckelord för CV/Personligt brev.
- Vad personliga brevet bör fokusera på.

**Presentation** - All data visas upp i en simpel webbapp.

## Tech Stack

### Backend (.NET 10)

| Component     | Technology                             |
| ------------- | -------------------------------------- |
| Framework     | ASP.NET Core 10.0                      |
| ORM           | Entity Framework Core 10.0             |
| Database      | SQL Server                             |
| Web Scraping  | Microsoft Playwright                   |
| AI Processing | Google Gemini (gemini-3-flash-preview) |
| Messaging     | System.Threading.Channels              |

### Frontend

| Component        | Technology     |
| ---------------- | -------------- |
| Framework        | Next.js 16     |
| Language         | TypeScript     |
| State Management | TanStack Query |
| Styling          | TailwindCSS v4 |
