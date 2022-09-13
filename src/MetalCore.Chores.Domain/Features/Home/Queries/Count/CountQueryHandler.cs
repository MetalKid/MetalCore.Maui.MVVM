using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;

namespace MetalCore.Chores.Domain.Features.Home.Queries.Count
{
    public class CountQueryHandler : IQueryHandler<CountQuery, CountDto>
    {
        public async Task<IResult<CountDto>> ExecuteAsync(CountQuery query, CancellationToken token)
        {
            return ResultHelper.Successful(await Task.FromResult(new CountDto { InitialCount = 1 }));
        }
    }
}
