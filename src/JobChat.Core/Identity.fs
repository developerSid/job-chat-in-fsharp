namespace Myers.Estate.JobChat.Core

open System

type SortableIdentifier = private SortableIdentifier of Guid

[<RequireQualifiedAccess>]
module SortableIdentifier =

  [<CompiledName("New")>]
  let newId () =
    SortableIdentifier(Guid.CreateVersion7())

  let newIdWrapped wrapper = newId () |> wrapper

  let toString (SortableIdentifier id) = id |> string

  let fromString invalidError (id: string) =
    let mutable guid = Guid.Empty

    if Guid.TryParse(id, &guid) && guid.Version = 7 then
      Ok(SortableIdentifier guid)
    else
      Error(invalidError id)
