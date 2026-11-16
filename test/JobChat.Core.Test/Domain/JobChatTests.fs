module JobChatTests

open System
open Xunit
open Myers.Estate.JobChat.Core.Domain
open Myers.Estate.JobChat.Core.Domain.Error

let private chars n = String('a', n)

let private validate systemPrompt resume story =
  JobChat.constructValidated systemPrompt resume story |> Result.map ignore

let private valid = Some "text"

// TODO: These tests should assert results when valid as well as when invalid.  The logic being tested isn't very clear

[<Fact>]
let ``constructValidated accepts valid text`` () =
  Assert.Equal(Ok(), validate valid valid valid)

[<Fact>]
let ``constructValidated accepts exactly 1000 characters after trimming`` () =
  let padded = Some $"  {chars 1000}  "
  Assert.Equal(Ok(), validate padded padded padded)

[<Fact>]
let ``constructValidated rejects missing or blank text`` () =
  for blank in [ None; Some ""; Some "   " ] do
    Assert.Equal(Error [ SystemPromptNullOrEmptyError ], validate blank valid valid)
    Assert.Equal(Error [ ResumeNullOrEmptyError ], validate valid blank valid)
    Assert.Equal(Error [ StoryNullOrEmptyError ], validate valid valid blank)

[<Fact>]
let ``constructValidated rejects text over 1000 characters`` () =
  let tooLong = Some(chars 1001)
  let expected = TooLong(chars 1001, 1000, 1001)

  Assert.Equal(Error [ SystemPromptTooLongError expected ], validate tooLong valid valid)
  Assert.Equal(Error [ ResumeTooLongError expected ], validate valid tooLong valid)
  Assert.Equal(Error [ StoryTooLongError expected ], validate valid valid tooLong)

[<Fact>]
let ``constructValidated accumulates every error`` () =
  let tooLong = Some(chars 1001)
  let expected = TooLong(chars 1001, 1000, 1001)

  Assert.Equal(
    Error
      [
        SystemPromptNullOrEmptyError
        ResumeTooLongError expected
        StoryNullOrEmptyError
      ],
    validate None tooLong (Some "  ")
  )
