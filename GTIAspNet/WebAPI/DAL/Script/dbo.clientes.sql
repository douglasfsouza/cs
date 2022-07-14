CREATE TABLE [dbo].[clientes] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Nome]                NVARCHAR (MAX) NOT NULL,
    [CPF]                 NVARCHAR (MAX) NOT NULL,
    [RG]                  NVARCHAR (MAX) NULL,
    [DataExpedicao]       DATETIME2 (7)  NULL,
    [OrgaoExpedicao]      NVARCHAR (MAX) NULL,
    [UF]                  NVARCHAR (MAX) NULL,
    [DataNascimento]      DATETIME2 (7)  NOT NULL,
    [Sexo]                NVARCHAR (MAX) NOT NULL,
    [EstadoCivil]         NVARCHAR (MAX) NOT NULL,
    [EnderecoCEP]         NVARCHAR (MAX) NOT NULL,
    [EnderecoLogradouro]  NVARCHAR (MAX) NOT NULL,
    [EnderecoNumero]      NVARCHAR (MAX) NOT NULL,
    [EnderecoComplemento] NVARCHAR (MAX) NULL,
    [EnderecoBairro]      NVARCHAR (MAX) NOT NULL,
    [EnderecoCidade]      NVARCHAR (MAX) NOT NULL,
    [EnderecoUF]          NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_clientes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

