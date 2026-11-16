namespace Myers.Estate.JobChat.Core.Domain.Error

open Myers.Estate.JobChat.Core

type TooLongText = Text
type TooLongLength = int
type ExpectedLength = int

type TooLong = TooLongText * TooLongLength * ExpectedLength
