namespace Myers.Estate.JobChat.Core.Domain.Error

type JobChatError =
  | SystemPromptNullOrEmptyError
  | SystemPromptTooLongError of TooLong
  | ResumeNullOrEmptyError
  | ResumeTooLongError of TooLong
  | StoryNullOrEmptyError
  | StoryTooLongError of TooLong
