export interface Response<T> {
    data: T | null;
    code: number;
    message?: string;
    isSuccess: boolean;
}

export interface PagedResponse<T> extends Response<T> {
    totalCount: number;
    pageSize: number;
    currentPage: number;
    totalPages: number;
}

export interface ApiError {
    message: string;
    errors?: { [key: string]: string[] };
    statusCode: number;
}