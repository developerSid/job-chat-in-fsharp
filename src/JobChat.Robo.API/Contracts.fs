namespace Myers.Estate.JobChat.Robo.API

type ChatRequest = { Message: string }

type ChatResponse = { Reply: string }

type HealthResponse = { Status: string }

type ChatError =
  | MessageNullOrEmpty
  | MessageTooLong of Length: int * MaxLength: int
