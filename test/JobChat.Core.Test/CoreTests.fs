module CoreTests

open System.Collections.Generic
open Xunit
open Myers.Estate.JobChat.Core.Core

type StringError =
  | Blank
  | TooLong of text: string * max: int * actual: int
  | LimitedTooLong of max: int
  | LimitedTooShort of min: int

[<Fact>]
let ``isNull is true for null and false for values`` () =
  Assert.True(isNull (null: string))
  Assert.False(isNull "")
  Assert.False(isNull "x")

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("   ")>]
let ``maybe turns blank strings into None`` (input: string) = Assert.Equal(None, maybe (Some input))

[<Fact>]
let ``maybe preserves non-blank strings and None`` () =
  Assert.Equal(Some " a ", maybe (Some " a "))
  Assert.Equal(None, maybe None)

[<Fact>]
let ``maybeObject turns Some null into None`` () =
  Assert.Equal(None, maybeObject (Some(null: string)))
  Assert.Equal(None, maybeObject (None: string option))
  Assert.Equal(Some "x", maybeObject (Some "x"))

[<Fact>]
let ``swap ignores its second argument`` () = Assert.Equal(1, swap 1 "ignored")

[<Fact>]
let ``constructStringMaybe returns Ok None for blank input`` () =
  Assert.Equal(Ok None, ConstrainedType.constructStringMaybe TooLong id 5 null)
  Assert.Equal(Ok None, ConstrainedType.constructStringMaybe TooLong id 5 "  ")

[<Fact>]
let ``constructStringMaybe accepts input up to maxLen`` () =
  Assert.Equal(Ok(Some "abcde"), ConstrainedType.constructStringMaybe TooLong id 5 "abcde")

[<Fact>]
let ``constructStringMaybe reports too-long input`` () =
  Assert.Equal(Error(TooLong("abcdef", 5, 6)), ConstrainedType.constructStringMaybe TooLong id 5 "abcdef")

[<Fact>]
let ``constructString errors on blank input`` () =
  Assert.Equal(Error Blank, ConstrainedType.constructString Blank TooLong id 5 "")
  Assert.Equal(Error Blank, ConstrainedType.constructString Blank TooLong id 5 null)

[<Fact>]
let ``constructString wraps valid input and reports too-long input`` () =
  Assert.Equal(Ok "abc", ConstrainedType.constructString Blank TooLong id 5 "abc")
  Assert.Equal(Error(TooLong("abcdef", 5, 6)), ConstrainedType.constructString Blank TooLong id 5 "abcdef")

[<Fact>]
let ``constructStringLimited enforces blank, max and min`` () =
  let construct =
    ConstrainedType.constructStringLimited Blank LimitedTooLong LimitedTooShort id 5 2

  Assert.Equal(Error Blank, construct " ")
  Assert.Equal(Error(LimitedTooLong 5), construct "abcdef")
  Assert.Equal(Error(LimitedTooShort 2), construct "a")
  Assert.Equal(Ok "ab", construct "ab")
  Assert.Equal(Ok "abcde", construct "abcde")

[<Fact>]
let ``constructNonEmpty rejects blank and returns original text`` () =
  Assert.Equal(Error Blank, ConstrainedType.constructNonEmpty Blank null)
  Assert.Equal(Error Blank, ConstrainedType.constructNonEmpty Blank "  ")
  Assert.Equal(Ok " a ", ConstrainedType.constructNonEmpty Blank " a ")

[<Fact>]
let ``List removeFirstElement splits head and tail`` () =
  Assert.Equal((None, []), List.removeFirstElement ([]: int list))
  Assert.Equal((Some 1, [ 2; 3 ]), List.removeFirstElement [ 1; 2; 3 ])

[<Fact>]
let ``generateRandomSalt returns the requested size`` () =
  Assert.Equal(16, (RandomNumberGenerator.generateRandomSalt 16).Length)
  Assert.Empty(RandomNumberGenerator.generateRandomSalt 0)

[<Fact>]
let ``generateRandomSalt returns different values each call`` () =
  Assert.NotEqual<byte[]>(RandomNumberGenerator.generateRandomSalt 32, RandomNumberGenerator.generateRandomSalt 32)

[<Fact>]
let ``Dictionary isEmpty handles null, empty and populated`` () =
  Assert.True(Dictionary.isEmpty (null: IDictionary<string, int>))
  Assert.True(Dictionary.isEmpty (Dictionary<string, int>()))
  Assert.False(Dictionary.isEmpty (dict [ "a", 1 ]))

[<Fact>]
let ``Seq peek runs the peeker per element and preserves elements`` () =
  let seen = List<int>()
  let result = Seq.peek seen.Add [ 1; 2; 3 ]

  Assert.Empty(seen) // lazy until enumerated
  Assert.Equal<int list>([ 1; 2; 3 ], List.ofSeq result)
  Assert.Equal<int list>([ 1; 2; 3 ], List.ofSeq seen)
