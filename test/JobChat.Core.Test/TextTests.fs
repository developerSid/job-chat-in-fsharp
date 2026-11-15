module TextTests

open System
open Xunit
open Myers.Estate.JobChat.Core

type TextError =
  | Blank
  | TooLong of text: string * max: int * actual: int
  | LimitedTooLong of max: int
  | LimitedTooShort of min: int

let private chars n = String('a', n)

// --- Text helpers ---

[<Fact>]
let ``nullSafe maps null to None and keeps other strings`` () =
  Assert.Equal(None, Text.nullSafe null)
  Assert.Equal(Some "", Text.nullSafe "")
  Assert.Equal(Some "a", Text.nullSafe "a")

[<Theory>]
[<InlineData(null, "")>]
[<InlineData("", "")>]
[<InlineData("   ", "")>]
[<InlineData("  a b  ", "a b")>]
let ``trim removes surrounding whitespace and maps null to empty`` (input: string, expected: string) =
  Assert.Equal(expected, Text.trim input)

[<Fact>]
let ``trimmed wraps the trimmed text in Ok`` () =
  Assert.Equal(Ok "a", Text.trimmed " a ")
  Assert.Equal(Ok "", Text.trimmed null)

[<Fact>]
let ``trimNullOrEmpty errors only on null or empty`` () =
  Assert.Equal(Error Blank, Text.trimNullOrEmpty Blank null)
  Assert.Equal(Error Blank, Text.trimNullOrEmpty Blank "")
  Assert.Equal(Ok "", Text.trimNullOrEmpty Blank "   ")
  Assert.Equal(Ok "a", Text.trimNullOrEmpty Blank " a ")

[<Fact>]
let ``lower lowercases, maps null to empty and ignores the current culture`` () =
  Assert.Equal("", Text.lower null)
  Assert.Equal("abc", Text.lower "AbC")
  Assert.Equal("i", Text.lower "I")

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("  ")>]
let ``ofNullOrBlank maps blank input to None`` (input: string) =
  Assert.Equal(None, Text.ofNullOrBlank input)

[<Fact>]
let ``ofNullOrBlank preserves other strings unchanged`` () =
  Assert.Equal(Some " a ", Text.ofNullOrBlank " a ")

[<Fact>]
let ``bridge returns the string and bridgeMaybe maps None to null`` () =
  Assert.Equal("a", Text.bridge "a")
  Assert.Equal("a", Text.bridgeMaybe (Some "a"))
  Assert.Null(Text.bridgeMaybe None)

// --- Untrimmed constrained types (no unwrap available, so assert on Ok/Error) ---

[<Fact>]
let ``Text2 accepts up to 2 characters`` () =
  Assert.True(Result.isOk (Text2.construct Blank TooLong (chars 2)))
  Assert.Equal(Error(TooLong(chars 3, 2, 3)), Text2.construct Blank TooLong (chars 3) |> Result.map ignore)
  Assert.Equal(Error Blank, Text2.construct Blank TooLong " " |> Result.map ignore)

[<Fact>]
let ``Text5 accepts up to 5 characters`` () =
  Assert.True(Result.isOk (Text5.construct Blank TooLong (chars 5)))
  Assert.Equal(Error(TooLong(chars 6, 5, 6)), Text5.construct Blank TooLong (chars 6) |> Result.map ignore)
  Assert.Equal(Error Blank, Text5.construct Blank TooLong null |> Result.map ignore)

[<Fact>]
let ``Text7 accepts up to 7 characters`` () =
  Assert.True(Result.isOk (Text7.construct Blank TooLong (chars 7)))
  Assert.Equal(Error(TooLong(chars 8, 7, 8)), Text7.construct Blank TooLong (chars 8) |> Result.map ignore)
  Assert.Equal(Error Blank, Text7.construct Blank TooLong "" |> Result.map ignore)

[<Fact>]
let ``Text30 accepts up to 30 characters`` () =
  Assert.True(Result.isOk (Text30.construct Blank TooLong (chars 30)))
  Assert.Equal(Error(TooLong(chars 31, 30, 31)), Text30.construct Blank TooLong (chars 31) |> Result.map ignore)
  Assert.Equal(Error Blank, Text30.construct Blank TooLong "  " |> Result.map ignore)

// --- Trimmed constrained types ---

