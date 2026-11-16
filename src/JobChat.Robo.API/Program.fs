namespace Myers.Estate.JobChat.Robo.API

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open System
open Microsoft.Extensions.Hosting
open Serilog

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
    let builder = WebApplication.CreateBuilder args

    builder.Host.UseSerilog(fun context services configuration ->
      configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services).Enrich.FromLogContext()
      |> ignore
    )
    |> ignore

    let app = builder.Build() |> configure
    app.UseSerilogRequestLogging() |> ignore
    app.Run()
    0
