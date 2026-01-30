using System;
using System.Diagnostics;
using System.Text;

namespace GpgApi.Services
{
    public class GpgService
    {
        private const string GpgHome = "/var/app/gpg";

        private string RunGpg(string arguments, string? input = null)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "gpg",
                Arguments = $"--homedir {GpgHome} {arguments}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi)
                ?? throw new Exception("Failed to start gpg process");

            if (!string.IsNullOrEmpty(input))
            {
                process.StandardInput.Write(input);
                process.StandardInput.Close();
            }

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"GPG failed: {stderr}");

            return stdout;
        }

        // ===========================
        // Key management
        // ===========================

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

            RunGpg("--batch --generate-key", keySpec);
        }

        public string ListKeys()
        {
            return RunGpg("--list-keys");
        }

        public string ExportPublicKey(string email)
        {
            return RunGpg($"--armor --export {email}");
        }

        public void ImportKey(string armoredKey)
        {
            RunGpg("--import", armoredKey);
        }

        // ===========================
        // Crypto operations
        // ===========================

        public string Encrypt(string recipient, string message)
        {
            return RunGpg(
                $"--armor --encrypt --trust-model always -r \"{recipient}\"",
                message
            );
        }

        public string Decrypt(string encryptedMessage)
        {
            return RunGpg("--decrypt", encryptedMessage);
        }
    }
}