[<Fact>]
let ``TrimmedText5 trims before checking length`` () =
  Assert.Equal(
    Ok "abcde",
    TrimmedText5.construct Blank TooLong (Some "  abcde  ")
    |> Result.map TrimmedText5.unwrap
  )

  Assert.Equal(Error Blank, TrimmedText5.construct Blank TooLong None |> Result.map TrimmedText5.unwrap)

  Assert.Equal(
    Error Blank,
    TrimmedText5.construct Blank TooLong (Some "   ")
    |> Result.map TrimmedText5.unwrap
  )

  Assert.Equal(
    Error(TooLong(chars 6, 5, 6)),
    TrimmedText5.construct Blank TooLong (Some(chars 6))
    |> Result.map TrimmedText5.unwrap
  )

[<Fact>]
let ``TrimmedText5 unwrapMaybe maps None to null`` () =
  let value = TrimmedText5.construct Blank TooLong (Some " ab ") |> Result.toOption
  Assert.Equal("ab", TrimmedText5.unwrapMaybe value)
  Assert.Null(TrimmedText5.unwrapMaybe None)

[<Fact>]
let ``TrimmedText7 trims before checking length`` () =
  Assert.Equal(
    Ok "abcdefg",
    TrimmedText7.construct Blank TooLong (Some "  abcdefg ")
    |> Result.map TrimmedText7.unwrap
  )

  Assert.Equal(Error Blank, TrimmedText7.construct Blank TooLong None |> Result.map TrimmedText7.unwrap)

  Assert.Equal(
    Error(TooLong(chars 8, 7, 8)),
    TrimmedText7.construct Blank TooLong (Some(chars 8))
    |> Result.map TrimmedText7.unwrap
  )

[<Fact>]
let ``TrimmedText30 trims before checking length`` () =
  Assert.True(Result.isOk (TrimmedText30.construct Blank TooLong (Some $" {chars 30} ")))
  Assert.Equal(Error Blank, TrimmedText30.construct Blank TooLong None |> Result.map ignore)
  Assert.Equal(Error Blank, TrimmedText30.construct Blank TooLong (Some "  ") |> Result.map ignore)

  Assert.Equal(
    Error(TooLong(chars 31, 30, 31)),
    TrimmedText30.construct Blank TooLong (Some(chars 31)) |> Result.map ignore
  )

// --- Text50 ---

[<Fact>]
let ``Text50 construct accepts up to 50 characters`` () =
  Assert.True(Result.isOk (Text50.construct Blank TooLong (chars 50)))
  Assert.Equal(Error(TooLong(chars 51, 50, 51)), Text50.construct Blank TooLong (chars 51) |> Result.map ignore)
  Assert.Equal(Error Blank, Text50.construct Blank TooLong "" |> Result.map ignore)

[<Fact>]
let ``Text50 constructMaybe returns None for blank input`` () =
  Assert.Equal(Ok None, Text50.constructMaybe TooLong null |> Result.map (Option.map ignore))
  Assert.Equal(Ok None, Text50.constructMaybe TooLong "  " |> Result.map (Option.map ignore))
  Assert.True(Text50.constructMaybe TooLong (chars 50) |> Result.map Option.isSome = Ok true)
  Assert.Equal(Error(TooLong(chars 51, 50, 51)), Text50.constructMaybe TooLong (chars 51) |> Result.map ignore)

[<Fact>]
let ``Text50 constructLimited enforces blank, max and min`` () =
  let construct str =
    Text50.constructLimited Blank LimitedTooLong LimitedTooShort 3 str
    |> Result.map ignore

  Assert.Equal(Error Blank, construct " ")
  Assert.Equal(Error(LimitedTooShort 3), construct "ab")
  Assert.Equal(Error(LimitedTooLong 50), construct (chars 51))
  Assert.Equal(Ok(), construct "abc")
  Assert.Equal(Ok(), construct (chars 50))

[<Fact>]
let ``TrimmedText50 construct trims and unwraps`` () =
  Assert.Equal(
    Ok "ab",
    TrimmedText50.construct Blank TooLong (Some "  ab  ")
    |> Result.map TrimmedText50.unwrap
  )

  Assert.Equal(Error Blank, TrimmedText50.construct Blank TooLong None |> Result.map TrimmedText50.unwrap)

  Assert.Equal(
    Error Blank,
    TrimmedText50.construct Blank TooLong (Some " ")
    |> Result.map TrimmedText50.unwrap
  )

  Assert.Equal(
    Error(TooLong(chars 51, 50, 51)),
    TrimmedText50.construct Blank TooLong (Some(chars 51))
    |> Result.map TrimmedText50.unwrap
  )

[<Fact>]
let ``TrimmedText50 constructMaybe treats None and blank as Ok None`` () =
  let construct text =
    TrimmedText50.constructMaybe TooLong text
    |> Result.map (Option.map TrimmedText50.unwrap)

  Assert.Equal(Ok None, construct None)
  Assert.Equal(Ok None, construct (Some "   "))
  Assert.Equal(Ok(Some "ab"), construct (Some " ab "))
  Assert.Equal(Error(TooLong(chars 51, 50, 51)), construct (Some(chars 51)))

