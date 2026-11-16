module ChatHandlerTests

open System
open Xunit
open Myers.Estate.JobChat.Robo.API

[<Fact>]
let ``handle echoes a trimmed message`` () =
  Assert.Equal(Ok { Reply = "You said: hi" }, ChatHandler.handle { Message = "  hi  " })

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("   ")>]
let ``handle rejects null or blank messages`` (message: string) =
  Assert.Equal(Error MessageNullOrEmpty, ChatHandler.handle { Message = message })

[<Fact>]
let ``handle rejects messages over 300 characters`` () =
  Assert.Equal(Error(MessageTooLong(301, 300)), ChatHandler.handle { Message = String('a', 301) })
