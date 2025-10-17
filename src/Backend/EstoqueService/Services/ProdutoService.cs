using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Produto;
using EstoqueService.Core.Responses.Produto;
using EstoqueService.Core.Responses;
using EstoqueService.Models;

namespace EstoqueService.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<ProdutoResponse>> CreateAsync(ProdutoCreateRequest request)
    {
        var produto = new Produto
        {
            Descricao = request.Descricao,
            CodigoSKU = request.CodigoSKU,
            Preco = request.Preco,
            Saldo = request.Saldo
        };

        await _produtoRepository.AddAsync(produto);
        await _unitOfWork.SaveChangesAsync();

        var response = new ProdutoResponse
        {
            Id = produto.Id,
            Descricao = produto.Descricao,
            CodigoSKU = produto.CodigoSKU,
            Preco = produto.Preco,
            Saldo = produto.Saldo
        };

        return new Response<ProdutoResponse>(response, 201, "Produto criado com sucesso.");
    }

    public async Task<Response<ProdutoResponse>> GetByIdAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            return new Response<ProdutoResponse>(null, 404, "Produto não encontrado.");

        var response = new ProdutoResponse
        {
            Id = produto.Id,
            Descricao = produto.Descricao,
            CodigoSKU = produto.CodigoSKU,
            Preco = produto.Preco,
            Saldo = produto.Saldo
        };

        return new Response<ProdutoResponse>(response, 200);
    }

    public async Task<PagedResponse<List<ProdutoResponse>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var (produtos, totalCount) = await _produtoRepository.GetAllAsync(pageNumber, pageSize);
        var responseData = produtos.Select(p => new ProdutoResponse
        {
            Id = p.Id,
            Descricao = p.Descricao,
            CodigoSKU = p.CodigoSKU,
            Preco = p.Preco,
            Saldo = p.Saldo
        }).ToList();

        if (responseData.Count == 0)
            return new PagedResponse<List<ProdutoResponse>>(null, 404, "Nenhum produto encontrado.");

        return new PagedResponse<List<ProdutoResponse>>(responseData, totalCount, pageNumber, pageSize);
    }

    public async Task<Response<ProdutoResponse>> UpdateAsync(int id, ProdutoUpdateRequest request)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            return new Response<ProdutoResponse>(null, 404, "Produto não encontrado.");

        if (request.Descricao != null) produto.Descricao = request.Descricao;
        if (request.CodigoSKU != null) produto.CodigoSKU = request.CodigoSKU;
        if (request.Preco.HasValue) produto.Preco = request.Preco.Value;
        if (request.Saldo.HasValue) produto.Saldo = request.Saldo.Value;

        await _produtoRepository.UpdateAsync(produto);
        await _unitOfWork.SaveChangesAsync();

        var response = new ProdutoResponse
        {
            Id = produto.Id,
            Descricao = produto.Descricao,
            CodigoSKU = produto.CodigoSKU,
            Preco = produto.Preco,
            Saldo = produto.Saldo
        };

        return new Response<ProdutoResponse>(response, 200, "Produto atualizado com sucesso.");
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        await _produtoRepository.DeleteAsync(id);
        var changes = await _unitOfWork.SaveChangesAsync();
        if (changes > 0)
            return new Response<bool>(true, 200, "Produto removido com sucesso.");
        return new Response<bool>(false, 404, "Produto não encontrado ou já removido.");
    }
}