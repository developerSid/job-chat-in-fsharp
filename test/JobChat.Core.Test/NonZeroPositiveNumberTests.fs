module NonZeroPositiveNumberTests

open System
open Xunit
open Myers.Estate.JobChat.Core.Core

type NumberError =
  | Empty
  | NotAllDigits of text: string
  | MustBeGreaterThanZero of text: string
  | TooBig of text: string

let private construct text =
  NonZeroPositiveNumber.construct MustBeGreaterThanZero Empty NotAllDigits TooBig text
  |> Result.map NonZeroPositiveNumber.value

[<Fact>]
let ``NonZeroPositiveNumber accepts positive numbers`` () =
  Assert.Equal(Ok 12L, construct "12")
  Assert.Equal(Ok Int64.MaxValue, construct "9223372036854775807")

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("  ")>]
let ``NonZeroPositiveNumber reports empty input`` (input: string) =
  Assert.Equal(Error Empty, construct input)

[<Theory>]
[<InlineData("abc")>]
[<InlineData("1.5")>]
[<InlineData("+5")>]
[<InlineData("-5")>]
[<InlineData(" 12")>]
let ``NonZeroPositiveNumber reports non-digit input`` (input: string) =
  Assert.Equal(Error(NotAllDigits input), construct input)

[<Fact>]
let ``NonZeroPositiveNumber rejects zero`` () =
  Assert.Equal(Error(MustBeGreaterThanZero "0"), construct "0")
  Assert.Equal(Error(MustBeGreaterThanZero "000"), construct "000")

[<Fact>]
let ``NonZeroPositiveNumber reports overflow`` () =
  Assert.Equal(Error(TooBig "9223372036854775808"), construct "9223372036854775808")
