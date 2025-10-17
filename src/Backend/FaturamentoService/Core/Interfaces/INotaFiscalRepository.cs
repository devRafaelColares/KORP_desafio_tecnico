using FaturamentoService.Models;

namespace FaturamentoService.Core.Interfaces;
public interface INotaFiscalRepository
{
    Task AddAsync(NotaFiscal nota);
    Task<NotaFiscal?> GetByIdAsync(int id);
}