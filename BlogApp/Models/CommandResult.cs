namespace BlogApp.Models;

/// <summary>
/// Response model containing the result of command execution.
/// </summary>
public record CommandResult
{
    /// <summary>
    /// The command that was executed
    /// </summary>
    public required string Command { get; init; }

    /// <summary>
    /// Exit code returned by the process (0 typically indicates success)
    /// </summary>
    public int ExitCode { get; init; }

    /// <summary>
    /// Standard output from the command
    /// </summary>
    public required string Output { get; init; }

    /// <summary>
    /// Error output from the command, if any
    /// </summary>
    public required string ErrorOutput { get; init; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; init; }

    /// <summary>
    /// Whether the command executed successfully (exit code 0)
    /// </summary>
    public bool Success => ExitCode == 0;
}
