using MassTransit;
using PalpiteFC.DataContracts.MessageTypes;
using PalpiteFC.Worker.Persistence.Interfaces;

namespace PalpiteFC.Worker.QueueManager.Consumers;

public class GuessConsumer : IConsumer<GuessMessage>
{
    private readonly ILogger _logger;
    private readonly IGuessService _guessService;

    public GuessConsumer(IGuessService guessService, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GuessConsumer>();
        _guessService = guessService;
    }

    public async Task Consume(ConsumeContext<GuessMessage> context)
    {
        //valida se a mensagem é nula
        if (context.Message is null)
        {
            _logger.LogError("Message is empty.");
            return;
        }
        // chama metodo para processar a mensagem

        await _guessService.ProcessMessage(context.Message!);
    }
}
