module MatcherTests

open Xunit
open Myers.Estate.JobChat.Core

type MatchError = DidNotMatch of text: string

let private order = Matcher.construct @"^order-(?<id>\d+)$"

[<Fact>]
let ``matches is true only when the pattern is found`` () =
  Assert.True(Matcher.matches order "order-42")
  Assert.False(Matcher.matches order "invoice-42")
  Assert.False(Matcher.matches order "")

[<Fact>]
let ``construct is case sensitive and constructIgnoreCase is not`` () =
  let sensitive = Matcher.construct "abc"
  let insensitive = Matcher.constructIgnoreCase "abc"

  Assert.False(Matcher.matches sensitive "ABC")
  Assert.True(Matcher.matches insensitive "ABC")
  Assert.True(Matcher.matches insensitive "abc")

[<Fact>]
let ``matches finds the pattern anywhere unless anchored`` () =
  Assert.True(Matcher.matches (Matcher.construct "b+") "aabbcc")
  Assert.False(Matcher.matches (Matcher.construct "^b+$") "aabbcc")

[<Fact>]
let ``tryMatch returns None when the text does not match`` () =
  Assert.True((Matcher.tryMatch order "nope").IsNone)

[<Fact>]
let ``tryMatch returns the original text and named groups`` () =
  let result = Matcher.tryMatch order "order-42" |> Option.get

  Assert.Equal("order-42", result.Value)
  Assert.Equal("42", result.Groups["id"].Value)
  Assert.Equal("order-42", result.Groups[0].Value)

[<Fact>]
let ``tryMatch exposes captures of the whole match`` () =
  let result = Matcher.tryMatch (Matcher.construct "a+") "xaay" |> Option.get

  Assert.Equal(1, result.Captures.Count)
  Assert.Equal("aa", result.Captures[0].Value)

[<Fact>]
let ``tryMatchDo applies the function to the match result`` () =
  let id =
    Matcher.tryMatchDo order (fun result -> int result.Groups["id"].Value) "order-42"

  Assert.Equal(Some 42, id)

[<Fact>]
let ``tryMatchDo returns None without calling the function when there is no match`` () =
  let mutable called = false

  let result =
    Matcher.tryMatchDo
      order
      (fun _ ->
        called <- true
        0
      )
      "nope"

  Assert.Equal(None, result)
  Assert.False(called)

[<Fact>]
let ``tryMatches returns the text when it matches`` () =
  Assert.Equal(Ok "order-42", Matcher.tryMatches order DidNotMatch "order-42")

[<Fact>]
let ``tryMatches returns the error built from the text when it does not match`` () =
  Assert.Equal(Error(DidNotMatch "nope"), Matcher.tryMatches order DidNotMatch "nope")
