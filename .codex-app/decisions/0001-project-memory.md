# 0001 - Project Memory

Date: 2026-11-15 22:07 UTC

## Decision
Use `.codex-app/` as the persistent project memory for app development continuity.

## Reason
Long-running AI-assisted development can lose structure after context compaction or in fresh chats. Repository-backed memory gives future agents a short, verifiable starting point.

## Consequences
- Agents must read `.codex-app/state.md` before substantial work.
- Agents must update session logs and state after meaningful changes.
- Source files remain authoritative when docs and code disagree.
