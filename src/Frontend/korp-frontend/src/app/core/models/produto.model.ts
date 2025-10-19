export interface Produto {
    id: number;
    codigoSKU: string;
    descricao: string;
    preco: number;
    saldo: number;
    dataCadastro?: Date;
}

export interface ProdutoCreateRequest {
    descricao: string;
    codigoSKU: string;
    preco: number;
    saldo: number;
}

export interface ProdutoUpdateRequest {
    descricao: string;
    // codigoSKU: string;
    preco: number;
    saldo: number;
}