namespace Myers.Estate.JobChat.Core.Core

open System

[<RequireQualifiedAccess>]
// Shared constructors for validating and wrapping constrained string values.
module ConstrainedType =
  /// Optionally constructs a constrained string, returning None for null or blank input.
  /// Returns the supplied too-long error when the input exceeds maxLen.
  let constructStringMaybe tooLongError ctor maxLen str =
    if String.IsNullOrWhiteSpace(str) then
      Ok None
    elif str |> String.length > maxLen then
      Error(tooLongError (str, maxLen, (String.length str)))
    else
      Ok(Some(ctor str))

  /// Constructs a constrained string, returning the supplied errors for null/blank or too-long input.
  let constructString nullOrEmptyError tooLongError ctor maxLen str =
    match constructStringMaybe tooLongError ctor maxLen str with
    | Error error -> Error error
    | Ok str ->
      match str with
      | None -> Error(nullOrEmptyError)
      | Some str -> Ok(str)

  /// Constructs a constrained string whose length is between minLen and maxLen, inclusive.
  /// Returns the supplied error for null/blank, too-long, or too-short input.
  let constructStringLimited
    createStringLimitedWasNullOrEmpty
    createStringLimitedWasTooLong
    createStringLimitedWasTooShort
    ctor
    maxLen
    minLen
    str
    =
    if String.IsNullOrWhiteSpace(str) || String.IsNullOrEmpty(str) then
      Error(createStringLimitedWasNullOrEmpty)
    elif str |> String.length > maxLen then
      Error(createStringLimitedWasTooLong maxLen)
    elif str |> String.length < minLen then
      Error(createStringLimitedWasTooShort minLen)
    else
      Ok(ctor str)

  /// Rejects null, empty, and whitespace-only text; otherwise returns the original text.
  let constructNonEmpty createStringWasNullOrEmpty text =
    if String.IsNullOrWhiteSpace(text) then
      Error createStringWasNullOrEmpty
    else
      Ok text
