using Microsoft.AspNetCore.Mvc;
using GpgApi.Services;

namespace GpgApi.Controllers;

[ApiController]
[Route("api/gpg")]
public class GpgController : ControllerBase
{
    private readonly GpgService _gpg;

    public GpgController(GpgService gpg)
    {
        _gpg = gpg;
    }

    [HttpPost("encrypt")]
    public IActionResult Encrypt([FromBody] EncryptRequest request)
    {
        var result = _gpg.Encrypt(request.Text, request.Recipient);
        return Ok(new { encrypted = result });
    }

    [HttpPost("decrypt")]
    public IActionResult Decrypt([FromBody] DecryptRequest request)
    {
        var result = _gpg.Decrypt(request.Text);
        return Ok(new { decrypted = result });
    }

    // Generate key on the API machine
    [HttpPost("keys/generate")]
    public IActionResult GenerateKey([FromBody] GenerateKeyRequest request)
    {
        _gpg.GenerateKey(request.Name, request.Email);
        return Ok(new { status = "key generated" });
    }

    // List keys on the API machine
    [HttpGet("keys")]
    public IActionResult ListKeys()
    {
        var keys = _gpg.ListKeys();
        return Ok(new { keys });
    }

     // Import a public key into the API machine
    [HttpPost("keys/import")]
    public IActionResult ImportKey([FromBody] string armoredKey)
    {
    _gpg.ImportKey(armoredKey);
    return Ok(new { status = "key imported" });
    }

    // Export a public key from the API machine
    [HttpGet("keys/export")]
    public IActionResult ExportKey([FromQuery] string keyId)
    {
    var key = _gpg.ExportPublicKey(keyId);
    return Content(key, "text/plain");
}

}

public record EncryptRequest(string Text, string Recipient);
public record DecryptRequest(string Text);
public record GenerateKeyRequest(string Name, string Email);

