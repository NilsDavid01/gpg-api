using System.Diagnostics;

namespace GpgApi.Services;

public class GpgService
{
    private string RunCommand(string args, string input = "")
    {
        var psi = new ProcessStartInfo
        {
            FileName = "gpg",
            Arguments = args,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(psi)!;

        if (!string.IsNullOrEmpty(input))
        {
            process.StandardInput.Write(input);
            process.StandardInput.Close();
        }

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception(error);

        return output;
    }

    public string Encrypt(string text, string recipient)
    {
        return RunCommand($"--encrypt --armor -r {recipient}", text);
    }

    public string Decrypt(string encryptedText)
    {
        return RunCommand("--decrypt", encryptedText);
    }
}
