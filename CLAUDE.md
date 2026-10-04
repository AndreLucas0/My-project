# AR Mechatronic Cell

## Contexto

Projeto acadêmico desenvolvido no Unity 6 para criação de um protótipo simples de Realidade Aumentada aplicado à visualização e manutenção de uma célula mecatrônica.

O projeto é baseado no relatório técnico de viabilidade existente para a atividade acadêmica.

## Objetivo

Criar um protótipo 3D/AR simples que permita:

* visualizar uma célula mecatrônica simplificada;
* representar componentes reais por objetos 3D;
* selecionar componentes;
* visualizar informações básicas dos componentes;
* demonstrar uma interação simples relacionada à manutenção;
* posicionar a célula em um ambiente real utilizando AR;
* manter proporções coerentes com o mundo real;
* gerar uma build para o dispositivo móvel definido na atividade;
* gerar um arquivo `.unitypackage`.

## Escopo

Este é um protótipo acadêmico de baixa complexidade.

Priorizar:

1. funcionamento;
2. simplicidade;
3. facilidade de demonstração;
4. facilidade de manutenção;
5. compatibilidade com o dispositivo-alvo.

## Fora do escopo atual

Não implementar, salvo solicitação explícita:

* integração com PLC;
* OPC UA;
* MQTT;
* WebSocket;
* backend;
* banco de dados;
* cloud;
* MDM;
* telemetria industrial;
* manutenção preditiva;
* visão computacional;
* assistência remota;
* transmissão de vídeo;
* comandos de voz;
* autenticação;
* sistema de usuários;
* digital twin industrial completo.

Esses recursos podem existir como possibilidades futuras no relatório de viabilidade, mas não fazem parte do protótipo acadêmico atual.

## Princípios de desenvolvimento

### Analyze First

Antes de alterar arquivos:

1. inspecione o projeto;
2. entenda a configuração atual;
3. identifique dependências;
4. confirme o requisito;
5. defina a menor implementação possível.

### Tests First

Para comportamentos testáveis:

1. defina o comportamento esperado;
2. crie o teste mínimo;
3. implemente;
4. execute o teste;
5. corrija falhas;
6. valide novamente.

Não criar testes artificiais para configurações puramente visuais ou assets.

### Simplicidade

Evitar:

* overengineering;
* abstrações desnecessárias;
* padrões arquiteturais sem necessidade;
* dependências desnecessárias;
* refatorações fora do escopo.

Sempre preferir a solução funcional mais simples.

### Validação

Nunca considerar uma funcionalidade concluída apenas porque o código compila.

Sempre que aplicável:

* executar testes;
* abrir a cena;
* executar o projeto;
* verificar o Console;
* gerar a build;
* validar a build.

### Mudanças

Não alterar arquivos fora do escopo da tarefa.

Não atualizar dependências sem necessidade.

Não alterar a versão do Unity sem justificativa.

## Escala

Sempre que possível:

> 1 Unity unit = 1 metro.

Os componentes da célula devem manter proporções coerentes entre si.

## Plataforma

A plataforma e o dispositivo-alvo devem ser confirmados a partir da documentação da atividade.

Caso não exista definição disponível no projeto, utilizar Android como padrão inicial.

## Documentação

Documentações importantes devem ficar em:

```text
Documentation/
```

## Estrutura esperada

```text
Assets/
Packages/
ProjectSettings/
Documentation/
CLAUDE.md
README.md
```

## Regra principal

Este projeto é uma atividade acadêmica.

Não transformar o protótipo em uma solução industrial completa.

Quando houver dúvida entre uma implementação simples e uma implementação complexa, escolher a simples, desde que cumpra o requisito.