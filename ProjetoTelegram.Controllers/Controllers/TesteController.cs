using Microsoft.AspNetCore.Mvc;

namespace ProjetoTelegram.Controllers
{
    [ApiController]
    [Route("")]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(
                $"""
                Deu bom até!
                {nameof(Environment.OSVersion)}: {Environment.OSVersion}
                {nameof(Environment.NewLine)}: {Environment.NewLine}
                {nameof(Environment.ProcessId)}: {Environment.ProcessId}
                {nameof(Environment.ProcessorCount)}: {Environment.ProcessorCount}
                {nameof(Environment.CommandLine)}: {Environment.CommandLine}
                {nameof(Environment.CurrentDirectory)}: {Environment.CurrentDirectory}
                {nameof(Environment.MachineName)}: {Environment.MachineName}
                {nameof(Environment.ProcessPath)}: {Environment.ProcessPath}
                {nameof(Environment.StackTrace)}: {Environment.StackTrace}
                {nameof(Environment.SystemDirectory)}: {Environment.SystemDirectory}
                {nameof(Environment.UserDomainName)}: {Environment.UserDomainName}
                {WriteEnvieronment()}
                """);
        }

        private string WriteEnvieronment()
        {
            var variables = Environment.GetEnvironmentVariables();
            var result = "";

            foreach (var key in variables.Keys)
            {
                result += $"{key}: {variables[key]}\n";
            }

            return result;
        }
    }
}
