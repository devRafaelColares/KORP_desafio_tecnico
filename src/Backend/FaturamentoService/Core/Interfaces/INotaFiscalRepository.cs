using FaturamentoService.Models;

namespace FaturamentoService.Core.Interfaces;
public interface INotaFiscalRepository
{
    Task AddAsync(NotaFiscal nota);
    Task<NotaFiscal?> GetByIdAsync(int id);
    Task DeleteAsync(NotaFiscal nota);
    Task<(List<NotaFiscal> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
}