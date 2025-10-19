import { Produto } from "./produto.model";

export interface NotaFiscal {
    id: number;
    numeroNota: string;
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

export interface NotaFiscalCreateRequest {
    numeroNota: string;
    itens: ItemNotaFiscalCreateRequest[];
}

export interface ItemNotaFiscalCreateRequest {
    produtoId: number;
    quantidade: number;
    precoUnitario: number;
}

export interface NotaFiscalImpressaoRequest {
    notaFiscalId: number;
}

export interface NotaFiscalImpressaoResponse {
    sucesso: boolean;
    mensagem: string;
    detalhes?: BaixaEstoqueDetalhe[];
}

export interface BaixaEstoqueDetalhe {
    produtoId: number;
    codigoSKU: string;
    quantidade: number;
    sucesso: boolean;
    mensagem: string;
}