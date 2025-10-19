using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Requests.Notas;
using FaturamentoService.Core.Responses;
using FaturamentoService.Core.Responses.Notas;
using FaturamentoService.Models;
using EstoqueService.Core.Requests.Movimentacoes;
using System.Linq;

namespace FaturamentoService.Services;

public class NotaFiscalService : INotaFiscalService
{
    private readonly INotaFiscalRepository _repo;
    private readonly IEstoqueClient _estoqueClient;
    private readonly IUnitOfWork _unitOfWork;

    public NotaFiscalService(
        INotaFiscalRepository repo,
        IEstoqueClient estoqueClient,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _estoqueClient = estoqueClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<NotaFiscalResponse>> CreateAsync(NotaFiscalCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Numero))
            return new Response<NotaFiscalResponse>(null, 400, "Número da nota é obrigatório.");

        if (request.Itens == null || request.Itens.Count == 0)
            return new Response<NotaFiscalResponse>(null, 400, "Nota precisa conter pelo menos um item.");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // valida existência dos produtos no EstoqueService
            foreach (var item in request.Itens)
            {
                var exists = await _estoqueClient.ProdutoExistsAsync(item.ProdutoId);
                if (!exists)
                {
                    await _unitOfWork.RollbackAsync();
                    return new Response<NotaFiscalResponse>(null, 400, $"Produto com id {item.ProdutoId} não encontrado no estoque.");
                }
            }

            // montar entidade
            var nota = new NotaFiscal
            {
                Numero = request.Numero,
                Status = Core.Enums.TipoStatusNF.Aberta,
                DataEmissao = DateTime.UtcNow,
                Itens = request.Itens.Select(i => new ItemNotaFiscal
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            await _repo.AddAsync(nota);
            await _unitOfWork.CommitAsync();

            var response = new NotaFiscalResponse
            {
                Id = nota.Id,
                Numero = nota.Numero,
                Status = nota.Status,
                DataEmissao = nota.DataEmissao,
                Itens = nota.Itens!.Select(it => new ItemNotaFiscalResponse
                {
                    Id = it.Id,
                    ProdutoId = it.ProdutoId,
                    Quantidade = it.Quantidade
                }).ToList()
            };

            return new Response<NotaFiscalResponse>(response, 201, "Nota fiscal criada com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<NotaFiscalResponse>(null, 500, $"Erro ao criar nota fiscal: {ex.Message}");
        }
    }

    public async Task<Response<NotaFiscalResponse>> GetByIdAsync(int id)
    {
        var nota = await _repo.GetByIdAsync(id);
        if (nota == null)
            return new Response<NotaFiscalResponse>(null, 404, "Nota fiscal não encontrada.");

        var response = new NotaFiscalResponse
        {
            Id = nota.Id,
            Numero = nota.Numero,
            Status = nota.Status,
            DataEmissao = nota.DataEmissao,
            Itens = nota.Itens?.Select(it => new ItemNotaFiscalResponse
            {
                Id = it.Id,
                ProdutoId = it.ProdutoId,
                Quantidade = it.Quantidade
            }).ToList() ?? new List<ItemNotaFiscalResponse>()
        };

        return new Response<NotaFiscalResponse>(response, 200);
    }

    public async Task<Response<object>> DeleteAsync(int id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var nota = await _repo.GetByIdAsync(id);
            if (nota == null)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<object>(null, 404, "Nota fiscal não encontrada.");
            }

            await _repo.DeleteAsync(nota);
            await _unitOfWork.CommitAsync();

            return new Response<object>(null, 200, "Nota fiscal excluída com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<object>(null, 500, $"Erro ao excluir nota fiscal: {ex.Message}");
        }
    }

    public async Task<PagedResponse<List<NotaFiscalResponse>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var (items, total) = await _repo.GetAllAsync(pageNumber, pageSize);

        var responseItems = items.Select(nota => new NotaFiscalResponse
        {
            Id = nota.Id,
            Numero = nota.Numero,
            Status = nota.Status,
            DataEmissao = nota.DataEmissao,
            Itens = nota.Itens?.Select(it => new ItemNotaFiscalResponse
            {
                Id = it.Id,
                ProdutoId = it.ProdutoId,
                Quantidade = it.Quantidade
            }).ToList() ?? []
        }).ToList();

        if (responseItems.Count == 0)
            return new PagedResponse<List<NotaFiscalResponse>>(null, 404, "Nenhuma nota fiscal encontrada.");

        return new PagedResponse<List<NotaFiscalResponse>>(responseItems, total, pageNumber, pageSize);
    }

    public async Task<Response<ImprimirNotaFiscalResponse>> PrintAsync(int id, ImprimirNotaFiscalRequest? request = null)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var nota = await _repo.GetByIdAsync(id);
            if (nota == null)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<ImprimirNotaFiscalResponse>(null, 404, "Nota fiscal não encontrada.");
            }

            if (nota.Status != Core.Enums.TipoStatusNF.Aberta)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<ImprimirNotaFiscalResponse>(null, 400, "Apenas notas com status 'Aberta' podem ser impressas.");
            }

            if (nota.Itens == null || nota.Itens.Count == 0)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<ImprimirNotaFiscalResponse>(null, 400, "Nota não contém itens.");
            }

            // Monta o request para o EstoqueService
            var movimentacaoRequest = new MovimentacaoBatchRequest
            {
                Itens = nota.Itens.Select(it => new ItemBaixaEstoque
                {
                    ProdutoId = it.ProdutoId,
                    Quantidade = it.Quantidade
                }).ToList(),
                Observacao = $"Baixa via impressão da NF {nota.Numero}"
            };

            var sucesso = await _estoqueClient.BaixarEstoqueAsync(movimentacaoRequest);
            if (!sucesso)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<ImprimirNotaFiscalResponse>(null, 400, "Saldo insuficiente para um ou mais produtos.");
            }

            nota.Status = Core.Enums.TipoStatusNF.Fechada;
            await _unitOfWork.CommitAsync();

            var response = new ImprimirNotaFiscalResponse
            {
                Id = nota.Id,
                Numero = nota.Numero,
                Status = (int)nota.Status,
                DataEmissao = nota.DataEmissao,
                Itens = [.. nota.Itens.Select(it => new ItemNotaFiscalResponse
                {
                    Id = it.Id,
                    ProdutoId = it.ProdutoId,
                    Quantidade = it.Quantidade
                })]
            };

            return new Response<ImprimirNotaFiscalResponse>(response, 200, "Nota impressa e estoque baixado com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<ImprimirNotaFiscalResponse>(null, 503, $"Erro ao comunicar EstoqueService: {ex.Message}");
        }
    }
}