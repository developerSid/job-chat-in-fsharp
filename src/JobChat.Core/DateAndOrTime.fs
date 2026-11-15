namespace Myers.Estate.JobChat.Core

open System
open System.Globalization

[<RequireQualifiedAccess>]
module Date =
  let construct error (dateIn: string) =
    match DateOnly.TryParse(dateIn, CultureInfo.InvariantCulture, DateTimeStyles.None) with
    | true, date -> Ok date
    | false, _ -> Error(error dateIn)

  let constructFromDateTime (dateIn: DateTime) = Ok(DateOnly.FromDateTime(dateIn))

  let maybeConstructFromDateTimeNullable (dateTimeIn: System.Nullable<DateTime>) =
    match Option.ofNullable dateTimeIn with
    | None -> None
    | Some d -> Some(DateOnly.FromDateTime(d))
