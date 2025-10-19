export interface MovimentacaoBatchRequest {
    itens: ItemBaixaEstoque[];
    observacao?: string;
}

export interface ItemBaixaEstoque {
    produtoId: number;
    quantidade: number;
}

export enum TipoMovimentacaoEstoque {
    Entrada = 'Entrada',
    Saida = 'Saida'
}

export interface MovimentacaoEstoqueRequest {
    quantidade: number;
    tipo: TipoMovimentacaoEstoque;
    observacao?: string;
}