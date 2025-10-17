using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Requests.Notas;
using FaturamentoService.Core.Responses;
using FaturamentoService.Core.Responses.Notas;
using FaturamentoService.Models;
using FaturamentoService.Infrastructure.Data.Context;

namespace FaturamentoService.Services;

public class NotaFiscalService : INotaFiscalService
{
    private readonly INotaFiscalRepository _repo;
    private readonly IEstoqueClient _estoqueClient;
    private readonly AppDbContext _context;

    public NotaFiscalService(INotaFiscalRepository repo, IEstoqueClient estoqueClient, AppDbContext context)
    {
        _repo = repo;
        _estoqueClient = estoqueClient;
        _context = context;
    }

    public async Task<Response<NotaFiscalResponse>> CreateAsync(NotaFiscalCreateRequest request)
    {
        // validações básicas
        if (string.IsNullOrWhiteSpace(request.Numero))
            return new Response<NotaFiscalResponse>(null, 400, "Número da nota é obrigatório.");

        if (request.Itens == null || request.Itens.Count == 0)
            return new Response<NotaFiscalResponse>(null, 400, "Nota precisa conter pelo menos um item.");

        // valida existência dos produtos no EstoqueService
        foreach (var item in request.Itens)
        {
            var exists = await _estoqueClient.ProdutoExistsAsync(item.ProdutoId);
            if (!exists)
                return new Response<NotaFiscalResponse>(null, 400, $"Produto com id {item.ProdutoId} não encontrado no estoque.");
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
        await _context.SaveChangesAsync();

        var response = new NotaFiscalResponse
        {
            Id = nota.Id,
            Numero = nota.Numero,
            Status = nota.Status,
            DataEmissao = nota.DataEmissao,
            Itens = nota.Itens!.Select(it => new Core.Responses.Notas.ItemNotaFiscalResponse
            {
                Id = it.Id,
                ProdutoId = it.ProdutoId,
                Quantidade = it.Quantidade
            }).ToList()
        };

        return new Response<NotaFiscalResponse>(response, 201, "Nota fiscal criada com sucesso.");
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
            Itens = nota.Itens?.Select(it => new Core.Responses.Notas.ItemNotaFiscalResponse
            {
                Id = it.Id,
                ProdutoId = it.ProdutoId,
                Quantidade = it.Quantidade
            }).ToList() ?? new List<Core.Responses.Notas.ItemNotaFiscalResponse>()
        };

        return new Response<NotaFiscalResponse>(response, 200);
    }
}
