module ValidationTests

open System
open Xunit
open Myers.Estate.JobChat.Core

type ValidationError =
  | TooShort of text: string * min: int * actual: int
  | TooLong of text: string * max: int * actual: int
  | TooManyItems of max: int
  | InFuture
  | IsToday
  | Missing
  | NotFound of id: Guid
  | Equal
  | Negative
  | WrongPrefix of text: string
  | WrongSuffix of text: string

let private today = DateOnly(2026, 11, 14)
let private now = DateTime(2026, 11, 14, 12, 30, 0)

[<Fact>]
let ``minLength allows text at or above the minimum`` () =
  Assert.Equal(Ok "abc", Validations.minLength 3 TooShort "abc")
  Assert.Equal(Ok "abcd", Validations.minLength 3 TooShort "abcd")

[<Fact>]
let ``minLength reports text, minimum and actual length`` () =
  Assert.Equal(Error(TooShort("ab", 3, 2)), Validations.minLength 3 TooShort "ab")

[<Fact>]
let ``maxLength allows text at or below the maximum`` () =
  Assert.Equal(Ok "abc", Validations.maxLength 3 TooLong "abc")
  Assert.Equal(Ok "", Validations.maxLength 3 TooLong "")

[<Fact>]
let ``maxLength reports text, maximum and actual length`` () =
  Assert.Equal(Error(TooLong("abcd", 3, 4)), Validations.maxLength 3 TooLong "abcd")

[<Fact>]
let ``maxItems allows lists up to and including the maximum`` () =
  Assert.Equal(Ok [], Validations.maxItems 2 TooManyItems ([]: int list))
  Assert.Equal(Ok [ 1; 2 ], Validations.maxItems 2 TooManyItems [ 1; 2 ])

[<Fact>]
let ``maxItems rejects lists over the maximum`` () =
  Assert.Equal(Error(TooManyItems 2), Validations.maxItems 2 TooManyItems [ 1; 2; 3 ])

[<Fact>]
let ``notFutureDateTime allows past and present but not future`` () =
  Assert.Equal(Ok(now.AddSeconds -1.0), Validations.notFutureDateTime InFuture now (now.AddSeconds -1.0))
  Assert.Equal(Ok now, Validations.notFutureDateTime InFuture now now)
  Assert.Equal(Error InFuture, Validations.notFutureDateTime InFuture now (now.AddSeconds 1.0))

[<Fact>]
let ``notFuture allows past and present but not future`` () =
  Assert.Equal(Ok(today.AddDays -1), Validations.notFuture InFuture today (today.AddDays -1))
  Assert.Equal(Ok today, Validations.notFuture InFuture today today)
  Assert.Equal(Error InFuture, Validations.notFuture InFuture today (today.AddDays 1))

[<Fact>]
let ``notToday rejects only today`` () =
  Assert.Equal(Error IsToday, Validations.notToday IsToday today today)
  Assert.Equal(Ok(today.AddDays -1), Validations.notToday IsToday today (today.AddDays -1))
  Assert.Equal(Ok(today.AddDays 1), Validations.notToday IsToday today (today.AddDays 1))

[<Fact>]
let ``notTodayDateTime compares dates and ignores the time of day`` () =
  Assert.Equal(Error IsToday, Validations.notTodayDateTime IsToday now now.Date)
  Assert.Equal(Error IsToday, Validations.notTodayDateTime IsToday now (now.Date.AddHours 23.0))
  Assert.Equal(Ok(now.AddDays -1.0), Validations.notTodayDateTime IsToday now (now.AddDays -1.0))
  Assert.Equal(Ok(now.AddDays 1.0), Validations.notTodayDateTime IsToday now (now.AddDays 1.0))

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("   ")>]
let ``required rejects blank text`` (input: string) =
  Assert.Equal(Error Missing, Validations.required Missing (Some input))

[<Fact>]
let ``required rejects None and returns non-blank text unchanged`` () =
  Assert.Equal(Error Missing, Validations.required Missing None)
  Assert.Equal(Ok " a ", Validations.required Missing (Some " a "))

[<Fact>]
let ``requiredObject rejects None and Some null`` () =
  Assert.Equal(Error Missing, Validations.requiredObject Missing (None: string option))
  Assert.Equal(Error Missing, Validations.requiredObject Missing (Some(null: string)))

[<Fact>]
let ``requiredObject returns present values`` () =
  Assert.Equal(Ok "x", Validations.requiredObject Missing (Some "x"))
  Assert.Equal(Ok 0, Validations.requiredObject Missing (Some 0))

[<Fact>]
let ``notFound reports the searched-for id when the value is missing`` () =
  let id = Guid.NewGuid()
  Assert.Equal(Error(NotFound id), Validations.notFound NotFound id (None: string option))
  Assert.Equal(Error(NotFound id), Validations.notFound NotFound id (Some(null: string)))

[<Fact>]
let ``notFound returns the value when present`` () =
  Assert.Equal(Ok "x", Validations.notFound NotFound (Guid.NewGuid()) (Some "x"))

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("  ")>]
let ``checkValueExists is false for blank text`` (input: string) =
  Assert.False(Validations.checkValueExists (Some input))

[<Fact>]
let ``checkValueExists is false for None and true for text`` () =
  Assert.False(Validations.checkValueExists None)
  Assert.True(Validations.checkValueExists (Some "a"))

[<Fact>]
let ``checkObjectExists is true only for non-null values`` () =
  Assert.False(Validations.checkObjectExists (None: string option))
  Assert.False(Validations.checkObjectExists (Some(null: string)))
  Assert.True(Validations.checkObjectExists (Some "x"))
  Assert.True(Validations.checkObjectExists (Some 0))

[<Fact>]
let ``checkNotEqual errors on equal values and returns the second otherwise`` () =
  Assert.Equal(Error Equal, Validations.checkNotEqual Equal "a" "a")
  Assert.Equal(Ok "b", Validations.checkNotEqual Equal "a" "b")

[<Fact>]
let ``checkNegative rejects only values below zero`` () =
  Assert.Equal(Error Negative, Validations.checkNegative Negative -0.01M)
  Assert.Equal(Ok 0M, Validations.checkNegative Negative 0M)
  Assert.Equal(Ok 1.5M, Validations.checkNegative Negative 1.5M)

[<Fact>]
let ``startsWith checks the prefix`` () =
  Assert.Equal(Ok "prefix-body", Validations.startsWith WrongPrefix "prefix" "prefix-body")
  Assert.Equal(Error(WrongPrefix "body"), Validations.startsWith WrongPrefix "prefix" "body")
  Assert.Equal(Error(WrongPrefix "Prefix-body"), Validations.startsWith WrongPrefix "prefix" "Prefix-body")

[<Fact>]
let ``endsWith checks the suffix`` () =
  Assert.Equal(Ok "body-suffix", Validations.endsWith WrongSuffix "suffix" "body-suffix")
  Assert.Equal(Error(WrongSuffix "body"), Validations.endsWith WrongSuffix "suffix" "body")
  Assert.Equal(Error(WrongSuffix "body-Suffix"), Validations.endsWith WrongSuffix "suffix" "body-Suffix")
