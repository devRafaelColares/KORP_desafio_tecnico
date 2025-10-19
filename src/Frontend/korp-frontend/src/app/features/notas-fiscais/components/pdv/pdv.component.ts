import { Component, OnInit, signal, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Produto } from '../../../../core/models/produto.model';
import { InputComponent } from '../../../../shared/components/input/input.component';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { AlertComponent } from '../../../../shared/components/alert/alert.component';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { ItemNotaFiscal, NotaFiscal, NotaFiscalCreateRequest, StatusNotaFiscal } from '../../../../core/models/nota-fiscal.model';
import { ProdutosService } from '../../../../core/services/produtos.service';
import { NotasFiscaisService } from '../../../../core/services/notas-fiscais.service';
import { HttpErrorHandlerService } from '../../../../core/services/http-error-handler.service';

@Component({
    selector: 'app-pdv',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        AlertComponent,
        LoadingComponent
    ],
    templateUrl: './pdv.component.html',
    styleUrls: ['./pdv.component.scss']
})
export class PdvComponent implements OnInit {
    // ViewChild para focar inputs
    @ViewChild('searchInput') searchInput?: ElementRef<HTMLInputElement>;
    @ViewChild('quantidadeInput') quantidadeInput?: ElementRef<HTMLInputElement>;

    // Estados principais
    produtos = signal<Produto[]>([]);
    produtosFiltrados = signal<Produto[]>([]);
    searchTerm = '';  // Variável normal para ngModel
    searchMode: 'sku' | 'descricao' | null = null;
    loading = signal(false);

    // Produto selecionado para adicionar à nota
    produtoSelecionado: Produto | null = null;
    quantidade = 1;  // Variável normal para ngModel

    // Itens da nota fiscal atual
    itensNota = signal<ItemNotaFiscal[]>([]);

    // Nota fiscal atual
    notaAtual = signal<NotaFiscal | null>(null);

    // Alertas e modais
    alert = signal<{ type: 'success' | 'error' | 'info'; message: string } | null>(null);
    showShortcuts = signal(false);

    constructor(
        private produtosService: ProdutosService,
        private notasFiscaisService: NotasFiscaisService,
        private httpErrorHandler: HttpErrorHandlerService
    ) { }

    ngOnInit(): void {
        this.carregarProdutos();
        this.iniciarNovaNota();
    }

    // Listener para atalhos do teclado
    @HostListener('window:keydown', ['$event'])
    handleKeyboardEvent(event: KeyboardEvent): void {
        // Evita conflito com inputs
        const target = event.target as HTMLElement;
        const isInput = target.tagName === 'INPUT' || target.tagName === 'TEXTAREA';

        // F1 - Buscar por SKU
        if (event.key === 'F1') {
            event.preventDefault();
            this.iniciarBusca('sku');
        }
        // F2 - Buscar por Descrição
        else if (event.key === 'F2') {
            event.preventDefault();
            this.iniciarBusca('descricao');
        }
        // F3 - Adicionar item
        else if (event.key === 'F3') {
            event.preventDefault();
            if (this.produtoSelecionado) {
                this.adicionarItem();
            }
        }
        // F4 - Remover último item
        else if (event.key === 'F4') {
            event.preventDefault();
            this.removerUltimoItem();
        }
        // F5 - Limpar nota
        else if (event.key === 'F5') {
            event.preventDefault();
            this.limparNota();
        }
        // F9 - Imprimir nota
        else if (event.key === 'F9') {
            event.preventDefault();
            this.imprimirNota();
        }
        // F10 - Nova nota
        else if (event.key === 'F10') {
            event.preventDefault();
            this.iniciarNovaNota();
        }
        // F12 - Mostrar atalhos
        else if (event.key === 'F12') {
            event.preventDefault();
            this.showShortcuts.set(true);
        }
        // ESC - Cancelar ação
        else if (event.key === 'Escape') {
            event.preventDefault();
            if (this.showShortcuts()) {
                this.showShortcuts.set(false);
            } else if (this.searchMode) {
                this.searchMode = null;
                this.searchTerm = '';
                this.produtosFiltrados.set([]);
                this.produtoSelecionado = null;
            }
        }
        // +/- para ajustar quantidade (quando input de quantidade está focado)
        else if (isInput && target === this.quantidadeInput?.nativeElement) {
            if (event.key === '+' || event.key === '=') {
                event.preventDefault();
                this.quantidade++;
            } else if (event.key === '-' && this.quantidade > 1) {
                event.preventDefault();
                this.quantidade--;
            }
        }
    }

