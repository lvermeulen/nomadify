namespace Nomadify.Execution;

public record ProcessRunResult(bool Success, string Output, string Error, int ExitCode);
