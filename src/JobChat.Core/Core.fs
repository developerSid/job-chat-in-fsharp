namespace Myers.Estate.JobChat.Core.Core

open System
open System.Collections.Generic

type Unknown = exn // alias exn (exception) to unknown as a stand-in for places we can't model at the moment
type Length = int
type Number = int64
type PositiveNumber = uint32
type Date = DateOnly
type PositionMargin = double

[<AutoOpen>]
// Core helpers for null handling, optional values, and constrained construction.
module Core =
  // https://stackoverflow.com/a/11696947/104021
  /// Returns true when the supplied reference is null.
  let inline isNull (x: ^T when ^T: not struct) = obj.ReferenceEquals(x, null)

  /// Converts None, null, empty, and whitespace-only strings to None; preserves other strings.
  let inline maybe (field: string option) =
    match field with
    | None -> None
    | Some f ->
      match String.IsNullOrEmpty(f) || String.IsNullOrWhiteSpace(f) with
      | true -> None
      | false -> Some f

  /// Converts None and Some null to None; preserves non-null reference values.
  let inline maybeObject<'O> (ob: 'O option) =
    match ob with
    | None -> None
    | Some o ->
      match obj.ReferenceEquals(o, null) with
      | true -> None
      | false -> Some o

  let inline swap swapTo _ = swapTo

[<RequireQualifiedAccess>]
module List =
  let removeFirstElement list =
    match list with
    | [] -> None, []
    | first :: rest -> (Some first), rest

[<RequireQualifiedAccess>]
module RandomNumberGenerator =
  let generateRandomSalt saltSize =
    System.Security.Cryptography.RandomNumberGenerator.GetBytes(saltSize)

[<RequireQualifiedAccess>]
module Dictionary =
  let isEmpty (d: IDictionary<'K, 'V>) =
    match d with
    | null -> true
    | _ -> d.Count = 0

[<RequireQualifiedAccess>]
module Seq =
  [<CompiledName("Peek")>]
  let peek peeker source =
    source
    |> Seq.map (fun item ->
      peeker item
      item
    )
