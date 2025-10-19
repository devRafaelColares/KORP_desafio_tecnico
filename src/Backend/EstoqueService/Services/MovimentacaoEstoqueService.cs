using EstoqueService.Core.Enums;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Movimentacoes;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Estoque;
using EstoqueService.Models;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Services;

/// <summary>
/// Serviço responsável por gerenciar movimentações de estoque
/// </summary>
public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MovimentacaoEstoqueService(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoRepository = movimentacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<BaixaProdutoResultado>> ProcessarMovimentacaoAsync(MovimentacaoEstoqueRequest request)
    {
        if (request.Quantidade <= 0)
            return new Response<BaixaProdutoResultado>(null, 400, "Quantidade inválida.");

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Busca produto
            var produto = await _produtoRepository.GetByIdAsync(request.ProdutoId);
            if (produto == null)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<BaixaProdutoResultado>(null, 404, "Produto não encontrado.");
            }

            // Validação de saldo para saída
            if (request.Tipo == TipoMovimentacoesEstoque.Saida && produto.Saldo < request.Quantidade)
            {
                await _unitOfWork.RollbackAsync();
                return new Response<BaixaProdutoResultado>(null, 400, $"Saldo insuficiente para produto '{produto.Descricao}' (SKU: {produto.CodigoSKU}).");
            }

            // Atualiza saldo
            produto.Saldo += (request.Tipo == TipoMovimentacoesEstoque.Entrada ? request.Quantidade : -request.Quantidade);
            await _produtoRepository.UpdateAsync(produto);

            // Registra movimentação
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produto.Id,
                Data = DateTime.UtcNow,
                Quantidade = request.Quantidade,
                Tipo = request.Tipo,
                Observacao = request.Observacao
            };
            await _movimentacaoRepository.AddAsync(movimentacao);

            await _unitOfWork.CommitAsync();

            var resultado = new BaixaProdutoResultado
            {
                ProdutoId = produto.Id,
                SaldoFinal = produto.Saldo,
                QuantidadeMovimentada = request.Quantidade,
                Tipo = request.Tipo,
                Sucesso = true,
                Mensagem = "Movimentação realizada com sucesso."
            };

            return new Response<BaixaProdutoResultado>(resultado, 200);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<BaixaProdutoResultado>(null, 500, $"Erro ao processar movimentação: {ex.Message}");
        }
    }

    public async Task<Response<List<BaixaProdutoResultado>>> ProcessarBaixaLoteAsync(MovimentacaoBatchRequest request)
    {
        var resultados = new List<BaixaProdutoResultado>();
        var erros = new List<string>();

        if (request.Itens == null || request.Itens.Count == 0)
            return new Response<List<BaixaProdutoResultado>>(null, 400, "Nenhum item informado para movimentação.");

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            foreach (var item in request.Itens)
            {
                if (item.Quantidade <= 0)
                {
                    erros.Add($"Quantidade inválida para produto {item.ProdutoId}.");
                    resultados.Add(new BaixaProdutoResultado
                    {
                        ProdutoId = item.ProdutoId,
                        QuantidadeMovimentada = item.Quantidade,
                        Sucesso = false,
                        Mensagem = "Quantidade inválida."
                    });
                    continue;
                }

                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);
                if (produto == null)
                {
                    erros.Add($"Produto {item.ProdutoId} não encontrado.");
                    resultados.Add(new BaixaProdutoResultado
                    {
                        ProdutoId = item.ProdutoId,
                        QuantidadeMovimentada = item.Quantidade,
                        Sucesso = false,
                        Mensagem = "Produto não encontrado."
                    });
                    continue;
                }

                if (produto.Saldo < item.Quantidade)
                {
                    erros.Add($"Saldo insuficiente para produto '{produto.Descricao}' (SKU: {produto.CodigoSKU}).");
                    resultados.Add(new BaixaProdutoResultado
                    {
                        ProdutoId = produto.Id,
                        QuantidadeMovimentada = item.Quantidade,
                        SaldoFinal = produto.Saldo,
                        Sucesso = false,
                        Mensagem = "Saldo insuficiente."
                    });
                    continue;
                }

                produto.Saldo -= item.Quantidade;
                await _produtoRepository.UpdateAsync(produto);

                var movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = produto.Id,
                    Data = DateTime.UtcNow,
                    Quantidade = item.Quantidade,
                    Tipo = TipoMovimentacoesEstoque.Saida,
                    Observacao = request.Observacao ?? "Movimentação em lote"
                };
                await _movimentacaoRepository.AddAsync(movimentacao);

                resultados.Add(new BaixaProdutoResultado
                {
                    ProdutoId = produto.Id,
                    QuantidadeMovimentada = item.Quantidade,
                    SaldoFinal = produto.Saldo,
                    Tipo = TipoMovimentacoesEstoque.Saida,
                    Sucesso = true,
                    Mensagem = "Movimentação realizada com sucesso."
                });
            }

            if (erros.Any())
            {
                await _unitOfWork.RollbackAsync();
                return new Response<List<BaixaProdutoResultado>>(resultados, 400, string.Join(" | ", erros));
            }

            await _unitOfWork.CommitAsync();
            return new Response<List<BaixaProdutoResultado>>(resultados, 200, "Movimentação em lote realizada com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<List<BaixaProdutoResultado>>(null, 500, $"Erro ao processar movimentação em lote: {ex.Message}");
        }
    }
}