namespace Myers.Estate.JobChat.Core.Core

open System
open System.Globalization

type NonZeroPositiveNumber = private NonZeroPositiveNumber of Number

[<RequireQualifiedAccess>]
module NonZeroPositiveNumber =
  let construct mustBeGreaterThanZero emptyNumberError wasNotAllDigits numberTooBig text =
    if String.IsNullOrWhiteSpace text then
      Error emptyNumberError
    else
      match Int64.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture) with
      | true, parsedNum when parsedNum > 0L -> Ok(NonZeroPositiveNumber parsedNum)
      | true, _ -> Error(mustBeGreaterThanZero text)
      | false, _ when text |> Seq.forall Char.IsAsciiDigit -> Error(numberTooBig text)
      | false, _ -> Error(wasNotAllDigits text)

  let value (NonZeroPositiveNumber num) = num
