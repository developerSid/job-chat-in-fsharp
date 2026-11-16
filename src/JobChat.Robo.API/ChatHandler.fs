namespace Myers.Estate.JobChat.Robo.API

open Myers.Estate.JobChat.Core

module ChatHandler =
  /// Validates the request message and builds a stub reply.
  let handle (request: ChatRequest) : Result<ChatResponse, ChatError> =
    request.Message
    |> Text.nullSafe
    |> TrimmedText300.construct MessageNullOrEmpty (fun (_, maxLength, length) -> MessageTooLong(length, maxLength))
    |> Result.map (fun message ->
      {
        Reply = $"You said: %s{TrimmedText300.unwrap message}"
      }
    )
