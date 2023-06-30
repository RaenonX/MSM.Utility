using Grpc.Core;
using MSM.Proto;

namespace MSM.Calc.Services;

public class CalculatorService : Calculator.CalculatorBase {
    private readonly ILogger<CalculatorService> _logger;

    public CalculatorService(ILogger<CalculatorService> logger) {
        _logger = logger;
    }

    public override Task<CalcBarReply> CalcBar(CalcBarRequest request, ServerCallContext context) {
        _logger.LogInformation(
            "Calculating bars of {Item} from {Start} to {End}",
            request.Item, request.Start, request.End
        );

        return Task.FromResult(new CalcBarReply {
            Message = $"Calculating bars of {request.Item} from {request.Start} to {request.End}"
        });
    }
}