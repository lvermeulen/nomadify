using System;

namespace Nomadify.Execution.Exceptions;

public class ExitCodeException(int exitCode) : Exception
{
    public int ExitCode { get; } = exitCode;

    public static void ExitNow(int exitCode = 1) => throw new ExitCodeException(exitCode);
}
