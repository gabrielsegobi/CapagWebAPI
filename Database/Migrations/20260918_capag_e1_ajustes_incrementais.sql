-- =============================================================================
-- CAPAG-e1 — persistência incremental (PRL-A + GRE)
-- Produção | MySQL 8+ | schema: gsaas
-- Idempotente: se as tabelas já existirem, não faz nada.
-- =============================================================================

USE `gsaas`;

CREATE TABLE IF NOT EXISTS `conta_prla_ajuste` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `codigo_conta` VARCHAR(64) NOT NULL,
  `acao` VARCHAR(32) NULL COMMENT 'incluir | incluir_com_desagio | excluir',
  `justificativa` VARCHAR(500) NULL,
  `saldo_manual` DECIMAL(20,2) NULL,
  `atualizado_em` DATETIME NOT NULL,
  `atualizado_por` VARCHAR(128) NOT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_conta_prla_ajuste_empresa_codigo` (`id_empresa`, `codigo_conta`),
  KEY `idx_conta_prla_ajuste_tenant` (`id_tenant`),

  CONSTRAINT `fk_conta_prla_ajuste_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_conta_prla_ajuste_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_conta_prla_ajuste_acao` CHECK (
    `acao` IS NULL OR `acao` IN ('incluir', 'incluir_com_desagio', 'excluir')
  )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Ação, justificativa e saldo editável do PRL-A';

CREATE TABLE IF NOT EXISTS `conta_gre_inversao` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `codigo_conta` VARCHAR(64) NOT NULL,
  `ano` SMALLINT NOT NULL,
  `invertido` TINYINT NOT NULL DEFAULT 1,
  `justificativa` VARCHAR(500) NULL,
  `atualizado_em` DATETIME NOT NULL,
  `atualizado_por` VARCHAR(128) NOT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_conta_gre_inversao_empresa_codigo_ano` (`id_empresa`, `codigo_conta`, `ano`),
  KEY `idx_conta_gre_inversao_tenant` (`id_tenant`),

  CONSTRAINT `fk_conta_gre_inversao_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_conta_gre_inversao_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Inversão de sinal por exercício na GRE';

CREATE TABLE IF NOT EXISTS `conta_gre_manual` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `codigo_conta` VARCHAR(64) NOT NULL,
  `codigo_pai` VARCHAR(64) NULL,
  `descricao` VARCHAR(255) NOT NULL,
  `tipo` VARCHAR(16) NOT NULL COMMENT 'receita | despesa',
  `usar_media` TINYINT NOT NULL DEFAULT 0,
  `justificativa` VARCHAR(500) NULL,
  `valores_json` JSON NOT NULL,
  `atualizado_em` DATETIME NOT NULL,
  `atualizado_por` VARCHAR(128) NOT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_conta_gre_manual_empresa_codigo` (`id_empresa`, `codigo_conta`),
  KEY `idx_conta_gre_manual_tenant` (`id_tenant`),

  CONSTRAINT `fk_conta_gre_manual_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_conta_gre_manual_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_conta_gre_manual_tipo` CHECK (`tipo` IN ('receita', 'despesa'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Contas manuais da GRE (descrição, tipo, média, valores, justificativa)';