    // Busca e filtro de produtos
    carregarProdutos(): void {
        this.loading.set(true);
        this.produtosService.getAll(1, 100).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    this.produtos.set(response.data);
                    this.filtrarProdutos();
                } else {
                    this.showAlert('error', response.message || 'Erro ao carregar produtos');
                }
                this.loading.set(false);
            },
            error: (error) => {
                let errorMessage = 'Erro ao processar nota fiscal';
                if (error instanceof Error) {
                    errorMessage = error.message;
                } else if (error && error.message) {
                    errorMessage = error.message;
                }
                this.showAlert('error', errorMessage);
                this.loading.set(false);
            }
        });
    }

    iniciarBusca(modo: 'sku' | 'descricao'): void {
        this.searchMode = modo;
        this.searchTerm = '';
        this.produtosFiltrados.set([]);
        
        // Foca no input de busca após um pequeno delay
        setTimeout(() => {
            this.searchInput?.nativeElement?.focus();
        }, 100);
    }

    onSearchChange(): void {
        this.filtrarProdutos();
    }

    filtrarProdutos(): void {
        const term = this.searchTerm.toLowerCase().trim();
        
        if (!term || !this.searchMode) {
            this.produtosFiltrados.set([]);
            return;
        }

        const filtered = this.produtos().filter(p => {
            if (this.searchMode === 'sku') {
                return p.codigoSKU.toLowerCase().includes(term);
            } else {
                return p.descricao.toLowerCase().includes(term);
            }
        });

        this.produtosFiltrados.set(filtered);
    }

    selecionarProduto(produto: Produto): void {
        this.produtoSelecionado = produto;
        this.quantidade = 1;
        this.searchTerm = '';
        this.produtosFiltrados.set([]);
        
        // Foca no input de quantidade
        setTimeout(() => {
            this.quantidadeInput?.nativeElement?.focus();
        }, 100);
    }

    adicionarItem(): void {
        if (!this.produtoSelecionado) {
            this.showAlert('error', 'Selecione um produto primeiro!');
            return;
        }

        const produto = this.produtoSelecionado;
        const quantidade = this.quantidade;

        // Validações
        if (quantidade < 1) {
            this.showAlert('error', 'Quantidade deve ser maior que zero!');
            return;
        }

        if (quantidade > produto.saldo) {
            this.showAlert('error', `Quantidade maior que o saldo disponível! (${produto.saldo} un)`);
            return;
        }

        // Verifica se o produto já está na nota
        const itemExistente = this.itensNota().find(item => item.produtoId === produto.id);
        
        if (itemExistente) {
            const novaQuantidade = itemExistente.quantidade + quantidade;
            
            if (novaQuantidade > produto.saldo) {
                this.showAlert('error', `Quantidade total excederia o saldo disponível! (${produto.saldo} un)`);
                return;
            }

            // Atualiza item existente
            const itensAtualizados = this.itensNota().map(item => 
                item.produtoId === produto.id
                    ? { 
                        ...item, 
                        quantidade: novaQuantidade,
                        subtotal: produto.preco * novaQuantidade
                    }
                    : item
            );
            this.itensNota.set(itensAtualizados);
        } else {
            // Adiciona novo item
            const novoItem: ItemNotaFiscal = {
                id: Date.now(),
                produtoId: produto.id,
                produto,
                quantidade,
                precoUnitario: produto.preco,
                subtotal: produto.preco * quantidade
            };
            this.itensNota.set([...this.itensNota(), novoItem]);
        }

        // Limpa seleção
        this.produtoSelecionado = null;
        this.quantidade = 1;
        this.searchMode = null;
        
        this.showAlert('success', 'Item adicionado com sucesso!');
    }

    removerItem(index: number): void {
        const itens = [...this.itensNota()];
        itens.splice(index, 1);
        this.itensNota.set(itens);
        this.showAlert('info', 'Item removido da nota');
    }

    removerUltimoItem(): void {
        if (this.itensNota().length === 0) {
            this.showAlert('info', 'Não há itens para remover');
            return;
        }

        const itens = [...this.itensNota()];
        itens.pop();
        this.itensNota.set(itens);
        this.showAlert('info', 'Último item removido');
    }

    limparNota(): void {
        if (this.itensNota().length === 0) {
            this.showAlert('info', 'A nota já está vazia');
            return;
        }

        this.itensNota.set([]);
        this.produtoSelecionado = null;
        this.quantidade = 1;
        this.searchMode = null;
        this.searchTerm = '';
        this.showAlert('info', 'Nota limpa com sucesso');
    }

    iniciarNovaNota(): void {
        const numeroNota = `NF-${Date.now()}`;
        
        this.notaAtual.set({
            id: 0,
            numero: numeroNota,
            status: StatusNotaFiscal.Aberta,
            dataEmissao: new Date(),
            dataCriacao: new Date(),
            itens: [],
            valorTotal: 0
        });

        this.itensNota.set([]);
        this.produtoSelecionado = null;
        this.quantidade = 1;
        this.searchMode = null;
        this.searchTerm = '';
        
        this.showAlert('success', `Nova nota iniciada: ${numeroNota}`);
    }

    calcularTotal(): number {
        return this.itensNota().reduce((acc, item) => acc + item.subtotal, 0);
    }

    imprimirNota(): void {
        if (this.itensNota().length === 0) {
            this.showAlert('error', 'Adicione itens à nota antes de imprimir.');
            return;
        }

        if (this.loading()) {
            return;
        }

        this.loading.set(true);

        // Monta o request para criação da nota fiscal
        const notaRequest: NotaFiscalCreateRequest = {
            numero: this.notaAtual()?.numero || `NF-${Date.now()}`,
            itens: this.itensNota().map(item => ({
                produtoId: item.produtoId,
                quantidade: item.quantidade
            }))
        };

        this.notasFiscaisService.create(notaRequest).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    this.showAlert('success', `Nota fiscal ${response.data.numero} processada com sucesso!`);
                    
                    // Atualiza a nota atual com os dados retornados
                    this.notaAtual.set({
                        ...response.data,
                        status: StatusNotaFiscal.Fechada
                    });

                    // Limpa a nota e recarrega produtos
                    setTimeout(() => {
                        this.iniciarNovaNota();
                        this.carregarProdutos(); // Atualiza saldo dos produtos
                    }, 2000);
                } else {
                    this.showAlert('error', response.message || 'Erro ao processar nota fiscal');
                }
                this.loading.set(false);
            },
            error: (error) => {
                let errorMessage = 'Erro ao processar nota fiscal';
                if (error instanceof Error) {
                    errorMessage = error.message;
                } else if (error && error.message) {
                    errorMessage = error.message;
                }
                this.showAlert('error', errorMessage);
                this.loading.set(false);
            }
        });
    }

    // Alertas
    showAlert(type: 'success' | 'error' | 'info', message: string): void {
        this.alert.set({ type, message });
        setTimeout(() => this.alert.set(null), 5000);
    }

    closeAlert(): void {
        this.alert.set(null);
    }
}