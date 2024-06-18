using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Libraries.DataContracts.MessageTypes;
namespace PalpiteFC.Worker.Persistence.Interfaces;

public interface IGuessService
{
    Task ProcessMessage(GuessMessage message);
}