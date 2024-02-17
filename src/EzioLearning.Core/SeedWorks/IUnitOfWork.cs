
namespace EzioLearning.Core.SeedWorks
{
    public interface IUnitOfWork
    {
        public Task<int> CompleteAsync();
    }
}
