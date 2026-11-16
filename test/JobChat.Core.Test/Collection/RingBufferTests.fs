module RingBufferTests

open Xunit
open Myers.Estate.JobChat.Core.Collection

type RingBufferError = SizeMustBePositive of size: int

[<Fact>]
let ``add drops the oldest element once the buffer is full`` () =
  let buffer =
    RingBuffer.construct SizeMustBePositive 5
    |> Result.map (fun empty -> [ 1..6 ] |> List.fold (fun b item -> RingBuffer.add item b) empty)

  match buffer with
  | Ok full -> Assert.Equal<int list>([ 2; 3; 4; 5; 6 ], RingBuffer.toList full)
  | Error e -> failwithf $"Expected Ok but got %A{e}"

[<Theory>]
[<InlineData(0)>]
[<InlineData(-1)>]
let ``construct rejects a size that is not positive`` (size: int) =
  match RingBuffer.construct SizeMustBePositive size: Result<RingBuffer<int>, RingBufferError> with
  | Error e -> Assert.Equal(SizeMustBePositive size, e)
  | Ok _ -> failwithf $"Expected Error for size %d{size}"
