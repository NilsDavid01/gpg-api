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
}

public record EncryptRequest(string Text, string Recipient);
public record DecryptRequest(string Text);
