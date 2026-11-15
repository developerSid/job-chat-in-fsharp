module DiscriminatedUnionsTests

open Xunit
open Myers.Estate.JobChat.Core.Core

type Color =
  | Red
  | Green
  | Rgb of int

[<Fact>]
let ``DiscriminatedUnions toText returns the case name`` () =
  Assert.Equal("Red", DiscriminatedUnions.toText Red)
  Assert.Equal("Rgb", DiscriminatedUnions.toText (Rgb 1))

[<Fact>]
let ``DiscriminatedUnions fromText round-trips fieldless cases`` () =
  Assert.Equal(Some Green, DiscriminatedUnions.fromText<Color> "Green")

[<Fact>]
let ``DiscriminatedUnions fromText returns None for unknown or data-carrying cases`` () =
  Assert.Equal(None, DiscriminatedUnions.fromText<Color> "Blue")
  Assert.Equal(None, DiscriminatedUnions.fromText<Color> "Rgb")
  Assert.Equal(None, DiscriminatedUnions.fromText<Color> "red")
