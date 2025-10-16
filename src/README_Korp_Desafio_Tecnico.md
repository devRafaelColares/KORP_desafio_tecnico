# 🧾 Projeto: Sistema de Emissão e Impressão de Notas Fiscais

## 🏢 Desafio Técnico — Processo Seletivo Korp
**Vaga:** Desenvolvedor(a) Júnior — C# ou Go + Angular  
**Modelo:** Remoto  
**Empresa:** Korp ERP  

---

## 🎯 Objetivo do Projeto
Desenvolver uma aplicação completa para **emissão e impressão de notas fiscais**, utilizando **arquitetura de microsserviços**.  
O sistema deve permitir o **cadastro de produtos**, **controle de estoque**, **criação de notas fiscais** e **impressão validada** conforme o saldo disponível.

Além disso, o desafio inclui **tratamento de falhas entre serviços**, **concorrência**, **idempotência** e **uso de IA aplicada**.

---

## 🧱 Arquitetura de Microsserviços

### **Serviços Principais**
1. **EstoqueService**  
   - Gerencia produtos, saldos e movimentações.  
   - Responsável por validações de estoque durante o processo de impressão.

2. **FaturamentoService**  
   - Gerencia notas fiscais, seus itens e status.  
   - Realiza a integração com o EstoqueService para validação e baixa de saldo.

### **Infraestrutura**
- Comunicação via **APIs REST**.
- Contêineres isolados utilizando **Docker** e **Docker Compose**.
- Banco de dados: **SQL Server** (rodando em container).
- Documentação via **Swagger/OpenAPI**.

---

## ⚙️ Tecnologias Utilizadas

### 🧩 Backend (.NET 9)
- **ASP.NET Core 9**
- **Entity Framework Core**
- **Microsoft.EntityFrameworkCore.SqlServer**
- **Microsoft.EntityFrameworkCore.Design**
- **Newtonsoft.Json**
- **Swashbuckle.AspNetCore** (Swagger/OpenAPI)
- **Microsoft.AspNetCore.Authentication.JwtBearer**

### 💻 Frontend (Angular)
- **Angular 17+**
- **TypeScript**
- **TailwindCSS**
- **Bootstrap 5**
- **CSS3 Customizado**

### 🐳 Infraestrutura
- **Docker**
- **Docker Compose**
- **SQL Server (imagem oficial)**
- **Git + GitHub Actions (CI/CD local)**

---

## 🚀 Plano de Execução por Sprints

| Sprint | Foco Principal | Entregas-Chave | Checkpoint |
|--------|----------------|----------------|-------------|
| **1** | Setup e Arquitetura | Docker, SQL Server, APIs base | Comunicação Angular ↔ API |
| **2** | Core de Estoque | CRUD Produtos e Movimentações | Swagger e EF Core funcional |
| **3** | Faturamento e Integração | Emissão, impressão e falhas | Fluxo completo com logs |
| **4** | Frontend Angular | UI, feedbacks e consumo APIs | Demo navegável |
| **5** | Robustez e IA | Concorrência, Idempotência, IA | Entrega final e documentação |

---

## 🏁 Sprint 1 — Planejamento e Setup

### 🎯 Objetivos
- Configurar ambiente de desenvolvimento e containers Docker.
- Criar estrutura base dos microsserviços e frontend Angular.
- Disponibilizar documentação via Swagger.

### ✅ Critérios de Aceite
- Containers sobem corretamente com `docker-compose up`.
- Swagger disponível em `/swagger` para ambos os serviços.
- Angular comunica-se com os endpoints backend.

---

## ⚙️ Sprint 2 — Core Backend: EstoqueService

### 🎯 Objetivos
- Implementar CRUD de produtos e controle de saldo.  
- Garantir persistência e isolamento entre microsserviços.

### 🔨 Backlog
- Entidades: `Produto`, `MovimentacaoEstoque`.  
- Endpoints:  
  - `POST /produtos`  
  - `GET /produtos`  
  - `PATCH /produtos/:id`  
  - `GET /estoque/saldo/:produtoId`

### ✅ Critérios de Aceite
- Produto é cadastrado e listado corretamente.  
- Movimentações afetam o saldo com consistência.  

---

## 💼 Sprint 3 — Faturamento e Integração

### 🎯 Objetivos
- Criar notas fiscais e implementar lógica de impressão com validação de estoque.  
- Realizar comunicação entre microsserviços e simular falha controlada.

### 🔨 Backlog
- Entidades: `NotaFiscal`, `ItemNotaFiscal`.  
- Endpoints:  
  - `POST /notas`  
  - `GET /notas`  
  - `POST /notas/:id/imprimir`  
- Integração REST entre EstoqueService e FaturamentoService.  
- Tratamento de falhas e rollback.  

### ✅ Critérios de Aceite
- Impressão atualiza saldo e altera status da nota.  
- Falhas exibem feedback claro ao usuário.  

