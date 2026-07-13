# Plataforma Bancária - Desafio Inovamobil

Este repositório contém a solução para o desafio de Desenvolvedor Backend .NET, focado na construção de uma Plataforma de Contas Bancárias. O sistema foi desenhado para ser altamente escalável, utilizando separação de responsabilidades para escrita e leitura de dados, além de comunicação assíncrona.

## Sobre
Este projeto consiste no desenvolvimento de uma Plataforma Bancária, permitindo a abertura de contas exclusivamente via CNPJ e o processamento de transações financeiras como depósitos, saques e transferências. Mais do que um fluxo financeiro tradicional, o objetivo central foi construir um motor bancário robusto e distribuído. A arquitetura foi desenhada para impedir duplicidade de transações através de idempotência e garantir consultas dos clientes, orquestrando múltiplos bancos de dados e mensageria assíncrona.

### Ferramentas e Tecnologias

1. **Padrões Arquiteturais**
     - **Domain-Driven Design (DDD):** O sistema foi modelado focando no núcleo do negócio, separando as responsabilidades em camadas claras e utilizando conceitos estruturais como Aggregates, Value Objects e Domain Events.
     - **CQRS (Command Query Responsibility Segregation):** Foi implementada a separação estrita entre a intenção de modificar dados (Commands) e a intenção de ler dados (Queries). O lado de escrita foi direcionado ao banco relacional, enquanto o lado de leitura foi direcionado a um banco NoSQL para projeções rápidas.
     - **Event-Driven Architecture (Arquitetura Orientada a Eventos):** A comunicação entre as ações do usuário e a consolidação dos dados foi feita de forma assíncrona, publicando e consumindo mensagens através de um message broker.
       
2. **Lógica**
   - **Event Sourcing (Abordagem Prática):** O saldo da conta (/balance) não é um número estático no banco de leitura; ele é calculado dinamicamente em tempo real através da soma do histórico de eventos financeiros (transferências, depósitos e saques) armazenados na coleção de transações.
   - **Idempotência:** Foi implementada uma barreira de proteção utilizando a `idempotencyKey` enviada pelo cliente nas operações financeiras. Isso garante que, em caso de falha na rede ou reenvio acidental, a mesma transação não seja processada e debitada duas vezes.
   - **Fail-Fast Validations:** Utilização de validações de borda para interceptar dados inválidos imediatamente (antes de chegarem ao domínio ou ao banco de dados), garantindo os inputs.
3. **Ferramentas**
   - **C# e .NET 8 (ASP.NET Core):** O framework principal e runtime utilizados para erguer a API REST.
   - **PostgreSQL e Entity Framework Core:** O banco de dados de escrita (Write Model) e o ORM responsáveis por garantir a integridade relacional.
   - **MongoDB:** O banco de dados de leitura (Read Model) voltado para altíssima performance em consultas sem bloqueios.
   - **RabbitMQ e MassTransit:** O agente de mensagens e a abstração utilizada para publicar eventos e criar os Workers (consumidores) de forma resiliente.
   - **Docker e Docker Compose:** Junta os cinco serviços exigidos (`api`, `worker`, `postgres`, `mongodb`, `rabbitmq`) para que subam juntos com um único comando.
4. **Integrações Externas**
   - **ReceitaWS:** O sistema se comunica com a API da Receita Federal (ReceitaWS) para validar e capturar ativamente os dados da empresa (RazaoSocial) através do CNPJ no momento da abertura da conta.

### Desenvolvimento do Projeto:

1. **API e o Padrão CQRS**
   - A API foi construída utilizando **ASP.NET Core no .NET 8**. O núcleo da API é a implementação rigorosa do padrão **CQRS** através da biblioteca **MediatR**.
   - A API não acessa o banco de dados diretamente de forma bagunçada. Quando o usuário quer modificar algo (abrir conta, depositar, transferir), a API despacha um **Command**. Quando o usuário quer consultar algo (saldo, extrato), a API despacha uma **Query**. Isso garante que o código de regras de negócio complexas não se misture com o código de consultas rápidas.
