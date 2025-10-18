# 🧾 Sistema KORP — Desafio Técnico

## 1. Visão Geral e Propósito
O **Sistema KORP** é uma solução corporativa para **gestão de notas fiscais e controle de estoque**, desenvolvida com **arquitetura de microsserviços**.  
O projeto tem como objetivo demonstrar **engenharia de software moderna**, com foco em **escalabilidade, resiliência, governança de design e experiência do usuário**.

A aplicação aplica princípios sólidos de desenvolvimento:
- **SOLID**, **DRY** e **Clean Architecture**
- **Design System** e **Acessibilidade (WCAG)**
- **Segurança (OWASP Top 10)**
- **Resiliência e Idempotência**

---

## 2. Arquitetura de Software

### 🧩 2.1 Macroestrutura
A solução está dividida em três camadas principais:

```
src/
  Backend/
    EstoqueService/
    FaturamentoService/
  Frontend/
    (Angular — a ser implementado)
```

- **Backend:** dois microsserviços independentes:
  - `EstoqueService`: responsável por produtos, movimentações e controle de saldo.
  - `FaturamentoService`: responsável por notas fiscais e integração com estoque.
- **Frontend:** aplicação Angular (Sprint 2), com interface responsiva, acessível e governada por Design System.
- **Infraestrutura:** SQL Server, Docker, Entity Framework Core e Swagger/OpenAPI.

---

### 🧱 2.2 Microsserviço EstoqueService

#### Responsabilidades
- Gerenciar produtos (CRUD)
- Controlar movimentações de estoque (entradas e saídas)
- Processar baixas de estoque em lote (integração com Faturamento)

#### Componentes Principais
- **Models:** `Produto`, `MovimentacaoEstoque`
- **Requests/Responses:** contratos padronizados de entrada e saída
- **Services:** regras de negócio (ProdutoService, MovimentacaoEstoqueService)
- **Repositories:** abstração do acesso ao banco de dados
- **Endpoints:** rotas RESTful
- **Configurations:** variáveis e constantes globais

#### Fluxo de Baixa de Estoque
1. Recebe requisição de baixa em lote.  
2. Valida o saldo dos produtos.  
3. Atualiza saldos e registra movimentações.  
4. Retorna resultado detalhado de sucesso ou falha por item.  

---

### 🧾 2.3 Microsserviço FaturamentoService

#### Responsabilidades
- Gerenciar notas fiscais (criação, consulta e impressão).
- Integrar com EstoqueService para processar baixas.
- Garantir consistência entre nota e estoque.

#### Componentes Principais
- **Models:** `NotaFiscal`, `ItemNotaFiscal`
- **Requests/Responses:** contratos para criação e impressão
- **Services:** lógica de negócio (`NotaFiscalService`)
- **Repositories:** persistência e consultas
- **Clients:** comunicação resiliente com EstoqueService (via Polly)
- **Endpoints:** CRUD e impressão de notas fiscais

#### Fluxo de Impressão de Nota Fiscal
1. Valida se a nota está “Aberta”.  
2. Monta requisição de baixa em lote para o EstoqueService.  
3. Caso sucesso → altera status da nota para “Fechada”.  
4. Retorna resposta detalhada ao usuário.  

---

## 3. Fluxos de Negócio

### 📦 3.1 Cadastro e Consulta de Produtos
- Complexidade: O(1) para operações CRUD e O(log n) para paginação.  
- Validação de SKU único.  
- Retornos padronizados e contratos claros.

### 🔄 3.2 Movimentação de Estoque
- Processamento em lote com atomicidade.  
- Resposta detalhada por item.  
- Logs estruturados sem dados sensíveis.

### 🧾 3.3 Emissão e Impressão de Nota Fiscal
- Criação de nota “Aberta”.  
- Impressão com verificação de saldo no EstoqueService.  
- Alteração automática do status para “Fechada” após sucesso.  
- Idempotência para evitar duplicidade.

---

## 4. Engenharia de Software

### 4.1 Princípios Aplicados
- **SRP:** cada classe/módulo tem responsabilidade única.  
- **OCP:** fácil extensão de serviços e endpoints.  
- **LSP / ISP:** contratos enxutos e substituíveis.  
- **DIP:** injeção de dependências em todos os serviços.  
- **DRY:** lógica e contratos reutilizáveis.  

### 4.2 Padrão de Código
- Verbos no infinitivo, português, PascalCase.  
- Métodos assíncronos com sufixo `Async`.  
- Uso de `#region` para organização.  
- Camadas: Domain, Application, Infrastructure, Presentation.  

---

## 5. Performance e Eficiência
- Consultas otimizadas (paginação, índices, lazy loading).  
- Operações críticas com complexidade O(1) ou O(log n).  
- Microsserviços desacoplados e escaláveis horizontalmente.  

---

## 6. Segurança
- Conformidade com **OWASP Top 10**.  
- Validação de entrada e tratamento de erros seguro.  
- Logs sem dados sensíveis.  
- Política **CORS restritiva**.  
- Pronto para integração com **JWT/OAuth2**.  
- Transações atômicas em operações críticas.  

---

## 7. Governança Visual e UX/UI

### 🎨 Design System
- **Tokens:** cores, tipografia, espaçamentos e sombras centralizados.  
- **Componentização atômica:** UI reutilizável e testável.  
- **Multi-tema:** suporte nativo a light/dark mode.  
- **Acessibilidade:** contraste, navegação por teclado e semântica HTML.  

Ferramentas sugeridas: **TailwindCSS**, **Shadcn UI**, **Storybook**, **Figma**.

---

## 8. Integração e Comunicação
- Comunicação via **HTTP RESTful** entre microsserviços.  
- **Polly** para retry e circuit breaker.  
- Contratos versionados e validados.  
- Documentação via **Swagger/OpenAPI**.  

---

## 9. Testabilidade e Manutenção
- Serviços e repositórios desacoplados.  
- Contratos bem definidos (Request/Response).  
- Fácil mock de dependências.  
- Estrutura modular, ideal para testes unitários e de integração.  

---

## 10. Setup, Execução e Deploy

### Pré-requisitos
- .NET 9.0 SDK  
- Node.js 18+  
- Angular CLI  
- Docker e Docker Compose  
- SQL Server

### Execução local

```bash
# Backend
cd src/Backend/EstoqueService
dotnet run

cd ../FaturamentoService
dotnet run

# Frontend (a ser implementado)
cd src/Frontend
npm install
ng serve
```

### Docker (opcional)
```bash
docker-compose up --build
```

### APIs disponíveis
- EstoqueService → http://localhost:5001/swagger  
- FaturamentoService → http://localhost:5002/swagger  

---

## 11. Diferenciais Técnicos
- Resiliência com retry/circuit breaker.  
- Idempotência em operações críticas.  
- Governança visual completa com Design System.  
- Performance em O(1)/O(log n).  
- Conformidade com OWASP Top 10.  

---

## 12. Referências
- Material Design  
- Fluent UI  
- WCAG Guidelines  
- OWASP Top 10  

---

## 13. Próximos Passos
- Implementar frontend Angular consumindo os endpoints REST.  
- Ampliar o Design System e documentar componentes.  
- Adicionar autenticação/autorização.  
- Implementar monitoramento e observabilidade (Application Insights, Prometheus).  

---

## 📞 Contato e Contribuição
**Autor:** Rafael Colares  
Desenvolvedor Full Stack • .NET • Angular • SQL Server  
📧 [rafaelcolares.dev@gmail.com]  
🔗 [LinkedIn](https://www.linkedin.com/in/rafael-colares/)
