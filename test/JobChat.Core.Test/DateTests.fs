module DateTests

open System
open Xunit
open Myers.Estate.JobChat.Core

type DateError = InvalidDate of text: string

[<Theory>]
[<InlineData("2026-11-14")>]
[<InlineData("11/14/2026")>]
[<InlineData("November 14, 2026")>]
[<InlineData(" 2026-11-14 ")>]
let ``construct parses valid dates using the invariant culture`` (input: string) =
  Assert.Equal(Ok(DateOnly(2026, 11, 14)), Date.construct InvalidDate input)

[<Fact>]
let ``construct accepts leap days only in leap years`` () =
  Assert.Equal(Ok(DateOnly(2028, 2, 29)), Date.construct InvalidDate "2028-02-29")
  Assert.Equal(Error(InvalidDate "2027-02-29"), Date.construct InvalidDate "2027-02-29")

[<Theory>]
[<InlineData("")>]
[<InlineData("  ")>]
[<InlineData("not a date")>]
[<InlineData("2026-13-01")>]
[<InlineData("2026-11-31")>]
[<InlineData("14/11/2026")>]
let ``construct returns the error for invalid dates`` (input: string) =
  Assert.Equal(Error(InvalidDate input), Date.construct InvalidDate input)

[<Fact>]
let ``construct returns the error for null instead of throwing`` () =
  Assert.Equal(Error(InvalidDate null), Date.construct InvalidDate null)

[<Fact>]
let ``constructFromDateTime keeps the date and drops the time`` () =
  Assert.Equal(Ok(DateOnly(2026, 11, 14)), Date.constructFromDateTime (DateTime(2026, 11, 14, 23, 59, 59)))
  Assert.Equal(Ok(DateOnly(2026, 11, 14)), Date.constructFromDateTime (DateTime(2026, 11, 14)))

[<Fact>]
let ``maybeConstructFromDateTimeNullable maps null to None`` () =
  Assert.Equal(None, Date.maybeConstructFromDateTimeNullable (Nullable<DateTime>()))

[<Fact>]
let ``maybeConstructFromDateTimeNullable maps a value to its date`` () =
  let value = Nullable(DateTime(2026, 11, 14, 8, 15, 0))
  Assert.Equal(Some(DateOnly(2026, 11, 14)), Date.maybeConstructFromDateTimeNullable value)