2. **O Fluxo de Inserção de Dados**
   - **A Escrita (PostgreSQL):** Quando um Command é executado (ex: realizar transferência), o sistema usa o Entity Framework Core para gravar no PostgreSQL. Este é o banco relacional, responsável por garantir a integridade da transação (debita de um, credita no outro).
   - **A Leitura (MongoDB):** As consultas de saldo e extrato leem dados exclusivamente do MongoDB. O saldo não é um valor estático salvo em uma tabela; ele é calculado de forma dinâmica (Event Sourcing) somando todas as transações daquela conta.
3. **RabbitMQ e o Worker**
   - **RabbitMQ:** Funciona como um Message Broker. Quando o PostgreSQL finaliza um depósito, a API "joga" um evento na fila do RabbitMQ (ex: `DepositoRealizadoEvent`) e devolve a resposta de sucesso para o usuário.
   - **Worker (MassTransit):** É um serviço que roda em segundo plano. Ele fica "escutando" as filas do RabbitMQ. Quando o evento chega, o Worker o captura e salva o registro dessa movimentação no MongoDB, atualizando o extrato.
4. **Idempotência**
   - O Problema que resolve: Se a internet do usuário oscilar e ele enviar a mesma requisição de "Transferir R$ 500" duas vezes, o banco não pode debitar R$ 1.000.
   - Como foi feito: Toda requisição financeira exige um campo `idempotencyKey` único gerado pelo cliente. A aplicação verifica se essa chave já foi processada anteriormente. Se sim, ela bloqueia ou ignora a requisição duplicada, garantindo que o dinheiro saia apenas uma vez.
5. **Integração com a ReceitaWS**
   - A regra de negócio dizia que a RazaoSocial da empresa nunca poderia ser digitada pelo usuário.
   - No endpoint de abertura de conta, quando o usuário envia um CNPJ, a aplicação faz uma chamada HTTP externa para a API pública da ReceitaWS. Ela bate o CNPJ lá, verifica se ele é válido e ativo na Receita Federal, extrai a Razão Social oficial e só então prossegue com a criação da conta no banco de dados.
6. **Validações e Regras de Negócio**
   - o FluentValidation para criar regras estritas logo na entrada da API. Ele barra depósitos com valor zero ou negativo, valida o formato do CNPJ através do cálculo de dígito verificador e impede operações em contas que estejam bloqueadas ou encerradas.
7. **Encerramento Seguro**
   - Para garantir o histórico de auditoria, quando uma conta é excluída, há um Soft Delete. O status dela é alterado para `Encerrada`. E uma regra de ouro é validada antes disso: o sistema verifica se o saldo é exatamente `0.00`; caso a conta tenha dinheiro ou dívidas, o encerramento é bloqueado.
8. **Swagger e Docker Compose**
   - **Swagger:** Uma ferramenta padrão do mercado para gerar uma interface gráfica e interativa, permitindo que qualquer desenvolvedor que pegue o seu projeto entenda os contratos (`/accounts`, `/deposit`, `/transfer`) e teste a API diretamente pelo navegador
   - **Docker Compose:** O projeto inteiro foi conteinerizado. Com um único comando (docker compose up), o Docker sobe a API, o Worker, o RabbitMQ, o PostgreSQL e o MongoDB, amarrando todas as redes e variáveis de ambiente automaticamente.

## Estrutura do Projeto

