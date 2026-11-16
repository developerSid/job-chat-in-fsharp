namespace Myers.Estate.JobChat.Core.Domain

open Myers.Estate.JobChat.Core
open Myers.Estate.JobChat.Core.Collection
open FsToolkit.ErrorHandling
open Myers.Estate.JobChat.Core.Domain.Error

type JobChat =
  private
    {
      SystemPrompt: TrimmedText1000
      Resume: TrimmedText1000
      Story: TrimmedText1000
      History: RingBuffer<TrimmedText300>
    }

module JobChat =
  let private constructSystemPrompt =
    TrimmedText1000.construct SystemPromptNullOrEmptyError SystemPromptTooLongError

  let private constructResume =
    TrimmedText1000.construct ResumeNullOrEmptyError ResumeTooLongError

  let private constructStory =
    TrimmedText1000.construct StoryNullOrEmptyError StoryTooLongError

  let construct systemPrompt resume story =
    {
      SystemPrompt = systemPrompt
      Resume = resume
      Story = story
      History = RingBuffer.constructUnsafe 15
    }

  let constructValidated systemPrompt resume story =
    validation {
      let! systemPrompt = constructSystemPrompt systemPrompt
      and! resume = constructResume resume
      and! story = constructStory story

      return construct systemPrompt resume story
    }
