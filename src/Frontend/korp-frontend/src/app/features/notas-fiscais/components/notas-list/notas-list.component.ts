import { CommonModule } from "@angular/common";
import { Component, OnInit, signal, ViewChild, ElementRef } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { ItemNotaFiscal, NotaFiscal, StatusNotaFiscal } from "../../../../core/models/nota-fiscal.model";
import { Produto } from "../../../../core/models/produto.model";
import { NotasFiscaisService } from "../../../../core/services/notas-fiscais.service";
import { ProdutosService } from "../../../../core/services/produtos.service";
import { AlertComponent } from "../../../../shared/components/alert/alert.component";
import { LoadingComponent } from "../../../../shared/components/loading/loading.component";
import { CardComponent } from "../../../../shared/components/card/card.component";

interface AlertMessage {
    type: 'success' | 'error' | 'info';
    message: string;
}

@Component({
    selector: 'app-notas-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        AlertComponent,
        LoadingComponent,
        CardComponent
    ],
    templateUrl: './notas-list.component.html',
    styleUrls: ['./notas-list.component.scss']
})
export class NotasListComponent implements OnInit {
    @ViewChild('searchProdutoInput') searchProdutoInput?: ElementRef<HTMLInputElement>;

    // Estados principais
    notas = signal<NotaFiscal[]>([]);
    notasFiltradas = signal<NotaFiscal[]>([]);
    produtos = signal<Produto[]>([]);
    loading = signal(false);
    alert = signal<AlertMessage | null>(null);

    // Modal
    showModal = signal(false);
    notaSelecionada = signal<NotaFiscal | null>(null);
    modoEdicao = signal(false);

    // Edição de itens
    itensEditados = signal<ItemNotaFiscal[]>([]);

    // Adicionar produto
    showAddProduto = signal(false);
    produtoSelecionado: Produto | null = null;
    quantidadeNova = 1;
    produtosFiltrados = signal<Produto[]>([]);
    searchProduto = '';

    // Filtros
    filtroStatus: 'todos' | 'aberta' | 'fechada' = 'todos';
    searchTerm = '';

    // Enum para template
    StatusNotaFiscal = StatusNotaFiscal;

    constructor(
        private notasService: NotasFiscaisService,
        private produtosService: ProdutosService,
        private router: Router
    ) { }

    ngOnInit(): void {
        this.carregarNotas();
        this.carregarProdutos();
    }

    // ========================================
    // Carregamento de Dados
    // ========================================

