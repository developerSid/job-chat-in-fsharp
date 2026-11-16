module ApiTests

open System.Net
open System.Net.Http.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open Myers.Estate.JobChat.Robo.API

let private client () =
  (new WebApplicationFactory<ChatRequest>()).CreateClient()

[<Fact>]
let ``health returns ok`` () =
  task {
    let! health = client().GetFromJsonAsync<HealthResponse>("/api/health")
    Assert.Equal({ Status = "ok" }, health)
  }

[<Fact>]
let ``chat returns the stub reply`` () =
  task {
    let! response = client().PostAsJsonAsync("/api/chat", { Message = "hi" })
    let! body = response.Content.ReadFromJsonAsync<ChatResponse>()
    Assert.Equal({ Reply = "You said: hi" }, body)
  }

[<Fact>]
let ``chat rejects a blank message`` () =
  task {
    let! response = client().PostAsJsonAsync("/api/chat", { Message = " " })
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode)
  }

[<Fact>]
let ``unknown routes fall back to the Blazor index page`` () =
  task {
    let! response = client().GetAsync("/chat")
    let! html = response.Content.ReadAsStringAsync()
    Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    Assert.Contains("<div id=\"app\"", html)
  }
