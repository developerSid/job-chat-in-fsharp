module IdentityTests

open System
open Xunit
open Myers.Estate.JobChat.Core

type IdError = InvalidId of text: string

type UserId = UserId of SortableIdentifier

let private fromString = SortableIdentifier.fromString InvalidId

let private v7 =
  Guid.CreateVersion7(DateTimeOffset(2026, 11, 14, 12, 0, 0, TimeSpan.Zero))

[<Fact>]
let ``newId produces a version 7 guid`` () =
  let guid = SortableIdentifier.newId () |> SortableIdentifier.toString |> Guid.Parse

  Assert.Equal(7, guid.Version)

[<Fact>]
let ``newId produces a different id each call`` () =
  Assert.NotEqual(SortableIdentifier.newId (), SortableIdentifier.newId ())

[<Fact>]
let ``newIdWrapped applies the wrapper to a new id`` () =
  let (UserId id) = SortableIdentifier.newIdWrapped UserId

  Assert.Equal(7, (SortableIdentifier.toString id |> Guid.Parse).Version)

[<Fact>]
let ``toString returns the lowercase hyphenated guid`` () =
  let text = fromString (string v7) |> Result.map SortableIdentifier.toString

  Assert.Equal(Ok(v7.ToString("D")), text)

[<Fact>]
let ``fromString round-trips through toString`` () =
  let id = SortableIdentifier.newId ()
  let text = SortableIdentifier.toString id

  Assert.Equal(Ok id, fromString text)

[<Fact>]
let ``fromString accepts other guid formats and normalizes them`` () =
  let expected = Ok(v7.ToString("D"))

  Assert.Equal(expected, fromString (v7.ToString("N")) |> Result.map SortableIdentifier.toString)

  Assert.Equal(
    expected,
    fromString (v7.ToString("D").ToUpperInvariant())
    |> Result.map SortableIdentifier.toString
  )

  Assert.Equal(expected, fromString (v7.ToString("B")) |> Result.map SortableIdentifier.toString)

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("  ")>]
[<InlineData("not-a-guid")>]
[<InlineData("00000000-0000-0000-0000-000000000000")>]
[<InlineData("0f8fad5b-d9cb-469f-a165-70867728950e")>] // version 4
let ``fromString returns the error for invalid or non-v7 ids`` (input: string) =
  Assert.Equal(Error(InvalidId input), fromString input |> Result.map SortableIdentifier.toString)

[<Fact>]
let ``ids created later sort after ids created earlier`` () =
  let at (time: DateTimeOffset) =
    Guid.CreateVersion7 time
    |> string
    |> fromString
    |> Result.map SortableIdentifier.toString
    |> Result.toOption
    |> Option.get

  let start = DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
  let ids = [ 0..5 ] |> List.map (fun days -> at (start.AddDays days))

  Assert.Equal<string list>(ids, List.sortWith (fun a b -> String.CompareOrdinal(a, b)) (List.rev ids))
