using EstoqueService.Core.Enums;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Movimentacoes;
using EstoqueService.Core.Responses;
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

    /// <summary>
    /// Processa baixa de estoque em lote com controle transacional
    /// </summary>
    public async Task<Response<bool>> ProcessarBaixaEmLoteAsync(MovimentacaoBatchRequest request)
    {
        if (request.Itens == null || request.Itens.Count == 0)
            return new Response<bool>(false, 400, "Nenhum item informado para baixa.");

        try
        {
            // Inicia transação para garantir atomicidade
            await _unitOfWork.BeginTransactionAsync();

            var movimentacoes = new List<MovimentacaoEstoque>();
            var erros = new List<string>();

            foreach (var item in request.Itens)
            {
                // Validações de negócio
                if (item.Quantidade <= 0)
                {
                    erros.Add($"Quantidade inválida para produto {item.ProdutoId}.");
                    continue;
                }

                // Busca produto com lock pessimista para evitar condição de corrida
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);

                if (produto == null)
                {
                    erros.Add($"Produto {item.ProdutoId} não encontrado.");
                    continue;
                }

                // Validação crítica: saldo suficiente
                if (produto.Saldo < item.Quantidade)
                {
                    erros.Add($"Saldo insuficiente para produto '{produto.Descricao}' (SKU: {produto.CodigoSKU}). Disponível: {produto.Saldo}, Solicitado: {item.Quantidade}");
                    continue;
                }

                // Baixa o saldo
                produto.Saldo -= item.Quantidade;
                await _produtoRepository.UpdateAsync(produto);

                // Registra movimentação
                var movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = produto.Id,
                    Data = DateTime.UtcNow,
                    Quantidade = item.Quantidade,
                    Tipo = TipoMovimentacoesEstoque.Saida,
                    Observacao = request.Observacao ?? "Baixa via impressão de nota fiscal"
                };
                movimentacoes.Add(movimentacao);
            }

            // Se houver erros, rollback e retorna
            if (erros.Any())
            {
                await _unitOfWork.RollbackAsync();
                return new Response<bool>(false, 400, string.Join(" | ", erros));
            }

            // Persiste movimentações
            await _movimentacaoRepository.AddRangeAsync(movimentacoes);

            // Commit da transação
            await _unitOfWork.CommitAsync();

            return new Response<bool>(true, 200, $"Baixa de estoque processada com sucesso. {movimentacoes.Count} item(ns) movimentado(s).");
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<bool>(false, 409, "Conflito de concorrência detectado. O estoque foi alterado por outro processo. Tente novamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<bool>(false, 500, $"Erro ao processar baixa: {ex.Message}");
        }
    }
}