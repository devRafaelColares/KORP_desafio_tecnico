import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Produto } from '../../../core/models/produto.model';
import { NotaFiscal, ItemNotaFiscal, StatusNotaFiscal, NotaFiscalCreateRequest, ItemNotaFiscalCreateRequest } from '../../../core/models/nota-fiscal.model';

import { ProdutosService } from '../../../core/services/produtos.service';
import { NotasFiscaisService } from '../../../core/services/notas-fiscais.service';
import { HttpErrorHandlerService } from '../../../core/services/http-error-handler.service';

import { InputComponent } from '../../../shared/components/input/input.component';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { AlertComponent } from '../../../shared/components/alert/alert.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';

@Component({
    selector: 'app-pdv',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        InputComponent,
        ModalComponent,
        AlertComponent,
        CardComponent,
        LoadingComponent
    ],
    templateUrl: './pdv.component.html',
    styleUrls: ['./pdv.component.scss']
})
export class PdvComponent implements OnInit {
    // Estados principais
    produtos = signal<Produto[]>([]);
    produtosFiltrados = signal<Produto[]>([]);
    searchTerm = signal('');
    searchMode: 'sku' | 'descricao' | null = 'sku';
    loading = signal(false);

    // Produto selecionado para adicionar à nota
    produtoSelecionado: Produto | null = null;
    quantidadeSelecionada = signal(1);

    // Itens da nota fiscal atual
    itensNota = signal<ItemNotaFiscal[]>([]);

    // Nota fiscal atual
    notaFiscal = signal<NotaFiscal | null>(null);

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
                const msg = this.httpErrorHandler.handleError(error);
                this.showAlert('error', msg.message || 'Erro ao carregar produtos');
                this.loading.set(false);
            }
        });
    }

    iniciarBusca(modo: 'sku' | 'descricao'): void {
        this.searchMode = modo;
        this.searchTerm.set('');
        this.filtrarProdutos();
    }

    onSearchChange(): void {
        this.filtrarProdutos();
    }

    filtrarProdutos(): void {
        const term = this.searchTerm().toLowerCase();
        if (!term) {
            this.produtosFiltrados.set([]);
            return;
        }
        const filtered = this.produtos().filter(p =>
            this.searchMode === 'sku'
                ? p.codigoSKU.toLowerCase().includes(term)
                : p.descricao.toLowerCase().includes(term)
        );
        this.produtosFiltrados.set(filtered);
    }

    selecionarProduto(produto: Produto): void {
        this.produtoSelecionado = produto;
        this.quantidadeSelecionada.set(1);
    }

    alterarQuantidade(qtd: number): void {
        if (qtd < 1) qtd = 1;
        this.quantidadeSelecionada.set(qtd);
    }

    adicionarItemNota(): void {
        if (!this.produtoSelecionado) return;
        const produto = this.produtoSelecionado;
        const quantidade = this.quantidadeSelecionada();
        if (quantidade > produto.saldo) {
            this.showAlert('error', 'Quantidade maior que o saldo disponível!');
            return;
        }
        const item: ItemNotaFiscal = {
            id: Date.now(),
            produtoId: produto.id,
            produto,
            quantidade,
            precoUnitario: produto.preco,
            subtotal: produto.preco * quantidade
        };
        this.itensNota.set([...this.itensNota(), item]);
        this.produtoSelecionado = null;
        this.quantidadeSelecionada.set(1);
    }

    removerUltimoItem(): void {
        const itens = [...this.itensNota()];
        itens.pop();
        this.itensNota.set(itens);
    }

    limparNota(): void {
        this.itensNota.set([]);
        this.produtoSelecionado = null;
        this.quantidadeSelecionada.set(1);
    }

    iniciarNovaNota(): void {
        this.notaFiscal.set({
            id: Date.now(),
            numero: `NF-${Date.now()}`,
            status: StatusNotaFiscal.Aberta,
            dataEmissao: new Date(),
            dataCriacao: new Date(),
            itens: [],
            valorTotal: 0
        });
        this.limparNota();
    }

    calcularTotal(): number {
        return this.itensNota().reduce((acc, item) => acc + item.subtotal, 0);
    }

    imprimirNota(): void {
        if (this.itensNota().length === 0) {
            this.showAlert('error', 'Adicione itens à nota antes de imprimir.');
            return;
        }

        this.loading.set(true);

        // Monta o request para criação da nota fiscal
        const notaRequest: NotaFiscalCreateRequest = {
            numero: this.notaFiscal()?.numero || '',
            itens: this.itensNota().map(item => ({
                produtoId: item.produtoId,
                quantidade: item.quantidade
            }))
        };

        this.notasFiscaisService.create(notaRequest).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    this.showAlert('success', 'Nota fiscal processada com sucesso!');
                    this.notaFiscal.set({
                        ...response.data,
                        status: StatusNotaFiscal.Fechada
                    });
                    this.limparNota();
                    this.carregarProdutos(); // Atualiza saldo dos produtos
                } else {
                    this.showAlert('error', response.message || 'Erro ao processar nota fiscal');
                }
                this.loading.set(false);
            },
            error: (error) => {
                const msg = this.httpErrorHandler.handleError(error);
                this.showAlert('error', msg.message || 'Erro ao processar nota fiscal');
                this.loading.set(false);
            }
        });
    }

    // Alertas
    showAlert(type: 'success' | 'error' | 'info', message: string): void {
        this.alert.set({ type, message });
        setTimeout(() => this.alert.set(null), 4000);
    }

    closeAlert(): void {
        this.alert.set(null);
    }
}