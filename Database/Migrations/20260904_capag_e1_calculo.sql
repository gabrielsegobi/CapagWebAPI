-- -----------------------------------------------------------------------------
-- Estado completo do cálculo CAPAG-e1 (GRE/PLRA/ajustes manuais/resultado).
-- gre_linhas fica dentro de payload_json (JSON opaco) para preservar campos
-- como valoresSinalInvertido sem DTO rígido.
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `capag_e1_calculo` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL,
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `modelo` VARCHAR(32) NOT NULL DEFAULT 'capag-e-1',
  `payload_json` JSON NOT NULL,
  `date_create` DATETIME NOT NULL,
  `date_update` DATETIME NOT NULL,
  `id_usuario` BIGINT NULL COMMENT 'Usuário Capag que criou/atualizou',

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_capag_e1_calculo_empresa` (`id_empresa`, `modelo`),
  KEY `idx_capag_e1_calculo_tenant` (`id_tenant`),
  KEY `idx_capag_e1_calculo_empresa` (`id_empresa`),

  CONSTRAINT `fk_capag_e1_calculo_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,

  CONSTRAINT `fk_capag_e1_calculo_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Snapshot do cálculo CAPAG-e1 (grade GRE + ajustes + resultado)';
