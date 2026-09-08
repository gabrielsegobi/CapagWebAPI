-- -----------------------------------------------------------------------------
-- Exceções FCO (L100/L300) por empresa.
-- O catálogo universal (~732 + ~386 contas) NÃO é duplicado no banco.
-- Empresa nova = zero linhas = 100% default do client.
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `fco_parametro_empresa` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `excecoes_l100_json` JSON NOT NULL,
  `excecoes_l300_json` JSON NOT NULL,
  `base_versao_hash` VARCHAR(16) NULL,
  `created_at` DATETIME NOT NULL,
  `updated_at` DATETIME NOT NULL,
  `id_usuario` BIGINT NULL COMMENT 'Usuário Capag que criou/atualizou',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_fco_parametro_empresa` (`id_empresa`),
  KEY `idx_fco_parametro_empresa_tenant` (`id_tenant`),
  KEY `idx_fco_parametro_empresa_usuario` (`id_usuario`),
  CONSTRAINT `fk_fco_parametro_empresa_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
