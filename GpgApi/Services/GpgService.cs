using System.Diagnostics;

namespace GpgApi.Services;

public class GpgService
{
    private string RunCommand(string args, string? input = null)
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
        return RunCommand(
            $"--batch --yes --armor --encrypt -r \"{recipient}\"",
            text
        );
    }

    public string Decrypt(string encryptedText)
    {
        return RunCommand(
            "--batch --yes --decrypt",
            encryptedText
        );
    }

    public void GenerateKey(string name, string email)
    {
        var keySpec = $@"
Key-Type: RSA
Key-Length: 2048
Name-Real: {name}
Name-Email: {email}
Expire-Date: 0
%no-protection
%commit
";

        RunCommand("--batch --generate-key", keySpec);
    }

    public string ListKeys()
    {
        return RunCommand("--list-keys");
    }

    public void ImportKey(string armoredKey)
    {
        RunCommand("--import", armoredKey);
    }

    public string ExportPublicKey(string keyId)
    {
        return RunCommand($"--armor --export \"{keyId}\"");
    }
}