```text
📦 DESAFIO-BACKEND-INOVAMO
 ┣ 📂 PlataformaBancaria.Api
 ┃ ┣ 📂 Controllers
 ┃ ┃ ┗ 📜 ContasController.cs
 ┃ ┣ 📂 Properties
 ┃ ┃ ┗ 📜 launchSettings.json
 ┃ ┣ 📜 appsettings.Development.json
 ┃ ┣ 📜 appsettings.json
 ┃ ┣ 📜 Dockerfile
 ┃ ┣ 📜 PlataformaBancaria.Api.csproj
 ┃ ┣ 📜 PlataformaBancaria.Api.http
 ┃ ┗ 📜 Program.cs
 ┃
 ┣ 📂 PlataformaBancaria.Application
 ┃ ┣ 📂 Commands
 ┃ ┃ ┣ 📂 Contas
 ┃ ┃ ┃ ┣ 📜 AbrirContaCommand.cs
 ┃ ┃ ┃ ┣ 📜 AbrirContaCommandHandler.cs
 ┃ ┃ ┃ ┣ 📜 AlterarStatusContaCommand.cs
 ┃ ┃ ┃ ┣ 📜 AlterarStatusContaCommandHandler.cs
 ┃ ┃ ┃ ┣ 📜 EncerrarContaCommand.cs
 ┃ ┃ ┃ ┗ 📜 EncerrarContaCommandHandler.cs
 ┃ ┃ ┗ 📂 Operacoes
 ┃ ┃   ┣ 📜 RealizarDepositoCommand.cs
 ┃ ┃   ┣ 📜 RealizarDepositoCommandHandler.cs
 ┃ ┃   ┣ 📜 RealizarSaqueCommand.cs
 ┃ ┃   ┣ 📜 RealizarSaqueCommandHandler.cs
 ┃ ┃   ┣ 📜 RealizarTransferenciaCommand.cs
 ┃ ┃   ┗ 📜 RealizarTransferenciaCommandHandler.cs
 ┃ ┣ 📂 DTOs
 ┃ ┃ ┣ 📜 ContaResponseDto.cs
 ┃ ┃ ┣ 📜 CriarContaRequestDto.cs
 ┃ ┃ ┣ 📜 RealizarDepositoRequestDto.cs
 ┃ ┃ ┗ 📜 RealizarSaqueRequestDto.cs
 ┃ ┣ 📂 Events
 ┃ ┃ ┣ 📜 DepositoRealizadoEvent.cs
 ┃ ┃ ┣ 📜 SaqueRealizadoEvent.cs
 ┃ ┃ ┗ 📜 TransferenciaRealizadaEvent.cs
 ┃ ┣ 📂 Queries
 ┃ ┃ ┗ 📂 Contas
 ┃ ┃   ┣ 📜 ObterContaPorCnpjQuery.cs
 ┃ ┃   ┣ 📜 ObterContaPorCnpjQueryHandler.cs
 ┃ ┃   ┣ 📜 ObterContaPorIdQuery.cs
 ┃ ┃   ┣ 📜 ObterContaPorIdQueryHandler.cs
 ┃ ┃   ┣ 📜 ObterSaldoQuery.cs
 ┃ ┃   ┣ 📜 ObterSaldoQueryHandler.cs
 ┃ ┃   ┗ 📜 ObterExtratoQuery.cs
 ┃ ┣ 📂 Services
 ┃ ┃ ┣ 📂 Interfaces
 ┃ ┃ ┃ ┗ 📜 IContaAppService.cs
 ┃ ┃ ┗ 📜 ContaAppService.cs
 ┃ ┣ 📜 Class1.cs
 ┃ ┗ 📜 PlataformaBancaria.Application.csproj
 ┃
 ┣ 📂 PlataformaBancaria.Domain
 ┃ ┣ 📂 Entities
 ┃ ┃ ┣ 📜 ChaveIdempotencia.cs
 ┃ ┃ ┣ 📜 Conta.cs
 ┃ ┃ ┗ 📜 Transacao.cs
 ┃ ┣ 📂 Enums
 ┃ ┃ ┗ 📜 TipoTransacao.cs
 ┃ ┣ 📂 Repositories
 ┃ ┃ ┣ 📜 IContaRepository.cs
 ┃ ┃ ┗ 📜 IIdempotenciaRepository.cs
 ┃ ┣ 📂 Services
 ┃ ┃ ┗ 📜 IEmpresaService.cs
 ┃ ┣ 📂 ValueObjects
 ┃ ┃ ┗ 📜 Cnpj.cs
 ┃ ┗ 📜 PlataformaBancaria.Domain.csproj
 ┃
 ┣ 📂 PlataformaBancaria.Infrastructure
 ┃ ┣ 📂 Data
 ┃ ┃ ┣ 📂 Configurations
 ┃ ┃ ┃ ┣ 📜 ContaConfiguration.cs
 ┃ ┃ ┃ ┗ 📜 TransacaoConfiguration.cs
 ┃ ┃ ┗ 📜 AppDbContext.cs
 ┃ ┣ 📂 Migrations
 ┃ ┣ 📂 Repositories
 ┃ ┃ ┣ 📜 ContaRepository.cs
 ┃ ┃ ┗ 📜 IdempotenciaRepository.cs
 ┃ ┣ 📂 Services
 ┃ ┃ ┗ 📜 EmpresaService.cs
 ┃ ┣ 📜 Class1.cs
 ┃ ┗ 📜 PlataformaBancaria.Infrastructure.csproj
 ┃
 ┣ 📂 PlataformaBancaria.Worker
 ┃ ┣ 📂 Consumers
 ┃ ┃ ┣ 📜 DepositoRealizadoConsumer.cs
 ┃ ┃ ┣ 📜 SaqueRealizadoConsumer.cs
 ┃ ┃ ┗ 📜 TransferenciaRealizadaConsumer.cs
 ┃ ┣ 📂 Models
 ┃ ┃ ┗ 📜 TransacaoDocument.cs
 ┃ ┣ 📂 Properties
 ┃ ┃ ┗ 📜 launchSettings.json
 ┃ ┣ 📜 appsettings.Development.json
 ┃ ┣ 📜 appsettings.json
 ┃ ┣ 📜 Dockerfile
 ┃ ┣ 📜 PlataformaBancaria.Worker.csproj
 ┃ ┣ 📜 Program.cs
 ┃ ┗ 📜 Worker.cs
 ┃
 ┣ 📜 .gitignore
 ┣ 📜 docker-compose.yml
 ┣ 📜 PlataformaBancaria.sln
 ┗ 📜 README.md
```