---

## 🎨 Sprint 4 — Frontend Angular

### 🎯 Objetivos
- Construir interface completa para cadastro e emissão de notas.  
- Aplicar design responsivo com Tailwind e Bootstrap.

### 🔨 Backlog
- Páginas:  
  - **Produtos** (CRUD)  
  - **Notas Fiscais** (criação, listagem, impressão)  
- Feedback visual (toasts, modais, loaders).  

### ✅ Critérios de Aceite
- Usuário realiza todo o fluxo via interface.  
- Feedbacks visuais para falhas e sucesso.  

---

## 🧠 Sprint 5 — Robustez, Concorrência e IA

### 🎯 Objetivos
- Implementar recursos de resiliência e IA.  
- Garantir robustez, idempotência e documentação final.

### 🔨 Backlog
- **Lock otimista/pessimista** para evitar vendas simultâneas.  
- **Idempotência** nas requisições de impressão.  
- Uso prático de **IA aplicada**, por exemplo:
  - Análise automática de logs para identificar falhas.
  - Geração de mensagens inteligentes de feedback.  
- Documentação técnica completa e `README.md` final.  

### ✅ Critérios de Aceite
- Concorrência e idempotência testadas com sucesso.  
- IA funcional e observável no sistema.  
- Projeto executável via `docker-compose up`.  

---

## 🧩 Estrutura de Diretórios (Proposta)

```
/src
 ├── Backend/
 │    ├── EstoqueService/
 │    │    ├── Controllers/
 │    │    │    └── ProdutosController.cs
 │    │    ├── Models/
 │    │    │    └── Produto.cs
 │    │    ├── Services/
 │    │    │    └── ProdutoService.cs
 │    │    ├── Repositories/
 │    │    │    └── ProdutoRepository.cs
 │    │    ├── Data/
 │    │    │    └── EstoqueDbContext.cs
 │    │    ├── appsettings.json
 │    │    ├── EstoqueService.csproj
 │    │    └── Program.cs
 │    │
 │    ├── FaturamentoService/
 │    │    ├── Controllers/
 │    │    │    └── NotasFiscaisController.cs
 │    │    ├── Models/
 │    │    │    └── NotaFiscal.cs
 │    │    ├── Services/
 │    │    │    └── NotaFiscalService.cs
 │    │    ├── Repositories/
 │    │    │    └── NotaFiscalRepository.cs
 │    │    ├── Data/
 │    │    │    └── FaturamentoDbContext.cs
 │    │    ├── appsettings.json
 │    │    ├── FaturamentoService.csproj
 │    │    └── Program.cs
 │
 ├── Frontend/
 │    ├── src/
 │    │    ├── app/
 │    │    │    ├── pages/
 │    │    │    │    ├── produtos/
 │    │    │    │    └── notas-fiscais/
 │    │    │    ├── components/
 │    │    │    │    └── shared/
 │    │    │    ├── services/
 │    │    │    │    ├── produto.service.ts
 │    │    │    │    └── nota-fiscal.service.ts
 │    │    │    └── app.module.ts
 │    │    ├── assets/
 │    │    └── environments/
 │    │         ├── environment.ts
 │    │         └── environment.prod.ts
 │    └── angular.json
 │
 ├── docker-compose.yml
 ├── .gitignore
 └── README.md
```

---

## 🧪 Execução do Projeto

### 🐳 Subir containers
```bash
docker-compose up --build
```

### 🌐 Acessar interfaces
- **Swagger Estoque:** http://localhost:5001/swagger  
- **Swagger Faturamento:** http://localhost:5002/swagger  
- **Frontend Angular:** http://localhost:4200  

---

## 📚 Documentação e Boas Práticas

- Código versionado.  
- APIs documentadas via **Swagger/OpenAPI**.  
- Logs estruturados e métricas básicas de performance.  
- Testes unitários e cenários de falha simulados.  

---

## 🧠 IA Aplicada (Exemplo)
A aplicação contará com um módulo de IA para:
- Analisar logs de falha entre microsserviços.  
- Sugerir diagnósticos automáticos (ex: falha de rede, concorrência).  
- Gerar mensagens inteligentes de feedback ao usuário final.  

---

## 🏆 Resultado Esperado
Ao final do projeto, espera-se:
- Sistema totalmente funcional via Docker Compose.  
- Comunicação resiliente entre microsserviços.  
- UI navegável e responsiva em Angular.  
- Tratamento de concorrência e idempotência aplicados.  
- Documentação técnica clara e estruturada.  

---

## 📄 Autor
**Rafael Colares**  
Desenvolvedor Full Stack • .NET • Angular • SQL Server  
📧 [rafaelcolares.dev@gmail.com]  
🔗 [LinkedIn](https://www.linkedin.com/in/rafael-colares/)
