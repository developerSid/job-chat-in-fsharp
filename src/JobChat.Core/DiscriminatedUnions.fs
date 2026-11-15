namespace Myers.Estate.JobChat.Core.Core

open System.Reflection
open Microsoft.FSharp.Reflection

[<RequireQualifiedAccess>]
module DiscriminatedUnions =
  let toText<'a> (x: 'a) =
    match
      FSharpValue.GetUnionFields(x, typeof<'a>, BindingFlags.NonPublic ||| BindingFlags.Default ||| BindingFlags.Public)
    with
    | case, _ -> case.Name

  let fromText<'a> (s: string) =
    match
      FSharpType.GetUnionCases(typeof<'a>, BindingFlags.NonPublic ||| BindingFlags.Default ||| BindingFlags.Public)
      |> Array.filter (fun case -> case.Name = s)
    with
    | [| case |] when case.GetFields().Length = 0 ->
      Some(
        FSharpValue.MakeUnion(case, [||], BindingFlags.NonPublic ||| BindingFlags.Default ||| BindingFlags.Public)
        :?> 'a
      )
    | _ -> None
