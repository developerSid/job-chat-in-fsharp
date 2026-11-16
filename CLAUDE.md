# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Git rules

The user handles all git operations. Agents must not modify the git repository or its history: no committing, amending, rebasing, merging, cherry-picking, resetting, stashing, tagging, branching/switching, pushing/pulling, or creating PRs. Read-only commands (`git status`, `git diff`, `git log`, `git show`) are fine. Leave changes uncommitted in the working tree.

## Overview

F# (.NET 10, SDK pinned in `global.json`) solution early in development. Currently a scaffold: `src/JobChat.Core` (library, `Library.fs`) and `test/JobChat.Core.Test` (xUnit). The solution file is the new `.slnx` format (`job-chat-in-fsharp.slnx`).

## Commands

Tools (paket, fantomas, csharpier, husky, fsautocomplete) are pinned in `dotnet-tools.json`; run `dotnet tool restore` first. `.envrc` (direnv) adds `.local/bin` to `PATH`, which provides `paket`, `fsautocomplete` and `format` wrappers.

- Restore: `dotnet paket restore` (or `dotnet restore`)
- Build: `dotnet build`
- Test all: `dotnet test`
- Single test: `dotnet test --filter "FullyQualifiedName~My test"`
- Coverage report: `coverage` (runs tests with coverlet, then ReportGenerator; open `coverage-report/index.html`)
- Format everything: `format` (runs `dotnet fantomas .` then `dotnet csharpier format .`)

## Conventions and architecture notes

- **Dependencies are managed by Paket, not NuGet `PackageReference`.** Add packages to `paket.dependencies`, then list them per-project in that project's `paket.references`, and run `dotnet paket install`/`update` (updates `paket.lock`). Each fsproj imports `.paket/Paket.Restore.targets`. Target framework is `net10.0`, storage is `none` (no `packages/` cache reliance).
- **Prefer `Result` or `Option` over throwing exceptions.** Use `Result` when a failure has a reason the caller should know about (the caller supplies the error value or function, as in `NonZeroPositiveNumber.construct`) and `Option` when a value is simply absent. Do not use `failwith`, `invalidArg`, `raise` etc. for expected failures such as invalid input.
- **Use interpolated strings with the `printf` family.** When calling `printf`, `printfn`, `sprintf`, `failwithf` and the like, pass an interpolated string (`$"..."`) with format specifiers inside the holes, e.g. `printfn $"Got %A{value}"`, rather than a plain format string followed by arguments.
- **F# compile order matters**: new `.fs` files must be added to the `<Compile Include>` list in the fsproj.
- **Pre-commit hook (Husky.Net)** (`.husky/task-runner.json`) runs on staged files: csharpier (xml/csproj/props/targets/config), fantomas (`.fs`/`.fsx`), then `dotnet build --no-restore` and `dotnet test`. Commits fail if formatting, build or tests fail. The hook is installed by a `Husky` target in `JobChat.Core.fsproj` after restore.
- Formatting rules live in `.editorconfig` (used by fantomas); `.paket/` is excluded from csharpier.
