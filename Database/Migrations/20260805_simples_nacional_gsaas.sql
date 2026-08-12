-- -----------------------------------------------------------------------------
-- 1) Declaração Simples Nacional (1 registro por empresa)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `simples_declaration` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `declaration_kind` ENUM('SIMPLES_EXERCISE_YEARS', 'NO_NATIONAL_SIMPLE_STRICT') NOT NULL,
  `date_create` DATETIME NOT NULL,
  `date_update` DATETIME NOT NULL,
  `id_usuario` BIGINT NULL COMMENT 'Usuário Capag que criou/atualizou',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_simples_declaration_empresa` (`id_empresa`),
  KEY `idx_simples_declaration_tenant` (`id_tenant`),
  KEY `idx_simples_declaration_usuario` (`id_usuario`),
  CONSTRAINT `fk_simples_declaration_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- -----------------------------------------------------------------------------
-- 2) Exercícios declarados (N por declaração)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `simples_exercise_year` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_simples_declaration` BIGINT UNSIGNED NOT NULL,
  `exercise_year` SMALLINT NOT NULL,
  `date_create` DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_simples_exercise_year` (`id_simples_declaration`, `exercise_year`),
  CONSTRAINT `fk_simples_exercise_year_declaration`
    FOREIGN KEY (`id_simples_declaration`) REFERENCES `simples_declaration` (`id`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- -----------------------------------------------------------------------------
-- 3) Valores manuais de DRE / Balanço (conta analítica × exercício)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `manual_demonstrative_value` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `demonstrative_kind` ENUM('DRE', 'BALANCE_SHEET') NOT NULL,
  `account_code` VARCHAR(64) NOT NULL,
  `exercise_year` SMALLINT NOT NULL,
  `amount` DECIMAL(20,4) NOT NULL,
  `date_create` DATETIME NOT NULL,
  `date_update` DATETIME NOT NULL,
  `id_usuario` BIGINT NULL COMMENT 'Usuário Capag que criou/atualizou',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_manual_demonstrative_value`
    (`id_empresa`, `demonstrative_kind`, `account_code`, `exercise_year`),
  KEY `idx_manual_demonstrative_value_empresa` (`id_empresa`),
  KEY `idx_manual_demonstrative_value_tenant` (`id_tenant`),
  KEY `idx_manual_demonstrative_value_usuario` (`id_usuario`),
  CONSTRAINT `fk_manual_demonstrative_value_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
