import { Produto } from './produto.model';

export interface NotaFiscal {
    id: number;
    numero: string;
    status: StatusNotaFiscal;
    dataEmissao: Date;
    dataCriacao: Date;
    itens: ItemNotaFiscal[];
    valorTotal: number;
}

export enum StatusNotaFiscal {
    Aberta = 'Aberta',
    Fechada = 'Fechada'
}

export interface ItemNotaFiscal {
    id: number;
    produtoId: number;
    produto?: Produto;
    quantidade: number;
    precoUnitario: number;
    subtotal: number;
}

// Requests
export interface NotaFiscalCreateRequest {
    numero: string;
    itens: ItemNotaFiscalCreateRequest[];
}

export interface ItemNotaFiscalCreateRequest {
    produtoId: number;
    quantidade: number;
}

export interface ImprimirNotaFiscalRequest {
    usuarioResponsavel?: string;
}

// Responses
export interface ImprimirNotaFiscalResponse {
    sucesso: boolean;
    mensagem?: string;
    notaFiscal?: NotaFiscal;
    resultadosBaixaEstoque?: BaixaProdutoResultado[];
}

export interface BaixaProdutoResultado {
    produtoId: number;
    codigoSKU: string;
    quantidade: number;
    sucesso: boolean;
    mensagem: string;
}