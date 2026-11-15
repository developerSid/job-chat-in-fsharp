namespace Myers.Estate.JobChat.Core

open System

module Validations =

  let minLength min error (text: Text) =
    match String.length text with
    | l when l < min -> Error(error (text, min, (String.length text)))
    | _ -> Ok text

  let maxLength max error (text: Text) =
    match String.length text with
    | l when l > max -> Error(error (text, max, (String.length text)))
    | _ -> Ok text

  let maxItems<'t, 'e> max (error: int -> 'e) (listing: List<'t>) =
    if listing.Length <= max then
      Ok listing
    else
      Error(error max)

  let notFutureDateTime error (today: DateTime) (dateTimeIn: DateTime) = // TODO internationalize maybe?
    if dateTimeIn.CompareTo(today) < 1 then
      Ok dateTimeIn
    else
      Error error

  let notFuture error (today: DateOnly) (dateIn: DateOnly) = // TODO internationalize maybe?
    if dateIn.CompareTo(today) < 1 then
      Ok dateIn
    else
      Error error

  let notToday error (today: DateOnly) (dateIn: DateOnly) =
    if dateIn.CompareTo(today) = 0 then
      Error error
    else
      Ok dateIn

  let notTodayDateTime error (today: DateTime) (dateIn: DateTime) =
    if dateIn.Date.CompareTo(today.Date) = 0 then
      Error error
    else
      Ok dateIn

  let required error (txt: Text option) =
    match txt with
    | None -> Error error
    | Some t ->
      match String.IsNullOrEmpty(t) || String.IsNullOrWhiteSpace(t) with
      | true -> Error error
      | false -> Ok t

  let requiredObject<'O, 'E> (error: 'E) (rObj: 'O option) =
    match rObj with
    | None -> Error error
    | Some o ->
      match obj.ReferenceEquals(o, null) with
      | true -> Error error
      | false -> Ok o

  let notFound<'O, 'E> (notFoundError: Guid -> 'E) searchedFor (rObj: 'O option) =
    match rObj with
    | None -> Error(notFoundError searchedFor)
    | Some o ->
      match obj.ReferenceEquals(o, null) with
      | true -> Error(notFoundError searchedFor)
      | false -> Ok o

  let checkValueExists fieldIn =
    match fieldIn with
    | None -> false
    | Some(fieldIn: Text) -> not (String.IsNullOrWhiteSpace fieldIn)

  let checkObjectExists<'O> (fieldIn: 'O option) =
    match fieldIn with
    | None -> false
    | Some o -> not (obj.ReferenceEquals(o, null))

  let checkNotEqual equalityError str1 str2 =
    if str1 = str2 then Error equalityError else Ok(str2)

  let checkNegative error (valueIn: decimal) =
    if valueIn < 0M then Error error else Ok valueIn

  let startsWith didNotStartWithError (prefix: Text) (str: Text) =
    if str.StartsWith prefix then
      Ok str
    else
      Error(didNotStartWithError str)

  let endsWith didNotEndWithError (suffix: Text) (str: Text) =
    if str.EndsWith suffix then
      Ok str
    else
      Error(didNotEndWithError str)
