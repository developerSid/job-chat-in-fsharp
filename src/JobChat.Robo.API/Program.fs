namespace Myers.Estate.JobChat.Robo.API

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open System
open Microsoft.Extensions.Hosting

module Program =
  let describe =
    function
    | MessageNullOrEmpty -> "Message must not be empty."
    | MessageTooLong(length, maxLength) -> $"Message is %d{length} characters; the maximum is %d{maxLength}."

  let configure (app: WebApplication) =
    app.UseBlazorFrameworkFiles() |> ignore
    app.UseStaticFiles() |> ignore

    app.MapGet("/api/health", Func<HealthResponse>(fun () -> { Status = "ok" }))
    |> ignore

    app.MapPost(
      "/api/chat",
      Func<ChatRequest, IResult>(fun request ->
        match ChatHandler.handle request with
        | Ok response -> Results.Ok response
        | Error error -> Results.BadRequest(describe error)
      )
    )
    |> ignore

    app.MapFallbackToFile "index.html" |> ignore
    app

  [<EntryPoint>]
  let main args =
    let app = WebApplication.CreateBuilder(args).Build() |> configure
    app.Run()
    0