## Como Executar o Projeto

O projeto foi totalmente conteinerizado utilizando **Docker** para garantir que a aplicação, os bancos de dados e o sistema de mensageria rodem em qualquer ambiente de forma isolada, padronizada e sem a necessidade de instalações locais complexas.

### Pré-requisitos
Antes de começar, certifique-se de ter as seguintes ferramentas instaladas na sua máquina:
* **[Git](https://git-scm.com/)** (Para clonar o repositório)
* **[Docker](https://www.docker.com/)** e **Docker Compose** (Para orquestrar os contêineres)

### Passo a Passo

1. **Clone o repositório**
Abra o seu terminal e execute o comando abaixo para baixar o código-fonte:

SSH
```bash
git clone git@github.com:otaviohiratsuka/desafio-backend-inovamobil.git
```

HTTPS
```bash
git clone https://github.com/otaviohiratsuka/desafio-backend-inovamobil.git
```

2. Acesse a pasta do projeto
Navegue até o diretório raiz do repositório clonado:

```bash
cd <desafio-backend-inovamobil>
```

3. Suba a infraestrutura
Na raiz do projeto (onde está localizado o arquivo `docker-compose.yml`), execute o comando abaixo para construir as imagens e levantar todos os microsserviços (API, Worker, PostgreSQL, MongoDB e RabbitMQ):
```bash
docker compose up --build
```

3. Aguarde a inicialização
O sistema estará pronto para uso quando os logs do terminal indicarem que o ``plataforma-api`` está escutando as requisições e o ``plataforma-worker`` declarou que se conectou com sucesso ao RabbitMQ.

5. Acessos Úteis
Com a infraestrutura rodando, você pode interagir com o sistema e monitorar os bastidores através dos seguintes links no seu navegador:

Interface interativa para testar os Commands (abrir conta, depósito, saque, transferência) e Queries (saldo, extrato).
`http://localhost:5117/swagger/index.html` (Ajuste a porta caso tenha mapeado diferente)

Interface visual para monitorar a saúde do Message Broker, visualizar a criação das filas (Ex: `transferencia-realizada-queue`) e o tráfego dos eventos de domínio.
`http://localhost:15672`
Usuário: `guest` | Senha: `guest`

6. Parar a aplicação
Para encerrar a execução de forma segura e desligar todos os contêineres, pressione `CTRL+C` no terminal onde os logs estão rodando.

Como alternativa, você pode abrir uma nova aba do terminal na raiz do projeto e executar
```bash
docker compose down
```

## Autor

Otávio Hiratsuka Camilo

https://github.com/otaviohiratsuka

