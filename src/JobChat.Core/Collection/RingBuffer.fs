namespace Myers.Estate.JobChat.Core.Collection

/// A fixed-capacity, immutable buffer. Once full, adding an item drops the oldest one.
type RingBuffer<'T> =
  private
    {
      Size: int
      Count: int
      /// Items from oldest to newest.
      Items: 'T list
    }

module RingBuffer =
  /// Assumes size is positive and does no validation; a non-positive size gives a buffer that holds one item.
  let constructUnsafe size : RingBuffer<'T> = { Size = size; Count = 0; Items = [] }

  let construct sizeMustBePositive size : Result<RingBuffer<'T>, 'e> =
    if size <= 0 then
      Error(sizeMustBePositive size)
    else
      Ok(constructUnsafe size)

  let isFull (buffer: RingBuffer<'T>) = buffer.Count >= buffer.Size

  /// Adds an item as the newest; when the buffer is full the oldest item falls off.
  let add item (buffer: RingBuffer<'T>) : RingBuffer<'T> =
    if isFull buffer then
      { buffer with
          Items = List.tail buffer.Items @ [ item ]
      }
    else
      { buffer with
          Count = buffer.Count + 1
          Items = buffer.Items @ [ item ]
      }

  /// Items from oldest to newest.
  let toList (buffer: RingBuffer<'T>) : 'T list = buffer.Items

  let toSeq buffer : 'T seq = toList buffer |> List.toSeq

  let toArray buffer = toList buffer |> List.toArray
