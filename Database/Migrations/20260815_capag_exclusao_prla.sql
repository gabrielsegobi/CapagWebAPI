-- Exclusão de contas do cálculo CAPAG e overrides do PRL-A (bloco / deságio)

CREATE TABLE IF NOT EXISTS `conta_exclusao_config` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `codigo_conta` VARCHAR(64) NOT NULL,
  `excluida` TINYINT(1) NOT NULL DEFAULT 0,
  `justificativa` VARCHAR(500) NULL,
  `atualizado_em` DATETIME NOT NULL,
  `atualizado_por` VARCHAR(128) NOT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_conta_exclusao_empresa_codigo` (`id_empresa`, `codigo_conta`),
  KEY `idx_conta_exclusao_tenant` (`id_tenant`),

  CONSTRAINT `fk_conta_exclusao_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_conta_exclusao_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Contas excluídas do cálculo hierárquico da CAPAG (GRE/indicadores)';

CREATE TABLE IF NOT EXISTS `conta_bloco_override` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `codigo_conta` VARCHAR(64) NOT NULL,
  `bloco_original` CHAR(1) NOT NULL,
  `bloco_ajustado` CHAR(1) NULL,
  `atualizado_em` DATETIME NOT NULL,
  `atualizado_por` VARCHAR(128) NOT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_conta_bloco_empresa_codigo` (`id_empresa`, `codigo_conta`),
  KEY `idx_conta_bloco_tenant` (`id_tenant`),

  CONSTRAINT `fk_conta_bloco_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_conta_bloco_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `chk_conta_bloco_original` CHECK (`bloco_original` IN ('A', 'B', 'C')),
  CONSTRAINT `chk_conta_bloco_ajustado` CHECK (`bloco_ajustado` IS NULL OR `bloco_ajustado` IN ('A', 'B', 'C'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Override do bloco de liquidez (PRL-A); o cálculo usa bloco_ajustado ?? bloco_original';

CREATE TABLE IF NOT EXISTS `conta_desagio_override` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `codigo_conta` VARCHAR(64) NOT NULL,
  `percentual_desagio` DECIMAL(7,4) NOT NULL,
  `atualizado_em` DATETIME NOT NULL,
  `atualizado_por` VARCHAR(128) NOT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_conta_desagio_empresa_codigo` (`id_empresa`, `codigo_conta`),
  KEY `idx_conta_desagio_tenant` (`id_tenant`),

  CONSTRAINT `fk_conta_desagio_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_conta_desagio_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Deságio manual do PRL-A; o total sempre soma saldo_ajustado ?? saldo_original';