    carregarNotas(): void {
        this.loading.set(true);

        this.notasService.getAll(1, 100).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    // Ordena por data mais recente
                    const notasOrdenadas = response.data.sort((a: NotaFiscal, b: NotaFiscal) =>
                        new Date(b.dataEmissao).getTime() - new Date(a.dataEmissao).getTime()
                    );
                    this.notas.set(notasOrdenadas);
                    this.filtrarNotas();
                } else {
                    this.showAlert('error', response.message || 'Erro ao carregar notas');
                }
                this.loading.set(false);
            },
            error: (error: any) => {
                this.showAlert('error', error.message || 'Erro ao carregar notas');
                this.loading.set(false);
            }
        });
    }

    carregarProdutos(): void {
        this.produtosService.getAll(1, 100).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    this.produtos.set(response.data);
                }
            },
            error: (error: any) => {
                console.error('Erro ao carregar produtos:', error);
            }
        });
    }

    // ========================================
    // Filtros
    // ========================================

    filtrarNotas(): void {
        let notas = [...this.notas()];

        // Filtro por status
        if (this.filtroStatus !== 'todos') {
            const status = this.filtroStatus === 'aberta'
                ? StatusNotaFiscal.Aberta
                : StatusNotaFiscal.Fechada;
            notas = notas.filter(n => n.status === status);
        }

        // Filtro por termo de busca
        if (this.searchTerm.trim()) {
            const term = this.searchTerm.toLowerCase();
            notas = notas.filter(n =>
                n.numero.toLowerCase().includes(term) ||
                n.id.toString().includes(term) ||
                n.itens.some(item =>
                    item.produto?.descricao.toLowerCase().includes(term) ||
                    item.produto?.codigoSKU.toLowerCase().includes(term)
                )
            );
        }

        this.notasFiltradas.set(notas);
    }

    onFiltroStatusChange(status: 'todos' | 'aberta' | 'fechada'): void {
        this.filtroStatus = status;
        this.filtrarNotas();
    }

    onSearchChange(term: string): void {
        this.searchTerm = term;
        this.filtrarNotas();
    }

    // ========================================
    // Modal - Visualizar/Editar Nota
    // ========================================

    abrirModal(nota: NotaFiscal): void {
        // Carrega a nota completa com todos os itens
        this.loading.set(true);

        this.notasService.getById(nota.id).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    this.notaSelecionada.set(response.data);
                    this.itensEditados.set([...response.data.itens]);
                    this.modoEdicao.set(response.data.status === StatusNotaFiscal.Aberta);
                    this.showModal.set(true);
                } else {
                    this.showAlert('error', 'Erro ao carregar detalhes da nota');
                }
                this.loading.set(false);
            },
            error: (error) => {
                this.showAlert('error', error.message || 'Erro ao carregar nota');
                this.loading.set(false);
            }
        });
    }

    fecharModal(): void {
        this.showModal.set(false);
        this.notaSelecionada.set(null);
        this.itensEditados.set([]);
        this.modoEdicao.set(false);
        this.showAddProduto.set(false);
        this.produtoSelecionado = null;
        this.quantidadeNova = 1;
        this.searchProduto = '';
        this.produtosFiltrados.set([]);
    }

    // ========================================
    // Edição de Itens
    // ========================================

    removerItem(index: number): void {
        if (!confirm('Deseja remover este item?')) {
            return;
        }

        const itens = [...this.itensEditados()];
        itens.splice(index, 1);
        this.itensEditados.set(itens);
        this.showAlert('info', 'Item removido. Salve para confirmar as alterações.');
    }

    atualizarQuantidade(index: number, valor: string): void {
        const novaQuantidade = parseInt(valor, 10);

        if (isNaN(novaQuantidade) || novaQuantidade < 1) {
            this.showAlert('error', 'Quantidade deve ser maior que zero');
            return;
        }

        const item = this.itensEditados()[index];
        const produto = this.produtos().find(p => p.id === item.produtoId);

        if (!produto) {
            this.showAlert('error', 'Produto não encontrado');
            return;
        }

        // Calcula o saldo disponível considerando a quantidade atual do item
        const saldoDisponivel = produto.saldo + item.quantidade;

        if (novaQuantidade > saldoDisponivel) {
            this.showAlert('error', `Saldo insuficiente! Disponível: ${saldoDisponivel}`);
            return;
        }

        const itens = this.itensEditados().map((it, i) =>
            i === index
                ? {
                    ...it,
                    quantidade: novaQuantidade,
                    subtotal: it.precoUnitario * novaQuantidade
                }
                : it
        );

        this.itensEditados.set(itens);
        this.showAlert('success', 'Quantidade atualizada. Salve para confirmar.');
    }

    // ========================================
    // Adicionar Produto
    // ========================================

    mostrarAddProduto(): void {
        this.showAddProduto.set(true);
        this.searchProduto = '';
        this.produtosFiltrados.set([]);

        setTimeout(() => {
            this.searchProdutoInput?.nativeElement?.focus();
        }, 100);
    }

    cancelarAddProduto(): void {
        this.showAddProduto.set(false);
        this.produtoSelecionado = null;
        this.quantidadeNova = 1;
        this.searchProduto = '';
        this.produtosFiltrados.set([]);
    }

    onSearchProdutoChange(): void {
        if (!this.searchProduto.trim()) {
            this.produtosFiltrados.set([]);
            return;
        }

        const term = this.searchProduto.toLowerCase();
        const filtered = this.produtos().filter(p =>
            p.descricao.toLowerCase().includes(term) ||
            p.codigoSKU.toLowerCase().includes(term)
        );

        this.produtosFiltrados.set(filtered);
    }

    selecionarProdutoParaAdicionar(produto: Produto): void {
        this.produtoSelecionado = produto;
        this.quantidadeNova = 1;
        this.produtosFiltrados.set([]);
        this.searchProduto = '';
    }

    adicionarProduto(): void {
        if (!this.produtoSelecionado) {
            this.showAlert('error', 'Selecione um produto');
            return;
        }

        if (this.quantidadeNova < 1) {
            this.showAlert('error', 'Quantidade deve ser maior que zero');
            return;
        }

        if (this.quantidadeNova > this.produtoSelecionado.saldo) {
            this.showAlert('error', `Saldo insuficiente! Disponível: ${this.produtoSelecionado.saldo}`);
            return;
        }

        // Verifica se produto já existe nos itens
        const itemExistente = this.itensEditados().find(
            it => it.produtoId === this.produtoSelecionado!.id
        );

        if (itemExistente) {
            const novaQuantidade = itemExistente.quantidade + this.quantidadeNova;

            if (novaQuantidade > this.produtoSelecionado.saldo) {
                this.showAlert('error', 'Quantidade total excederia o saldo disponível');
                return;
            }

            const itens = this.itensEditados().map(it =>
                it.produtoId === this.produtoSelecionado!.id
                    ? {
                        ...it,
                        quantidade: novaQuantidade,
                        subtotal: it.precoUnitario * novaQuantidade
                    }
                    : it
            );

            this.itensEditados.set(itens);
            this.showAlert('success', 'Quantidade do produto atualizada!');
        } else {
            const novoItem: ItemNotaFiscal = {
                id: 0, // Será gerado pelo backend
                produtoId: this.produtoSelecionado.id,
                produto: this.produtoSelecionado,
                quantidade: this.quantidadeNova,
                precoUnitario: this.produtoSelecionado.preco,
                subtotal: this.produtoSelecionado.preco * this.quantidadeNova
            };

            this.itensEditados.set([...this.itensEditados(), novoItem]);
            this.showAlert('success', 'Produto adicionado! Salve para confirmar.');
        }

        this.cancelarAddProduto();
    }

    // ========================================
    // Salvar/Imprimir Nota
    // ========================================

    calcularTotalEditado(): number {
        return this.itensEditados().reduce((sum, item) => sum + item.subtotal, 0);
    }

    salvarAlteracoes(): void {
        if (this.itensEditados().length === 0) {
            this.showAlert('error', 'A nota deve ter pelo menos um item');
            return;
        }

        const nota = this.notaSelecionada();
        if (!nota) return;

        if (!confirm('Deseja salvar as alterações?')) {
            return;
        }

        this.loading.set(true);

        // Prepara os dados para atualização
        const updateData = {
            itens: this.itensEditados().map(item => ({
                produtoId: item.produtoId,
                quantidade: item.quantidade
            }))
        };

        this.notasService.update(nota.id, updateData).subscribe({
            next: (response) => {
                if (response.isSuccess) {
                    this.showAlert('success', 'Alterações salvas com sucesso!');
                    this.fecharModal();
                    this.carregarNotas();
                    this.carregarProdutos();
                } else {
                    this.showAlert('error', response.message || 'Erro ao salvar alterações');
                }
                this.loading.set(false);
            },
            error: (error) => {
                this.showAlert('error', error.message || 'Erro ao salvar alterações');
                this.loading.set(false);
            }
        });
    }

    imprimirNota(): void {
        const nota = this.notaSelecionada();

        if (!nota) return;

        if (nota.status === StatusNotaFiscal.Fechada) {
            this.showAlert('info', 'Esta nota já foi impressa e finalizada');
            return;
        }

        if (this.itensEditados().length === 0) {
            this.showAlert('error', 'A nota deve ter pelo menos um item');
            return;
        }

        // Se há alterações não salvas, avisar o usuário
        const notaOriginal = this.notaSelecionada();
        const temAlteracoes = JSON.stringify(this.itensEditados()) !== JSON.stringify(notaOriginal?.itens);

        if (temAlteracoes) {
            this.showAlert('info', 'Há alterações não salvas. Salve antes de imprimir.');
            return;
        }

        if (!confirm('Confirma a impressão da nota? Esta ação irá baixar o estoque e fechar a nota.')) {
            return;
        }

        this.loading.set(true);

        this.notasService.imprimir(nota.id).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data?.sucesso) {
                    this.showAlert('success', '✅ Nota impressa e finalizada com sucesso!');
                    this.fecharModal();
                    this.carregarNotas();
                    this.carregarProdutos();
                } else {
                    this.showAlert('error', response.data?.mensagem || response.message || 'Erro ao imprimir nota');
                }
                this.loading.set(false);
            },
            error: (error) => {
                this.showAlert('error', error.message || 'Erro ao imprimir nota');
                this.loading.set(false);
            }
        });
    }

    // ========================================
    // Navegação
    // ========================================

    voltarParaPDV(nota: NotaFiscal): void {
        if (nota.status === StatusNotaFiscal.Fechada) {
            this.showAlert('error', 'Não é possível editar uma nota fechada');
            return;
        }

        // Redireciona para o PDV com os dados da nota
        this.router.navigate(['/pdv'], {
            state: { notaParaEditar: nota }
        });
    }

    excluirNota(nota: NotaFiscal): void {
        if (nota.status === StatusNotaFiscal.Fechada) {
            this.showAlert('error', 'Não é possível excluir uma nota fechada (já impressa)');
            return;
        }

        if (!confirm(`Tem certeza que deseja excluir a nota ${nota.numero}?\n\nEsta ação não pode ser desfeita.`)) {
            return;
        }

        this.loading.set(true);

        this.notasService.delete(nota.id).subscribe({
            next: (response) => {
                if (response.isSuccess) {
                    this.showAlert('success', 'Nota excluída com sucesso');
                    this.carregarNotas();
                } else {
                    this.showAlert('error', response.message || 'Erro ao excluir nota');
                }
                this.loading.set(false);
            },
            error: (error) => {
                this.showAlert('error', error.message || 'Erro ao excluir nota');
                this.loading.set(false);
            }
        });
    }

    // ========================================
    // Helpers
    // ========================================

    showAlert(type: AlertMessage['type'], message: string): void {
        this.alert.set({ type, message });
        setTimeout(() => this.alert.set(null), 5000);
    }

    closeAlert(): void {
        this.alert.set(null);
    }

    getStatusClass(status: StatusNotaFiscal): string {
        return status === StatusNotaFiscal.Aberta
            ? 'status-aberta'
            : 'status-fechada';
    }

    formatarData(data: Date | string): string {
        return new Date(data).toLocaleDateString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    }

    formatarMoeda(valor: number): string {
        return valor.toLocaleString('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        });
    }

    // Getters para estatísticas
    get totalNotas(): number {
        return this.notasFiltradas().length;
    }

    get notasAbertas(): number {
        return this.notasFiltradas().filter(n => n.status === StatusNotaFiscal.Aberta).length;
    }

    get notasFechadas(): number {
        return this.notasFiltradas().filter(n => n.status === StatusNotaFiscal.Fechada).length;
    }

    get valorTotalNotas(): number {
        return this.notasFiltradas().reduce((sum, n) => sum + n.valorTotal, 0);
    }
}