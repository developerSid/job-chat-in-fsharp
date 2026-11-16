namespace JobChat.Robo.Web;

public record ChatRequest(string Message);

public record ChatResponse(string Reply);

public record HealthResponse(string Status);
