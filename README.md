# ⚡ MinimalApiResilience

MinimalApiResilience is a lightweight .NET 8 API showcasing resilience patterns using Polly, structured logging with Serilog, and observability through SEQ.

Inspired by real-world events like the power outage in Portugal and Spain on April 28, 2025, this project demonstrates how to prevent "digital blackouts" in distributed systems.


## 🧰 Technologies Used

- .NET 8 - Minimal API
- Polly - Retry, Timeout, Circuit Breaker
- Serilog - Structured Logging
- SEQ - Log Visualization
- Swagger - API Documentation

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Docker (for running SEQ)

### Steps

Clone the repository:

```bash
git clone https://github.com/yourusername/MinimalApiResilience.git
cd MinimalApiResilience
```

Restore dependencies:

```bash
dotnet restore
```

Run SEQ using Docker:

```bash
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
```

Start the application:

```bash
dotnet run
```

Access Swagger UI:

```
http://localhost:7284/swagger
```

---

## 🛠️ Project Structure

- `Program.cs` - Configures the application and endpoints.
- `Middlewares/ExceptionHandlingMiddleware.cs` - Handles exceptions globally.
- `ServiceCollectionExtensions/` - Sets up services like HttpClient and Serilog.
- `Settings/ExternalApiSettings.cs` - Holds external API settings.
- `appsettings.json` - Application configuration.

---

## 🔧 Configuration

Adjust resilience settings in `appsettings.json`:

```json
"ExternalApiSettings": {
  "BaseUrl": "https://api.example.com/",
  "RetryCount": 3,
  "TimeoutSeconds": 5,
  "CircuitBreakerFailureThreshold": 2,
  "CircuitBreakerDurationSeconds": 30
}
```

---

## 📈 Observability

- Access SEQ dashboard: [http://localhost:5341](http://localhost:5341)

Log examples:

- Retries: `Retry attempt {retryAttempt} failed. Waiting {timespan} before next attempt.`
- Circuit Breaker: `Circuit opened for {timespan.TotalSeconds} seconds due to: {reason}`
- Circuit Breaker Reset: `Circuit closed. Operations resumed.`
- Half-Open State: `Circuit half-open. Testing the system.`

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.