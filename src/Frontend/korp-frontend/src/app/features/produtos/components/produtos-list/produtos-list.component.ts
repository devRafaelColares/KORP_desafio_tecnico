import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutosService } from '../../../../core/services/produtos.service';
import { Produto, ProdutoCreateRequest } from '../../../../core/models/produto.model';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { InputComponent } from '../../../../shared/components/input/input.component';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { AlertComponent } from '../../../../shared/components/alert/alert.component';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header/page-header.component';

interface AlertMessage {
    type: 'success' | 'error' | 'info';
    message: string;
}

@Component({
    selector: 'app-produtos-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ButtonComponent,
        InputComponent,
        ModalComponent,
        AlertComponent,
        CardComponent,
        LoadingComponent,
        PageHeaderComponent
    ],
    templateUrl: './produtos-list.component.html',
    styleUrls: ['./produtos-list.component.scss']
})
export class ProdutosListComponent implements OnInit {
    produtos = signal<Produto[]>([]);
    filteredProdutos = signal<Produto[]>([]);
    searchTerm = signal('');
    isLoading = signal(false);
    isModalOpen = signal(false);
    isSaving = signal(false);
    alert = signal<AlertMessage | null>(null);

    editingProduto: Produto | null = null;

    formData = {
        codigoSKU: '',
        descricao: '',
        preco: 0,
        saldo: 0
    };

    formErrors: { [key: string]: string } = {};

    // Estatísticas
    get totalProdutos(): number {
        return this.produtos().length;
    }

    get produtosEmEstoque(): number {
        return this.produtos().filter(p => p.saldo > 0).length;
    }

    get produtosSemEstoque(): number {
        return this.produtos().filter(p => p.saldo === 0).length;
    }

    constructor(private produtosService: ProdutosService) { }

    ngOnInit(): void {
        this.loadProdutos();
    }

    loadProdutos(): void {
        this.isLoading.set(true);

        this.produtosService.getAll(1, 100).subscribe({
            next: (response) => {
                if (response.isSuccess && response.data) {
                    this.produtos.set(response.data);
                    this.filterProdutos();
                }
                this.isLoading.set(false);
            },
            error: (error) => {
                this.showAlert('error', error.message || 'Erro ao carregar produtos');
                this.isLoading.set(false);
            }
        });
    }

    onSearchChange(term: string): void {
        this.searchTerm.set(term);
        this.filterProdutos();
    }

    filterProdutos(): void {
        const term = this.searchTerm().toLowerCase();

        if (!term) {
            this.filteredProdutos.set(this.produtos());
            return;
        }

        const filtered = this.produtos().filter(p =>
            p.descricao.toLowerCase().includes(term) ||
            p.codigoSKU.toLowerCase().includes(term)
        );

        this.filteredProdutos.set(filtered);
    }

    openModal(produto?: Produto): void {
        if (produto) {
            this.editingProduto = produto;
            this.formData = {
                codigoSKU: produto.codigoSKU,
                descricao: produto.descricao,
                preco: produto.preco,
                saldo: produto.saldo
            };
        } else {
            this.editingProduto = null;
            this.resetForm();
        }

        this.formErrors = {};
        this.isModalOpen.set(true);
    }

    closeModal(): void {
        this.isModalOpen.set(false);
        this.editingProduto = null;
        this.resetForm();
        this.formErrors = {};
    }

    resetForm(): void {
        this.formData = {
            codigoSKU: '',
            descricao: '',
            preco: 0,
            saldo: 0
        };
    }

    validateForm(): boolean {
        this.formErrors = {};
        let isValid = true;

        if (!this.formData.codigoSKU.trim()) {
            this.formErrors['codigoSKU'] = 'Código SKU é obrigatório';
            isValid = false;
        }

        if (!this.formData.descricao.trim()) {
            this.formErrors['descricao'] = 'Descrição é obrigatória';
            isValid = false;
        }

        if (this.formData.preco <= 0) {
            this.formErrors['preco'] = 'Preço deve ser maior que zero';
            isValid = false;
        }

        if (this.formData.saldo < 0) {
            this.formErrors['saldo'] = 'Saldo não pode ser negativo';
            isValid = false;
        }

        // Validar SKU duplicado apenas na criação
        if (!this.editingProduto) {
            const skuExists = this.produtos().some(
                p => p.codigoSKU.toLowerCase() === this.formData.codigoSKU.trim().toLowerCase()
            );

            if (skuExists) {
                this.formErrors['codigoSKU'] = 'Já existe um produto com este SKU';
                isValid = false;
            }
        }

        return isValid;
    }

    saveProduto(): void {
        if (!this.validateForm()) {
            return;
        }

        this.isSaving.set(true);

        const request: ProdutoCreateRequest = {
            codigoSKU: this.formData.codigoSKU.trim(),
            descricao: this.formData.descricao.trim(),
            preco: this.formData.preco,
            saldo: this.formData.saldo
        };

        if (this.editingProduto) {
            // Atualizar (implementar quando endpoint estiver pronto)
            this.showAlert('info', 'Funcionalidade de edição em desenvolvimento');
            this.isSaving.set(false);
            this.closeModal();
        } else {
            // Criar
            this.produtosService.create(request).subscribe({
                next: (response) => {
                    if (response.isSuccess) {
                        this.showAlert('success', 'Produto cadastrado com sucesso!');
                        this.loadProdutos();
                        this.closeModal();
                    } else {
                        this.showAlert('error', response.message || 'Erro ao cadastrar produto');
                    }
                    this.isSaving.set(false);
                },
                error: (error) => {
                    this.showAlert('error', error.message || 'Erro ao cadastrar produto');
                    this.isSaving.set(false);
                }
            });
        }
    }

    deleteProduto(produto: Produto): void {
        if (!confirm(`Tem certeza que deseja excluir o produto "${produto.descricao}"?`)) {
            return;
        }

        this.produtosService.delete(produto.id).subscribe({
            next: (response) => {
                if (response.isSuccess) {
                    this.showAlert('success', 'Produto excluído com sucesso!');
                    this.loadProdutos();
                } else {
                    this.showAlert('error', response.message || 'Erro ao excluir produto');
                }
            },
            error: (error) => {
                this.showAlert('error', error.message || 'Erro ao excluir produto');
            }
        });
    }

    showAlert(type: 'success' | 'error' | 'info', message: string): void {
        this.alert.set({ type, message });

        // Auto-fechar após 5 segundos
        setTimeout(() => {
            this.alert.set(null);
        }, 5000);
    }

    closeAlert(): void {
        this.alert.set(null);
    }

    getSaldoClass(saldo: number): string {
        if (saldo === 0) return 'badge-danger';
        if (saldo < 10) return 'badge-warning';
        return 'badge-success';
    }
}