[<Fact>]
let ``TrimmedText50 constructLimited wraps valid text and passes errors through`` () =
  let construct str =
    TrimmedText50.constructLimited Blank LimitedTooLong LimitedTooShort 3 str
    |> Result.map TrimmedText50.unwrap

  Assert.Equal(Ok "abc", construct "abc")
  Assert.Equal(Error Blank, construct "")
  Assert.Equal(Error(LimitedTooShort 3), construct "ab")
  Assert.Equal(Error(LimitedTooLong 50), construct (chars 51))

[<Fact>]
let ``TrimmedText50 unwrapMaybe maps None to null`` () =
  let value = TrimmedText50.construct Blank TooLong (Some " ab ") |> Result.toOption
  Assert.Equal("ab", TrimmedText50.unwrapMaybe value)
  Assert.Null(TrimmedText50.unwrapMaybe None)

// --- Text300 ---

[<Fact>]
let ``Text300 accepts up to 300 characters`` () =
  Assert.True(Result.isOk (Text300.construct Blank TooLong (chars 300)))
  Assert.Equal(Error(TooLong(chars 301, 300, 301)), Text300.construct Blank TooLong (chars 301) |> Result.map ignore)
  Assert.Equal(Error Blank, Text300.construct Blank TooLong null |> Result.map ignore)
  Assert.Equal(Ok None, Text300.constructMaybe TooLong " " |> Result.map (Option.map ignore))

[<Fact>]
let ``TrimmedText300 trims and unwraps`` () =
  Assert.Equal(
    Ok "ab",
    TrimmedText300.construct Blank TooLong (Some " ab ")
    |> Result.map TrimmedText300.unwrap
  )

  Assert.Equal(Error Blank, TrimmedText300.construct Blank TooLong None |> Result.map TrimmedText300.unwrap)

  Assert.Equal(
    Error(TooLong(chars 301, 300, 301)),
    TrimmedText300.construct Blank TooLong (Some(chars 301))
    |> Result.map TrimmedText300.unwrap
  )

[<Fact>]
let ``TrimmedText300 constructMaybe and unwrapMaybe handle missing values`` () =
  let construct text =
    TrimmedText300.constructMaybe TooLong text
    |> Result.map (Option.map TrimmedText300.unwrap)

  Assert.Equal(Ok None, construct None)
  Assert.Equal(Ok None, construct (Some "  "))
  Assert.Equal(Ok(Some "ab"), construct (Some " ab "))
  Assert.Equal(Error(TooLong(chars 301, 300, 301)), construct (Some(chars 301)))
  Assert.Null(TrimmedText300.unwrapMaybe None)

// --- Text1000 ---

[<Fact>]
let ``Text1000 accepts up to 1000 characters`` () =
  Assert.True(Result.isOk (Text1000.construct Blank TooLong (chars 1000)))

  Assert.Equal(
    Error(TooLong(chars 1001, 1000, 1001)),
    Text1000.construct Blank TooLong (chars 1001) |> Result.map ignore
  )

  Assert.Equal(Error Blank, Text1000.construct Blank TooLong "" |> Result.map ignore)
  Assert.Equal(Ok None, Text1000.constructMaybe TooLong null |> Result.map (Option.map ignore))

[<Fact>]
let ``TrimmedText1000 trims and unwraps`` () =
  Assert.Equal(
    Ok "ab",
    TrimmedText1000.construct Blank TooLong (Some " ab ")
    |> Result.map TrimmedText1000.unwrap
  )

  Assert.Equal(
    Error Blank,
    TrimmedText1000.construct Blank TooLong None
    |> Result.map TrimmedText1000.unwrap
  )

  Assert.Equal(
    Error(TooLong(chars 1001, 1000, 1001)),
    TrimmedText1000.construct Blank TooLong (Some(chars 1001))
    |> Result.map TrimmedText1000.unwrap
  )

[<Fact>]
let ``TrimmedText1000 constructMaybe and unwrapMaybe handle missing values`` () =
  let construct text =
    TrimmedText1000.constructMaybe TooLong text
    |> Result.map (Option.map TrimmedText1000.unwrap)

  Assert.Equal(Ok None, construct None)
  Assert.Equal(Ok None, construct (Some "  "))
  Assert.Equal(Ok(Some "ab"), construct (Some " ab "))
  Assert.Equal(Error(TooLong(chars 1001, 1000, 1001)), construct (Some(chars 1001)))
  Assert.Null(TrimmedText1000.unwrapMaybe None)
