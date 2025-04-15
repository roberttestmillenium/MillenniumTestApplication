using MillenniumTestApplication.Domain.Entities;

namespace MillenniumTestApplication.Domain.Interfaces
{
    public interface ICardReader
    {
        Task<CardDetails?> GetCardDetails(string userId, string cardNumber);
    }
}
